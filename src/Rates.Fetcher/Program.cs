using System;
using System.ServiceProcess;

namespace Rates.Fetcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var developerMode = Environment.UserInteractive;
            var service = new Service(developerMode);

            if (developerMode)
            {
                service.Start();
            }
            else
            {
                ServiceBase.Run(service);
            }
        }
    }
}