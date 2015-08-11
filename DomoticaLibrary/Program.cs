using System;
using System.Collections.Generic;
using Vendjuuren.SQL;
using Vendjuuren.Library;

namespace Vendjuuren.Domotica.Library
{
  [Serializable]
  public class Program : Record
  {
    private const string TableName = "Program";
    private const string ScheduleStartModeColumnName = "ScheduleStartMode";
    private const string ScheduleStopModeColumnName = "ScheduleStopMode";
    private const string NameColumnName = "Name";
    private const string DescriptionColumnName = "Description";
    private const string EnabledColumnName = "Enabled";

    // Start columns
    private const string StartDayColumnName = "StartDay";
    private const string StartMonthColumnName = "StartMonth";
    private const string StartTimeColumnName = "StartTime";

    // End columns
    private const string EndDayColumnName = "EndDay";
    private const string EndMonthColumnName = "EndMonth";
    private const string EndTimeColumnName = "EndTime";

    /// <summary>
    /// Serializable
    /// </summary>
    public Program()
      : base(new Vendjuuren.SQL.Table(TableName))
    { }

    /// <summary>
    /// Nieuw Program
    /// </summary>
    /// <param name="databaseConnection"></param>
    /// <param name="scheduleStartMode"></param>
    /// <param name="scheduleStopMode"></param>
    /// <param name="enabled"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    public Program(ProgramScheduleMode scheduleStartMode, ProgramScheduleMode scheduleStopMode, bool enabled, string name, string description, DateTime startDate,
      DateTime endDate, DateTime? startTime, DateTime? endTime)
      : base(new Vendjuuren.SQL.Table(TableName))
    {
      ScheduleStartMode = scheduleStartMode;
      ScheduleStopMode = scheduleStopMode;
      Enabed = enabled;
      Name = name;
      Description = description;
      StartDate = startDate;
      EndDate = endDate;
      StartTime = startTime;
      EndTime = endTime;
    }

    /// <summary>
    /// Bestaand Program
    /// </summary>
    /// <param name="databaseConnection"></param>
    /// <param name="row"></param>
    public Program(Row row)
      : base(new Vendjuuren.SQL.Table(TableName), row)
    { }

    /// <summary>
    /// Delete Program
    /// </summary>
    public override void Delete()
    {
      // Verwijder eerst gekoppelde records, gekoppeld via koppel tabel
      DevProgCollection devProgCollection = new DevProgCollection();
      devProgCollection.GetAllByProgramID(ID);
      devProgCollection.DeleteAll();

      base.Delete();
    }

    /// <summary>
    /// 
    /// </summary>
    private void setRandomStartEndTime()
    {
      DateTime minDT = DateTime.Parse("17:00");
      DateTime maxDT = DateTime.Parse("23:30");
      DateTime ranDT = RandomProvider.Next(minDT, maxDT);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="device"></param>
    public void AddDevice(Device device)
    {
      DevProg devProg = new DevProg(device, this);
      devProg.Save();

      if (_deviceCollection != null)
        _deviceCollection.Add(device);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="device"></param>
    public void RemoveDevice(Device device)
    {
      // Verwijder koppel records
      DevProgCollection devProgCollection = new DevProgCollection();
      devProgCollection.GetAllByDeviceID(device.ID);
      devProgCollection.DeleteAll();

      if (_deviceCollection != null)
        _deviceCollection.Remove(device);
    }

    /// <summary>
    /// 
    /// </summary>
    public void PowerDevicesOn()
    {
      foreach (DeviceView device in DeviceCollection)
        device.PowerOn(Name);
    }

    /// <summary>
    /// 
    /// </summary>
    public void PowerDevicesOff()
    {
      foreach (DeviceView device in DeviceCollection)
        device.PowerOff(Name);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return (Name + " (" + Description + ")");
    }

    /// <summary>
    /// 
    /// </summary>
    public ProgramScheduleMode ScheduleStartMode
    {
      get { return (Row[ScheduleStartModeColumnName].GetScheduleMode()); }
      set { Row[ScheduleStartModeColumnName] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public ProgramScheduleMode ScheduleStopMode
    {
      get { return (Row[ScheduleStopModeColumnName].GetScheduleMode()); }
      set { Row[ScheduleStopModeColumnName] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool Enabed
    {
      get { return (Row[EnabledColumnName].GetBool()); }
      set { Row[EnabledColumnName] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Name
    {
      get { return (Row[NameColumnName].GetString()); }
      set { Row[NameColumnName] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string Description
    {
      get { return (Row[DescriptionColumnName].GetString()); }
      set { Row[DescriptionColumnName] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public Range DateRange
    {
      get { return StartDate != null && EndDate != null ? new Range(StartDate, EndDate) : null; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime? StartTime
    {
      get { return (Row[StartTimeColumnName].GetDateTime()); }
      set { Row[StartTimeColumnName] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime? EndTime
    {
      get { return (Row[EndTimeColumnName].GetDateTime()); }
      set { Row[EndTimeColumnName] = value; }
    }


    private void makeDateTimeSqlValid()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// If you want the last day of the month, set value to '0'
    /// </remarks>
    private int _startDay
    {
      get { return (Row[StartDayColumnName].GetInt()); }
      set { Row[StartDayColumnName] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    private int _startMonth
    {
      get { return (Row[StartMonthColumnName].GetInt()); }
      set { Row[StartMonthColumnName] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// /// <remarks>
    /// If you want the last day of the month, set value to '0'
    /// </remarks>
    private int _endDay
    {
      get { return (Row[EndDayColumnName].GetInt()); }
      set { Row[EndDayColumnName] = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    private int _endMonth
    {
      get { return (Row[EndMonthColumnName].GetInt()); }
      set { Row[EndMonthColumnName] = value; }
    }


    /// <summary>
    /// 
    /// </summary>
    public DateTime StartDate
    {
      get { return (DateTimeHelper.GetDateTime(_startMonth, _startDay)); }
      set
      {
        _startDay = (DateTimeHelper.IsLastDayOfMonth(value.Month, value.Day) ? 0 : value.Day);
        _startMonth = value.Month;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime EndDate
    {
      get { return (DateTimeHelper.GetDateTime(_endMonth, _endDay)); }
      set
      {
        _endDay = (DateTimeHelper.IsLastDayOfMonth(value.Month, value.Day) ? 0 : value.Day);
        _endMonth = value.Month;
      }
    }

    /// <summary>
    /// Get mounted devices, lazy loading.
    /// </summary>
    private DeviceCollection _deviceCollection;
    public DeviceCollection DeviceCollection
    {
      get
      {
        // TODO: onderstaande geeft problemen indien het omgezet wordt naar 'DeviceView', op een gegeven moment blijft resultaat van alle SQL query's leeg.
        if (_deviceCollection == null)
        {
          DevProgCollection devProgCollection = new DevProgCollection();
          devProgCollection.GetAllByProgramID(ID);
          _deviceCollection = new DeviceCollection();
          foreach (DevProg devProg in devProgCollection)
          {
            // TODO: 1 SQL query maken
            Device device = new Device();
            device.GetByID(devProg.DevID);
            _deviceCollection.Add(device);
          }
        }
        return _deviceCollection;
      }
    }
  }
}