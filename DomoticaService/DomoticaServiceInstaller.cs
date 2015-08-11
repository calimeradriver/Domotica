using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Vendjuuren.Domotica.Service
{
  [RunInstaller(true)]
  public class DomoticaServiceInstaller : Installer
  {
    public DomoticaServiceInstaller()
    {
      var processInstaller = new ServiceProcessInstaller();
      var serviceInstaller = new ServiceInstaller();

      //set the privileges
      processInstaller.Account = ServiceAccount.LocalSystem;

      serviceInstaller.DisplayName = "Domotica";
      serviceInstaller.StartType = ServiceStartMode.Automatic;
      serviceInstaller.Description = "Domotica Service to control home automation";

      //must be the same as what was set in Program's constructor
      serviceInstaller.ServiceName = "DomoticaService";

      this.Installers.Add(processInstaller);
      this.Installers.Add(serviceInstaller);
    }
  }
}
