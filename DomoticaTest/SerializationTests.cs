using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vendjuuren.SQL;
using Vendjuuren.Domotica.Library;
using Newtonsoft.Json;

namespace DomoticaTest
{
  /// <summary>
  /// Summary description for Serialization
  /// </summary>
  [TestClass]
  public class SerializationTests
  {
    public SerializationTests()
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
    public void SerializeTest()
    {
      DatabaseConnection.Initialize(@"SERVER\SQLEXPRESS", "Domotica", "kaku", "kaku");
      DeviceCollection devices = new DeviceCollection();
      devices.GetAll();

      JsonSerializerSettings settings = new JsonSerializerSettings
      {
        TypeNameHandling = TypeNameHandling.All
      };

      List<Record> tests = new List<Record>();
      tests.Add(devices[0]);
      tests.Add(devices[1]);
      tests.Add(devices[2]);
      string jsonResultTest = JsonConvert.SerializeObject((Device)devices[0], settings);
      string jsonResultTests = JsonConvert.SerializeObject(tests);
      string jsonResult = JsonConvert.SerializeObject(devices);
    }
  }
}
