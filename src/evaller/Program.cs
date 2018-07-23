using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flash.Infrastructure;
using Flash.Infrastructure.AI;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;
using Flash.Infrastructure.Simulation;

namespace evaller
{
	class Program
	{
		static void Main(string[] args)
		{
			var inputPath = "input";
			var modelsPath = @"..\..\..\data\models";

			// "LA145" -> xx
			var modelToSize = GetModelToSize(modelsPath);

			// "LA145" -> ..
			var summary = new Dictionary<string, List<(string strategyName, long energy, string tracePath)>>();
			foreach (var directory in Directory.EnumerateDirectories(inputPath))
			{
				Console.WriteLine($"## Start processing {directory}");

				var startegyName = Path.GetDirectoryName(directory);
				foreach (var tracePath in Directory.EnumerateFiles(directory))
				{

					var ai = new FileAI(tracePath);
					var mongoOplogWriter = new FakeOpLog();
					mongoOplogWriter.WriteLogName(startegyName);

					var simulator = new Simulator();
					var modelName = Path.GetFileName(tracePath).Substring(0, 5);
					Console.Write($"# evaluating {modelName}... ");
					var size = modelToSize[modelName];
					var state = State.CreateInitial(size, mongoOplogWriter);
					mongoOplogWriter.WriteInitialState(state);

					while (true)
					{
						var commands = ai.NextStep(state).ToList();
						simulator.NextStep(state, new Trace(commands));

						if (commands[0] is HaltCommand)
						{
							break;
						}
					}

					UpdateSummary(summary, state, startegyName, tracePath, modelName);

					mongoOplogWriter.Save();
				}
			}

			var defaultTracksPath = @"..\..\..\data\track";
			PrepareBestSubmission(summary, defaultTracksPath, modelsPath, "output");
		}

		private static void PrepareBestSubmission(Dictionary<string, List<(string strategyName, long energy, string tracePath)>> summary,
			string defaultTracksPath, string modelsPath, string outputPath)
		{
			if (Directory.Exists(outputPath))
			{
				Directory.Delete(outputPath, true);
			}
			Directory.CreateDirectory(outputPath);

			foreach (var modelPath in Directory.EnumerateFiles(modelsPath))
			{
				var modelName = Path.GetFileName(modelPath).Substring(0, 5);
				var bestTracePath = Path.Combine(defaultTracksPath, $"{modelName}.nbt");
				var src = "default";
				if (summary.ContainsKey(modelName))
				{
					var (strategyName, energy, tracePath) = summary[modelName].OrderByDescending(x => x.energy).First();
					bestTracePath = tracePath;
					src = strategyName;
				}

				Console.WriteLine($"Choose '{src}' strategy for '{modelName}'");
				File.Copy(bestTracePath, Path.Combine(outputPath, Path.GetFileName(bestTracePath)));
			}
		}

		private static void UpdateSummary(Dictionary<string, List<(string strategyName, long energy, string tracePath)>> summary,
			State state, string strategyName, string tracePath, string modelName)
		{
			if (!summary.ContainsKey(modelName))
				summary[modelName] = new List<(string strategyName, long energy, string tracePath)>();

			var list = summary[modelName];
			list.Add((strategyName, state.Energy, tracePath));
			Console.WriteLine($"energy: {state.Energy}");
		}

		private static Dictionary<string, byte> GetModelToSize(string modelsPath)
		{
			var dictionary = Directory.EnumerateFiles(modelsPath)
				.ToDictionary(p => Path.GetFileName(p).Substring(0, 5), p => File.ReadAllBytes(p)[0]);

			return dictionary;
		}
	}
}
