using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Dynamic;
using FluentAssertions;

namespace Jaxosoft.CommandLineParser.UnitTests
{
    public class PositionalParameterTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_01()
        {
            string[] args = "-f filename1 filename2".Split(' ');
            ConfigurationModel config = new ConfigurationModel
            {
                SupportPosititionalParameters = true,
                NumberOfRequiredPositionalParameters = 2
            };

            Parser p = new Parser(config, args);
            var foo = p.AsDynamicArgsBag();
            ((string)foo.FirstPositionalParameter).Should().Be("filename1");
            ((string)foo.LastPositionalParmeter).Should().Be("filename2");
            ((string)foo.PositionalParameter0).Should().Be("filename1");
            ((string)foo.PositionalParameter1).Should().Be("filename2");

            var dictString = p.AsStringStringDictionary();
            dictString.Should().ContainKey("FirstPositionalParameter");
            dictString.Should().ContainKey("LastPositionalParmeter");
            dictString["FirstPositionalParameter"].Should().Be("filename1");
            dictString["LastPositionalParmeter"].Should().Be("filename2");
        }
    }
}
