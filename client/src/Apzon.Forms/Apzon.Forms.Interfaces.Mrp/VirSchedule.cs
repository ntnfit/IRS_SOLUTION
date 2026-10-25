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
using Apzon.Controls.Pivot;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Mrps;
using Apzon.Forms.Properties;
using Apzon.Forms.RoutingAndResource;
using Apzon.Forms.UserControls;
using Apzon.Forms.UserControls.FilterControl;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid;

namespace Apzon.Forms.Interfaces.Mrp;

public class VirSchedule : XtraFormEx
{
	private const string OkString = "OK";

	private const string FinString = "Find";

	private const string UpdateString = "Update";

	private const string SaveString = "Save";

	protected const string FieldNameDemadEntry = "DemandEntry";

	protected const string FieldNameDemandType = "DemandType";

	protected const string FieldNameDemandLine = "DemandLine";

	private int PsCode = -1;

	private string _itemCode;

	private DateTime? _dueDate;

	private string _shift;

	protected bool _isLoad;

	protected new readonly IHttpClient HttpClient = HttpClientHelperFactory.GetHttpClient(ApzObjectTypes.oSchedule);

	protected DataTable DocStructure;

	protected int ColumnIndexAutoFill = -1;

	private bool _isChangeValue;

	private IContainer components;

	private ContextMenuStrip DocContextMenu;

	private ToolStripMenuItem addNewToolStripMenuItem;

	protected LabelLine lbInputCode;

	protected ButtonEditEx txtInputCode;

	protected LabelLine lbStatus;

	protected LookUpEdit txtStatus;

	protected LabelLine lbfromdate;

	protected DateEditEx txtFromDate;

	protected DateEditEx txtToDate;

	protected LabelLine lbtofstDate;

	protected SimpleButtonEx btn_Save;

	protected SimpleButtonEx btn_Cancel;

	protected LabelLine lbfixdate;

	protected DateEditEx txtFixDate;

	protected LabelLine lbTYpe;

	protected LookUpEdit txtType;

	protected TextEditEx txtName;

	protected LabelLine lbName;

	protected LabelLine lbCode;

	protected ButtonEditEx txtCode;

	protected SimpleButtonEx btnAutoFillData;

	protected PivotGridField field_Quantity;

	private RepositoryItemCalcEdit rQuantity;

	protected PivotGridField field_Shift;

	protected PivotGridField field_WorkDate;

	protected PivotGridField field_Planning;

	protected PivotGridField field_Version;

	protected PivotGridField field_BP;

	private PivotGridField field_ItemName;

	protected PivotGridField field_ItemCode;

	private RepositoryItemLookUpEdit rPlaning;

	protected PivotGridControlEx pgcDetail;

	private ToolStripMenuItem autoFillColumnToolStripMenuItem;

	private LookUpEditEx txtFilterField;

	private LabelLine lbFilterBy;

	private FilterValueControlEx txtFilterValue;

	private ToolStripMenuItemFilter toolStripMenuItemFilter1;

	private ToolStripMenuItem capacityGanttChartToolStripMenuItem;

