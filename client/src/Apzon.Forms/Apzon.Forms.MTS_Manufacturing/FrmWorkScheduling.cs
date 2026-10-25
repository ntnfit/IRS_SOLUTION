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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
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

namespace Apzon.Forms.MTS_Manufacturing;

public class FrmWorkScheduling : XtraFormEx
{
	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string SaveString = "Save";

	private const string Controller = "WorkScheduling";

	private bool _isLoad;

	private IContainer components;

	private GridColumnEx cl_ShiftName;

	private SimpleButtonEx btn_Refresh;

	private SimpleButtonEx btn_Save;

	private SimpleButtonEx btn_Scheduling;

	private MemoEditEx txt_Remarks;

	private DateEditEx txt_From;

	private DateEditEx txt_To;

	private TextEditEx txt_Name;

	private GridColumnEx gc_Remark;

	private GridColumnEx gc_Shift;

	private GridColumnEx gc_Date;

	private GridControlEx grd_Lines;

	private ComboBoxEditEx txt_Year;

	private GroupControl groupControl3;

	private GroupControl groupControl2;

	private GridColumnEx gcYear;

	private GridColumnEx gc_Name;

	private GridColumnEx gc_ID;

	private GridControlEx grd_WorkScheduling;

	private GroupControl groupControl1;

	private GridColumnEx cl_ShiftCode;

	private LabelLine labelLine1;

	private LabelLine labelLine5;

	private LabelLine labelLine4;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private GridViewEx gv_Lines;

	private GridViewEx gv_WorkScheduling;

	private SimpleButtonEx btnCancel;

	private ContextMenuStrip contextMenuStrip;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ToolStripMenuItem addToolStripMenuItem;

	private RepositoryItemButtonEdit rShift;

	private ContextMenuStrip contextMenuStripLine;

	private ToolStripMenuItem chooseShiftToolStripMenuItem;

	private LabelLine lbBranch;

	private LookUpEdit txt_Branch;

	private RepositoryItemDateEdit rdate;

