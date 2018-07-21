using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
