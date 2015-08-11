using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vendjuuren.SQL;
using Vendjuuren.Domotica.Library;
using System.Data;

namespace DomoticaTest
{
  /// <summary>
  /// Summary description for DatabaseTest
  /// </summary>
  [TestClass]
  public class DatabaseTests
  {
    public DatabaseTests()
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
    public void UpdateTest()
    {
      DatabaseConnection.Initialize(@"SERVER\SQLEXPRESS", "Domotica", "kaku", "kaku");

      Device device = new Device();
      device.GetByID(new Guid("9AC77C6F-838F-49AB-95D1-B7E25CAE2F4B"));
      device.Power = Power.On;
      device.Save();
    }

    [TestMethod]
    public void DatabaseConnectionTest()
    {
      DatabaseConnection.Initialize(@"SERVER\SQLEXPRESS", "Domotica", "kaku", "kaku");

      Assert.IsTrue(DatabaseConnection.IsInitialized, "DatabaseConnection is niet geïnitialiseerd.");
      DeviceViewCollection devCol = new DeviceViewCollection();
      devCol.GetAll();
      Assert.IsTrue(devCol.Count > 0, "Er zijn geen devices gevonden.");

      ProgramCollection progCol = new ProgramCollection();
      progCol.GetAll();
      Assert.IsTrue(progCol.Count > 0, "Er zijn geen programs gevonden.");
    }
  }
}