	public int idDrillDown { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
			{
				WinFuntions.DisableAllControl(Controls);
				btnCancel.Enabled = true;
			}
			else
			{
				if (FormAuth == Enums.FormAuthorities.Read)
				{
					return;
				}
				_formMode = value;
				switch (value)
				{
				case Enums.FormViewModeType.View:
					EnableDisableControl(isEnable: true);
					if (gv_WorkScheduling.FocusedRowHandle < 0)
					{
						gv_Lines.ReadOnly = true;
						txt_Name.ReadOnly = true;
						txt_Remarks.ReadOnly = true;
						txt_From.ReadOnly = true;
						txt_To.ReadOnly = true;
						txt_Year.ReadOnly = true;
					}
					btn_Save.Text = "OK";
					break;
				case Enums.FormViewModeType.Update:
					EnableDisableControl(isEnable: true);
					btn_Save.Text = "Update";
					break;
				case Enums.FormViewModeType.Add:
					EnableDisableControl(isEnable: true);
					btn_Save.Text = "Save";
					break;
				case Enums.FormViewModeType.Busy:
					EnableDisableControl(isEnable: false);
					break;
				}
			}
		}
		catch
		{
		}
	}

	public FrmWorkScheduling()
	{
		InitializeComponent();
		OnLoadAsync += FrmWorkScheduling_Load;
		FormMode = Enums.FormViewModeType.View;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private async Task FrmWorkScheduling_Load(object sender, EventArgs e)
	{
		deleteToolStripMenuItem.Visible = false;
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			await SetDataBranch();
			InitFormatControl();
			LoadDataYear();
			await LoadData();
			await FocusedRowByIdDrillDown();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
	}

	private void btn_Refresh_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			RefreshData(isRefreshDetail: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private void btn_Scheduling_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		WinCommons._CloseProcessing();
	}

	private void gv_WorkScheduling_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			await Save();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void gv_WorkScheduling_ColumnFilterChanged(object sender, EventArgs e)
	{
		try
		{
			gv_WorkScheduling.FocusedRowHandle = -2147483646;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_WorkScheduling_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
	}

	private void gv_WorkScheduling_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			_isLoad = true;
			if (gv_WorkScheduling.FocusedRowHandle >= 0)
			{
				LoadDataDetail(gv_WorkScheduling.GetFocusedDataRow());
			}
			else
			{
				LoadDataDetail(null);
			}
			_isLoad = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private void gv_Lines_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				await Delete(Function.ParseInt(gv_WorkScheduling.GetFocusedRowCellValue("Id")));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			AddNew();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.Add;
		WinCommons._CloseProcessing();
	}

	private void txt_Remarks_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (!_isLoad)
			{
				if (FormMode != Enums.FormViewModeType.Busy && string.IsNullOrEmpty(txt_Name.Text))
				{
					WinCommons.Error(Enums.MessageCode.YouMustEnterNameBeforeEnterRemarks, "");
					e.Cancel = true;
				}
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
	}

	private void txt_Name_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (!_isLoad && FormMode == Enums.FormViewModeType.View)
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

	private void txt_Year_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (_isLoad)
			{
				return;
			}
			if (string.IsNullOrEmpty(txt_Name.Text))
			{
				WinCommons.Error(Enums.MessageCode.YouMustEnterNameBeforeEnterYear, "");
				e.Cancel = true;
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				txt_From.EditValue = null;
				txt_To.EditValue = null;
				gv_WorkScheduling.SetFocusedRowCellValue(gcYear.FieldName, Function.ParseInt(e.NewValue));
				grd_WorkScheduling.RefreshDataSource();
				grd_WorkScheduling.DataSourceTable.AcceptChanges();
				DataTable dataTable = grd_Lines.DataSourceTable.Copy();
				dataTable.Clear();
				grd_Lines.DataSource = dataTable;
				grd_Lines.RefreshDataSource();
				grd_Lines.DataSourceTable.AcceptChanges();
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
			else
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

	private void txt_From_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (_isLoad)
			{
				return;
			}
			if (string.IsNullOrEmpty(txt_Name.Text))
			{
				WinCommons.Error(Enums.MessageCode.YouMustEnterNameBeforeEnterFromDate, "");
				e.Cancel = true;
				return;
			}
			if (Function.ParseDateTimes(e.NewValue).Year != Function.ParseInt(txt_Year.EditValue))
			{
				WinCommons.Error(Enums.MessageCode.YouMustChooseDayInYearSelected, "");
				e.Cancel = true;
				return;
			}
			if (Function.ParseDateTimes(e.NewValue) > Function.ParseDateTimes(txt_To.EditValue))
			{
				txt_To.EditValue = e.NewValue;
			}
			if (FormMode == Enums.FormViewModeType.View)
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

	private void txt_To_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (_isLoad)
			{
				return;
			}
			if (string.IsNullOrEmpty(txt_Name.Text))
			{
				WinCommons.Error(Enums.MessageCode.YouMustEnterNameBeforeEnterToDate, "");
				e.Cancel = true;
				return;
			}
			if (Function.ParseDateTimes(e.NewValue).Year != Function.ParseInt(txt_Year.EditValue))
			{
				WinCommons.Error(Enums.MessageCode.YouMustChooseDayInYearSelected, "");
				e.Cancel = true;
				return;
			}
			if (Function.ParseDateTimes(e.NewValue) < Function.ParseDateTimes(txt_From.EditValue))
			{
				txt_From.EditValue = e.NewValue;
			}
			if (FormMode == Enums.FormViewModeType.View)
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

	private void txt_From_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy)
			{
				ChangeDataLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_To_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Busy)
			{
				ChangeDataLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rShift_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string[] prms = new string[1] { Function.ToString(txt_Branch.EditValue) };
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.DefinedShift, multiSelect: true, "", "", Enums.ItemObjectType.NoItemType, null, "", prms);
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count <= 0)
			{
				return;
			}
			string text = "";
			foreach (DataRow chooseItem in frm_CFL2.ChooseItems)
			{
				text = ((!string.IsNullOrEmpty(text)) ? (text + "," + Function.ToString(chooseItem["Code"])) : Function.ToString(chooseItem["Code"]));
			}
			gv_Lines.SetFocusedRowCellValue(gc_Shift.FieldName, text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Lines_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
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

	private void gv_Lines_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			switch (Function.ParseDateTimes(gv_Lines.GetRowCellValue(e.RowHandle, gc_Date.FieldName)).DayOfWeek)
			{
			case DayOfWeek.Saturday:
				e.Appearance.BackColor = Color.FromArgb(233, 249, 242);
				break;
			case DayOfWeek.Sunday:
				e.Appearance.BackColor = Color.FromArgb(225, 234, 237);
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void grd_Lines_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				Enums.FormViewModeType formMode = FormMode;
				FormMode = Enums.FormViewModeType.Busy;
				ExtenseionMethods.PasteFromExcelToDataTable(grd_Lines, gv_Lines, new List<string> { gc_Date.FieldName });
				FormMode = formMode;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chooseShiftToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int[] selectedRows = gv_Lines.GetSelectedRows();
			if (selectedRows == null || selectedRows.Count() == 0)
			{
				return;
			}
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.DefinedShift, multiSelect: true, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItems == null || frm_ChooseFromList2.ChooseItems.Count <= 0)
			{
				return;
			}
			string text = "";
			foreach (dynamic chooseItem in frm_ChooseFromList2.ChooseItems)
			{
				text = ((!string.IsNullOrEmpty(text)) ? ((string)(text + ("," + Function.ToString(chooseItem["Code"])))) : ((string)Function.ToString(chooseItem["Code"])));
			}
			for (int i = 0; i < selectedRows.Count(); i++)
			{
				gv_Lines.SetRowCellValue(selectedRows[i], gc_Shift.FieldName, text);
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
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void txt_Branch_EditValueChanged(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Busy)
		{
			return;
		}
		Enums.FormViewModeType view = FormMode;
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (view != Enums.FormViewModeType.Add)
			{
				await LoadData();
				view = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = view;
		WinCommons._CloseProcessing();
	}

	private void gv_Lines_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			if (e.Button != MouseButtons.Right || !(gv_Lines.FocusedColumn.FieldName == gc_Shift.FieldName))
			{
				return;
			}
			string[] prms = new string[1] { Function.ToString(txt_Branch.EditValue) };
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.DefinedShift, multiSelect: true, "", "", Enums.ItemObjectType.NoItemType, null, "", prms);
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count <= 0)
			{
				return;
			}
			string text = "";
			foreach (DataRow chooseItem in frm_CFL2.ChooseItems)
			{
				text = ((!string.IsNullOrEmpty(text)) ? (text + "," + Function.ToString(chooseItem["Code"])) : Function.ToString(chooseItem["Code"]));
			}
			int[] selectedRows = gv_Lines.GetSelectedRows();
			for (int i = 0; i < selectedRows.Length; i++)
			{
				gv_Lines.SetRowCellValue(selectedRows[i], gc_Shift.FieldName, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Lines_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gv_Lines.FocusedColumn.FieldName.Equals(gc_Shift.FieldName) && Function.ParseDateTimes(gv_Lines.GetFocusedRowCellValue(gc_Date)).Date < DateTime.Now.Date)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MODE_FIND()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			AddNew();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.Add;
		WinCommons._CloseProcessing();
	}

	private void InitFormatControl()
	{
		try
		{
			txt_From.SetFormatControl(Enums.DisplayFormatType.DateTime);
			txt_To.SetFormatControl(Enums.DisplayFormatType.DateTime);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void RefreshData(bool isRefreshDetail)
	{
		if (isRefreshDetail)
		{
			_isLoad = true;
			LoadDataDetail(gv_WorkScheduling.GetFocusedDataRow());
			_isLoad = false;
		}
		else
		{
			await LoadData();
		}
	}

	private async Task<DataTable> GetDataBrach()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			return await http.GetAsync("GetDataBrach", new
			{
				userCode = GlobalData.UserCodeLogin
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task SetDataBranch()
	{
		try
		{
			txt_Branch.EditValue = GlobalData.DefaultBranch;
			if (!GlobalData.IsMultiBranch)
			{
				txt_Branch.Enabled = false;
				return;
			}
			DataTable dataSource = await GetDataBrach();
			txt_Branch.Properties.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetDataDoc()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "WorkScheduling");
			return await http.GetAsync("GetDataDoc", new
			{
				branch = Function.ParseInt(txt_Branch.EditValue)
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task LoadData()
	{
		try
		{
			int oldFocus = gv_WorkScheduling.FocusedRowHandle;
			DataTable dataSource = await GetDataDoc();
			grd_WorkScheduling.DataSource = dataSource;
			grd_WorkScheduling.RefreshDataSource();
			grd_WorkScheduling.DataSourceTable.AcceptChanges();
			_isLoad = true;
			if (gv_WorkScheduling.FocusedRowHandle < 0)
			{
				LoadDataDetail(null);
			}
			else if (gv_WorkScheduling.FocusedRowHandle == oldFocus)
			{
				LoadDataDetail(gv_WorkScheduling.GetFocusedDataRow());
			}
			_isLoad = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadDataDetail(DataRow row)
	{
		try
		{
			if (row == null)
			{
				txt_Name.EditValue = "";
				txt_Remarks.EditValue = "";
				txt_Year.EditValue = DateTime.Now.Year;
				txt_From.EditValue = null;
				txt_To.EditValue = null;
				grd_Lines.DataSource = null;
				grd_Lines.RefreshDataSource();
				grd_Lines.DataSourceTable.AcceptChanges();
				return;
			}
			txt_Name.EditValue = Function.ToString(row["Name"]);
			txt_Remarks.EditValue = Function.ToString(row["Notes"]);
			txt_Year.EditValue = Function.ParseInt(row["Year"]);
			txt_From.EditValue = Function.ParseDateTime(row["FromDate"]);
			txt_To.EditValue = Function.ParseDateTime(row["ToDate"]);
			if (Function.ParseInt(row["Id"]) == 0)
			{
				DataTable dataTable = grd_Lines.DataSourceTable.Copy();
				if (dataTable.Rows.Count == 0)
				{
					dataTable = GetColumnLine();
				}
				dataTable.Clear();
				grd_Lines.DataSource = dataTable;
				grd_Lines.RefreshDataSource();
				grd_Lines.DataSourceTable.AcceptChanges();
			}
			else
			{
				DataTable dataLine = GetDataLine(Function.ParseInt(row["Id"]));
				grd_Lines.DataSource = dataLine;
				grd_Lines.RefreshDataSource();
				grd_Lines.DataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataTable GetDataLine(int docId)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "WorkScheduling");
			return httpClientHelper.Get("GetDataLine", new { docId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task Save()
	{
		Enums.FormViewModeType oldview = FormMode;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					Close();
					WinCommons._CloseProcessing();
					return;
				}
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
				{
					return;
				}
				WinCommons._OpenProcessing();
				FormMode = Enums.FormViewModeType.Busy;
				string text = CheckSave();
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					FormMode = oldview;
					return;
				}
				DataRow row = gv_WorkScheduling.GetFocusedDataRow();
				text = GetDataSave(ref row);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					FormMode = oldview;
					return;
				}
				DataTable dataTable = grd_WorkScheduling.DataSourceTable.Clone();
				dataTable.ImportRow(row);
				DataSet dataSet = new DataSet();
				dataTable.TableName = "Table Doc";
				dataSet.Tables.Add(dataTable);
				dataSet.Tables.Add(GetDataLineSave());
				using (HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "WorkScheduling"))
				{
					text = (await http.PostAsync((oldview == Enums.FormViewModeType.Add) ? "Save" : "Update", dataSet)).Message;
				}
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success((oldview == Enums.FormViewModeType.Update) ? Enums.MessageCode.UpdateSuccess : Enums.MessageCode.SaveSuccess, "");
					await LoadData();
					oldview = Enums.FormViewModeType.View;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = oldview;
		WinCommons._CloseProcessing();
	}

	private string GetDataSave(ref DataRow row)
	{
		try
		{
			if (row == null)
			{
				return "Data is null!";
			}
			row[gc_Name.FieldName] = txt_Name.Text;
			row["FromDate"] = txt_From.EditValue;
			row["ToDate"] = txt_To.EditValue;
			row["Notes"] = txt_Remarks.Text;
			row["Year"] = txt_Year.EditValue;
			row["BplID"] = GlobalData.DefaultBranch;
			if (GlobalData.IsMultiBranch)
			{
				row["BplID"] = Function.ParseInt(txt_Branch.EditValue);
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private DataTable GetDataLineSave()
	{
		try
		{
			DataTable dataSourceTable = grd_Lines.DataSourceTable;
			if (dataSourceTable == null || dataSourceTable.Rows.Count == 0)
			{
				throw new ArgumentException("Data lines must not null!");
			}
			DataTable dataTable = dataSourceTable.Copy();
			dataTable.Clear();
			for (int i = 0; i < dataSourceTable.Rows.Count; i++)
			{
				string text = Function.ToString(dataSourceTable.Rows[i]["Shift"]);
				if (string.IsNullOrEmpty(text))
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow.ItemArray = dataSourceTable.Rows[i].ItemArray;
					dataRow["LinID"] = 0;
					dataTable.Rows.Add(dataRow);
					continue;
				}
				int num = 0;
				string[] array = text.Split(',');
				foreach (string value in array)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow.ItemArray = dataSourceTable.Rows[i].ItemArray;
					dataRow["LinID"] = num;
					dataRow["Shift"] = value;
					dataTable.Rows.Add(dataRow);
					num++;
				}
			}
			dataTable.AcceptChanges();
			dataTable.TableName = "Table Line";
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private async Task Delete(int id)
	{
		Enums.FormViewModeType view = FormMode;
		try
		{
			FormMode = Enums.FormViewModeType.Busy;
			using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "WorkScheduling");
			string message = (await http.PostAsync("Delete", id)).Message;
			if (string.IsNullOrEmpty(message))
			{
				WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
				await LoadData();
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, message);
				FormMode = view;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			FormMode = view;
		}
	}

	private void LoadDataYear()
	{
		try
		{
			List<int> list = new List<int>();
			for (int i = DateTime.Now.Year - 100; i < DateTime.Now.Year + 100; i++)
			{
				list.Add(i);
			}
			txt_Year.Properties.Items.AddRange(list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private string CheckSave()
	{
		try
		{
			if (GlobalData.IsMultiBranch && Function.ParseInt(txt_Branch.EditValue) == 0)
			{
				return "You must chosse branch to save!";
			}
			if (string.IsNullOrEmpty(txt_Name.Text))
			{
				return "Work scheduling name must not null!";
			}
			if (Function.ParseDateTimes(txt_From.EditValue) > Function.ParseDateTimes(txt_To.EditValue))
			{
				return "TO date must be greater than from date!";
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private DataTable GetColumnDoc()
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "WorkScheduling");
		return httpClientHelper.Get("GetColumnDoc");
	}

	private DataTable GetColumnLine()
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "WorkScheduling");
		return httpClientHelper.Get("GetColumnLine");
	}

	private void AddNew()
	{
		try
		{
			DataTable dataTable = grd_WorkScheduling.DataSourceTable.Copy();
			if (dataTable.Rows.Count == 0)
			{
				dataTable = GetColumnDoc();
			}
			else
			{
				dataTable.Clear();
				DataRow dataRow = dataTable.NewRow();
				dataRow["Year"] = DateTime.Now.Year;
				dataTable.Rows.Add(dataRow);
			}
			grd_WorkScheduling.DataSource = dataTable;
			grd_WorkScheduling.RefreshDataSource();
			grd_WorkScheduling.DataSourceTable.AcceptChanges();
			gv_WorkScheduling.FocusedRowHandle = 0;
			gv_WorkScheduling_FocusedRowChanged(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChangeDataLine()
	{
		try
		{
			if (_isLoad || !Function.ParseDateTime(txt_To.EditValue).HasValue || !Function.ParseDateTime(txt_From.EditValue).HasValue)
			{
				return;
			}
			List<DataRow> list = grd_Lines.DataSourceTable.Copy().Select().ToList();
			DataTable dataTable = grd_Lines.DataSourceTable.Copy();
			dataTable.Clear();
			DateTime i = Function.ParseDateTimes(txt_From.EditValue);
			while (i <= Function.ParseDateTimes(txt_To.EditValue))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Id"] = Function.ParseInt(gv_WorkScheduling.GetFocusedRowCellValue("Id"));
				dataRow["LinID"] = dataTable.Rows.Count;
				dataRow["WorkDate"] = i.Date;
				dataRow["Shift"] = "";
				if (list.Exists((DataRow t) => Function.ParseDateTimes(t["WorkDate"]).Date == i.Date))
				{
					dataRow["Shift"] = list.FindLast((DataRow t) => Function.ParseDateTimes(t["WorkDate"]).Date == i.Date)["Shift"];
				}
				dataTable.Rows.Add(dataRow);
				i = i.AddDays(1.0);
			}
			grd_Lines.DataSource = dataTable;
			grd_Lines.RefreshDataSource();
			grd_Lines.DataSourceTable.AcceptChanges();
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
			gv_Lines.ReadOnly = !isEnable;
			contextMenuStrip.Enabled = isEnable;
			txt_Name.ReadOnly = !isEnable;
			txt_Remarks.ReadOnly = !isEnable;
			txt_From.ReadOnly = !isEnable;
			txt_To.ReadOnly = !isEnable;
			txt_Year.ReadOnly = !isEnable;
			btn_Scheduling.Enabled = isEnable;
			btn_Save.Enabled = isEnable;
			btn_Refresh.Enabled = isEnable;
			btnCancel.Enabled = isEnable;
			txt_Branch.ReadOnly = !isEnable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task FocusedRowByIdDrillDown()
	{
		try
		{
			int num = gv_WorkScheduling.LocateByValue(gc_ID.FieldName, idDrillDown);
			if (num != int.MinValue)
			{
				gv_WorkScheduling.FocusedRowHandle = num;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTS_Manufacturing.FrmWorkScheduling));
		this.cl_ShiftName = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Scheduling = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_Remarks = new Apzon.Controls.Editor.MemoEditEx();
		this.txt_From = new Apzon.Controls.Editor.DateEditEx();
		this.txt_To = new Apzon.Controls.Editor.DateEditEx();
		this.txt_Name = new Apzon.Controls.Editor.TextEditEx();
		this.gv_Lines = new Apzon.Controls.Grid.GridViewEx();
		this.gc_Date = new Apzon.Controls.Grid.GridColumnEx();
		this.rdate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gc_Shift = new Apzon.Controls.Grid.GridColumnEx();
		this.rShift = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_Remark = new Apzon.Controls.Grid.GridColumnEx();
		this.grd_Lines = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStripLine = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.chooseShiftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.txt_Year = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.groupControl3 = new DevExpress.XtraEditors.GroupControl();
		this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Refresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.gv_WorkScheduling = new Apzon.Controls.Grid.GridViewEx();
		this.gc_ID = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Name = new Apzon.Controls.Grid.GridColumnEx();
		this.gcYear = new Apzon.Controls.Grid.GridColumnEx();
		this.grd_WorkScheduling = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.cl_ShiftCode = new Apzon.Controls.Grid.GridColumnEx();
		this.lbBranch = new Apzon.Controls.Controls.LabelLine();
		this.txt_Branch = new DevExpress.XtraEditors.LookUpEdit();
		((System.ComponentModel.ISupportInitialize)this.txt_Remarks.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_From.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_From.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_To.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_To.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Lines).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rdate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rdate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rShift).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Lines).BeginInit();
		this.contextMenuStripLine.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Year.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl3).BeginInit();
		this.groupControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).BeginInit();
		this.groupControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_WorkScheduling).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_WorkScheduling).BeginInit();
		this.contextMenuStrip.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Branch.Properties).BeginInit();
		base.SuspendLayout();
		this.cl_ShiftName.Caption = "Name";
		this.cl_ShiftName.CurrencyDateField = null;
		this.cl_ShiftName.CurrencyField = null;
		this.cl_ShiftName.CurrencyFixed = null;
		this.cl_ShiftName.CurrencyFixedRate = 0.0;
		this.cl_ShiftName.CurrencyRateField = null;
		this.cl_ShiftName.FieldName = "Name";
		this.cl_ShiftName.Name = "cl_ShiftName";
		this.cl_ShiftName.PrimaryKey = false;
		this.cl_ShiftName.UnitField = null;
		this.cl_ShiftName.Visible = true;
		this.cl_ShiftName.VisibleIndex = 1;
		this.btn_Scheduling.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Scheduling.EditControl = null;
		this.btn_Scheduling.EditValue = "Scheduling";
		this.btn_Scheduling.FieldName = null;
		this.btn_Scheduling.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Scheduling.ImageOptions.Image");
		this.btn_Scheduling.IsSearchControl = false;
		this.btn_Scheduling.Location = new System.Drawing.Point(111, 404);
		this.btn_Scheduling.Name = "btn_Scheduling";
		this.btn_Scheduling.ReadOnly = false;
		this.btn_Scheduling.SettingInVisible = false;
		this.btn_Scheduling.SettingReadonly = false;
		this.btn_Scheduling.Size = new System.Drawing.Size(80, 20);
		this.btn_Scheduling.TabIndex = 6;
		this.btn_Scheduling.Text = "Scheduling";
		this.btn_Scheduling.Visible = false;
		this.btn_Scheduling.Click += new System.EventHandler(this.btn_Scheduling_Click);
		this.txt_Remarks.EditControl = null;
		this.txt_Remarks.FieldName = null;
		this.txt_Remarks.IsSearchControl = false;
		this.txt_Remarks.Location = new System.Drawing.Point(135, 47);
		this.txt_Remarks.Name = "txt_Remarks";
		this.txt_Remarks.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_Remarks.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_Remarks.SettingInVisible = false;
		this.txt_Remarks.SettingReadonly = false;
		this.txt_Remarks.Size = new System.Drawing.Size(282, 66);
		this.txt_Remarks.TabIndex = 3;
		this.txt_Remarks.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_Remarks_EditValueChanging);
		this.txt_From.EditControl = null;
		this.txt_From.EditValue = null;
		this.txt_From.FieldName = null;
		this.txt_From.Location = new System.Drawing.Point(135, 116);
		this.txt_From.Name = "txt_From";
		this.txt_From.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_From.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_From.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_From.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txt_From.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.txt_From.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.txt_From.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.txt_From.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.txt_From.Properties.IsSearchControl = false;
		this.txt_From.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.txt_From.SettingInVisible = false;
		this.txt_From.SettingReadonly = false;
		this.txt_From.Size = new System.Drawing.Size(83, 20);
		this.txt_From.TabIndex = 4;
		this.txt_From.EditValueChanged += new System.EventHandler(this.txt_From_EditValueChanged);
		this.txt_From.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_From_EditValueChanging);
		this.txt_To.EditControl = null;
		this.txt_To.EditValue = null;
		this.txt_To.FieldName = null;
		this.txt_To.Location = new System.Drawing.Point(334, 116);
		this.txt_To.Name = "txt_To";
		this.txt_To.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_To.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_To.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_To.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txt_To.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.txt_To.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.txt_To.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.txt_To.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.txt_To.Properties.IsSearchControl = false;
		this.txt_To.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.txt_To.SettingInVisible = false;
		this.txt_To.SettingReadonly = false;
		this.txt_To.Size = new System.Drawing.Size(83, 20);
		this.txt_To.TabIndex = 5;
		this.txt_To.EditValueChanged += new System.EventHandler(this.txt_To_EditValueChanged);
		this.txt_To.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_To_EditValueChanging);
		this.txt_Name.EditControl = null;
		this.txt_Name.FieldName = null;
		this.txt_Name.IsSearchControl = false;
		this.txt_Name.Location = new System.Drawing.Point(135, 24);
		this.txt_Name.Name = "txt_Name";
		this.txt_Name.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_Name.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_Name.SettingInVisible = false;
		this.txt_Name.SettingReadonly = false;
		this.txt_Name.Size = new System.Drawing.Size(149, 20);
		this.txt_Name.TabIndex = 1;
		this.txt_Name.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_Name_EditValueChanging);
		this.gv_Lines.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Lines.ColumnKey = null;
		this.gv_Lines.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gc_Date, this.gc_Shift, this.gc_Remark });
		this.gv_Lines.DisableFormSetting = false;
		this.gv_Lines.DisplayException = false;
		this.gv_Lines.EditColumn = null;
		this.gv_Lines.FieldCurrency = null;
		this.gv_Lines.FieldRate = null;
		this.gv_Lines.FormId = "oWorkScheduling";
		this.gv_Lines.GridControl = this.grd_Lines;
		this.gv_Lines.IgnoreFilter = false;
		this.gv_Lines.IndexEditRow = -1;
		this.gv_Lines.IndicatorWidth = 30;
		this.gv_Lines.IsApzTable = false;
		this.gv_Lines.IsInsertNewRow = false;
		this.gv_Lines.IsLoadingData = false;
		this.gv_Lines.IsMappingContextAction = false;
		this.gv_Lines.ItemId = "APZ_WSH1";
		this.gv_Lines.LstColumnAllowBatch = null;
		this.gv_Lines.LstColumnNotAllowBatch = null;
		this.gv_Lines.LstColumnNotEditable = null;
		this.gv_Lines.LstColumNotSetting = null;
		this.gv_Lines.Name = "gv_Lines";
		this.gv_Lines.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
		this.gv_Lines.OptionsSelection.MultiSelect = true;
		this.gv_Lines.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.gv_Lines.OptionsView.ShowGroupPanel = false;
		this.gv_Lines.UseBatchMod = false;
		this.gv_Lines.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Lines_CustomDrawRowIndicator);
		this.gv_Lines.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Lines_RowStyle);
		this.gv_Lines.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Lines_ShowingEditor);
		this.gv_Lines.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Lines_CellValueChanged);
		this.gv_Lines.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gv_Lines_MouseDown);
		this.gc_Date.Caption = "Work Day";
		this.gc_Date.ColumnEdit = this.rdate;
		this.gc_Date.CurrencyDateField = null;
		this.gc_Date.CurrencyField = null;
		this.gc_Date.CurrencyFixed = null;
		this.gc_Date.CurrencyFixedRate = 0.0;
		this.gc_Date.CurrencyRateField = null;
		this.gc_Date.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gc_Date.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gc_Date.FieldName = "WorkDate";
		this.gc_Date.Name = "gc_Date";
		this.gc_Date.OptionsColumn.ReadOnly = true;
		this.gc_Date.PrimaryKey = false;
		this.gc_Date.UnitField = null;
		this.gc_Date.Visible = true;
		this.gc_Date.VisibleIndex = 0;
		this.gc_Date.Width = 162;
		this.rdate.AutoHeight = false;
		this.rdate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rdate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rdate.Name = "rdate";
		this.gc_Shift.Caption = "Shift";
		this.gc_Shift.ColumnEdit = this.rShift;
		this.gc_Shift.CurrencyDateField = null;
		this.gc_Shift.CurrencyField = null;
		this.gc_Shift.CurrencyFixed = null;
		this.gc_Shift.CurrencyFixedRate = 0.0;
		this.gc_Shift.CurrencyRateField = null;
		this.gc_Shift.FieldName = "Shift";
		this.gc_Shift.Name = "gc_Shift";
		this.gc_Shift.PrimaryKey = false;
		this.gc_Shift.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_Shift.UnitField = null;
		this.gc_Shift.Visible = true;
		this.gc_Shift.VisibleIndex = 1;
		this.gc_Shift.Width = 225;
		this.rShift.AutoHeight = false;
		this.rShift.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rShift.Name = "rShift";
		this.rShift.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rShift_ButtonClick);
		this.gc_Remark.Caption = "Remark";
		this.gc_Remark.CurrencyDateField = null;
		this.gc_Remark.CurrencyField = null;
		this.gc_Remark.CurrencyFixed = null;
		this.gc_Remark.CurrencyFixedRate = 0.0;
		this.gc_Remark.CurrencyRateField = null;
		this.gc_Remark.FieldName = "Remark";
		this.gc_Remark.Name = "gc_Remark";
		this.gc_Remark.PrimaryKey = false;
		this.gc_Remark.UnitField = null;
		this.grd_Lines.AllowPasteExcel = true;
		this.grd_Lines.ClipboardData = null;
		this.grd_Lines.ContextMenuStrip = this.contextMenuStripLine;
		this.grd_Lines.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Lines.Location = new System.Drawing.Point(2, 23);
		this.grd_Lines.MainView = this.gv_Lines;
		this.grd_Lines.Name = "grd_Lines";
		this.grd_Lines.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rShift, this.rdate });
		this.grd_Lines.Size = new System.Drawing.Size(421, 226);
		this.grd_Lines.TabIndex = 0;
		this.grd_Lines.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Lines });
		this.grd_Lines.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_Lines_EditorKeyDown);
		this.contextMenuStripLine.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.chooseShiftToolStripMenuItem });
		this.contextMenuStripLine.Name = "contextMenuStripLine";
		this.contextMenuStripLine.Size = new System.Drawing.Size(142, 26);
		this.chooseShiftToolStripMenuItem.Name = "chooseShiftToolStripMenuItem";
		this.chooseShiftToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
		this.chooseShiftToolStripMenuItem.Text = "Choose Shift";
		this.chooseShiftToolStripMenuItem.Click += new System.EventHandler(this.chooseShiftToolStripMenuItem_Click);
		this.txt_Year.EditControl = null;
		this.txt_Year.FieldName = null;
		this.txt_Year.IsSearchControl = false;
		this.txt_Year.Location = new System.Drawing.Point(334, 24);
		this.txt_Year.Name = "txt_Year";
		this.txt_Year.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_Year.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_Year.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_Year.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.txt_Year.SettingInVisible = false;
		this.txt_Year.SettingReadonly = false;
		this.txt_Year.Size = new System.Drawing.Size(83, 20);
		this.txt_Year.TabIndex = 2;
		this.txt_Year.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_Year_EditValueChanging);
		this.groupControl3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.groupControl3.Controls.Add(this.grd_Lines);
		this.groupControl3.Location = new System.Drawing.Point(1, 144);
		this.groupControl3.Name = "groupControl3";
		this.groupControl3.Size = new System.Drawing.Size(425, 251);
		this.groupControl3.TabIndex = 9;
		this.groupControl3.Text = "Work Scheduling";
		this.groupControl2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.groupControl2.Controls.Add(this.btnCancel);
		this.groupControl2.Controls.Add(this.labelLine5);
		this.groupControl2.Controls.Add(this.labelLine4);
		this.groupControl2.Controls.Add(this.labelLine3);
		this.groupControl2.Controls.Add(this.labelLine2);
		this.groupControl2.Controls.Add(this.labelLine1);
		this.groupControl2.Controls.Add(this.txt_Year);
		this.groupControl2.Controls.Add(this.groupControl3);
		this.groupControl2.Controls.Add(this.btn_Refresh);
		this.groupControl2.Controls.Add(this.btn_Save);
		this.groupControl2.Controls.Add(this.btn_Scheduling);
		this.groupControl2.Controls.Add(this.txt_Remarks);
		this.groupControl2.Controls.Add(this.txt_From);
		this.groupControl2.Controls.Add(this.txt_To);
		this.groupControl2.Controls.Add(this.txt_Name);
		this.groupControl2.Location = new System.Drawing.Point(243, 0);
		this.groupControl2.Name = "groupControl2";
		this.groupControl2.Size = new System.Drawing.Size(427, 433);
		this.groupControl2.TabIndex = 2;
		this.groupControl2.Text = "Details";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(348, 404);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(70, 20);
		this.btnCancel.TabIndex = 10;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "To Date";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(239, 118);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(95, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 0;
		this.labelLine5.Utext = "To Date";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "From Date";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(18, 117);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(117, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 0;
		this.labelLine4.Utext = "From Date";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Remarks";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(17, 95);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(117, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 0;
		this.labelLine3.Utext = "Remarks";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Year";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(296, 26);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(38, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 0;
		this.labelLine2.Utext = "Year";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Name";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(17, 26);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(117, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Name";
		this.btn_Refresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Refresh.EditControl = null;
		this.btn_Refresh.EditValue = "Refresh";
		this.btn_Refresh.FieldName = null;
		this.btn_Refresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Refresh.ImageOptions.Image");
		this.btn_Refresh.IsSearchControl = false;
		this.btn_Refresh.Location = new System.Drawing.Point(273, 404);
		this.btn_Refresh.Name = "btn_Refresh";
		this.btn_Refresh.ReadOnly = false;
		this.btn_Refresh.SettingInVisible = false;
		this.btn_Refresh.SettingReadonly = false;
		this.btn_Refresh.Size = new System.Drawing.Size(70, 20);
		this.btn_Refresh.TabIndex = 9;
		this.btn_Refresh.Text = "Refresh";
		this.btn_Refresh.Click += new System.EventHandler(this.btn_Refresh_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(197, 404);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(70, 20);
		this.btn_Save.TabIndex = 7;
		this.btn_Save.Text = "Save";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.gv_WorkScheduling.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_WorkScheduling.ColumnKey = null;
		this.gv_WorkScheduling.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gc_ID, this.gc_Name, this.gcYear });
		this.gv_WorkScheduling.DisableFormSetting = false;
		this.gv_WorkScheduling.DisplayException = false;
		this.gv_WorkScheduling.EditColumn = null;
		this.gv_WorkScheduling.FieldCurrency = null;
		this.gv_WorkScheduling.FieldRate = null;
		this.gv_WorkScheduling.FormId = "oWorkScheduling";
		this.gv_WorkScheduling.GridControl = this.grd_WorkScheduling;
		this.gv_WorkScheduling.GroupCount = 1;
		this.gv_WorkScheduling.IgnoreFilter = false;
		this.gv_WorkScheduling.IndexEditRow = -1;
		this.gv_WorkScheduling.IndicatorWidth = 30;
		this.gv_WorkScheduling.IsApzTable = false;
		this.gv_WorkScheduling.IsInsertNewRow = false;
		this.gv_WorkScheduling.IsLoadingData = false;
		this.gv_WorkScheduling.IsMappingContextAction = false;
		this.gv_WorkScheduling.ItemId = "APZ_OWSH";
		this.gv_WorkScheduling.LstColumnAllowBatch = null;
		this.gv_WorkScheduling.LstColumnNotAllowBatch = null;
		this.gv_WorkScheduling.LstColumnNotEditable = null;
		this.gv_WorkScheduling.LstColumNotSetting = null;
		this.gv_WorkScheduling.Name = "gv_WorkScheduling";
		this.gv_WorkScheduling.OptionsBehavior.ReadOnly = true;
		this.gv_WorkScheduling.OptionsView.ShowGroupPanel = false;
		this.gv_WorkScheduling.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gcYear, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_WorkScheduling.UseBatchMod = false;
		this.gv_WorkScheduling.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_WorkScheduling_CustomDrawRowIndicator);
		this.gv_WorkScheduling.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_WorkScheduling_FocusedRowChanged);
		this.gv_WorkScheduling.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_WorkScheduling_FocusedRowObjectChanged);
		this.gv_WorkScheduling.ColumnFilterChanged += new System.EventHandler(this.gv_WorkScheduling_ColumnFilterChanged);
		this.gc_ID.Caption = "ID";
		this.gc_ID.CurrencyDateField = null;
		this.gc_ID.CurrencyField = null;
		this.gc_ID.CurrencyFixed = null;
		this.gc_ID.CurrencyFixedRate = 0.0;
		this.gc_ID.CurrencyRateField = null;
		this.gc_ID.FieldName = "Id";
		this.gc_ID.Name = "gc_ID";
		this.gc_ID.PrimaryKey = false;
		this.gc_ID.UnitField = null;
		this.gc_Name.Caption = "Name";
		this.gc_Name.CurrencyDateField = null;
		this.gc_Name.CurrencyField = null;
		this.gc_Name.CurrencyFixed = null;
		this.gc_Name.CurrencyFixedRate = 0.0;
		this.gc_Name.CurrencyRateField = null;
		this.gc_Name.FieldName = "Name";
		this.gc_Name.Name = "gc_Name";
		this.gc_Name.OptionsColumn.AllowEdit = false;
		this.gc_Name.PrimaryKey = false;
		this.gc_Name.UnitField = null;
		this.gc_Name.Visible = true;
		this.gc_Name.VisibleIndex = 0;
		this.gcYear.Caption = "Year";
		this.gcYear.CurrencyDateField = null;
		this.gcYear.CurrencyField = null;
		this.gcYear.CurrencyFixed = null;
		this.gcYear.CurrencyFixedRate = 0.0;
		this.gcYear.CurrencyRateField = null;
		this.gcYear.FieldName = "Year";
		this.gcYear.Name = "gcYear";
		this.gcYear.PrimaryKey = false;
		this.gcYear.UnitField = null;
		this.gcYear.Visible = true;
		this.gcYear.VisibleIndex = 1;
		this.grd_WorkScheduling.AllowPasteExcel = false;
		this.grd_WorkScheduling.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.grd_WorkScheduling.ClipboardData = null;
		this.grd_WorkScheduling.ContextMenuStrip = this.contextMenuStrip;
		this.grd_WorkScheduling.Location = new System.Drawing.Point(2, 21);
		this.grd_WorkScheduling.MainView = this.gv_WorkScheduling;
		this.grd_WorkScheduling.Name = "grd_WorkScheduling";
		this.grd_WorkScheduling.Size = new System.Drawing.Size(235, 382);
		this.grd_WorkScheduling.TabIndex = 0;
		this.grd_WorkScheduling.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_WorkScheduling });
		this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.deleteToolStripMenuItem, this.addToolStripMenuItem });
		this.contextMenuStrip.Name = "contextMenuStrip";
		this.contextMenuStrip.Size = new System.Drawing.Size(108, 48);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.groupControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.groupControl1.Controls.Add(this.grd_WorkScheduling);
		this.groupControl1.Location = new System.Drawing.Point(0, 30);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(239, 404);
		this.groupControl1.TabIndex = 3;
		this.groupControl1.Text = "List of Work Scheduling";
		this.cl_ShiftCode.Caption = "Code";
		this.cl_ShiftCode.CurrencyDateField = null;
		this.cl_ShiftCode.CurrencyField = null;
		this.cl_ShiftCode.CurrencyFixed = null;
		this.cl_ShiftCode.CurrencyFixedRate = 0.0;
		this.cl_ShiftCode.CurrencyRateField = null;
		this.cl_ShiftCode.FieldName = "Code";
		this.cl_ShiftCode.Name = "cl_ShiftCode";
		this.cl_ShiftCode.PrimaryKey = false;
		this.cl_ShiftCode.UnitField = null;
		this.cl_ShiftCode.Visible = true;
		this.cl_ShiftCode.VisibleIndex = 0;
		this.lbBranch.EditControl = null;
		this.lbBranch.EditValue = "Branch";
		this.lbBranch.FieldName = null;
		this.lbBranch.HideWhenEmpty = false;
		this.lbBranch.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbBranch.Location = new System.Drawing.Point(13, 7);
		this.lbBranch.Name = "lbBranch";
		this.lbBranch.ReadOnly = false;
		this.lbBranch.SettingInVisible = false;
		this.lbBranch.SettingReadonly = false;
		this.lbBranch.ShowUnderLine = true;
		this.lbBranch.Size = new System.Drawing.Size(60, 20);
		this.lbBranch.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbBranch.TabIndex = 4;
		this.lbBranch.Utext = "Branch";
		this.txt_Branch.Location = new System.Drawing.Point(73, 5);
		this.txt_Branch.Name = "txt_Branch";
		this.txt_Branch.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_Branch.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_Branch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_Branch.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.txt_Branch.Properties.DisplayMember = "Name";
		this.txt_Branch.Properties.NullText = "";
		this.txt_Branch.Properties.ShowFooter = false;
		this.txt_Branch.Properties.ShowHeader = false;
		this.txt_Branch.Properties.ValueMember = "Value";
		this.txt_Branch.Size = new System.Drawing.Size(145, 20);
		this.txt_Branch.TabIndex = 5;
		this.txt_Branch.EditValueChanged += new System.EventHandler(this.txt_Branch_EditValueChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(670, 433);
		base.Controls.Add(this.lbBranch);
		base.Controls.Add(this.groupControl2);
		base.Controls.Add(this.groupControl1);
		base.Controls.Add(this.txt_Branch);
		base.FormId = "oWorkScheduling";
		base.IconOptions.ShowIcon = false;
		base.Name = "FrmWorkScheduling";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Work Scheduling";
		((System.ComponentModel.ISupportInitialize)this.txt_Remarks.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_From.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_From.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_To.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_To.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Lines).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rdate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rdate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rShift).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Lines).EndInit();
		this.contextMenuStripLine.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_Year.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl3).EndInit();
		this.groupControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl2).EndInit();
		this.groupControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_WorkScheduling).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_WorkScheduling).EndInit();
		this.contextMenuStrip.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_Branch.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
