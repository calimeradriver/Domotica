using System;
using Vendjuuren.Library;

namespace Vendjuuren.Domotica.Library
{
  public static class ExtensionMethods
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Power GetPower(this object obj)
    {
      if (obj != null)
        return ((Power)Enum.Parse(typeof(Power), obj.ToString()));
      else
        return Power.Off;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Letter GetLetter(this object obj)
    {
      if (obj != null)
        return ((Letter)Enum.Parse(typeof(Letter), obj.ToString()));
      else
        return Letter.Undefined;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static BrandType GetBrandType(this object obj)
    {
      if (obj != null)
        return ((BrandType)Enum.Parse(typeof(BrandType), obj.ToString()));
      else
        return BrandType.Undefined;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static ProgramScheduleMode GetScheduleMode(this object obj)
    {
      if (obj != null)
        return ((ProgramScheduleMode)Enum.Parse(typeof(ProgramScheduleMode), obj.ToString()));
      else
        return ProgramScheduleMode.Undefined;
    }
  }
}

