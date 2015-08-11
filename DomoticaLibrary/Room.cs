using Vendjuuren.SQL;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Vendjuuren.Domotica.Library
{
  [Serializable]
  public class Room : Record
  {
    private const string TableName = "Room";
    private const string FloorIDColumnName = "Floor_ID";
    private const string NameColumnName = "Name";
    private const string DescriptionColumnName = "Description";
    private const string OrderColumnName = "Order";

    /// <summary>
    /// Serializable
    /// </summary>
    public Room()
      : base(new Vendjuuren.SQL.Table(TableName))
    { }

    /// <summary>
    /// Nieuwe Room
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    public Room(string name, string description)
      : base(new Vendjuuren.SQL.Table(TableName))
    {
      Name = name;
      Description = description;
    }

    /// <summary>
    /// Bestaande Room
    /// </summary>
    /// <param name="databaseConnection"></param>
    /// <param name="row"></param>
    public Room(Row row)
      : base(new Vendjuuren.SQL.Table(TableName), row)
    { }

    /// <summary>
    /// Delete Device
    /// </summary>
    public override void Delete()
    {
      base.Delete();

      // Verwijder; direct gekoppelde records
      if (Floor != null)
        Floor.TryDelete();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return (Name + ", " + Floor.ToString());
    }

    /// <summary>
    /// FloorID
    /// </summary>
    [JsonProperty()]
    public Guid FloorID
    {
      get { return (Row[FloorIDColumnName].GetGuid()); }
      set { Row[FloorIDColumnName] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    private Floor _floor;
    [XmlIgnore]
    public Floor Floor
    {
      get
      {
        if (_floor == null)
        {
          _floor = new Floor();
          _floor.GetByID(FloorID);
        }

        return (_floor);
      }
      set
      {
        _floor = value;
        FloorID = _floor.ID;
      }
    }

    /// <summary>
    /// Name
    /// </summary>
    [JsonProperty()]
    public string Name
    {
      get { return (Row[NameColumnName].GetString()); }
      set { Row[NameColumnName] = value; }
    }

    /// <summary>
    /// Description
    /// </summary>
    [JsonProperty()]
    public string Description
    {
      get { return (Row[DescriptionColumnName].GetString()); }
      set { Row[DescriptionColumnName] = value; }
    }

    /// <summary>
    /// Order
    /// </summary>
    [JsonProperty()]
    public int Order
    {
      get { return (Row[OrderColumnName].GetInt()); }
      set { Row[OrderColumnName] = value; }
    }
  }
}