using System;
using System.Data.SqlTypes;

namespace Vendjuuren.Domotica.Library
{
  public class Range
  {
    /// <summary>
    /// DateTime Range
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    public Range(DateTime startDate, DateTime endDate)
    {
      _startDate = startDate;
      _endDate = endDate;
    }

    /// <summary>
    /// New, Good
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public bool Validate(DateTime dateTime)
    {
       return (dateTime >= _startDate && dateTime <= _endDate);
    }

    ///// <summary>
    ///// New, Good
    ///// </summary>
    ///// <param name="dateTime"></param>
    ///// <returns></returns>
    //public bool Validate(DateTime dateTime)
    //{
    //  return ((dateTime.Month >= _startDate.Month && dateTime.Month <= _endDate.Month) &&
    //   (dateTime.Day >= _startDate.Day && dateTime.Day <= _endDate.Day));
    //}

    /// <summary>
    /// Old, Bad
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    //public bool Validate(DateTime dateTime)
    //{
    //  return ((dateTime.Month >= _startDate.Month && dateTime.Day >= _startDate.Day) ||
    //        (dateTime.Month <= _endDate.Month && dateTime.Day <= _endDate.Day));
    //}

    private DateTime _startDate;
    public DateTime StartDate
    {
      get { return _startDate; }
      //set { _startDate = value; }
    }

    private DateTime _endDate;
    public DateTime EndDate
    {
      get { return _endDate; }
      //set { _endDate = value; }
    }
  }
}
