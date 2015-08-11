using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vendjuuren.Domotica.WebService.State
{
  public class StateManager
  {
    private const string applicationId = "KakuApplicationInfo";

    /// <summary>
    /// ApplicationInfo object containing all application data
    /// </summary>
    public static ApplicationInfo ApplicationInfo
    {
      get
      {
        return HttpContext.Current.Application[applicationId] as ApplicationInfo;
      }
      set
      {
        HttpContext.Current.Application[applicationId] = value;
      }
    }
  }
}
