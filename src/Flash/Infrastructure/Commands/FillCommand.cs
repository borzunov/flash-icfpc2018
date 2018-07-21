﻿using System;

namespace Flash.Infrastructure.Commands
{
    public class FillCommand : ICommand
    {
        public readonly Vector NearDistance;

        public FillCommand(Vector nearDistance)
        {
            NearDistance = nearDistance;
        }

        public void Apply(State state, Bot bot)
        {
            var voxel = bot.Pos + NearDistance;

            if (state.Matrix.IsFull(voxel))
            {
                state.Energy += 6;
            }
            else
            {
                state.Energy += 12;
                state.Matrix.Fill(voxel);
            }
        }
    }
}