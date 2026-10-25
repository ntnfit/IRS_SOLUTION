using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Controls;
using Apzon.Controls.Form;
using Apzon.Forms.Widget;
using Apzon.Global;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.Dashboard;

public class frm_ListDashboard : XtraFormEx
{
	private static frm_ListDashboard _instance;

	private static readonly object _syncLock = new object();

	public List<int> ListDashboardId;

	private List<UcDashboardViewer> _listDashboardViewer;

	private static int _columnCount;

	private IContainer components;

	private Button button1;

	private SpinEdit spinEdit1;

	private Panel panel1;

	private Label label1;

	public int ColumnCount
	{
		get
		{
			return _columnCount;
		}
		set
		{
			if (_listDashboardViewer == null || _listDashboardViewer.Count == 0 || value < 2 || value > 10 || value == _columnCount)
			{
				return;
			}
			_columnCount = (GlobalData.ColumnCountDashboard = value);
			if (spinEdit1.Value != (decimal)value)
			{
				spinEdit1.Value = value;
			}
			int num = (_listDashboardViewer.Count + value - 1) / value;
			if (num < 2)
			{
				num = 2;
			}
			TableLayoutPanel tableLayoutPanel = new TableLayoutPanel
			{
				ColumnCount = value,
				RowCount = num,
				Dock = DockStyle.Fill,
				AutoScroll = true,
				Padding = new Padding(5)
			};
			tableLayoutPanel.SizeChanged += (object s, EventArgs e) =>
			{
				foreach (UcDashboardViewer item in _listDashboardViewer)
				{
					item.Height = item.Width;
				}
			};
			for (int num2 = 0; num2 < value; num2++)
			{
				tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100 / value));
			}
			for (int num3 = 0; num3 < tableLayoutPanel.RowCount; num3++)
			{
				tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			}
			panel1.Controls.Clear();
			panel1.Controls.Add(tableLayoutPanel);
			TableLayoutControlCollection controls = tableLayoutPanel.Controls;
			Control[] controls2 = _listDashboardViewer.ToArray();
			controls.AddRange(controls2);
			foreach (UcDashboardViewer item2 in _listDashboardViewer)
			{
				item2.Height = item2.Width;
			}
			SaveColumnCountConfig();
		}
	}

	public frm_ListDashboard()
	{
		InitializeComponent();
	}

	public static frm_ListDashboard MyDashboard()
	{
		if (_instance == null || _instance.IsDisposed)
		{
			lock (_syncLock)
			{
				if (_instance == null || _instance.IsDisposed)
				{
					_instance = new frm_ListDashboard();
				}
			}
		}
		return _instance;
	}

	private void frm_ListDashboard_Load(object sender, EventArgs e)
	{
		spinEdit1.Value = GlobalData.ColumnCountDashboard;
	}

	private void button1_Click(object sender, EventArgs e)
	{
		ColumnCount = Function.ParseInt(spinEdit1.Value);
	}

	private void SaveColumnCountConfig()
	{
		Task.Run(() =>
		{
			try
			{
				string startupFolderName = Function.GetStartupFolderName();
				string pathRoot = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
				string text = string.Format("{0}\\Users\\All Users\\APZON\\{2}\\{1}\\Config\\ShareConfig.xml", pathRoot, Environment.UserName, startupFolderName);
				if (File.Exists(text))
				{
					Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
					{
						ExeConfigFilename = text
					}, ConfigurationUserLevel.None);
					if (configuration.AppSettings.Settings["ColumnDashboard"] == null)
					{
						configuration.AppSettings.Settings.Add("ColumnDashboard", ColumnCount.ToString());
					}
					else
					{
						configuration.AppSettings.Settings["ColumnDashboard"].Value = ColumnCount.ToString();
					}
					configuration.Save();
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex.ToString());
			}
		});
	}

	private void spinEdit1_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			button1.PerformClick();
		}
	}

	private async void frm_ListDashboard_Shown(object sender, EventArgs e)
	{
		try
		{
			if ((ListDashboardId == null || ListDashboardId.Count == 0) && _listDashboardViewer == null)
			{
				ListDashboardId = await HttpClient.DashboardClient.GetListDashboardByUser(GlobalData.UserName);
				if (ListDashboardId == null || ListDashboardId.Count == 0)
				{
					return;
				}
			}
			int num = (ListDashboardId.Count + GlobalData.ColumnCountDashboard - 1) / GlobalData.ColumnCountDashboard;
			if (num < 2)
			{
				num = 2;
			}
			TableLayoutPanel tableLayoutPanel = new TableLayoutPanel
			{
				ColumnCount = GlobalData.ColumnCountDashboard,
				RowCount = num,
				Dock = DockStyle.Fill,
				AutoScroll = true,
				Padding = new Padding(5)
			};
			for (int i = 0; i < tableLayoutPanel.ColumnCount; i++)
			{
				tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100 / tableLayoutPanel.ColumnCount));
			}
			for (int j = 0; j < tableLayoutPanel.RowCount; j++)
			{
				tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			}
			panel1.Controls.Clear();
			panel1.Controls.Add(tableLayoutPanel);
			if (_listDashboardViewer != null)
			{
				for (int k = 0; k < _listDashboardViewer.Count; k++)
				{
					tableLayoutPanel.Controls.Add(_listDashboardViewer[k]);
				}
			}
			else if (ListDashboardId != null)
			{
				_listDashboardViewer = new List<UcDashboardViewer>();
				for (int l = 0; l < ListDashboardId.Count; l++)
				{
					UcDashboardViewer ucDashboardViewer = new UcDashboardViewer(ListDashboardId[l], ignorePara: true);
					ucDashboardViewer.Dock = DockStyle.Top;
					_listDashboardViewer.Add(ucDashboardViewer);
					tableLayoutPanel.Controls.Add(ucDashboardViewer);
				}
			}
			foreach (UcDashboardViewer item in _listDashboardViewer)
			{
				item.Height = item.Width;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Dashboard.frm_ListDashboard));
		this.button1 = new System.Windows.Forms.Button();
		this.spinEdit1 = new DevExpress.XtraEditors.SpinEdit();
		this.panel1 = new System.Windows.Forms.Panel();
		this.label1 = new System.Windows.Forms.Label();
		((System.ComponentModel.ISupportInitialize)this.spinEdit1.Properties).BeginInit();
		base.SuspendLayout();
		this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.button1.Location = new System.Drawing.Point(177, 608);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(75, 23);
		this.button1.TabIndex = 1;
		this.button1.Text = "Apply";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(this.button1_Click);
		this.spinEdit1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.spinEdit1.EditValue = new decimal(new int[4] { 4, 0, 0, 0 });
		this.spinEdit1.Location = new System.Drawing.Point(112, 610);
		this.spinEdit1.Name = "spinEdit1";
		this.spinEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.spinEdit1.Properties.IsFloatValue = false;
		this.spinEdit1.Properties.Mask.EditMask = "N00";
		this.spinEdit1.Properties.MaxValue = new decimal(new int[4] { 10, 0, 0, 0 });
		this.spinEdit1.Properties.MinValue = new decimal(new int[4] { 2, 0, 0, 0 });
		this.spinEdit1.Size = new System.Drawing.Size(59, 20);
		this.spinEdit1.TabIndex = 2;
		this.spinEdit1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.spinEdit1_KeyDown);
		this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel1.AutoScroll = true;
		this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.panel1.Location = new System.Drawing.Point(0, 0);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(1162, 600);
		this.panel1.TabIndex = 3;
		this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(32, 613);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(74, 13);
		this.label1.TabIndex = 4;
		this.label1.Text = "Column Count";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.AutoScroll = true;
		base.ClientSize = new System.Drawing.Size(1162, 641);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.panel1);
		base.Controls.Add(this.spinEdit1);
		base.Controls.Add(this.button1);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ListDashboard.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ListDashboard";
		base.ShowInTaskbar = false;
		this.Text = "My Dashboard";
		base.Load += new System.EventHandler(this.frm_ListDashboard_Load);
		base.Shown += new System.EventHandler(this.frm_ListDashboard_Shown);
		((System.ComponentModel.ISupportInitialize)this.spinEdit1.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
