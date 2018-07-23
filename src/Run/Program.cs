using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run
{
    class Program
    {
        static void Main(string[] args)
        {
            var tgt = args.Single(a => a.StartsWith("--tgt=")).Substring(6);
            var trace = args.Single(a => a.StartsWith("--trace=")).Substring(8);

            File.WriteAllText(trace, "aaaa");
        }
    }
}
