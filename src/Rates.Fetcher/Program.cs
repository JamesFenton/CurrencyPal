using System;
using System.ServiceProcess;

namespace Rates.Fetcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new Service();

            if (Environment.UserInteractive)
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