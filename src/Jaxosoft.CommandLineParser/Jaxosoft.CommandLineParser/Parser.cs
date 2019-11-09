using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace Jaxosoft.CommandLineParser
{
    public class Parser
    {
        private const string _letterCharacter = @"\p{Lu}|\p{Ll}|\p{Lt}|\p{Lo}|\p{Nl}";
        private const string _otherCharacter = @"\p{Mn}|\p{Mc}|\p{Nd}|\p{Pc}|\p{Cf}";
        private const int _defaultCollectionSize = 8;

        private Regex _pullArgNameRegex;
        private Regex _dynamicNameRegex = new Regex(@"^\W*(?<firstWord>(?<firstLetter>[A-Za-z_])(?<afterFirstLetter>\w*))|(?<otherWord>(?<otherWordFirstLetter>\w)(?<otherWordAfterFirstLetter>\w*))", RegexOptions.Compiled);
        private Regex _dynamicIdentifier = new Regex(@"(^((@_[^_])|(@_)|(@)|(_)|("+_letterCharacter+@")))|" + $"{_letterCharacter}|{_otherCharacter}", RegexOptions.Compiled );
        private Dictionary<string, string>? _aliases;
        private Dictionary<string, CommandLineArgumentInstance>? _instances;
        private dynamic? _instanceBag;
        private Dictionary<string, string?>? _instanceStringStringDictionary;
        private Dictionary<string, object?>? _instanceStringObjectDictionary;
        private List<string>? _additionalRawArguments;
        private CultureInfo? _culture;
        private StringComparer _stringComparer;

        public Parser()
        {
        }

        public Parser(ConfigurationModel config)
        {
            Config = config;
        }

        public Parser(string[] args)
        {
            ParseCommandLine(args);
        }

        public Parser(ConfigurationModel config, string[] args)
        {
            Config = config;
            ParseCommandLine(args);
        }

        public ConfigurationModel Config { get; set; } = new ConfigurationModel();

        public CultureInfo Culture 
        {
            get
            {
                if(_culture == null)
                {
                    if (Config.InvariantComparisons)
                    {
                        _culture = CultureInfo.InvariantCulture;
                    }
                    else if (Config.UseCurrentCulture)
                    {
                        _culture = CultureInfo.CurrentCulture;
                    }
                    else
                    {
                        try
                        {
                            _culture = new CultureInfo("en-US");
                        }
                        catch
                        {
                            _culture = CultureInfo.CurrentCulture;
                        }
                    }
                }
                return _culture;
            }
            private set
            {
                _culture = value;
            }
        }

        public StringComparer Comparer
        {
            get
            {
                if(_stringComparer == null)
                {
                    if (Config.AreArgsCaseSensitive)
                    {
                        if (Config.InvariantComparisons)
                            _stringComparer = Culture.CompareInfo.GetStringComparer(CompareOptions.Ordinal);
                        else
                            _stringComparer = Culture.CompareInfo.GetStringComparer(CompareOptions.None);
                    }
                    else
                    {
                        if (Config.InvariantComparisons)
                            _stringComparer = Culture.CompareInfo.GetStringComparer(CompareOptions.OrdinalIgnoreCase);
                        else
                            _stringComparer = Culture.CompareInfo.GetStringComparer(CompareOptions.IgnoreCase);
                    }
                }

                return _stringComparer;
            }

            private set
            {
                _stringComparer = value;
            }
        }

        public void ParseCommandLine(string[] args)
        {
            FillDefaultStructuresFromConfig();

            if (_aliases == null)
                _aliases = new Dictionary<string, string>(_defaultCollectionSize, Comparer);
            if (_instances == null)
                _instances = new Dictionary<string, CommandLineArgumentInstance>(_defaultCollectionSize, Comparer);
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            for (int i = 0; i < args.Length; i++)
            {
                string currentArg = args[i];
                GroupCollection groups = _pullArgNameRegex.Match(currentArg).Groups;

                if (groups["argType"].Success && groups["argType"].Value.Trim() == "--")
                {
                    _additionalRawArguments = new List<string>(args.Length - i);
                    if (i == args.Length - 1)
                    {
                        _additionalRawArguments = new List<string>();
                    }
                    else
                    {
                        for (int j = i + 1; j < args.Length; j++)
                        {
                            _additionalRawArguments.Add(args[j]);
                        }
                        break;
                    }
                }

                if (groups["argName"].Success)
                {
                    List<string> argNames = new List<string>();
                    if(groups.Any(x => x.Name == "singleLetterArg"))
                    {
                        foreach(Capture c in groups["singleLetterArg"].Captures)
                        {
                            argNames.Add(c.Value.Trim());
                        }
                    }
                        
                    argNames.Add(groups["argName"].Value.Trim());

                    foreach(var argName in argNames)
                    {
                        CommandLineArgumentInstance tempInstance = new CommandLineArgumentInstance(argName);

                        if (groups["argValue"].Success)
                        {
                            string valOrVals = groups["argValue"].Value.Trim();
                            if (valOrVals.Contains(',', StringComparison.Ordinal))
                            {
                                tempInstance.Values.AddRange(valOrVals.Split(','));
                            }
                            else
                            {
                                tempInstance.BoolValue = CalculateBooleanValue(valOrVals);
                                tempInstance.Values.Add(valOrVals);
                            }
                        }

                        while (i < args.Length - 1 && !IsArg(args[i + 1]))
                        {
                            string nextArg = args[i + 1].Trim();
                            if (nextArg.Contains(',', StringComparison.Ordinal))
                                tempInstance.Values.AddRange(nextArg.Split(','));
                            else
                            {
                                tempInstance.BoolValue = CalculateBooleanValue(nextArg);
                                tempInstance.Values.Add(nextArg);
                            }
                            tempInstance.Values.Add(nextArg);
                            i++;
                        }

                        if (_aliases.TryGetValue(argName, out string cName))
                        {
                            if (_instances.TryGetValue(cName, out CommandLineArgumentInstance instance))
                            {
                                instance.Exists = true;
                                instance.BoolValue = tempInstance.BoolValue;
                                instance.DataType = tempInstance.DataType;
                                instance.Values.AddRange(tempInstance.Values);
                            }
                        }
                        else if (_instances.TryGetValue(argName, out CommandLineArgumentInstance instance))
                        {
                            instance.Exists = true;
                            instance.BoolValue = tempInstance.BoolValue;
                            instance.DataType = tempInstance.DataType;
                            instance.Values.AddRange(tempInstance.Values);
                        }
                        else
                        {
                            _aliases.Add(argName, argName);
                            tempInstance.Name = argName;

                            CommandLineArgumentInstance newInstance = new CommandLineArgumentInstance(argName);
                            newInstance.Exists = true;
                            newInstance.BoolValue = tempInstance.BoolValue;
                            newInstance.DataType = tempInstance.DataType;
                            if (tempInstance.Values.Count > 0)
                                newInstance.Values.AddRange(tempInstance.Values);

                            _instances.Add(argName, newInstance);
                        }
                    }
                }
            }
        }

        public dynamic AsDynamicArgsBag()
        {
            if (_instanceBag != null)
                return _instanceBag;

            FillDefaultStructuresFromConfig();

            if (_aliases == null)
                _aliases = new Dictionary<string, string>(_defaultCollectionSize, Comparer);
            if (_instances == null)
                _instances = new Dictionary<string, CommandLineArgumentInstance>(_defaultCollectionSize, Comparer);

            ExpandoObject dynamo = new ExpandoObject();
            
            var query = (from x in _instances
                            join y in _aliases on x.Key equals y.Value
                            select new { Name = x.Value.Name, Alias = y.Key, Value = x.Value })
                            .AsEnumerable();
            foreach(var item in query.GroupBy(x => x.Name))
            {
                string dynamicMemberName = ToDynamicMemberName(item.Key);
                string dynamicMemberIdentifier = ToDynamicMemberIdentifier(item.Key);

                dynamo.TryAdd(dynamicMemberName, item.First().Value.ComputedValue);
                if(dynamicMemberName != dynamicMemberIdentifier)
                    dynamo.TryAdd(dynamicMemberIdentifier, item.First().Value.ComputedValue);
                foreach (var alias in item)
                {
                    string aliasDynamicMemberName = ToDynamicMemberName(alias.Alias);
                    string aliasDynamicMemberIdentifier = ToDynamicMemberIdentifier(alias.Alias);
                    if (aliasDynamicMemberName != dynamicMemberName && aliasDynamicMemberName != dynamicMemberIdentifier)
                        dynamo.TryAdd(aliasDynamicMemberName, alias.Value.ComputedValue);
                    if(aliasDynamicMemberIdentifier != dynamicMemberIdentifier && aliasDynamicMemberIdentifier != dynamicMemberName && aliasDynamicMemberIdentifier != aliasDynamicMemberName)
                        dynamo.TryAdd(aliasDynamicMemberIdentifier, alias.Value.ComputedValue);
                }
            }

            _instanceBag = dynamo;
            return _instanceBag;
        }

        public dynamic AsDynamicArgsBag(string[] args)
        {
            ParseCommandLine(args);
            return AsDynamicArgsBag();
        }

        public Dictionary<string, object?> AsStringObjectDictionary()
        {
            if (_instanceStringObjectDictionary != null)
                return _instanceStringObjectDictionary;

            FillDefaultStructuresFromConfig();

            if (_aliases == null)
                _aliases = new Dictionary<string, string>(_defaultCollectionSize, Comparer);
            if (_instances == null)
                _instances = new Dictionary<string, CommandLineArgumentInstance>(_defaultCollectionSize, Comparer);

            Dictionary<string, object?> returnData = new Dictionary<string, object?>(_defaultCollectionSize, Comparer);

            var query = (from x in _instances
                         join y in _aliases on x.Key equals y.Value
                         select new { Name = x.Value.Name, Alias = y.Key, Value = x.Value })
                             .AsEnumerable();
            foreach (var item in query.GroupBy(x => x.Name))
            {
                returnData.TryAdd(item.Key, item.First().Value.ComputedValue);
                foreach (var alias in item)
                {
                    if (alias.Alias != item.Key)
                        returnData.TryAdd(alias.Alias, alias.Value.ComputedValue);
                }
            }

            _instanceStringObjectDictionary = returnData;
            return _instanceStringObjectDictionary;
        }

        public Dictionary<string, string?> AsStringStringDictionary()
        {
            if (_instanceStringStringDictionary != null)
                return _instanceStringStringDictionary;

            FillDefaultStructuresFromConfig();

            if (_aliases == null)
                _aliases = new Dictionary<string, string>(_defaultCollectionSize, Comparer);
            if (_instances == null)
                _instances = new Dictionary<string, CommandLineArgumentInstance>(_defaultCollectionSize, Comparer);

            Dictionary<string, string?> returnData = new Dictionary<string, string?>(_defaultCollectionSize, Comparer);

            var query = (from x in _instances
                         join y in _aliases on x.Key equals y.Value
                         select new { Name = x.Value.Name, Alias = y.Key, Value = x.Value })
                             .AsEnumerable();
            foreach (var item in query.GroupBy(x => x.Name))
            {
                returnData.TryAdd(item.Key, item.First().Value.ComputedStringValue);
                foreach (var alias in item)
                {
                    if (alias.Alias != item.Key)
                        returnData.TryAdd(alias.Alias, alias.Value.ComputedStringValue);
                }
            }

            _instanceStringStringDictionary = returnData;
            return _instanceStringStringDictionary;
        }

        public static Parser CreateAndParse(string[] args)
        {
            Parser p = new Parser();
            p.ParseCommandLine(args);
            return p;
        }

        private void FillDefaultStructuresFromConfig()
        {
            if (Config.InvariantComparisons)
                Culture = CultureInfo.InvariantCulture;

            if (_aliases != null && _instances != null)
                return;

            _aliases = new Dictionary<string, string>(_defaultCollectionSize, Comparer);
            _instances = new Dictionary<string, CommandLineArgumentInstance>(_defaultCollectionSize, Comparer);

            string argTypes = @"\/|-{1,2}";
            List<string> allowArgsTypes = new List<string>(2);
            if (Config.AllowDosArgs)
                allowArgsTypes.Add("\\/");

            if (Config.AllowStandardArgs && Config.AllowExtendedArgs)
            {
                allowArgsTypes.Add("-{1,2}");
            }
            else
            {
                if (Config.AllowStandardArgs)
                    allowArgsTypes.Add("-");
                if (Config.AllowExtendedArgs)
                    allowArgsTypes.Add("--");
            }

            if (allowArgsTypes.Count > 0)
                argTypes = String.Join("|", allowArgsTypes);

            string argNames;
            if (Config.AllowSingleLetterConcatArgs)
                argNames = @"(?<singleLetterArg>[^=])+";
            else
                argNames = @"[^=]+";

            _pullArgNameRegex =
                    new Regex(@"^(?<argType>" + argTypes + ")(?<argName>" + argNames + @")(?<argEquals>=(?<argValue>.*))?$", RegexOptions.Compiled);

            foreach (var clp in Config.Arguments)
            {
                string name = clp.Name.Trim();
                foreach (var a in clp.Aliases)
                    _aliases.TryAdd(a, name);
                _instances.TryAdd(name, new CommandLineArgumentInstance(name));
            }
        }

        private bool IsArg(string arg)
        {
            return arg.StartsWith('/') || arg.StartsWith('-');
        }

        private bool? CalculateBooleanValue(string? argument)
        {
            if (argument == null)
                return null;
            
            string lowerVal = argument.ToLower(Culture);

            switch (lowerVal)
            {
                case "y":
                case "yes":
                case "t":
                case "true":
                    return true;

                case "n":
                case "no":
                case "f":
                case "false":
                    return false;

                default:
                    return null;
            }
        }

        // to PASCAL case
        private string ToDynamicMemberName(string s)
        {
            if (String.IsNullOrWhiteSpace(s))
                return s;

            string returnValue = String.Empty;

            MatchCollection mc = _dynamicNameRegex.Matches(s);
            foreach(Match m in mc)
            {
                if (m.Success)
                {
                    Group g1a = m.Groups["firstLetter"];
                    Group g1b = m.Groups["afterFirstLetter"];
                    Group g2a = m.Groups["otherWordFirstLetter"];
                    Group g2b = m.Groups["otherWordAfterFirstLetter"];

                    if (g1a.Success)
                        returnValue += Config.AreArgsCaseSensitive ? g1a.Value : g1a.Value.ToUpper(Culture);
                    if (g1b.Success)
                        returnValue += Config.AreArgsCaseSensitive ? g1b.Value : g1b.Value.ToLower(Culture);
                    if (g2a.Success)
                        returnValue += Config.AreArgsCaseSensitive ? g2a.Value : g2a.Value.ToUpper(Culture);
                    if (g2b.Success)
                        returnValue += Config.AreArgsCaseSensitive ? g2b.Value : g2b.Value.ToLower(Culture);
                }
            }
            return returnValue;
        }

        // to a valid C# identifier
        private string ToDynamicMemberIdentifier(string s)
        {
            if (String.IsNullOrWhiteSpace(s))
                return s;

            string returnValue = String.Empty;

            MatchCollection mc = _dynamicIdentifier.Matches(s);
            foreach (Match m in mc)
            {
                if(m.Success)
                    returnValue += m.Value;
            }
            return returnValue;
        }
    }
}
