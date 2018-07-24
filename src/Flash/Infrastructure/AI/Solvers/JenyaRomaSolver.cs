using System;
using System.Linq;
using Flash.Infrastructure.Algorithms;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;
using Flash.Infrastructure.Simulation;

namespace Flash.Infrastructure.AI.Solvers
{
    public class JenyaRomaSolver: ISolver
    {
        private readonly IOpLogWriter opLogWriter;

        public JenyaRomaSolver(IOpLogWriter opLogWriter)
        {
            this.opLogWriter = opLogWriter;
        }

        public Trace Solve(Matrix srcMatrix, Matrix tgtMatrix)
        {
            if (srcMatrix == null && tgtMatrix != null)
            {
                return Assembly(tgtMatrix);
            }

            //disasm && reasm
            return null;
        }

        private Trace Assembly(Matrix matrix)
        {
            var tasks = new FigureDecomposer(matrix).Decompose();

            var ai = new GreedyWithFigureDecomposeAI(tasks, new IsGroundedChecker(matrix));

            Console.WriteLine("test greedy");

            //
            opLogWriter.WriteLogName("myTest");
            var state = State.CreateInitial(matrix.R, opLogWriter);
            opLogWriter.WriteInitialState(state);

            var simulator = new Simulator();

            while (true)
            {
                var commands = ai.NextStep(state).ToList();
                var trace = new Trace(commands);

                simulator.NextStep(state, trace);

                if (commands.Count == 1 && commands[0] is HaltCommand)
                {
                    break;
                }
            }

            opLogWriter.Save();

            return simulator.CreateResultTraceAsTrace();
        }
    }
}
