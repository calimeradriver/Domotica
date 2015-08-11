using Vendjuuren.SQL;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Vendjuuren.Domotica.Library
{
  [Serializable]
  public class Floor : Record
  {
    private const string TableName = "Floor";
    private const string NameColumnName = "Name";
    private const string DescriptionColumnName = "Description";
    private const string OrderColumnName = "Order";

    /// <summary>
    /// Serializable
    /// </summary>
    public Floor()
      : base(new Vendjuuren.SQL.Table(TableName))
    { }

    /// <summary>
    /// Nieuwe Floor
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    public Floor(string name, string description)
      : base(new Vendjuuren.SQL.Table(TableName))
    {
      Name = name;
      Description = description;
    }

    /// <summary>
    /// Bestaande Floor
    /// </summary>
    /// <param name="databaseConnection"></param>
    /// <param name="row"></param>
    public Floor(Row row)
      : base(new Vendjuuren.SQL.Table(TableName), row)
    { }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return (Name);
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