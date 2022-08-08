using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MyService
{
    internal class Logger
    {
        FileSystemWatcher watcher;
        object obj = new object();
        bool enabled = true;
        string startPath, endPath;

        public Logger(string startPath, string endPath)
        {
            this.startPath = startPath;
            this.endPath = endPath;

            watcher = new FileSystemWatcher(startPath);
            watcher.Created += Watcher_Created;
            watcher.Changed += Watcher_Changed;
        }

        public void Start()
        {
            watcher.EnableRaisingEvents = true;
            while (enabled)
            {
                Thread.Sleep(1000);
            }
        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            enabled = false;
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {           
            ETL process = new ETL();
            process.Save(e.FullPath, endPath);        
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {

        }
    }
}
