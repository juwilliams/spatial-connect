using gbc.DAL;
using gbc.DAO;
using log4net;
using SpatialConnect.Windows.DataServices.App;
using SpatialConnect.Windows.DataServices.BaseClasses;
using SpatialConnect.Windows.DataServices.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Container = SpatialConnect.Entity.Container;

namespace SpatialConnect.Windows.DataServices.Service
{
    public sealed class RecordCaptureService : IDataService
    {
        #region Members

        public Container Container { get; set; }

        private static ILog _log = LogManager.GetLogger(typeof(RecordCaptureService));
        private DataRetrievalManager _dataRetrievalManager;
        private BackgroundWorker _worker;

        #endregion


        #region ISpatialContainerService

        public void Run(BackgroundWorker worker)
        {
            _worker = worker;

            this.Run();
        }

        public void Run()
        {
            try
            {
                _dataRetrievalManager = new DataRetrievalManager(this.Container);

                //  assign handlers for data retrieval events
                _dataRetrievalManager.OnDataRetrievalError += dataRetrievalManager_OnDataRetrievalError;
                _dataRetrievalManager.OnDataRetrievalSuccess += dataRetrievalManager_OnDataRetrievalSuccess;

                _log.Info("pull starting for... " + Container.name);

                _dataRetrievalManager.GetData();
            }
            catch (Exception ex)
            {
                //  TODO: log this error
                _log.Error(ex.Message, ex);

                _dataRetrievalManager.OnDataRetrievalError -= dataRetrievalManager_OnDataRetrievalError;
                _dataRetrievalManager.OnDataRetrievalSuccess -= dataRetrievalManager_OnDataRetrievalSuccess;
            }
        }

        public void AfterRun(List<GeoRecord> records)
        {
            try
            {
                if (records == null ||
                        !records.Any())
                {
                    return;
                }

                //  create a new log of activity
                ServiceActivity activity = new ServiceActivity();
                activity.created = DateTime.Now;
                activity.records = records.Where(p => !this.Container.PullHistory.uids.Contains(p.uid)).ToList();

                _log.Info("[" + activity.records.Count + "/" + records.Count + "]: records were not duplicates and will be saved.");

                this.Container.PullHistory.uids = 
                    this.Container.PullHistory.uids.Concat(activity.records.Select(p => p.uid)).ToList();

                //  write the files
                activity.Write(ServiceApp.app_path + "\\" + this.Container.name + "\\pull\\" + string.Format(ServiceActivity.PullFileNameFormat, activity.created.ToString("yyyyMMddhhmmss")));
                this.Container.PullHistory.Write(ServiceApp.app_path + "\\" + this.Container.name + "\\pull\\history.json");

                _log.Info("pull complete!");
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            finally //  clean up resources
            {
                DataServiceScheduler.Instance.DeRegister(this.Container);
            }
        }

        #endregion


        #region DataRetrievalManager Event Handlers

        private void dataRetrievalManager_OnDataRetrievalSuccess(object sender, List<GeoRecord> capturedRecords)
        {
            _dataRetrievalManager.OnDataRetrievalError -= dataRetrievalManager_OnDataRetrievalError;
            _dataRetrievalManager.OnDataRetrievalSuccess -= dataRetrievalManager_OnDataRetrievalSuccess;

            this.AfterRun(capturedRecords);
        }

        private void dataRetrievalManager_OnDataRetrievalError(object sender, Exception ex)
        {
            _dataRetrievalManager.OnDataRetrievalError -= dataRetrievalManager_OnDataRetrievalError;
            _dataRetrievalManager.OnDataRetrievalSuccess -= dataRetrievalManager_OnDataRetrievalSuccess;

            this.AfterRun(null);
        }

        #endregion


        #region Ctor

        public RecordCaptureService(Container container)
        {
            this.Container = container;
        }

        #endregion
    }
}
