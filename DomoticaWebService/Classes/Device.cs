using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml;

namespace KakuWebService.Classes
{
  public class Device
  {
    [DllImport("TPC200L10.dll")]
    private static extern int Send(byte Scode, byte OnOff);

    private const string nameAttributeName = "name";
    private const string groupAttributeName = "group";
    private const string numberAttributeName = "number";

    public Device(XmlNode deviceNode)
    {
      XmlAttribute nameAttribute = deviceNode.Attributes[nameAttributeName];
      XmlAttribute groupAttribute = deviceNode.Attributes[groupAttributeName];
      XmlAttribute numberAttribute = deviceNode.Attributes[numberAttributeName];
      this.name = nameAttribute.Value;

      this.identifier = new Identifier(
        (Group)Enum.Parse(typeof(Group), groupAttribute.Value, true),
        int.Parse(numberAttribute.Value));
      PowerdOff();
    }

    public void PowerOn()
    {
      send(Power.On);
    }

    public void PowerdOff()
    {
      send(Power.Off);
    }

    public override string ToString()
    {
      return (name + " (" + powerStatus.ToString() + ")");
    }

    /// <summary>
    /// Send comments to the receiver
    /// </summary>
    /// <param name="group">Alphabetic string of the receiver</param>
    /// <param name="number">Number of the receiver</param>
    /// <param name="action">On or Off</param>
    private void send(Power power)
    {
      //Scode = (Cijfercode-1) x 16 + ascii(uppercase(Lettercode)-65 (I5)
      byte[] asciiCharacters = Encoding.ASCII.GetBytes(identifier.Group.ToString());
      int sCode = (identifier.Number - 1) * 16 + asciiCharacters[0] - 65;
      int check = Send((byte)sCode, (byte)power);
      powerStatus = power;
    }

    private string name;
    private Power powerStatus;
    private Identifier identifier;
 
    public Identifier Identifier
    {
      get
      {
        return (identifier);
      }
    }

    public string Id
    {
      get
      {
        return (identifier.ToString());
      }
    }
  }
}
