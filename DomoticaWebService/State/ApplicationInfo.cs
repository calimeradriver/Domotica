using System.Xml;
using Vendjuuren.Domotica.Library;
using System.Timers;
using Vendjuuren.SQL;
using System;
using DwellNet;
using Vendjuuren.Domotica.X10;
using System.Net;

namespace Vendjuuren.Domotica.WebService.State
{
  public class ApplicationInfo : IDisposable
  {
    public ApplicationInfo()
    {
      DatabaseConnection.Initialize(@"SERVER\SQLEXPRESS", "Domotica", "kaku", "kaku");

      this._deviceCollection = new DeviceCollection();
      this._deviceCollection.GetAll();
      this._programCollection = new ProgramCollection();
      this._programCollection.GetAll();

      TcpClientHelperClass.Connect(IPAddress.Parse("192.168.1.1"), 8221);

      DeviceHelper.InitializeAsClient(Statics.DeviceViewCollection);
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

    private Cm11 cm11;
    public Cm11 Cm11
    {
      get { return cm11; }
    }

    private DeviceCollection _deviceCollection;
    public DeviceCollection DeviceCollection
    {
      get { return (_deviceCollection); }
    }

    private ProgramCollection _programCollection;
    public ProgramCollection ProgramCollection
    {
      get { return (_programCollection); }
    }

    #region IDisposable Members

    public void Dispose()
    {
      //if (DatabaseConnection.IsInitialized)
        //DatabaseConnection.Close();

      if (_deviceCollection != null)
      {
        _deviceCollection.Clear();
        _deviceCollection = null;
      }

      if (_programCollection != null)
      {
        _programCollection.Clear();
        _programCollection = null;
      }

      //if (cm11 != null)
      //{
      //  cm11.Dispose();
      //  cm11.Close();
      //}
    }

    #endregion
  }
}
