﻿namespace JobExecutor
{
    class Program
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            var orchestrator = new Orchestrator(Log);
            orchestrator.Start();
        }
    }
}
