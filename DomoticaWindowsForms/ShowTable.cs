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

namespace Vendjuuren.Domotica.Windows
{
  public partial class ShowTable : Form
  {
    public ShowTable()
    {
      InitializeComponent();
    }

    public ShowTable(Vendjuuren.Domotica.Library.Table table)
      : this()
    {
      switch (table)
      {
        case Vendjuuren.Domotica.Library.Table.Logs:
          LogCollection logCollection = new LogCollection();
          logCollection.GetAll();
          List<Log> logs = new List<Log>();

          foreach (Log log in logCollection)
          {
            logs.Add(log);
          }
          dataGridView.DataSource = logs;
          break;
        case Vendjuuren.Domotica.Library.Table.Programs:
          ProgramCollection programCollection = new ProgramCollection();
          programCollection.GetAll();
          List<Vendjuuren.Domotica.Library.Program> programs = new List<Vendjuuren.Domotica.Library.Program>();

          foreach (Vendjuuren.Domotica.Library.Program program in programCollection)
          {
            programs.Add(program);
          }
          dataGridView.DataSource = programs;
          break;
      }
      dataGridView.Update();
    }
  }
}
