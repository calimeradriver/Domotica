using Vendjuuren.Library.Network;
using System;
using System.Net;
using Vendjuuren.SQL;

namespace Vendjuuren.Domotica.Library
{
  /// <summary>
  /// 
  /// </summary>
  public class TcpClientHelperClass
  {
    private static TcpClient _tcpClient;

    public static void Connect(IPAddress serverIP, int port)
    {
      _tcpClient = new TcpClient(serverIP, port);
      _tcpClient.OnConnected += new Tcp.ConnectionEventDelegate(_tcpClient_OnConnected);
      _tcpClient.OnDisconnected += new Tcp.ConnectionEventDelegate(_tcpClient_OnDisconnected);
      _tcpClient.OnDataSent += new Tcp.DataLinkEventDelegate(_tcpClient_OnDataSent);
      _tcpClient.OnDataReceived += new Tcp.DataLinkEventDelegate(_tcpClient_OnDataReceived);
      _tcpClient.Connect();
    }

    public static void SendMessage(TcpMessage tcpMessage)
    {
      //_tcpClient.Send("<debug>" + tcpMessage.TcpActions[0].Record.ToString() + "</debug>");
      tcpActionComplete = false;
      _tcpClient.Send(Serialization.SerializeObjectToString(tcpMessage));
    }

    static void _tcpClient_OnDataReceived(IntPtr handle, IPEndPoint iPEndPoint, string data)
    {
      
    }

    static void _tcpClient_OnDataSent(IntPtr handle, IPEndPoint iPEndPoint, string data)
    {
      tcpActionComplete = true;
    }

    static void _tcpClient_OnDisconnected(IntPtr handle, IPEndPoint iPEndPoint)
    {

    }

    static void _tcpClient_OnConnected(IntPtr handle, IPEndPoint iPEndPoint)
    {
    }

    private static bool tcpActionComplete = false;
    /// <summary>
    /// TcpAction is afgerond.
    /// </summary>
    public static bool TcpActionComplete
    {
      get { return tcpActionComplete; }
    }
  }

  /// <summary>
  /// 
  /// </summary>
  public class TcpServerHelperClass
  {
    private static TcpServer _tcpServer;

    public static void Start(IPAddress ipAddress, int port)
    {
      _tcpServer = new TcpServer(ipAddress, port);
      _tcpServer.OnConnected += new Tcp.ConnectionEventDelegate(_tcpServer_OnConnected);
      _tcpServer.OnDisconnected += new Tcp.ConnectionEventDelegate(_tcpServer_OnDisconnected);
      _tcpServer.OnServerStarted += new TcpServer.ServerStartEventDelegate(_tcpServer_OnServerStarted);
      _tcpServer.OnDataSent += new Tcp.DataLinkEventDelegate(_tcpServer_OnDataSent);
      _tcpServer.OnDataReceived += new Tcp.DataLinkEventDelegate(_tcpServer_OnDataReceived);
      _tcpServer.Start();
    }

    public static void SendMessage(IntPtr clientHandle, TcpMessage tcpMessage)
    {
      _tcpServer.Send(clientHandle, Serialization.SerializeObjectToString(tcpMessage));
    }

    static void _tcpServer_OnConnected(IntPtr handle, System.Net.IPEndPoint iPEndPoint)
    {
      new Log(LogType.Information,
        LogAction.TcpClientConnected, "Client: [" + handle + "] " + iPEndPoint.Address);
    }

    static void _tcpServer_OnDisconnected(IntPtr handle, System.Net.IPEndPoint iPEndPoint)
    {
      new Log(LogType.Information,
        LogAction.TcpClientDisconnected, "Client: [" + handle + "] " + iPEndPoint.Address);
    }

    static void _tcpServer_OnServerStarted(System.Net.IPEndPoint iPEndPoint)
    {
      new Log(LogType.Information,
        LogAction.TcpServerStarted, iPEndPoint.Address + " is listen on port: " + iPEndPoint.Port);
    }

    static void _tcpServer_OnDataSent(IntPtr handle, System.Net.IPEndPoint iPEndPoint, string data)
    {
      new Log(LogType.Information,
        LogAction.TcpServerDataSent, "Data: " + data + " to: [" + handle + "] " + iPEndPoint.Address);
    }

    static void _tcpServer_OnDataReceived(IntPtr handle, System.Net.IPEndPoint iPEndPoint, string data)
    {
      TcpMessage tcpMessage = (TcpMessage)Serialization.DeserializeObject(data, typeof(TcpMessage));
      if (tcpMessage != null)
      {
        if (tcpMessage.UserName.Equals("kaku") && tcpMessage.Password.Equals("kaku"))
        {
          // // TODO: Dit gaat niet goed, ID wordt Guid.Empty er lijkt geen data terug te komen. Onderstaand nu niet meer nodig?
          tcpMessage.ResolveTcpActions();

          foreach (TcpMessage.TcpAction tcpAction in tcpMessage.TcpActions)
          {
            DeviceView device = tcpAction.Record as DeviceView;

            if (device != null)
            {
              switch (tcpAction.Type)
              {
                case TcpMessage.Type.PowerDeviceOn:
                  DeviceHelper.PowerOn(device, "from tcp");
                  break;
                case TcpMessage.Type.PowerDeviceOff:
                  DeviceHelper.PowerOff(device, "from tcp");
                  break;
                case TcpMessage.Type.GetAllDevices:
                  DeviceHelper.GetAllDevices();
                  break;
              }
            }
          }
        }

        new Log(LogType.Information,
        LogAction.TcpServerDataReceived, "Data: " + data + " from: [" + handle + "] " + iPEndPoint.Address);
      }
    }
  }
}
