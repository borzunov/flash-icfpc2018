using JobsCommon;
using MongoDB.Driver;

namespace JobExecutor
{
    class Program
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string aaaa;

        static void Main(string[] args)
        {
            var orchestrator = new Orchestrator(Log);
            orchestrator.Start();
        }
    }
}
