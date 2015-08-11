using System.Runtime.InteropServices;
using System.Threading;

namespace Vendjuuren.Domotica.Radiographically
{
  public class RF
  {
    public delegate void DevicePowerDelegate(string groupLetter, int number);
    public static event DevicePowerDelegate PoweredOn;
    public static event DevicePowerDelegate PoweredOff;

    public delegate void DevicePowerErrorDelegate(string groupLetter, int number, string error);
    public static event DevicePowerErrorDelegate OnPowerError;

    [DllImport("TPC200L10.dll")]
    private static extern int Send(byte Scode, byte OnOff);

    /// <summary>
    /// Power Device On
    /// </summary>
    /// <param name="groupLetter"></param>
    /// <param name="number"></param>
    public static void PowerDeviceOn(string groupLetter, int number)
    {
      send(groupLetter, number, 1);
    }

    /// <summary>
    /// Power Device Off
    /// </summary>
    /// <param name="groupLetter"></param>
    /// <param name="number"></param>
    public static void PowerDeviceOff(string groupLetter, int number)
    {
      send(groupLetter, number, 0);
    }

    /// <summary>
    /// Send command to the receiver
    /// </summary>
    /// <param name="sendCode">Kaku calculated send code</param>
    /// <param name="power">On or Off</param>
    /// <returns>1 is OK</returns>
    private static void send(string groupLetter, int number, int power)
    {
      SendCode sendCode = new SendCode(groupLetter, number);

      if (Send((byte)sendCode.TransmitCode, (byte)power) == 1)
      {
        if (power == 1)
        {
          if (PoweredOn != null)
            PoweredOn(groupLetter, number);
        }
        else
        {
          // Send off command again, just to be sure that the device is turned off.
          Thread.Sleep(10);
          Send((byte)sendCode.TransmitCode, (byte)power);
          if (PoweredOff != null)
            PoweredOff(groupLetter, number);
        }
      }
      else
      {
        if (OnPowerError != null)
          OnPowerError(groupLetter, number, "Can't power(On/Off) device: " + groupLetter + number);
      }
    }
  }
}
