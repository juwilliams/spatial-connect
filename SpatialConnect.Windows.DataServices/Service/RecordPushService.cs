using gbc.BL;
using gbc.DAL;
using gbc.DAO;
using gbc.Interfaces;
using gbc.Util;
using log4net;
using Newtonsoft.Json;
using SpatialConnect.Entity;
using SpatialConnect.Windows.DataServices.App;
using SpatialConnect.Windows.DataServices.BaseClasses;
using SpatialConnect.Windows.DataServices.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Container = SpatialConnect.Entity.Container;

namespace SpatialConnect.Windows.DataServices.Service
{
    public sealed class RecordPushService : IDataService
    {
        #region Members

        public Container Container { get; set; }
        public event SpatialConnectDataServiceEventHandler DataServiceEvent;

        private static ILog _log = LogManager.GetLogger(typeof(RecordPushService));
        private BackgroundWorker _worker;

        #endregion


        #region ISpatialConnectDataService


        public void Run(BackgroundWorker worker)
        {
            _worker = worker;

            this.Run();
        }

        public void Run()
        {
            try
            {
                _log.Info("push starting for... " + this.Container.name);

                //  if this container is updating only, and we dont have any established relationship keys, no need to push
                //  populate data ids if relationships are being used
                if (this.Container.update_only && 
                        this.Container.use_relationships && !this.Container.Relationships.keys.Any())
                {
                    this.AfterRun(new List<GeoRecord>());

                    return;
                }

                List<GeoRecord> records = new List<GeoRecord>();

                //  get a list of the json files that we need to run from the /pull directory for this container
                string[] pulls = 
                    Directory.GetFiles(ServiceApp.app_path + "\\" + this.Container.name + "\\pull\\", "*.pull.json");

                //  load each file, concatentate the records
                for (int i = 0; i <= pulls.Length - 1; i++)
                {
                    string pull = pulls[i];

                    ServiceActivity activity = DataUtil.GetObjFromJson<ServiceActivity>(pull);

                    records = records.Concat(activity.records).ToList();
                }

                //  filter out previously pushed records
                records = records.Where(p => !this.Container.PushHistory.uids.Contains(p.uid)).ToList();

                //  populate data ids if relationships are being used
                if (this.Container.use_relationships && this.Container.Relationships.keys.Any())
                {
                    var related = records.Where(p => this.Container.Relationships.keys.Any(x => x.internal_id == p.id));

                    foreach (GeoRecord relation in related)
                    {
                        relation.dataid = this.Container.Relationships.keys.FirstOrDefault(p => p.internal_id == relation.id).external_id;
                        relation.was_update = true;
                    }

                    //  only send update records if this is update only
                    if (this.Container.update_only)
                    {
                        records = related.ToList();
                    }
                }

                //  push the concatenated record lists
                switch (this.Container.destination.ToLower())
                {
                    case "webeoc":
                        {
                            PushToWebEOC(records);

                            break;
                        }
                    case "arcgis":
                        {
                            PushToArcGIS(records);

                            break;
                        }
                    default:
                        {
                            throw new Exception("no known destination for records specified in container configuration, cannot continue..");
                        }
                }
            }
            catch (Exception ex)
            {
                _log.Error("error during push!");
                _log.Error(ex.Message, ex);

                this.AfterRun(null);
            }
        }

        public void AfterRun(List<GeoRecord> results)
        {
            try
            {
                //  record relationships if they are being used
                if (this.Container.use_relationships)
                {
                    var added = results.Where(p => p.was_update == false);
                    foreach (GeoRecord add in added)
                    {
                        Key key = new Key()
                        {
                            external_id = add.dataid,
                            internal_id = add.id,
                            field_name = this.Container.key
                        };

                        this.Container.Relationships.keys.Add(key);
                    }
                }

                //  record what was pushed
                this.Container.PushHistory.uids = 
                    this.Container.PushHistory.uids.Concat(results.Select(p => p.uid)).ToList();

                //  save the history
                this.Container.PushHistory.Write(ServiceApp.app_path + "\\" + this.Container.name + "\\push\\history.json");
                this.Container.Relationships.Write(ServiceApp.app_path + "\\" + (!string.IsNullOrEmpty(this.Container.relationships_dir) ? this.Container.relationships_dir : this.Container.name) + "\\relationships.json");
                this.Container.Cache.Write(ServiceApp.app_path + "\\" + this.Container.name + "\\cache.json");

                _log.Info("push complete!");
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            finally
            {
                DataServiceScheduler.Instance.DeRegister(this.Container);
            }
        }

        #endregion


        #region Push Helpers

        private void PushToArcGIS(List<GeoRecord> records)
        {
            _log.Info("communicating with arcgis...");

            SDEManager sdeManager = new SDEManager(this.Container);

            UpdateResult updateResult =
                sdeManager.UpdateSDE(this.Container.license_type,
                                     this.Container.name,
                                     this.Container.geometry,
                                     this.Container.wkid, 
                                     records);

            this.AfterRun(updateResult.Affected);
        }

        private void PushToWebEOC(IEnumerable<IGeoRecord> records)
        {
            _log.Info("communicating with webeoc...");

            WebEOCManager webEOCManager = new WebEOCManager(this.Container);

            UpdateResult updateResult =
                webEOCManager.UpdateWebEOC(this.Container.board,
                                           this.Container.view,
                                           records,
                                           this.Container.has_attachments,
                                           _worker);

            this.AfterRun(updateResult.Affected);
        }

        #endregion


        #region Ctor

        public RecordPushService(Container container)
        {
            this.Container = container;
        }

        #endregion
    }
}
