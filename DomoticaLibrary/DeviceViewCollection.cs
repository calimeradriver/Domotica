using Vendjuuren.SQL;
using System.Linq;
using System.Collections.Generic;

namespace Vendjuuren.Domotica.Library
{
  public class DeviceViewCollection : RecordCollection
  {
    private const string TableName = "Device_View";

    public DeviceViewCollection()
      : base(new Vendjuuren.SQL.Table(TableName))
    { }

    /// <summary>
    /// Get DeviceView from collection; queried with Linq
    /// </summary>
    /// <param name="groupLetter"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    public DeviceView GetDevice(Letter groupLetter, int number)
    {
      DeviceView[] deviceViewArray = new DeviceView[this.Count];
      this.CopyTo(deviceViewArray);

      var device =
        from d in deviceViewArray
        where d.Group == groupLetter.ToString() && d.Number == number
        select d;

      return (device.SingleOrDefault() as DeviceView);
    }

    /// <summary>
    /// 
    /// </summary>
    public override void BuildCollection()
    {
      foreach (Row row in Rows)
        Add(new DeviceView(row));
    }

    /// <summary>
    /// Delete
    /// </summary>
    public override void DeleteAll()
    {
      foreach (DeviceView deviceView in this)
        deviceView.Delete();
    }
  }
}