using System.IO;
using System.ServiceProcess;
using System.Configuration;
using System.Threading;
using System;
using System.Diagnostics;

namespace MyService
{
    public partial class Service1 : ServiceBase
    {
        Logger logger;

        public Service1()
        {
            InitializeComponent();
            this.CanStop = true;
            this.AutoLog = false;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                string startPath = ConfigurationManager.AppSettings["startPath"];
                string endPath = ConfigurationManager.AppSettings["endPath"];

                if (startPath != null && endPath != null)
                {
                    if (!Directory.Exists(startPath)) { Directory.CreateDirectory(startPath); }
                    if (!Directory.Exists(endPath)) { Directory.CreateDirectory(endPath); }

                    logger = new Logger(startPath, endPath);
                    Thread loggerThread = new Thread(new ThreadStart(logger.Start));
                    loggerThread.Start();
                }
            }
            catch (Exception) { }
        }

        protected override void OnStop()
        {
            logger.Stop();
            Thread.Sleep(1000);
        }

    }
}
