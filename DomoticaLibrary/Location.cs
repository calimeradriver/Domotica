using Vendjuuren.SQL;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Vendjuuren.Domotica.Library
{
  [Serializable]
  public class Location : Record
  {
    private const string TableName = "Location";
    private const string RoomIDColumnName = "Room_ID";
    private const string NameColumnName = "Name";
    private const string DescriptionColumnName = "Description";
    private const string OrderColumnName = "Order";

    /// <summary>
    /// Serializable
    /// </summary>
    public Location()
      : base(new Vendjuuren.SQL.Table(TableName))
    { }

    /// <summary>
    /// Nieuwe Location
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    public Location(string name, string description)
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
    public Location(Row row)
      : base(new Vendjuuren.SQL.Table(TableName), row)
    { }

    /// <summary>
    /// Delete Device
    /// </summary>
    public override void Delete()
    {
      base.Delete();

      // Verwijder; direct gekoppelde records
      if (Room != null)
        Room.TryDelete();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return (Name + " (" + Room.ToString() + ")");
    }

    /// <summary>
    /// FloorID
    /// </summary>
    [JsonProperty()]
    public Guid RoomID
    {
      get { return (Row[RoomIDColumnName].GetGuid()); }
      set { Row[RoomIDColumnName] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    private Room _room;
    [XmlIgnore]
    public Room Room
    {
      get
      {
        if (_room == null)
        {
          _room = new Room();
          _room.GetByID(RoomID);
        }

        return (_room);
      }
      set
      {
        _room = value;
        RoomID = _room.ID;
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