using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Vendjuuren.Library
{
  public static class Serialization
  {
    public static string SerializeObjectToStringWithoutDeclaration(Object inputObject)
    {
      return SerializeObjectToStringWithoutDeclaration(inputObject, false);
    }

    public static string SerializeObjectToStringWithoutDeclaration(Object inputObject, bool indent)
    {
      try
      {
        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
        ns.Add("", "");

        XmlSerializer serializer = new XmlSerializer(inputObject.GetType());

        XmlSerializer xs = new XmlSerializer(inputObject.GetType());
        StringBuilder xmlString = new StringBuilder();
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Encoding = Encoding.UTF8;
        settings.OmitXmlDeclaration = true;
        settings.Indent = indent;

        using (XmlWriter stringWriter = XmlWriter.Create(xmlString, settings))
        {
          serializer.Serialize(stringWriter, inputObject, ns);
          return xmlString.ToString();
        }

      }
      catch (Exception ex)
      {
        throw new Exception("Error while serializing object: " + inputObject.GetType() + ".\n" + ex.Message);
      }
    }

    public static string SerializeObjectToString(Object inputObject)
    {
      return SerializeObjectToString(inputObject, true);
    }

    public static string SerializeObjectToString(Object inputObject, bool withNamespace)
    {
      try
      {
        String XmlizedString = null;
        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
        if (!withNamespace)
        {
          ns.Add("", "");
        }
        XmlSerializer xs = new XmlSerializer(inputObject.GetType());

        MemoryStream memoryStream = new MemoryStream();
        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        xs.Serialize(memoryStream, inputObject, ns);
        memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
        XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());

        return XmlizedString;
      }
      catch (Exception ex)
      {
        throw new Exception("Error while serializing object: " + inputObject.GetType() + ".\n" + ex.Message);
      }
    }

    public static Object DeserializeObject(String xmlizedString, Type t)
    {
      try
      {
        XmlSerializer xs = new XmlSerializer(t);
        MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(xmlizedString));

        return xs.Deserialize(memoryStream);
      }
      catch (Exception ex)
      {
        throw new Exception("Error while deserializing object: " + t.GetType() + ".\n" + ex.Message);
      }
    }

    public static T DeserializeObject<T>(String xmlizedString)
    {
      return (T)DeserializeObject(xmlizedString, typeof(T));
    }

    // To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
    private static String UTF8ByteArrayToString(Byte[] characters)
    {
      UTF8Encoding encoding = new UTF8Encoding();
      String constructedString = encoding.GetString(characters);
      return (constructedString);
    }

    /// Converts the String to UTF8 Byte array and is used in De serialization
    private static Byte[] StringToUTF8ByteArray(String pXmlString)
    {
      UTF8Encoding encoding = new UTF8Encoding();
      Byte[] byteArray = encoding.GetBytes(pXmlString);
      return byteArray;
    }
  }
}
