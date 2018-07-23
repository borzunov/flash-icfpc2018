using System.IO;
using Flash.Infrastructure.AI.Solvers;
using Flash.Infrastructure.Deserializers;
using Flash.Infrastructure.Models;
using Flash.Infrastructure.Serializers;

namespace Flash
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var modelFilePath = @"\\vm-dev-cont1\c$\data\problemsF\FA019_tgt.mdl";
            var mongoOplogWriter = new JsonOpLogWriter(new MongoJsonWriter());
            var jenyaRomaSolver = new JenyaRomaSolver(mongoOplogWriter);
            var tgtMatrix = MatrixDeserializer.Deserialize(File.ReadAllBytes(modelFilePath));

            var trace = jenyaRomaSolver.Solve(srcMatrix: null, tgtMatrix: tgtMatrix);

            var traceBytes = TraceBinarySerializer.Create().Serialize(trace);
            File.WriteAllBytes(@"FA019.nbt", traceBytes);
        }
    }
}
