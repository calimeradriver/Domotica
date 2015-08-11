using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vendjuuren.Domotica.Service.State
{
  public class StateManager
  {
    private static ApplicationInfo _applicationInfo;
    /// <summary>
    /// ApplicationInfo object containing all application data
    /// </summary>
    public static ApplicationInfo ApplicationInfo
    {
      get
      {
        return _applicationInfo;
      }
      set
      {
        _applicationInfo = value;
      }
    }
  }
}
