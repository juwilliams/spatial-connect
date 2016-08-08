using log4net;
using SpatialConnect.Entity;
using SpatialConnect.Windows.DataServices.App;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

using Container = SpatialConnect.Entity.Container;

namespace SpatialConnect.Windows.DataServices.BaseClasses
{
    internal sealed class DataServiceManagerBase
    {
        #region Members

        public bool IsRunning;
        public BackgroundWorker ManagerWorker;

        private static readonly ILog _log = LogManager.GetLogger(typeof(DataServiceManagerBase));
        private Config _config;
        private string _appPath;
        private List<Container> _containers;
        private readonly object _lock = new object();

        #endregion


        #region Management Helpers

        public void ProcessSynchronousContainers()
        {
            DataServiceScheduler.Instance.SetServiceManager(this);

            //  roll through any pending captures
            for (int i = 0, containersLen = _containers.Count; i < containersLen; i++)
            {
                Container container = _containers[i];

                //  filter out containers which exist in the config.running_containers array
                if (_config.running_containers.Contains(container.name))
                {
                    continue;
                }

                DataServiceScheduler.Instance.Register(container);
            }

            while (DataServiceScheduler.Instance.ServiceQueue.Count > 0)
            {
                Thread.Sleep(1000);
            }
        }

        #endregion


        #region Ctor

        public DataServiceManagerBase(string appPath, Config config, List<Container> containers)
        {
            _appPath = appPath;
            _config = config;
            _containers = containers;
        }

        #endregion
    }
}
