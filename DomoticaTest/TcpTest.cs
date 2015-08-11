using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using Vendjuuren.SQL;
using Vendjuuren.Domotica.Library;

namespace DomoticaTest
{
  /// <summary>
  /// Summary description for TcpTest
  /// </summary>
  [TestClass]
  public class TcpTest
  {
    public TcpTest()
    {
      //
      // TODO: Add constructor logic here
      //
    }

    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }

    #region Additional test attributes
    //
    // You can use the following additional attributes as you write your tests:
    //
    // Use ClassInitialize to run code before running the first test in the class
    // [ClassInitialize()]
    // public static void MyClassInitialize(TestContext testContext) { }
    //
    // Use ClassCleanup to run code after all tests in a class have run
    // [ClassCleanup()]
    // public static void MyClassCleanup() { }
    //
    // Use TestInitialize to run code before running each test 
    // [TestInitialize()]
    // public void MyTestInitialize() { }
    //
    // Use TestCleanup to run code after each test has run
    // [TestCleanup()]
    // public void MyTestCleanup() { }
    //
    #endregion

     [TestMethod]
    public void ServerStartTest()
    {
      DatabaseConnection.Initialize(@"SERVER\SQLEXPRESS", "Domotica", "kaku", "kaku");
      TcpServerHelperClass.Start(IPAddress.Loopback, 8222);
    }

     [TestMethod]
     public void ClientConnectTest()
     {
       DatabaseConnection.Initialize(@"SERVER\SQLEXPRESS", "Domotica", "kaku", "kaku");
       TcpServerHelperClass.Start(IPAddress.Loopback, 8222);
       TcpClientHelperClass.Connect(IPAddress.Loopback, 8222);
     }

    [TestMethod]
    public void ClientSendMessagePowerDeviceOffTest()
    {
      DatabaseConnection.Initialize(@"SERVER\SQLEXPRESS", "Domotica", "kaku", "kaku");
      TcpServerHelperClass.Start(IPAddress.Loopback, 8222);
      TcpClientHelperClass.Connect(IPAddress.Loopback, 8222);
      DeviceHelper.InitializeAsClient(Statics.DeviceViewCollection);
      Statics.DeviceViewCollection = new DeviceViewCollection();
      Statics.DeviceViewCollection.GetAll();

      DeviceView device = Statics.DeviceViewCollection.GetDevice(Letter.E, 1);

      TcpMessage message = new TcpMessage("kaku", "kaku");
      message.AddTcpAction(TcpMessage.Type.PowerDeviceOff, device);
      TcpClientHelperClass.SendMessage(message);
    }

    [TestMethod]
    public void ClientSendMessageGetAllDevicesTest()
    {
      DatabaseConnection.Initialize(@"SERVER\SQLEXPRESS", "Domotica", "kaku", "kaku");
      TcpServerHelperClass.Start(IPAddress.Loopback, 8222);
      TcpClientHelperClass.Connect(IPAddress.Loopback, 8222);
      DeviceHelper.InitializeAsClient(Statics.DeviceViewCollection);
      Statics.DeviceViewCollection = new DeviceViewCollection();
      Statics.DeviceViewCollection.GetAll();

      DeviceView device = Statics.DeviceViewCollection.GetDevice(Letter.E, 1);

      TcpMessage message = new TcpMessage("kaku", "kaku");
      message.AddTcpAction(TcpMessage.Type.GetAllDevices, null);
      TcpClientHelperClass.SendMessage(message);
    }
  }
}
