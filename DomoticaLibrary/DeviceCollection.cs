using Vendjuuren.SQL;
using System.Linq;
using System.Collections.Generic;

namespace Vendjuuren.Domotica.Library
{
  public class DeviceCollection : RecordCollection
  {
    private const string TableName = "Device";

    public DeviceCollection()
      : base(new Vendjuuren.SQL.Table(TableName))
    { }

    /// <summary>
    /// Get Device from collection; queried with Linq
    /// </summary>
    /// <param name="groupLetter"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    public Device GetDevice(Letter groupLetter, int number)
    {
      Device[] deviceArray = new Device[this.Count];
      this.CopyTo(deviceArray);

      var device =
        from d in deviceArray
        where d.Group.Letter == groupLetter && d.Number == number
        select d;

      return (device.SingleOrDefault() as Device);
    }

    /// <summary>
    /// 
    /// </summary>
    public override void BuildCollection()
    {
      foreach (Row row in Rows)
        Add(new Device(row));
    }

    /// <summary>
    /// Delete
    /// </summary>
    public override void DeleteAll()
    {
      foreach (Device device in this)
        device.Delete();
    }
  }
}