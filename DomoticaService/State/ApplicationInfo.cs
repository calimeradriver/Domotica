using System.Xml;
using Vendjuuren.Domotica.Library;
using System.Timers;
using Vendjuuren.SQL;
using System;
using DwellNet;
using System.Net;
using Vendjuuren.Domotica.Service.Properties;

namespace Vendjuuren.Domotica.Service.State
{
  public class ApplicationInfo : IDisposable
  {
    private Scheduler _scheduler;

    public ApplicationInfo()
    {
      DatabaseConnection.Initialize(Settings.Default.SQLserverName, Settings.Default.DatabaseName,
        Settings.Default.UserName, Settings.Default.Password);

      TcpServerHelperClass.Start(IPAddress.Any, 8221);

      Statics.DeviceViewCollection = new DeviceViewCollection();
      Statics.DeviceViewCollection.GetAll();

      Statics.ProgramCollection = new ProgramCollection();
      Statics.ProgramCollection.GetAll();

      DeviceHelper.InitializeAsServer(Statics.DeviceViewCollection);

      _scheduler = new Scheduler(Statics.ProgramCollection);
      _scheduler.Start();

      DeviceHelper.OnDevicePowerOn += new DeviceHelper.DevicePowerEventDelegate(DeviceHelper_OnDevicePowerOn);
      DeviceHelper.OnDevicePowerOff += new DeviceHelper.DevicePowerEventDelegate(DeviceHelper_OnDevicePowerOff);
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
      if (_scheduler != null)
        _scheduler.Stop();

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
