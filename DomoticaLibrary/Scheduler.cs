using System;
using System.Threading;
using Vendjuuren.SQL;
using Newtonsoft.Json;

namespace Vendjuuren.Domotica.Library
{
  public class Scheduler
  {
    public Scheduler(ProgramCollection programCollection)
    {
      _programCollection = programCollection;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
      TimeSpan updateTimerFrequency = TimeSpan.FromSeconds(60);
      _timer = new Timer(timerElapsed, null, updateTimerFrequency, updateTimerFrequency);
      new Log(LogType.Information,
        LogAction.SchedulerStarted, "");
    }

    /// <summary>
    /// 
    /// </summary>
    public void Stop()
    {
      stop(string.Empty);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    private void stop(string message)
    {
      if (_timer != null)
        _timer.Dispose();
      new Log(LogType.Information,
        LogAction.SchedulerStopped, message);
    }

    private void timerElapsed(object state)
    {
      try
      {
        if (_programCollection == null || _programCollection.Count == 0)
        {
          stop("ProgramCollection is null or empty.");
          return;
        }

        DateTime dateTime = DateTime.Now;

        foreach (Vendjuuren.Domotica.Library.Program program in _programCollection)
        {
          if (program.Enabed)
          {
            if (program.DateRange.Validate(dateTime))
            {
              switch (program.ScheduleStartMode)
              {
                case ProgramScheduleMode.Time:
                  timeMode(dateTime, program);
                  break;
                case ProgramScheduleMode.Sun:
                  sunMode(dateTime, program);
                  break;
                case ProgramScheduleMode.Random:
                  randomMode();
                  break;
                case ProgramScheduleMode.Undefined:
                  new Log(LogType.Warning,
        LogAction.SchedulerException, program.Name + " ScheduleMode is undefined.");
                  break;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        new Log(LogType.Error,
        LogAction.SchedulerException, ex.Message);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    private void timeMode(DateTime dateTime, Program program)
    {
      // Power device on
      if (dateTime.Hour == program.StartTime.Value.Hour && dateTime.Minute == program.StartTime.Value.Minute)
        program.PowerDevicesOn();

      // Power device off
      if (dateTime.Hour == program.EndTime.Value.Hour && dateTime.Minute == program.EndTime.Value.Minute)
        program.PowerDevicesOff();
    }

    /// <summary>
    /// 
    /// </summary>
    private void sunMode(DateTime dateTime, Program program)
    {
      if (_sun == null)
        _sun = new Sun();

      _sun.Resolve();

      // Power device on
      if (dateTime.Hour == _sun.Sunset.Hour && dateTime.Minute == _sun.Sunset.Minute)
        program.PowerDevicesOn();

      // Power device off
      if (dateTime.Hour == program.EndTime.Value.Hour && dateTime.Minute == program.EndTime.Value.Minute)
        program.PowerDevicesOff();
    }

    /// <summary>
    /// 
    /// </summary>
    private void randomMode()
    {
      throw new NotImplementedException();
    }

    private Timer _timer;
    private ProgramCollection _programCollection;
    private Sun _sun;
  }
}