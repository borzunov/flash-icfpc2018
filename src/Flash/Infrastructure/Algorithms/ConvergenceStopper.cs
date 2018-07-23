using System;
using System.Collections.Generic;

namespace Flash.Infrastructure.Algorithms
{
    public static class ConvergenceStopper
    {
        public static void Run(Func<int, double?> iterationFunc,
            double minRelChange, int patienceInEpochs)
        {
            // `iterationFunc` should return a value of the metric to check
            // or `null` to break immediately.

            var metrics = new List<double>();
            for (var iter = 1;; iter++)
            {
                var curMetric = iterationFunc(iter);
                if (curMetric == null)
                    break;
                metrics.Add(curMetric.Value);

                if (iter < patienceInEpochs + 1)
                    continue;
                var prevMetric = metrics[iter - 1 - patienceInEpochs];
                if (prevMetric - curMetric < minRelChange * prevMetric)
                    break;
            }
        }
    }
}
