using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO.Ports;

namespace DomoticaWeb
{
  public partial class Test : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {

      SerialPort mySerialPort = new SerialPort("COM5");

      mySerialPort.BaudRate = 9600;
      mySerialPort.Parity = Parity.None;
      mySerialPort.StopBits = StopBits.One;
      mySerialPort.DataBits = 8;
      mySerialPort.Handshake = Handshake.None;
      
      mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
      mySerialPort.ErrorReceived += new SerialErrorReceivedEventHandler(mySerialPort_ErrorReceived);
      mySerialPort.PinChanged += new SerialPinChangedEventHandler(mySerialPort_PinChanged);

      mySerialPort.Open();
      mySerialPort.Write("#55#17#17#AA");
      mySerialPort.Close();
    }

    void mySerialPort_PinChanged(object sender, SerialPinChangedEventArgs e)
    {
     // throw new NotImplementedException();
    }

    void mySerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
    {
      //throw new NotImplementedException();
    }

    private static void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
    {
      SerialPort sp = (SerialPort)sender;
      string indata = sp.ReadExisting();
    }
  }
}
