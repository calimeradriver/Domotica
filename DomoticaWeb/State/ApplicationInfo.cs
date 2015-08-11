using System.Xml;
using Vendjuuren.Domotica.Library;
using System.Timers;
using Vendjuuren.SQL;
using System;
using DwellNet;
using System.Net;

namespace Vendjuuren.Domotica.Web.State
{
  public class ApplicationInfo : IDisposable
  {
    public ApplicationInfo()
    {
      DatabaseConnection.Initialize(@"SERVER\SQLEXPRESS", "Domotica", "kaku", "kaku");

      Statics.DeviceViewCollection = new DeviceViewCollection();
      Statics.DeviceViewCollection.GetAll();

      Statics.ProgramCollection = new ProgramCollection();
      Statics.ProgramCollection.GetAll();

      TcpClientHelperClass.Connect(IPAddress.Parse("192.168.1.1"), 8221);

      DeviceHelper.InitializeAsClient(Statics.DeviceViewCollection);
      DeviceHelper.OnDevicePowerOn += new DeviceHelper.DevicePowerEventDelegate(DeviceHelper_OnDevicePowerOn);
      DeviceHelper.OnDevicePowerOff += new DeviceHelper.DevicePowerEventDelegate(DeviceHelper_OnDevicePowerOff);
      DeviceHelper.UpdateDevices += new DeviceHelper.UpdateDevicesDelegate(DeviceHelper_UpdateDevices);
    }

    void DeviceHelper_UpdateDevices(DeviceViewCollection devices)
    {
      Statics.DeviceViewCollection = devices;
    }

    void DeviceHelper_OnDevicePowerOff(DeviceView device)
    {
      //foreach (ListViewItem item in deviceCollectionView.Items)
      //{
      //  createDeviceLVI(item, device);
      //}
    }

    void DeviceHelper_OnDevicePowerOn(DeviceView device)
    {
      //foreach (ListViewItem item in deviceCollectionView.Items)
      //{
      //  createDeviceLVI(item, device);
      //}
    }

    #region IDisposable Members

    public void Dispose()
    {
      //if (DatabaseConnection.SqlConnection != null)
        ////DatabaseConnection.Close();

      if (Statics.DeviceViewCollection != null)
        Statics.DeviceViewCollection.Clear();

      if (Statics.ProgramCollection != null)
        Statics.ProgramCollection.Clear();

      //if (cm11 != null)
      //{
      //  cm11.Dispose();
      //  cm11.Close();
      //}
    }

    #endregion
  }
}
