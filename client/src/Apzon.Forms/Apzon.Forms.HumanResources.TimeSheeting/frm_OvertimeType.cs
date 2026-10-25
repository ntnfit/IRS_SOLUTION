using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_OvertimeType : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	protected Enums.FormAuthorities Authoritues = Enums.FormAuthorities.Read;

	private bool _forceClose;

	private bool _isCancel;

	private IContainer components;

	private GridControlEx grd_OvertimeType;

	private GridViewEx gv_OvertimeType;

	private GridColumnEx cl_Code;

	private GridColumnEx cl_Name;

	private GridColumnEx cl_Rate;

	private RepositoryItemCalcEdit CalcRate;

	private GridColumnEx cl_Remarks;

	private GridColumnEx cl_Color;

	private GridColumnEx cl_Branch;

	private RepositoryItemLookUpEdit lukBranch;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private ContextMenuStrip contextMenuStrip_OverType;

	private ToolStripMenuItem toolStripMenuItem_Remove;

	private RepositoryItemTextEdit reItemCode;

	private RepositoryItemColorPickEdit colorPick_Color;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
			{
				WinFuntions.DisableAllControl(Controls);
				btnCancel.Enabled = true;
				return;
			}
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Update:
				btn_Save.Text = Resources.ButtonUpdate;
				break;
			case Enums.FormViewModeType.View:
				btn_Save.Text = Resources.ButtonOK;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_OvertimeType()
	{
		InitializeComponent();
		FormId = "oOvertimeType";
		gv_OvertimeType.ItemId = "APZ_OHOT";
		gv_OvertimeType.FormId = FormId;
		OnLoadAsync += frm_OvertimeType_Load;
	}

	private async Task frm_OvertimeType_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			await LoadLookupBranches();
			await RefreshOvertimeType();
			await WinFuntions.LoadUdfColumns(gv_OvertimeType, "APZ_OHOT");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void AddNewRow()
	{
		try
		{
			gv_OvertimeType.BeginInsertRow();
			DataTable dataSourceTable = grd_OvertimeType.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				if (!dataSourceTable.AsEnumerable().Any((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Code"])) && string.IsNullOrEmpty(Function.ToString(t["Name"]))))
				{
					DataRow dataRow = dataSourceTable.NewRow();
					dataRow["Code"] = "";
					dataRow["Name"] = "";
					dataRow["Rate"] = 0.0;
					dataRow["Remarks"] = "";
					dataRow["Color"] = 0;
					dataRow["Branch"] = 0;
					dataRow["IsNewRow"] = "Y";
					dataSourceTable.Rows.Add(dataRow);
					dataSourceTable.AcceptChanges();
					dataSourceTable.SetDefaultAndMaxLen(gv_OvertimeType.ItemId);
					grd_OvertimeType.DataSource = dataSourceTable;
					grd_OvertimeType.RefreshDataSource();
				}
			}
			else
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OHOT");
				if (!tableStructure.ExistsColumn("IsNewRow"))
				{
					tableStructure.Columns.Add("IsNewRow", typeof(string));
				}
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["Code"] = "";
				dataRow2["Name"] = "";
				dataRow2["Rate"] = 0.0;
				dataRow2["Remarks"] = "";
				dataRow2["Color"] = 0;
				dataRow2["Branch"] = 0;
				dataRow2["IsNewRow"] = "Y";
				tableStructure.Rows.Add(dataRow2);
				tableStructure.AcceptChanges();
				tableStructure.SetDefaultAndMaxLen(gv_OvertimeType.ItemId);
				grd_OvertimeType.DataSource = tableStructure;
				grd_OvertimeType.RefreshDataSource();
			}
			gv_OvertimeType.EndInsertRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		gv_OvertimeType.EndInsertRow();
	}

	public async Task RefreshOvertimeType()
	{
		DataTable dataTable = await GetOvertimeType();
		dataTable.SetDefaultAndMaxLen(gv_OvertimeType.ItemId);
		grd_OvertimeType.DataSource = dataTable;
		grd_OvertimeType.DataSourceTable.AcceptChanges();
		grd_OvertimeType.RefreshDataSource();
		AddNewRow();
		FormMode = Enums.FormViewModeType.View;
	}

	public async Task<DataTable> GetOvertimeType()
	{
		try
		{
			DataTable dataTable = await _httpClient.TimeSheetingClient.GetOvertimeType();
			if (dataTable.IsNotNull() && !dataTable.ExistsColumn("IsNewRow"))
			{
				dataTable.Columns.Add("IsNewRow", typeof(string));
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task LoadLookupBranches()
	{
		try
		{
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadComboBranch();
			lukBranch.DataSource = dataTable;
			lukBranch.DisplayMember = "Name";
			lukBranch.ValueMember = "Code";
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				gv_OvertimeType.SetFocusedRowCellValue("Branch", dataTable.Rows[0]["Code"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await RefreshOvertimeType();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_OvertimeType_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Name" || e.Column.FieldName == "Code")
			{
				AddNewRow();
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_OvertimeType_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_OvertimeType_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			ColumnView columnView = (ColumnView)sender;
			if (Function.ToString(gv_OvertimeType.GetFocusedRowCellValue("IsNewRow")) != "Y" && (columnView.FocusedColumn.FieldName == "Code" || columnView.FocusedColumn.FieldName == "Branch"))
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

	private void gv_OvertimeType_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (sender is ColumnView)
			{
				_ = Function.ToString(gv_OvertimeType.GetFocusedRowCellValue("IsNewRow")) == "Y";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_OvertimeType_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		try
		{
			if (sender is ColumnView columnView)
			{
				e.ExceptionMode = ExceptionMode.DisplayError;
				e.WindowCaption = "Input Error";
				columnView.HideEditor();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				_isCancel = true;
				Close();
				_isCancel = false;
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				btn_Save.Enabled = false;
				btnCancel.Enabled = false;
				await SaveOvertimeType();
				btn_Save.Enabled = true;
				btnCancel.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<string> SaveOvertimeType()
	{
		_ = 1;
		try
		{
			if (grd_OvertimeType.DataSource is DataTable dataTable)
			{
				DataTable dataTable2 = dataTable.Copy();
				if (dataTable2.IsNotNull())
				{
					DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Code"]) == "");
					if (dataRow != null && Function.ParseInt(dataRow["Branch"]) == 0 && string.IsNullOrEmpty(Function.ToString(dataRow["Name"])))
					{
						dataTable2.Rows.Remove(dataRow);
						dataTable2.AcceptChanges();
					}
					if (dataTable2.IsNotNull())
					{
						dataTable2 = dataTable2.Copy();
						if (dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["code"]).Length > 10) != null)
						{
							WinCommons.Warning(Enums.MessageCode.TheLengthOfStringEnteredIsLargerThanSpecified, "");
							return "Error!";
						}
						if (dataTable2.AsEnumerable().FirstOrDefault((DataRow d) => string.IsNullOrEmpty(Function.ToString(d["Code"]))) != null)
						{
							WinCommons.Error(Enums.MessageCode.CodeCannotBeEmpty, "");
							return "Error!";
						}
						if (dataTable2.AsEnumerable().FirstOrDefault((DataRow d) => string.IsNullOrEmpty(Function.ToString(d["Name"]))) != null)
						{
							WinCommons.Error(Enums.MessageCode.NameCannotBeEmpty, "");
							return "Error!";
						}
						if (dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["Branch"]) == 0) != null)
						{
							WinCommons.Error(Enums.MessageCode.BranchCannotBeEmpty, "");
							return "Error!";
						}
						if (dataTable2.ExistsColumn("IsNewRow"))
						{
							dataTable2.Columns.Remove("IsNewRow");
						}
						for (int num = 0; num < dataTable2.Rows.Count; num++)
						{
							dataTable2.Rows[num]["UserSign2"] = GlobalData.UserID;
							dataTable2.Rows[num]["UpdateDate"] = DateTime.Today;
						}
						dataTable2.AcceptChanges();
						WinCommons._OpenProcessing();
						HttpResult httpResult = await _httpClient.TimeSheetingClient.SaveOvertimeType(dataTable2);
						WinCommons._CloseProcessing();
						if (httpResult.MessageCode == Enums.MessageCode.Success)
						{
							WinCommons.Success(Enums.MessageCode.Success, "");
							FormMode = Enums.FormViewModeType.View;
							await RefreshOvertimeType();
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.None, Function.ToString(httpResult.Message));
						}
					}
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return string.Empty;
	}

	private void toolStripMenuItem_Remove_Click(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_OvertimeType.GetFocusedRowCellValue("Code"));
			int branch = Function.ParseInt(gv_OvertimeType.GetFocusedRowCellValue("Branch"));
			if (!string.IsNullOrEmpty(text) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveItem, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				RemoveOvertimeType(text, branch);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void RemoveOvertimeType(string code, int branch)
	{
		_ = 1;
		try
		{
			string text = await _httpClient.TimeSheetingClient.RemoveOvertimeType(code, branch);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				gv_OvertimeType.BeginUpdate();
				gv_OvertimeType.DeleteSelectedRows();
				gv_OvertimeType.EndUpdate();
				await RefreshOvertimeType();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		_isCancel = true;
		Close();
		_isCancel = false;
	}

	private async void frm_OvertimeType_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (_isCancel)
			{
				return;
			}
			if (!_forceClose)
			{
				e.Cancel = true;
			}
			if (_forceClose)
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
				{
				case DialogResult.No:
					_forceClose = true;
					Close();
					break;
				case DialogResult.Yes:
					if (string.IsNullOrEmpty(await SaveOvertimeType()))
					{
						_forceClose = true;
						Close();
					}
					break;
				}
			}
			else
			{
				_forceClose = true;
				Close();
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_OvertimeType));
		this.grd_OvertimeType = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_OverType = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_OvertimeType = new Apzon.Controls.Grid.GridViewEx();
		this.cl_Code = new Apzon.Controls.Grid.GridColumnEx();
		this.reItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.cl_Name = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Rate = new Apzon.Controls.Grid.GridColumnEx();
		this.CalcRate = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.cl_Remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Color = new Apzon.Controls.Grid.GridColumnEx();
		this.colorPick_Color = new DevExpress.XtraEditors.Repository.RepositoryItemColorPickEdit();
		this.cl_Branch = new Apzon.Controls.Grid.GridColumnEx();
		this.lukBranch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_OvertimeType).BeginInit();
		this.contextMenuStrip_OverType.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_OvertimeType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.reItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.CalcRate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.colorPick_Color).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lukBranch).BeginInit();
		base.SuspendLayout();
		this.grd_OvertimeType.AllowPasteExcel = true;
		this.grd_OvertimeType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_OvertimeType.ClipboardData = null;
		this.grd_OvertimeType.ContextMenuStrip = this.contextMenuStrip_OverType;
		this.grd_OvertimeType.Location = new System.Drawing.Point(6, 3);
		this.grd_OvertimeType.MainView = this.gv_OvertimeType;
		this.grd_OvertimeType.Name = "grd_OvertimeType";
		this.grd_OvertimeType.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.CalcRate, this.lukBranch, this.reItemCode, this.colorPick_Color });
		this.grd_OvertimeType.Size = new System.Drawing.Size(768, 404);
		this.grd_OvertimeType.TabIndex = 78;
		this.grd_OvertimeType.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_OvertimeType });
		this.contextMenuStrip_OverType.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripMenuItem_Remove });
		this.contextMenuStrip_OverType.Name = "contextMenuStrip_OverType";
		this.contextMenuStrip_OverType.Size = new System.Drawing.Size(118, 26);
		this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
		this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Remove.Text = "Remove";
		this.toolStripMenuItem_Remove.Click += new System.EventHandler(this.toolStripMenuItem_Remove_Click);
		this.gv_OvertimeType.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clOvertimeType;
		this.gv_OvertimeType.ColumnKey = null;
		this.gv_OvertimeType.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.cl_Code, this.cl_Name, this.cl_Rate, this.cl_Remarks, this.cl_Color, this.cl_Branch });
		this.gv_OvertimeType.DisableFormSetting = false;
		this.gv_OvertimeType.DisplayException = false;
		this.gv_OvertimeType.EditColumn = null;
		this.gv_OvertimeType.FieldCurrency = null;
		this.gv_OvertimeType.FieldRate = null;
		this.gv_OvertimeType.FormId = "oOverTime";
		this.gv_OvertimeType.GridControl = this.grd_OvertimeType;
		this.gv_OvertimeType.IgnoreFilter = false;
		this.gv_OvertimeType.IndexEditRow = -1;
		this.gv_OvertimeType.IndicatorWidth = 30;
		this.gv_OvertimeType.IsApzTable = true;
		this.gv_OvertimeType.IsInsertNewRow = false;
		this.gv_OvertimeType.IsLoadingData = false;
		this.gv_OvertimeType.ItemId = "OOVT";
		this.gv_OvertimeType.LstColumnAllowBatch = null;
		this.gv_OvertimeType.LstColumnNotAllowBatch = null;
		this.gv_OvertimeType.LstColumnNotEditable = null;
		this.gv_OvertimeType.LstColumNotSetting = null;
		this.gv_OvertimeType.Name = "gv_OvertimeType";
		this.gv_OvertimeType.OptionsView.RowAutoHeight = true;
		this.gv_OvertimeType.OptionsView.ShowGroupPanel = false;
		this.gv_OvertimeType.UseBatchMod = false;
		this.gv_OvertimeType.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_OvertimeType_CustomDrawRowIndicator);
		this.gv_OvertimeType.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_OvertimeType_ShowingEditor);
		this.gv_OvertimeType.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_OvertimeType_CellValueChanged);
		this.gv_OvertimeType.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_OvertimeType_ValidatingEditor);
		this.gv_OvertimeType.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gv_OvertimeType_InvalidValueException);
		this.cl_Code.Caption = "Code";
		this.cl_Code.ColumnEdit = this.reItemCode;
		this.cl_Code.CurrencyDateField = null;
		this.cl_Code.CurrencyField = null;
		this.cl_Code.CurrencyFixed = null;
		this.cl_Code.CurrencyFixedRate = 0.0;
		this.cl_Code.CurrencyRateField = null;
		this.cl_Code.FieldName = "Code";
		this.cl_Code.Name = "cl_Code";
		this.cl_Code.PrimaryKey = true;
		this.cl_Code.UnitField = null;
		this.cl_Code.Visible = true;
		this.cl_Code.VisibleIndex = 0;
		this.cl_Code.Width = 44;
		this.reItemCode.AutoHeight = false;
		this.reItemCode.MaxLength = 10;
		this.reItemCode.Name = "reItemCode";
		this.cl_Name.Caption = "Name";
		this.cl_Name.CurrencyDateField = null;
		this.cl_Name.CurrencyField = null;
		this.cl_Name.CurrencyFixed = null;
		this.cl_Name.CurrencyFixedRate = 0.0;
		this.cl_Name.CurrencyRateField = null;
		this.cl_Name.FieldName = "Name";
		this.cl_Name.Name = "cl_Name";
		this.cl_Name.PrimaryKey = false;
		this.cl_Name.UnitField = null;
		this.cl_Name.Visible = true;
		this.cl_Name.VisibleIndex = 1;
		this.cl_Name.Width = 112;
		this.cl_Rate.Caption = "Rate";
		this.cl_Rate.ColumnEdit = this.CalcRate;
		this.cl_Rate.CurrencyDateField = null;
		this.cl_Rate.CurrencyField = null;
		this.cl_Rate.CurrencyFixed = null;
		this.cl_Rate.CurrencyFixedRate = 0.0;
		this.cl_Rate.CurrencyRateField = null;
		this.cl_Rate.FieldName = "Rate";
		this.cl_Rate.Name = "cl_Rate";
		this.cl_Rate.PrimaryKey = false;
		this.cl_Rate.UnitField = null;
		this.cl_Rate.Visible = true;
		this.cl_Rate.VisibleIndex = 2;
		this.CalcRate.AutoHeight = false;
		this.CalcRate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.CalcRate.Name = "CalcRate";
		this.cl_Remarks.Caption = "Remarks";
		this.cl_Remarks.CurrencyDateField = null;
		this.cl_Remarks.CurrencyField = null;
		this.cl_Remarks.CurrencyFixed = null;
		this.cl_Remarks.CurrencyFixedRate = 0.0;
		this.cl_Remarks.CurrencyRateField = null;
		this.cl_Remarks.FieldName = "Remarks";
		this.cl_Remarks.Name = "cl_Remarks";
		this.cl_Remarks.PrimaryKey = false;
		this.cl_Remarks.UnitField = null;
		this.cl_Remarks.Visible = true;
		this.cl_Remarks.VisibleIndex = 3;
		this.cl_Color.Caption = "Color";
		this.cl_Color.ColumnEdit = this.colorPick_Color;
		this.cl_Color.CurrencyDateField = null;
		this.cl_Color.CurrencyField = null;
		this.cl_Color.CurrencyFixed = null;
		this.cl_Color.CurrencyFixedRate = 0.0;
		this.cl_Color.CurrencyRateField = null;
		this.cl_Color.FieldName = "Color";
		this.cl_Color.Name = "cl_Color";
		this.cl_Color.PrimaryKey = false;
		this.cl_Color.UnitField = null;
		this.cl_Color.Visible = true;
		this.cl_Color.VisibleIndex = 4;
		this.colorPick_Color.AutoHeight = false;
		this.colorPick_Color.AutomaticColor = System.Drawing.Color.Black;
		this.colorPick_Color.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.colorPick_Color.Name = "colorPick_Color";
		this.colorPick_Color.StoreColorAsInteger = true;
		this.colorPick_Color.TooltipFormat = DevExpress.XtraEditors.ColorTooltipFormat.Hex;
		this.cl_Branch.Caption = "Branch";
		this.cl_Branch.ColumnEdit = this.lukBranch;
		this.cl_Branch.CurrencyDateField = null;
		this.cl_Branch.CurrencyField = null;
		this.cl_Branch.CurrencyFixed = null;
		this.cl_Branch.CurrencyFixedRate = 0.0;
		this.cl_Branch.CurrencyRateField = null;
		this.cl_Branch.FieldName = "Branch";
		this.cl_Branch.Name = "cl_Branch";
		this.cl_Branch.PrimaryKey = false;
		this.cl_Branch.UnitField = null;
		this.cl_Branch.Visible = true;
		this.cl_Branch.VisibleIndex = 5;
		this.lukBranch.AutoHeight = false;
		this.lukBranch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lukBranch.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.lukBranch.DisplayMember = "Name";
		this.lukBranch.Name = "lukBranch";
		this.lukBranch.NullText = "";
		this.lukBranch.ValueMember = "Code";
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(6, 413);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 81;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(73, 413);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 80;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(779, 435);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grd_OvertimeType);
		base.FormId = "oOverTime";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_OvertimeType";
		this.Text = "Overtime Type";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_OvertimeType_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_OvertimeType).EndInit();
		this.contextMenuStrip_OverType.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_OvertimeType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.reItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.CalcRate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.colorPick_Color).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lukBranch).EndInit();
		base.ResumeLayout(false);
	}
}
