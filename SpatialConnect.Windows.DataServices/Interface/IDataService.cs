using System;
using System.ComponentModel;

using Container = SpatialConnect.Entity.Container;

namespace SpatialConnect.Windows.DataServices.Interface
{
    public delegate void SpatialConnectDataServiceEventHandler(object sender, Type type);

    public interface IDataService
    {
        Container Container { get; set; }

        void Run();
        void Run(BackgroundWorker worker);
    }
}
