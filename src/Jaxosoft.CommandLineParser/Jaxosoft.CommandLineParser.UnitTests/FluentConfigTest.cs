using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Dynamic;
using FluentAssertions;

namespace Jaxosoft.CommandLineParser.UnitTests
{
    public class FluentConfigTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_01()
        {
            var p = Parser
                .Create()
                .DisableAllowDosArgs()
                .EnableAllowDosArgs()
                .EnableAllowExtendedArgs()
                .Compile(new string[2])
                .AsDynamicArgsBag();
        }
    }
}