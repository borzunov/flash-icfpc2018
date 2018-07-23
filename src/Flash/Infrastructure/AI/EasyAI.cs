using System.Collections.Generic;
using System.Linq;
using Flash.Infrastructure.Commands;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.AI
{
    public class EasyAI : IAI
    {
        private readonly Matrix modelToDraw;
        public List<HashSet<Vector>> readyToFill;
        public bool finished;

        public EasyAI(Matrix modelToDraw)
        {
            this.modelToDraw = modelToDraw;
            readyToFill = new List<HashSet<Vector>>();
            for (var x = 0; x < modelToDraw.R; x++)
            {
                readyToFill.Add(new HashSet<Vector>());
                for (var z = 0; z < modelToDraw.R; z++)
                {
                    if (modelToDraw.IsFull(new Vector(x, 0, z)))
                    {
                        readyToFill[0].Add(new Vector(x, 0, z));
                    }
                }
            }
            finished = false;
        }

        public IEnumerable<ICommand> NextStep(State state)
        {
            var currentPos = state.Bots[0].Pos;
            if (finished && !Equals(currentPos, new Vector(0, 0, 0))) // Если закончили, и не в нуле, то идем туда
            {
                yield return new SMoveCommand(new Vector(0, 0, 0) - currentPos);
            }
            else
            {
                if (finished) // Если закончили и пришли в 0, то мы красавцы
                {
                    yield return new HaltCommand();
                }
                else
                {
                    var level = 0;
                    while (level < modelToDraw.R && !readyToFill[level].Any()) //ищем слой, на котором будем красить
                        level++;
                    if (level >= modelToDraw.R) // Если не нашли такой слой, значит все покрасили и идем домой
                    {
                        finished = true;
                        yield return new SMoveCommand(new Vector(0, 0, 0) - currentPos);
                    }
                    else
                    {
                        var voxelToFill =
                            readyToFill[level].OrderBy(v => (currentPos - v).Mlen)
                                .First(); // Берем ближайшую точку на слое
                        if ((voxelToFill - currentPos).IsNd) //Если она в nd, тогда красим её
                        {
                            var adjacents = state.Matrix.GetAdjacents(voxelToFill);
                            var newReadyToFill = adjacents.Where(a => state.Matrix.IsVoid(a) && modelToDraw.IsFull(a) &&
                                                                      !readyToFill[a.Y].Contains(a));
                            foreach (var newReadyVector in newReadyToFill)
                            {
                                readyToFill[newReadyVector.Y]
                                    .Add(
                                        newReadyVector); //смежные блоки, которые ещё не закрашены, но должны быть добавляем в список
                            }
                            readyToFill[level].Remove(voxelToFill);
                            yield return new FillCommand(voxelToFill - currentPos);
                        }
                        else
                        {
                            var destination = new Vector(voxelToFill.X, voxelToFill.Y + 1, voxelToFill.Z);
                            yield return new SMoveCommand(destination - currentPos); //если она не в nd, то идем к ней
                        }
                    }
                }
            }
        }
    }
}