using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator.SetUp.Inventory;

public class frm_CycleCountDeterminationSubtable : XtraFormEx
{
	private string _whsCode;

	private int _cycleBy;

	private const string Controller = "CycleCDSubTableClient";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public bool isUpdate;

	private IContainer components;

	private GridControlEx grd_CycleCountDeterminationSubtable;

	private GridViewEx gv_CycleCountDeterminationSubtable;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Cancel;

	private GridColumnEx EntryName;

	private GridColumnEx CycleCode;

	private RepositoryItemLookUpEdit look_CycleCode;

	private GridColumnEx Alert;

	private RepositoryItemCheckEdit check_Alert;

	private GridColumnEx DestUser;

	private RepositoryItemLookUpEdit look_User;

	private GridColumnEx NextDate;

	private GridColumnEx Time;

	private GridColumnEx ExcldZrQty;

	private GridColumnEx Entrya;

	private RepositoryItemCheckEdit check_exQty;

	private GridColumnEx Alerted;

	private GridColumnEx ChangExist;

	private GridColumnEx WhsCode;

	private RepositoryItemDateEdit date_NextDate;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Ok.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Ok.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_CycleCountDeterminationSubtable()
	{
		InitializeComponent();
	}

	public frm_CycleCountDeterminationSubtable(string whsCode, int cycleBy)
		: this()
	{
		_whsCode = whsCode;
		_cycleBy = cycleBy;
	}

