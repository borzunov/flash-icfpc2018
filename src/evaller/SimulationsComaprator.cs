using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Flash.Infrastructure.AI;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Deserializers;
using Flash.Infrastructure.Models;
using Flash.Infrastructure.Simulation;
using Trace = Flash.Infrastructure.Models.Trace;

namespace evaller
{
    public class SimulationsComaprator
    {
        private readonly Simulator simulator;
        private readonly string ethalonSimPath;

        public SimulationsComaprator(Simulator simulator, string ethalonSimPath)
        {
            this.simulator = simulator;
            this.ethalonSimPath = ethalonSimPath;
        }

        public bool Compare(string tgtModelPath, string srcModelPath, string tracePath)
        {
            var ethalonEnergy = GetEthalonEnergy(tgtModelPath, srcModelPath, tracePath);
            var actualEnergy = GetActualEnergy(tracePath, srcModelPath ?? tgtModelPath);

            Console.WriteLine("EthalonEnergy: " + ethalonEnergy);
            Console.WriteLine("ActualEnergy : " + actualEnergy);

            return ethalonEnergy == actualEnergy;
        }

        private long GetActualEnergy(string tracePath, string anyModelPath)
        {
            var fileAi = new FileAI(tracePath);
            var matrix = MatrixDeserializer.Deserialize(File.ReadAllBytes(anyModelPath));
            var state = State.CreateInitial(matrix.R);
            while (true)
            {
                var commands = fileAi.NextStep(state).ToList();
                simulator.NextStep(state, new Trace(commands));

                if (commands.Count == 1 && commands[0] is HaltCommand)
                    break;
            }

            return state.Energy;
        }

        private long GetEthalonEnergy(string tgtModelPath, string srcModelPath, string tracePath)
        {
            string energy = "aaaa";
            var regex = new Regex(@"Energy:[^\d]*(?<energy>\d+)", RegexOptions.Compiled);
            string arguments = PrepareArguments(tgtModelPath, srcModelPath, tracePath);
            var process = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "node",
                    Arguments = arguments
                }
            };
            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data == null)
                    return;

                var match = regex.Match(args.Data);
                if (match.Success)
                    energy = match.Groups["energy"].Value;
            };
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();

            return long.Parse(energy);
        }

        private string PrepareArguments(string tgtModelPath, string srcModelPath, string tracePath)
        {
            if (string.IsNullOrEmpty(tracePath))
                throw new InvalidOperationException();

            var res = ethalonSimPath;
            if (!string.IsNullOrEmpty(tgtModelPath))
                res += $" --tgtModelPath={tgtModelPath}";

            if (!string.IsNullOrEmpty(srcModelPath))
                res += $" --srcModelPath={srcModelPath}";

            res += $" --tracePath={tracePath}";

            return res;
        }
    }
}
