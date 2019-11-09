using Jaxosoft.CommandLineParser;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Internal;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        public const string DefaultSectionName = "CommandLineParserOptions";

        public static ConfigurationModel GetCommandLineParserOptions(this IConfiguration config) =>
          GetCommandLineParserOptions(config, DefaultSectionName);

        public static ConfigurationModel GetCommandLineParserOptions(this IConfiguration config, string sectionName)
        {
            if (config == null)
                return new ConfigurationModel();

            return config.GetSection(sectionName).Get<ConfigurationModel>();
        }
    }
}
