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

        public FigureDecomposer(Matrix targetMatrix)
        {
            R = targetMatrix.R;
            costPerStep = 3 * R * R * R;

            curMatrix = new Matrix(R);
            this.targetMatrix = targetMatrix;
            curSums = new PointCounter(curMatrix);
            xorSums = new PointCounter(curMatrix ^ targetMatrix);
            targetSums = new PointCounter(targetMatrix);
            rand = new Random(42);
        }

        private const int RegionCount = 20;

        public void Decompose()
        {
            var mongoOplogWriter = new JsonOpLogWriter(new MongoJsonWriter());
            mongoOplogWriter.WriteLogName("FigureDecomposer");
            var modelState = Models.State.CreateInitial(targetMatrix.R, mongoOplogWriter);
            mongoOplogWriter.WriteInitialState(modelState);

            double initialTemp = EvaluateDifferenceFrom(curMatrix) / 10.0;
            for (var i = 0; i < RegionCount; i++)
            {
                if (xorSums.TotalFulls <= 1)
                    break;
                Console.WriteLine($"Points to change: {xorSums.TotalFulls}\n");

                var state = GenerateState();
                var fitness = Evaluate(state);

                for (var j = 1; j <= 10000; j++)
                {
                    double temp = initialTemp / j;
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

                if (state.Region.Dim == 0)
                {
                    Console.WriteLine("Got 1x1x1 region, it will be skipped");
                    continue;
                }

                if (state.Fill)
                    curMatrix.Fill(state.Region);
                else
                    curMatrix.Clear(state.Region);
                curSums = new PointCounter(curMatrix);
                xorSums = new PointCounter(curMatrix ^ targetMatrix);

                var points = new List<Vector>();
                for (var x = state.Region.Min.X; x <= state.Region.Max.X; x++)
                    for (var y = state.Region.Min.Y; y <= state.Region.Max.Y; y++)
                        for (var z = state.Region.Min.Z; z <= state.Region.Max.Z; z++)
                            points.Add(new Vector(x, y, z));
                mongoOplogWriter.WriteGroupColor(points.ToArray(),
                    state.Fill ? "00FF00" : "FF0000", state.Fill ? 0.8 : 0.5);
            }

            for (var i = 0; i < R; i++)
                for (var j = 0; j < R; j++)
                    for (var k = 0; k < R; k++)
                    {
                        var point = new Vector(i, j, k);
                        if (curMatrix.IsVoid(point) && targetMatrix.IsFull(point))
                            mongoOplogWriter.WriteColor(point, "0000FF", 0.8);
                        else
                        if (curMatrix.IsFull(point) && targetMatrix.IsVoid(point))
                            mongoOplogWriter.WriteColor(point, "FFFF00", 0.5);
                    }

            mongoOplogWriter.Save();

            Console.WriteLine($"Points to change: {xorSums.TotalFulls}\n");
            Console.ReadLine();
        }

        double GetTransitionProba(long fitness, long newFitness, double temp)
        {
            if (newFitness < fitness)
                return 1;

            return Math.Exp(-(newFitness - fitness) / temp);
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
