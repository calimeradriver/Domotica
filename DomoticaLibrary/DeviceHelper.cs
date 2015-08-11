using Vendjuuren.Domotica.Radiographically;
using Vendjuuren.Domotica.X10;
using Vendjuuren.SQL;
using System;
using Vendjuuren.Library.Network;
using System.Linq;

namespace Vendjuuren.Domotica.Library
{
  public class DeviceHelper
  {
    private static bool isInitialized;
    private static bool _isServerContext;
    private static Cm11 _cm11;

    public delegate void DevicePowerEventDelegate(DeviceView device);
    public static event DevicePowerEventDelegate OnDevicePowerOn;
    public static event DevicePowerEventDelegate OnDevicePowerOff;

    public delegate void UpdateDevicesDelegate(DeviceViewCollection devices);
    public static event UpdateDevicesDelegate UpdateDevices;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="databaseConnection"></param>
    /// <param name="deviceCollection"></param>
    public static void InitializeAsServer(DeviceViewCollection deviceCollection)
    {
      RF.PoweredOn += new RF.DevicePowerDelegate(RF_PoweredOn);
      RF.PoweredOff += new RF.DevicePowerDelegate(RF_PoweredOff);
      RF.OnPowerError += new RF.DevicePowerErrorDelegate(RF_OnPowerError);

      _cm11 = new Cm11();
      _cm11.Close();
      _cm11.Notification += new Cm11LowLevelNotificationEventDelegate(_cm11_Notification);
      _cm11.OffReceived += new Cm11DeviceNotificationEventDelegate(_cm11_OffReceived);
      _cm11.OnReceived += new Cm11DeviceNotificationEventDelegate(_cm11_OnReceived);
      _cm11.LogMessage += new Cm11LogMessageEventDelegate(_cm11_LogMessage);
      _cm11.DimReceived += new Cm11BrightenOrDimNotificationEventDelegate(_cm11_DimReceived);
      _cm11.BrightenReceived += new Cm11BrightenOrDimNotificationEventDelegate(_cm11_BrightenReceived);
      _cm11.IdleStateChange += new Cm11IdleStateChangeEventDelegate(_cm11_IdleStateChange);

      try
      {
        if (!_cm11.IsOpen)
          _cm11.Open("COM4");
      }
      catch (Exception ex)
      {
        new Log(LogType.Error, LogAction.Exception, "Can't connect to CM11 device, because: " + ex.ToString());
      }

      _isServerContext = true;

      isInitialized = DatabaseConnection.IsInitialized;
    }

    public static void _cm11_IdleStateChange(bool idle)
    {
      
    }

    public static void InitializeAsClient(DeviceViewCollection deviceCollection)
    {
      _isServerContext = false;
      isInitialized = DatabaseConnection.IsInitialized;
    }

    public static void GetAllDevices()
    {
      Statics.DeviceViewCollection.GetAll();

      if (UpdateDevices != null)
        UpdateDevices(Statics.DeviceViewCollection);
    }

    /// <summary>
    /// Power device On
    /// </summary>
    /// <param name="device"></param>
    public static void PowerOn(DeviceView device, string logDetails)
    {
      powerOnOrOff(device, Power.On, logDetails);
    }

    /// <summary>
    /// Power device Off
    /// </summary>
    /// <param name="device"></param>
    public static void PowerOff(DeviceView device, string logDetails)
    {
      powerOnOrOff(device, Power.Off, logDetails);

    }

    /// <summary>
    /// Toggle power state of device
    /// </summary>
    /// <param name="device"></param>
    public static void PowerToggle(DeviceView device, string logDetails)
    {
      if (device.Power == Power.On)
        PowerOff(device, logDetails);
      else
        PowerOn(device, logDetails);
    }

