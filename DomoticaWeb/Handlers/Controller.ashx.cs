using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Vendjuuren.Domotica.Library;
using System.Web.Script.Serialization;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using Vendjuuren.SQL;

namespace DomoticaWeb.Handlers
{
  /// <summary>
  /// Summary description for $codebehindclassname$
  /// </summary>
  [WebService(Namespace = "http://tempuri.org/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class Controller : IHttpHandler
  {

    public void ProcessRequest(HttpContext context)
    {
      Vendjuuren.Domotica.Library.TcpMessage.Type action = Vendjuuren.Domotica.Library.TcpMessage.Type.GetAllDevices;

      JsonSerializerSettings settings = new JsonSerializerSettings
      {
        TypeNameHandling = TypeNameHandling.All
      };

      string postData = new StreamReader(context.Request.InputStream).ReadToEnd();
      if (String.IsNullOrEmpty(postData))
      {
        throw new ArgumentNullException("Er is geen postdata ontvangen.");
      }
      
      TcpMessage tcpMessage = JsonConvert.DeserializeObject<TcpMessage>(postData, settings);


      string jsonResult = string.Empty;

      // Alleen eerste actie doen..
      switch (tcpMessage.TcpActions[0].Type)
      {
        case TcpMessage.Type.PowerDeviceOn:
        case TcpMessage.Type.PowerDeviceOff:
          tcpMessage.ResolveTcpActions();
          TcpClientHelperClass.SendMessage(tcpMessage);

          //while (!TcpClientHelperClass.TcpActionComplete)
          //{
          //  // Wachten tot de TcpAction afgerond is.
          //}
          Thread.Sleep(500);

          goto case TcpMessage.Type.GetAllDevices;
        case TcpMessage.Type.GetAllDevices:
          Statics.DeviceViewCollection.GetAll();
          jsonResult = JsonConvert.SerializeObject(Statics.DeviceViewCollection);
          break;
        case TcpMessage.Type.GetServiceInformation:
          jsonResult = JsonConvert.SerializeObject(new ServiceInformation(new Sun(), new Weather()));
          break;
      }

      context.Response.ContentType = "text/json";
      context.Response.Write(jsonResult);
    }

    

    public bool IsReusable
    {
      get
      {
        return false;
      }
    }
  }
}
