using System.Collections.Generic;

namespace Flash.Infrastructure.Simulation
{
    public static class QueueExtensions
    {
        public static T[] Dequeue<T>(this Queue<T> queue, int count)
        {
            var result = new T[count];
            for (var i = 0; i < count; i++)
            {
                result[i] = queue.Dequeue();
            }

            return result;
        }
    }
}