	private void gv_CycleCountDeterminationSubtable_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (!gv_CycleCountDeterminationSubtable.FocusedColumn.FieldName.Equals("CycleCode") && Function.ParseInt(gv_CycleCountDeterminationSubtable.GetFocusedRowCellValue("CycleCode")) <= 0 && FormMode == Enums.FormViewModeType.View)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_CycleCountDeterminationSubtable_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator && e.RowHandle >= 0)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_CycleCountDeterminationSubtable_Load(object sender, EventArgs e)
	{
		_ = 4;
		try
		{
			await SetInventoryCycles();
			await GetDataByWhs(_whsCode);
			await SetUsers();
			await GetDataByWhs(_whsCode);
			DataTable dataSource = await SearchDataBywhs(_whsCode, _cycleBy);
			grd_CycleCountDeterminationSubtable.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_CycleCountDeterminationSubtable_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				await Save(2);
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void gv_CycleCountDeterminationSubtable_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		if (e.Column.FieldName == "Time")
		{
			e.DisplayText = Function.ParseIntToTimeSpanString(Function.ParseInt(gv_CycleCountDeterminationSubtable.GetRowCellValue(e.ListSourceRowIndex, "Time")));
		}
	}

	public async Task<DataTable> GetDataByWhs(string whsCode)
	{
		try
		{
			return await _httpClient.CycleCDSubTable.GetByWhsCode(whsCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public async Task SetInventoryCycles()
	{
		DataTable dataSource = await _httpClient.CycleCDSubTable.GetInventoryCycles();
		look_CycleCode.DataSource = dataSource;
		look_CycleCode.DisplayMember = "Name";
		look_CycleCode.ValueMember = "Code";
	}

	public async Task SetUsers()
	{
		DataTable dataSource = await _httpClient.CycleCDSubTable.GetUsers();
		look_User.DataSource = dataSource;
		look_User.DisplayMember = "U_NAME";
		look_User.ValueMember = "USERID";
	}

	public async Task<DataTable> SearchDataBywhs(string whs, int cycleBy)
	{
		try
		{
			return await _httpClient.CycleCDSubTable.SearchDataBywhs(whs, cycleBy);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public DataTable GetByCycleCode(int cycleCode)
	{
		try
		{
			return _httpClient.CycleCDSubTable.GetByCycleCode(cycleCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<int> Save(int type)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				DataTable dataSourceTable = grd_CycleCountDeterminationSubtable.DataSourceTable;
				HttpResult httpResult = await _httpClient.CycleCDSubTable.Saves(dataSourceTable);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					FormMode = Enums.FormViewModeType.View;
					isUpdate = true;
					return 0;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
		return 1;
	}

	private void gv_CycleCountDeterminationSubtable_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "CycleCode")
			{
				int cycleCode = Function.ParseInt(e.Value);
				DataTable byCycleCode = GetByCycleCode(cycleCode);
				if (byCycleCode.Rows.Count > 0)
				{
					gv_CycleCountDeterminationSubtable.SetFocusedRowCellValue("NextDate", Function.ParseDateTimes(byCycleCode.Rows[0]["NextDate"]));
					gv_CycleCountDeterminationSubtable.SetFocusedRowCellValue("Time", Function.ParseInt(byCycleCode.Rows[0]["Hour"]));
					gv_CycleCountDeterminationSubtable.SetFocusedRowCellValue("Alert", "Y");
					gv_CycleCountDeterminationSubtable.SetFocusedRowCellValue("ExcldZrQty", "Y");
				}
				FormMode = Enums.FormViewModeType.Update;
			}
			if (e.Column.FieldName == "DestUser")
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			grd_CycleCountDeterminationSubtable.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_CycleCountDeterminationSubtable_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				e.Cancel = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SetUp.Inventory.frm_CycleCountDeterminationSubtable));
		this.grd_CycleCountDeterminationSubtable = new Apzon.Controls.Grid.GridControlEx();
		this.gv_CycleCountDeterminationSubtable = new Apzon.Controls.Grid.GridViewEx();
		this.EntryName = new Apzon.Controls.Grid.GridColumnEx();
		this.Entrya = new Apzon.Controls.Grid.GridColumnEx();
		this.CycleCode = new Apzon.Controls.Grid.GridColumnEx();
		this.look_CycleCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.Alert = new Apzon.Controls.Grid.GridColumnEx();
		this.check_Alert = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.DestUser = new Apzon.Controls.Grid.GridColumnEx();
		this.look_User = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.NextDate = new Apzon.Controls.Grid.GridColumnEx();
		this.date_NextDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.Time = new Apzon.Controls.Grid.GridColumnEx();
		this.ExcldZrQty = new Apzon.Controls.Grid.GridColumnEx();
		this.check_exQty = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.Alerted = new Apzon.Controls.Grid.GridColumnEx();
		this.ChangExist = new Apzon.Controls.Grid.GridColumnEx();
		this.WhsCode = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_CycleCountDeterminationSubtable).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_CycleCountDeterminationSubtable).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_CycleCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_Alert).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_User).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_NextDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_NextDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_exQty).BeginInit();
		base.SuspendLayout();
		this.grd_CycleCountDeterminationSubtable.AllowPasteExcel = true;
		this.grd_CycleCountDeterminationSubtable.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_CycleCountDeterminationSubtable.ClipboardData = null;
		this.grd_CycleCountDeterminationSubtable.Location = new System.Drawing.Point(1, 1);
		this.grd_CycleCountDeterminationSubtable.MainView = this.gv_CycleCountDeterminationSubtable;
		this.grd_CycleCountDeterminationSubtable.Name = "grd_CycleCountDeterminationSubtable";
		this.grd_CycleCountDeterminationSubtable.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.look_CycleCode, this.look_User, this.check_Alert, this.check_exQty, this.date_NextDate });
		this.grd_CycleCountDeterminationSubtable.Size = new System.Drawing.Size(813, 261);
		this.grd_CycleCountDeterminationSubtable.TabIndex = 0;
		this.grd_CycleCountDeterminationSubtable.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_CycleCountDeterminationSubtable });
		this.gv_CycleCountDeterminationSubtable.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_CycleCountDeterminationSubtable.ColumnKey = null;
		this.gv_CycleCountDeterminationSubtable.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.EntryName, this.Entrya, this.CycleCode, this.Alert, this.DestUser, this.NextDate, this.Time, this.ExcldZrQty, this.Alerted, this.ChangExist,
			this.WhsCode
		});
		this.gv_CycleCountDeterminationSubtable.DisableFormSetting = false;
		this.gv_CycleCountDeterminationSubtable.DisplayException = false;
		this.gv_CycleCountDeterminationSubtable.EditColumn = null;
		this.gv_CycleCountDeterminationSubtable.FieldCurrency = null;
		this.gv_CycleCountDeterminationSubtable.FieldRate = null;
		this.gv_CycleCountDeterminationSubtable.FormId = "oCycleCountDeterminationSubTable";
		this.gv_CycleCountDeterminationSubtable.GridControl = this.grd_CycleCountDeterminationSubtable;
		this.gv_CycleCountDeterminationSubtable.IgnoreFilter = false;
		this.gv_CycleCountDeterminationSubtable.IndexEditRow = -1;
		this.gv_CycleCountDeterminationSubtable.IndicatorWidth = 30;
		this.gv_CycleCountDeterminationSubtable.IsApzTable = false;
		this.gv_CycleCountDeterminationSubtable.IsInsertNewRow = false;
		this.gv_CycleCountDeterminationSubtable.IsLoadingData = false;
		this.gv_CycleCountDeterminationSubtable.IsMappingContextAction = false;
		this.gv_CycleCountDeterminationSubtable.ItemId = "ccs1";
		this.gv_CycleCountDeterminationSubtable.LstColumnAllowBatch = null;
		this.gv_CycleCountDeterminationSubtable.LstColumnNotAllowBatch = null;
		this.gv_CycleCountDeterminationSubtable.LstColumnNotEditable = null;
		this.gv_CycleCountDeterminationSubtable.LstColumNotSetting = null;
		this.gv_CycleCountDeterminationSubtable.Name = "gv_CycleCountDeterminationSubtable";
		this.gv_CycleCountDeterminationSubtable.OptionsView.ShowGroupPanel = false;
		this.gv_CycleCountDeterminationSubtable.UseBatchMod = false;
		this.gv_CycleCountDeterminationSubtable.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_CycleCountDeterminationSubtable_CustomDrawRowIndicator);
		this.gv_CycleCountDeterminationSubtable.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_CycleCountDeterminationSubtable_ShowingEditor);
		this.gv_CycleCountDeterminationSubtable.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_CycleCountDeterminationSubtable_CellValueChanged);
		this.gv_CycleCountDeterminationSubtable.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_CycleCountDeterminationSubtable_CellValueChanging);
		this.gv_CycleCountDeterminationSubtable.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_CycleCountDeterminationSubtable_CustomColumnDisplayText);
		this.EntryName.Caption = "Entry";
		this.EntryName.CurrencyDateField = null;
		this.EntryName.CurrencyField = null;
		this.EntryName.CurrencyFixed = null;
		this.EntryName.CurrencyFixedRate = 0.0;
		this.EntryName.CurrencyRateField = null;
		this.EntryName.FieldName = "Name";
		this.EntryName.Name = "EntryName";
		this.EntryName.OptionsColumn.ReadOnly = true;
		this.EntryName.PrimaryKey = true;
		this.EntryName.UnitField = null;
		this.EntryName.Visible = true;
		this.EntryName.VisibleIndex = 0;
		this.EntryName.Width = 108;
		this.Entrya.Caption = "EntryId";
		this.Entrya.CurrencyDateField = null;
		this.Entrya.CurrencyField = null;
		this.Entrya.CurrencyFixed = null;
		this.Entrya.CurrencyFixedRate = 0.0;
		this.Entrya.CurrencyRateField = null;
		this.Entrya.FieldName = "Entry";
		this.Entrya.Name = "Entrya";
		this.Entrya.PrimaryKey = false;
		this.Entrya.UnitField = null;
		this.CycleCode.Caption = "Cycle Code";
		this.CycleCode.ColumnEdit = this.look_CycleCode;
		this.CycleCode.CurrencyDateField = null;
		this.CycleCode.CurrencyField = null;
		this.CycleCode.CurrencyFixed = null;
		this.CycleCode.CurrencyFixedRate = 0.0;
		this.CycleCode.CurrencyRateField = null;
		this.CycleCode.FieldName = "CycleCode";
		this.CycleCode.Name = "CycleCode";
		this.CycleCode.PrimaryKey = false;
		this.CycleCode.UnitField = null;
		this.CycleCode.Visible = true;
		this.CycleCode.VisibleIndex = 1;
		this.CycleCode.Width = 108;
		this.look_CycleCode.AutoHeight = false;
		this.look_CycleCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_CycleCode.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.look_CycleCode.DisplayMember = "Name";
		this.look_CycleCode.Name = "look_CycleCode";
		this.look_CycleCode.NullText = "";
		this.look_CycleCode.ValueMember = "Code";
		this.Alert.Caption = "Alert";
		this.Alert.ColumnEdit = this.check_Alert;
		this.Alert.CurrencyDateField = null;
		this.Alert.CurrencyField = null;
		this.Alert.CurrencyFixed = null;
		this.Alert.CurrencyFixedRate = 0.0;
		this.Alert.CurrencyRateField = null;
		this.Alert.CustomizationCaption = "Alert";
		this.Alert.FieldName = "Alert";
		this.Alert.Name = "Alert";
		this.Alert.PrimaryKey = false;
		this.Alert.UnitField = null;
		this.Alert.Visible = true;
		this.Alert.VisibleIndex = 2;
		this.Alert.Width = 50;
		this.check_Alert.AutoHeight = false;
		this.check_Alert.Name = "check_Alert";
		this.check_Alert.ValueChecked = "Y";
		this.check_Alert.ValueGrayed = "N";
		this.check_Alert.ValueUnchecked = "N";
		this.DestUser.Caption = "User";
		this.DestUser.ColumnEdit = this.look_User;
		this.DestUser.CurrencyDateField = null;
		this.DestUser.CurrencyField = null;
		this.DestUser.CurrencyFixed = null;
		this.DestUser.CurrencyFixedRate = 0.0;
		this.DestUser.CurrencyRateField = null;
		this.DestUser.FieldName = "DestUser";
		this.DestUser.Name = "DestUser";
		this.DestUser.PrimaryKey = false;
		this.DestUser.UnitField = null;
		this.DestUser.Visible = true;
		this.DestUser.VisibleIndex = 3;
		this.DestUser.Width = 122;
		this.look_User.AutoHeight = false;
		this.look_User.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_User.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("U_NAME", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("USERID", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.look_User.DisplayMember = "U_NAME";
		this.look_User.Name = "look_User";
		this.look_User.NullText = "";
		this.look_User.ValueMember = "USERID";
		this.NextDate.Caption = "Next Count Date";
		this.NextDate.ColumnEdit = this.date_NextDate;
		this.NextDate.CurrencyDateField = null;
		this.NextDate.CurrencyField = null;
		this.NextDate.CurrencyFixed = null;
		this.NextDate.CurrencyFixedRate = 0.0;
		this.NextDate.CurrencyRateField = null;
		this.NextDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.NextDate.FieldName = "NextDate";
		this.NextDate.Name = "NextDate";
		this.NextDate.OptionsColumn.AllowEdit = false;
		this.NextDate.OptionsColumn.ReadOnly = true;
		this.NextDate.PrimaryKey = false;
		this.NextDate.UnitField = null;
		this.NextDate.Visible = true;
		this.NextDate.VisibleIndex = 4;
		this.NextDate.Width = 122;
		this.date_NextDate.AutoHeight = false;
		this.date_NextDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_NextDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_NextDate.Name = "date_NextDate";
		this.Time.Caption = "Time";
		this.Time.CurrencyDateField = null;
		this.Time.CurrencyField = null;
		this.Time.CurrencyFixed = null;
		this.Time.CurrencyFixedRate = 0.0;
		this.Time.CurrencyRateField = null;
		this.Time.FieldName = "Time";
		this.Time.Name = "Time";
		this.Time.OptionsColumn.AllowEdit = false;
		this.Time.OptionsColumn.ReadOnly = true;
		this.Time.PrimaryKey = false;
		this.Time.UnitField = null;
		this.Time.Visible = true;
		this.Time.VisibleIndex = 5;
		this.Time.Width = 122;
		this.ExcldZrQty.Caption = "Exclude Bins for Item with Zero Qty";
		this.ExcldZrQty.ColumnEdit = this.check_exQty;
		this.ExcldZrQty.CurrencyDateField = null;
		this.ExcldZrQty.CurrencyField = null;
		this.ExcldZrQty.CurrencyFixed = null;
		this.ExcldZrQty.CurrencyFixedRate = 0.0;
		this.ExcldZrQty.CurrencyRateField = null;
		this.ExcldZrQty.FieldName = "ExcldZrQty";
		this.ExcldZrQty.Name = "ExcldZrQty";
		this.ExcldZrQty.PrimaryKey = false;
		this.ExcldZrQty.UnitField = null;
		this.ExcldZrQty.Visible = true;
		this.ExcldZrQty.VisibleIndex = 6;
		this.ExcldZrQty.Width = 128;
		this.check_exQty.AutoHeight = false;
		this.check_exQty.Name = "check_exQty";
		this.check_exQty.ValueChecked = "Y";
		this.check_exQty.ValueGrayed = "N";
		this.check_exQty.ValueUnchecked = "N";
		this.Alerted.Caption = "Alerted";
		this.Alerted.CurrencyDateField = null;
		this.Alerted.CurrencyField = null;
		this.Alerted.CurrencyFixed = null;
		this.Alerted.CurrencyFixedRate = 0.0;
		this.Alerted.CurrencyRateField = null;
		this.Alerted.FieldName = "Alerted";
		this.Alerted.Name = "Alerted";
		this.Alerted.PrimaryKey = false;
		this.Alerted.UnitField = null;
		this.ChangExist.Caption = "ChangExist";
		this.ChangExist.CurrencyDateField = null;
		this.ChangExist.CurrencyField = null;
		this.ChangExist.CurrencyFixed = null;
		this.ChangExist.CurrencyFixedRate = 0.0;
		this.ChangExist.CurrencyRateField = null;
		this.ChangExist.FieldName = "ChangExist";
		this.ChangExist.Name = "ChangExist";
		this.ChangExist.PrimaryKey = false;
		this.ChangExist.UnitField = null;
		this.WhsCode.Caption = "WhsCode";
		this.WhsCode.CurrencyDateField = null;
		this.WhsCode.CurrencyField = null;
		this.WhsCode.CurrencyFixed = null;
		this.WhsCode.CurrencyFixedRate = 0.0;
		this.WhsCode.CurrencyRateField = null;
		this.WhsCode.FieldName = "WhsCode";
		this.WhsCode.Name = "WhsCode";
		this.WhsCode.PrimaryKey = false;
		this.WhsCode.UnitField = null;
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(1, 265);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 1;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(78, 265);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AcceptButton = this.btn_Ok;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(816, 291);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.grd_CycleCountDeterminationSubtable);
		base.FormId = "oCycleCountDeterminationSubTable";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_CycleCountDeterminationSubtable";
		this.Text = "Cycle Count Determination";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_CycleCountDeterminationSubtable_Load);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_CycleCountDeterminationSubtable_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_CycleCountDeterminationSubtable).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_CycleCountDeterminationSubtable).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_CycleCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_Alert).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_User).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_NextDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_NextDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_exQty).EndInit();
		base.ResumeLayout(false);
	}
}
