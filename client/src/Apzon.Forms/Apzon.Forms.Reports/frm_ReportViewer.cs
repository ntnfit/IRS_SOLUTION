using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Windows.Forms;
using DevExpress.XtraEditors;

namespace Apzon.Forms.Reports;

public class frm_ReportViewer : XtraForm
{
	public ReportDocument _rp;

	public Function.ConfigData _ConfigData;

	public bool print;

	public bool _ShowPrintButton = true;

	public bool _ShowExportButton = true;

	public bool _Dispose = true;

	private IContainer components;

	private CrystalReportViewer ReportViewer;

	public bool IsDataSet { get; set; } = true;

	public frm_ReportViewer()
	{
		InitializeComponent();
		SettingLanguage();
	}

	private void SettingLanguage()
	{
		Text = Resources.Titel_frmReportViewer;
	}

	private async void F_ReportViewer_Load(object sender, EventArgs e)
	{
		if (_rp == null)
		{
			return;
		}
		ReportViewer.ShowPrintButton = _ShowPrintButton;
		ReportViewer.ShowExportButton = _ShowExportButton;
		if (!IsDataSet)
		{
			using (HttpClientHelper<Function.ConfigData> helper = new HttpClientHelper<Function.ConfigData>(Configuration.ApiServer, "ConfigConnect"))
			{
				_ConfigData = await helper.GetAsync("GetConfig");
				if (_ConfigData == null || string.IsNullOrEmpty(_ConfigData.Server) || string.IsNullOrEmpty(GlobalData.DatabaseName))
				{
					WinCommons._CloseProcessing();
					WinCommons.writeStatus(Enums.MessageCode.CanNotConnect2Server, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
					return;
				}
			}
			ConnectionInfo connectionInfo = new ConnectionInfo
			{
				DatabaseName = GlobalData.DatabaseName,
				ServerName = _ConfigData.Server,
				UserID = _ConfigData.User,
				Password = _ConfigData.Pass
			};
			foreach (Table table in _rp.Database.Tables)
			{
				TableLogOnInfo logOnInfo = table.LogOnInfo;
				logOnInfo.ConnectionInfo = connectionInfo;
				table.ApplyLogOnInfo(logOnInfo);
			}
		}
		ReportViewer.ReportSource = _rp;
		ReportViewer.Refresh();
		ReportViewer.DrillDownSubreport += ReportViewer_DrillDownSubreport;
		if (print)
		{
			ReportViewer.PrintReport();
		}
	}

	private void ReportViewer_DrillDownSubreport(object source, DrillSubreportEventArgs e)
	{
	}

	private void frm_ReportViewer_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (!_Dispose)
			{
				return;
			}
			foreach (Table table in _rp.Database.Tables)
			{
				table.Dispose();
			}
			_rp.Database.Dispose();
			_rp.Close();
			_rp.Dispose();
			GC.Collect();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_ReportViewer_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.P && e.Modifiers == Keys.Control)
			{
				ReportViewer.PrintReport();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Reports.frm_ReportViewer));
		this.ReportViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
		base.SuspendLayout();
		this.ReportViewer.ActiveViewIndex = -1;
		this.ReportViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.ReportViewer.Cursor = System.Windows.Forms.Cursors.Default;
		this.ReportViewer.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ReportViewer.Location = new System.Drawing.Point(0, 0);
		this.ReportViewer.Name = "ReportViewer";
		this.ReportViewer.ShowCloseButton = false;
		this.ReportViewer.ShowGroupTreeButton = false;
		this.ReportViewer.ShowLogo = false;
		this.ReportViewer.ShowParameterPanelButton = false;
		this.ReportViewer.Size = new System.Drawing.Size(790, 468);
		this.ReportViewer.TabIndex = 0;
		this.ReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(790, 468);
		base.Controls.Add(this.ReportViewer);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ReportViewer.IconOptions.Icon");
		base.KeyPreview = true;
		base.MinimizeBox = false;
		base.Name = "frm_ReportViewer";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Report Viewer";
		base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_ReportViewer_FormClosing);
		base.Load += new System.EventHandler(this.F_ReportViewer_Load);
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_ReportViewer_KeyDown);
		base.ResumeLayout(false);
	}
}
