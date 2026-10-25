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
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_ListTemplateQueries : XtraFormEx
{
	private DataTable dt = new DataTable();

	private string qName = "";

	public string sql = "";

	public string FilterObject = "OUQR";

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private SimpleButtonEx bt_Choose;

	private SimpleButtonEx bt_Cancel;

	public frm_ListTemplateQueries()
	{
		InitializeComponent();
	}

	private void bt_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void frm_ListQueries_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await GetData(await WinFuntions.ShowFilterSetting(FilterObject));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetData(string filterCode)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			dt = await helper.GetDataTableAsync("GetListSQL", new
			{
				filter = filterCode
			});
			if (dt != null && dt.Rows.Count > 0)
			{
				grd_Doc.DataSource = dt;
				grd_Doc.RefreshDataSource();
				gv_Doc.Columns["CatName"].GroupIndex = 0;
				gv_Doc.Columns["IntrnalKey"].Visible = false;
				gv_Doc.Columns["QCategory"].Visible = false;
				gv_Doc.DoubleClick += gv_Doc_DoubleClick;
			}
			else
			{
				grd_Doc.DataSource = null;
				grd_Doc.RefreshDataSource();
				gv_Doc.Columns.Clear();
				gv_Doc.HideLoadingPanel();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (gv_Doc.FocusedColumn.ToString() == "QName" && gv_Doc.FocusedRowHandle > -1)
			{
				sql = gv_Doc.GetFocusedRowCellValue("QString").ToString();
				qName = gv_Doc.GetFocusedRowCellValue("QName").ToString();
				frm_QueryTemplateRpt frm_QueryTemplateRpt2 = new frm_QueryTemplateRpt();
				frm_QueryTemplateRpt2.Sql = sql;
				frm_QueryTemplateRpt2.Title = qName;
				frm_QueryTemplateRpt2.ShowDialog(this);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bt_Choose_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Doc.FocusedColumn.ToString() == "QName" && gv_Doc.FocusedRowHandle > -1)
			{
				sql = gv_Doc.GetFocusedRowCellValue("QString").ToString();
				qName = gv_Doc.GetFocusedRowCellValue("QName").ToString();
				frm_QueryTemplateRpt frm_QueryTemplateRpt2 = new frm_QueryTemplateRpt();
				frm_QueryTemplateRpt2.Sql = sql;
				frm_QueryTemplateRpt2.Title = qName;
				frm_QueryTemplateRpt2.ShowDialog(this);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_RowCellClick(object sender, RowCellClickEventArgs e)
	{
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
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.bt_Choose = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.Location = new System.Drawing.Point(1, 1);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(500, 287);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.Editable = false;
		this.gv_Doc.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gv_Doc_RowCellClick);
		this.bt_Choose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_Choose.Location = new System.Drawing.Point(1, 294);
		this.bt_Choose.Name = "bt_Choose";
		this.bt_Choose.Size = new System.Drawing.Size(75, 23);
		this.bt_Choose.TabIndex = 1;
		this.bt_Choose.Text = "Choose";
		this.bt_Choose.Click += new System.EventHandler(this.bt_Choose_Click);
		this.bt_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.bt_Cancel.Location = new System.Drawing.Point(82, 294);
		this.bt_Cancel.Name = "bt_Cancel";
		this.bt_Cancel.Size = new System.Drawing.Size(75, 23);
		this.bt_Cancel.TabIndex = 1;
		this.bt_Cancel.Text = "Cancel";
		this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.bt_Cancel;
		base.ClientSize = new System.Drawing.Size(504, 322);
		base.Controls.Add(this.bt_Cancel);
		base.Controls.Add(this.bt_Choose);
		base.Controls.Add(this.grd_Doc);
		base.Name = "frm_ListTemplateQueries";
		base.ShowIcon = false;
		this.Text = "List Queries";
		base.Load += new System.EventHandler(this.frm_ListQueries_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		base.ResumeLayout(false);
	}
}
