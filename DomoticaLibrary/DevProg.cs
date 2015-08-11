using System;
using Vendjuuren.SQL;

namespace Vendjuuren.Domotica.Library
{
  [Serializable]
  public class DevProg : Record
  {
    private const string TableName = "DevProg";
    private const string DeviceIDColumnName = "Device_ID";
    private const string ProgramIDColumnName = "Program_ID";

    /// <summary>
    /// Serializable
    /// </summary>
    public DevProg()
      : base()
    { }

    /// <summary>
    /// Nieuw DevProg
    /// </summary>
    /// <param name="databaseConnection"></param>
    /// <param name="device"></param>
    /// <param name="program"></param>
    public DevProg(Device device, Program program)
      : base(new Vendjuuren.SQL.Table(TableName))
    {
      DevID = device.ID;
      ProgID = program.ID;
    }

    /// <summary>
    /// Bestaand DevProg
    /// </summary>
    /// <param name="databaseConnection"></param>
    /// <param name="row"></param>
    public DevProg(Row row)
      : base(new Vendjuuren.SQL.Table(TableName), row)
    { }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return (DevID + "<-->" + ProgID);
    }

    /// <summary>
    /// 
    /// </summary>
    public Guid DevID
    {
      get { return (Row[DeviceIDColumnName].GetGuid()); }
      set { Row[DeviceIDColumnName] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public Guid ProgID
    {
      get { return (Row[ProgramIDColumnName].GetGuid()); }
      set { Row[ProgramIDColumnName] = value; }
    }
  }
}
