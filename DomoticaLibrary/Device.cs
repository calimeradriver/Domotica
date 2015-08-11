using Vendjuuren.SQL;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Vendjuuren.Domotica.Library
{
  [Serializable]
  public class Device : Record
  {
    private const string TableName = "Device";
    private const string LocationIDColumnName = "Location_ID";
    private const string ModelIDColumnName = "Model_ID";
    private const string GroupIDColumnName = "Group_ID";
    private const string NumberColumnName = "Number";
    private const string LetterColumnName = "Letter";
    private const string NameColumnName = "Name";
    private const string DescriptionColumnName = "Description";
    private const string PowerColumnName = "Power";
    private const string DefaultPowerColumnName = "DefaultPower";
    private const string PercentageColumnName = "Percentage";

    /// <summary>
    /// Serializable
    /// </summary>
    public Device()
      : base(new Vendjuuren.SQL.Table(TableName))
    { }

    /// <summary>
    /// Nieuw Device
    /// </summary>
    /// <param name="databaseConnection"></param>
    /// <param name="model"></param>
    /// <param name="group"></param>
    /// <param name="number"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    public Device(Model model, Group group, int number, string name, string description)
      : base(new Vendjuuren.SQL.Table(TableName))
    {
      Model = model;
      Group = group;
      Number = number;
      Name = name;
      DefaultPower = Power.Off;
      Power = Power.Off;
    }

    /// <summary>
    /// Bestaand Device
    /// </summary>
    /// <param name="databaseConnection"></param>
    /// <param name="row"></param>
    public Device(Row row)
      : base(new Vendjuuren.SQL.Table(TableName), row)
    { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="group"></param>
    /// <param name="number"></param>
    public void GetByLetterNumber(Group group, int number)
    {
      SELECT select = new SELECT(Table);
      select.AddWhere(new Column(GroupIDColumnName), Operator.Equals, group.ID, Vendjuuren.SQL.Boolean.AND);
      select.AddWhere(new Column(NumberColumnName), Operator.Equals, number, Vendjuuren.SQL.Boolean.AND);
      base.GetByStatement(select);
    }

    /// <summary>
    /// Delete Device
    /// </summary>
    public override void Delete()
    {
      // Verwijder eerst gekoppelde records, gekoppeld via koppel tabel
      DevProgCollection devProgCollection = new DevProgCollection();
      devProgCollection.GetAllByDeviceID(ID);
      devProgCollection.DeleteAll();

      base.Delete();

      // Verwijder; direct gekoppelde records
      if (Group != null)
        Group.TryDelete();
    }

    ///// <summary>
    ///// Power device On
    ///// </summary>
    //public void PowerOn(string logDetails)
    //{
    //  DeviceHelper.PowerOn(this, logDetails);
    //}

    ///// <summary>
    ///// Power device Off
    ///// </summary>
    //public void PowerOff(string logDetails)
    //{
    //  DeviceHelper.PowerOff(this, logDetails);
    //}

    ///// <summary>
    ///// Toggle power state of device
    ///// </summary>
    //public void PowerToggle(string logDetails)
    //{
    //  DeviceHelper.PowerToggle(this, logDetails);
    //}

    /// <summary>
    /// Address of Device e.g. A1
    /// </summary>
    [XmlIgnore]
    public string Address
    {
      get { return (Group.ToString() + Number); }
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
    /// LocationID
    /// </summary>
    [JsonProperty()]
    public Guid LocationID
    {
      get { return (Row[LocationIDColumnName].GetGuid()); }
      set { Row[LocationIDColumnName] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    private Location _location;
    [XmlIgnore]
    public Location Location
    {
      get
      {
        if (_location == null)
        {
          _location = new Location();
          _location.GetByID(LocationID);
        }

        return (_location);
      }
      set
      {
        _location = value;
        LocationID = _location.ID;
      }
    }

    [JsonProperty()]
    public string LocationSummary
    {
      get
      {
        return (Location != null ? Location.ToString() : string.Empty);
      }
    }

    /// <summary>
    /// ModelID
    /// </summary>
    [JsonProperty()]
    public Guid ModelID
    {
      get { return (Row[ModelIDColumnName].GetGuid()); }
      set { Row[ModelIDColumnName] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    private Model _model;
    [XmlIgnore]
    public Model Model
    {
      get
      {
        if (_model == null)
        {
          _model = new Model();
          _model.GetByID(ModelID);
        }

        return (_model);
      }
      set
      {
        _model = value;
        ModelID = _model.ID;
      }
    }

    [JsonProperty()]
    public string Model_Type
    {
      get
      {
        return (Model != null ? Model.Type : string.Empty);
      }
    }

    /// <summary>
    /// GroupID
    /// </summary>
    [JsonProperty()]
    public Guid GroupID
    {
      get { return (Row[GroupIDColumnName].GetGuid()); }
      set { Row[GroupIDColumnName] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    private Group _group;
    [XmlIgnore]
    public Group Group
    {
      get
      {
        if (_group == null)
        {
          _group = new Group();
          _group.GetByID(GroupID);
        }

        return (_group);
      }
      set
      {
        _group = value;
        GroupID = _group.ID;
      }
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
    public string GroupNumberSummary
    {
      get
      {
        return (Group != null ? Group.ToString() + Number : string.Empty);
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
    /// Power
    /// </summary>
    [JsonProperty()]
    public Power Power
    {
      get { return (Row[PowerColumnName].GetPower()); }
      set { Row[PowerColumnName] = value; }
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
  }
}
