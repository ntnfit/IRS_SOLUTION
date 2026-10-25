using System;
using System.Collections.Generic;
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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Pivot;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Forms.UserControls.FilterControl;
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
using DevExpress.XtraPivotGrid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Sales;

public class frm_AvailableToPromise : XtraFormEx
{
	private const string UpDateString = "Update";

	private const string OkString = "OK";

	private const string FindString = "Find";

	private const string Runstring = "Run";

	private const string SaveString = "Save";

	private Enums.FormViewModeType _currentState = Enums.FormViewModeType.Add;

	private DataTable _docStructure;

	private bool _isLoad;

	private int _code;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient(ApzObjectTypes.oMrp);

	private IContainer components;

	private XtraTabControlEx Xtra_Tab;

	private XtraTabPageEx Tab_Input;

	private XtraTabPageEx Tab_Result;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnSave;

	private LabelLine lbType;

	private LabelLine lbFromDate;

	private LabelLine lbToDate;

	private LookUpEdit txtType;

	private DateEditEx txtFromDate;

	private DateEditEx txtToDate;

	private GridControlEx grd_Input;

	private GridViewEx gv_Input;

	private PivotGridControlEx pgcResult;

	private LabelLine lbAtpName;

	private LabelLine lbStatus;

	private LabelLine lbRemarks;

	private LabelLine lbBranch;

	private LabelLine lbATPCode;

	private TextEditEx txtName;

	private LookUpEdit txtStatus;

	private TextEditEx txtRemark;

	private LookUpEdit txtBranch;

	private PivotGridField field_ItemCode;

	private PivotGridField field_Name;

	private PivotGridField field_ItemGroup;

	private PivotGridField field_Note;

	private PivotGridField fiel_Date;

	private PivotGridField fiel_Qty;

	private GridColumnEx gcItemCode;

	private GridColumnEx gcItemName;

	private GridColumnEx gcWhsCode;

	private GridColumnEx gcShipDate;

	private GridColumnEx gcQty;

	private RepositoryItemButtonEdit rItemCode;

	private RepositoryItemButtonEdit rWhsCode;

	private RepositoryItemDateEdit rInputDateEdit;

	private RepositoryItemCalcEdit rInputCal;

	private ButtonEditEx txtCode;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ToolStripMenuItem dupllicateRowToolStripMenuItem;

	private RepositoryItemLookUpEdit rItemGroup;

	private LookUpEditEx txtFilterField;

	private LabelLine lbFilterField;

	private LabelLine lbFilterValue;

