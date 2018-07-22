using Flash.Infrastructure.Models;
using System;

namespace Flash.Infrastructure.Algorithms
{
    public class FigureDecomposer
    {
        private readonly int R;
        private readonly Matrix curMatrix, targetMatrix;
        private readonly Random rand;

        public FigureDecomposer(Matrix targetMatrix)
        {
            R = targetMatrix.R;
            curMatrix = new Matrix(R);
            this.targetMatrix = targetMatrix;
            rand = new Random(42);
        }

        const int RegionCount = 10;

        public void Decompose()
        {
            for (var i = 0; i < RegionCount; i++)
            {
                Console.WriteLine($"Points to change: {(curMatrix ^ targetMatrix).CountFulls()}\n");
                var state = GenerateState();
                var fitness = Evaluate(state);

                for (var j = 1; j <= 10000; j++)
                {
                    var temp = InitialTemp / j;
                    var newState = MutateState(state);
                    var newFitness = Evaluate(newState);

                    if (j % 10000 == 0)
                    {
                        Console.WriteLine($"Region {i}, iteration {j}:");
                        Console.WriteLine($"    state.Fill = {state.Fill}, fitness = {fitness}");
                        Console.WriteLine($"    state.Region = {state.Region}");
                        Console.WriteLine($"    newState.Fill = {newState.Fill}, newFitness = {newFitness}");
                        Console.WriteLine($"    newState.Region = {newState.Region}\n");
                    }

                    var proba = GetTransitionProba(fitness, newFitness, temp);
                    if (rand.NextDouble() < proba)
                    {
                        state = newState;
                        fitness = newFitness;
                    }
                }

                if (state.Fill)
                    curMatrix.Fill(state.Region);
                else
                    curMatrix.Clear(state.Region);
            }

            Console.WriteLine($"Points to change: {(curMatrix ^ targetMatrix).CountFulls()}\n");
            Console.ReadLine();
        }

        const double InitialTemp = 1e6;

        double GetTransitionProba(int fitness, int newFitness, double temp)
        {
            if (newFitness < fitness)
                return 1;

            return Math.Exp(-(newFitness - fitness) / temp);
        }

        private class State
        {
            public readonly bool Fill;
            public readonly Region Region;

            public State(bool fill, Region region)
            {
                Fill = fill;
                Region = region;
            }
        }

        private State GenerateState()
        {
            return new State(rand.Next(1) == 1,
                new Region(GenerateInitialVector()));
        }

        private State MutateState(State state)
        {
            var newFill = (rand.NextDouble() < 0.1 ? !state.Fill : state.Fill);
            return new State(newFill,
                new Region(MutateVector(state.Region.Min), MutateVector(state.Region.Max)));
        }

        private Vector GenerateInitialVector()
        {
            return new Vector(rand.Next(1, R - 1), rand.Next(0, R - 1), rand.Next(1, R - 1));
        }

        public Vector MutateVector(Vector v)
        {
            return new Vector(
                Clip(v.X + GenerateMutateDiff(), 1, R - 1),
                Clip(v.Y + GenerateMutateDiff(), 0, R - 1),
                Clip(v.Z + GenerateMutateDiff(), 1, R - 1));
        }

        private static int Clip(int value, int min, int max)
        {
            if (value < min)
                value = min;
            else
            if (value > max)
                value = max;
            return value;
        }

        private int GenerateMutateDiff()
        {
            var sigma = R / 20.0;
            return (int) Math.Round(rand.NextNormal(0, sigma));
        }

        private int Evaluate(State state)
        {
            var R3 = R * R * R;

            var wasFull = curMatrix.CountFulls(state.Region);
            var wasVoid = state.Region.Volume - wasFull;
            var spentForRect = 3 * R3;
            if (state.Fill)
                spentForRect += wasFull * 6;
            else
                spentForRect += wasVoid * 3;

            var nextMatrix = curMatrix.Clone();
            if (state.Fill)
                nextMatrix.Fill(state.Region);
            else
                nextMatrix.Clear(state.Region);
            var spentForRem = (nextMatrix ^ targetMatrix).CountFulls() * 3 * R3;

            return spentForRect + spentForRem;
        }
    }
}
