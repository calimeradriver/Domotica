using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KakuWebService.Classes
{
  public class Identifier
  {
    public Identifier(Group group, int number)
    {
      this.group = group;
      this.number = number;
    }

    public override string ToString()
    {
      return group.ToString() + number.ToString();
    }

    private Group group;
    public Group Group
    {
      get
      {
        return (group);
      }
    }

    private int number;
    public int Number
    {
      get
      {
        return (number);
      }
    }
  }
}
