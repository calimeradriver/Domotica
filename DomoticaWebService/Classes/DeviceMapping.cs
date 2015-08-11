using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace KakuWebService.Classes
{
  public class DeviceMapping
  {
    private const string sourceElementName = "Source";
    private const string destinationElementName = "Destination";
    private const string groupAttributeName = "group";
    private const string numberAttributeName = "number";

    public DeviceMapping(XmlNode mappingNode)
    {
      XmlElement sourceElement = mappingNode[sourceElementName];
      XmlAttribute sourceGroupAttribute = sourceElement.Attributes[groupAttributeName];
      XmlAttribute sourceNumberAttribute = sourceElement.Attributes[numberAttributeName];
      XmlElement destinationElement = mappingNode[destinationElementName];
      XmlAttribute destinationGroupAttribute = destinationElement.Attributes[groupAttributeName];
      XmlAttribute destinationNumberpAttribute = destinationElement.Attributes[numberAttributeName];

      this.sourceIdentifier = new Identifier((Group)Enum.Parse(typeof(Group), 
        sourceGroupAttribute.Value), int.Parse(sourceNumberAttribute.Value));
      this.destinationIdentifier = new Identifier((Group)Enum.Parse(typeof(Group), 
        destinationGroupAttribute.Value), int.Parse(destinationNumberpAttribute.Value));
    }

    public override string ToString()
    {
      return (sourceIdentifier.ToString() + " --> " + destinationIdentifier.ToString());
    }

    private Identifier sourceIdentifier;

    private Identifier destinationIdentifier;
    public Identifier DestinationIdentifier
    {
      get
      {
        return (destinationIdentifier);
      }
    }

    public string Id
    {
      get
      {
        return (sourceIdentifier.ToString() + destinationIdentifier.ToString());
      }
    }
  }
}
