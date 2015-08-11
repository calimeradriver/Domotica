using System.Web.Services;
using Vendjuuren.Domotica.Library;
using Vendjuuren.Domotica.WebService.State;
using System;
using System.Threading;
using System.IO.Ports;
using Vendjuuren.SQL;
using System.Linq;

namespace Vendjuuren.Domotica.WebService
{
  /// <summary>
  /// Summary description for WebService
  /// </summary>
  [WebService(Namespace = "http://tempuri.org/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  [System.ComponentModel.ToolboxItem(false)]
  // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
  // [System.Web.Script.Services.ScriptService]
  public class WebService : System.Web.Services.WebService
  {
    //[WebMethod]
    //public void AddTest()
    //{
    //  Letter letter = Letter.C;
    //  Group group = new Group(StateManager.ApplicationInfo.DatabaseConnection);
    //  group.GetByLetter(letter);

    //  if (group.ID == Guid.Empty)
    //  {
    //    group = new Group(StateManager.ApplicationInfo.DatabaseConnection, letter, "Teest");
    //    group.Save();
    //  }

    //  Device device = new Device(StateManager.ApplicationInfo.DatabaseConnection);
    //  device.GetByLetterNumber(group, 2);

    //  if (device.ID == Guid.Empty)
    //  {
    //    device = new Device(StateManager.ApplicationInfo.DatabaseConnection, group, 2,
    //      "Kerst verlichting", "Speciaal voor de kerst");
    //    device.Save();
    //  }

    //  Program program = new Program(StateManager.ApplicationInfo.DatabaseConnection, true, "test program", "test description", null, null,
    //    Convert.ToDateTime("22:00"), Convert.ToDateTime("22:01"));
    //  program.Save();

    //  program.AddDevice(device);


    //  StateManager.ApplicationInfo.ProgramList.GetAll();

    //  //var blabla = programList[new Guid("30117fd6-4bd7-412e-b664-85595e27581b")].DeviceList;
    //}

    [WebMethod]
    public void X10aan(string address)
    {
      StateManager.ApplicationInfo.Cm11.TurnOnDevice(address);

    }

    [WebMethod]
    public void X10uit(string address)
    {
      StateManager.ApplicationInfo.Cm11.TurnOffDevice(address);
    }

    [WebMethod]
    public void X10dim(string address, int percentage)
    {
      //StateManager.ApplicationInfo.Cm11.DimLamp(address, percentage);
    }

    [WebMethod]
    public void X10bright(string address, int percentage)
    {
      //StateManager.ApplicationInfo.Cm11.BrightenLamp(address, percentage);
    }

    [WebMethod]
    public void RemoveDemoData()
    {
      ProgramCollection programCollection = new ProgramCollection();
      programCollection.GetAll();
      programCollection.DeleteAll();

      DeviceCollection deviceCollection = new DeviceCollection();
      deviceCollection.GetAll();
      deviceCollection.DeleteAll();

      ModelCollection modelCollection = new ModelCollection();
      modelCollection.GetAll();
      modelCollection.DeleteAll();

      BrandCollection brandCollection = new BrandCollection();
      brandCollection.GetAll();
      brandCollection.DeleteAll();

      LogCollection logCollection = new LogCollection();
      logCollection.GetAll();
      logCollection.DeleteAll();
    }

    [WebMethod]
    public void AddDemoData()
    {
      Brand kakuBrand = new Brand();
      kakuBrand.GetByName("KlikaanKlikuit");
      if (kakuBrand.ID == Guid.Empty)
      {
        kakuBrand = new Brand(BrandType.Radiographically, "KlikaanKlikuit", "");
        kakuBrand.Save();
      }

      Brand marmitekBrand = new Brand();
      marmitekBrand.GetByName("Marmitek");
      if (marmitekBrand.ID == Guid.Empty)
      {
        marmitekBrand = new Brand(BrandType.X10, "Marmitek", "");
        marmitekBrand.Save();
      }

      Model switchModel = new Model();
      switchModel.GetByBrandAndType(kakuBrand, "Schakelaar");
      if (switchModel.ID == Guid.Empty)
      {
        switchModel = new Model(kakuBrand, "Schakelaar", false, false);
        switchModel.Save();
      }

      Model dimmModel = new Model();
      dimmModel.GetByBrandAndType(kakuBrand, "Dimmer");
      if (dimmModel.ID == Guid.Empty)
      {
        dimmModel = new Model(kakuBrand, "Dimmer", true, false);
        dimmModel.Save();
      }

      Model lwm1Model = new Model();
      lwm1Model.GetByBrandAndType(kakuBrand, "LWM1");
      if (lwm1Model.ID == Guid.Empty)
      {
        lwm1Model = new Model(marmitekBrand, "LWM1", true, true);
        lwm1Model.Save();
      }

      Letter letterC = Letter.C;
      Group groupC = new Group();
      groupC.GetByLetter(letterC);

      if (groupC.ID == Guid.Empty)
      {
        groupC = new Group(letterC, string.Empty);
        groupC.Save();
      }

      Letter letterE = Letter.E;
      Group groupE = new Group();
      groupE.GetByLetter(letterE);

      if (groupE.ID == Guid.Empty)
      {
        groupE = new Group(letterE, string.Empty);
        groupE.Save();
      }

      Device vaasTvDevice = AddDevice(switchModel, groupC, 1, "VaasTV", "Verlichting in vaas bij TV");
      Device kerstDevice = AddDevice(switchModel, groupC, 2, "Kerst", "Kerstboom en overige kerstverlichting");
      Device gangDevice = AddDevice(switchModel, groupC, 4, "Gang", "Gang beneden");
      Device tuinDevice = AddDevice(switchModel, groupC, 5, "Tuin", "Achtertuin");
      Device drankkastDevice = AddDevice(switchModel, groupC, 6, "Drankkast", "Witte leds in drankkast");
      Device openhaardDevice = AddDevice(switchModel, groupC, 7, "Openhaard", "Openhaard in huiskamer");
      Device dressiorDevice = AddDevice(dimmModel, groupC, 8, "Dressoir", "2 Lampjes op dressior");
      Device aircoSlaapkamerDevice = AddDevice(switchModel, groupC, 9, "AircoSlaapkamer", "Airco in de kledingkast slaapkamer");

      Device eettafelDevice = AddDevice(lwm1Model, groupE, 2, "Eettafel", "Lamp boven de eettafel");
      Device fotoHoekDevice = AddDevice(lwm1Model, groupE, 1, "Fotohoek", "Lamp bij de fotohoek");

      Program kerstProgram = AddProgram(ProgramScheduleMode.Time, ProgramScheduleMode.Time, true, "Kerst",
              "Speciaal voor kerstverlichting in de decembermaand",
              Convert.ToDateTime("06-12"), Convert.ToDateTime("06-01"),
              Convert.ToDateTime("18:00"), Convert.ToDateTime("23:30"));
      kerstProgram.AddDevice(kerstDevice);

      Program verlichtingQ1Program = AddProgram(ProgramScheduleMode.Sun, ProgramScheduleMode.Time, true, "VerlichtingQ1",
              "Verlichting huiskamer 1e kwartaal",
              Convert.ToDateTime("01-01"), Convert.ToDateTime("31-03"),
              Convert.ToDateTime("17:00"), Convert.ToDateTime("23:30"));
      verlichtingQ1Program.AddDevice(vaasTvDevice);
      verlichtingQ1Program.AddDevice(tuinDevice);
      verlichtingQ1Program.AddDevice(drankkastDevice);
      verlichtingQ1Program.AddDevice(openhaardDevice);
      verlichtingQ1Program.AddDevice(dressiorDevice);

      Program verlichtingQ2Program = AddProgram(ProgramScheduleMode.Sun, ProgramScheduleMode.Time, true, "VerlichtingQ2",
             "Verlichting huiskamer 2e kwartaal",
             Convert.ToDateTime("01-04"), Convert.ToDateTime("30-06"),
             Convert.ToDateTime("19:00"), Convert.ToDateTime("23:30"));
      verlichtingQ2Program.AddDevice(vaasTvDevice);
      verlichtingQ2Program.AddDevice(tuinDevice);
      verlichtingQ2Program.AddDevice(drankkastDevice);
      verlichtingQ2Program.AddDevice(openhaardDevice);
      verlichtingQ2Program.AddDevice(dressiorDevice);
      verlichtingQ2Program.AddDevice(aircoSlaapkamerDevice);

      Program verlichtingQ3Program = AddProgram(ProgramScheduleMode.Sun, ProgramScheduleMode.Time, true, "VerlichtingQ3",
             "Verlichting huiskamer 3e kwartaal",
             Convert.ToDateTime("01-07"), Convert.ToDateTime("30-09"),
             Convert.ToDateTime("21:00"), Convert.ToDateTime("0:00"));
      verlichtingQ3Program.AddDevice(vaasTvDevice);
      verlichtingQ3Program.AddDevice(tuinDevice);
      verlichtingQ3Program.AddDevice(drankkastDevice);
      verlichtingQ3Program.AddDevice(openhaardDevice);
      verlichtingQ3Program.AddDevice(dressiorDevice);
      verlichtingQ2Program.AddDevice(aircoSlaapkamerDevice);

      Program verlichtingQ4Program = AddProgram(ProgramScheduleMode.Sun, ProgramScheduleMode.Time, true, "VerlichtingQ4",
             "Verlichting huiskamer 4e kwartaal",
             Convert.ToDateTime("01-10"), Convert.ToDateTime("31-12"),
             Convert.ToDateTime("18:00"), Convert.ToDateTime("23:30"));
      verlichtingQ4Program.AddDevice(vaasTvDevice);
      verlichtingQ4Program.AddDevice(tuinDevice);
      verlichtingQ4Program.AddDevice(drankkastDevice);
      verlichtingQ4Program.AddDevice(openhaardDevice);
      verlichtingQ4Program.AddDevice(dressiorDevice);

      Program randomProgramTuin = AddProgram(ProgramScheduleMode.Random, ProgramScheduleMode.Random, false, "RandomTuin",
             "Random verlichting tuin",
             Convert.ToDateTime("01-01"), Convert.ToDateTime("31-12"),
             null, null);
      randomProgramTuin.AddDevice(tuinDevice);

      Program randomProgramHuiskamer = AddProgram(ProgramScheduleMode.Random, ProgramScheduleMode.Random, false, "RandomHuiskamer",
             "Random verlichting huiskamer",
             Convert.ToDateTime("01-01"), Convert.ToDateTime("31-12"),
             null, null);
      randomProgramHuiskamer.AddDevice(drankkastDevice);
      randomProgramHuiskamer.AddDevice(openhaardDevice);
      randomProgramHuiskamer.AddDevice(dressiorDevice);

      Program randomProgramGang = AddProgram(ProgramScheduleMode.Random, ProgramScheduleMode.Random, false, "RandomGang",
             "Random verlichting gang",
             Convert.ToDateTime("01-01"), Convert.ToDateTime("31-12"),
             null, null);
      randomProgramGang.AddDevice(gangDevice);
    }

    [WebMethod]
    public Device AddDevice(Model model, Group group, int number, string name, string description)
    {
      Device device = new Device();
      device.GetByLetterNumber(group, number);

      if (device.ID == Guid.Empty)
      {
        device = new Device(model, group, number, name, description);
        device.Save();
      }

      return (device);
    }

    [WebMethod]
    public Program AddProgram(ProgramScheduleMode scheduleStartMode, ProgramScheduleMode scheduleStopMode, bool enabled, string name, string description, DateTime startDate,
      DateTime endDate, DateTime? startTime, DateTime? endTime)
    {
      Program program = new Program(
        scheduleStartMode, scheduleStopMode, enabled, name, description,
        startDate, endDate, startTime, endTime);
      program.Save();

      return (program);
    }

    //[WebMethod]
    //public void AddDeviceManually(string group, int number, string name, string programId)
    //{
    //  AddDevice(group, number, name, Power.Off, new Guid(programId));
    //}

    [WebMethod]
    public void RemoveDevice(Device device)
    {
      //StateManager.ApplicationInfo.DatabaseConnection.Remove(device);
      //StateManager.ApplicationInfo.DeviceList.Remove(device.ID.ToString());
    }

    [WebMethod]
    public void RemoveDeviceManually(string group, int number)
    {
      //RemoveDevice(new Device(group, number, string.Empty));
    }

    //[WebMethod]
    //public void AddProgram(string name, DateTime startDate, DateTime endDate, DateTime startTime, DateTime endTime)
    //{
    //  //Program program = new Program(name, startDate, endDate, startTime, endTime);
    //  //StateManager.ApplicationInfo.DatabaseConnection.Add(program);
    //  //StateManager.ApplicationInfo.ProgramList.Add(program.Identifier, program);
    //}

    [WebMethod]
    public void AddDevice2Program(Guid programId, Device device)
    {

    }

    [WebMethod]
    public void AddProgramManually()
    {
      //AddProgram("testProgramma", DateTime.Now, DateTime.Now, Convert.ToDateTime("12:00"), Convert.ToDateTime("12:01"));
    }

    [WebMethod]
    public Device[] GetAllDevices()
    {
      StateManager.ApplicationInfo.DeviceCollection.GetAll();
      Device[] devices = new Device[StateManager.ApplicationInfo.DeviceCollection.Count];
      StateManager.ApplicationInfo.DeviceCollection.CopyTo(devices, 0);
      return (devices);
    }

    [WebMethod]
    public ProgramCollection GetAllPrograms()
    {
      ProgramCollection programs = new ProgramCollection();
      programs.GetAll();
      return (programs);
    }


    [WebMethod]
    public void PowerDeviceOn(DeviceView device)
    {
      device.PowerOn("ff testen vanuit webservice");
    }

    [WebMethod]
    public void PowerDeviceOff(DeviceView device)
    {
      device.PowerOff("ff testen vanuit webservice");
    }

    [WebMethod]
    public void PowerGroupOn(string group)
    {
    }

    [WebMethod]
    public void PowerGroupOff(string group)
    {
    }
  }
}
