using System;
using System.Collections.Generic;
using Vendjuuren.SQL;
using System.Xml;
using System.Xml.Serialization;

namespace Vendjuuren.Domotica.Library
{
  [Serializable]
  public class TcpMessage
  {
    public TcpMessage()
    {
      _tcpActions = new List<TcpAction>();
    }

    public TcpMessage(string userName, string password)
    {
      _userName = userName;
      _password = password;
      _tcpActions = new List<TcpAction>();
    }

    public void AddTcpAction(Type type, Record record)
    {
      _tcpActions.Add(new TcpAction() { Type = type, Record = record });
    }

    /// <summary>
    /// Haal de records op uit de database.
    /// </summary>
    public void ResolveTcpActions()
    {
      foreach (TcpAction tcpAction in TcpActions)
      {
        Guid id = tcpAction.Record.ID;
        // TODO: ondersteuning maken voor alle type record, generic?
        tcpAction.Record = new DeviceView();
        tcpAction.Record.GetByID(id);
      }
    }

    private string _userName;
    public string UserName
    {
      get { return _userName; }
      set { _userName = value; }
    }

    private string _password;
    public string Password
    {
      get { return _password; }
      set { _password = value; }
    }

    private List<TcpAction> _tcpActions;
    public List<TcpAction> TcpActions
    {
      get { return _tcpActions; }
      set { _tcpActions = value; }
    }

    [Serializable]
    public class TcpAction
    {
      public TcpAction()
      { }

      private Record _record;
      [XmlElement(typeof(Device))]
      [XmlElement(typeof(DeviceView))]
      public Record Record
      {
        get { return _record; }
        set { _record = value; }
      }
      private Type _type;
      public Type Type
      {
        get { return _type; }
        set { _type = value; }
      }
    }

    public enum Type
    {
      PowerDeviceOn,
      PowerDeviceOff,
      GetAllDevices,
      GetServiceInformation
    }
  }
}
