using System;
using System.ServiceProcess;
using System.Threading;
using Vendjuuren.Domotica.Service.Properties;
using Vendjuuren.Domotica.Library;
using Vendjuuren.SQL;
using Vendjuuren.Library.Network;
using Vendjuuren.Domotica.Service.State;

namespace Vendjuuren.Domotica.Service
{
  class Program : ServiceBase
  {
    static void Main(string[] args)
    {
      ServiceBase.Run(new Program());
    }

    public Program()
    {
      this.ServiceName = "DomoticaService";
    }

    protected override void OnStart(string[] args)
    {
      base.OnStart(args);

      StateManager.ApplicationInfo = new ApplicationInfo();

      new Log(LogType.Information,
        LogAction.ServiceStarted, "ServiceName: " + ServiceName);
    }

    protected override void OnStop()
    {
      base.OnStop();

      new Log(LogType.Information,
        LogAction.ServiceStopped, "ServiceName: " + ServiceName);

      StateManager.ApplicationInfo.Dispose();
    }
  }
}
