using System;
using System.Collections.Generic;
using System.Text;

namespace Jaxosoft.CommandLineParser
{
    public interface IParserConfigurator
    {
        Parser Compile(IEnumerable<string> args);

        IParserConfigurator EnableCaseSensitiveArgs();
        IParserConfigurator DisableCaseSensitiveArgs();

        IParserConfigurator EnableAllowDosArgs();
        IParserConfigurator DisableAllowDosArgs();

        IParserConfigurator EnableAllowStandardArgs();
        IParserConfigurator DisableAllowStandardArgs();

        IParserConfigurator EnableAllowExtendedArgs();
        IParserConfigurator DisableAllowExtendedArgs();

        IParserConfigurator EnableAllowSingleLetterConcatArgs();
        IParserConfigurator DisableAllowSingleLetterConcatArgs();

        IParserConfigurator EnableInvariantComparisons();
        IParserConfigurator DisableInvariantComparisons();

        IParserConfigurator EnableUseCurrentCulture();
        IParserConfigurator DisableUseCurrentCulture();

        IParserConfigurator EnableSupportMultilineArguments();
        IParserConfigurator DisableSupportMultilineArguments();

        IParserConfigurator EnableSupportPositionalParameters();
        IParserConfigurator DisableSupportPositionalParameters();

        IParserConfigurator SetNumberOfRequiredPositionalParameters(int number);

        IParserConfigurator AddArgument(CommandLineArgumentDefinition arg);
    }
}
