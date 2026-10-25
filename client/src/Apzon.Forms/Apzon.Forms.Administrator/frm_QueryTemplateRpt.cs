using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Administrator;

public class frm_QueryTemplateRpt : XtraFormEx
{
	private DataTable _dt = new DataTable();

	public string Sql = "";

	public string Title = "";

	private IContainer components;

	private GroupControl grC_Doc;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	public frm_QueryTemplateRpt()
	{
		InitializeComponent();
	}

	private async void frm_QueryTemplateRpt_Load(object sender, EventArgs e)
	{
		await Execute();
	}

	private async Task Execute()
	{
		try
		{
			Sql = Function.ToString(Sql).ToUpper();
			if (Sql.Contains("@CURRENTUSER"))
			{
				Sql = Sql.Replace("@CURRENTUSER", $"N'{GlobalData.UserName}'");
			}
			if (Sql.Contains("@CURRENTBRANCH"))
			{
				Sql = Sql.Replace("@CURRENTBRANCH", $"N'{GlobalData.DefaultBranch}'");
			}
			using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons"))
			{
				_dt = await helper.PostDataTableAsync("GetExecute", Sql);
			}
			if (_dt != null && _dt.Rows.Count > 0)
			{
				grC_Doc.Text = Title.ToUpper();
				gv_Doc.Columns.Clear();
				grd_Doc.DataSource = _dt;
				grd_Doc.RefreshDataSource();
				gv_Doc.ReadOnly = false;
				gv_Doc.PopulateColumns();
				return;
			}
			if (Sql != "SELECT *")
			{
				WinCommons.Error(Enums.MessageCode.ExecuteQueryError, "");
			}
			grd_Doc.DataSource = null;
			grd_Doc.RefreshDataSource();
			gv_Doc.Columns.Clear();
			gv_Doc.HideLoadingPanel();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
		this.grC_Doc = new DevExpress.XtraEditors.GroupControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		((System.ComponentModel.ISupportInitialize)this.grC_Doc).BeginInit();
		this.grC_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		base.SuspendLayout();
		this.grC_Doc.Controls.Add(this.grd_Doc);
		this.grC_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grC_Doc.Location = new System.Drawing.Point(0, 0);
		this.grC_Doc.Name = "grC_Doc";
		this.grC_Doc.Size = new System.Drawing.Size(600, 338);
		this.grC_Doc.TabIndex = 0;
		this.grC_Doc.Text = "No data found";
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(2, 21);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(596, 315);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(600, 338);
		base.Controls.Add(this.grC_Doc);
		base.Name = "frm_QueryTemplateRpt";
		base.ShowIcon = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Queries View";
		base.Load += new System.EventHandler(this.frm_QueryTemplateRpt_Load);
		((System.ComponentModel.ISupportInitialize)this.grC_Doc).EndInit();
		this.grC_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		base.ResumeLayout(false);
	}
}
