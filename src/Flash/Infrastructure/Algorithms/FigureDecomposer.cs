using Flash.Infrastructure.Models;
using System;
using System.Collections.Generic;

namespace Flash.Infrastructure.Algorithms
{
    public class FigureDecomposer
    {
        private readonly int R;
        private readonly long costPerStep;
        private readonly Matrix curMatrix, targetMatrix;
        private PointCounter curSums, xorSums;
        private readonly PointCounter targetSums;
        private readonly Random rand;

        public FigureDecomposer(Matrix sourceMatrix, Matrix targetMatrix)
        {
            R = targetMatrix.R;
            costPerStep = 3 * R * R * R;

            curMatrix = sourceMatrix.Clone();
            this.targetMatrix = targetMatrix.Clone();
            curSums = new PointCounter(curMatrix);
            xorSums = new PointCounter(curMatrix ^ targetMatrix);
            targetSums = new PointCounter(targetMatrix);
            rand = new Random(42);
        }

        public List<BuildingTask> Decompose()
        {
            Console.WriteLine("FigureDecomposer.Decompose() started");

            double initialTemp = EvaluateDifferenceFrom(curMatrix) / 10.0;
            var tasks = new List<BuildingTask>();
            ConvergenceStopper.Run(regionNo =>
            {
                if (xorSums.TotalFulls <= 2)
                    return null;
                Console.WriteLine($"Points to change: {xorSums.TotalFulls}\n");

                var state = FindNextRectangle(regionNo, initialTemp);
                if ((state.Region.Max - state.Region.Min).Clen <= 2)
                {
                    Console.WriteLine("Too small region, it will be skipped");
                    return xorSums.TotalFulls;
                }

                if (state.Fill)
                    curMatrix.Fill(state.Region);
                else
                    curMatrix.Clear(state.Region);
                curSums = new PointCounter(curMatrix);
                xorSums = new PointCounter(curMatrix ^ targetMatrix);

                var type = state.Fill ? BuildingTaskType.GFill : BuildingTaskType.GVoid;
                tasks.Add(new BuildingTask(type, state.Region));
                return xorSums.TotalFulls;
            }, 0.001, 20);

            Console.WriteLine($"Points to change: {xorSums.TotalFulls}");
            tasks.AddRange(CreatePointwiseTasks());

            Console.WriteLine("FigureDecomposer.Decompose() complete");
            return tasks;
        }

        private State FindNextRectangle(int regionNo, double initialTemp)
        {
            var state = GenerateState();
            var fitness = Evaluate(state);
            ConvergenceStopper.Run(iter =>
            {
                double temp = initialTemp / iter;
                var newState = MutateState(state);
                var newFitness = Evaluate(newState);

                if (iter % 10000 == 0)
                {
                    Console.WriteLine($"Region {regionNo}, iteration {iter}:");
                    Console.WriteLine($"    state.Fill = {state.Fill}, fitness = {fitness}");
                    Console.WriteLine($"    state.Region = {state.Region}");
                }

                var proba = GetTransitionProba(fitness, newFitness, temp);
                if (rand.NextDouble() < proba)
                {
                    state = newState;
                    fitness = newFitness;
                }
                return fitness;
            }, 0.01, 10000);
            return state;
        }

        double GetTransitionProba(long fitness, long newFitness, double temp)
        {
            if (newFitness < fitness)
                return 1;

            return Math.Exp(-(newFitness - fitness) / temp);
        }

        private IEnumerable<BuildingTask> CreatePointwiseTasks()
        {
            var result = new List<BuildingTask>();
            for (var i = 0; i < R; i++)
            for (var j = 0; j < R; j++)
            for (var k = 0; k < R; k++)
            {
                var point = new Vector(i, j, k);
                if (curMatrix.IsVoid(point) && targetMatrix.IsFull(point))
                    result.Add(new BuildingTask(BuildingTaskType.Fill, new Region(point)));
                else
                if (curMatrix.IsFull(point) && targetMatrix.IsVoid(point))
                    result.Add(new BuildingTask(BuildingTaskType.Void, new Region(point)));
            }
            return result;
        }
        
        const int MaxRegionCLen = 30;

        private class State
        {
            public readonly bool Fill;
            public readonly Region Region;

            public State(bool fill, Region region)
            {
                if ((region.Max - region.Min).Clen > MaxRegionCLen - 1)
                    throw new ArgumentException("Region is too big to build in one operation");

                Fill = fill;
                Region = region;
            }
        }

        private State GenerateState()
        {
            var region = new Region(GenerateInitialVector(), GenerateInitialVector());
            return new State(rand.Next(1) == 1, ClipRegion(region));
        }

        private State MutateState(State state)
        {
            var newFill = rand.NextDouble() < 0.1 ? !state.Fill : state.Fill;
            var newRegion = new Region(MutateVector(state.Region.Min), MutateVector(state.Region.Max));
            return new State(newFill, ClipRegion(newRegion));
        }

        private Region ClipRegion(Region region)
        {
            if ((region.Max - region.Min).Clen <= MaxRegionCLen - 1)
                return region;

            var diff = Clipping.Clip(region.Max - region.Min, MaxRegionCLen - 1);
            return rand.NextDouble() < 0.5
                ? new Region(region.Min, region.Min + diff)
                : new Region(region.Max - diff, region.Max);
        }

        private Vector GenerateInitialVector()
        {
            return new Vector(rand.Next(1, R - 1), rand.Next(0, R - 1), rand.Next(1, R - 1));
        }

        public Vector MutateVector(Vector v)
        {
            return new Vector(
                Clipping.Clip(v.X + GenerateMutateDiff(), 1, R - 1),
                Clipping.Clip(v.Y + GenerateMutateDiff(), 0, R - 1),
                Clipping.Clip(v.Z + GenerateMutateDiff(), 1, R - 1));
        }

        private int GenerateMutateDiff()
        {
            var sigma = R / 20.0;
            return (int) Math.Round(rand.NextNormal(0, sigma));
        }

        private long Evaluate(State state)
        {
            var wasFull = curSums.CountFulls(state.Region);
            var wasVoid = state.Region.Volume - wasFull;
            long spentForRect = costPerStep;
            if (state.Fill)
                spentForRect += wasFull * (long) 6;
            else
                spentForRect += wasVoid * (long) 3;
            spentForRect += (state.Region.Max - state.Region.Min).Mlen * costPerStep;

            var diffOutside = xorSums.TotalFulls - xorSums.CountFulls(state.Region);
            var diffInside = targetSums.CountFulls(state.Region);
            if (state.Fill)
                diffInside = state.Region.Volume - diffInside;
            long spentForRem = 2 * (diffInside + diffOutside) * costPerStep;

            return spentForRect + spentForRem;
        }

        private long EvaluateDifferenceFrom(Matrix matrix)
        {
            return 2 * (matrix ^ targetMatrix).CountFulls() * costPerStep;
        }
    }
}
