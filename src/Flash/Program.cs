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
using Flash.Infrastructure.Serializers;
using Flash.Infrastructure.Simulation;

namespace Flash
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var trackFilePath = @"..\..\..\data\track\LA001.nbt";
            var modelFilePath = @"..\..\..\data\models\LA080_tgt.mdl";

            var model = MatrixDeserializer.Deserialize(File.ReadAllBytes(modelFilePath));
			//var ai = new GreedyGravityAI(matrix);

			Console.WriteLine("matrix loaded");

			var mongoOplogWriter = new JsonOpLogWriter(new MongoJsonWriter());
            mongoOplogWriter.WriteLogName("GreedyGravityAI_IsGrounded");
	        var state = State.CreateInitial(model.R, mongoOplogWriter);
	        mongoOplogWriter.WriteInitialState(state);

			var groundedChecker = new IsGroundedChecker(state.Matrix);
			
			var figure = new HashSet<Vector>();

	        for (var x = 0; x < model.R; x++)
	        for (var y = 0; y < model.R; y++)
	        for (var z = 0; z < model.R; z++)
	        {
		        var point = new Vector(x, y, z);
		        if (model.IsFull(point))
		        {
			        figure.Add(point);
		        }
	        }

	        var fillWork = new GreedyFiller(state.Matrix, figure, null);
			var path = new PathWork(new Vector(0, 0, 0), fillWork.SetWorkerAndGetInput(groundedChecker, vector => false, new Vector(0, 0, 0), 0), state.Matrix, groundedChecker, 29, 0, model);

	        var works = new[] {(IWork)path, fillWork };

	        var simulator = new Simulator();

	        int i = 0;
	        List<ICommand> commands = null;
	        int commandIdx = 0;

			var traces = new List<Trace>();

			while (true)
	        {
		        if ((commands == null || commandIdx >= commands.Count) && i < works.Length)
		        {
					works[i].DoWork(groundedChecker, vector => false, out commands, out var p);
					if(p.Contains(new Vector(23, 26, 18)))
						Console.WriteLine();
			        i++;
			        commandIdx = 0;
				}

		        if (commands == null || commandIdx >= commands.Count)
		        {
			        var restFigures = figure.Where(f => !state.Matrix.IsFull(f) && state.Bots[0].Pos != f).ToHashSet();
					if (restFigures.Count > 0)
					{
						fillWork = new GreedyFiller(state.Matrix, restFigures, null);
						path = new PathWork(state.Bots[0].Pos, 
							fillWork.SetWorkerAndGetInput(groundedChecker, vector => false, state.Bots[0].Pos, 0), 
							state.Matrix, groundedChecker, 29, 0, model);
						works = new IWork[] {path, fillWork};
						i = 0;
						continue;
					}
					if (state.Bots[0].Pos == new Vector(0, 0, 0))
					{
						commands = new List<ICommand>{new HaltCommand()};
						commandIdx = 0;
					}
					else
					{
						var path1 = new PathWork(state.Bots[0].Pos, new Vector(0, 0, 0), state.Matrix, groundedChecker, 29, 0, model);
						path1.DoWork(groundedChecker, vector => false, out commands, out _);
						commandIdx = 0;
					}
				}

		        mongoOplogWriter.WriteColor(state.Bots[0].Pos, "00FFFF", 0.2);

				if(state.Matrix.IsFull(state.Bots[0].Pos))
					Console.WriteLine();

				simulator.NextStep(state, new Trace(new []{ commands[commandIdx]}));
		        traces.Add(new Trace(new[] { commands[commandIdx] }));


				if (commands[commandIdx] is FillCommand && ((FillCommand)commands[commandIdx]).RealFill != null)
				{
					groundedChecker.UpdateWithFill(((FillCommand)commands[commandIdx]).RealFill);
					//mongoOplogWriter.WriteColor(((FillCommand)commands[commandIdx]).RealFill, "FF00FF", 0.8);
				}

		        if (commands[commandIdx] is VoidCommand && ((VoidCommand)commands[commandIdx]).RealVoid != null)
		        {
			        groundedChecker.UpdateWithFill(((VoidCommand)commands[commandIdx]).RealVoid);
					//mongoOplogWriter.WriteColor(((VoidCommand)commands[commandIdx]).RealVoid, "FF00FF", 0.8);
				}

				commandIdx++;

				if (commands.Count == 1 && commands[0] is HaltCommand)
		        {
			        break;
		        }
	        }
			
			File.WriteAllBytes("atatat.nbt", simulator.CreateResultTrace());

			mongoOplogWriter.Save();
			
        }
    }
}
