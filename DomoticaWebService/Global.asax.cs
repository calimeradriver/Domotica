using System;
using System.Threading;
using Vendjuuren.Domotica.Library;
using Vendjuuren.Domotica.WebService.State;
using System.IO.Ports;
using DwellNet;
using Vendjuuren.SQL;
using Vendjuuren.Library;
using Vendjuuren.Domotica.X10;


namespace Vendjuuren.Domotica.WebService
{
  /// <summary>
  /// Bron timer: http://efreedom.com/Question/1-3447266/Webservice-WCF-Timer-Update-Engine
  /// </summary>
  public class Global : System.Web.HttpApplication
  {
    protected void Application_Start(object sender, EventArgs e)
    {
      StateManager.ApplicationInfo = new ApplicationInfo();
      new Log(LogType.Information,
        LogAction.ApplicationStarted, string.Empty);

      // Bind Cm11 X10 events
      //StateManager.ApplicationInfo.Cm11.Notification += new Cm11LowLevelNotificationEventDelegate(cm11_Notification);
      //StateManager.ApplicationInfo.Cm11.OffReceived += new Cm11DeviceNotificationEventDelegate(cm11_OffReceived);
      //StateManager.ApplicationInfo.Cm11.OnReceived += new Cm11DeviceNotificationEventDelegate(cm11_OnReceived);
      //StateManager.ApplicationInfo.Cm11.LogMessage += new Cm11LogMessageEventDelegate(cm11_LogMessage);
      //StateManager.ApplicationInfo.Cm11.DimReceived += new Cm11BrightenOrDimNotificationEventDelegate(cm11_DimReceived);
      //StateManager.ApplicationInfo.Cm11.BrightenReceived += new Cm11BrightenOrDimNotificationEventDelegate(cm11_BrightenReceived);
    }

    void cm11_BrightenReceived(string address, int percent)
    {
      //Log log = new Log(databaseConnection, LogType.Test,
      //  LogAction.DeviceBright, "X10 Device: " + address + "; " + percent + "%");
    }

    void cm11_DimReceived(string address, int percent)
    {
      //Log log = new Log(databaseConnection, LogType.Test,
      //  LogAction.DeviceDim, "X10 Device: " + address + "; " + percent + "%");
    }

    void cm11_LogMessage(string message)
    { }

    void cm11_OnReceived(string address)
    {
      //Log log = new Log(databaseConnection, LogType.Test,
      //  LogAction.DeviceOn, "X10 Device: " + address);
    }

    void cm11_OffReceived(string address)
    {
      //Log log = new Log(databaseConnection, LogType.Test,
      //  LogAction.DeviceOff, "X10 Device: " + address);
    }

    void cm11_Notification(string commandName, int commandParameter)
    { }

    protected void Session_Start(object sender, EventArgs e)
    { }

    protected void Application_BeginRequest(object sender, EventArgs e)
    {
      

      DeviceCollection deviceCollection = new DeviceCollection();
      deviceCollection.GetAll();

      ProgramCollection programCollection = new ProgramCollection();
      programCollection.GetAll();

      //Scheduler scheduler = new Scheduler(StateManager.ApplicationInfo.DatabaseConnection, programCollection);
      //scheduler.Start();

      //DeviceList devices = new DeviceList(StateManager.ApplicationInfo.DatabaseConnection);
      //devices.GetAll();

      //Device device = new Device(StateManager.ApplicationInfo.DatabaseConnection);
      //device.GetByID(new Guid("06B61A32-554C-4992-B4A6-F718BA988F38"));
      //Group group = device.Group;
      //device.Power = Power.Off;
      //device.Save();

      //TestRecord newTestRecord = new TestRecord(StateManager.ApplicationInfo.DatabaseConnection, group, 1, "", "");
      //newTestRecord.Save();


      //Group group = new Group(StateManager.ApplicationInfo.DatabaseConnection);
      //group.GetByLetter(Letter.C);

      //Device device = new Device(StateManager.ApplicationInfo.DatabaseConnection);
      //device.GetByLetterNumber(group, 120);
      //Kaku.PowerDeviceOn(StateManager.ApplicationInfo.DatabaseConnection, device);
    }

    protected void Application_AuthenticateRequest(object sender, EventArgs e)
    {

    }

    protected void Application_Error(object sender, EventArgs e)
    {
      Exception ex = sender as Exception;
      if (ex != null)
      {
        new Log(LogType.Error,
          LogAction.Exception, ex.Message);
      }
      else
      {
        new Log(LogType.Error,
          LogAction.Exception, sender.ToString());
      }
    }

    protected void Session_End(object sender, EventArgs e)
    {

    }

    protected void Application_End(object sender, EventArgs e)
    {
      if (StateManager.ApplicationInfo != null)
      {
        new Log(LogType.Information,
          LogAction.ApplicationEnded, string.Empty);
        StateManager.ApplicationInfo.Dispose();
      }
    }
  }
}