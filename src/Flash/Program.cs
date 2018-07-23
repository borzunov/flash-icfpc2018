using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Flash.Infrastructure;
using Flash.Infrastructure.AI;
using Flash.Infrastructure.AI.Solvers;
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
			//var trackFilePath = @"..\..\..\data\track\LA001.nbt";
			var model1FilePath = @"..\..\..\data\models\LA001_tgt.mdl";
	        var model2FilePath = @"..\..\..\data\models\LA002_tgt.mdl";

	        var model1 = MatrixDeserializer.Deserialize(File.ReadAllBytes(model1FilePath));
	        var model2 = MatrixDeserializer.Deserialize(File.ReadAllBytes(model2FilePath));
			//var ai = new GreedyGravityAI(matrix);


			File.WriteAllBytes(@"C:\Projects\icfpc2018\flash-icfpc2018\data\atatat.nbt", TraceBinarySerializer.Create().Serialize(new MishaSolverv1().Solve(model1, model2)));
        }
    }
}
