using System.IO;
using System.Linq;
using Flash.Infrastructure;
using Flash.Infrastructure.AI.Solvers;
using Flash.Infrastructure.Deserializers;
using Flash.Infrastructure.Models;
using Flash.Infrastructure.Serializers;

namespace Run
{
    class Program
    {
        private static readonly ISolver Solver = new JenyaRomaSolver(new FakeOpLog());

        static void Main(string[] args)
        {
            var srcPath = args.SingleOrDefault(a => a.StartsWith("--src="))?.Substring(6);
            var tgtPath = args.SingleOrDefault(a => a.StartsWith("--tgt="))?.Substring(6);
            var tracePath = args.Single(a => a.StartsWith("--trace=")).Substring(8);
            var tgtMatrix = tgtPath == null ? null : MatrixDeserializer.Deserialize(File.ReadAllBytes(tgtPath));
            var srcMatrix = srcPath == null ? null : MatrixDeserializer.Deserialize(File.ReadAllBytes(srcPath));

            var trace = Solver.Solve(srcMatrix, tgtMatrix);
            if(trace == null)
                return;

            var traceBytes = TraceBinarySerializer.Create().Serialize(trace);
            File.WriteAllBytes(tracePath, traceBytes);
        }
    }
}