	private FilterValueControlEx txtFilterValue;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItemFilter toolStripMenuItemFilter1;

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
			case Enums.FormViewModeType.View:
				EnableDisableControl(isEnable: true);
				txtBranch.ReadOnly = true;
				txtBranch.Properties.Buttons[0].Visible = false;
				txtCode.ReadOnly = true;
				txtCode.Properties.Buttons[0].Visible = false;
				txtType.ReadOnly = true;
				txtType.Properties.Buttons[0].Visible = false;
				btnSave.Text = "OK";
				_currentState = Enums.FormViewModeType.Update;
				if (!Function.ToString(txtStatus.EditValue).Equals("D"))
				{
					gv_Input.ReadOnly = true;
					rItemCode.Buttons[0].Visible = false;
					rWhsCode.Buttons[0].Visible = false;
				}
				txtCode.Focus();
				break;
			case Enums.FormViewModeType.Update:
				EnableDisableControl(isEnable: true);
				txtBranch.ReadOnly = true;
				txtBranch.Properties.Buttons[0].Visible = false;
				txtCode.ReadOnly = true;
				txtCode.Properties.Buttons[0].Visible = false;
				txtType.ReadOnly = true;
				txtType.Properties.Buttons[0].Visible = false;
				btnSave.Text = "Update";
				if (!Function.ToString(txtStatus.EditValue).Equals("D"))
				{
					gv_Input.ReadOnly = true;
					rItemCode.Buttons[0].Visible = false;
					rWhsCode.Buttons[0].Visible = false;
				}
				break;
			case Enums.FormViewModeType.Find:
			{
				LoadStatus("D");
				EnableDisableControl(isEnable: true);
				SetDataDocControl(null);
				DataTable dataTable = pgcResult.DataSourceTable();
				if (dataTable.IsNotNull())
				{
					dataTable.Rows.Clear();
					pgcResult.DataSource = dataTable;
					pgcResult.DataSourceTable().AcceptChanges();
					pgcResult.RefreshData();
				}
				DataTable dataSourceTable = grd_Input.DataSourceTable;
				if (dataSourceTable.IsNotNull())
				{
					dataSourceTable.Rows.Clear();
					grd_Input.DataSource = dataSourceTable;
					grd_Input.DataSourceTable.AcceptChanges();
					grd_Input.RefreshDataSource();
				}
				btnSave.Text = "Find";
				btnSave.Text = "Find";
				txtCode.Focus();
				break;
			}
			case Enums.FormViewModeType.Busy:
				EnableDisableControl(isEnable: false);
				break;
			case Enums.FormViewModeType.Add:
				EnableDisableControl(isEnable: true);
				txtCode.ReadOnly = true;
				btnSave.Text = "Save";
				txtCode.Properties.Buttons[0].Visible = false;
				break;
			case Enums.FormViewModeType.Run:
				EnableDisableControl(isEnable: true);
				txtCode.ReadOnly = true;
				btnSave.Text = "Run";
				txtCode.Properties.Buttons[0].Visible = false;
				if (!Function.ToString(txtStatus.EditValue).Equals("D"))
				{
					gv_Input.ReadOnly = true;
					rItemCode.Buttons[0].Visible = false;
					rWhsCode.Buttons[0].Visible = false;
				}
				break;
			}
			SetColorControll(value);
		}
		catch
		{
		}
	}

	public frm_AvailableToPromise()
	{
		InitializeComponent();
		OnLoadAsync += frm_AvailableToPromise_Load;
	}

	public frm_AvailableToPromise(int code)
	{
		_code = code;
		InitializeComponent();
		OnLoadAsync += frm_AvailableToPromise_Load;
	}

	private void gv_Input_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_AvailableToPromise_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await LoadAtpType();
			LoadDataBranch();
			InitFormatControl();
			await GetDocStructure();
			LoadItemGroupData();
			if (_code > 0)
			{
				txtCode.EditValue = _code;
			}
			FormMode = Enums.FormViewModeType.Find;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Input.GetFocusedRowCellValue(gcItemCode));
			if (e.Button.Index == 0)
			{
				ChooseFromListItemCode(text);
			}
			else if (!string.IsNullOrEmpty(text))
			{
				this.DrillDown(ApzObjectTypes.oItems, ApzObjectTypes.oItems, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rWhsCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Input.GetFocusedRowCellValue(gcWhsCode));
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.GetListWareHouse, false, text, "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					gv_Input.SetFocusedRowCellValue(gcWhsCode, Function.ToString(frm_CFL2.ChooseItem["WhsCode"]));
				}
			}
			else if (!string.IsNullOrEmpty(text))
			{
				this.DrillDown(ApzObjectTypes.oWarehouses, ApzObjectTypes.oWarehouses, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Input_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName.Equals(gcItemCode.FieldName))
			{
				DataTable dataSourceTable = grd_Input.DataSourceTable;
				int focusedDataSourceRowIndex = gv_Input.GetFocusedDataSourceRowIndex();
				DataTable dataTable = _httpClient.Commons.ItemMaster(Function.ToString(e.Value), GlobalData.DefaultBranch, Enums.ItemObjectType.NoItemType);
				if (dataTable.IsNotNull())
				{
					dataSourceTable.Rows[focusedDataSourceRowIndex]["ItemName"] = dataTable.Rows[0]["ItemName"];
					dataSourceTable.Rows[focusedDataSourceRowIndex]["WhsCode"] = dataTable.Rows[0]["dfltWH"];
				}
				else
				{
					gv_Input.FocusedColumn = gcItemCode;
					gv_Input.ShowEditor();
					ChooseFromListItemCode(Function.ToString(e.Value));
				}
				grd_Input.DataSource = dataSourceTable;
				grd_Input.DataSourceTable.AcceptChanges();
				await AddNewRowInput();
			}
			else if (e.Column.FieldName.Equals(gcShipDate.FieldName) && (e.Value == null || Function.ParseDateTimes(e.Value) < txtFromDate.DateTime.Date || Function.ParseDateTimes(e.Value) > txtToDate.DateTime.Date))
			{
				WinCommons.Error(Enums.MessageCode.ShipdateMustBetweenFromDateAndToDate, "");
				gv_Input.FocusedColumn = gcShipDate;
				gv_Input.FocusedRowHandle = e.RowHandle;
				gv_Input.ShowEditor();
			}
			grd_Input.DataSourceTable.AcceptChanges();
			if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Run;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Input_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(txtFromDate.Text))
			{
				WinCommons.Error(Enums.MessageCode.YouMustChooseFromDateBeforeEditInput, "");
				txtFromDate.Focus();
				e.Cancel = true;
			}
			else if (string.IsNullOrEmpty(txtToDate.Text))
			{
				WinCommons.Error(Enums.MessageCode.YouMustChooseToDateBeforeEditInput, "");
				txtToDate.Focus();
				e.Cancel = true;
			}
			else if (!gv_Input.FocusedColumn.FieldName.Equals(gcItemCode.FieldName) && string.IsNullOrEmpty(Function.ToString(gv_Input.GetFocusedRowCellValue(gcItemCode))))
			{
				WinCommons.Error(Enums.MessageCode.YouMustEnterItemCodeBefore, "");
				gv_Input.FocusedColumn = gcItemCode;
				gv_Input.ShowEditor();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_AvailableToPromise_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Busy)
		{
			e.Cancel = true;
		}
		else if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.Run) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
		{
			e.Cancel = true;
		}
	}

	private async void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Input.FocusedRowHandle >= 0)
			{
				gv_Input.DeleteSelectedRows();
				grd_Input.DataSourceTable.AcceptChanges();
				grd_Input.RefreshDataSource();
				await AddNewRowInput();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void dupllicateRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Input.FocusedRowHandle >= 0 && !string.IsNullOrEmpty(Function.ToString(gv_Input.GetFocusedRowCellValue(gcItemCode))))
			{
				int focusedDataSourceRowIndex = gv_Input.GetFocusedDataSourceRowIndex();
				DataTable dataSourceTable = grd_Input.DataSourceTable;
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow.ItemArray = gv_Input.GetFocusedDataRow().ItemArray;
				dataSourceTable.Rows.InsertAt(dataRow, focusedDataSourceRowIndex);
				grd_Input.DataSource = dataSourceTable;
				grd_Input.DataSourceTable.AcceptChanges();
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
		Close();
	}

	private void pgcResult_CustomAppearance(object sender, PivotCustomAppearanceEventArgs e)
	{
		try
		{
			if (e == null || e.ColumnField == null || !e.ColumnField.FieldName.Equals(fiel_Date.FieldName))
			{
				return;
			}
			if (e.RowValueType == PivotGridValueType.Total)
			{
				e.Appearance.BackColor = Color.PapayaWhip;
				e.Appearance.BackColor2 = Color.PapayaWhip;
				return;
			}
			Point focusedCell = pgcResult.Cells.FocusedCell;
			PivotCellEventArgs cellInfo = pgcResult.Cells.GetCellInfo(focusedCell.X, focusedCell.Y);
			if (e.RowIndex == cellInfo.RowIndex)
			{
				e.Appearance.BackColor = Color.Wheat;
			}
			else
			{
				if (!e.ColumnField.FieldName.Equals(fiel_Date.FieldName))
				{
					return;
				}
				if (e.ColumnIndex == 0)
				{
					e.Appearance.BackColor = pgcResult.BackColor;
					return;
				}
				switch (Function.ParseDateTimes(pgcResult.GetFieldValue(pgcResult.Fields[fiel_Date.FieldName], e.ColumnIndex)).DayOfWeek)
				{
				case DayOfWeek.Saturday:
					e.Appearance.BackColor = Color.FromArgb(233, 249, 242);
					break;
				case DayOfWeek.Sunday:
					e.Appearance.BackColor = Color.FromArgb(225, 234, 237);
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pgcResult_CustomCellDisplayText(object sender, PivotCellDisplayTextEventArgs e)
	{
		try
		{
			if (e != null)
			{
				if (e.RowValueType == PivotGridValueType.Total)
				{
					e.DisplayText = "";
				}
				else if (e.RowValueType == PivotGridValueType.GrandTotal)
				{
					e.DisplayText = "";
				}
				else if (e.ColumnValueType == PivotGridValueType.Total)
				{
					e.DisplayText = "";
				}
				else if (e.RowValueType == PivotGridValueType.Value && Function.ParseDouble(e.Value).Equals(0.0))
				{
					e.DisplayText = "";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void Control_EditValueChanged(object sender, EventArgs e)
	{
		if (_isLoad)
		{
			return;
		}
		if (((dynamic)sender).Name.Equals(txtCode.Name))
		{
			await SearchData(Enums.NavigateType.Current);
		}
		else if (((dynamic)sender).Name.Equals(txtFromDate.Name))
		{
			if (txtFromDate.EditValue == null || Function.ParseDateTimes(txtFromDate.EditValue).Date < DateTime.Now.Date)
			{
				txtFromDate.EditValue = Function.ParseDateTimes(txtFromDate.OldEditValue);
				return;
			}
			if (string.IsNullOrEmpty(txtToDate.Text) || Function.ParseDateTimes(txtFromDate.EditValue) > Function.ParseDateTimes(txtToDate.EditValue))
			{
				txtToDate.EditValue = txtFromDate.EditValue;
			}
		}
		else if ((((dynamic)sender).Name.Equals(txtToDate.Name)) && (txtToDate.EditValue == null || (!string.IsNullOrEmpty(txtFromDate.Text) && Function.ParseDateTimes(txtToDate.EditValue).Date < Function.ParseDateTimes(txtFromDate.EditValue).Date) || (string.IsNullOrEmpty(txtFromDate.Text) && Function.ParseDateTimes(txtToDate.EditValue).Date < DateTime.Now.Date)))
		{
			txtToDate.EditValue = Function.ParseDateTimes(txtToDate.OldEditValue);
			return;
		}
		if (!((((dynamic)sender).Name.Equals(txtFromDate.Name) || ((dynamic)sender).Name.Equals(txtToDate.Name)) ? true : false))
		{
			return;
		}
		DataTable dataTable = grd_Input.DataSourceTable();
		if (dataTable.IsNotNull())
		{
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				DateTime date = Function.ParseDateTimes(dataTable.Rows[i]["ShipDate"]).Date;
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[i]["ItemCode"])) && !string.IsNullOrEmpty(Function.ToString(dataTable.Rows[i]["ShipDate"])) && (date > Function.ParseDateTimes(txtToDate.EditValue).Date || date < Function.ParseDateTimes(txtFromDate.EditValue).Date))
				{
					dataTable.Rows.RemoveAt(i--);
				}
			}
		}
		grd_Input.DataSource = dataTable;
		grd_Input.DataSourceTable.AcceptChanges();
		if (FormMode != Enums.FormViewModeType.Run)
		{
			_currentState = ((FormMode == Enums.FormViewModeType.Add) ? Enums.FormViewModeType.Add : Enums.FormViewModeType.Update);
		}
		FormMode = Enums.FormViewModeType.Run;
	}

	private void Control_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (_isLoad)
			{
				return;
			}
			if (FormMode != Enums.FormViewModeType.Add && !Function.ToString(txtStatus.EditValue).Equals("D") && !((dynamic)sender).Name.Equals(txtName.Name) && !((dynamic)sender).Name.Equals(txtRemark.Name) && !((dynamic)sender).Name.Equals(txtStatus.Name))
			{
				e.Cancel = true;
			}
			else if (sender is DateEdit)
			{
				if (FormMode != Enums.FormViewModeType.Add && !Function.ToString(txtStatus.EditValue).Equals("D"))
				{
					e.Cancel = true;
				}
			}
			else if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListAtp, false, Function.ToString(txtCode.EditValue), "", Enums.ItemObjectType.NoItemType, null, "")
			{
				AtpType = Function.ToString(txtType.EditValue)
			};
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				txtCode.EditValue = Function.ParseInt(frm_CFL2.ChooseItem["Code"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btnSave_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Run:
				await RunAtp();
				break;
			case Enums.FormViewModeType.Update:
			case Enums.FormViewModeType.Add:
				await Save();
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			case Enums.FormViewModeType.Find:
				txtCode_ButtonClick(null, null);
				break;
			case Enums.FormViewModeType.Busy:
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void grd_Input_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				WinCommons._OpenProcessing();
				ExtenseionMethods.PasteFromExcelToDataTable(grd_Input, gv_Input, new List<string> { gcItemCode.FieldName });
				await AddNewRowInput();
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_PREVIOUS()
	{
		WinCommons._OpenProcessing();
		await SearchData(Enums.NavigateType.Previous);
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		WinCommons._OpenProcessing();
		await SearchData(Enums.NavigateType.Next);
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		WinCommons._OpenProcessing();
		await SearchData(Enums.NavigateType.First);
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		WinCommons._OpenProcessing();
		await SearchData(Enums.NavigateType.Last);
		WinCommons._CloseProcessing();
	}

	public async Task MODE_FIND()
	{
		FormMode = Enums.FormViewModeType.Find;
	}

	public async Task REFRESH()
	{
		FormMode = Enums.FormViewModeType.Busy;
		WinCommons._OpenProcessing();
		try
		{
			SetDataDocControl(null);
			_isLoad = true;
			ButtonEditEx buttonEditEx = txtCode;
			buttonEditEx.EditValue = await GetNewIdToAddNew();
			DateEditEx dateEditEx = txtFromDate;
			object editValue = (txtToDate.EditValue = DateTime.Now.Date);
			dateEditEx.EditValue = editValue;
			DataTable dataSourceTable = grd_Input.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				dataSourceTable.Rows.Clear();
				grd_Input.DataSourceTable.AcceptChanges();
			}
			DataTable dataTable = pgcResult.DataSourceTable();
			if (dataTable.IsNotNull())
			{
				dataTable.Rows.Clear();
				pgcResult.DataSourceTable().AcceptChanges();
			}
			await AddNewRowInput();
			LoadStatus("D");
			txtStatus.EditValue = "D";
			DataTable dataTable2 = txtType.Properties.DataSource as DataTable;
			if (dataTable2.IsNotNull())
			{
				txtType.EditValue = Function.ToString(dataTable2.Rows[0]["Code"]);
			}
			txtName.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoad = false;
		_currentState = Enums.FormViewModeType.Add;
		FormMode = Enums.FormViewModeType.Run;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH_DATA()
	{
		if (FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Find)
		{
			WinCommons._OpenProcessing();
			await SearchData(Enums.NavigateType.Current);
			WinCommons._CloseProcessing();
		}
	}

	private async Task RunAtp()
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRun, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
		{
			return;
		}
		WinCommons._OpenProcessing();
		try
		{
			FormMode = Enums.FormViewModeType.Busy;
			DataTable doc = new DataTable();
			Enums.MessageCode dataDoc = GetDataDoc(ref doc);
			if (dataDoc != Enums.MessageCode.Success)
			{
				FormMode = Enums.FormViewModeType.Run;
				WinCommons.Error(dataDoc, "");
				return;
			}
			doc.TableName = "Doc";
			DataTable dtInput = new DataTable();
			dataDoc = GetDataInput(ref dtInput);
			if (dataDoc != Enums.MessageCode.Success)
			{
				FormMode = Enums.FormViewModeType.Run;
				WinCommons.Error(dataDoc, "");
				return;
			}
			dtInput.TableName = "InputItem";
			DataSet dataSet = new DataSet();
			dataSet.Tables.AddRange(new DataTable[2] { doc, dtInput });
			DataTable dataSource = await _httpClient.AtpClient.RunAtp(dataSet);
			pgcResult.DataSource = dataSource;
			pgcResult.DataSourceTable().AcceptChanges();
			Xtra_Tab.SelectedTabPage = Tab_Result;
			FormMode = _currentState;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			FormMode = Enums.FormViewModeType.Run;
		}
		WinCommons._CloseProcessing();
	}

	private async Task Save()
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
		{
			return;
		}
		WinCommons._OpenProcessing();
		Enums.FormViewModeType oldView = FormMode;
		try
		{
			FormMode = Enums.FormViewModeType.Busy;
			DataTable doc = new DataTable();
			Enums.MessageCode dataDoc = GetDataDoc(ref doc);
			if (dataDoc != Enums.MessageCode.Success)
			{
				FormMode = oldView;
				WinCommons.Error(dataDoc, "");
				return;
			}
			doc.TableName = "Doc";
			DataTable dtInput = new DataTable();
			dataDoc = GetDataInput(ref dtInput);
			if (dataDoc != Enums.MessageCode.Success)
			{
				FormMode = oldView;
				WinCommons.Error(dataDoc, "");
				return;
			}
			dtInput.TableName = "InputItem";
			DataTable dataTable = pgcResult.DataSourceTable().Copy();
			if (!dataTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DataResultIsNullCannotSave, "");
				FormMode = oldView;
				return;
			}
			dataTable.TableName = "Result";
			DataSet dataSet = new DataSet();
			dataSet.Tables.AddRange(new DataTable[3] { doc, dtInput, dataTable });
			HttpResult httpResult = ((_currentState != Enums.FormViewModeType.Add) ? (await _httpClient.AtpClient.AtpUpdate(dataSet)) : (await _httpClient.AtpClient.Save(dataSet)));
			HttpResult httpResult2 = httpResult;
			if (!string.IsNullOrEmpty(httpResult2.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult2.Message);
				FormMode = oldView;
				return;
			}
			_isLoad = true;
			WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			txtCode.EditValue = Function.ParseInt(httpResult2.Content);
			LoadStatus(Function.ToString(txtStatus.EditValue));
			oldView = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = oldView;
		WinCommons._CloseProcessing();
		_isLoad = false;
	}

	private void ChooseFromListItemCode(string itemCode)
	{
		frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, true, itemCode, "", Enums.ItemObjectType.NoItemType, null, "");
		frm_CFL2.ShowDialog(this);
		if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count <= 0)
		{
			return;
		}
		gv_Input.BeginInsertRow();
		DataTable dataSourceTable = grd_Input.DataSourceTable;
		int focusedDataSourceRowIndex = gv_Input.GetFocusedDataSourceRowIndex();
		dataSourceTable.Rows.RemoveAt(focusedDataSourceRowIndex);
		for (int i = 0; i < frm_CFL2.ChooseItems.Count; i++)
		{
			DataRow row = dataSourceTable.NewRow();
			dataSourceTable.Rows.InsertAt(row, focusedDataSourceRowIndex);
			dataSourceTable.AcceptChanges();
			grd_Input.DataSource = dataSourceTable;
			grd_Input.DataSourceTable.AcceptChanges();
			int num = gv_Input.FindRowHandleByDataRow(row);
			if (num >= 0)
			{
				gv_Input.FocusedRowHandle = num;
				gv_Input.SetFocusedRowCellValue(gcItemCode, Function.ToString(frm_CFL2.ChooseItems[i]["ItemCode"]));
			}
		}
		gv_Input.EndInsertRow();
		grd_Input.DataSourceTable.AcceptChanges();
	}

	private async Task AddNewRowInput()
	{
		try
		{
			DataTable dataTable = grd_Input.DataSourceTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = await _httpClient.AtpClient.GetStructureInput();
			}
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.CannotGetDataToAddNewInputTab, "");
			}
			else if (!dataTable.IsNotNull() || dataTable.Select($"ItemCode is null or ItemCode = ''").Length == 0)
			{
				DataRow row = dataTable.NewRow();
				dataTable.Rows.Add(row);
				grd_Input.DataSource = dataTable;
				grd_Input.DataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<int> GetNewIdToAddNew()
	{
		try
		{
			HttpResult httpResult = await _httpClient.AtpClient.GetNewIdAtp();
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				return Function.ParseInt(httpResult.Content);
			}
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return 0;
	}

	private async Task SearchData(Enums.NavigateType type)
	{
		Enums.FormViewModeType oldView = FormMode;
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			DataSet dataSet = await _httpClient.AtpClient.SearchData(type, Function.ParseInt(txtCode.EditValue));
			if (dataSet == null || dataSet.Tables.Count != 3 || !dataSet.Tables[0].IsNotNull())
			{
				if (type == Enums.NavigateType.Current)
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				}
			}
			else
			{
				SetDataDocControl(dataSet.Tables[0]);
				grd_Input.DataSource = dataSet.Tables[1];
				pgcResult.DataSource = dataSet.Tables[2];
				grd_Input.DataSourceTable.AcceptChanges();
				pgcResult.DataSourceTable().AcceptChanges();
				await AddNewRowInput();
				Xtra_Tab.SelectedTabPage = Tab_Result;
				oldView = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = oldView;
	}

	private async Task LoadAtpType()
	{
		try
		{
			RepositoryItemLookUpEdit properties = txtType.Properties;
			properties.DataSource = await _httpClient.AtpClient.LoadAtpType();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadStatus(string currentStatus)
	{
		try
		{
			Dictionary<string, string> dataSource = new Dictionary<string, string>();
			switch (currentStatus)
			{
			case "D":
				dataSource = new Dictionary<string, string>
				{
					{ "D", "Draft" },
					{ "P", "Pending" },
					{ "A", "Approved" },
					{ "C", "Closed" }
				};
				break;
			case "P":
				dataSource = new Dictionary<string, string>
				{
					{ "P", "Pending" },
					{ "A", "Approved" },
					{ "C", "Closed" }
				};
				break;
			case "A":
				dataSource = new Dictionary<string, string>
				{
					{ "A", "Approved" },
					{ "C", "Closed" }
				};
				break;
			case "C":
				dataSource = new Dictionary<string, string> { { "C", "Closed" } };
				break;
			}
			txtStatus.Properties.DataSource = dataSource;
			txtStatus.EditValue = currentStatus;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private Enums.MessageCode GetDataDoc(ref DataTable doc)
	{
		if (_docStructure == null || _docStructure.Columns.Count == 0)
		{
			return Enums.MessageCode.CanNotGetHeaderStruc2Save;
		}
		doc = _docStructure.Clone();
		DataRow dataRow = doc.NewRow();
		if (string.IsNullOrEmpty(Function.ToString(txtType.EditValue)))
		{
			txtType.Focus();
			return Enums.MessageCode.YouMustChooseAtpType;
		}
		if (string.IsNullOrEmpty(Function.ToString(txtFromDate.EditValue)))
		{
			txtFromDate.Focus();
			return Enums.MessageCode.YouDoNotEnterFromDate;
		}
		if (string.IsNullOrEmpty(Function.ToString(txtToDate.EditValue)))
		{
			txtToDate.Focus();
			return Enums.MessageCode.YouDoNotEnterToDate;
		}
		if (string.IsNullOrEmpty(Function.ToString(txtStatus.EditValue)))
		{
			txtStatus.Focus();
			return Enums.MessageCode.YouMustEnterAtpStatus;
		}
		if (Function.ParseDateTimes(txtFromDate.EditValue).Date > Function.ParseDateTimes(txtToDate.EditValue))
		{
			txtFromDate.Focus();
			return Enums.MessageCode.ToDateMustGreaterOrEqualFromDate;
		}
		dataRow["Type"] = Function.ToString(txtType.EditValue);
		dataRow["Code"] = Function.ParseInt(txtCode.EditValue);
		dataRow["Name"] = Function.ToString(txtName.EditValue);
		dataRow["FromDate"] = Function.ParseDateTime(txtFromDate.EditValue);
		dataRow["ToDate"] = Function.ParseDateTime(txtToDate.EditValue);
		dataRow["Status"] = Function.ToString(txtStatus.EditValue);
		dataRow["BplId"] = Function.ParseInt(txtBranch.EditValue);
		dataRow["Remarks"] = Function.ToString(txtRemark.EditValue);
		doc.Rows.Add(dataRow);
		doc.AcceptChanges();
		return Enums.MessageCode.Success;
	}

	private Enums.MessageCode GetDataInput(ref DataTable dtInput)
	{
		dtInput = grd_Input.DataSourceTable.Copy();
		List<DataRow> list = (from t in dtInput.AsEnumerable()
			where string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))
			select t).ToList();
		for (int num = 0; num < list.Count; num++)
		{
			dtInput.Rows.Remove(list[num]);
		}
		dtInput.AcceptChanges();
		for (int num2 = 0; num2 < dtInput.Rows.Count; num2++)
		{
			if (!string.IsNullOrEmpty(Function.ToString(dtInput.Rows[num2][gcItemCode.FieldName])))
			{
				if (string.IsNullOrEmpty(Function.ToString(dtInput.Rows[num2][gcWhsCode.FieldName])))
				{
					Xtra_Tab.SelectedTabPage = Tab_Input;
					grd_Input.Select();
					gv_Input.FocusedRowHandle = num2;
					gv_Input.FocusedColumn = gcWhsCode;
					gv_Input.ShowEditor();
					return Enums.MessageCode.YouMustEnterWhsCode;
				}
				if (string.IsNullOrEmpty(Function.ToString(dtInput.Rows[num2][gcShipDate.FieldName])) || Function.ParseDateTimes(dtInput.Rows[num2][gcShipDate.FieldName]).Date < Function.ParseDateTimes(txtFromDate.EditValue) || Function.ParseDateTimes(dtInput.Rows[num2][gcShipDate.FieldName]).Date > Function.ParseDateTimes(txtToDate.EditValue))
				{
					Xtra_Tab.SelectedTabPage = Tab_Input;
					grd_Input.Select();
					gv_Input.FocusedRowHandle = num2;
					gv_Input.FocusedColumn = gcShipDate;
					gv_Input.ShowEditor();
					return Enums.MessageCode.ShipdateMustBetweenFromDateAndToDate;
				}
			}
		}
		return Enums.MessageCode.Success;
	}

	private async Task GetDocStructure()
	{
		try
		{
			_docStructure = await _httpClient.AtpClient.GetDocStructure();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadDataBranch()
	{
		try
		{
			if (!GlobalData.IsMultiBranch)
			{
				txtBranch.Enabled = false;
				return;
			}
			DataTable dataBrach = _httpClient.Commons.GetDataBrach(GlobalData.UserCodeLogin);
			txtBranch.Properties.DataSource = dataBrach;
			txtBranch.EditValue = GlobalData.DefaultBranch;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadItemGroupData()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemGroups");
			DataTable dataSource = httpClientHelper.Get("GetItemGroup");
			rItemGroup.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void InitFormatControl()
	{
		try
		{
			txtFromDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			txtToDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			fiel_Date.SetFormatControl(Enums.DisplayFormatType.DateTime);
			fiel_Qty.SetFormatControl(Enums.DisplayFormatType.Qty);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetDataDocControl(DataTable dt)
	{
		try
		{
			_isLoad = true;
			if (dt.IsNotNull())
			{
				txtBranch.EditValue = Function.ParseInt(dt.Rows[0]["BplId"]);
				txtCode.EditValue = Function.ParseInt(dt.Rows[0]["Code"]);
				txtName.EditValue = Function.ToString(dt.Rows[0]["Name"]);
				txtFromDate.EditValue = Function.ParseDateTime(dt.Rows[0]["FromDate"]);
				txtToDate.EditValue = Function.ParseDateTime(dt.Rows[0]["ToDate"]);
				txtStatus.EditValue = Function.ToString(dt.Rows[0]["Status"]);
				txtType.EditValue = Function.ToString(dt.Rows[0]["Type"]);
				txtRemark.EditValue = Function.ToString(dt.Rows[0]["Remarks"]);
			}
			else
			{
				DataTable dataTable = txtType.Properties.DataSource as DataTable;
				txtBranch.EditValue = GlobalData.DefaultBranch;
				txtCode.EditValue = null;
				txtName.EditValue = null;
				txtFromDate.EditValue = null;
				txtToDate.EditValue = null;
				txtStatus.EditValue = "D";
				txtType.EditValue = (dataTable.IsNotNull() ? Function.ToString(dataTable.Rows[0]["Code"]) : null);
				txtRemark.EditValue = null;
			}
			LoadStatus(Function.ToString(txtStatus.EditValue));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoad = false;
	}

	private void SetColorControll(Enums.FormViewModeType type)
	{
		try
		{
			txtBranch.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtCode.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtFromDate.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtName.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtStatus.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtToDate.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtType.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtRemark.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void EnableDisableControl(bool isEnable)
	{
		try
		{
			txtBranch.ReadOnly = !isEnable;
			txtBranch.Properties.Buttons[0].Visible = isEnable;
			txtCode.ReadOnly = !isEnable;
			txtCode.Properties.Buttons[0].Visible = isEnable;
			txtFromDate.ReadOnly = !isEnable;
			txtName.ReadOnly = !isEnable;
			txtStatus.ReadOnly = !isEnable;
			txtToDate.ReadOnly = !isEnable;
			txtType.ReadOnly = !isEnable;
			gv_Input.ReadOnly = !isEnable;
			rItemCode.Buttons[0].Visible = isEnable;
			rWhsCode.Buttons[0].Visible = isEnable;
			rInputDateEdit.Buttons[0].Visible = isEnable;
			rInputCal.Buttons[0].Visible = isEnable;
			txtRemark.ReadOnly = !isEnable;
			deleteRowToolStripMenuItem.Enabled = isEnable;
			dupllicateRowToolStripMenuItem.Enabled = isEnable;
			btnCancel.Enabled = isEnable;
			btnSave.Enabled = isEnable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Input_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			DateTime? dateTime = Function.ParseDateTime(gv_Input.GetRowCellValue(e.RowHandle, gcShipDate));
			if (dateTime < txtFromDate.DateTime.Date || dateTime > txtToDate.DateTime.Date)
			{
				WinCommons.Error(Enums.MessageCode.ShipdateMustBetweenFromDateAndToDate, "");
				gv_Input.FocusedColumn = gcShipDate;
				gv_Input.FocusedRowHandle = e.RowHandle;
				gv_Input.ShowEditor();
				e.Allow = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Sales.frm_AvailableToPromise));
		this.Xtra_Tab = new Apzon.Controls.Controls.XtraTabControlEx();
		this.Tab_Input = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Input = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.dupllicateRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Input = new Apzon.Controls.Grid.GridViewEx();
		this.gcItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcWhsCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rWhsCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcShipDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rInputDateEdit = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcQty = new Apzon.Controls.Grid.GridColumnEx();
		this.rInputCal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.Tab_Result = new Apzon.Controls.Controls.XtraTabPageEx();
		this.pgcResult = new Apzon.Controls.Pivot.PivotGridControlEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItemFilter1 = new Apzon.Forms.UserControls.ToolStripMenuItemFilter(this.components);
		this.field_ItemCode = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_Name = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_ItemGroup = new DevExpress.XtraPivotGrid.PivotGridField();
		this.rItemGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.field_Note = new DevExpress.XtraPivotGrid.PivotGridField();
		this.fiel_Date = new DevExpress.XtraPivotGrid.PivotGridField();
		this.fiel_Qty = new DevExpress.XtraPivotGrid.PivotGridField();
		this.txtFilterField = new Apzon.Controls.Editor.LookUpEditEx();
		this.txtFilterValue = new Apzon.Forms.UserControls.FilterControl.FilterValueControlEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnSave = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbType = new Apzon.Controls.Controls.LabelLine();
		this.lbFromDate = new Apzon.Controls.Controls.LabelLine();
		this.lbToDate = new Apzon.Controls.Controls.LabelLine();
		this.txtType = new DevExpress.XtraEditors.LookUpEdit();
		this.txtFromDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtToDate = new Apzon.Controls.Editor.DateEditEx();
		this.lbAtpName = new Apzon.Controls.Controls.LabelLine();
		this.lbStatus = new Apzon.Controls.Controls.LabelLine();
		this.lbRemarks = new Apzon.Controls.Controls.LabelLine();
		this.lbBranch = new Apzon.Controls.Controls.LabelLine();
		this.lbATPCode = new Apzon.Controls.Controls.LabelLine();
		this.txtName = new Apzon.Controls.Editor.TextEditEx();
		this.txtStatus = new DevExpress.XtraEditors.LookUpEdit();
		this.txtRemark = new Apzon.Controls.Editor.TextEditEx();
		this.txtBranch = new DevExpress.XtraEditors.LookUpEdit();
		this.txtCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.lbFilterField = new Apzon.Controls.Controls.LabelLine();
		this.lbFilterValue = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.Xtra_Tab).BeginInit();
		this.Xtra_Tab.SuspendLayout();
		this.Tab_Input.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Input).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Input).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rWhsCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rInputDateEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rInputDateEdit.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rInputCal).BeginInit();
		this.Tab_Result.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pgcResult).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.rItemGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFilterField.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFilterValue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtRemark.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBranch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).BeginInit();
		base.SuspendLayout();
		this.Xtra_Tab.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.Xtra_Tab.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.Xtra_Tab.IsSearchControl = false;
		this.Xtra_Tab.Location = new System.Drawing.Point(0, 65);
		this.Xtra_Tab.Name = "Xtra_Tab";
		this.Xtra_Tab.SelectedTabPage = this.Tab_Input;
		this.Xtra_Tab.Size = new System.Drawing.Size(1045, 468);
		this.Xtra_Tab.TabIndex = 0;
		this.Xtra_Tab.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.Tab_Input, this.Tab_Result });
		this.Tab_Input.Controls.Add(this.grd_Input);
		this.Tab_Input.EditControl = null;
		this.Tab_Input.EditValue = "Input";
		this.Tab_Input.FieldName = "Tab_Input";
		this.Tab_Input.IsSearchControl = false;
		this.Tab_Input.Name = "Tab_Input";
		this.Tab_Input.ReadOnly = false;
		this.Tab_Input.SettingInVisible = false;
		this.Tab_Input.SettingReadonly = false;
		this.Tab_Input.Size = new System.Drawing.Size(1043, 443);
		this.Tab_Input.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_Input.Text = "Input";
		this.grd_Input.AllowPasteExcel = true;
		this.grd_Input.ClipboardData = null;
		this.grd_Input.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Input.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Input.Location = new System.Drawing.Point(0, 0);
		this.grd_Input.MainView = this.gv_Input;
		this.grd_Input.Name = "grd_Input";
		this.grd_Input.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rItemCode, this.rWhsCode, this.rInputDateEdit, this.rInputCal });
		this.grd_Input.Size = new System.Drawing.Size(1043, 443);
		this.grd_Input.TabIndex = 0;
		this.grd_Input.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Input });
		this.grd_Input.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_Input_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.deleteRowToolStripMenuItem, this.dupllicateRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(154, 48);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.dupllicateRowToolStripMenuItem.Name = "dupllicateRowToolStripMenuItem";
		this.dupllicateRowToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
		this.dupllicateRowToolStripMenuItem.Text = "Dupllicate Row";
		this.dupllicateRowToolStripMenuItem.Click += new System.EventHandler(this.dupllicateRowToolStripMenuItem_Click);
		this.gv_Input.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Input.ColumnKey = "ItemCode";
		this.gv_Input.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gcItemCode, this.gcItemName, this.gcWhsCode, this.gcShipDate, this.gcQty });
		this.gv_Input.DisableFormSetting = false;
		this.gv_Input.DisplayException = false;
		this.gv_Input.EditColumn = null;
		this.gv_Input.FieldCurrency = null;
		this.gv_Input.FieldRate = null;
		this.gv_Input.FormId = "oAvaliableToPromise";
		this.gv_Input.GridControl = this.grd_Input;
		this.gv_Input.IgnoreFilter = false;
		this.gv_Input.IndexEditRow = -1;
		this.gv_Input.IndicatorWidth = 40;
		this.gv_Input.IsApzTable = false;
		this.gv_Input.IsInsertNewRow = false;
		this.gv_Input.IsLoadingData = false;
		this.gv_Input.IsMappingContextAction = false;
		this.gv_Input.ItemId = "APZ_OATP";
		this.gv_Input.LstColumnAllowBatch = null;
		this.gv_Input.LstColumnNotAllowBatch = null;
		this.gv_Input.LstColumnNotEditable = null;
		this.gv_Input.LstColumNotSetting = null;
		this.gv_Input.Name = "gv_Input";
		this.gv_Input.OptionsSelection.MultiSelect = true;
		this.gv_Input.OptionsView.ColumnAutoWidth = false;
		this.gv_Input.OptionsView.ShowGroupPanel = false;
		this.gv_Input.UseBatchMod = false;
		this.gv_Input.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Input_CustomDrawRowIndicator);
		this.gv_Input.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Input_ShowingEditor);
		this.gv_Input.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Input_CellValueChanged);
		this.gv_Input.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Input_BeforeLeaveRow);
		this.gcItemCode.Caption = "Item Code";
		this.gcItemCode.ColumnEdit = this.rItemCode;
		this.gcItemCode.CurrencyDateField = null;
		this.gcItemCode.CurrencyField = null;
		this.gcItemCode.CurrencyFixed = null;
		this.gcItemCode.CurrencyFixedRate = 0.0;
		this.gcItemCode.CurrencyRateField = null;
		this.gcItemCode.FieldName = "ItemCode";
		this.gcItemCode.Name = "gcItemCode";
		this.gcItemCode.PrimaryKey = true;
		this.gcItemCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcItemCode.UnitField = null;
		this.gcItemCode.Visible = true;
		this.gcItemCode.VisibleIndex = 0;
		this.gcItemCode.Width = 100;
		this.rItemCode.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rItemCode.Name = "rItemCode";
		this.rItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rItemCode_ButtonClick);
		this.gcItemName.Caption = "Item Name";
		this.gcItemName.CurrencyDateField = null;
		this.gcItemName.CurrencyField = null;
		this.gcItemName.CurrencyFixed = null;
		this.gcItemName.CurrencyFixedRate = 0.0;
		this.gcItemName.CurrencyRateField = null;
		this.gcItemName.FieldName = "ItemName";
		this.gcItemName.Name = "gcItemName";
		this.gcItemName.OptionsColumn.ReadOnly = true;
		this.gcItemName.PrimaryKey = false;
		this.gcItemName.UnitField = null;
		this.gcItemName.Visible = true;
		this.gcItemName.VisibleIndex = 1;
		this.gcWhsCode.Caption = "Whs Code";
		this.gcWhsCode.ColumnEdit = this.rWhsCode;
		this.gcWhsCode.CurrencyDateField = null;
		this.gcWhsCode.CurrencyField = null;
		this.gcWhsCode.CurrencyFixed = null;
		this.gcWhsCode.CurrencyFixedRate = 0.0;
		this.gcWhsCode.CurrencyRateField = null;
		this.gcWhsCode.FieldName = "WhsCode";
		this.gcWhsCode.Name = "gcWhsCode";
		this.gcWhsCode.PrimaryKey = false;
		this.gcWhsCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcWhsCode.UnitField = null;
		this.gcWhsCode.Visible = true;
		this.gcWhsCode.VisibleIndex = 2;
		this.gcWhsCode.Width = 100;
		this.rWhsCode.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rWhsCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rWhsCode.Name = "rWhsCode";
		this.rWhsCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rWhsCode_ButtonClick);
		this.gcShipDate.Caption = "Ship Date";
		this.gcShipDate.ColumnEdit = this.rInputDateEdit;
		this.gcShipDate.CurrencyDateField = null;
		this.gcShipDate.CurrencyField = null;
		this.gcShipDate.CurrencyFixed = null;
		this.gcShipDate.CurrencyFixedRate = 0.0;
		this.gcShipDate.CurrencyRateField = null;
		this.gcShipDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcShipDate.FieldName = "ShipDate";
		this.gcShipDate.Name = "gcShipDate";
		this.gcShipDate.PrimaryKey = false;
		this.gcShipDate.UnitField = null;
		this.gcShipDate.Visible = true;
		this.gcShipDate.VisibleIndex = 3;
		this.gcShipDate.Width = 119;
		this.rInputDateEdit.AutoHeight = false;
		this.rInputDateEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rInputDateEdit.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rInputDateEdit.Name = "rInputDateEdit";
		this.gcQty.Caption = "Quantity";
		this.gcQty.ColumnEdit = this.rInputCal;
		this.gcQty.CurrencyDateField = null;
		this.gcQty.CurrencyField = null;
		this.gcQty.CurrencyFixed = null;
		this.gcQty.CurrencyFixedRate = 0.0;
		this.gcQty.CurrencyRateField = null;
		this.gcQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcQty.FieldName = "Qty";
		this.gcQty.Name = "gcQty";
		this.gcQty.PrimaryKey = false;
		this.gcQty.UnitField = null;
		this.gcQty.Visible = true;
		this.gcQty.VisibleIndex = 4;
		this.gcQty.Width = 129;
		this.rInputCal.AutoHeight = false;
		this.rInputCal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rInputCal.Name = "rInputCal";
		this.Tab_Result.Controls.Add(this.pgcResult);
		this.Tab_Result.EditControl = null;
		this.Tab_Result.EditValue = "Result";
		this.Tab_Result.FieldName = "Tab_Result";
		this.Tab_Result.IsSearchControl = false;
		this.Tab_Result.Name = "Tab_Result";
		this.Tab_Result.ReadOnly = false;
		this.Tab_Result.SettingInVisible = false;
		this.Tab_Result.SettingReadonly = false;
		this.Tab_Result.Size = new System.Drawing.Size(1043, 443);
		this.Tab_Result.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_Result.Text = "Result";
		this.pgcResult.AllowPasteExcel = false;
		this.pgcResult.ClipboardData = null;
		this.pgcResult.ContextMenuStrip = this.contextMenuStrip2;
		this.pgcResult.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pgcResult.Fields.AddRange(new DevExpress.XtraPivotGrid.PivotGridField[6] { this.field_ItemCode, this.field_Name, this.field_ItemGroup, this.field_Note, this.fiel_Date, this.fiel_Qty });
		this.pgcResult.FilterFieldControl = this.txtFilterField;
		this.pgcResult.FilterValueControl = this.txtFilterValue;
		this.pgcResult.FormId = "oAvaliableToPromise";
		this.pgcResult.ItemId = "APZ_ATP1";
		this.pgcResult.Location = new System.Drawing.Point(0, 0);
		this.pgcResult.Name = "pgcResult";
		this.pgcResult.ObjectType = Apzon.Commons.ApzObjectTypes.oAtp;
		this.pgcResult.OptionsView.ShowButtonMode = DevExpress.XtraPivotGrid.PivotShowButtonModeEnum.ShowAlways;
		this.pgcResult.OptionsView.ShowFilterHeaders = false;
		this.pgcResult.OptionsView.ShowRowGrandTotals = false;
		this.pgcResult.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rItemGroup });
		this.pgcResult.Size = new System.Drawing.Size(1043, 443);
		this.pgcResult.TabIndex = 0;
		this.pgcResult.CustomCellDisplayText += new DevExpress.XtraPivotGrid.PivotCellDisplayTextEventHandler(this.pgcResult_CustomCellDisplayText);
		this.pgcResult.CustomAppearance += new DevExpress.XtraPivotGrid.PivotCustomAppearanceEventHandler(this.pgcResult_CustomAppearance);
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripMenuItemFilter1 });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(141, 26);
		this.toolStripMenuItemFilter1.DataSource = null;
		this.toolStripMenuItemFilter1.Name = "toolStripMenuItemFilter1";
		this.toolStripMenuItemFilter1.objType = Apzon.Commons.ApzObjectTypes.oAtp;
		this.toolStripMenuItemFilter1.pivod = this.pgcResult;
		this.toolStripMenuItemFilter1.Size = new System.Drawing.Size(140, 22);
		this.toolStripMenuItemFilter1.Text = "Filter Setting";
		this.field_ItemCode.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_ItemCode.AreaIndex = 0;
		this.field_ItemCode.Caption = "Item No.";
		this.field_ItemCode.FieldName = "ItemCode";
		this.field_ItemCode.Name = "field_ItemCode";
		this.field_Name.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_Name.AreaIndex = 1;
		this.field_Name.Caption = "Item Name";
		this.field_Name.FieldName = "ItemName";
		this.field_Name.Name = "field_Name";
		this.field_ItemGroup.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_ItemGroup.AreaIndex = 2;
		this.field_ItemGroup.Caption = "Item Group";
		this.field_ItemGroup.FieldEdit = this.rItemGroup;
		this.field_ItemGroup.FieldName = "ItmGrpName";
		this.field_ItemGroup.Name = "field_ItemGroup";
		this.field_ItemGroup.Options.AllowEdit = false;
		this.field_ItemGroup.Options.ReadOnly = true;
		this.rItemGroup.AutoHeight = false;
		this.rItemGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ITMSGRPNAM", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ITMSGRPCOD", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rItemGroup.DisplayMember = "ITMSGRPNAM";
		this.rItemGroup.Name = "rItemGroup";
		this.rItemGroup.NullText = "";
		this.rItemGroup.ShowFooter = false;
		this.rItemGroup.ShowHeader = false;
		this.rItemGroup.ValueMember = "ITMSGRPCOD";
		this.field_Note.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_Note.AreaIndex = 3;
		this.field_Note.Caption = "Note";
		this.field_Note.FieldName = "Note";
		this.field_Note.Name = "field_Note";
		this.fiel_Date.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
		this.fiel_Date.AreaIndex = 0;
		this.fiel_Date.Caption = "Date";
		this.fiel_Date.FieldName = "DueDate";
		this.fiel_Date.Name = "fiel_Date";
		this.fiel_Qty.Appearance.CellGrandTotal.BackColor = System.Drawing.Color.PeachPuff;
		this.fiel_Qty.Appearance.CellGrandTotal.BackColor2 = System.Drawing.Color.PeachPuff;
		this.fiel_Qty.Appearance.CellGrandTotal.Options.UseBackColor = true;
		this.fiel_Qty.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
		this.fiel_Qty.AreaIndex = 0;
		this.fiel_Qty.Caption = "Quantity";
		this.fiel_Qty.FieldName = "Qty";
		this.fiel_Qty.Name = "fiel_Qty";
		this.txtFilterField.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.txtFilterField.EditControl = null;
		this.txtFilterField.FieldName = null;
		this.txtFilterField.IsSearchControl = false;
		this.txtFilterField.Location = new System.Drawing.Point(95, 535);
		this.txtFilterField.Name = "txtFilterField";
		this.txtFilterField.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFilterField.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("FilterCode", "FilterCode"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("FilterName", "FilterName")
		});
		this.txtFilterField.Properties.DisplayMember = "FilterName";
		this.txtFilterField.Properties.NullText = "";
		this.txtFilterField.Properties.ValueMember = "FilterCode";
		this.txtFilterField.SettingInVisible = false;
		this.txtFilterField.SettingReadonly = false;
		this.txtFilterField.Size = new System.Drawing.Size(160, 20);
		this.txtFilterField.TabIndex = 9;
		this.txtFilterValue.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.txtFilterValue.EditControl = null;
		this.txtFilterValue.FieldName = null;
		this.txtFilterValue.IsSearchControl = false;
		this.txtFilterValue.Location = new System.Drawing.Point(352, 535);
		this.txtFilterValue.Name = "txtFilterValue";
		this.txtFilterValue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txtFilterValue.SettingInVisible = false;
		this.txtFilterValue.SettingReadonly = false;
		this.txtFilterValue.Size = new System.Drawing.Size(160, 20);
		this.txtFilterValue.SourceFilter = null;
		this.txtFilterValue.TabIndex = 12;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(960, 536);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 20);
		this.btnCancel.TabIndex = 1;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnSave.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btnSave.EditControl = null;
		this.btnSave.EditValue = "Save";
		this.btnSave.FieldName = null;
		this.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		this.btnSave.IsSearchControl = false;
		this.btnSave.Location = new System.Drawing.Point(878, 536);
		this.btnSave.Name = "btnSave";
		this.btnSave.ReadOnly = false;
		this.btnSave.SettingInVisible = false;
		this.btnSave.SettingReadonly = false;
		this.btnSave.Size = new System.Drawing.Size(75, 20);
		this.btnSave.TabIndex = 2;
		this.btnSave.Text = "Save";
		this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
		this.lbType.EditControl = null;
		this.lbType.EditValue = "ATP Type";
		this.lbType.FieldName = null;
		this.lbType.HideWhenEmpty = false;
		this.lbType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbType.Location = new System.Drawing.Point(14, 10);
		this.lbType.Name = "lbType";
		this.lbType.ReadOnly = false;
		this.lbType.SettingInVisible = false;
		this.lbType.SettingReadonly = false;
		this.lbType.ShowUnderLine = true;
		this.lbType.Size = new System.Drawing.Size(80, 20);
		this.lbType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbType.TabIndex = 0;
		this.lbType.Utext = "ATP Type";
		this.lbFromDate.EditControl = null;
		this.lbFromDate.EditValue = "From Date";
		this.lbFromDate.FieldName = null;
		this.lbFromDate.HideWhenEmpty = false;
		this.lbFromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbFromDate.Location = new System.Drawing.Point(221, 39);
		this.lbFromDate.Name = "lbFromDate";
		this.lbFromDate.ReadOnly = false;
		this.lbFromDate.SettingInVisible = false;
		this.lbFromDate.SettingReadonly = false;
		this.lbFromDate.ShowUnderLine = true;
		this.lbFromDate.Size = new System.Drawing.Size(80, 20);
		this.lbFromDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbFromDate.TabIndex = 0;
		this.lbFromDate.Utext = "From Date";
		this.lbToDate.EditControl = null;
		this.lbToDate.EditValue = "To Date";
		this.lbToDate.FieldName = null;
		this.lbToDate.HideWhenEmpty = false;
		this.lbToDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbToDate.Location = new System.Drawing.Point(429, 39);
		this.lbToDate.Name = "lbToDate";
		this.lbToDate.ReadOnly = false;
		this.lbToDate.SettingInVisible = false;
		this.lbToDate.SettingReadonly = false;
		this.lbToDate.ShowUnderLine = true;
		this.lbToDate.Size = new System.Drawing.Size(80, 20);
		this.lbToDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbToDate.TabIndex = 0;
		this.lbToDate.Utext = "To Date";
		this.txtType.Location = new System.Drawing.Point(94, 7);
		this.txtType.Name = "txtType";
		this.txtType.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtType.Properties.AppearanceFocused.BackColor2 = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtType.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtType.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtType.Properties.AppearanceReadOnly.BackColor2 = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtType.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txtType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name1", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name2")
		});
		this.txtType.Properties.DisplayMember = "Name";
		this.txtType.Properties.DropDownRows = 5;
		this.txtType.Properties.NullText = "";
		this.txtType.Properties.ShowFooter = false;
		this.txtType.Properties.ShowHeader = false;
		this.txtType.Properties.ValueMember = "Code";
		this.txtType.Size = new System.Drawing.Size(120, 20);
		this.txtType.TabIndex = 1;
		this.txtFromDate.EditControl = null;
		this.txtFromDate.EditValue = null;
		this.txtFromDate.FieldName = null;
		this.txtFromDate.Location = new System.Drawing.Point(301, 36);
		this.txtFromDate.Name = "txtFromDate";
		this.txtFromDate.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtFromDate.Properties.AppearanceFocused.BackColor2 = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtFromDate.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtFromDate.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtFromDate.Properties.AppearanceReadOnly.BackColor2 = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtFromDate.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txtFromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.IsSearchControl = false;
		this.txtFromDate.SettingInVisible = false;
		this.txtFromDate.SettingReadonly = false;
		this.txtFromDate.Size = new System.Drawing.Size(120, 20);
		this.txtFromDate.TabIndex = 6;
		this.txtFromDate.EditValueChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.txtFromDate.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.txtToDate.EditControl = null;
		this.txtToDate.EditValue = null;
		this.txtToDate.FieldName = null;
		this.txtToDate.Location = new System.Drawing.Point(509, 36);
		this.txtToDate.Name = "txtToDate";
		this.txtToDate.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtToDate.Properties.AppearanceFocused.BackColor2 = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtToDate.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtToDate.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtToDate.Properties.AppearanceReadOnly.BackColor2 = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtToDate.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txtToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtToDate.Properties.IsSearchControl = false;
		this.txtToDate.SettingInVisible = false;
		this.txtToDate.SettingReadonly = false;
		this.txtToDate.Size = new System.Drawing.Size(120, 20);
		this.txtToDate.TabIndex = 7;
		this.txtToDate.EditValueChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.txtToDate.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.lbAtpName.EditControl = null;
		this.lbAtpName.EditValue = "ATP Name";
		this.lbAtpName.FieldName = null;
		this.lbAtpName.HideWhenEmpty = false;
		this.lbAtpName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbAtpName.Location = new System.Drawing.Point(429, 10);
		this.lbAtpName.Name = "lbAtpName";
		this.lbAtpName.ReadOnly = false;
		this.lbAtpName.SettingInVisible = false;
		this.lbAtpName.SettingReadonly = false;
		this.lbAtpName.ShowUnderLine = true;
		this.lbAtpName.Size = new System.Drawing.Size(80, 20);
		this.lbAtpName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbAtpName.TabIndex = 0;
		this.lbAtpName.Utext = "ATP Name";
		this.lbStatus.EditControl = null;
		this.lbStatus.EditValue = "Status";
		this.lbStatus.FieldName = null;
		this.lbStatus.HideWhenEmpty = false;
		this.lbStatus.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbStatus.Location = new System.Drawing.Point(14, 39);
		this.lbStatus.Name = "lbStatus";
		this.lbStatus.ReadOnly = false;
		this.lbStatus.SettingInVisible = false;
		this.lbStatus.SettingReadonly = false;
		this.lbStatus.ShowUnderLine = true;
		this.lbStatus.Size = new System.Drawing.Size(80, 20);
		this.lbStatus.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbStatus.TabIndex = 0;
		this.lbStatus.Utext = "Status";
		this.lbRemarks.EditControl = null;
		this.lbRemarks.EditValue = "Remarks";
		this.lbRemarks.FieldName = null;
		this.lbRemarks.HideWhenEmpty = false;
		this.lbRemarks.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbRemarks.Location = new System.Drawing.Point(641, 39);
		this.lbRemarks.Name = "lbRemarks";
		this.lbRemarks.ReadOnly = false;
		this.lbRemarks.SettingInVisible = false;
		this.lbRemarks.SettingReadonly = false;
		this.lbRemarks.ShowUnderLine = true;
		this.lbRemarks.Size = new System.Drawing.Size(80, 20);
		this.lbRemarks.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbRemarks.TabIndex = 0;
		this.lbRemarks.Utext = "Remarks";
		this.lbBranch.EditControl = null;
		this.lbBranch.EditValue = "Branch";
		this.lbBranch.FieldName = null;
		this.lbBranch.HideWhenEmpty = false;
		this.lbBranch.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbBranch.Location = new System.Drawing.Point(641, 10);
		this.lbBranch.Name = "lbBranch";
		this.lbBranch.ReadOnly = false;
		this.lbBranch.SettingInVisible = false;
		this.lbBranch.SettingReadonly = false;
		this.lbBranch.ShowUnderLine = true;
		this.lbBranch.Size = new System.Drawing.Size(80, 20);
		this.lbBranch.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbBranch.TabIndex = 0;
		this.lbBranch.Utext = "Branch";
		this.lbATPCode.EditControl = null;
		this.lbATPCode.EditValue = "ATP Code";
		this.lbATPCode.FieldName = null;
		this.lbATPCode.HideWhenEmpty = false;
		this.lbATPCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbATPCode.Location = new System.Drawing.Point(221, 10);
		this.lbATPCode.Name = "lbATPCode";
		this.lbATPCode.ReadOnly = false;
		this.lbATPCode.SettingInVisible = false;
		this.lbATPCode.SettingReadonly = false;
		this.lbATPCode.ShowUnderLine = true;
		this.lbATPCode.Size = new System.Drawing.Size(80, 20);
		this.lbATPCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbATPCode.TabIndex = 0;
		this.lbATPCode.Utext = "ATP Code";
		this.txtName.EditControl = null;
		this.txtName.FieldName = null;
		this.txtName.IsSearchControl = false;
		this.txtName.Location = new System.Drawing.Point(509, 7);
		this.txtName.Name = "txtName";
		this.txtName.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtName.Properties.AppearanceFocused.BackColor2 = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtName.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtName.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtName.Properties.AppearanceReadOnly.BackColor2 = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtName.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txtName.SettingInVisible = false;
		this.txtName.SettingReadonly = false;
		this.txtName.Size = new System.Drawing.Size(120, 20);
		this.txtName.TabIndex = 3;
		this.txtName.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.txtStatus.Location = new System.Drawing.Point(94, 35);
		this.txtStatus.Name = "txtStatus";
		this.txtStatus.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtStatus.Properties.AppearanceFocused.BackColor2 = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtStatus.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtStatus.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtStatus.Properties.AppearanceReadOnly.BackColor2 = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtStatus.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txtStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtStatus.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Key", "Name11", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name12")
		});
		this.txtStatus.Properties.DisplayMember = "Value";
		this.txtStatus.Properties.DropDownRows = 5;
		this.txtStatus.Properties.NullText = "";
		this.txtStatus.Properties.ShowFooter = false;
		this.txtStatus.Properties.ShowHeader = false;
		this.txtStatus.Properties.ValueMember = "Key";
		this.txtStatus.Size = new System.Drawing.Size(120, 20);
		this.txtStatus.TabIndex = 5;
		this.txtStatus.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.txtRemark.EditControl = null;
		this.txtRemark.FieldName = null;
		this.txtRemark.IsSearchControl = false;
		this.txtRemark.Location = new System.Drawing.Point(721, 36);
		this.txtRemark.Name = "txtRemark";
		this.txtRemark.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtRemark.Properties.AppearanceFocused.BackColor2 = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtRemark.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtRemark.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtRemark.Properties.AppearanceReadOnly.BackColor2 = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtRemark.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txtRemark.SettingInVisible = false;
		this.txtRemark.SettingReadonly = false;
		this.txtRemark.Size = new System.Drawing.Size(120, 20);
		this.txtRemark.TabIndex = 8;
		this.txtRemark.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.txtBranch.Location = new System.Drawing.Point(721, 7);
		this.txtBranch.Name = "txtBranch";
		this.txtBranch.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtBranch.Properties.AppearanceFocused.BackColor2 = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtBranch.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtBranch.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtBranch.Properties.AppearanceReadOnly.BackColor2 = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtBranch.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txtBranch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtBranch.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name7"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name8", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.txtBranch.Properties.DisplayMember = "Name";
		this.txtBranch.Properties.NullText = "";
		this.txtBranch.Properties.ShowFooter = false;
		this.txtBranch.Properties.ShowHeader = false;
		this.txtBranch.Properties.ValueMember = "Value";
		this.txtBranch.Size = new System.Drawing.Size(120, 20);
		this.txtBranch.TabIndex = 4;
		this.txtCode.EditControl = null;
		this.txtCode.FieldName = null;
		this.txtCode.IsSearchControl = false;
		this.txtCode.Location = new System.Drawing.Point(301, 8);
		this.txtCode.Name = "txtCode";
		this.txtCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtCode.Properties.AppearanceFocused.BackColor2 = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtCode.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtCode.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtCode.Properties.AppearanceReadOnly.BackColor2 = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtCode.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txtCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txtCode.SettingInVisible = false;
		this.txtCode.SettingReadonly = false;
		this.txtCode.Size = new System.Drawing.Size(120, 20);
		this.txtCode.TabIndex = 2;
		this.txtCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtCode_ButtonClick);
		this.txtCode.EditValueChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.lbFilterField.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbFilterField.EditControl = null;
		this.lbFilterField.EditValue = "Filter by";
		this.lbFilterField.FieldName = null;
		this.lbFilterField.HideWhenEmpty = false;
		this.lbFilterField.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbFilterField.Location = new System.Drawing.Point(14, 538);
		this.lbFilterField.Name = "lbFilterField";
		this.lbFilterField.ReadOnly = false;
		this.lbFilterField.SettingInVisible = false;
		this.lbFilterField.SettingReadonly = false;
		this.lbFilterField.ShowUnderLine = true;
		this.lbFilterField.Size = new System.Drawing.Size(80, 20);
		this.lbFilterField.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbFilterField.TabIndex = 0;
		this.lbFilterField.Utext = "Filter by";
		this.lbFilterValue.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbFilterValue.EditControl = null;
		this.lbFilterValue.EditValue = "Filter value";
		this.lbFilterValue.FieldName = null;
		this.lbFilterValue.HideWhenEmpty = false;
		this.lbFilterValue.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbFilterValue.Location = new System.Drawing.Point(270, 538);
		this.lbFilterValue.Name = "lbFilterValue";
		this.lbFilterValue.ReadOnly = false;
		this.lbFilterValue.SettingInVisible = false;
		this.lbFilterValue.SettingReadonly = false;
		this.lbFilterValue.ShowUnderLine = true;
		this.lbFilterValue.Size = new System.Drawing.Size(80, 20);
		this.lbFilterValue.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbFilterValue.TabIndex = 0;
		this.lbFilterValue.Utext = "Filter value";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1045, 562);
		this.ContextMenuStrip = this.contextMenuStrip2;
		base.Controls.Add(this.txtFilterValue);
		base.Controls.Add(this.lbFilterValue);
		base.Controls.Add(this.lbFilterField);
		base.Controls.Add(this.txtFilterField);
		base.Controls.Add(this.txtCode);
		base.Controls.Add(this.txtBranch);
		base.Controls.Add(this.txtRemark);
		base.Controls.Add(this.txtStatus);
		base.Controls.Add(this.txtName);
		base.Controls.Add(this.lbATPCode);
		base.Controls.Add(this.lbBranch);
		base.Controls.Add(this.lbRemarks);
		base.Controls.Add(this.lbStatus);
		base.Controls.Add(this.lbAtpName);
		base.Controls.Add(this.txtToDate);
		base.Controls.Add(this.txtFromDate);
		base.Controls.Add(this.txtType);
		base.Controls.Add(this.lbToDate);
		base.Controls.Add(this.lbFromDate);
		base.Controls.Add(this.lbType);
		base.Controls.Add(this.btnSave);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.Xtra_Tab);
		base.FormId = "oAvaliableToPromise";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AvailableToPromise";
		this.Text = "Available To Promise";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_AvailableToPromise_Load);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_AvailableToPromise_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.Xtra_Tab).EndInit();
		this.Xtra_Tab.ResumeLayout(false);
		this.Tab_Input.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Input).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Input).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rWhsCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rInputDateEdit.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rInputDateEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rInputCal).EndInit();
		this.Tab_Result.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pgcResult).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.rItemGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFilterField.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFilterValue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtRemark.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBranch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
