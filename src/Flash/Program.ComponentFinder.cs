using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Flash.Infrastructure;
using Flash.Infrastructure.AI;
using Flash.Infrastructure.Algorithms;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Deserializers;
using Flash.Infrastructure.Models;
using Flash.Infrastructure.Serializers;
using Flash.Infrastructure.Simulation;

namespace Flash
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var src = MatrixDeserializer.Deserialize(File.ReadAllBytes(@"..\..\..\data\problemsF\FR001_src.mdl"));
            var tgt = MatrixDeserializer.Deserialize(File.ReadAllBytes(@"..\..\..\data\problemsF\FR001_tgt.mdl"));

            var mongoOplogWriter = new JsonOpLogWriter(new MongoJsonWriter());
            mongoOplogWriter.WriteLogName("ComponentFinder");
            var state = State.CreateInitial(src.R, mongoOplogWriter);
            mongoOplogWriter.WriteInitialState(state);

            var rand = new Random();
            foreach (var comp in new ComponentFinder(src ^ tgt, tgt).Find())
            {
                if (comp.Fill)
                {
                    var color = new[] {"00FF00", "FF0000", "0000FF", "FFFF00"}[rand.Next(3)];
                    mongoOplogWriter.WriteGroupColor(comp.Points.ToArray(), color, 0.8);
                }
            }

            mongoOplogWriter.Save();
        }
    }
}
