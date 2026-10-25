using System;
using System.Collections.Generic;
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
using Apzon.Forms.Commons;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_WorkPeriod : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public int BranchFromTimeSheeting;

	private bool isLoad;

	private IContainer components;

	private GridControlEx grd_WorkPeriod;

	private GridViewEx gv_WorkPeriod;

	private GridColumnEx PYear;

	private GridColumnEx PeriodCode;

	private GridColumnEx PeriodName;

	private GridColumnEx SubPeriod;

	private RepositoryItemLookUpEdit rep_lookSub;

	private GridColumnEx NumOfPeriod;

	private GridColumnEx PeriodStatus;

	private RepositoryItemLookUpEdit rep_lookStatus;

	private GridColumnEx FromDate;

	private RepositoryItemDateEdit rep_dateFD;

	private GridColumnEx ToDate;

	private RepositoryItemDateEdit rep_dateTD;

	private GridColumnEx Branch;

	private RepositoryItemLookUpEdit rep_lookBrand;

	private SimpleButtonEx btn_Close;

	private SimpleButtonEx btn_NewPeriod;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	public frm_WorkPeriod()
	{
		InitializeComponent();
		FormId = "oWorkPeriod";
		gv_WorkPeriod.FormId = FormId;
		gv_WorkPeriod.ItemId = "APZ_OHWP";
		OnLoadAsync += frm_WorkPeriod_Load;
	}

	private void gv_WorkPeriod_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void btn_Close_Click(object sender, EventArgs e)
	{
		Close();
	}

	public async Task REFRESH()
	{
		btn_NewPeriod_Click(null, null);
	}

	public async Task REFRESH_DATA()
	{
		await RefreshData();
	}

	private void btn_NewPeriod_Click(object sender, EventArgs e)
	{
		try
		{
			OpenFormNewPeriod();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void OpenFormNewPeriod()
	{
		try
		{
			frm_NewPeriod frm_NewPeriod2 = new frm_NewPeriod();
			if (BranchFromTimeSheeting != 0)
			{
				frm_NewPeriod2.BranchFromTimeSheeting = BranchFromTimeSheeting;
			}
			frm_NewPeriod2.MdiParent = MdiParent;
			frm_NewPeriod2.SendDataBackToMain = (ReloadWorkPeriod)Delegate.Combine(frm_NewPeriod2.SendDataBackToMain, new ReloadWorkPeriod(ReloadWorkPeriod));
			frm_NewPeriod2.StartPosition = FormStartPosition.CenterScreen;
			frm_NewPeriod2.Show();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_WorkPeriod_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await RefreshData();
			await WinFuntions.LoadUdfColumns(gv_WorkPeriod, "APZ_OHWP");
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task RefreshData()
	{
		_ = 1;
		try
		{
			SetDataSub();
			SetDataStaus();
			await LoadLookupBranches();
			await GetListWorkPeriod();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_WorkPeriod_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			string code = Function.ToString(gv_WorkPeriod.GetFocusedRowCellValue("PeriodCode"));
			int branch = Function.ParseInt(gv_WorkPeriod.GetFocusedRowCellValue("Branch"));
			DataTable dataTable = await GetWorkPeriodByCode(code, branch);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				frm_NewPeriod frm_NewPeriod2 = new frm_NewPeriod(dataTable);
				frm_NewPeriod2.SendDataBackToMain = (ReloadWorkPeriod)Delegate.Combine(frm_NewPeriod2.SendDataBackToMain, new ReloadWorkPeriod(ReloadWorkPeriod));
				frm_NewPeriod2.StartPosition = FormStartPosition.CenterScreen;
				frm_NewPeriod2.MdiParent = MdiParent;
				frm_NewPeriod2.Show();
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Info(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void SetDataSub()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Year",
			Code = "Y"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Quarters",
			Code = "Q"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Month",
			Code = "M"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Day",
			Code = "D"
		});
		rep_lookSub.DataSource = list;
		rep_lookSub.DisplayMember = "Name";
		rep_lookSub.ValueMember = "Code";
	}

	public void SetDataStaus()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Unlock",
			Code = "U"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Closing",
			Code = "C"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Closed",
			Code = "Y"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Archived",
			Code = "A"
		});
		rep_lookStatus.DataSource = list;
		rep_lookStatus.DisplayMember = "Name";
		rep_lookStatus.ValueMember = "Code";
	}

	private async Task LoadLookupBranches()
	{
		try
		{
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadComboBranch();
			rep_lookBrand.DataSource = dataTable;
			rep_lookBrand.DisplayMember = "Name";
			rep_lookBrand.ValueMember = "Code";
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				gv_WorkPeriod.SetFocusedRowCellValue("Branch", dataTable.Rows[0]["Code"]);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task GetListWorkPeriod()
	{
		try
		{
			DataTable dataTable = await _httpClient.WorkPeriod.GetListWorkPeriod();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				grd_WorkPeriod.DataSource = dataTable;
				grd_WorkPeriod.DataSourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<DataTable> GetWorkPeriodByCode(string code, int branch)
	{
		try
		{
			return await _httpClient.WorkPeriod.GetWorkPeriodByCode(code, branch);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task ReloadWorkPeriod()
	{
		try
		{
			await GetListWorkPeriod();
		}
		catch (Exception ex)
		{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_WorkPeriod));
		this.grd_WorkPeriod = new Apzon.Controls.Grid.GridControlEx();
		this.gv_WorkPeriod = new Apzon.Controls.Grid.GridViewEx();
		this.PYear = new Apzon.Controls.Grid.GridColumnEx();
		this.PeriodCode = new Apzon.Controls.Grid.GridColumnEx();
		this.PeriodName = new Apzon.Controls.Grid.GridColumnEx();
		this.SubPeriod = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_lookSub = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.NumOfPeriod = new Apzon.Controls.Grid.GridColumnEx();
		this.PeriodStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_lookStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.FromDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_dateFD = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.ToDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_dateTD = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.Branch = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_lookBrand = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.btn_Close = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_NewPeriod = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_WorkPeriod).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_WorkPeriod).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookSub).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateFD).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateFD.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateTD).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateTD.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookBrand).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		base.SuspendLayout();
		this.grd_WorkPeriod.AllowPasteExcel = true;
		this.grd_WorkPeriod.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_WorkPeriod.ClipboardData = null;
		this.grd_WorkPeriod.Location = new System.Drawing.Point(12, 12);
		this.grd_WorkPeriod.MainView = this.gv_WorkPeriod;
		this.grd_WorkPeriod.Name = "grd_WorkPeriod";
		this.grd_WorkPeriod.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.rep_lookSub, this.rep_lookStatus, this.rep_dateFD, this.rep_dateTD, this.rep_lookBrand, this.repositoryItemCheckEdit1 });
		this.grd_WorkPeriod.Size = new System.Drawing.Size(1000, 375);
		this.grd_WorkPeriod.TabIndex = 0;
		this.grd_WorkPeriod.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_WorkPeriod });
		this.gv_WorkPeriod.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clWorkPeriod;
		this.gv_WorkPeriod.ColumnKey = null;
		this.gv_WorkPeriod.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[16]
		{
			this.PYear, this.PeriodCode, this.PeriodName, this.SubPeriod, this.NumOfPeriod, this.PeriodStatus, this.FromDate, this.ToDate, this.Branch, this.gridColumnEx1,
			this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7
		});
		this.gv_WorkPeriod.DisableFormSetting = false;
		this.gv_WorkPeriod.DisplayException = false;
		this.gv_WorkPeriod.EditColumn = null;
		this.gv_WorkPeriod.FieldCurrency = null;
		this.gv_WorkPeriod.FieldRate = null;
		this.gv_WorkPeriod.FormId = "700022";
		this.gv_WorkPeriod.GridControl = this.grd_WorkPeriod;
		this.gv_WorkPeriod.IgnoreFilter = false;
		this.gv_WorkPeriod.IndexEditRow = -1;
		this.gv_WorkPeriod.IndicatorWidth = 30;
		this.gv_WorkPeriod.IsApzTable = true;
		this.gv_WorkPeriod.IsInsertNewRow = false;
		this.gv_WorkPeriod.IsLoadingData = false;
		this.gv_WorkPeriod.IsMappingContextAction = false;
		this.gv_WorkPeriod.ItemId = "APZ_OHWP";
		this.gv_WorkPeriod.LstColumnAllowBatch = null;
		this.gv_WorkPeriod.LstColumnNotAllowBatch = null;
		this.gv_WorkPeriod.LstColumnNotEditable = null;
		this.gv_WorkPeriod.LstColumNotSetting = null;
		this.gv_WorkPeriod.Name = "gv_WorkPeriod";
		this.gv_WorkPeriod.OptionsBehavior.ReadOnly = true;
		this.gv_WorkPeriod.OptionsView.ShowGroupPanel = false;
		this.gv_WorkPeriod.UseBatchMod = false;
		this.gv_WorkPeriod.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_WorkPeriod_CustomDrawRowIndicator);
		this.gv_WorkPeriod.DoubleClick += new System.EventHandler(this.gv_WorkPeriod_DoubleClick);
		this.PYear.Caption = "Year";
		this.PYear.CurrencyDateField = null;
		this.PYear.CurrencyField = null;
		this.PYear.CurrencyFixed = null;
		this.PYear.CurrencyFixedRate = 0.0;
		this.PYear.CurrencyRateField = null;
		this.PYear.FieldName = "PYear";
		this.PYear.Name = "PYear";
		this.PYear.PrimaryKey = true;
		this.PYear.UnitField = null;
		this.PYear.Visible = true;
		this.PYear.VisibleIndex = 0;
		this.PYear.Width = 52;
		this.PeriodCode.Caption = "Period Code";
		this.PeriodCode.CurrencyDateField = null;
		this.PeriodCode.CurrencyField = null;
		this.PeriodCode.CurrencyFixed = null;
		this.PeriodCode.CurrencyFixedRate = 0.0;
		this.PeriodCode.CurrencyRateField = null;
		this.PeriodCode.FieldName = "PeriodCode";
		this.PeriodCode.Name = "PeriodCode";
		this.PeriodCode.PrimaryKey = false;
		this.PeriodCode.UnitField = null;
		this.PeriodCode.Visible = true;
		this.PeriodCode.VisibleIndex = 1;
		this.PeriodCode.Width = 88;
		this.PeriodName.Caption = "Period Name";
		this.PeriodName.CurrencyDateField = null;
		this.PeriodName.CurrencyField = null;
		this.PeriodName.CurrencyFixed = null;
		this.PeriodName.CurrencyFixedRate = 0.0;
		this.PeriodName.CurrencyRateField = null;
		this.PeriodName.FieldName = "PeriodName";
		this.PeriodName.Name = "PeriodName";
		this.PeriodName.PrimaryKey = false;
		this.PeriodName.UnitField = null;
		this.PeriodName.Visible = true;
		this.PeriodName.VisibleIndex = 2;
		this.PeriodName.Width = 88;
		this.SubPeriod.Caption = "Sub Period";
		this.SubPeriod.ColumnEdit = this.rep_lookSub;
		this.SubPeriod.CurrencyDateField = null;
		this.SubPeriod.CurrencyField = null;
		this.SubPeriod.CurrencyFixed = null;
		this.SubPeriod.CurrencyFixedRate = 0.0;
		this.SubPeriod.CurrencyRateField = null;
		this.SubPeriod.FieldName = "SubPeriod";
		this.SubPeriod.Name = "SubPeriod";
		this.SubPeriod.PrimaryKey = false;
		this.SubPeriod.UnitField = null;
		this.SubPeriod.Visible = true;
		this.SubPeriod.VisibleIndex = 3;
		this.SubPeriod.Width = 88;
		this.rep_lookSub.AutoHeight = false;
		this.rep_lookSub.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_lookSub.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_lookSub.DisplayMember = "Name";
		this.rep_lookSub.Name = "rep_lookSub";
		this.rep_lookSub.NullText = "";
		this.rep_lookSub.ValueMember = "Code";
		this.NumOfPeriod.Caption = "Num Of Period";
		this.NumOfPeriod.CurrencyDateField = null;
		this.NumOfPeriod.CurrencyField = null;
		this.NumOfPeriod.CurrencyFixed = null;
		this.NumOfPeriod.CurrencyFixedRate = 0.0;
		this.NumOfPeriod.CurrencyRateField = null;
		this.NumOfPeriod.FieldName = "NumOfPeriod";
		this.NumOfPeriod.Name = "NumOfPeriod";
		this.NumOfPeriod.PrimaryKey = false;
		this.NumOfPeriod.UnitField = null;
		this.NumOfPeriod.Visible = true;
		this.NumOfPeriod.VisibleIndex = 4;
		this.NumOfPeriod.Width = 88;
		this.PeriodStatus.Caption = "Period Status";
		this.PeriodStatus.ColumnEdit = this.rep_lookStatus;
		this.PeriodStatus.CurrencyDateField = null;
		this.PeriodStatus.CurrencyField = null;
		this.PeriodStatus.CurrencyFixed = null;
		this.PeriodStatus.CurrencyFixedRate = 0.0;
		this.PeriodStatus.CurrencyRateField = null;
		this.PeriodStatus.FieldName = "PeriodStatus";
		this.PeriodStatus.Name = "PeriodStatus";
		this.PeriodStatus.PrimaryKey = false;
		this.PeriodStatus.UnitField = null;
		this.PeriodStatus.Visible = true;
		this.PeriodStatus.VisibleIndex = 5;
		this.PeriodStatus.Width = 89;
		this.rep_lookStatus.AutoHeight = false;
		this.rep_lookStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_lookStatus.DisplayMember = "Name";
		this.rep_lookStatus.Name = "rep_lookStatus";
		this.rep_lookStatus.NullText = "";
		this.rep_lookStatus.ValueMember = "Code";
		this.FromDate.Caption = "From Date";
		this.FromDate.ColumnEdit = this.rep_dateFD;
		this.FromDate.CurrencyDateField = null;
		this.FromDate.CurrencyField = null;
		this.FromDate.CurrencyFixed = null;
		this.FromDate.CurrencyFixedRate = 0.0;
		this.FromDate.CurrencyRateField = null;
		this.FromDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.FromDate.FieldName = "FromDate";
		this.FromDate.Name = "FromDate";
		this.FromDate.PrimaryKey = false;
		this.FromDate.UnitField = null;
		this.FromDate.Visible = true;
		this.FromDate.VisibleIndex = 6;
		this.FromDate.Width = 89;
		this.rep_dateFD.AutoHeight = false;
		this.rep_dateFD.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_dateFD.Name = "rep_dateFD";
		this.ToDate.Caption = "To Date";
		this.ToDate.ColumnEdit = this.rep_dateTD;
		this.ToDate.CurrencyDateField = null;
		this.ToDate.CurrencyField = null;
		this.ToDate.CurrencyFixed = null;
		this.ToDate.CurrencyFixedRate = 0.0;
		this.ToDate.CurrencyRateField = null;
		this.ToDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.ToDate.FieldName = "ToDate";
		this.ToDate.Name = "ToDate";
		this.ToDate.PrimaryKey = false;
		this.ToDate.UnitField = null;
		this.ToDate.Visible = true;
		this.ToDate.VisibleIndex = 7;
		this.ToDate.Width = 89;
		this.rep_dateTD.AutoHeight = false;
		this.rep_dateTD.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_dateTD.Name = "rep_dateTD";
		this.Branch.Caption = "Branch";
		this.Branch.ColumnEdit = this.rep_lookBrand;
		this.Branch.CurrencyDateField = null;
		this.Branch.CurrencyField = null;
		this.Branch.CurrencyFixed = null;
		this.Branch.CurrencyFixedRate = 0.0;
		this.Branch.CurrencyRateField = null;
		this.Branch.FieldName = "Branch";
		this.Branch.Name = "Branch";
		this.Branch.PrimaryKey = false;
		this.Branch.UnitField = null;
		this.Branch.Visible = true;
		this.Branch.VisibleIndex = 8;
		this.Branch.Width = 58;
		this.rep_lookBrand.AutoHeight = false;
		this.rep_lookBrand.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_lookBrand.DisplayMember = "Name";
		this.rep_lookBrand.Name = "rep_lookBrand";
		this.rep_lookBrand.NullText = "";
		this.rep_lookBrand.ValueMember = "Code";
		this.gridColumnEx1.Caption = "WorkQuota";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "WorkQuota";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx2.Caption = "HourPerDay";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "HourPerDay";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx3.Caption = "TimeKeep";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "TimeKeep";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx4.Caption = "NumOfWorkDf";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "NumOfWorkDf";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx5.Caption = "First Period Leave Total";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "FPLTotal";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 9;
		this.gridColumnEx5.Width = 125;
		this.gridColumnEx6.Caption = "Increase Leave Total";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "IncreaseLeaveTotal";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 10;
		this.gridColumnEx6.Width = 114;
		this.gridColumnEx7.Caption = "IsLastPeriod";
		this.gridColumnEx7.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "IsLastPeriod";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 11;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueGrayed = "N";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.btn_Close.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Close.EditControl = null;
		this.btn_Close.EditValue = "Close Window";
		this.btn_Close.FieldName = null;
		this.btn_Close.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Close.IsSearchControl = false;
		this.btn_Close.Location = new System.Drawing.Point(12, 402);
		this.btn_Close.Name = "btn_Close";
		this.btn_Close.ReadOnly = false;
		this.btn_Close.SettingInVisible = false;
		this.btn_Close.SettingReadonly = false;
		this.btn_Close.Size = new System.Drawing.Size(96, 23);
		this.btn_Close.TabIndex = 1;
		this.btn_Close.Text = "Close Window";
		this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
		this.btn_NewPeriod.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_NewPeriod.EditControl = null;
		this.btn_NewPeriod.EditValue = "New Period";
		this.btn_NewPeriod.FieldName = null;
		this.btn_NewPeriod.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_NewPeriod.ImageOptions.Image");
		this.btn_NewPeriod.IsSearchControl = false;
		this.btn_NewPeriod.Location = new System.Drawing.Point(925, 402);
		this.btn_NewPeriod.Name = "btn_NewPeriod";
		this.btn_NewPeriod.ReadOnly = false;
		this.btn_NewPeriod.SettingInVisible = false;
		this.btn_NewPeriod.SettingReadonly = false;
		this.btn_NewPeriod.Size = new System.Drawing.Size(87, 23);
		this.btn_NewPeriod.TabIndex = 2;
		this.btn_NewPeriod.Text = "New Period";
		this.btn_NewPeriod.Click += new System.EventHandler(this.btn_NewPeriod_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1024, 428);
		base.Controls.Add(this.btn_NewPeriod);
		base.Controls.Add(this.btn_Close);
		base.Controls.Add(this.grd_WorkPeriod);
		base.FormId = "700022";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_WorkPeriod";
		this.Text = "Work Period";
		((System.ComponentModel.ISupportInitialize)this.grd_WorkPeriod).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_WorkPeriod).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookSub).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateFD.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateFD).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateTD.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateTD).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookBrand).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		base.ResumeLayout(false);
	}
}
