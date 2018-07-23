﻿using Flash.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Flash.Infrastructure.Algorithms
{
    public class FigureDecomposer
    {
        private readonly int R;
        private readonly long costPerStep;
        private readonly Matrix curMatrix, targetMatrix, xorMatrix;
        private readonly PointCounter curSums, targetSums, xorSums;
        private IsGroundedChecker curGroundChecker;
        private readonly Random rand;

        public FigureDecomposer(Matrix targetMatrix, Matrix startMatrix = null)
        {
            R = targetMatrix.R;
            costPerStep = 3 * R * R * R;

            curMatrix = startMatrix?.Clone() ?? new Matrix(R);
            this.targetMatrix = targetMatrix.Clone();
            xorMatrix = curMatrix ^ targetMatrix;

            curSums = new PointCounter(curMatrix);
            targetSums = new PointCounter(targetMatrix);
            xorSums = new PointCounter(xorMatrix);

            curGroundChecker = new IsGroundedChecker(curMatrix);

            rand = new Random(42);
        }

        public List<BuildingTask> Decompose()
        {
            Console.WriteLine("FigureDecomposer.Decompose() started");

            double initialTemp = EvaluateDifferenceFrom(curMatrix) / 10.0;
            var tasks = new List<BuildingTask>();
            ConvergenceStopper.Run(regionNo =>
            {
                var stopwatch = Stopwatch.StartNew();

                if (xorSums.TotalFulls <= 3)
                    return null;
                Console.WriteLine($"Points to change: {xorSums.TotalFulls}\n");

                var state = FindNextRectangle(regionNo, initialTemp);
                if ((state.Region.Max - state.Region.Min).Clen <= 3)
                {
                    Console.WriteLine("Too small region, it will be skipped");
                    return xorSums.TotalFulls;
                }

                Region leg = null;
                if (state.Fill && BreakesGrounding(state) && state.Region.Min.Y > 0)
                {
                    var direction = state.Region.Max - state.Region.Min;
                    if (direction.X < 3 || direction.Z < 3)
                    {
                        Console.WriteLine("We don't build regions like vertical lines if they are not grounded");
                        return xorSums.TotalFulls;
                    }
                    
                    var legX = state.Region.Min.X + 1;
                    var legZ = state.Region.Min.Z + 1;
                    leg = new Region(
                        new Vector(legX, 0, legZ),
                        new Vector(legX, state.Region.Min.Y - 1, legZ));
                    Console.WriteLine($"Building leg {leg} for region {state.Region}");

                    curMatrix.Fill(leg);
                    UpdateXorMatrix(leg);
                    if (leg.Volume >= 4)
                        tasks.Add(new BuildingTask(BuildingTaskType.GFill, leg));
                    else
                        tasks.AddRange(leg.AllPoints()
                            .Select(p => new BuildingTask(BuildingTaskType.Fill, new Region(p))));
                }
                if (!state.Fill && BreakesGrounding(state))
                {
                    Console.WriteLine("Can't fix ungrounded GVoids");
                    return xorSums.TotalFulls;
                }

                if (state.Fill)
                    curMatrix.Fill(state.Region);
                else
                    curMatrix.Clear(state.Region);
                UpdateXorMatrix(state.Region);
                var type = state.Fill ? BuildingTaskType.GFill : BuildingTaskType.GVoid;
                tasks.Add(new BuildingTask(type, state.Region));

                var minChanged = leg == null ? state.Region.Min : new Vector(0, 0, 0);
                curSums.Update(curMatrix, minChanged);
                xorSums.Update(xorMatrix, minChanged);
                curGroundChecker = new IsGroundedChecker(curMatrix);

                Console.WriteLine($"Elapsed {stopwatch.ElapsedMilliseconds} ms");
                return xorSums.TotalFulls;
            }, 0.001, 50);

            Console.WriteLine($"Points to change: {xorSums.TotalFulls}");
            tasks.AddRange(CreatePointwiseTasks());

            Console.WriteLine("FigureDecomposer.Decompose() complete");
            return tasks;
        }

        private void UpdateXorMatrix(Region region)
        {
            // This violates incapsulation principle but provides optimization benefits

            var curContent = curMatrix.GetContent();
            var targetContent = targetMatrix.GetContent();
            var xorContent = xorMatrix.GetContent();
            for (var i = region.Min.X; i <= region.Max.X; i++)
            for (var j = region.Min.Y; j <= region.Max.Y; j++)
            for (var k = region.Min.Z; k <= region.Max.Z; k++)
                xorContent[i, j, k] = curContent[i, j, k] ^ targetContent[i, j, k];
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
            return new State(rand.NextDouble() < 0.5, ClipRegion(region));
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

	    private Vector MutateVector(Vector v)
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

        private bool BreakesGrounding(State state)
        {
            if (state.Fill)
                return !curGroundChecker.CanPlace(state.Region.AllPoints().ToList());
            else
                return !curGroundChecker.CanRemove(state.Region.AllPoints().ToHashSet());
        }

        private long EvaluateDifferenceFrom(Matrix matrix)
        {
            return 2 * (matrix ^ targetMatrix).CountFulls() * costPerStep;
        }
    }
}
