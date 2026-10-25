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
using Apzon.Entities.Models.Common;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Commons;

public class frm_ChangeLog : XtraFormEx
{
	public Enums.TypeChangeLog TypeChangeLog;

	public int DocEntry;

	public int OppId;

	private IContainer components;

	private GridControlEx grd_ChangeLog;

	private GridViewEx gv_ChangeLog;

	private SimpleButtonEx btn_OK;

	public frm_ChangeLog()
	{
		InitializeComponent();
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void frm_ChangeLog_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (TypeChangeLog == Enums.TypeChangeLog.OpportunityItemDetail)
			{
				DocEntry = await GetDocEntryItemDetail();
			}
			DataTable dataSource = await LoadData();
			grd_ChangeLog.DataSource = dataSource;
			grd_ChangeLog.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> LoadData()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ChangeLog");
			return await helper.GetAsync("GetLog", new
			{
				docEntry = DocEntry
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<int> GetDocEntryItemDetail()
	{
		try
		{
			using HttpClientHelper<Common.Table_Code> helper = new HttpClientHelper<Common.Table_Code>(Configuration.ApiServer, "ChangeLog");
			return await helper.GetIntAsync("GetDocEntryItemDetail", new
			{
				id = OppId
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	private async void gv_ChangeLog_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			int focusedRowHandle = gv_ChangeLog.FocusedRowHandle;
			if (focusedRowHandle >= 0)
			{
				int docEntry = Function.ParseInt(gv_ChangeLog.GetRowCellValue(focusedRowHandle, "DocEntry"));
				int logId = Function.ParseInt(gv_ChangeLog.GetRowCellValue(focusedRowHandle, "LogInst"));
				DataTable dtSource = await GetChangeLogRow(docEntry, logId, (int)TypeChangeLog);
				frm_ChangeLogRow frm_ChangeLogRow2 = new frm_ChangeLogRow();
				frm_ChangeLogRow2.DtSource = dtSource;
				frm_ChangeLogRow2.MdiParent = MdiParent;
				frm_ChangeLogRow2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetChangeLogRow(int docEntry, int logId, int type)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ChangeLog");
			return await helper.GetAsync("GetChangeLogRow", new { docEntry, logId, type });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.frm_ChangeLog));
		this.grd_ChangeLog = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ChangeLog = new Apzon.Controls.Grid.GridViewEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_ChangeLog).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ChangeLog).BeginInit();
		base.SuspendLayout();
		this.grd_ChangeLog.AllowPasteExcel = true;
		this.grd_ChangeLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_ChangeLog.ClipboardData = null;
		this.grd_ChangeLog.Location = new System.Drawing.Point(0, 0);
		this.grd_ChangeLog.MainView = this.gv_ChangeLog;
		this.grd_ChangeLog.Name = "grd_ChangeLog";
		this.grd_ChangeLog.Size = new System.Drawing.Size(632, 324);
		this.grd_ChangeLog.TabIndex = 0;
		this.grd_ChangeLog.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ChangeLog });
		this.gv_ChangeLog.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ChangeLog.ColumnKey = null;
		this.gv_ChangeLog.DisableFormSetting = false;
		this.gv_ChangeLog.DisplayException = false;
		this.gv_ChangeLog.EditColumn = null;
		this.gv_ChangeLog.FieldCurrency = null;
		this.gv_ChangeLog.FieldRate = null;
		this.gv_ChangeLog.FormId = null;
		this.gv_ChangeLog.GridControl = this.grd_ChangeLog;
		this.gv_ChangeLog.IgnoreFilter = false;
		this.gv_ChangeLog.IndexEditRow = -1;
		this.gv_ChangeLog.IndicatorWidth = 35;
		this.gv_ChangeLog.IsApzTable = false;
		this.gv_ChangeLog.IsInsertNewRow = false;
		this.gv_ChangeLog.IsLoadingData = false;
		this.gv_ChangeLog.IsMappingContextAction = false;
		this.gv_ChangeLog.ItemId = null;
		this.gv_ChangeLog.LstColumnAllowBatch = null;
		this.gv_ChangeLog.LstColumnNotAllowBatch = null;
		this.gv_ChangeLog.LstColumnNotEditable = null;
		this.gv_ChangeLog.LstColumNotSetting = null;
		this.gv_ChangeLog.Name = "gv_ChangeLog";
		this.gv_ChangeLog.UseBatchMod = false;
		this.gv_ChangeLog.DoubleClick += new System.EventHandler(this.gv_ChangeLog_DoubleClick);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(1, 331);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(634, 357);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_ChangeLog);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ChangeLog";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Change log";
		base.Load += new System.EventHandler(this.frm_ChangeLog_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_ChangeLog).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ChangeLog).EndInit();
		base.ResumeLayout(false);
	}
}
