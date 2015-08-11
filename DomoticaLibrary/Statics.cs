using Vendjuuren.SQL;
using Vendjuuren.Library.Network;
using System.Linq;

namespace Vendjuuren.Domotica.Library
{
  public class Statics
  {
    /// <summary>
    /// 
    /// </summary>
    private static DeviceViewCollection _deviceViewCollection;
    public static DeviceViewCollection DeviceViewCollection
    {
      get { return Statics._deviceViewCollection; }
      set { Statics._deviceViewCollection = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    private static ProgramCollection _programCollection;
    public static ProgramCollection ProgramCollection
    {
      get { return Statics._programCollection; }
      set { Statics._programCollection = value; }
    }
  }
}
