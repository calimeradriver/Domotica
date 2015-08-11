using System.Windows.Forms;
using Vendjuuren.SQL;
using Vendjuuren.Domotica.Library;

namespace Vendjuuren.Domotica.Windows
{
  public partial class Overview : Form
  {
    public Overview()
    {
      InitializeComponent();

      ProgramCollection programCollection = new ProgramCollection();
      programCollection.GetAll();

      foreach (Vendjuuren.Domotica.Library.Program program in programCollection)
      {
        TreeNode programNode = new TreeNode(program.Name);
      
        foreach (Device device in program.DeviceCollection)
        {
          programNode.Nodes.Add(device.Name);
        }
        treeView1.Nodes.Add(programNode);
      }
    }
  }
}
