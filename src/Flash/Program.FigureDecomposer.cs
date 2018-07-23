using System;
using System.Collections.Generic;
using System.IO;
using Flash.Infrastructure.Algorithms;
using Flash.Infrastructure.Deserializers;
using Flash.Infrastructure.Models;

namespace Flash
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var srcModelPath = @"..\..\..\data\problemsF\FR002_src.mdl";
            var tgtModelPath = @"..\..\..\data\problemsF\FR002_tgt.mdl";

            var sourceMatrix = MatrixDeserializer.Deserialize(File.ReadAllBytes(srcModelPath));
            var targetMatrix = MatrixDeserializer.Deserialize(File.ReadAllBytes(tgtModelPath));
//            sourceMatrix = new Matrix(targetMatrix.R);

            var mongoOplogWriter = new JsonOpLogWriter(new MongoJsonWriter());
            mongoOplogWriter.WriteLogName("FigureDecomposer");
            var state = State.CreateInitial(sourceMatrix.R, mongoOplogWriter);
            mongoOplogWriter.WriteInitialState(state);

            var tasks = new FigureDecomposer(targetMatrix, sourceMatrix).Decompose();
            var fillPoints = new List<Vector>();
            var voidPoints = new List<Vector>();
            var rand = new Random();
            foreach (var task in tasks)
            {
                if (task.Type == BuildingTaskType.GFill || task.Type == BuildingTaskType.GVoid)
                {
                    var points = new List<Vector>();
                    for (var x = task.Region.Min.X; x <= task.Region.Max.X; x++)
                        for (var y = task.Region.Min.Y; y <= task.Region.Max.Y; y++)
                            for (var z = task.Region.Min.Z; z <= task.Region.Max.Z; z++)
                                points.Add(new Vector(x, y, z));

                    var fill = task.Type == BuildingTaskType.GFill;
                    mongoOplogWriter.WriteGroupColor(points.ToArray(),
                        fill ? $"00FF{rand.Next(10, 99)}" : $"FF00{rand.Next(10, 99)}", fill ? 0.8 : 0.5);
                } else if (task.Type == BuildingTaskType.Fill || task.Type == BuildingTaskType.Void)
                {
                    if (task.Type == BuildingTaskType.Fill)
                        fillPoints.Add(task.Region.Min);
                    else
                        voidPoints.Add(task.Region.Min);
                }
            }
            mongoOplogWriter.WriteGroupColor(fillPoints.ToArray(), "0000FF", 0.8);
            mongoOplogWriter.WriteGroupColor(voidPoints.ToArray(), "FFFF00", 0.5);

            mongoOplogWriter.Save();

            Console.WriteLine("Saved to MongoDB");
            Console.ReadLine();
        }
    }
}
