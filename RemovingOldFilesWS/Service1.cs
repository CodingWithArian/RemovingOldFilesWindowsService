using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Configuration;

namespace RemovingOldFilesWS
{
    [RunInstaller(true)]
    public partial class Service1 : ServiceBase
    {
        //Timer timer =new Timer();
        public Thread worker = null;
        int schad = Convert.ToInt32(ConfigurationSettings.AppSettings["ThreadTime"]);
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] arg)
        {
            try
            {
                ThreadStart start = new ThreadStart(working);

                worker = new Thread(start);
                worker.Start();
            }
            catch (Exception) { throw; }



            //timer.Interval = 5000; //number in milisecinds  
            //timer.Enabled = true;
            if (!Directory.Exists(@"F:\RemoveOldFilesServiceConfig"))
            {
                Directory.CreateDirectory(@"F:\RemoveOldFilesServiceConfig");
            }


            if (!File.Exists(@"F:\RemoveOldFilesServiceConfig\AddFoldersPaths.txt"))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(@"F:\RemoveOldFilesServiceConfig\AddFoldersPaths.txt"))
                {
                    sw.Write(@"F:\hazfi");

                }
            }


            if (!File.Exists(@"F:\RemoveOldFilesServiceConfig\SetNumberOfDays.txt"))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(@"F:\RemoveOldFilesServiceConfig\SetNumberOfDays.txt"))
                {
                    sw.Write("1");
                 
                }
            }


            if (!File.Exists(@"F:\RemoveOldFilesServiceConfig\SetThreadDuration.txt"))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(@"F:\RemoveOldFilesServiceConfig\SetThreadDuration.txt"))
                {
                    sw.Write("30000");
                }
                 
            }
            DeleteOldFiles(@"F:\RemoveOldFilesServiceConfig\AddFoldersPaths.txt");

            //System.Threading.Thread.Sleep(5000);



        }
        public void working()
        {
            while (true)
            {
                DeleteOldFiles(@"F:\RemoveOldFilesServiceConfig\AddFoldersPaths.txt");
                string setthread = File.ReadAllText(@"F:\RemoveOldFilesServiceConfig\SetThreadDuration.txt");
                int setthreadint = int.Parse(setthread);
                Thread.Sleep(setthreadint);
            }

        }


        protected override void OnStop()
        {
        }

        public void DeleteOldFiles(string path)
        {
            try
            {
                string setday = File.ReadAllText(@"F:\RemoveOldFilesServiceConfig\SetNumberOfDays.txt");
                int setdayint = int.Parse(setday);
                string[] readtext = File.ReadAllLines(path);
                DateTime dt = DateTime.Now;
                foreach (var line in readtext)
                {
                    string[] fileList = Directory.GetFiles(line);
                    //if (fileList.Length > 0)
                    //{
                    foreach (var item in fileList)
                    {

                        if (File.GetCreationTime(item).Minute+setdayint < dt.Minute)
                        {
                            File.Delete(item);
                        }
                    }
                }
                //}
            }
            catch (Exception)
            { }

        }


    }
}
