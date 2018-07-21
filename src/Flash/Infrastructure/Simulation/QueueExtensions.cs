using System.Collections.Generic;
using Flash.Infrastructure.Models;

namespace Flash.Infrastructure.Simulation
{
    public static class QueueExtensions
    {
        public static T[] Dequeue<T>(this Queue<T> queue, int count)
        {
            var result = new List<T>();
            for (var i = 0; queue.Count > 0 && i < count; i++)
            {
                result.Add(queue.Dequeue());
            }

            return result.ToArray();
        }
    }
}