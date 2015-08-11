using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vendjuuren.SQL;
using Vendjuuren.Domotica.Windows.Properties;
using Vendjuuren.Domotica.Library;
using System.ServiceProcess;
using System.Diagnostics;

using System.Net;
using Vendjuuren.Library.Network;

namespace Vendjuuren.Domotica.Windows
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();

      Process[] processes = System.Diagnostics.Process.GetProcessesByName("Vendjuuren.Domotica.Service");

      try
      {
        DatabaseConnection.Initialize(Settings.Default.SQLserverName, Settings.Default.DatabaseName,
             Settings.Default.UserName, Settings.Default.Password);       
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Database connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

      new Log(LogType.Information,
        LogAction.ApplicationStarted, "Domotica WindowsForms");

      Statics.DeviceViewCollection = new DeviceViewCollection();
      Statics.DeviceViewCollection.GetAll();

      TcpClientHelperClass.Connect(IPAddress.Parse("192.168.1.1"), 8221);

      DeviceHelper.InitializeAsClient(Statics.DeviceViewCollection);
      DeviceHelper.OnDevicePowerOn += new DeviceHelper.DevicePowerEventDelegate(DeviceHelper_OnDevicePowerOn);
      DeviceHelper.OnDevicePowerOff += new DeviceHelper.DevicePowerEventDelegate(DeviceHelper_OnDevicePowerOff);

      foreach (DeviceView device in Statics.DeviceViewCollection)
      {
        deviceCollectionView.Items.Add(createDeviceLVI(null, device));
      }
    }

    void DeviceHelper_OnDevicePowerOff(DeviceView device)
    {
      //foreach (ListViewItem item in deviceCollectionView.Items)
      //{
      //  createDeviceLVI(item, device);
      //}
    }

    void DeviceHelper_OnDevicePowerOn(DeviceView device)
    {
      //foreach (ListViewItem item in deviceCollectionView.Items)
      //{
      //  createDeviceLVI(item, device);
      //}
    }

    private void logsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      ShowTable showTable = new ShowTable(Vendjuuren.Domotica.Library.Table.Logs);
      showTable.ShowDialog();
    }

    private void programsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Overview overview = new Overview();
      overview.ShowDialog();
    }

    private void deviceCollectionView_MouseClick(object sender, MouseEventArgs e)
    {
      ListView listView = sender as ListView;
      ListViewItem item = listView.SelectedItems[0];

      if (item != null)
      {
        DeviceView device = item.Tag as DeviceView;
        device.PowerToggle(string.Empty);
        // update
        createDeviceLVI(item, device);
      }
    }

    private ListViewItem createDeviceLVI(ListViewItem item, DeviceView device)
    {
      if (item == null)
        item = new ListViewItem();

      item.Text = device + " (" + device.Power + ")";
      item.Tag = device;
      switch (device.Power)
      {
        case Power.On:
          item.ForeColor = Color.Green;
          break;
        case Power.Off:
          item.ForeColor = Color.Red;
          break;
      }
      return (item);
    }

    private void Form1_FormClosed(object sender, FormClosedEventArgs e)
    {
      DeviceHelper.Dispose();
      //DatabaseConnection.Close();
    }
  }
}
