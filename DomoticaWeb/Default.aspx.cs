using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Vendjuuren.Domotica.Web.State;
using Vendjuuren.Domotica.Library;

namespace DomoticaWeb
{
  public partial class _Default : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      foreach (DeviceView device in Statics.DeviceViewCollection)
      {
        Button button = new Button();
        button.Height = 100;
        button.Style.Add(HtmlTextWriterStyle.FontSize, "75px");
        button.ID = "btn_" + device.Group + "_" + device.Number;
        button.Text = device.ToString();
        button.Click += new EventHandler(button_Click);
        form1.Controls.Add(button);
        form1.Controls.Add(new LiteralControl("<br /><br />"));
      }
    }


    void button_Click(object sender, EventArgs e)
    {
      Button button = sender as Button;
      
      string[] splittedID = button.ID.Split('_');
      DeviceView device = Statics.DeviceViewCollection.GetDevice(splittedID[1].GetLetter(), int.Parse(splittedID[2]));
      device.GetByID(device.ID);
      device.PowerToggle("From WebApp");
    }
  }
}
