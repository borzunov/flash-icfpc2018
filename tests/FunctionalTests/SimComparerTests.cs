using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using evaller;
using Flash.Infrastructure.Simulation;
using FluentAssertions;
using NUnit.Framework;

namespace FunctionalTests
{
    public class SimComparerTests
    {
        private SimulationsComaprator comparator;

        [OneTimeSetUp]
        public void Setup()
        {
            var currentAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", string.Empty);
            var projectPath = Directory.GetParent(currentAssemblyPath).Parent.FullName;
            string ethalonSimPath = Path.Combine(projectPath, @"src\evaller\js\exec-trace-novis_files\exec-trace-node.js");
            comparator = new SimulationsComaprator(new Simulator(), ethalonSimPath);
        }

    

        [TestCaseSource(nameof(GetTestCases))]
        public void Run(string tgtModelPath, string srcModelPath, string tracePath)
        {
            var compareResult = comparator.Compare(tgtModelPath, null, tracePath);
            compareResult.Should().BeTrue();
        }

        public static IEnumerable<TestCaseData> GetTestCases()
        {
            var currentAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace(@"file:\", string.Empty);
            var projectPath = Directory.GetParent(currentAssemblyPath).Parent.FullName;
            var trackPath = Path.Combine(projectPath, @"data\track");
            var list = Directory.EnumerateFiles(trackPath).ToList();

            foreach (var tgtModelPath in Directory.EnumerateFiles(Path.Combine(projectPath, @"data\models")))
            {
                var prefix = Path.GetFileName(tgtModelPath).Substring(0, 5);
                Console.WriteLine(prefix);
                var tracePath = list.Single(p => Path.GetFileName(p).StartsWith(prefix));

                yield return new TestCaseData(tgtModelPath, "", tracePath);
            }
        }
    }
}
