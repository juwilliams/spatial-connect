using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace SpatialConnect.Windows.DataServices.Util
{
    public class ServiceUtil
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ServiceUtil));

        //  was once used to store record in SQL
        //public static void SessionFunc(ApiSoapClient client, Action action)
        //{
        //    using (new OperationContextScope(client.InnerChannel))
        //    {
        //        HttpRequestMessageProperty requestProperty = new HttpRequestMessageProperty();
        //        SmplSession session = new SmplSession()
        //        {
        //            ApiKey = ApiEndpoints.Keys.API_SESSION_KEY
        //        };
        //        requestProperty.Headers.Add("Cookie", "session=" + JsonConvert.SerializeObject(session));
        //        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestProperty;

        //        action();
        //    }
        //}

        //public static IEnumerable<IGeoRecord> GetJoinedRecords(SpatialContainer container, IGeoRecord record, IGeoRecordJoin join)
        //{
        //    try
        //    {
        //        SpatialConnectAPI.ApiSoapClient client = new SpatialConnectAPI.ApiSoapClient();

        //        SpatialContainer joinWithContainer = null;
        //        ServiceUtil.SessionFunc(client, delegate() { JsonConvert.DeserializeObject<SpatialContainer>(client.GetContainerByNameJson(join.WithContainerName)); });
        //        if (joinWithContainer == null)
        //        {
        //            _log.Debug("Join container not found for association.");

        //            return null;
        //        }

        //        joinWithContainer.IGeoRecords = null;
        //        ServiceUtil.SessionFunc(client, delegate() { JsonConvert.DeserializeObject<List<IGeoRecord>>(client.GetIGeoRecordsJson(joinWithContainer.Id, -1)); });

        //        if (joinWithContainer == null ||
        //                joinWithContainer.IGeoRecords == null ||
        //                    !joinWithContainer.IGeoRecords.Any())
        //        {
        //            _log.Debug("No records found in associated hub.");

        //            return null;
        //        }

        //        IEnumerable<IGeoRecord> IGeoRecordsToField =
        //            joinWithContainer.IGeoRecords.Where(p => p.Fields.Any(x =>
        //                x.Name.ToLower() == join.ToFieldName.ToLower()));

        //        IEnumerable<IGeoRecord> IGeoRecordsWithFieldAndValue =
        //            IGeoRecordsToField.Where(p =>
        //                p.Fields.Any(x => record != null && x.Value == record.Fields.First(y => y.Name.ToLower() == join.WithFieldName.ToLower()).Value));

        //        return IGeoRecordsWithFieldAndValue;
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Error(ex.Message, ex);

        //        return new List<IGeoRecord>();
        //    }
        //}

        //public static IGeoRecordField GetIGeoFieldWithTagOrDefault(IEnumerable<IGeoRecordField> fieldsToSearch, string tag)
        //{
        //    foreach (IGeoRecordField f in fieldsToSearch)
        //    {
        //        if (string.IsNullOrEmpty(f.Tags))
        //        {
        //            continue;
        //        }

        //        string[] tags = f.Tags.Split(',');

        //        if (tags.Any(p => p.ToLower() == tag.ToLower()))
        //        {
        //            return f;
        //        }
        //    }

        //    return null;
        //}
    }
}
