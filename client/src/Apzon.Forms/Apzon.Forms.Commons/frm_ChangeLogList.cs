using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Commons;

public class frm_ChangeLogList : XtraFormEx
{
	public string LogFormId;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GridControlEx grd_ChangeLog;

	private GridViewEx gv_ChangeLog;

	private SimpleButton btn_Close;

	private SimpleButton simpleButton1;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	public GetChangeLogParams GetChangeLogParams { get; set; }

	public frm_ChangeLogList()
	{
		InitializeComponent();
		FormId = "oChangeLogList";
		gv_ChangeLog.FormId = FormId;
		gv_ChangeLog.ItemId = "APZ_ChangeLogList";
	}

	private async Task frm_ChangeLogList_Load(object sender, EventArgs e)
	{
		try
		{
			if (GetChangeLogParams != null)
			{
				DataTable dataSource = await _httpClient.ChangeLogClient.GetChangeLog(GetChangeLogParams);
				grd_ChangeLog.DataSource = dataSource;
				grd_ChangeLog.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void simpleButton1_Click(object sender, EventArgs e)
	{
		try
		{
			GetChangeLogParams getChangeLogParams = new GetChangeLogParams();
			getChangeLogParams.PrimaryKey = GetChangeLogParams.PrimaryKey;
			getChangeLogParams.UDOObjectCode = GetChangeLogParams.UDOObjectCode;
			getChangeLogParams.Object = GetChangeLogParams.Object;
			getChangeLogParams.LogInstance = Function.ParseInt(gv_ChangeLog.GetRowCellValue(gv_ChangeLog.FocusedRowHandle - 1, "LogInstance"));
			getChangeLogParams.LogInstance2 = Function.ParseInt(gv_ChangeLog.GetFocusedRowCellValue("LogInstance"));
			getChangeLogParams.IsApzTable = GetChangeLogParams.IsApzTable;
			frm_ChangeLogDiff frm_ChangeLogDiff2 = new frm_ChangeLogDiff();
			frm_ChangeLogDiff2.LogFormId = LogFormId;
			frm_ChangeLogDiff2.GetChangeLogParams = getChangeLogParams;
			frm_ChangeLogDiff2.MdiParent = MdiParent;
			frm_ChangeLogDiff frm_ChangeLogDiff3 = frm_ChangeLogDiff2;
			frm_ChangeLogDiff3.Text = frm_ChangeLogDiff3.Text + " (" + getChangeLogParams.LogInstance + " - " + getChangeLogParams.LogInstance2 + ")";
			frm_ChangeLogDiff2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Close_Click(object sender, EventArgs e)
	{
		Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.frm_ChangeLogList));
		this.grd_ChangeLog = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ChangeLog = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Close = new DevExpress.XtraEditors.SimpleButton();
		this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.grd_ChangeLog).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ChangeLog).BeginInit();
		base.SuspendLayout();
		this.grd_ChangeLog.AllowPasteExcel = true;
		this.grd_ChangeLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_ChangeLog.ClipboardData = null;
		this.grd_ChangeLog.Location = new System.Drawing.Point(1, 1);
		this.grd_ChangeLog.MainView = this.gv_ChangeLog;
		this.grd_ChangeLog.Name = "grd_ChangeLog";
		this.grd_ChangeLog.Size = new System.Drawing.Size(756, 444);
		this.grd_ChangeLog.TabIndex = 0;
		this.grd_ChangeLog.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ChangeLog });
		this.gv_ChangeLog.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_ChangeLog.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_ChangeLog.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ChangeLog.ColumnKey = null;
		this.gv_ChangeLog.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4 });
		this.gv_ChangeLog.DisableFormSetting = false;
		this.gv_ChangeLog.DisplayException = false;
		this.gv_ChangeLog.EditColumn = null;
		this.gv_ChangeLog.FieldCurrency = null;
		this.gv_ChangeLog.FieldRate = null;
		this.gv_ChangeLog.FormId = "oChangeLogList";
		this.gv_ChangeLog.GridControl = this.grd_ChangeLog;
		this.gv_ChangeLog.IgnoreFilter = false;
		this.gv_ChangeLog.IndexEditRow = -1;
		this.gv_ChangeLog.IndicatorWidth = 35;
		this.gv_ChangeLog.IsApzTable = false;
		this.gv_ChangeLog.IsInsertNewRow = false;
		this.gv_ChangeLog.IsLoadingData = false;
		this.gv_ChangeLog.IsMappingContextAction = false;
		this.gv_ChangeLog.ItemId = "ODATALOG";
		this.gv_ChangeLog.LstColumnAllowBatch = null;
		this.gv_ChangeLog.LstColumnNotAllowBatch = null;
		this.gv_ChangeLog.LstColumnNotEditable = null;
		this.gv_ChangeLog.LstColumNotSetting = null;
		this.gv_ChangeLog.Name = "gv_ChangeLog";
		this.gv_ChangeLog.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_ChangeLog.OptionsPrint.AutoWidth = false;
		this.gv_ChangeLog.OptionsView.BestFitMaxRowCount = 100;
		this.gv_ChangeLog.OptionsView.ColumnAutoWidth = false;
		this.gv_ChangeLog.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_ChangeLog.OptionsView.ShowGroupPanel = false;
		this.gv_ChangeLog.UseBatchMod = false;
		this.gridColumnEx1.Caption = "Instance";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "LogInstance";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx1.Width = 147;
		this.gridColumnEx2.Caption = "Object Code";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "ObjectCode";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 139;
		this.gridColumnEx3.Caption = "User Name";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "UserName";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.gridColumnEx3.Width = 183;
		this.gridColumnEx4.Caption = "Updated Date";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx4.FieldName = "UpdatedDate";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.ReadOnly = true;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.gridColumnEx4.Width = 193;
		this.btn_Close.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Close.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Close.ImageOptions.Image");
		this.btn_Close.Location = new System.Drawing.Point(1, 451);
		this.btn_Close.Name = "btn_Close";
		this.btn_Close.Size = new System.Drawing.Size(75, 23);
		this.btn_Close.TabIndex = 1;
		this.btn_Close.Text = "OK";
		this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
		this.simpleButton1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton1.Location = new System.Drawing.Point(656, 451);
		this.simpleButton1.Name = "simpleButton1";
		this.simpleButton1.Size = new System.Drawing.Size(101, 23);
		this.simpleButton1.TabIndex = 2;
		this.simpleButton1.Text = "Show Differences";
		this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(759, 481);
		base.Controls.Add(this.simpleButton1);
		base.Controls.Add(this.btn_Close);
		base.Controls.Add(this.grd_ChangeLog);
		base.FormId = "oChangeLogList";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ChangeLogList.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ChangeLogList";
		base.ShowInTaskbar = false;
		this.Text = "Change Log";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_ChangeLogList_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_ChangeLog).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ChangeLog).EndInit();
		base.ResumeLayout(false);
	}
}
