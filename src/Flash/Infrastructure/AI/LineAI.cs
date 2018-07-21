using System;
using System.Collections.Generic;
using Flash.Infrastructure.AI;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure
{
    public class LineAI : IAI
    {
        private readonly Matrix modelToDraw;

        public LineAI(Matrix modelToDraw)
        {
            this.modelToDraw = modelToDraw;
        }

        public IEnumerable<ICommand> NextStep(State state)
        {
            throw new NotImplementedException();
        }
    }
}