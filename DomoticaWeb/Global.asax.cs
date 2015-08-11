using System;
using Vendjuuren.Domotica.Web.State;
using Vendjuuren.SQL;

namespace DomoticaWeb
{
  public class Global : System.Web.HttpApplication
  {

    protected void Application_Start(object sender, EventArgs e)
    {
      StateManager.ApplicationInfo = new ApplicationInfo();
      new Log(LogType.Information,
        LogAction.ApplicationStarted, string.Empty);
    }

    protected void Session_Start(object sender, EventArgs e)
    {

    }

    protected void Application_BeginRequest(object sender, EventArgs e)
    {

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

    }
  }
}