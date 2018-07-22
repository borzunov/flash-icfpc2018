﻿using System;
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
using Flash.Infrastructure.Simulation;

namespace Flash
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var trackFilePath = @"..\..\..\data\track\LA001.nbt";
            var modelFilePath = @"..\..\..\data\models\LA006_tgt.mdl";

            var matrix = MatrixDeserializer.Deserialize(File.ReadAllBytes(modelFilePath));
            //var ai = new GreedyGravityAI(matrix);
	        
			var mongoOplogWriter = new JsonOpLogWriter(new MongoJsonWriter());
            mongoOplogWriter.WriteLogName("GreedyGravityAI_IsGrounded");
	        var state = State.CreateInitial(matrix.R, mongoOplogWriter);
	        mongoOplogWriter.WriteInitialState(state);

			var groundedChecker = new IsGroundedChecker(matrix);
			var startPosition = new Vector(0, 0, 0);

	        for (int i = 0; i < matrix.R; i++)
	        {
				for (int j = 0; j < matrix.R; j++)
				{
					for (int k = 0; k < matrix.R; k++)
					{
						var vector = new Vector(i, j, k);
						if(matrix.IsVoid(vector))
							continue;
						mongoOplogWriter.WriteColor(vector, "0000FF", 0.5);
					}
				}
			}

			var rand = new Random(13);
			var forbidden = new HashSet<Vector>();
	        for (int i = 0; i < 1000; i++)
	        {
		        var endPosition = new Vector(rand.Next(matrix.R), rand.Next(matrix.R), rand.Next(matrix.R));
				while(forbidden.Contains(endPosition))
					endPosition = new Vector(rand.Next(matrix.R), rand.Next(matrix.R), rand.Next(matrix.R));

				mongoOplogWriter.WriteColor(endPosition, "00FF00", 1);
				var pathBuilder = new BotMoveSearcher(matrix, startPosition, vector => forbidden.Contains(vector), 39, endPosition, groundedChecker);
		        pathBuilder.FindPath(out var movePath, out var commands);

				if (movePath == null)
					break;
		        movePath.ForEach(c => forbidden.Add(c));


				foreach (var vector in movePath)
				{
					mongoOplogWriter.WriteFill(vector);
				}
		        mongoOplogWriter.WriteColor(endPosition, "0000FF", 1);

				startPosition = endPosition;
	        }

	        mongoOplogWriter.Save();

			return;
			/*
            var simulator = new Simulator();

            while (true)
            {
                var commands = ai.NextStep(state).ToList();
                simulator.NextStep(state, new Trace(commands));

                if (commands.Count == 1 && commands[0] is HaltCommand)
                {
                    break;
                }
            }

            mongoOplogWriter.Save();*/
        }
    }
}
