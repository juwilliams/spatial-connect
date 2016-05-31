using log4net;
using log4net.Config;
using SpatialConnect.Windows.DataServices.App;
using System;
using System.Configuration;

namespace SpatialConnect.Windows.Task
{
    class Program
    {
        private static ILog _log = LogManager.GetLogger("Spatial Connect Task App");
        private static ServiceApp _app;

        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    throw new Exception("No task type command line argument supplied. Aborting");
                }

                ServiceApp.task_type = args[0];

                System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrap;
                System.AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
                //  Initialize log4net
                string appPath = ConfigurationManager.AppSettings["app-path"];

                GlobalContext.Properties["AppPath"] = appPath;
                XmlConfigurator.Configure();

                _app = new ServiceApp(appPath);
                
                _app.StartManager();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            ServiceApp.GlobalConfig.ReleaseManagedContainers();
            ServiceApp.GlobalConfig.Write(ServiceApp.app_path + "\\" + "config.json");
        }

        private static void UnhandledExceptionTrap(object sender, UnhandledExceptionEventArgs e)
        {
            ServiceApp.GlobalConfig.ReleaseManagedContainers();
            ServiceApp.GlobalConfig.Write(ServiceApp.app_path + "\\" + "config.json");

            _log.Error(e.ExceptionObject.ToString());
        }
    }
}
