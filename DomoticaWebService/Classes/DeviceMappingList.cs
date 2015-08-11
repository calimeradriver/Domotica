using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace KakuWebService.Classes
{
  public class DeviceMappingList : Dictionary<string, DeviceMapping>
  {
    private const string mappingListRootName = "MappingList";
    private const string mappingNodeName = "Mapping";

    public DeviceMappingList(XmlDocument deviceMappingListDocument)
    {
      XmlElement rootElement = deviceMappingListDocument[mappingListRootName];
      XmlNodeList mappingNodeList = rootElement.SelectNodes(mappingNodeName);

      foreach (XmlNode mappingNode in mappingNodeList)
      {
        DeviceMapping deviceMapping = new DeviceMapping(mappingNode);
        Add(deviceMapping.Id, deviceMapping);
      }
    }
  }
}
