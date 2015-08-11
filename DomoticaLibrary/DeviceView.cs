using Vendjuuren.SQL;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Vendjuuren.Domotica.Library
{
  [Serializable]
  public class DeviceView : Record
  {
    private const string TableName = "Device_View";
    private const string NameColumnName = "Name";
    private const string DescriptionColumnName = "Description";
    private const string LocationColumnName = "Location";
    private const string BrandColumnName = "Brand";
    private const string ModelColumnName = "Model";
    private const string TypeColumnName = "Type";
    private const string TwoWayColumnName = "TwoWay";
    private const string DimmableColumnName = "Dimmable";
    private const string GroupColumnName = "Group";
    private const string NumberColumnName = "Number";
    private const string AddressColumnName = "Address";
    private const string DefaultPowerColumnName = "DefaultPower";
    private const string PowerColumnName = "Power";
    private const string PercentageColumnName = "Percentage";

    /// <summary>
    /// Serializable
    /// </summary>
    public DeviceView()
      : base(new Vendjuuren.SQL.Table(TableName))
    { }

    /// <summary>
    /// Bestaand DeviceView
    /// </summary>
    /// <param name="databaseConnection"></param>
    /// <param name="row"></param>
    public DeviceView(Row row)
      : base(new Vendjuuren.SQL.Table(TableName), row)
    { }

    /// <summary>
    /// Power device On
    /// </summary>
    public void PowerOn(string logDetails)
    {
      DeviceHelper.PowerOn(this, logDetails);
    }

    /// <summary>
    /// Power device Off
    /// </summary>
    public void PowerOff(string logDetails)
    {
      DeviceHelper.PowerOff(this, logDetails);
    }

    /// <summary>
    /// Toggle power state of device
    /// </summary>
    public void PowerToggle(string logDetails)
    {
      DeviceHelper.PowerToggle(this, logDetails);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return (Name + " (" + Group.ToString() + Number + ")");
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
    /// Location
    /// </summary>
    [JsonProperty()]
    public string Location
    {
      get { return (Row[LocationColumnName].ToString()); }
      set { Row[LocationColumnName] = value; }
    }

    /// <summary>
    /// Brand
    /// </summary>
    [JsonProperty()]
    public string Brand
    {
      get { return (Row[BrandColumnName].ToString()); }
      set { Row[BrandColumnName] = value; }
    }

    /// <summary>
    /// Model
    /// </summary>
    [JsonProperty()]
    public string Model
    {
      get { return (Row[ModelColumnName].ToString()); }
      set { Row[ModelColumnName] = value; }
    }

    [JsonProperty()]
    public BrandType Type
    {
      get { return (Row[TypeColumnName].GetBrandType()); }
      set { Row[TypeColumnName] = value; }
    }

    [JsonProperty()]
    public bool TwoWay
    {
      get { return (Row[TwoWayColumnName].GetBool()); }
      set { Row[TwoWayColumnName] = value; }
    }

    [JsonProperty()]
    public bool Dimmable
    {
      get { return (Row[DimmableColumnName].GetBool()); }
      set { Row[DimmableColumnName] = value; }
    }

    /// <summary>
    /// Group
    /// </summary>
    [JsonProperty()]
    public string Group
    {
      get { return (Row[GroupColumnName].ToString()); }
      set { Row[GroupColumnName] = value; }
    }

    /// <summary>
    /// Number
    /// </summary>
    [JsonProperty()]
    public int Number
    {
      get { return (Row[NumberColumnName].GetInt()); }
      set { Row[NumberColumnName] = value; }
    }

    [JsonProperty()]
    public string Address
    {
      get { return (Row[AddressColumnName].ToString()); }
      set { Row[AddressColumnName] = value; }
    }

    /// <summary>
    /// Default Power
    /// </summary>
    public Power DefaultPower
    {
      get { return (Row[DefaultPowerColumnName].GetPower()); }
      set { Row[DefaultPowerColumnName] = value; }
    }

    /// <summary>
    /// Percentage on
    /// </summary>
    [JsonProperty()]
    public int Percentage
    {
      get { return (Row[DefaultPowerColumnName].GetInt()); }
      set { Row[DefaultPowerColumnName] = value; }
    }

    /// <summary>
    /// Power
    /// </summary>
    [JsonProperty()]
    public Power Power
    {
      get { return (Row[PowerColumnName].GetPower()); }
      set { Row[PowerColumnName] = value; }
    }
  }
}
