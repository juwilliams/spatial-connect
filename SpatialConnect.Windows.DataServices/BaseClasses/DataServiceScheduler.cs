using log4net;
using SpatialConnect.Entity;
using SpatialConnect.Windows.DataServices.App;
using SpatialConnect.Windows.DataServices.Interface;
using SpatialConnect.Windows.DataServices.Service;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Threading;

using Container = SpatialConnect.Entity.Container;

namespace SpatialConnect.Windows.DataServices.BaseClasses
{
    internal sealed class DataServiceScheduler
    {
        #region Members

        public BackgroundWorker ManagerWorker;
        public bool IsStarted { get; set; }
        public ConcurrentStack<IDataService> ServiceQueue { get; set; }

        protected readonly object _lock = new object();
        protected ManualResetEvent _stoppingEvent;
        protected ManualResetEvent _stoppedEvent;

        private DataServiceManagerBase _serviceManager;
        private static readonly ILog _log = LogManager.GetLogger(typeof(DataServiceManagerBase));
        private TimeSpan _updateInterval = new TimeSpan(5000);
        private bool _isSingleRun;
        private string _appPath;
        private Config _config;

        private static DataServiceScheduler _instance;
        public static DataServiceScheduler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DataServiceScheduler();
                }

                return _instance;
            }
            set { _instance = value; }
        }

        #endregion


        #region Ctor

        public DataServiceScheduler()
        {
            ServiceQueue = new ConcurrentStack<IDataService>();

            ManagerWorker = new BackgroundWorker();
            ManagerWorker.WorkerReportsProgress = true;
            ManagerWorker.WorkerSupportsCancellation = true;
            ManagerWorker.DoWork += ManagerWorker_DoWork;
        }

        #endregion


        #region Scheduler Actions

        public void Start()
        {
            lock (_lock)
            {
                if (_stoppedEvent != null || (ManagerWorker != null && ManagerWorker.IsBusy))
                {
                    return;
                }

                this.IsStarted = true;

                _stoppedEvent = new ManualResetEvent(false);
                _stoppingEvent = new ManualResetEvent(false);
                
                _stoppingEvent.Reset();
                _stoppedEvent.Reset();
                
                ManagerWorker.RunWorkerAsync();
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (_stoppingEvent != null)
                {
                    ManagerWorker.CancelAsync();

                    _stoppingEvent.Set();
                }
            }

            if (_stoppedEvent != null)
            {
                _stoppedEvent.WaitOne();
            }
        }

        public static void StopScheduler()
        {
            if (_instance == null)
            {
                return;
            }

            _instance.Stop();
        }

        #endregion


        #region Event Handlers

        protected void ManagerWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _log.Debug("SpatialContainer processing started");

            try
            {
                while (!e.Cancel)
                {
                    //  run the next item in the queue
                    if (_instance.ServiceQueue.Any(p => !p.Container.working))
                    {
                        IDataService service = _instance.ServiceQueue.First();

                        this.SetContainerWorking(service.Container);

                        service.Run(ManagerWorker);
                    }

                    if (!_instance.ServiceQueue.Any())
                    {
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            finally
            {
                _stoppedEvent.Set();

                _stoppingEvent = null;
                _stoppedEvent = null;

                this.IsStarted = false;
            }
        }

        #endregion


        #region Data Service Registration

        public void DeRegister(Container container)
        {
            this.ServiceQueue =
                new ConcurrentStack<IDataService>(this.ServiceQueue.Where(p => p.Container != null && p.Container != container));

            this.SetContainerNotWorking(container);

            if (_isSingleRun &&
                    this.ServiceQueue != null &&
                        this.ServiceQueue.Count < 1)
            {
                _log.Info("No further containers to process. Shutting down.");

                this.Stop();
            }
        }

        public void Register(Container container)
        {
            IDataService dataService;

            if (this.ServiceQueue.Any(p => p.Container.name == container.name))
            {
                return;
            }

            //  notify the config that this container is now managed and should be released
            ServiceApp.GlobalConfig.AddManagedContainer(container.name);

            if (ServiceApp.task_type == "push")
            {
                _log.Debug("Container Push Service Created");

                dataService = new RecordPushService(container);
            }
            else
            {
                _log.Debug("Container Capture Service Created");

                dataService = new RecordCaptureService(container);
            }

            this.ServiceQueue.Push(dataService);

            _log.Debug("SpatialContainer Service Registered with Scheduler");

            if (!this.IsStarted)
            {
                this.Start();
            }
        }

        public void SetServiceManager(DataServiceManagerBase serviceManager)
        {
            _serviceManager = serviceManager;
        }

        public void SetContainerWorking(Container container)
        {
            container.working = true;
            ServiceApp.GlobalConfig.running_containers.Add(container.name);
            ServiceApp.GlobalConfig.Write(ServiceApp.app_path + "\\config.json");
        }

        public void SetContainerNotWorking(Container container)
        {
            container.working = false;
            ServiceApp.GlobalConfig.running_containers.Remove(container.name);
            ServiceApp.GlobalConfig.Write(ServiceApp.app_path + "\\config.json");
        }

        #endregion

    }
}
