using System;
using System.Collections.Generic;
using System.IO;
using Flash.Infrastructure.Deserializers;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure
{
    public class ModelsVisualizer
    {
        public static void Visualize(string modelPath, string name = null)
        {
            var resultMatrix = MatrixDeserializer.Deserialize(File.ReadAllBytes(modelPath));
            name = name ?? Path.GetFileNameWithoutExtension(modelPath);

            Visualize(resultMatrix, name);
        }

        public static void Visualize(Matrix matrix, string name)
        {
            var oplogWriter = new JsonOpLogWriter(new MongoJsonWriter("models"));
            oplogWriter.WriteLogName(name);
            oplogWriter.WriteResolution(matrix.R);

            var points = new List<Vector>();

            for (var x = 0; x < matrix.R; x++)
            {
                for (var y = 0; y < matrix.R; y++)
                {
                    for (var z = 0; z < matrix.R; z++)
                    {
                        var vector = new Vector(x, y, z);
                        if (matrix.IsFull(vector))
                        {
                            points.Add(vector);
                        }
                    }
                }
            }

            oplogWriter.WriteGroupAdd(points.ToArray());

            oplogWriter.Save();
        }
    }
}