	protected CheckEditEx chkDetailSchedule;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && value != Enums.FormViewModeType.Find)
			{
				_formMode = Enums.FormViewModeType.View;
				return;
			}
			_formMode = value;
			SetColorControll(value);
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				EnableDisableControl(isEnable: true);
				SetDataStatus("D");
				txtCode.ReadOnly = true;
				txtStatus.ReadOnly = true;
				txtCode.Properties.Buttons[0].Visible = false;
				txtInputCode.Properties.Buttons[0].Visible = true;
				btn_Save.Text = "Save";
				txtCode.Properties.Appearance.BackColor = Color.WhiteSmoke;
				break;
			case Enums.FormViewModeType.Busy:
				EnableDisableControl(isEnable: false);
				break;
			case Enums.FormViewModeType.Find:
				EnableDisableControl(isEnable: true);
				DocContextMenu.Enabled = false;
				txtInputCode.Properties.Buttons[0].Visible = false;
				SetDataDoc(null);
				txtName.ReadOnly = true;
				SetDataPivot(null);
				btn_Save.Text = "Find";
				SetDataStatus("D");
				break;
			case Enums.FormViewModeType.Update:
				EnableDisableControl(isEnable: true);
				txtCode.ReadOnly = true;
				chkDetailSchedule.ReadOnly = true;
				txtInputCode.ReadOnly = true;
				btn_Save.Text = "Update";
				txtCode.Properties.Buttons[0].Visible = false;
				txtInputCode.Properties.Buttons[0].Visible = false;
				txtType.ReadOnly = true;
				break;
			case Enums.FormViewModeType.View:
				EnableDisableControl(isEnable: true);
				chkDetailSchedule.ReadOnly = true;
				txtCode.Properties.Buttons[0].Visible = false;
				txtInputCode.Properties.Buttons[0].Visible = false;
				txtCode.ReadOnly = true;
				txtInputCode.ReadOnly = true;
				txtType.ReadOnly = true;
				btn_Save.Text = "OK";
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public VirSchedule()
	{
		InitializeComponent();
		OnLoadAsync += VirSchedule_Load;
	}

	public VirSchedule(int psCode)
	{
		PsCode = psCode;
		InitializeComponent();
		OnLoadAsync += VirSchedule_Load;
	}

	public VirSchedule(int psCode, string itemCode, DateTime? dueDate, string shiftCode)
	{
		PsCode = psCode;
		_itemCode = itemCode;
		_dueDate = dueDate;
		_shift = shiftCode;
		InitializeComponent();
		OnLoadAsync += VirSchedule_Load;
	}

	private void txtInputCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(txtInputCode.EditValue);
			InputControllButtonClick(sender, e);
			if (!text.Equals(Function.ToString(txtInputCode.EditValue)))
			{
				pgcDetail.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task VirSchedule_Load(object sender, EventArgs e)
	{
		try
		{
			chkDetailSchedule.EditValue = "N";
			await GetDocScheduleStructure();
			SetDataType();
			SetDataStatus("D");
			InitFormatControl();
			if (PsCode > 0)
			{
				txtCode.EditValue = PsCode;
				_isChangeValue = true;
				txtCode_Validating(null, null);
			}
			else
			{
				FormMode = Enums.FormViewModeType.Find;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pgcDetail_CustomAppearance(object sender, PivotCustomAppearanceEventArgs e)
	{
		try
		{
			if (e == null || e.RowField == null || e.ColumnField == null)
			{
				return;
			}
			if (e.RowValueType == PivotGridValueType.Total)
			{
				e.Appearance.BackColor = Color.OldLace;
				e.Appearance.BackColor2 = Color.OldLace;
				return;
			}
			Point focusedCell = pgcDetail.Cells.FocusedCell;
			PivotCellEventArgs cellInfo = pgcDetail.Cells.GetCellInfo(focusedCell.X, focusedCell.Y);
			if (e.RowIndex == cellInfo.RowIndex)
			{
				if (e.ColumnIndex == cellInfo.ColumnIndex && Function.ToString(pgcDetail.GetFieldValue(field_Planning, e.RowIndex)).StartsWith("2."))
				{
					if (e.ColumnField == field_Shift)
					{
						if (string.IsNullOrEmpty(Function.ToString(pgcDetail.GetFieldValue(field_Shift, e.ColumnIndex))))
						{
							e.Appearance.BackColor = field_Quantity.Appearance.Cell.BackColor;
							e.Appearance.BackColor2 = field_Quantity.Appearance.Cell.BackColor2;
							return;
						}
						DateTime? dateTime = Function.ParseDateTime(pgcDetail.GetFieldValue(field_WorkDate, e.ColumnIndex));
						if (dateTime.HasValue && (dateTime.Value.Date > Function.ParseDateTimes(txtFixDate.EditValue).Date || dateTime.Value.Date < DateTime.Now.Date))
						{
							e.Appearance.BackColor = field_Quantity.Appearance.Cell.BackColor;
							e.Appearance.BackColor2 = field_Quantity.Appearance.Cell.BackColor2;
							return;
						}
						PivotDrillDownDataSource pivotDrillDownDataSource = pgcDetail.CreateDrillDownDataSource(e.ColumnIndex, e.RowIndex);
						if (pivotDrillDownDataSource != null && pivotDrillDownDataSource.RowCount > 0 && Function.ParseInt(pivotDrillDownDataSource[0]["DocEntry"]) > 0)
						{
							e.Appearance.BackColor = field_Quantity.Appearance.Cell.BackColor;
							e.Appearance.BackColor2 = field_Quantity.Appearance.Cell.BackColor2;
							return;
						}
					}
					e.Appearance.BackColor = Color.White;
					e.Appearance.BackColor2 = Color.White;
				}
				else
				{
					e.Appearance.BackColor = Color.Moccasin;
				}
				return;
			}
			if (e.ColumnField.FieldName.Equals(field_WorkDate.FieldName) && e.ColumnIndex == 0)
			{
				e.Appearance.BackColor = pgcDetail.BackColor;
			}
			if (e.RowField.FieldName.Equals("Planning") && Function.ToString(pgcDetail.GetFieldValue(field_Planning, e.RowIndex)).StartsWith("2."))
			{
				if (e.ColumnField == field_Shift)
				{
					if (string.IsNullOrEmpty(Function.ToString(pgcDetail.GetFieldValue(field_Shift, e.ColumnIndex))))
					{
						e.Appearance.BackColor = field_Quantity.Appearance.Cell.BackColor;
						e.Appearance.BackColor2 = field_Quantity.Appearance.Cell.BackColor2;
						return;
					}
					DateTime? dateTime2 = Function.ParseDateTime(pgcDetail.GetFieldValue(field_WorkDate, e.ColumnIndex));
					if (dateTime2.HasValue && (dateTime2.Value.Date > Function.ParseDateTimes(txtFixDate.EditValue).Date || dateTime2.Value.Date < DateTime.Now.Date))
					{
						e.Appearance.BackColor = field_Quantity.Appearance.Cell.BackColor;
						e.Appearance.BackColor2 = field_Quantity.Appearance.Cell.BackColor2;
						return;
					}
					PivotDrillDownDataSource pivotDrillDownDataSource2 = pgcDetail.CreateDrillDownDataSource(e.ColumnIndex, e.RowIndex);
					if (pivotDrillDownDataSource2 != null && pivotDrillDownDataSource2.RowCount > 0 && Function.ParseInt(pivotDrillDownDataSource2[0]["DocEntry"]) > 0)
					{
						e.Appearance.BackColor = field_Quantity.Appearance.Cell.BackColor;
						e.Appearance.BackColor2 = field_Quantity.Appearance.Cell.BackColor2;
						return;
					}
				}
				e.Appearance.BackColor = Color.White;
			}
			if (e.ColumnField.FieldName.Equals("Shift"))
			{
				switch (Function.ParseDateTimes(pgcDetail.GetFieldValue(field_WorkDate, e.ColumnIndex)).DayOfWeek)
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

	private void pgcDetail_CustomDrawFieldValue(object sender, PivotCustomDrawFieldValueEventArgs e)
	{
		try
		{
			if (e != null && e.ValueType == PivotGridValueType.Total)
			{
				e.Handled = true;
				Rectangle bounds = e.Bounds;
				bounds.Inflate(-1, -1);
				e.GraphicsCache.FillRectangle(Color.OldLace, bounds);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pivot_Detail_CustomCellDisplayText(object sender, PivotCellDisplayTextEventArgs e)
	{
		try
		{
			if (e != null)
			{
				if (e.RowValueType == PivotGridValueType.Total)
				{
					e.DisplayText = "";
				}
				else if ((e.RowValueType == PivotGridValueType.Value || e.RowValueType == PivotGridValueType.GrandTotal) && Function.ParseDouble(e.Value).Equals(0.0))
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

	private void pivot_Detail_EditValueChanged(object sender, EditValueChangedEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			if (e == null || e.ColumnField == null || e.RowField == null)
			{
				return;
			}
			if (e.DataField == field_Quantity)
			{
				if (e.Editor.EditValue == null)
				{
					e.Editor.EditValue = 0;
				}
				ChangeCellValue(e, Function.ParseDouble(e.Editor.EditValue));
				pgcDetail.ShowEditor();
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

	private void pivot_Detail_ShowingEditor(object sender, CancelPivotCellEditEventArgs e)
	{
		try
		{
			if (e.ColumnValueType == PivotGridValueType.Total || e.ColumnValueType == PivotGridValueType.GrandTotal || e.RowValueType == PivotGridValueType.Total || e.RowValueType == PivotGridValueType.GrandTotal)
			{
				e.Cancel = true;
			}
			if (e.RowField == field_Planning && !Function.ToString(pgcDetail.GetFieldValue(field_Planning, e.RowIndex)).StartsWith("2"))
			{
				e.Cancel = true;
			}
			if (e.ColumnField == field_Shift)
			{
				if (string.IsNullOrEmpty(Function.ToString(pgcDetail.GetFieldValue(field_Shift, e.ColumnIndex))))
				{
					e.Cancel = true;
				}
				DateTime? dateTime = Function.ParseDateTime(pgcDetail.GetFieldValue(field_WorkDate, e.ColumnIndex));
				if (dateTime.HasValue && (dateTime.Value.Date > Function.ParseDateTimes(txtFixDate.EditValue).Date || dateTime.Value.Date < DateTime.Now.Date))
				{
					e.Cancel = true;
				}
				PivotDrillDownDataSource pivotDrillDownDataSource = pgcDetail.CreateDrillDownDataSource(e.ColumnIndex, e.RowIndex);
				if (pivotDrillDownDataSource != null && pivotDrillDownDataSource.RowCount > 0 && Function.ParseInt(pivotDrillDownDataSource[0]["DocEntry"]) > 0)
				{
					e.Cancel = true;
				}
			}
			ShowingEditorExtentions(e);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Find:
			{
				string text = Function.ToString(txtCode.EditValue);
				Find(isFind: true);
				if (!text.Equals(txtCode.EditValue))
				{
					pgcDetail.Focus();
				}
				break;
			}
			case Enums.FormViewModeType.Update:
			case Enums.FormViewModeType.Add:
				await Save();
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
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
			string text = Function.ToString(txtCode.EditValue);
			Find(isFind: false);
			if (!text.Equals(txtCode.EditValue))
			{
				pgcDetail.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void txtCode_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (_isLoad || pgcDetail == null || !_isChangeValue)
			{
				_isChangeValue = false;
				return;
			}
			await GetDataDoc(Enums.NavigateType.Current);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isChangeValue = false;
	}

	private async void txtInputCode_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				if (pgcDetail == null || !_isChangeValue)
				{
					_isChangeValue = false;
					return;
				}
				await GetDataInput();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isChangeValue = false;
	}

	private void txtName_EditValueChanging(object sender, ChangingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void txtStatus_EditValueChanging(object sender, ChangingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void txtFromDate_EditValueChanging(object sender, ChangingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void txtToDate_EditValueChanging(object sender, ChangingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void txtType_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (!_isLoad)
			{
				SetDataDoc(null);
				SetDataPivot(null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			switch (Function.ToString(txtType.EditValue))
			{
			case "O":
			case "T":
				field_Version.Visible = true;
				field_BP.Visible = false;
				break;
			case "P":
				field_BP.Visible = true;
				field_Version.Visible = false;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		REFRESH();
	}

	private void btnAutoFillData_Click(object sender, EventArgs e)
	{
		try
		{
			LoadDataAutoFillClick();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void frm_Schedule_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View && FormMode != Enums.FormViewModeType.Find && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void DocContextMenu_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			ColumnIndexAutoFill = -1;
			Point hitPoint = PointToClient(Cursor.Position);
			if ((hitPoint.X < (field_BP.Visible ? field_BP.Width : 0) + (field_ItemCode.Visible ? field_ItemCode.Width : 0) + (field_Version.Visible ? field_Version.Width : 0) + (field_ItemName.Visible ? field_ItemName.Width : 0) + (field_Planning.Visible ? field_Planning.Width : 0) + 20 * Main.WidScreen / 1920 && hitPoint.Y > pgcDetail.Location.Y) || (hitPoint.Y > pgcDetail.Location.Y && hitPoint.Y < pgcDetail.Location.Y + 75 * Main.HeightScreen / 1080))
			{
				e.Cancel = true;
				return;
			}
			PivotGridHitInfo pivotGridHitInfo = pgcDetail.CalcHitInfo(hitPoint);
			if (pivotGridHitInfo.HitTest == PivotGridHitTest.Cell)
			{
				ColumnIndexAutoFill = pivotGridHitInfo.CellInfo.ColumnIndex;
			}
			autoFillColumnToolStripMenuItem.Visible = ColumnIndexAutoFill != -1;
			capacityGanttChartToolStripMenuItem.Visible = Function.ToString(txtType.EditValue).Equals("O") && !Function.ToString(txtStatus.EditValue).Equals("D");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pgcDetail_FieldValueDisplayText(object sender, PivotFieldDisplayTextEventArgs e)
	{
		try
		{
			if (e != null && e.Field != null && e.ValueType != PivotGridValueType.Total)
			{
				if (e.ValueType == PivotGridValueType.Total)
				{
					e.DisplayText = "";
				}
				else if (e.Field.FieldName.Equals(field_WorkDate.FieldName) && e.FieldIndex == 0 && Function.ParseDateTimes(pgcDetail.GetFieldValue(field_WorkDate, 0)).Date == txtFromDate.DateTime.Date.AddDays(-1.0))
				{
					e.DisplayText = "Over Due";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pgcDetail_CustomCellValue(object sender, PivotCellValueEventArgs e)
	{
		try
		{
			if (e != null && e.ColumnField == null && e.ColumnValueType == PivotGridValueType.GrandTotal && Function.ToString(pgcDetail.GetFieldValue(pgcDetail.Fields[field_Planning.FieldName], e.RowIndex)).StartsWith("4."))
			{
				PivotDrillDownDataSource pivotDrillDownDataSource = pgcDetail.CreateDrillDownDataSource(e.ColumnIndex - 1, e.RowIndex);
				if (pivotDrillDownDataSource != null && pivotDrillDownDataSource.RowCount > 0)
				{
					e.Value = pivotDrillDownDataSource[0][field_Quantity.FieldName];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void autoFillColumnToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			LoadDataAutoFillColumn();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pgcDetail_CustomRowHeight(object sender, PivotCustomRowHeightEventArgs e)
	{
		try
		{
			if (e.ValueType == PivotGridValueType.Total)
			{
				e.RowHeight = e.RowHeight * 2 / 3;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void capacityGanttChartToolStripMenuItem_Click(object sender, EventArgs e)
	{
		frm_CapacityGantt frm_CapacityGantt2 = new frm_CapacityGantt(Function.ParseInt(txtCode.EditValue));
		frm_CapacityGantt2.MdiParent = MdiParent;
		frm_CapacityGantt2.Show();
	}

	protected virtual void pgcDetail_CellDoubleClick(object sender, PivotCellEventArgs e)
	{
	}

	private void chkDetailSchedule_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void Control_EditValueChanging(object sender, ChangingEventArgs e)
	{
		_isChangeValue = true;
	}

	public async Task MOVE_PREVIOUS()
	{
		WinCommons._OpenProcessing();
		try
		{
			await GetDataDoc(Enums.NavigateType.Previous);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		WinCommons._OpenProcessing();
		try
		{
			await GetDataDoc(Enums.NavigateType.Next);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		WinCommons._OpenProcessing();
		try
		{
			await GetDataDoc(Enums.NavigateType.First);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		WinCommons._OpenProcessing();
		try
		{
			await GetDataDoc(Enums.NavigateType.Last);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MODE_FIND()
	{
		FormMode = Enums.FormViewModeType.Find;
	}

	public async Task REFRESH()
	{
		try
		{
			SetDataDoc(null);
			_isLoad = true;
			txtCode.EditValue = GetNewIdToAddNew();
			_isLoad = false;
			SetDataPivot(null);
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH_DATA()
	{
		WinCommons._OpenProcessing();
		try
		{
			await GetDataDoc(Enums.NavigateType.Current);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	protected virtual async Task GetDocScheduleStructure()
	{
		try
		{
			DocStructure = await HttpClient.ScheduleClient.GetDocStructure();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			DocStructure = new DataTable();
		}
	}

	private void InitFormatControl()
	{
		try
		{
			field_WorkDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			txtFromDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			txtToDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			txtFixDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			field_Quantity.SetFormatControl(Enums.DisplayFormatType.Qty);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual void SetDataType()
	{
		try
		{
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Production",
					Code = "O"
				},
				new Common.Table_Code
				{
					Name = "Purchase",
					Code = "P"
				},
				new Common.Table_Code
				{
					Name = "Transfer",
					Code = "T"
				}
			};
			txtType.Properties.DataSource = dataSource;
			txtType.EditValue = "O";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual void Find(bool isFind)
	{
		frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Schedule, false, Function.ToString(txtCode.EditValue), "", Enums.ItemObjectType.NoItemType, null, "")
		{
			MrpCode = (isFind ? Function.ToString(txtInputCode.EditValue) : ""),
			ScheduleStatus = (isFind ? Function.ToString(txtStatus.EditValue) : ""),
			ScheduleType = Function.ToString(txtType.EditValue)
		};
		frm_CFL2.ShowDialog(this);
		if (frm_CFL2.ChooseItem != null)
		{
			txtCode.EditValue = Function.ParseInt(frm_CFL2.ChooseItem["PSCode"]);
			_isChangeValue = true;
			txtCode_Validating(null, null);
			FormMode = Enums.FormViewModeType.View;
		}
	}

	private void ChangeCellValue(EditValueChangedEventArgs e, double newValue)
	{
		try
		{
			PivotDrillDownDataSource pivotDrillDownDataSource = e.CreateDrillDownDataSource();
			int columnCount = pgcDetail.Cells.ColumnCount;
			for (int i = 1; i < columnCount - 1; i++)
			{
				int num = 0;
				if (num >= pivotDrillDownDataSource.RowCount)
				{
					continue;
				}
				PivotDrillDownDataSource pivotDrillDownDataSource2 = pgcDetail.CreateDrillDownDataSource(i, e.RowIndex + 1);
				PivotDrillDownDataSource pivotDrillDownDataSource3 = ((e.RowIndex == 0) ? null : pgcDetail.CreateDrillDownDataSource(i, e.RowIndex - 1));
				PivotDrillDownDataSource pivotDrillDownDataSource4 = pgcDetail.CreateDrillDownDataSource(i, e.RowIndex + 2);
				PivotDrillDownDataSource pivotDrillDownDataSource5 = pgcDetail.CreateDrillDownDataSource(i - 1, e.RowIndex + 2);
				PivotDrillDownDataSource pivotDrillDownDataSource6 = pgcDetail.CreateDrillDownDataSource(i, e.RowIndex);
				double num2 = 0.0;
				double num3 = 0.0;
				if (i == 1)
				{
					PivotDrillDownDataSource pivotDrillDownDataSource7 = ((e.RowIndex == 0) ? null : pgcDetail.CreateDrillDownDataSource(0, e.RowIndex - 1));
					if (pivotDrillDownDataSource7 != null && pivotDrillDownDataSource7.RowCount > 0)
					{
						num3 = Function.ParseDouble(pivotDrillDownDataSource7[0][field_Quantity]);
					}
				}
				if (pivotDrillDownDataSource[num] != null)
				{
					if (e.ColumnIndex == i)
					{
						num2 = Function.ParseDouble(newValue);
						pivotDrillDownDataSource[num][field_Quantity] = num2;
					}
					else if (pivotDrillDownDataSource6 != null && pivotDrillDownDataSource6[num] != null)
					{
						num2 = Function.ParseDouble(pivotDrillDownDataSource6[num][field_Quantity]);
					}
					if (pivotDrillDownDataSource2 != null && pivotDrillDownDataSource2.RowCount > num)
					{
						pivotDrillDownDataSource2[num][field_Quantity] = num2;
					}
				}
				double num4 = ((pivotDrillDownDataSource3 != null && pivotDrillDownDataSource3[num] != null) ? Function.ParseDouble(pivotDrillDownDataSource3[num][field_Quantity]) : 0.0);
				double num5 = 0.0;
				if (pivotDrillDownDataSource5 != null && pivotDrillDownDataSource5.RowCount > num)
				{
					num5 = Function.ParseDouble(pivotDrillDownDataSource5[num][field_Quantity]);
				}
				if (pivotDrillDownDataSource4 != null && pivotDrillDownDataSource4.RowCount > num)
				{
					pivotDrillDownDataSource4[num][field_Quantity] = num4 + num5 + num3 - num2;
				}
			}
			pgcDetail.DataSourceTable().AcceptChanges();
			pgcDetail.RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual Enums.MessageCode GetDataDoc(ref DataTable dt)
	{
		if (DocStructure == null || DocStructure.Columns.Count == 0)
		{
			return Enums.MessageCode.CanNotGetHeaderStruc2Save;
		}
		if (string.IsNullOrEmpty(Function.ToString(txtInputCode.EditValue)))
		{
			txtInputCode.Focus();
			return Enums.MessageCode.MrpCodeNotNull;
		}
		if (string.IsNullOrEmpty(Function.ToString(txtFromDate.EditValue)))
		{
			txtFromDate.Focus();
			return Enums.MessageCode.FromDateCanNotBeNull;
		}
		if (string.IsNullOrEmpty(Function.ToString(txtToDate.EditValue)))
		{
			txtToDate.Focus();
			return Enums.MessageCode.ToDateCanNotBeNull;
		}
		if (string.IsNullOrEmpty(Function.ToString(txtFixDate.EditValue)))
		{
			txtToDate.Focus();
			return Enums.MessageCode.FixDateCanNotBeNull;
		}
		dt = DocStructure.Clone();
		DataRow dataRow = dt.NewRow();
		dataRow["PSCode"] = Function.ParseInt(txtCode.EditValue);
		dataRow["PSName"] = txtName.EditValue;
		dataRow["MrpId"] = txtInputCode.EditValue;
		dataRow["Status"] = txtStatus.EditValue;
		dataRow["Type"] = txtType.EditValue;
		dataRow["CreateDate"] = DateTime.Now.Date;
		dataRow["DetailSchedule"] = chkDetailSchedule.EditValue;
		dt.Rows.Add(dataRow);
		return Enums.MessageCode.Success;
	}

	protected void EnableDisableControl(bool isEnable)
	{
		try
		{
			btn_Save.Enabled = isEnable;
			btn_Cancel.Enabled = isEnable;
			txtCode.ReadOnly = !isEnable;
			txtInputCode.ReadOnly = !isEnable;
			txtName.ReadOnly = !isEnable;
			txtStatus.ReadOnly = !isEnable;
			rQuantity.ReadOnly = !isEnable;
			txtCode.Properties.Buttons[0].Visible = isEnable;
			txtCode.Properties.Buttons[0].Visible = isEnable;
			txtType.ReadOnly = !isEnable;
			DocContextMenu.Enabled = isEnable;
			chkDetailSchedule.ReadOnly = !isEnable || pgcDetail.DataSourceTable().IsNotNull();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual async Task Run()
	{
		try
		{
			DataTable dataPivot = JoinDataLine(await HttpClient.ProductionSchedule.Run(Function.ToString(txtInputCode.EditValue), Function.ToString(txtType.EditValue), Function.ParseDateTimes(txtFromDate.EditValue).ToString("yyyyMMdd"), Function.ParseDateTimes(txtToDate.EditValue).ToString("yyyyMMdd"), Function.ToString(chkDetailSchedule.EditValue)));
			SetDataPivot(dataPivot);
			pgcDetail.BestFit();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual void SetDataDoc(DataRow row)
	{
		try
		{
			_isLoad = true;
			if (row != null)
			{
				txtType.EditValue = Function.ToString(row["Type"]);
				txtCode.EditValue = Function.ParseInt(row["PSCode"]);
				txtName.EditValue = Function.ToString(row["PSName"]);
				txtInputCode.EditValue = Function.ToString(row["MrpId"]);
				txtStatus.EditValue = Function.ToString(row["Status"]);
				txtFromDate.EditValue = Function.ParseDateTime(row["BeginDate"]);
				txtToDate.EditValue = Function.ParseDateTime(row["FCstDate"]);
				txtFixDate.EditValue = Function.ParseDateTime(row["FixDate"]);
				chkDetailSchedule.EditValue = Function.ToString(row["DetailSchedule"]);
			}
			else
			{
				if (FormMode != Enums.FormViewModeType.Add)
				{
					txtName.EditValue = null;
					txtCode.EditValue = null;
				}
				txtInputCode.EditValue = null;
				txtFromDate.EditValue = null;
				txtToDate.EditValue = null;
				txtFixDate.EditValue = null;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoad = false;
	}

	protected virtual async Task LoadDataLine()
	{
		try
		{
			DataTable dataPivot = JoinDataLine(await GetDataLine(Function.ParseInt(txtCode.EditValue)));
			SetDataPivot(dataPivot);
			pgcDetail.BestFit();
			if (!_dueDate.HasValue)
			{
				return;
			}
			for (int i = 2; i < pgcDetail.Cells.RowCount; i += 4)
			{
				for (int j = 0; j < pgcDetail.Cells.ColumnCount; j++)
				{
					PivotDrillDownDataSource pivotDrillDownDataSource = pgcDetail.CreateDrillDownDataSource(j, i);
					if (pivotDrillDownDataSource != null && pivotDrillDownDataSource.RowCount > 0)
					{
						string text = Function.ToString(pivotDrillDownDataSource[0]["ItemCode"]);
						string text2 = Function.ToString(pivotDrillDownDataSource[0]["Shift"]);
						DateTime dateTime = Function.ParseDateTimes(pivotDrillDownDataSource[0]["WorkDate"]);
						if (text.Equals(_itemCode) && text2.Equals(_shift) && Function.ParseDateTimes(_dueDate) == dateTime)
						{
							pgcDetail.Cells.FocusedCell = new Point(j, i);
							return;
						}
					}
				}
			}
			_dueDate = null;
			_itemCode = (_shift = "");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual async Task<DataSet> GetDataLine(int id)
	{
		try
		{
			return await HttpClient.ScheduleClient.GetDataLine(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	protected virtual DataTable JoinDataLine(DataSet line)
	{
		try
		{
			if (line == null || line.Tables.Count < 2 || line.Tables[1] == null || line.Tables[1].Rows.Count == 0)
			{
				WinCommons.Warning(Enums.MessageCode.DataLinesIsEmpty, "");
				return null;
			}
			if (line.Tables[0] == null || line.Tables[0].Rows.Count == 0)
			{
				WinCommons.Warning(Enums.MessageCode.YouMustMakeWorkScheduleBeforeMakingSchedule, "");
				return null;
			}
			DataTable dtresult = new DataTable();
			dtresult.Columns.Add("ItemCode", typeof(string));
			dtresult.Columns.Add("ItemName", typeof(string));
			dtresult.Columns.Add("Planning", typeof(string));
			dtresult.Columns.Add("Shift", typeof(string));
			dtresult.Columns.Add("WorkDate", typeof(DateTime));
			dtresult.Columns.Add("Qty", typeof(double));
			dtresult.Columns.Add("DocEntry", typeof(int));
			dtresult.Columns.Add("BomVersion", typeof(string));
			dtresult.Columns.Add("CardCode", typeof(string));
			dtresult.Columns.Add("LTime", typeof(int));
			dtresult.Columns.Add("ItemType", typeof(string));
			dtresult.Columns.Add("ItemGroup", typeof(int));
			dtresult.Columns.Add("DemandType", typeof(string));
			dtresult.Columns.Add("DemandEntry", typeof(int));
			dtresult.Columns.Add("DemandLine", typeof(int));
			dtresult.Columns.Add("FatherCode", typeof(string));
			dtresult.Columns.Add("BaseType", typeof(string));
			dtresult.Columns.Add("BaseEntry", typeof(int));
			dtresult.Columns.Add("BaseLine", typeof(int));
			return (from tem in (from distinct in line.Tables[1].DefaultView.ToTable(true, "ItemCode", "ItemName", "BomVersion", "CardCode", "LTime", "ItemType", "ItemGroup", "DemandType", "DemandEntry", "DemandLine", "FatherCode", "BaseType", "BaseEntry", "BaseLine").AsEnumerable()
					join date in line.Tables[0].AsEnumerable() on 1 equals 1 into temps
					from temp in temps.DefaultIfEmpty()
					select dtresult.LoadDataRow(new object[19]
					{
						distinct["ItemCode"],
						distinct["ItemName"],
						temp["Planning"],
						temp["Shift"],
						temp["WorkDate"],
						0,
						0,
						distinct["BomVersion"],
						distinct["CardCode"],
						distinct["LTime"],
						distinct["ItemType"],
						distinct["ItemGroup"],
						distinct["DemandType"],
						distinct["DemandEntry"],
						distinct["DemandLine"],
						distinct["FatherCode"],
						distinct["BaseType"],
						distinct["BaseEntry"],
						distinct["BaseLine"]
					}, fAcceptChanges: true)).CopyToDataTable().AsEnumerable()
				join row in line.Tables[1].AsEnumerable() on new
				{
					ItemCode = Function.ToString(tem["ItemCode"]),
					TypePlanning = Function.ToString(tem["Planning"]),
					Shift = Function.ToString(tem["Shift"]),
					WorkDate = Function.ParseDateTimes(tem["WorkDate"]).Date,
					BomVersion = Function.ToString(tem["BomVersion"]),
					CardCode = Function.ToString(tem["CardCode"]),
					DemandType = Function.ToString(tem["DemandType"]),
					DemandEntry = Function.ToString(tem["DemandEntry"]),
					DemandLine = Function.ToString(tem["DemandLine"]),
					FatherCode = Function.ToString(tem["FatherCode"]),
					BaseType = Function.ToString(tem["BaseType"]),
					BaseEntry = Function.ToString(tem["BaseEntry"]),
					BaseLine = Function.ToString(tem["BaseLine"])
				} equals new
				{
					ItemCode = Function.ToString(row["ItemCode"]),
					TypePlanning = Function.ToString(row["Planning"]),
					Shift = Function.ToString(row["Shift"]),
					WorkDate = Function.ParseDateTimes(row["WorkDate"]).Date,
					BomVersion = Function.ToString(row["BomVersion"]),
					CardCode = Function.ToString(row["CardCode"]),
					DemandType = Function.ToString(row["DemandType"]),
					DemandEntry = Function.ToString(row["DemandEntry"]),
					DemandLine = Function.ToString(row["DemandLine"]),
					FatherCode = Function.ToString(row["FatherCode"]),
					BaseType = Function.ToString(row["BaseType"]),
					BaseEntry = Function.ToString(row["BaseEntry"]),
					BaseLine = Function.ToString(row["BaseLine"])
				} into join2
				from rowjoin in join2.DefaultIfEmpty()
				select dtresult.LoadDataRow(new object[19]
				{
					tem["ItemCode"],
					tem["ItemName"],
					tem["Planning"],
					tem["Shift"],
					tem["WorkDate"],
					(rowjoin == null) ? 0.0 : Function.ParseDouble(rowjoin["Qty"]),
					(rowjoin != null) ? Function.ParseInt(rowjoin["DocEntry"]) : 0,
					tem["BomVersion"],
					tem["CardCode"],
					tem["LTime"],
					tem["ItemType"],
					tem["ItemGroup"],
					tem["DemandType"],
					tem["DemandEntry"],
					tem["DemandLine"],
					tem["FatherCode"],
					tem["BaseType"],
					tem["BaseEntry"],
					tem["BaseLine"]
				}, fAcceptChanges: true)).CopyToDataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return null;
		}
	}

	protected virtual async Task Save()
	{
		Enums.FormViewModeType oldView = FormMode;
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			WinCommons._OpenProcessing();
			DataTable dt = new DataTable();
			Enums.MessageCode dataDoc = GetDataDoc(ref dt);
			if (dataDoc != Enums.MessageCode.Success)
			{
				WinCommons.Error(dataDoc, "");
				return;
			}
			DataTable dataTable = pgcDetail.DataSourceTable().Copy();
			if (!dt.IsNotNull() || !dataTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.SaveDataCanNotBeNull, "");
			}
			dt.TableName = "Table doc";
			dataTable.TableName = "Table Line";
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dt);
			dataSet.Tables.Add(dataTable);
			HttpResult httpResult = ((oldView != Enums.FormViewModeType.Add) ? (await HttpClient.ScheduleClient.Update(dataSet)) : (await HttpClient.ScheduleClient.Save(dataSet)));
			HttpResult httpResult2 = httpResult;
			if (string.IsNullOrEmpty(httpResult2.Message))
			{
				txtCode.EditValue = Function.ParseInt(httpResult2.Content);
				WinCommons.Success((oldView == Enums.FormViewModeType.Add) ? Enums.MessageCode.SaveSuccess : Enums.MessageCode.UpdateSuccess, "");
				SetDataStatus(Function.ToString(txtStatus.EditValue));
				oldView = Enums.FormViewModeType.View;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult2.Message);
			}
		}
		FormMode = oldView;
	}

	protected void SetDataStatus(string currentStatus)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Columns.Add("Value", typeof(string));
			switch (currentStatus)
			{
			case "D":
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Value"] = "D";
				dataRow["Name"] = "Draft";
				dataTable.Rows.Add(dataRow);
				dataRow = dataTable.NewRow();
				dataRow["Value"] = "A";
				dataRow["Name"] = "Aprroved";
				dataTable.Rows.Add(dataRow);
				dataRow = dataTable.NewRow();
				dataRow["Value"] = "R";
				dataRow["Name"] = "Reject";
				dataTable.Rows.Add(dataRow);
				break;
			}
			case "A":
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Value"] = "A";
				dataRow["Name"] = "Aprroved";
				dataTable.Rows.Add(dataRow);
				dataRow = dataTable.NewRow();
				dataRow["Value"] = "R";
				dataRow["Name"] = "Reject";
				dataTable.Rows.Add(dataRow);
				break;
			}
			case "R":
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Value"] = "R";
				dataRow["Name"] = "Reject";
				dataTable.Rows.Add(dataRow);
				break;
			}
			}
			txtStatus.Properties.DataSource = dataTable;
			txtStatus.EditValue = currentStatus;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual async Task GetDataDoc(Enums.NavigateType type)
	{
		Enums.FormViewModeType oldView = FormMode;
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			DataTable dataTable = await HttpClient.ScheduleClient.GetDataDoc(Function.ParseInt(txtCode.EditValue), type);
			if (dataTable.IsNotNull())
			{
				SetDataDoc(dataTable.Rows[0]);
				SetDataStatus(Function.ToString(txtStatus.EditValue));
				await LoadDataLine();
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

	protected virtual async Task GetDataInput()
	{
		Enums.FormViewModeType oldView = FormMode;
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			DataTable dataTable = await HttpClient.ScheduleClient.GetDataMrp(Function.ParseInt(txtInputCode.EditValue));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (!Function.ToString(dataTable.Rows[0]["Status"]).Equals("A"))
				{
					WinCommons.Error(Enums.MessageCode.YouMustChooseApprovalMrp, "");
					FormMode = oldView;
					WinCommons._CloseProcessing();
					return;
				}
				if (Function.ToString(txtType.EditValue).Equals("P"))
				{
					if (Function.ToString(dataTable.Rows[0]["Type"]).Equals("2"))
					{
						WinCommons.Error(Enums.MessageCode.YouMustChooseMrp134, "");
						FormMode = oldView;
						WinCommons._CloseProcessing();
						return;
					}
				}
				else if (Function.ToString(txtType.EditValue).Equals("T"))
				{
					if (!Function.ToString(dataTable.Rows[0]["Type"]).Equals("3"))
					{
						WinCommons.Error(Enums.MessageCode.YouMustChooseMrp3, "");
						FormMode = oldView;
						WinCommons._CloseProcessing();
						return;
					}
				}
				else if (Function.ToString(dataTable.Rows[0]["Type"]).Equals("1"))
				{
					WinCommons.Error(Enums.MessageCode.YouMustChooseMrp234, "");
					FormMode = oldView;
					WinCommons._CloseProcessing();
					return;
				}
				txtFromDate.EditValue = Function.ParseDateTimes(dataTable.Rows[0]["BeginDate"]);
				txtToDate.EditValue = Function.ParseDateTimes(dataTable.Rows[0]["FCstDate"]);
				txtFixDate.EditValue = Function.ParseDateTimes(dataTable.Rows[0]["FixDate"]);
				await Run();
				oldView = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = oldView;
		WinCommons._CloseProcessing();
	}

	private void SetColorControll(Enums.FormViewModeType type)
	{
		try
		{
			txtCode.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtFixDate.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtFromDate.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtInputCode.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtName.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtStatus.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtStatus.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtToDate.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtType.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual int GetNewIdToAddNew()
	{
		try
		{
			HttpResult newIdSchedule = HttpClient.ScheduleClient.GetNewIdSchedule();
			if (string.IsNullOrEmpty(newIdSchedule.Message))
			{
				return Function.ParseInt(newIdSchedule.Content);
			}
			WinCommons.Error(Enums.MessageCode.None, newIdSchedule.Message);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return 0;
	}

	protected virtual void InputControllButtonClick(object sender, ButtonPressedEventArgs e)
	{
		if (e.Button.Index == 0)
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.MRP, false, txtInputCode.Text, "", Enums.ItemObjectType.NoItemType, null, "")
			{
				MrpStatus = "A",
				MrpType = (Function.ToString(txtType.EditValue).Equals("P") ? "1,3,4" : (Function.ToString(txtType.EditValue).Equals("T") ? "3" : "2,3,4"))
			};
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				txtInputCode.EditValue = frm_CFL2.ChooseItem["MrpCode"];
			}
		}
		else
		{
			int num = Function.ParseInt(txtInputCode.Text);
			if (num != 0)
			{
				frm_MRP frm_MRP2 = new frm_MRP(num);
				frm_MRP2.MdiParent = MdiParent;
				frm_MRP2.Show();
			}
		}
	}

	protected virtual void LoadDataAutoFillClick()
	{
		WinCommons._OpenProcessing();
		int columnCount = pgcDetail.Cells.ColumnCount;
		int rowCount = pgcDetail.Cells.RowCount;
		for (int i = 0; i < rowCount; i++)
		{
			if (!Function.ToString(pgcDetail.GetFieldValue(field_Planning, i)).StartsWith("1."))
			{
				continue;
			}
			for (int j = 1; j < columnCount - 1; j++)
			{
				PivotDrillDownDataSource pivotDrillDownDataSource = pgcDetail.CreateDrillDownDataSource(j, i + 2);
				PivotDrillDownDataSource pivotDrillDownDataSource2 = pgcDetail.CreateDrillDownDataSource(j, i);
				PivotDrillDownDataSource pivotDrillDownDataSource3 = pgcDetail.CreateDrillDownDataSource(j, i + 3);
				PivotDrillDownDataSource pivotDrillDownDataSource4 = pgcDetail.CreateDrillDownDataSource(j - 1, i + 3);
				PivotDrillDownDataSource pivotDrillDownDataSource5 = pgcDetail.CreateDrillDownDataSource(j, i + 1);
				if (pivotDrillDownDataSource5 == null || pivotDrillDownDataSource5.RowCount <= 0)
				{
					continue;
				}
				for (int k = 0; k < pivotDrillDownDataSource5.RowCount; k++)
				{
					int demandEntry = Function.ParseInt(pivotDrillDownDataSource5[k]["DemandEntry"]);
					int demandLine = Function.ParseInt(pivotDrillDownDataSource5[k]["DemandLine"]);
					string demandType = Function.ToString(pivotDrillDownDataSource5[k]["DemandType"]);
					string baseType = Function.ToString(pivotDrillDownDataSource5[k]["BaseType"]);
					string baseEntry = Function.ToString(pivotDrillDownDataSource5[k]["BaseEntry"]);
					string baseLine = Function.ToString(pivotDrillDownDataSource5[k]["BaseLine"]);
					string fatherCode = Function.ToString(pivotDrillDownDataSource5[k]["FatherCode"]);
					double num = 0.0;
					double num2 = 0.0;
					double num3 = 0.0;
					bool flag = true;
					if (pivotDrillDownDataSource2 != null && pivotDrillDownDataSource2.RowCount > 0)
					{
						PivotDrillDownDataRow pivotDrillDownDataRow = pivotDrillDownDataSource2.Cast<PivotDrillDownDataRow>().FirstOrDefault((PivotDrillDownDataRow t) => Function.ToString(t["DemandType"]).Equals(demandType) && Function.ParseInt(t["DemandEntry"]) == demandEntry && Function.ParseInt(t["DemandLine"]) == demandLine && Function.ToString(t["FatherCode"]).Equals(fatherCode) && Function.ToString(t["BaseType"]).Equals(baseType) && Function.ToString(t["BaseEntry"]).Equals(baseEntry) && Function.ToString(t["BaseLine"]).Equals(baseLine));
						if (pivotDrillDownDataRow != null)
						{
							num3 = Function.ParseDouble(pivotDrillDownDataRow[field_Quantity]);
							if (string.IsNullOrEmpty(Function.ToString(pivotDrillDownDataRow[field_Shift])) || DateTime.Now.Date > Function.ParseDateTimes(pivotDrillDownDataRow[field_WorkDate]) || txtFixDate.DateTime.Date < Function.ParseDateTimes(pivotDrillDownDataRow[field_WorkDate]) || Function.ParseInt(pivotDrillDownDataRow["DocEntry"]) > 0)
							{
								flag = false;
							}
							num = Function.ParseDouble(pivotDrillDownDataRow[field_Quantity]);
						}
					}
					if (j == 1)
					{
						PivotDrillDownDataSource pivotDrillDownDataSource6 = pgcDetail.CreateDrillDownDataSource(0, i);
						if (pivotDrillDownDataSource6 != null && pivotDrillDownDataSource6.RowCount > 0)
						{
							PivotDrillDownDataRow pivotDrillDownDataRow2 = pivotDrillDownDataSource6.Cast<PivotDrillDownDataRow>().FirstOrDefault((PivotDrillDownDataRow t) => Function.ToString(t["DemandType"]).Equals(demandType) && Function.ParseInt(t["DemandEntry"]) == demandEntry && Function.ParseInt(t["DemandLine"]) == demandLine && Function.ToString(t["FatherCode"]).Equals(fatherCode) && Function.ToString(t["BaseType"]).Equals(baseType) && Function.ToString(t["BaseEntry"]).Equals(baseEntry) && Function.ToString(t["BaseLine"]).Equals(baseLine));
							if (pivotDrillDownDataRow2 != null)
							{
								num2 = Function.ParseDouble(pivotDrillDownDataRow2[field_Quantity]);
							}
						}
					}
					if (flag)
					{
						if (Math.Abs(Function.ParseDouble(pivotDrillDownDataSource5[k][field_Quantity])) < 1E-07)
						{
							num += num2;
							if (Math.Abs(Function.ParseDouble(pivotDrillDownDataSource5[k][field_Quantity]) - num) > 1E-06)
							{
								pivotDrillDownDataSource5[k][field_Quantity] = num;
							}
							if (pivotDrillDownDataSource != null && pivotDrillDownDataSource.RowCount > 0)
							{
								PivotDrillDownDataRow pivotDrillDownDataRow3 = pivotDrillDownDataSource.Cast<PivotDrillDownDataRow>().FirstOrDefault((PivotDrillDownDataRow t) => Function.ToString(t["DemandType"]).Equals(demandType) && Function.ParseInt(t["DemandEntry"]) == demandEntry && Function.ParseInt(t["DemandLine"]) == demandLine && Function.ToString(t["FatherCode"]).Equals(fatherCode) && Function.ToString(t["BaseType"]).Equals(baseType) && Function.ToString(t["BaseEntry"]).Equals(baseEntry) && Function.ToString(t["BaseLine"]).Equals(baseLine));
								if (pivotDrillDownDataRow3 != null && Math.Abs(Function.ParseDouble(pivotDrillDownDataRow3[field_Quantity]) - num) > 1E-06)
								{
									pivotDrillDownDataRow3[field_Quantity] = num;
								}
							}
						}
						else
						{
							num = Function.ParseDouble(pivotDrillDownDataSource5[k][field_Quantity]);
						}
					}
					else
					{
						num = 0.0;
					}
					double num4 = 0.0;
					if (pivotDrillDownDataSource4 != null && pivotDrillDownDataSource4.RowCount > 0)
					{
						PivotDrillDownDataRow pivotDrillDownDataRow4 = pivotDrillDownDataSource4.Cast<PivotDrillDownDataRow>().FirstOrDefault((PivotDrillDownDataRow t) => Function.ToString(t["DemandType"]).Equals(demandType) && Function.ParseInt(t["DemandEntry"]) == demandEntry && Function.ParseInt(t["DemandLine"]) == demandLine && Function.ToString(t["FatherCode"]).Equals(fatherCode) && Function.ToString(t["BaseType"]).Equals(baseType) && Function.ToString(t["BaseEntry"]).Equals(baseEntry) && Function.ToString(t["BaseLine"]).Equals(baseLine));
						if (pivotDrillDownDataRow4 != null)
						{
							num4 = Function.ParseDouble(pivotDrillDownDataRow4[field_Quantity]);
						}
					}
					if (pivotDrillDownDataSource3 == null || pivotDrillDownDataSource3[0] == null)
					{
						continue;
					}
					PivotDrillDownDataRow pivotDrillDownDataRow5 = pivotDrillDownDataSource3.Cast<PivotDrillDownDataRow>().FirstOrDefault((PivotDrillDownDataRow t) => Function.ToString(t["DemandType"]).Equals(demandType) && Function.ParseInt(t["DemandEntry"]) == demandEntry && Function.ParseInt(t["DemandLine"]) == demandLine && Function.ToString(t["FatherCode"]).Equals(fatherCode) && Function.ToString(t["BaseType"]).Equals(baseType) && Function.ToString(t["BaseEntry"]).Equals(baseEntry) && Function.ToString(t["BaseLine"]).Equals(baseLine));
					if (pivotDrillDownDataRow5 != null)
					{
						double num5 = num3 + num4 + num2 - num;
						if (Math.Abs(Function.ParseDouble(pivotDrillDownDataRow5[field_Quantity]) - num5) > 1E-06)
						{
							pivotDrillDownDataRow5[field_Quantity] = num5;
						}
					}
				}
			}
			i += 4;
		}
		pgcDetail.DataSourceTable().AcceptChanges();
		pgcDetail.RefreshData();
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
		WinCommons._CloseProcessing();
	}

	protected virtual void LoadDataAutoFillColumn()
	{
		int columnCount = pgcDetail.Cells.ColumnCount;
		int rowCount = pgcDetail.Cells.RowCount;
		for (int i = 0; i < rowCount; i++)
		{
			if (!Function.ToString(pgcDetail.GetFieldValue(field_Planning, i)).StartsWith("1."))
			{
				continue;
			}
			for (int j = 1; j < columnCount - 1; j++)
			{
				PivotDrillDownDataSource pivotDrillDownDataSource = pgcDetail.CreateDrillDownDataSource(j, i + 2);
				PivotDrillDownDataSource pivotDrillDownDataSource2 = pgcDetail.CreateDrillDownDataSource(j, i);
				PivotDrillDownDataSource pivotDrillDownDataSource3 = pgcDetail.CreateDrillDownDataSource(j, i + 3);
				PivotDrillDownDataSource pivotDrillDownDataSource4 = pgcDetail.CreateDrillDownDataSource(j - 1, i + 3);
				PivotDrillDownDataSource pivotDrillDownDataSource5 = pgcDetail.CreateDrillDownDataSource(j, i + 1);
				if (pivotDrillDownDataSource5 == null || pivotDrillDownDataSource5.RowCount <= 0)
				{
					continue;
				}
				for (int k = 0; k < pivotDrillDownDataSource5.RowCount; k++)
				{
					int demandEntry = Function.ParseInt(pivotDrillDownDataSource5[k]["DemandEntry"]);
					int demandLine = Function.ParseInt(pivotDrillDownDataSource5[k]["DemandLine"]);
					string demandType = Function.ToString(pivotDrillDownDataSource5[k]["DemandType"]);
					string baseType = Function.ToString(pivotDrillDownDataSource5[k]["BaseType"]);
					string baseEntry = Function.ToString(pivotDrillDownDataSource5[k]["BaseEntry"]);
					string baseLine = Function.ToString(pivotDrillDownDataSource5[k]["BaseLine"]);
					string fatherCode = Function.ToString(pivotDrillDownDataSource5[k]["FatherCode"]);
					double num = 0.0;
					double num2 = 0.0;
					bool flag = true;
					double num3 = 0.0;
					if (pivotDrillDownDataSource2 != null && pivotDrillDownDataSource2.RowCount > 0)
					{
						PivotDrillDownDataRow pivotDrillDownDataRow = pivotDrillDownDataSource2.Cast<PivotDrillDownDataRow>().FirstOrDefault((PivotDrillDownDataRow t) => Function.ToString(t["DemandType"]).Equals(demandType) && Function.ParseInt(t["DemandEntry"]) == demandEntry && Function.ParseInt(t["DemandLine"]) == demandLine && Function.ToString(t["FatherCode"]).Equals(fatherCode) && Function.ToString(t["BaseType"]).Equals(baseType) && Function.ToString(t["BaseEntry"]).Equals(baseEntry) && Function.ToString(t["BaseLine"]).Equals(baseLine));
						if (pivotDrillDownDataRow != null)
						{
							num3 = Function.ParseDouble(pivotDrillDownDataRow[field_Quantity]);
							if (string.IsNullOrEmpty(Function.ToString(pivotDrillDownDataRow[field_Shift])) || DateTime.Now.Date > Function.ParseDateTimes(pivotDrillDownDataRow[field_WorkDate]) || txtFixDate.DateTime.Date < Function.ParseDateTimes(pivotDrillDownDataRow[field_WorkDate]) || Function.ParseInt(pivotDrillDownDataRow["DocEntry"]) > 0)
							{
								flag = false;
							}
							num = Function.ParseDouble(pivotDrillDownDataRow[field_Quantity]);
						}
					}
					if (j == 1)
					{
						PivotDrillDownDataSource pivotDrillDownDataSource6 = pgcDetail.CreateDrillDownDataSource(0, i);
						if (pivotDrillDownDataSource6 != null && pivotDrillDownDataSource6.RowCount > 0)
						{
							PivotDrillDownDataRow pivotDrillDownDataRow2 = pivotDrillDownDataSource6.Cast<PivotDrillDownDataRow>().FirstOrDefault((PivotDrillDownDataRow t) => Function.ToString(t["DemandType"]).Equals(demandType) && Function.ParseInt(t["DemandEntry"]) == demandEntry && Function.ParseInt(t["DemandLine"]) == demandLine && Function.ToString(t["FatherCode"]).Equals(fatherCode) && Function.ToString(t["BaseType"]).Equals(baseType) && Function.ToString(t["BaseEntry"]).Equals(baseEntry) && Function.ToString(t["BaseLine"]).Equals(baseLine));
							if (pivotDrillDownDataRow2 != null)
							{
								num2 = Function.ParseDouble(pivotDrillDownDataRow2[field_Quantity]);
							}
						}
					}
					if (Math.Abs(Function.ParseDouble(pivotDrillDownDataSource5[k][field_Quantity])) < 1E-07)
					{
						num += num2;
						if (flag && j == ColumnIndexAutoFill)
						{
							pivotDrillDownDataSource5[k][field_Quantity] = num;
							if (pivotDrillDownDataSource != null && pivotDrillDownDataSource.RowCount > 0)
							{
								PivotDrillDownDataRow pivotDrillDownDataRow3 = pivotDrillDownDataSource.Cast<PivotDrillDownDataRow>().FirstOrDefault((PivotDrillDownDataRow t) => Function.ToString(t["DemandType"]).Equals(demandType) && Function.ParseInt(t["DemandEntry"]) == demandEntry && Function.ParseInt(t["DemandLine"]) == demandLine && Function.ToString(t["FatherCode"]).Equals(fatherCode) && Function.ToString(t["BaseType"]).Equals(baseType) && Function.ToString(t["BaseEntry"]).Equals(baseEntry) && Function.ToString(t["BaseLine"]).Equals(baseLine));
								if (pivotDrillDownDataRow3 != null)
								{
									pivotDrillDownDataRow3[field_Quantity] = num;
								}
							}
						}
						else
						{
							num = 0.0;
						}
					}
					else
					{
						num = Function.ParseDouble(pivotDrillDownDataSource5[k][field_Quantity]);
					}
					double num4 = 0.0;
					if (pivotDrillDownDataSource4 != null && pivotDrillDownDataSource4.RowCount > 0)
					{
						PivotDrillDownDataRow pivotDrillDownDataRow4 = pivotDrillDownDataSource4.Cast<PivotDrillDownDataRow>().FirstOrDefault((PivotDrillDownDataRow t) => Function.ToString(t["DemandType"]).Equals(demandType) && Function.ParseInt(t["DemandEntry"]) == demandEntry && Function.ParseInt(t["DemandLine"]) == demandLine && Function.ToString(t["FatherCode"]).Equals(fatherCode) && Function.ToString(t["BaseType"]).Equals(baseType) && Function.ToString(t["BaseEntry"]).Equals(baseEntry) && Function.ToString(t["BaseLine"]).Equals(baseLine));
						if (pivotDrillDownDataRow4 != null)
						{
							num4 = Function.ParseDouble(pivotDrillDownDataRow4[field_Quantity]);
						}
					}
					if (pivotDrillDownDataSource3 != null && pivotDrillDownDataSource3[0] != null)
					{
						PivotDrillDownDataRow pivotDrillDownDataRow5 = pivotDrillDownDataSource3.Cast<PivotDrillDownDataRow>().FirstOrDefault((PivotDrillDownDataRow t) => Function.ToString(t["DemandType"]).Equals(demandType) && Function.ParseInt(t["DemandEntry"]) == demandEntry && Function.ParseInt(t["DemandLine"]) == demandLine && Function.ToString(t["FatherCode"]).Equals(fatherCode) && Function.ToString(t["BaseType"]).Equals(baseType) && Function.ToString(t["BaseEntry"]).Equals(baseEntry) && Function.ToString(t["BaseLine"]).Equals(baseLine));
						if (pivotDrillDownDataRow5 != null)
						{
							pivotDrillDownDataRow5[field_Quantity] = num3 + num4 + num2 - num;
						}
					}
				}
			}
			i += 4;
		}
		pgcDetail.DataSourceTable().AcceptChanges();
		pgcDetail.RefreshData();
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	protected virtual void ShowingEditorExtentions(CancelPivotCellEditEventArgs e)
	{
	}

	protected void SetDataPivot(DataTable dt)
	{
		try
		{
			pgcDetail.DataSource = dt;
			pgcDetail.RefreshData();
			if (dt.IsNotNull())
			{
				dt.AcceptChanges();
				chkDetailSchedule.ReadOnly = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
		this.components = new System.ComponentModel.Container();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Interfaces.Mrp.VirSchedule));
		DevExpress.XtraPivotGrid.PivotGridFormatRule pivotGridFormatRule = new DevExpress.XtraPivotGrid.PivotGridFormatRule();
		DevExpress.XtraEditors.FormatConditionRuleDateOccuring formatConditionRuleDateOccuring = new DevExpress.XtraEditors.FormatConditionRuleDateOccuring();
		DevExpress.XtraPivotGrid.FormatRuleTotalTypeSettings settings = new DevExpress.XtraPivotGrid.FormatRuleTotalTypeSettings();
		this.txtInputCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.txtStatus = new DevExpress.XtraEditors.LookUpEdit();
		this.txtFromDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtToDate = new Apzon.Controls.Editor.DateEditEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbtofstDate = new Apzon.Controls.Controls.LabelLine();
		this.lbfromdate = new Apzon.Controls.Controls.LabelLine();
		this.lbStatus = new Apzon.Controls.Controls.LabelLine();
		this.lbInputCode = new Apzon.Controls.Controls.LabelLine();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbfixdate = new Apzon.Controls.Controls.LabelLine();
		this.txtFixDate = new Apzon.Controls.Editor.DateEditEx();
		this.lbTYpe = new Apzon.Controls.Controls.LabelLine();
		this.txtType = new DevExpress.XtraEditors.LookUpEdit();
		this.txtName = new Apzon.Controls.Editor.TextEditEx();
		this.lbName = new Apzon.Controls.Controls.LabelLine();
		this.lbCode = new Apzon.Controls.Controls.LabelLine();
		this.txtCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.DocContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.autoFillColumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItemFilter1 = new Apzon.Forms.UserControls.ToolStripMenuItemFilter(this.components);
		this.pgcDetail = new Apzon.Controls.Pivot.PivotGridControlEx();
		this.field_ItemCode = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_ItemName = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_BP = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_Version = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_Planning = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_WorkDate = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_Shift = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_Quantity = new DevExpress.XtraPivotGrid.PivotGridField();
		this.rQuantity = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.txtFilterField = new Apzon.Controls.Editor.LookUpEditEx();
		this.txtFilterValue = new Apzon.Forms.UserControls.FilterControl.FilterValueControlEx();
		this.rPlaning = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.capacityGanttChartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.btnAutoFillData = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbFilterBy = new Apzon.Controls.Controls.LabelLine();
		this.chkDetailSchedule = new Apzon.Controls.Editor.CheckEditEx();
		((System.ComponentModel.ISupportInitialize)this.txtInputCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFixDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFixDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).BeginInit();
		this.DocContextMenu.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pgcDetail).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rQuantity).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFilterField.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFilterValue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rPlaning).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkDetailSchedule.Properties).BeginInit();
		base.SuspendLayout();
		this.txtInputCode.EditControl = null;
		this.txtInputCode.FieldName = null;
		this.txtInputCode.IsSearchControl = false;
		this.txtInputCode.Location = new System.Drawing.Point(486, 8);
		this.txtInputCode.Name = "txtInputCode";
		this.txtInputCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txtInputCode.Properties.AppearanceFocused.Options.UseBackColor = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.txtInputCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtInputCode.SettingInVisible = false;
		this.txtInputCode.SettingReadonly = false;
		this.txtInputCode.Size = new System.Drawing.Size(120, 20);
		this.txtInputCode.TabIndex = 5;
		this.txtInputCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtInputCode_ButtonClick);
		this.txtInputCode.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.txtInputCode.Validating += new System.ComponentModel.CancelEventHandler(this.txtInputCode_Validating);
		this.txtStatus.Location = new System.Drawing.Point(282, 32);
		this.txtStatus.Name = "txtStatus";
		this.txtStatus.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txtStatus.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtStatus.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name3"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name4", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.txtStatus.Properties.DisplayMember = "Name";
		this.txtStatus.Properties.NullText = "";
		this.txtStatus.Properties.ShowFooter = false;
		this.txtStatus.Properties.ShowHeader = false;
		this.txtStatus.Properties.ValueMember = "Value";
		this.txtStatus.Size = new System.Drawing.Size(120, 20);
		this.txtStatus.TabIndex = 4;
		this.txtStatus.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txtStatus_EditValueChanging);
		this.txtFromDate.EditControl = null;
		this.txtFromDate.EditValue = null;
		this.txtFromDate.FieldName = null;
		this.txtFromDate.Location = new System.Drawing.Point(486, 32);
		this.txtFromDate.Name = "txtFromDate";
		this.txtFromDate.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txtFromDate.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtFromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.IsSearchControl = false;
		this.txtFromDate.Properties.ReadOnly = true;
		this.txtFromDate.SettingInVisible = false;
		this.txtFromDate.SettingReadonly = false;
		this.txtFromDate.Size = new System.Drawing.Size(120, 20);
		this.txtFromDate.TabIndex = 6;
		this.txtFromDate.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txtFromDate_EditValueChanging);
		this.txtToDate.EditControl = null;
		this.txtToDate.EditValue = null;
		this.txtToDate.FieldName = null;
		this.txtToDate.Location = new System.Drawing.Point(687, 32);
		this.txtToDate.Name = "txtToDate";
		this.txtToDate.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txtToDate.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtToDate.Properties.IsSearchControl = false;
		this.txtToDate.Properties.ReadOnly = true;
		this.txtToDate.SettingInVisible = false;
		this.txtToDate.SettingReadonly = false;
		this.txtToDate.Size = new System.Drawing.Size(120, 20);
		this.txtToDate.TabIndex = 8;
		this.txtToDate.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txtToDate_EditValueChanging);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(2, 576);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(70, 20);
		this.btn_Save.TabIndex = 9;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_OK_Click);
		this.lbtofstDate.EditControl = null;
		this.lbtofstDate.EditValue = "ToFstDate";
		this.lbtofstDate.FieldName = null;
		this.lbtofstDate.HideWhenEmpty = false;
		this.lbtofstDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbtofstDate.Location = new System.Drawing.Point(617, 34);
		this.lbtofstDate.Name = "lbtofstDate";
		this.lbtofstDate.ReadOnly = false;
		this.lbtofstDate.SettingInVisible = false;
		this.lbtofstDate.SettingReadonly = false;
		this.lbtofstDate.ShowUnderLine = true;
		this.lbtofstDate.Size = new System.Drawing.Size(70, 20);
		this.lbtofstDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbtofstDate.TabIndex = 0;
		this.lbtofstDate.Utext = "ToFstDate";
		this.lbfromdate.EditControl = null;
		this.lbfromdate.EditValue = "From Date";
		this.lbfromdate.FieldName = null;
		this.lbfromdate.HideWhenEmpty = false;
		this.lbfromdate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbfromdate.Location = new System.Drawing.Point(416, 34);
		this.lbfromdate.Name = "lbfromdate";
		this.lbfromdate.ReadOnly = false;
		this.lbfromdate.SettingInVisible = false;
		this.lbfromdate.SettingReadonly = false;
		this.lbfromdate.ShowUnderLine = true;
		this.lbfromdate.Size = new System.Drawing.Size(70, 20);
		this.lbfromdate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbfromdate.TabIndex = 0;
		this.lbfromdate.Utext = "From Date";
		this.lbStatus.EditControl = null;
		this.lbStatus.EditValue = "Status";
		this.lbStatus.FieldName = null;
		this.lbStatus.HideWhenEmpty = false;
		this.lbStatus.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbStatus.Location = new System.Drawing.Point(212, 34);
		this.lbStatus.Name = "lbStatus";
		this.lbStatus.ReadOnly = false;
		this.lbStatus.SettingInVisible = false;
		this.lbStatus.SettingReadonly = false;
		this.lbStatus.ShowUnderLine = true;
		this.lbStatus.Size = new System.Drawing.Size(70, 20);
		this.lbStatus.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbStatus.TabIndex = 0;
		this.lbStatus.Utext = "Status";
		this.lbInputCode.EditControl = null;
		this.lbInputCode.EditValue = "MRP";
		this.lbInputCode.FieldName = null;
		this.lbInputCode.HideWhenEmpty = false;
		this.lbInputCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbInputCode.Location = new System.Drawing.Point(416, 10);
		this.lbInputCode.Name = "lbInputCode";
		this.lbInputCode.ReadOnly = false;
		this.lbInputCode.SettingInVisible = false;
		this.lbInputCode.SettingReadonly = false;
		this.lbInputCode.ShowUnderLine = true;
		this.lbInputCode.Size = new System.Drawing.Size(70, 20);
		this.lbInputCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbInputCode.TabIndex = 0;
		this.lbInputCode.Utext = "MRP";
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(78, 576);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(70, 20);
		this.btn_Cancel.TabIndex = 10;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.Cancel_Click);
		this.lbfixdate.EditControl = null;
		this.lbfixdate.EditValue = "FixDate";
		this.lbfixdate.FieldName = null;
		this.lbfixdate.HideWhenEmpty = false;
		this.lbfixdate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbfixdate.Location = new System.Drawing.Point(617, 10);
		this.lbfixdate.Name = "lbfixdate";
		this.lbfixdate.ReadOnly = false;
		this.lbfixdate.SettingInVisible = false;
		this.lbfixdate.SettingReadonly = false;
		this.lbfixdate.ShowUnderLine = true;
		this.lbfixdate.Size = new System.Drawing.Size(70, 20);
		this.lbfixdate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbfixdate.TabIndex = 0;
		this.lbfixdate.Utext = "FixDate";
		this.txtFixDate.EditControl = null;
		this.txtFixDate.EditValue = null;
		this.txtFixDate.FieldName = null;
		this.txtFixDate.Location = new System.Drawing.Point(687, 8);
		this.txtFixDate.Name = "txtFixDate";
		this.txtFixDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFixDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFixDate.Properties.IsSearchControl = false;
		this.txtFixDate.Properties.ReadOnly = true;
		this.txtFixDate.SettingInVisible = false;
		this.txtFixDate.SettingReadonly = false;
		this.txtFixDate.Size = new System.Drawing.Size(120, 20);
		this.txtFixDate.TabIndex = 7;
		this.lbTYpe.EditControl = null;
		this.lbTYpe.EditValue = "Type";
		this.lbTYpe.FieldName = null;
		this.lbTYpe.HideWhenEmpty = false;
		this.lbTYpe.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbTYpe.Location = new System.Drawing.Point(212, 10);
		this.lbTYpe.Name = "lbTYpe";
		this.lbTYpe.ReadOnly = false;
		this.lbTYpe.SettingInVisible = false;
		this.lbTYpe.SettingReadonly = false;
		this.lbTYpe.ShowUnderLine = true;
		this.lbTYpe.Size = new System.Drawing.Size(70, 20);
		this.lbTYpe.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbTYpe.TabIndex = 0;
		this.lbTYpe.Utext = "Type";
		this.txtType.Location = new System.Drawing.Point(282, 8);
		this.txtType.Name = "txtType";
		this.txtType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.txtType.Properties.DisplayMember = "Name";
		this.txtType.Properties.NullText = "";
		this.txtType.Properties.ShowFooter = false;
		this.txtType.Properties.ShowHeader = false;
		this.txtType.Properties.ValueMember = "Code";
		this.txtType.Size = new System.Drawing.Size(120, 20);
		this.txtType.TabIndex = 3;
		this.txtType.EditValueChanged += new System.EventHandler(this.txtType_EditValueChanged);
		this.txtType.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txtType_EditValueChanging);
		this.txtName.EditControl = null;
		this.txtName.FieldName = null;
		this.txtName.IsSearchControl = false;
		this.txtName.Location = new System.Drawing.Point(79, 32);
		this.txtName.Name = "txtName";
		this.txtName.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txtName.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtName.SettingInVisible = false;
		this.txtName.SettingReadonly = false;
		this.txtName.Size = new System.Drawing.Size(120, 20);
		this.txtName.TabIndex = 2;
		this.txtName.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txtName_EditValueChanging);
		this.lbName.EditControl = null;
		this.lbName.EditValue = "Name";
		this.lbName.FieldName = null;
		this.lbName.HideWhenEmpty = false;
		this.lbName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbName.Location = new System.Drawing.Point(9, 34);
		this.lbName.Name = "lbName";
		this.lbName.ReadOnly = false;
		this.lbName.SettingInVisible = false;
		this.lbName.SettingReadonly = false;
		this.lbName.ShowUnderLine = true;
		this.lbName.Size = new System.Drawing.Size(70, 20);
		this.lbName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbName.TabIndex = 16;
		this.lbName.Utext = "Name";
		this.lbCode.EditControl = null;
		this.lbCode.EditValue = "Code";
		this.lbCode.FieldName = null;
		this.lbCode.HideWhenEmpty = false;
		this.lbCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbCode.Location = new System.Drawing.Point(9, 10);
		this.lbCode.Name = "lbCode";
		this.lbCode.ReadOnly = false;
		this.lbCode.SettingInVisible = false;
		this.lbCode.SettingReadonly = false;
		this.lbCode.ShowUnderLine = true;
		this.lbCode.Size = new System.Drawing.Size(70, 20);
		this.lbCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbCode.TabIndex = 17;
		this.lbCode.Utext = "Code";
		this.txtCode.EditControl = null;
		this.txtCode.FieldName = null;
		this.txtCode.IsSearchControl = false;
		this.txtCode.Location = new System.Drawing.Point(79, 8);
		this.txtCode.Name = "txtCode";
		this.txtCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txtCode.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txtCode.SettingInVisible = false;
		this.txtCode.SettingReadonly = false;
		this.txtCode.Size = new System.Drawing.Size(120, 20);
		this.txtCode.TabIndex = 1;
		this.txtCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtCode_ButtonClick);
		this.txtCode.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.txtCode.Validating += new System.ComponentModel.CancelEventHandler(this.txtCode_Validating);
		this.DocContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.addNewToolStripMenuItem, this.autoFillColumnToolStripMenuItem, this.toolStripMenuItemFilter1, this.capacityGanttChartToolStripMenuItem });
		this.DocContextMenu.Name = "DocContextMenu";
		this.DocContextMenu.Size = new System.Drawing.Size(185, 92);
		this.DocContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.DocContextMenu_Opening);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.autoFillColumnToolStripMenuItem.Name = "autoFillColumnToolStripMenuItem";
		this.autoFillColumnToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
		this.autoFillColumnToolStripMenuItem.Text = "Auto Fill Column";
		this.autoFillColumnToolStripMenuItem.Click += new System.EventHandler(this.autoFillColumnToolStripMenuItem_Click);
		this.toolStripMenuItemFilter1.DataSource = null;
		this.toolStripMenuItemFilter1.Name = "toolStripMenuItemFilter1";
		this.toolStripMenuItemFilter1.objType = Apzon.Commons.ApzObjectTypes.oSchedule;
		this.toolStripMenuItemFilter1.pivod = this.pgcDetail;
		this.toolStripMenuItemFilter1.Size = new System.Drawing.Size(184, 22);
		this.toolStripMenuItemFilter1.Text = "Filter Setting";
		this.pgcDetail.AllowPasteExcel = false;
		this.pgcDetail.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pgcDetail.ClipboardData = null;
		this.pgcDetail.ContextMenuStrip = this.DocContextMenu;
		this.pgcDetail.Fields.AddRange(new DevExpress.XtraPivotGrid.PivotGridField[8] { this.field_ItemCode, this.field_ItemName, this.field_BP, this.field_Version, this.field_Planning, this.field_WorkDate, this.field_Shift, this.field_Quantity });
		this.pgcDetail.FilterFieldControl = this.txtFilterField;
		this.pgcDetail.FilterValueControl = this.txtFilterValue;
		pivotGridFormatRule.Name = "Format0";
		formatConditionRuleDateOccuring.DateType = DevExpress.XtraEditors.FilterDateType.None;
		pivotGridFormatRule.Rule = formatConditionRuleDateOccuring;
		pivotGridFormatRule.Settings = settings;
		this.pgcDetail.FormatRules.Add(pivotGridFormatRule);
		this.pgcDetail.FormId = "oSchedule";
		this.pgcDetail.ItemId = "APZ_PRS1";
		this.pgcDetail.Location = new System.Drawing.Point(2, 60);
		this.pgcDetail.Name = "pgcDetail";
		this.pgcDetail.ObjectType = Apzon.Commons.ApzObjectTypes.oSchedule;
		this.pgcDetail.OptionsView.ShowColumnTotals = false;
		this.pgcDetail.OptionsView.ShowFilterHeaders = false;
		this.pgcDetail.OptionsView.ShowRowGrandTotals = false;
		this.pgcDetail.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rQuantity, this.rPlaning });
		this.pgcDetail.Size = new System.Drawing.Size(966, 510);
		this.pgcDetail.TabIndex = 0;
		this.pgcDetail.FieldValueDisplayText += new DevExpress.XtraPivotGrid.PivotFieldDisplayTextEventHandler(this.pgcDetail_FieldValueDisplayText);
		this.pgcDetail.CustomCellDisplayText += new DevExpress.XtraPivotGrid.PivotCellDisplayTextEventHandler(this.pivot_Detail_CustomCellDisplayText);
		this.pgcDetail.CustomCellValue += new System.EventHandler<DevExpress.XtraPivotGrid.PivotCellValueEventArgs>(this.pgcDetail_CustomCellValue);
		this.pgcDetail.CellDoubleClick += new DevExpress.XtraPivotGrid.PivotCellEventHandler(this.pgcDetail_CellDoubleClick);
		this.pgcDetail.CustomAppearance += new DevExpress.XtraPivotGrid.PivotCustomAppearanceEventHandler(this.pgcDetail_CustomAppearance);
		this.pgcDetail.CustomDrawFieldValue += new DevExpress.XtraPivotGrid.PivotCustomDrawFieldValueEventHandler(this.pgcDetail_CustomDrawFieldValue);
		this.pgcDetail.CustomRowHeight += new System.EventHandler<DevExpress.XtraPivotGrid.PivotCustomRowHeightEventArgs>(this.pgcDetail_CustomRowHeight);
		this.pgcDetail.EditValueChanged += new DevExpress.XtraPivotGrid.EditValueChangedEventHandler(this.pivot_Detail_EditValueChanged);
		this.pgcDetail.ShowingEditor += new System.EventHandler<DevExpress.XtraPivotGrid.CancelPivotCellEditEventArgs>(this.pivot_Detail_ShowingEditor);
		this.field_ItemCode.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_ItemCode.AreaIndex = 0;
		this.field_ItemCode.Caption = "Item Code";
		this.field_ItemCode.FieldName = "ItemCode";
		this.field_ItemCode.Name = "field_ItemCode";
		this.field_ItemCode.Options.ShowCustomTotals = false;
		this.field_ItemCode.Options.ShowGrandTotal = false;
		this.field_ItemCode.Options.ShowTotals = false;
		this.field_ItemName.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_ItemName.AreaIndex = 1;
		this.field_ItemName.Caption = "Description";
		this.field_ItemName.FieldName = "ItemName";
		this.field_ItemName.Name = "field_ItemName";
		this.field_ItemName.Width = 120;
		this.field_BP.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_BP.AreaIndex = 2;
		this.field_BP.FieldName = "CardCode";
		this.field_BP.Name = "field_BP";
		this.field_Version.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_Version.AreaIndex = 3;
		this.field_Version.Caption = "Version";
		this.field_Version.FieldName = "BomVersion";
		this.field_Version.Name = "field_Version";
		this.field_Planning.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_Planning.AreaIndex = 4;
		this.field_Planning.Caption = "Planning";
		this.field_Planning.FieldName = "Planning";
		this.field_Planning.Name = "field_Planning";
		this.field_Planning.Width = 150;
		this.field_WorkDate.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
		this.field_WorkDate.AreaIndex = 0;
		this.field_WorkDate.Caption = "Work Date";
		this.field_WorkDate.FieldName = "WorkDate";
		this.field_WorkDate.Name = "field_WorkDate";
		this.field_Shift.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
		this.field_Shift.AreaIndex = 1;
		this.field_Shift.Caption = "Shift";
		this.field_Shift.FieldName = "Shift";
		this.field_Shift.Name = "field_Shift";
		this.field_Shift.Width = 69;
		this.field_Quantity.Appearance.Cell.BackColor = System.Drawing.Color.FromArgb(244, 245, 246);
		this.field_Quantity.Appearance.Cell.Options.UseBackColor = true;
		this.field_Quantity.Appearance.CellGrandTotal.BackColor = System.Drawing.Color.PeachPuff;
		this.field_Quantity.Appearance.CellGrandTotal.Options.UseBackColor = true;
		this.field_Quantity.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
		this.field_Quantity.AreaIndex = 0;
		this.field_Quantity.Caption = "Quantity";
		this.field_Quantity.FieldEdit = this.rQuantity;
		this.field_Quantity.FieldName = "Qty";
		this.field_Quantity.Name = "field_Quantity";
		this.rQuantity.Appearance.BackColor = System.Drawing.Color.White;
		this.rQuantity.Appearance.BackColor2 = System.Drawing.Color.White;
		this.rQuantity.Appearance.Options.UseBackColor = true;
		this.rQuantity.AutoHeight = false;
		this.rQuantity.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rQuantity.Name = "rQuantity";
		this.txtFilterField.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.txtFilterField.EditControl = null;
		this.txtFilterField.FieldName = null;
		this.txtFilterField.IsSearchControl = false;
		this.txtFilterField.Location = new System.Drawing.Point(650, 576);
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
		this.txtFilterField.Properties.ShowHeader = false;
		this.txtFilterField.Properties.ValueMember = "FilterCode";
		this.txtFilterField.SettingInVisible = false;
		this.txtFilterField.SettingReadonly = false;
		this.txtFilterField.Size = new System.Drawing.Size(100, 20);
		this.txtFilterField.TabIndex = 21;
		this.txtFilterValue.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.txtFilterValue.EditControl = null;
		this.txtFilterValue.FieldName = null;
		this.txtFilterValue.IsSearchControl = false;
		this.txtFilterValue.Location = new System.Drawing.Point(757, 576);
		this.txtFilterValue.Name = "txtFilterValue";
		this.txtFilterValue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txtFilterValue.SettingInVisible = false;
		this.txtFilterValue.SettingReadonly = false;
		this.txtFilterValue.Size = new System.Drawing.Size(111, 20);
		this.txtFilterValue.SourceFilter = null;
		this.txtFilterValue.TabIndex = 22;
		this.rPlaning.AutoHeight = false;
		this.rPlaning.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rPlaning.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("TypePlanning", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rPlaning.DisplayMember = "Name";
		this.rPlaning.Name = "rPlaning";
		this.rPlaning.ShowFooter = false;
		this.rPlaning.ShowHeader = false;
		this.rPlaning.ValueMember = "TypePlanning";
		this.capacityGanttChartToolStripMenuItem.Name = "capacityGanttChartToolStripMenuItem";
		this.capacityGanttChartToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
		this.capacityGanttChartToolStripMenuItem.Text = "Capacity Gantt Chart";
		this.capacityGanttChartToolStripMenuItem.Click += new System.EventHandler(this.capacityGanttChartToolStripMenuItem_Click);
		this.btnAutoFillData.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnAutoFillData.EditControl = null;
		this.btnAutoFillData.EditValue = "Auto Fill";
		this.btnAutoFillData.FieldName = null;
		this.btnAutoFillData.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnAutoFillData.ImageOptions.Image");
		this.btnAutoFillData.IsSearchControl = false;
		this.btnAutoFillData.Location = new System.Drawing.Point(874, 576);
		this.btnAutoFillData.Name = "btnAutoFillData";
		this.btnAutoFillData.ReadOnly = false;
		this.btnAutoFillData.SettingInVisible = false;
		this.btnAutoFillData.SettingReadonly = false;
		this.btnAutoFillData.Size = new System.Drawing.Size(87, 20);
		this.btnAutoFillData.TabIndex = 18;
		this.btnAutoFillData.Text = "Auto Fill";
		this.btnAutoFillData.Click += new System.EventHandler(this.btnAutoFillData_Click);
		this.lbFilterBy.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.lbFilterBy.EditControl = null;
		this.lbFilterBy.EditValue = "Filter by";
		this.lbFilterBy.FieldName = null;
		this.lbFilterBy.HideWhenEmpty = false;
		this.lbFilterBy.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbFilterBy.Location = new System.Drawing.Point(568, 578);
		this.lbFilterBy.Name = "lbFilterBy";
		this.lbFilterBy.ReadOnly = false;
		this.lbFilterBy.SettingInVisible = false;
		this.lbFilterBy.SettingReadonly = false;
		this.lbFilterBy.ShowUnderLine = true;
		this.lbFilterBy.Size = new System.Drawing.Size(80, 20);
		this.lbFilterBy.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbFilterBy.TabIndex = 20;
		this.lbFilterBy.Utext = "Filter by";
		this.chkDetailSchedule.EditControl = null;
		this.chkDetailSchedule.FieldName = null;
		this.chkDetailSchedule.IsSearchControl = false;
		this.chkDetailSchedule.Location = new System.Drawing.Point(824, 10);
		this.chkDetailSchedule.Name = "chkDetailSchedule";
		this.chkDetailSchedule.Properties.Caption = "Detailed scheduling";
		this.chkDetailSchedule.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.chkDetailSchedule.Properties.ValueChecked = "Y";
		this.chkDetailSchedule.Properties.ValueUnchecked = "N";
		this.chkDetailSchedule.SettingInVisible = false;
		this.chkDetailSchedule.SettingReadonly = false;
		this.chkDetailSchedule.Size = new System.Drawing.Size(123, 20);
		this.chkDetailSchedule.TabIndex = 23;
		this.chkDetailSchedule.EditValueChanged += new System.EventHandler(this.chkDetailSchedule_EditValueChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(970, 603);
		this.ContextMenuStrip = this.DocContextMenu;
		base.Controls.Add(this.chkDetailSchedule);
		base.Controls.Add(this.txtFilterValue);
		base.Controls.Add(this.txtFilterField);
		base.Controls.Add(this.lbFilterBy);
		base.Controls.Add(this.pgcDetail);
		base.Controls.Add(this.btnAutoFillData);
		base.Controls.Add(this.txtName);
		base.Controls.Add(this.lbName);
		base.Controls.Add(this.lbCode);
		base.Controls.Add(this.txtCode);
		base.Controls.Add(this.lbTYpe);
		base.Controls.Add(this.lbfixdate);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.txtToDate);
		base.Controls.Add(this.lbtofstDate);
		base.Controls.Add(this.txtFromDate);
		base.Controls.Add(this.lbfromdate);
		base.Controls.Add(this.txtStatus);
		base.Controls.Add(this.lbStatus);
		base.Controls.Add(this.txtInputCode);
		base.Controls.Add(this.lbInputCode);
		base.Controls.Add(this.txtFixDate);
		base.Controls.Add(this.txtType);
		base.FormId = "oSchedule";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("VirSchedule.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "VirSchedule";
		base.ShowInTaskbar = false;
		this.Text = "Schedule";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Schedule_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.txtInputCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFixDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFixDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).EndInit();
		this.DocContextMenu.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pgcDetail).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rQuantity).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFilterField.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFilterValue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rPlaning).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkDetailSchedule.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
