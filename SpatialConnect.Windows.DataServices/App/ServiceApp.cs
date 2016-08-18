using gbc.Util;
using log4net;
using SpatialConnect.Entity;
using SpatialConnect.Windows.DataServices.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpatialConnect.Windows.DataServices.App
{
    public sealed class ServiceApp
    {
        #region Members

        public List<Container> containers { get; set; }

        private static ILog _log = LogManager.GetLogger(typeof(ServiceApp));
        private DataServiceManagerBase _serviceManager { get; set; }

        public static string app_path;
        public static Config GlobalConfig;
        public static string task_type { get; set; }
        
        #endregion


        #region Ctor

        public ServiceApp(string appPath)
        {
            try
            {
                containers = new List<Container>();

                app_path = appPath;
                GlobalConfig = DataUtil.GetObjFromJson<Config>(app_path + "\\" + "config.json");
                GlobalConfig.app_path = appPath;

                this.GetSpatialContainers();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        #endregion


        #region Service Manager

        public void StartManager()
        {
            if (!this.containers.Any())
            {
                _log.Debug("No Containers. Aborting record push.");

                return;
            }

            if (_serviceManager == null)
            {
                _serviceManager = new DataServiceManagerBase(ServiceApp.app_path, ServiceApp.GlobalConfig, this.containers);
            }

            _serviceManager.ProcessSynchronousContainers();
        }

        #endregion


        #region Initialization Helpers

        void GetSpatialContainers()
        {
            foreach (string c in GlobalConfig.containers)
            {
                Container container = DataUtil.GetObjFromJson<Container>(ServiceApp.app_path + "\\" + c + "\\container.json");
                container.Config = ServiceApp.GlobalConfig;
                container.Fields = DataUtil.GetObjFromJson<Fields>(ServiceApp.app_path + "\\" + c + "\\fields.json");
                container.PullHistory = DataUtil.GetObjFromJson<History>(ServiceApp.app_path + "\\" + c + "\\pull\\history.json");
                container.PushHistory = DataUtil.GetObjFromJson<History>(ServiceApp.app_path + "\\" + c + "\\push\\history.json");

                container.Cache =
                    DataUtil.GetObjFromJson<Cache>(ServiceApp.app_path + "\\" + c + "\\cache.json");

                container.Relationships = 
                    DataUtil.GetObjFromJson<Relationships>(ServiceApp.app_path + "\\" + (!string.IsNullOrEmpty(container.relationships_dir) ? container.relationships_dir : c) + "\\relationships.json");

                this.containers.Add(container);
            }
        }        

        #endregion
    }
}
