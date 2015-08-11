using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace KakuWebService.Classes
{
  public class DeviceList : Dictionary<string, Device>
  {
    private const string deviceListRootName = "DeviceList";
    private const string deviceNodeName = "Device";

    public DeviceList(XmlDocument deviceConfigListDocument)
    {
      XmlElement rootElement = deviceConfigListDocument[deviceListRootName];
      XmlNodeList deviceNodeList = rootElement.SelectNodes(deviceNodeName);

      foreach (XmlNode deviceNode in deviceNodeList)
      {
        Device device = new Device(deviceNode);
        Add(device.Id, device);
      }
    }

    public void PowerOn()
    {
      
    }

    public void PowerdOff()
    {
      
    }
  }
}
