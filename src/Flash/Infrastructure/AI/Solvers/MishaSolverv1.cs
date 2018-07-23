using System;
using System.Collections.Generic;
using System.Linq;
using Flash.Infrastructure.Algorithms;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;
using Flash.Infrastructure.Simulation;

namespace Flash.Infrastructure.AI.Solvers
{
	public class MishaSolverv1 : ISolver
	{
		public Trace Solve(Matrix srcMatrix, Matrix tgtMatrix)
		{
            if (srcMatrix == null && tgtMatrix != null)
            {
                return Assembly(tgtMatrix);
            }

            if (srcMatrix != null && tgtMatrix == null)
            {
                return Disassembly(srcMatrix);
            }

            return Reassembly(srcMatrix, tgtMatrix);
        }

        private Trace Disassembly(Matrix model)
        {
			var mongoOplogWriter = new FakeOpLog();
            mongoOplogWriter.WriteLogName("GreedyGravityAI_IsGrounded");
	        var state = State.CreateInitial(model.R, mongoOplogWriter);
	        var matrixToDo = state.Matrix;
	        state.Matrix = model;

			mongoOplogWriter.WriteInitialState(state);

			var groundedChecker = new IsGroundedChecker(model);
			
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
			
			var clearWork = new GreedyClearer(state.Matrix, figure, null);
			var path = new PathWork(new Vector(0, 0, 0), 
				clearWork.SetWorkerAndGetInput(groundedChecker, vector => false, new Vector(0, 0, 0), 0), state.Matrix, groundedChecker, 29, 0, matrixToDo);

	        var works = new[] {(IWork)path, clearWork };

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
			        i++;
			        commandIdx = 0;
					if(p.Contains(new Vector(12, 0, 12)))
						Console.WriteLine();
				}

		        if (commands == null || commandIdx >= commands.Count)
		        {
			        var restFigures = figure.Where(f => !state.Matrix.IsVoid(f)).ToHashSet();
					if (restFigures.Count > 0)
					{
						clearWork = new GreedyClearer(state.Matrix, restFigures, null);
						path = new PathWork(state.Bots[0].Pos, 
							clearWork.SetWorkerAndGetInput(groundedChecker, vector => false, state.Bots[0].Pos, 0), 
							state.Matrix, groundedChecker, 29, 0, matrixToDo);
						works = new IWork[] {path, clearWork};
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
			        groundedChecker.UpdateWithClear(((VoidCommand)commands[commandIdx]).RealVoid);
					//mongoOplogWriter.WriteColor(((VoidCommand)commands[commandIdx]).RealVoid, "FF00FF", 0.8);
				}

				commandIdx++;

				if (commands.Count == 1 && commands[0] is HaltCommand)
		        {
			        break;
		        }
	        }

            return simulator.CreateResultTraceAsTrace();
        }

        private Trace Assembly(Matrix model)
        {
            //asm
		    var state = State.CreateInitial(model.R, new FakeOpLog());

			Console.WriteLine("matrix loaded");

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

			var works = new[] { (IWork)path, fillWork };

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
					if (p.Contains(new Vector(23, 26, 18)))
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
						works = new IWork[] { path, fillWork };
						i = 0;
						continue;
					}
					if (state.Bots[0].Pos == new Vector(0, 0, 0))
					{
						commands = new List<ICommand> { new HaltCommand() };
						commandIdx = 0;
					}
					else
					{
						var path1 = new PathWork(state.Bots[0].Pos, new Vector(0, 0, 0), state.Matrix, groundedChecker, 29, 0, model);
						path1.DoWork(groundedChecker, vector => false, out commands, out _);
						commandIdx = 0;
					}
				}
				
				if (state.Matrix.IsFull(state.Bots[0].Pos))
					Console.WriteLine();

				simulator.NextStep(state, new Trace(new[] { commands[commandIdx] }));
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

			return simulator.CreateResultTraceAsTrace();
        }

        private Trace Reassembly(Matrix srcMatrix, Matrix tgtMatrix)
        {
            return null;
        }
    }
}
