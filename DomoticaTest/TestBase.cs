using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vendjuuren.SQL;

namespace DomoticaTest
{
  public class TestBase
  {
    /// <summary>
    /// Aanmaken van test benodigdheden.
    /// </summary>
    [TestInitialize]
    public void TestBaseInit()
    {
      DatabaseConnection.Initialize(@"SERVER\SQLEXPRESS", "Domotica", "kaku", "kaku");

      //#if DEBUG
      //      DatabaseConnection.Initialize(@"SERVER\SQLEXPRESS", "Domotica", "kaku", "kaku");
      //#endif
    }

    /// <summary>
    /// Opruimen van test benodigdheden.
    /// </summary>
    [TestCleanup]
    public void TestBaseCleanup()
    {

    }
  }
}