    /// <summary>
    /// Power device On/Off
    /// </summary>
    /// <param name="device"></param>
    /// <param name="power"></param>
    /// <param name="logDetails"></param>
    private static void powerOnOrOff(DeviceView device, Power power, string logDetails)
    {
      if (!isInitialized)
        throw new Exception("DeviceHelper is not initialized!");

      LogAction logAction = (power == Power.On ? LogAction.DeviceOn : LogAction.DeviceOff);

      switch (device.Type)
      {
        case BrandType.Radiographically:
          if (power == Power.On)
          {
            if (_isServerContext)
              RF.PowerDeviceOn(device.Group.ToString(), device.Number);
            else
            {
              TcpMessage message = new TcpMessage("kaku", "kaku");
              message.AddTcpAction(TcpMessage.Type.PowerDeviceOn, device);
              TcpClientHelperClass.SendMessage(message);
            }
          }
          if (power == Power.Off)
          {
            if (_isServerContext)
              RF.PowerDeviceOff(device.Group.ToString(), device.Number);
            else
            {
              TcpMessage message = new TcpMessage("kaku", "kaku");
              message.AddTcpAction(TcpMessage.Type.PowerDeviceOff, device);              
              TcpClientHelperClass.SendMessage(message);
            }
          }
          break;
        case BrandType.X10:
          if (power == Power.On)
          {
            if (_isServerContext)
            {
              _cm11.TurnOnDevice(device.Address);
              //_cm11.BrightenLamp(device.Address, 50);
              updateDatabase(device, power, logAction, logDetails);
              if (OnDevicePowerOn != null)
                OnDevicePowerOn(device);
            }
            else
            {
              TcpMessage message = new TcpMessage("kaku", "kaku");
              message.AddTcpAction(TcpMessage.Type.PowerDeviceOn, device);
              TcpClientHelperClass.SendMessage(message);
            }
          }
          if (power == Power.Off)
          {
            if (_isServerContext)
            {
              _cm11.TurnOffDevice(device.Address);
              //_cm11.BrightenLamp(device.Address, 50);
              updateDatabase(device, power, logAction, logDetails);
              if (OnDevicePowerOff != null)
                OnDevicePowerOff(device);
            }
            else
            {
              TcpMessage message = new TcpMessage("kaku", "kaku");
              message.AddTcpAction(TcpMessage.Type.PowerDeviceOff, device);
              TcpClientHelperClass.SendMessage(message);
            }
          }
          break;
      }
    }

    /// <summary>
    /// Update database
    /// </summary>
    /// <param name="device"></param>
    /// <param name="logAction"></param>
    /// <param name="logDetails"></param>
    private static void updateDatabase(DeviceView device, Power power, LogAction logAction, string logDetails)
    {
      device.Power = power;
      device.Save();
      new Log(LogType.Information, logAction, device.ToString() + " " + logDetails);
    }

    protected static void RF_PoweredOn(string groupLetter, int number)
    {
      if (Statics.DeviceViewCollection == null || Statics.DeviceViewCollection.Count == 0)
        new Log(LogType.Error, LogAction.DevicePowerError, "Static DeviceCollection is null or empty, can't power on " + groupLetter + number);
      DeviceView device = Statics.DeviceViewCollection.GetDevice(groupLetter.GetLetter(), number);
      device.Power = Power.On;
      device.Save();
      new Log(LogType.Information, LogAction.DeviceOn, device.ToString());
      if (OnDevicePowerOn != null)
        OnDevicePowerOn(device);
    }

    protected static void RF_PoweredOff(string groupLetter, int number)
    {
      if (Statics.DeviceViewCollection == null || Statics.DeviceViewCollection.Count == 0)
        new Log(LogType.Error, LogAction.DevicePowerError, "Static DeviceCollection is null or empty, can't power off " + groupLetter + number);
      DeviceView device = Statics.DeviceViewCollection.GetDevice(groupLetter.GetLetter(), number);
      device.Power = Power.Off;
      device.Save();
      new Log(LogType.Information, LogAction.DeviceOff, device.ToString());
      if (OnDevicePowerOff != null)
        OnDevicePowerOff(device);
    }

    protected static void RF_OnPowerError(string groupLetter, int number, string error)
    {
      new Log(LogType.Error, LogAction.DevicePowerError, error);
    }

    protected static void _cm11_BrightenReceived(string address, int percent)
    {
      // throw new System.NotImplementedException();
    }

    protected static void _cm11_DimReceived(string address, int percent)
    {
      // throw new System.NotImplementedException();


    }

    protected static void _cm11_LogMessage(string message)
    {
      //new Log(LogType.Debug, LogAction.CM11LogMessage, message);
    }

    protected static void _cm11_OnReceived(string address)
    {
      Letter letter = address.Substring(0, 1).GetLetter();
      int number = address.Substring(1).GetInt();

      DeviceView device = Statics.DeviceViewCollection.GetDevice(letter, number);
      if (device != null)
      {
        updateDatabase(device, Power.On, LogAction.DeviceOn, "*Handbediening*");
        if (OnDevicePowerOn != null)
          OnDevicePowerOn(device);
      }
      else
        new Log(LogType.Error, LogAction.DevicePowerError, "Can't power(On) device: " + address);
    }

    protected static void _cm11_OffReceived(string address)
    {
      Letter letter = address.Substring(0, 1).GetLetter();
      int number = address.Substring(1).GetInt();

      DeviceView device = Statics.DeviceViewCollection.GetDevice(letter, number);
      if (device != null)
      {
        updateDatabase(device, Power.Off, LogAction.DeviceOff, "*Handbediening*");
        if (OnDevicePowerOff != null)
          OnDevicePowerOff(device);
      }
      else
        new Log(LogType.Error, LogAction.DevicePowerError, "Can't power(Off) device: " + address);
    }

    protected static void _cm11_Notification(string commandName, int commandParameter)
    { }

    #region IDisposable Members

    public static void Dispose()
    {
      if (_cm11 != null)
      {
        _cm11.Close();
        _cm11 = null;
      }
    }

    #endregion
  }
}
