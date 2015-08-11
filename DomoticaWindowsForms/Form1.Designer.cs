namespace Vendjuuren.Domotica.Windows
{
  partial class Form1
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.deviceCollectionBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
      this.programBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.deviceCollectionBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.bestandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.extraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.programsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.logsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.deviceCollectionView = new System.Windows.Forms.ListView();
      this.programListBindingSource = new System.Windows.Forms.BindingSource(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.deviceCollectionBindingSource1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.programBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.deviceCollectionBindingSource)).BeginInit();
      this.menuStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.programListBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // deviceCollectionBindingSource1
      // 
      this.deviceCollectionBindingSource1.DataMember = "DeviceCollection";
      this.deviceCollectionBindingSource1.DataSource = this.programBindingSource;
      // 
      // programBindingSource
      // 
      this.programBindingSource.DataSource = typeof(Vendjuuren.Domotica.Library.Program);
      // 
      // deviceCollectionBindingSource
      // 
      this.deviceCollectionBindingSource.DataMember = "DeviceCollection";
      this.deviceCollectionBindingSource.DataSource = this.programBindingSource;
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bestandToolStripMenuItem,
            this.extraToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(838, 24);
      this.menuStrip1.TabIndex = 2;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // bestandToolStripMenuItem
      // 
      this.bestandToolStripMenuItem.Name = "bestandToolStripMenuItem";
      this.bestandToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
      this.bestandToolStripMenuItem.Text = "Bestand";
      // 
      // extraToolStripMenuItem
      // 
      this.extraToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.programsToolStripMenuItem,
            this.logsToolStripMenuItem});
      this.extraToolStripMenuItem.Name = "extraToolStripMenuItem";
      this.extraToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
      this.extraToolStripMenuItem.Text = "Extra";
      // 
      // programsToolStripMenuItem
      // 
      this.programsToolStripMenuItem.Name = "programsToolStripMenuItem";
      this.programsToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
      this.programsToolStripMenuItem.Text = "Programs";
      this.programsToolStripMenuItem.Click += new System.EventHandler(this.programsToolStripMenuItem_Click);
      // 
      // logsToolStripMenuItem
      // 
      this.logsToolStripMenuItem.Name = "logsToolStripMenuItem";
      this.logsToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
      this.logsToolStripMenuItem.Text = "Logs";
      this.logsToolStripMenuItem.Click += new System.EventHandler(this.logsToolStripMenuItem_Click);
      // 
      // deviceCollectionView
      // 
      this.deviceCollectionView.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
      this.deviceCollectionView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.deviceCollectionView.Location = new System.Drawing.Point(0, 24);
      this.deviceCollectionView.Name = "deviceCollectionView";
      this.deviceCollectionView.Size = new System.Drawing.Size(838, 472);
      this.deviceCollectionView.TabIndex = 3;
      this.deviceCollectionView.UseCompatibleStateImageBehavior = false;
      this.deviceCollectionView.View = System.Windows.Forms.View.List;
      this.deviceCollectionView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.deviceCollectionView_MouseClick);
      // 
      // programListBindingSource
      // 
      this.programListBindingSource.DataSource = typeof(Vendjuuren.Domotica.Library.ProgramCollection);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(838, 496);
      this.Controls.Add(this.deviceCollectionView);
      this.Controls.Add(this.menuStrip1);
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "Form1";
      this.Text = "Domotica";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
      ((System.ComponentModel.ISupportInitialize)(this.deviceCollectionBindingSource1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.programBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.deviceCollectionBindingSource)).EndInit();
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.programListBindingSource)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.BindingSource programListBindingSource;
    private System.Windows.Forms.BindingSource programBindingSource;
    private System.Windows.Forms.BindingSource deviceCollectionBindingSource;
    private System.Windows.Forms.BindingSource deviceCollectionBindingSource1;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem bestandToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem extraToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem logsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem programsToolStripMenuItem;
    private System.Windows.Forms.ListView deviceCollectionView;


  }
}

