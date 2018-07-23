using System;
using System.Collections.Generic;
using System.Linq;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Simulation
{
    public class Simulator
    {
        public void NextStep(State state, Trace trace)
        {
            //if (state.Bots.Length != trace.Count)
            //    throw new Exception("Commands count should be equal to bots count");

            UpdateEnergy(state);

            while (trace.Any())
            {
                var bots = state.Bots.ToList();
                var commands = trace.Dequeue(bots.Count);
                Execute(state, bots.Zip(commands, (x, y) => (x, y)).ToList());
            }
        }

        private static void Execute(State state, List<(Bot bot, ICommand command)> commands)
        {
            var singleCommands = new List<(Bot bot, ICommand command)>();
            var groupCommands = new Dictionary<Region, List<(Bot bot, IGroupCommand command)>>();

            // split commands on single and group
            foreach (var (bot, command) in commands)
            {
                if (command is IGroupCommand groupCommand)
                {
                    var region = new Region(bot.Pos + groupCommand.NearDistance, bot.Pos + groupCommand.NearDistance + groupCommand.FarDistance);
                    if (!groupCommands.ContainsKey(region))
                    {
                        groupCommands[region] = new List<(Bot bot, IGroupCommand command)>();
                    }
                    groupCommands[region].Add((bot, groupCommand));
                }
                else
                {
                    singleCommands.Add((bot, command));
                }
            }

            // validation
            //Validate(state, singleCommands, groupCommands);

            // apply
            foreach (var (bot, command) in singleCommands)
            {
                command.Apply(state, bot);
            }

            foreach (var (bot, command) in groupCommands.Values.Select(v => v.First()))
            {
                command.Apply(state, bot);
            }
        }

        private static void Validate(
            State state, 
            List<(Bot bot, ICommand command)> singleCommands, 
            Dictionary<Region, List<(Bot bot, IGroupCommand command)>> groupCommands)
        {
            foreach (var pair in groupCommands)
            {
                var region = pair.Key;
                var commands = pair.Value;

                if (region.Dim == 0 || (int) Math.Pow(2, region.Dim) != commands.Count)
                {
                    throw new Exception("Invalid group command");
                }
            }

            var regions = singleCommands.SelectMany(c => GetRegions(c.command, c.bot))
                .Concat(groupCommands.Values.SelectMany(GetRegions))
                .ToList();

            for (var i = 0; i < regions.Count-1; i++)
            {
                for (var j = i+1; j < regions.Count; j++)
                {
                    if (regions[i].AreIntersectsWith(regions[j]))
                    {
                        throw new Exception("Commands used regionst itersects");
                    }
                }
            }

        }

        private static void UpdateEnergy(State state)
        {
            var r = state.Matrix.R;
            if (state.Harmonics)
            {
                state.Energy += 30*r*r*r;
            }
            else
            {
                state.Energy += 3*r*r*r;
            }

            state.Energy += 20*state.Bots.Length;
        }

        private static Region[] GetRegions(ICommand command, Bot bot)
        {
            switch (command)
            {
                case FillCommand f:
                    return new[]
                    {
                        new Region(bot.Pos),
                        new Region(bot.Pos + f.NearDistance)
                    };
                case FissionCommand f:
                    return new[]
                    {
                        new Region(bot.Pos),
                        new Region(bot.Pos + f.NearDistance)
                    };
                case FlipCommand f:
                case HaltCommand h:
                case WaitCommand w:
                    return new[]
                    {
                        new Region(bot.Pos)
                    };
                case LMoveCommand l:
                    return new[]
                    {
                        new Region(bot.Pos, bot.Pos + l.FirstDirection),
                        new Region((bot.Pos + l.FirstDirection) + l.SecondDirection.Normalize(), bot.Pos + l.FirstDirection + l.SecondDirection)
                    };
                case SMoveCommand s:
                    return new[]
                    {
                        new Region(bot.Pos, bot.Pos + s.Direction)
                    };
                case FusionPCommand f:
                    return new[]
                    {
                        new Region(bot.Pos),
                        new Region(bot.Pos + f.NearDistance)
                    };
                case FusionSCommand f:
                    return new Region[]
                    {
                    };
                case VoidCommand v:
                    return new[]
                    {
                        new Region(bot.Pos),
                        new Region(bot.Pos + v.NearDistance)
                    };
                default:
                    throw new Exception($"Unknown command {command}");
            }
        }

        private static Region[] GetRegions(List<(Bot bot, IGroupCommand command)> commands)
        {
            var (bot, command) = commands.First();
            var region = new Region(bot.Pos + command.FarDistance, bot.Pos + command.FarDistance + command.NearDistance);

            return commands.Select(x => new Region(x.bot.Pos)).Concat(new[] {region}).ToArray();
        }
    }
}
