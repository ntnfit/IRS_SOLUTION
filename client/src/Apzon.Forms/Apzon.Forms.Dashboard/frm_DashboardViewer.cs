using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Controls;
using Apzon.Controls.Form;
using Apzon.Forms.Widget;
using DevExpress.DashboardCommon;

namespace Apzon.Forms.Dashboard;

public class frm_DashboardViewer : XtraFormEx
{
	private readonly DevExpress.DashboardCommon.Dashboard _dashboard;

	private readonly int _dashboardId;

	private IContainer components;

	private UcDashboardViewer ucDashboardViewer1;

	public frm_DashboardViewer()
	{
		InitializeComponent();
	}

	public frm_DashboardViewer(DevExpress.DashboardCommon.Dashboard dashboard, int id)
	{
		InitializeComponent();
		_dashboard = dashboard;
		_dashboardId = id;
	}

	private void frm_DashboardViewer_Load(object sender, EventArgs e)
	{
		try
		{
			if (_dashboard != null)
			{
				ucDashboardViewer1.SetDashboard(_dashboard, _dashboardId);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Dashboard.frm_DashboardViewer));
		this.ucDashboardViewer1 = new Apzon.Forms.Widget.UcDashboardViewer();
		base.SuspendLayout();
		this.ucDashboardViewer1.DashboardName = null;
		this.ucDashboardViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucDashboardViewer1.Location = new System.Drawing.Point(0, 0);
		this.ucDashboardViewer1.Name = "ucDashboardViewer1";
		this.ucDashboardViewer1.Size = new System.Drawing.Size(820, 516);
		this.ucDashboardViewer1.TabIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(820, 516);
		base.Controls.Add(this.ucDashboardViewer1);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_DashboardViewer.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_DashboardViewer";
		this.Text = "Dashboard Viewer";
		base.Load += new System.EventHandler(this.frm_DashboardViewer_Load);
		base.ResumeLayout(false);
	}
}
