using System;
using System.Collections.Generic;
using System.Linq;

namespace Jaxosoft.CommandLineParser
{
    public class ConfigurationModel : IParserConfigurator
    {
        public bool AreArgsCaseSensitive { get; set; } = true;
        public bool AllowDosArgs { get; set; } = true;
        public bool AllowStandardArgs { get; set; } = true;
        public bool AllowExtendedArgs { get; set; } = true;
        public bool AllowSingleLetterConcatArgs { get; set; } = false;
        public bool InvariantComparisons { get; set; } = false;
        public bool UseCurrentCulture { get; set; } = false;
        public bool SupportMultilineArguments { get; set; } = false;
        public bool SupportPosititionalParameters { get; set; } = false;
        public int NumberOfRequiredPositionalParameters { get; set; } = 0;

        public List<CommandLineArgumentDefinition> Arguments { get; set; } = new List<CommandLineArgumentDefinition>();

        #region IParserConfigurator 
        public IParserConfigurator AddArgument(CommandLineArgumentDefinition arg)
        {
            this.Arguments.Add(arg);
            return this;
        }

        public Parser Compile(IEnumerable<string> args)
        {
            return new Parser(this, args.ToArray());
        }

        public IParserConfigurator DisableAllowDosArgs()
        {
            this.AllowDosArgs = false;
            return this;
        }

        public IParserConfigurator DisableAllowExtendedArgs()
        {
            this.AllowExtendedArgs = false;
            return this;
        }

        public IParserConfigurator DisableAllowSingleLetterConcatArgs()
        {
            this.AllowSingleLetterConcatArgs = false;
            return this;
        }

        public IParserConfigurator DisableAllowStandardArgs()
        {
            this.AllowStandardArgs = false;
            return this;
        }

        public IParserConfigurator DisableCaseSensitiveArgs()
        {
            this.AreArgsCaseSensitive = false;
            return this;
        }

        public IParserConfigurator DisableInvariantComparisons()
        {
            this.InvariantComparisons = false;
            return this;
        }

        public IParserConfigurator DisableSupportMultilineArguments()
        {
            this.SupportMultilineArguments = false;
            return this;
        }

        public IParserConfigurator DisableSupportPositionalParameters()
        {
            this.SupportPosititionalParameters = false;
            return this;
        }

        public IParserConfigurator DisableUseCurrentCulture()
        {
            this.UseCurrentCulture = false;
            return this;
        }

        public IParserConfigurator EnableAllowDosArgs()
        {
            this.AllowDosArgs = true;
            return this;
        }

        public IParserConfigurator EnableAllowExtendedArgs()
        {
            this.AllowExtendedArgs = true;
            return this;
        }

        public IParserConfigurator EnableAllowSingleLetterConcatArgs()
        {
            this.AllowSingleLetterConcatArgs = true;
            return this;
        }

        public IParserConfigurator EnableAllowStandardArgs()
        {
            this.AllowStandardArgs = true;
            return this;
        }

        public IParserConfigurator EnableCaseSensitiveArgs()
        {
            this.AreArgsCaseSensitive = true;
            return this;
        }

        public IParserConfigurator EnableInvariantComparisons()
        {
            this.InvariantComparisons = true;
            return this;
        }

        public IParserConfigurator EnableSupportMultilineArguments()
        {
            this.SupportMultilineArguments = true;
            return this;
        }

        public IParserConfigurator EnableSupportPositionalParameters()
        {
            this.SupportPosititionalParameters = true;
            return this;
        }

        public IParserConfigurator EnableUseCurrentCulture()
        {
            this.UseCurrentCulture = true;
            return this;
        }

        public IParserConfigurator SetNumberOfRequiredPositionalParameters(int number)
        {
            this.NumberOfRequiredPositionalParameters = number;
            return this;
        }
        #endregion
    }

    public class CommandLineArgumentDefinition
    {
        public string Name { get; set; }
        public List<string> Aliases { get; set; } = new List<string>();
        public bool IsRequired { get; set; } = false;
        public bool RequiresValue { get; set; } = false;
        public bool IsSingleLetter { get; set; } = false;

        public string? Usage { get; set; }
    }
}
