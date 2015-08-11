using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vendjuuren.Domotica.Library
{
  public enum Power
  {
    Off = 0,
    On = 1
  }

  public enum Letter
  {
    A,B,C,D,E,F,G,H,I,J,Undefined
  }

  public enum Table
  {
    Programs,
    Logs
  }

  public enum BrandType
  {
    Radiographically,
    X10,
    Undefined
  }

  public enum ProgramScheduleMode
  {
    Sun,
    Random,
    Time,
    Undefined
  }
}
