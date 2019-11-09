using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Dynamic;
using FluentAssertions;


namespace Jaxosoft.CommandLineParser.UnitTests
{
    public class CaseSensitiveTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_01()
        {
            string[] args = "--lart".Split(' ');
            ConfigurationModel config = new ConfigurationModel
            {
                AllowDosArgs = true,
                AllowExtendedArgs = true,
                AllowSingleLetterConcatArgs = true,
                AllowStandardArgs = true,
                AreArgsCaseSensitive = true,
                InvariantComparisons = false,
                SupportMultilineArguments = true,
                Arguments = new List<CommandLineArgumentDefinition>
                {
                    { new CommandLineArgumentDefinition { IsRequired = false, IsSingleLetter = true, Name = "a", RequiresValue = false, Aliases = new List<string> { "all" }, Usage = "-a" } },
                    { new CommandLineArgumentDefinition { IsRequired = false, IsSingleLetter = true, Name = "l", RequiresValue = false, Aliases = new List<string> { "list" }, Usage = "-l" } },
                    { new CommandLineArgumentDefinition { IsRequired = false, IsSingleLetter = true, Name = "r", RequiresValue = false, Aliases = new List<string> { "recursive" }, Usage = "-r" } },
                    { new CommandLineArgumentDefinition { IsRequired = false, IsSingleLetter = true, Name = "t", RequiresValue = false, Aliases = new List<string> { "time" }, Usage = "-t" } }
                }
            };

            Parser p = new Parser(config, args);

            p.Should().NotBeNull();
            var commandBag = p.AsDynamicArgsBag();
            var all = ((bool?)commandBag.all)?.Should().BeTrue();
            ((bool)commandBag.l).Should().BeTrue();

            var expando = (ExpandoObject)commandBag;
            expando.Should().HaveCount(9);
            expando.Should().ContainKey("l");
            expando.Should().ContainKey("t");
            expando.Should().NotContainKey("y");
            expando.Should().NotContainKey("L");
            expando.Should().ContainKey("lart");
            expando.Should().ContainKey("time");

            Assert.Pass();
        }
    }
}