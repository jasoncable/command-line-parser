using System;
using System.Collections.Generic;

namespace Jaxosoft.CommandLineParser
{
    public class ConfigurationModel
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
