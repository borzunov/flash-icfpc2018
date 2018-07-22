using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flash.Infrastructure.Simulation;

namespace evaller
{
	class Program
	{
		public class AIResult
		{
			public long energy;
			public byte[] data;
		}

		static void Main(string[] args)
		{


            var comaprator = new SimulationsComaprator(new Simulator(), @"\js\exec-trace-novis_files\exec-trace-node.js");

            foreach (var tgtModelPath in Directory.EnumerateFiles(@"C:\icfpc\flash-icfpc2018\data\models"))
            {
                var prefix = Path.GetFileName(tgtModelPath).Substring(0, 5);
                var tracePath = Directory.EnumerateFiles(@"C:\icfpc\flash-icfpc2018\data\track")
                    .Single(p => Path.GetFileName(p).StartsWith(prefix));
                var compareResult = comaprator.Compare(tgtModelPath, null, tracePath);
                Console.WriteLine($"{prefix} {compareResult}");
            }

		    /*comaprator.Compare(
		        tgtModelPath: "C:\\icfpc\\flash-icfpc2018\\data\\models\\LA001_tgt.mdl",
		        srcModelPath: null,
		        tracePath: "C:\\icfpc\\flash-icfpc2018\\data\\track\\LA001.nbt");
                */

            return;

			var resourcePath = "C:\\Users\\starcev.m\\Desktop\\result";
			var outPath = "C:\\Users\\starcev.m\\Desktop\\result";
			List<Func<string,AIResult>> listOfAI = new List<Func<string, AIResult>>();

			foreach (var file in Directory.EnumerateFiles(outPath))
			{
				var resultsForFile = new List<AIResult>();
				foreach (var ai in listOfAI)
				{
					resultsForFile.Add(ai(file));
				}

				var best = resultsForFile.OrderBy(x => x.energy).First();
				File.WriteAllBytes(
					$"c:\\users\\starcev.m\\desktop\\result\\{Path.GetFileName(file).Substring(0, 5)}.nbt",best.data);
			}

		}
	}
}
