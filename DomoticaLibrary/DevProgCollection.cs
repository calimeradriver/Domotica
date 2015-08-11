using Vendjuuren.SQL;
using System;

namespace Vendjuuren.Domotica.Library
{
  public class DevProgCollection : RecordCollection
  {
    private const string TableName = "DevProg";
    private const string DeviceIDColumnName = "Device_ID";
    private const string ProgramIDColumnName = "Program_ID";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="databaseConnection"></param>
    public DevProgCollection()
      : base(new Vendjuuren.SQL.Table(TableName))
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// TODO: 1 algemene method maken
    /// </remarks>
    /// <param name="id"></param>
    public void GetAllByProgramID(Guid id)
    {
      SELECT select = new SELECT(Table);
      select.AddWhere(new Column(ProgramIDColumnName), Operator.Equals, id, Vendjuuren.SQL.Boolean.AND);
      base.GetAllByStatement(select);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// TODO: 1 algemene method maken
    /// </remarks>
    /// <param name="id"></param>
    public void GetAllByDeviceID(Guid id)
    {
      SELECT select = new SELECT(Table);
      select.AddWhere(new Column(DeviceIDColumnName), Operator.Equals, id, Vendjuuren.SQL.Boolean.AND);
      base.GetAllByStatement(select);
    }

    /// <summary>
    /// 
    /// </summary>
    public override void BuildCollection()
    {
      foreach (Row row in Rows)
        Add(new DevProg(row));
    }
  }
}
