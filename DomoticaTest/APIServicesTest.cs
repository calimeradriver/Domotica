using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using Vendjuuren.Domotica.Library;
using Vendjuuren.SQL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DomoticaTest
{
  /// <summary>
  /// Summary description for APIServicesTest
  /// </summary>
  [TestClass]
  public class APIServicesTest : TestBase
  {
    public APIServicesTest()
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

    private const string errorDownloadResponse = "Fout tijdens download response '{0}'.";
    private const string errorResponseNull = "Het response is NULL '{0}'.";

    /// <summary>
    /// Bron: http://sunrise-sunset.org/api
    /// Location: Jan Sluijtersstraat 28, Woerden
    /// Position: lat=52.091226 & lng=4.866822
    /// </summary>
    [TestMethod]
    public void SunriseSunSetAPITest()
    {
      Sun sun = new Sun();
      sun.Resolve();

      Assert.IsFalse(sun.ErrorDetected, string.Format(errorDownloadResponse, sun.Name));
      Assert.IsTrue(sun.LastUpdatedSuccessful.HasValue);
      Assert.AreNotEqual<DateTime>(DateTime.MinValue, sun.LastUpdatedSuccessful.Value);
      Assert.IsNotNull(sun.LastResponse, string.Format(errorResponseNull, sun.Name));
      Assert.IsNotNull(sun.Sunrise);
      Assert.IsNotNull(sun.Sunset);
      Assert.AreNotEqual<DateTime>(DateTime.MinValue, sun.Sunrise);
      Assert.AreNotEqual<DateTime>(DateTime.MinValue, sun.Sunset);
    }

    /// <summary>
    /// Bron: http://www.myweather2.com/
    /// Location: Jan Sluijtersstraat 28, Woerden
    /// Position: lat=52.091226 & lng=4.866822
    /// </summary>
    [TestMethod]
    public void WeatherAPITest()
    {
      Weather weather = new Weather();
      weather.Resolve();

      Assert.IsFalse(weather.ErrorDetected, string.Format(errorDownloadResponse, weather.Name));
      Assert.IsTrue(weather.LastUpdatedSuccessful.HasValue);
      Assert.AreNotEqual<DateTime>(DateTime.MinValue, weather.LastUpdatedSuccessful.Value);
      Assert.IsNotNull(weather.LastResponse, string.Format(errorResponseNull, weather.Name));
      Assert.IsNotNull(weather.Temperature);
      Assert.AreNotEqual<int>(0, weather.Temperature);
    }
  }
}
