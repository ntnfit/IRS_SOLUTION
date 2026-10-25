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
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
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

namespace Apzon.Forms.Financials;

public class frm_DistributeRule : XtraFormEx
{
	private string _Controller = "DistributeRule";

	private int _keyDimesion;

	private DataTable _dtGeneralCostCenter = new DataTable();

	private DataTable _dtOriginal = new DataTable();

	private List<string> _prcCodes = new List<string>();

	private List<string> _lstOriginalCode = new List<string>();

	private bool _editableRow;

	private bool _isFocusedRowChanged;

	private bool _isFocusedRowObjectChanged;

	private bool _isChangedDataWhenAdd;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private SimpleButtonEx bt_OK;

	private SimpleButtonEx bt_Cancel;

	private GridControlEx grd_OOCR;

	private GridViewEx gv_OOCR;

	private GridColumnEx gcOcrCode;

	private GridColumnEx gcOcrName;

	private GridColumnEx gcDimCode;

	private GridColumnEx gcOcrTotal;

	private GridColumnEx gcActive;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private GridColumnEx gcDirect;

	private GridColumnEx gcLocked;

	private GridColumnEx gcDataSource;

	private GridColumnEx gcUserSign;

	private GridColumnEx gcAbsEntry;

	private GridColumnEx gclogInstanc;

	private GridColumnEx gcUserSign2;

	private GridColumnEx gcupdateDate;

	private GridControlEx grd_OCR1;

	private GridViewEx gv_OCR1;

	private GridColumnEx gcOcrCode_line;

	private GridColumnEx gcPrcCode_line;

	private GridColumnEx gcPrcAmount_line;

	private GridColumnEx gcOcrTotal_line;

	private GridColumnEx gcDirect_line;

	private GridColumnEx gcUserSign_line;

	private GridColumnEx gcValidFrom_line;

	private GridColumnEx gcValidTo_line;

	private GridColumnEx gclogInstanc_line;

	private GridColumnEx gcUserSign2_line;

	private GridColumnEx gcupdateDate_line;

	private TextEditEx txt_Code;

	private LabelLine lb_Code;

	private TextEditEx txt_Name;

	private LabelLine lb_Description;

	private LabelLine lb_Dimension;

	private LookUpEdit luk_Dimension;

	private SimpleButtonEx bt_Search;

	private RepositoryItemLookUpEdit rep_luk_Dimesion;

	private SimpleButtonEx bt_Refresh;

	private ContextMenuStrip ctm_Doc;

	private ToolStripMenuItem removeToolStripMenuItem;

	private ContextMenuStrip ctm_Line;

	private ToolStripMenuItem removeToolStripMenuItem1;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem addNewToolStripMenuItem1;

	private RepositoryItemButtonEdit rep_luk_OcrCode_line;

	private GridColumnEx gcOcrName_line;

	private GridColumnEx gcValidFrom_Doc;

	private GridColumnEx gcValidTo_Doc;

	private RepositoryItemDateEdit repositoryItemDateEdit1;

	private RepositoryItemDateEdit repositoryItemDateEdit4;

	private Label lbl_Centr_z;

	private LabelLine lbl_Centr_z_Name;

	private Label label2;

	private Label lb_TotalLine;

	private Label lb_GeneralCenter;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			bt_Cancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			bt_OK.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			bt_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			bt_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public async Task REFRESH()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update)
			{
				bool flag = true;
				if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					flag = false;
				}
				if (flag)
				{
					FormMode = Enums.FormViewModeType.Add;
					gcOcrCode.OptionsColumn.ReadOnly = false;
					lb_TotalLine.Text = string.Empty;
					lb_GeneralCenter.Text = string.Empty;
					grd_OOCR.DataSource = null;
					grd_OOCR.RefreshDataSource();
					grd_OCR1.DataSource = null;
					grd_OCR1.RefreshDataSource();
					AddNewRow();
					AddNewRowLine();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_DistributeRule()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_DistributeRule"))
		{
			InitializeComponent();
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async Task frm_DistributeRule_Load(object sender, EventArgs e)
	{
		await RefreshData();
	}

	private void gv_OOCR_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		_isFocusedRowChanged = true;
		try
		{
			if (_isFocusedRowObjectChanged)
			{
				_isFocusedRowChanged = false;
				return;
			}
			WinCommons._OpenProcessing();
			GetDataLine();
			CalculatorTableTotal();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		_isFocusedRowChanged = false;
	}

	private void lookUpEdit_Dimension_EditValueChanged(object sender, EventArgs e)
	{
		int keyDimesion = Function.ParseInt(luk_Dimension.EditValue);
		_keyDimesion = keyDimesion;
	}

	private void gv_OCR1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_OOCR_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_OCR1_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "PrcAmount")
			{
				CalculatorTableTotal();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_OOCR_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "OcrCode" && FormMode == Enums.FormViewModeType.Update)
			{
				string code = Function.ToString(gv_OOCR.GetFocusedRowCellValue("OcrCode"));
				if (grd_OOCR.DataSource is DataTable source)
				{
					EnumerableRowCollection<DataRow> enumerableRowCollection = from x in source.AsEnumerable()
						where x.Field<string>("OcrCode").ToUpper() == code.ToUpper()
						select x;
					if (enumerableRowCollection != null && enumerableRowCollection.Count() > 1)
					{
						WinCommons.Error(Enums.MessageCode.DuplicateDistributeRuleCode, "");
						gv_OOCR.FocusedColumn = gcOcrCode;
						return;
					}
				}
			}
			if (e.Column.FieldName == "OcrTotal")
			{
				CalculatorGeneralCenter();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_OOCR_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			UpdateButtonOKText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_OCR1_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			UpdateButtonOKText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_OOCR_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		_isFocusedRowObjectChanged = true;
		try
		{
			if (_isFocusedRowChanged)
			{
				_isFocusedRowObjectChanged = false;
				return;
			}
			WinCommons._OpenProcessing();
			GetDataLine();
			CalculatorTableTotal();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		_isFocusedRowObjectChanged = false;
	}

	private void gv_OOCR_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_OOCR.GetFocusedRowCellValue("OcrCode"));
			if (gv_OOCR.FocusedColumn.FieldName == "OcrCode")
			{
				if (IsGeneralCenter(text) || (_lstOriginalCode.Contains(text) && !_editableRow))
				{
					e.Cancel = true;
				}
			}
			else if (IsGeneralCenter(text) || !_editableRow)
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

	private void gv_OCR1_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			string prcCode = Function.ToString(gv_OOCR.GetFocusedRowCellValue("OcrCode"));
			if (IsGeneralCenter(prcCode) || gv_OCR1.FocusedColumn.FieldName == "PrcName")
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

	private void gv_OCR1_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			string prcCode = Function.ToString(gv_OOCR.GetFocusedRowCellValue("OcrCode"));
			if (IsGeneralCenter(prcCode) || e.Column.FieldName == "PrcName")
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_OOCR_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			string prcCode = Function.ToString(gv_OOCR.GetRowCellValue(e.RowHandle, "OcrCode"));
			if (IsGeneralCenter(prcCode) || e.Column.FieldName == "OcrCode")
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_OOCR_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					e.Allow = false;
					return;
				}
				grd_OOCR.DataSource = _dtOriginal.Copy();
				grd_OOCR.RefreshDataSource();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rIB_OcrCode_line_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index != 0)
			{
				return;
			}
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListOfCostCenterByDimCode, false, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.DataInt1 = Function.ParseInt(gv_OOCR.GetFocusedRowCellValue("DimCode"));
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem == null)
			{
				return;
			}
			DataTable dataTable = grd_OCR1.DataSource as DataTable;
			DataRow dataRow = dataTable.Rows[gv_OCR1.GetFocusedDataSourceRowIndex()];
			if (!ExistPrcCodeInGrid(Function.ToString(frm_CFL2.ChooseItem["PrcCode"])))
			{
				FormMode = Enums.FormViewModeType.Update;
				dataRow["PrcCode"] = frm_CFL2.ChooseItem["PrcCode"];
				dataRow["PrcName"] = frm_CFL2.ChooseItem["PrcName"];
				double num = Function.ParseDouble(gv_OOCR.GetFocusedRowCellValue("OcrTotal"));
				double num2 = Function.ParseDouble(dataTable.Sum("PrcAmount")) - Function.ParseDouble(gv_OOCR.GetFocusedRowCellValue("PrcAmount"));
				if (num > num2)
				{
					dataRow["PrcAmount"] = num - num2;
				}
				else
				{
					dataRow["PrcAmount"] = 0;
				}
				dataRow["OcrCode"] = Function.ToString(gv_OOCR.GetFocusedRowCellValue("OcrCode"));
				DateTime? dateTime = Function.ParseDateTime(gv_OOCR.GetFocusedRowCellValue("ValidFrom"));
				if (dateTime.HasValue)
				{
					_ = dateTime.Value;
					dataRow["ValidTo"] = dateTime.Value;
				}
				else
				{
					dataRow["ValidFrom"] = DBNull.Value;
				}
				DateTime? dateTime2 = Function.ParseDateTime(gv_OOCR.GetFocusedRowCellValue("ValidTo"));
				if (dateTime2.HasValue)
				{
					_ = dateTime2.Value;
					dataRow["ValidTo"] = dateTime2.Value;
				}
				else
				{
					dataRow["ValidTo"] = DBNull.Value;
				}
				dataRow["updateDate"] = DBNull.Value;
				dataRow["OcrTotal"] = Function.ParseDouble(gv_OOCR.GetFocusedRowCellValue("OcrTotal"));
				dataTable.AcceptChanges();
				grd_OCR1.DataSource = dataTable;
				grd_OCR1.RefreshDataSource();
				gv_OCR1.CloseEditor();
				AddNewRowLine();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.PrcCodeExistsInList, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void bt_Search_Click(object sender, EventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			string value = txt_Code.Text;
			string value2 = txt_Name.Text;
			int keyDimesion = _keyDimesion;
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("OcrCode", typeof(string));
			dataTable.Columns.Add("OcrName", typeof(string));
			dataTable.Columns.Add("DimCode", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			dataRow["OcrCode"] = value;
			dataRow["OcrName"] = value2;
			dataRow["DimCode"] = keyDimesion;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			using HttpClientHelper<DataTable> _hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, _Controller);
			DataTable dataTable2 = await _hp.PostAsync("Search", dataTable);
			if (dataTable2 != null && dataTable2.Rows.Count > 0)
			{
				grd_OOCR.DataSource = dataTable2;
				grd_OOCR.RefreshDataSource();
				GetDataLine();
				_dtOriginal = dataTable2.Copy();
				return;
			}
			DataTable defaultTable = GetDefaultTable();
			if (defaultTable != null)
			{
				grd_OOCR.DataSource = defaultTable;
				grd_OOCR.RefreshDataSource();
			}
			_dtOriginal = defaultTable.Copy();
			DataTable defaultTableLine = GetDefaultTableLine();
			if (defaultTableLine != null)
			{
				grd_OCR1.DataSource = defaultTableLine;
				grd_OCR1.RefreshDataSource();
			}
			WinCommons.Warning(Enums.MessageCode.NoMatchingRecordFound, "");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void bt_Refresh_Click(object sender, EventArgs e)
	{
		try
		{
			bool flag = true;
			if (_isChangedDataWhenAdd && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				flag = false;
			}
			if (flag)
			{
				await RefreshData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bt_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void bt_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (IsValidData() && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await Save();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool IsValidData()
	{
		bool result = true;
		try
		{
			DataTable dataTable = grd_OOCR.DataSource as DataTable;
			DataTable dataTable2 = grd_OCR1.DataSource as DataTable;
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoDistributeRuleDataToSave, "");
				result = false;
			}
			if (dataTable2 == null || dataTable2.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoDistributeRuleLineToSave, "");
				result = false;
			}
			if (Function.ParseDouble(gv_OOCR.GetFocusedRowCellValue("OcrTotal")) < Function.ParseDouble(dataTable2.Sum("PrcAmount")))
			{
				WinCommons.Error(Enums.MessageCode.EnterValidAmountInTotal, "");
				result = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			result = false;
		}
		return result;
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			return;
		}
		try
		{
			string text = Function.ToString(gv_OOCR.GetFocusedRowCellValue("OcrCode"));
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, _Controller);
			string text2 = httpClientHelper.Post("DeleteDistributeRule", text);
			if (string.IsNullOrEmpty(text2))
			{
				if (grd_OOCR.DataSource is DataTable dataTable)
				{
					dataTable.Rows.RemoveAt(gv_OOCR.FocusedRowHandle);
					dataTable.AcceptChanges();
					grd_OOCR.DataSource = dataTable;
				}
				grd_OOCR.RefreshDataSource();
				WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void removeToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
			string value = Function.ToString(gv_OCR1.GetFocusedRowCellValue("OcrCode"));
			string value2 = Function.ToString(gv_OCR1.GetFocusedRowCellValue("PrcCode"));
			if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value2))
			{
				if (grd_OCR1.DataSource is DataTable dataTable)
				{
					dataTable.Rows.RemoveAt(gv_OCR1.FocusedRowHandle);
					dataTable.AcceptChanges();
					grd_OCR1.DataSource = dataTable;
					CalculatorTableTotal();
				}
				grd_OCR1.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		REFRESH();
	}

	private void addNewToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewRowLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Save()
	{
		_ = 1;
		try
		{
			DataTable obj = grd_OOCR.DataSource as DataTable;
			DataRow focusedDataRow = gv_OOCR.GetFocusedDataRow();
			DataTable dataTable = obj.Clone();
			dataTable.ImportRow(focusedDataRow);
			dataTable.AcceptChanges();
			DataTable dataTable2 = (grd_OCR1.DataSource as DataTable).Copy();
			foreach (DataRow row in dataTable2.Rows)
			{
				row["ValidFrom"] = focusedDataRow["ValidFrom"];
				row["ValidTo"] = focusedDataRow["ValidTo"];
				row["Direct"] = focusedDataRow["Direct"];
			}
			dataTable2.Columns.Remove("PrcName");
			dataTable2.AcceptChanges();
			dataTable.Columns.Remove("ValidFrom");
			dataTable.Columns.Remove("ValidTo");
			dataTable.AcceptChanges();
			for (int num = dataTable2.Rows.Count - 1; num >= 0; num--)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[num]["PrcCode"])))
				{
					dataTable2.Rows.Remove(dataTable2.Rows[num]);
					dataTable2.AcceptChanges();
				}
			}
			if (dataTable2.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoDistributeLineData, "");
				return;
			}
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable.Copy());
			dataSet.Tables.Add(dataTable2.Copy());
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, _Controller);
			string text = await helper.PostAsync("Save", dataSet);
			if (string.IsNullOrEmpty(text))
			{
				await RefreshData();
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task RefreshData()
	{
		try
		{
			_isChangedDataWhenAdd = false;
			gcOcrCode.OptionsColumn.ReadOnly = true;
			LoadCombo();
			LoadPrcCode();
			bt_Search_Click(null, null);
			txt_Code.Text = string.Empty;
			txt_Name.Text = string.Empty;
			CalculatorTableTotal();
			CalculatorGeneralCenter();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void CalculatorTableTotal()
	{
		try
		{
			DataTable dataTable = grd_OCR1.DataSource as DataTable;
			DataTable dataTable2 = grd_OOCR.DataSource as DataTable;
			if (dataTable == null || dataTable2 == null)
			{
				return;
			}
			double num = Function.ParseDouble(gv_OOCR.GetFocusedRowCellValue("OcrTotal"));
			double num2 = 0.0;
			if (dataTable.IsNotNull())
			{
				for (int i = 0; i < dataTable.Rows.Count - 1; i++)
				{
					num2 = Function.ParseDouble(dataTable.Sum("PrcAmount"));
					lb_TotalLine.Text = Function.ToString(num2);
				}
			}
			else
			{
				lb_TotalLine.Text = Function.ToString(num2);
			}
			if (num2 > num)
			{
				WinCommons.Error(Enums.MessageCode.EnterValidAmountInTotal, "");
			}
			else
			{
				CalculatorGeneralCenter();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadCombo()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, _Controller);
			DataSet dataSet = httpClientHelper.Get("LoadComBoBox");
			if (dataSet == null)
			{
				return;
			}
			if (dataSet.Tables.Count > 0 && dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
			{
				DataTable dataTable = dataSet.Tables[0].Clone();
				foreach (DataRow row3 in dataSet.Tables[0].Rows)
				{
					dataTable.ImportRow(row3);
				}
				dataTable.AcceptChanges();
				luk_Dimension.Properties.DataSource = dataTable;
				rep_luk_Dimesion.DataSource = dataTable;
			}
			if (dataSet.Tables.Count <= 0 || dataSet.Tables[1] == null || dataSet.Tables[1].Rows.Count <= 0)
			{
				return;
			}
			_dtGeneralCostCenter = dataSet.Tables[1].Clone();
			foreach (DataRow row4 in dataSet.Tables[1].Rows)
			{
				_dtGeneralCostCenter.ImportRow(row4);
			}
			_dtGeneralCostCenter.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData()
	{
		try
		{
			LoadPrcCode();
			using HttpClientHelper<DataTable> _hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, _Controller);
			DataTable dataTable = await _hp.GetAsync("GetDataOOCR");
			_lstOriginalCode.Clear();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				grd_OOCR.DataSource = dataTable;
				grd_OOCR.RefreshDataSource();
				gv_OOCR.RefreshData();
				GetDataLine();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					_lstOriginalCode.Add(Function.ToString(dataTable.Rows[i]["OcrCode"]));
				}
				_dtOriginal = dataTable.Copy();
			}
			else
			{
				_dtOriginal = GetDefaultTable();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetDataLine()
	{
		_editableRow = false;
		try
		{
			string key = Function.ToString(gv_OOCR.GetFocusedRowCellValue("OcrCode"));
			using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _Controller))
			{
				if (httpClientHelper.GetBool("GetIsHasLinkedObject", new { key }))
				{
					_editableRow = false;
				}
				else
				{
					_editableRow = true;
				}
			}
			using HttpClientHelper<DataTable> httpClientHelper2 = new HttpClientHelper<DataTable>(Configuration.ApiServer, _Controller);
			DataTable dataTable = httpClientHelper2.Get("GetDataOCR1", new { key });
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				grd_OCR1.DataSource = dataTable;
				if (!_prcCodes.Any((string t) => t.ToUpper() == key.ToUpper()))
				{
					AddNewRowLine();
				}
				if (_dtGeneralCostCenter != null)
				{
					int dimCode = Function.ParseInt(gv_OOCR.GetFocusedRowCellValue("DimCode"));
					DataRow dataRow = _dtGeneralCostCenter.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["DimCode"]) == dimCode);
					if (dataRow != null)
					{
						lbl_Centr_z.Text = Function.ToString(dataRow["PrcCode"]);
						lbl_Centr_z_Name.Utext = Function.ToString(dataRow["PrcName"]);
					}
				}
			}
			else
			{
				DataTable defaultTableLine = GetDefaultTableLine();
				if (defaultTableLine != null)
				{
					grd_OCR1.DataSource = defaultTableLine;
					grd_OCR1.RefreshDataSource();
				}
			}
			CalculatorGeneralCenter();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private int GetUserSignByUserCode_tableOUSR()
	{
		string userCodeLogin = GlobalData.UserCodeLogin;
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Users");
			DataTable dataTable = httpClientHelper.Get("GetUserSign", new
			{
				nameLogin = userCodeLogin
			});
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["UserSign"].ToString());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return -1;
	}

	private void AddNewRow()
	{
		try
		{
			DataTable dataTable = grd_OOCR.DataSource as DataTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = GetDefaultTable();
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["OcrCode"] = string.Empty;
			dataRow["OcrName"] = string.Empty;
			dataRow["OcrTotal"] = DBNull.Value;
			dataRow["Direct"] = "Y";
			dataRow["Locked"] = "N";
			dataRow["DataSource"] = "I";
			object value = (dataRow["UserSign"] = GetUserSignByUserCode_tableOUSR());
			dataRow["UserSign2"] = value;
			dataRow["DimCode"] = 1;
			dataRow["AbsEntry"] = DBNull.Value;
			dataRow["Active"] = "Y";
			dataRow["logInstanc"] = DBNull.Value;
			dataRow["updateDate"] = DBNull.Value;
			dataRow["ValidFrom"] = DateTime.Today.Date;
			dataRow["ValidTo"] = DBNull.Value;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_OOCR.DataSource = dataTable;
			grd_OOCR.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewRowLine()
	{
		try
		{
			DataTable dataTable = grd_OCR1.DataSource as DataTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = GetDefaultTableLine();
			}
			if (!dataTable.AsEnumerable().Any((DataRow t) => string.IsNullOrWhiteSpace(Function.ToString(t["PrcCode"]))))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["OcrCode"] = Function.ToString(gv_OOCR.GetFocusedRowCellValue("OcrCode"));
				dataRow["PrcCode"] = string.Empty;
				dataRow["PrcAmount"] = DBNull.Value;
				dataRow["OcrTotal"] = Function.ParseDouble(gv_OOCR.GetFocusedRowCellValue("OcrTotal"));
				dataRow["Direct"] = "Y";
				object value = (dataRow["UserSign"] = GetUserSignByUserCode_tableOUSR());
				dataRow["UserSign2"] = value;
				DateTime? dateTime = Function.ParseDateTime(gv_OOCR.GetFocusedRowCellValue("ValidFrom"));
				if (dateTime.HasValue)
				{
					_ = dateTime.Value;
					dataRow["ValidFrom"] = dateTime.Value;
				}
				else
				{
					dataRow["ValidFrom"] = DBNull.Value;
				}
				DateTime? dateTime2 = Function.ParseDateTime(gv_OOCR.GetFocusedRowCellValue("ValidTo"));
				if (dateTime2.HasValue)
				{
					_ = dateTime2.Value;
					dataRow["ValidTo"] = dateTime2.Value;
				}
				else
				{
					dataRow["ValidTo"] = DBNull.Value;
				}
				dataRow["logInstanc"] = DBNull.Value;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_OCR1.DataSource = dataTable;
				grd_OCR1.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private bool ExistPrcCodeInGrid(string prcCode, int index = -1)
	{
		bool result = false;
		try
		{
			if (string.IsNullOrWhiteSpace(prcCode))
			{
				result = true;
			}
			else if (grd_OCR1.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				return dataTable.AsEnumerable().Any((DataRow t) => Function.ToString(t["PrcCode"]).ToUpper() == prcCode.ToUpper());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
	}

	private void CalculatorGeneralCenter()
	{
		try
		{
			DataTable dataTable = grd_OCR1.DataSource as DataTable;
			double num = Function.ParseDouble(gv_OOCR.GetFocusedRowCellValue("OcrTotal"));
			double num2 = Function.ParseDouble(0);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				num2 = Function.ParseDouble(dataTable.Sum("PrcAmount"));
			}
			double num3 = num - num2;
			lb_GeneralCenter.Text = Function.ToString(num3);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable GetDefaultTable()
	{
		try
		{
			DataTable tableStructure = WinFuntions.GetTableStructure("OOCR");
			tableStructure.Columns.Add("ValidFrom", typeof(DateTime));
			tableStructure.Columns.Add("ValidTo", typeof(DateTime));
			tableStructure.AcceptChanges();
			return tableStructure;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private DataTable GetDefaultTableLine()
	{
		try
		{
			DataTable tableStructure = WinFuntions.GetTableStructure("OCR1");
			tableStructure.Columns.Add("PrcName", typeof(string));
			tableStructure.AcceptChanges();
			return tableStructure;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private bool IsGeneralCenter(string prcCode)
	{
		try
		{
			if (_prcCodes.Any((string t) => t.ToUpper() == prcCode.ToUpper()))
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return false;
	}

	private void LoadPrcCode()
	{
		try
		{
			_prcCodes.Clear();
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _Controller);
			DataTable dataTable = httpClientHelper.Get("LoadPrcCode");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					_prcCodes.Add(Function.ToString(dataTable.Rows[i][0]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void UpdateButtonOKText()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			else if (FormMode == Enums.FormViewModeType.Add)
			{
				_isChangedDataWhenAdd = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Financials.frm_DistributeRule));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_OOCR = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_OOCR = new Apzon.Controls.Grid.GridViewEx();
		this.gcOcrCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOcrName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDimCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_Dimesion = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcOcrTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.gcActive = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcDirect = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLocked = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDataSource = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAbsEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.gclogInstanc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUserSign2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcupdateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcValidFrom_Doc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcValidTo_Doc = new Apzon.Controls.Grid.GridColumnEx();
		this.grd_OCR1 = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Line = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_OCR1 = new Apzon.Controls.Grid.GridViewEx();
		this.gcOcrCode_line = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPrcCode_line = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_OcrCode_line = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcPrcAmount_line = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOcrTotal_line = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDirect_line = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUserSign_line = new Apzon.Controls.Grid.GridColumnEx();
		this.gcValidFrom_line = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcValidTo_line = new Apzon.Controls.Grid.GridColumnEx();
		this.gclogInstanc_line = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUserSign2_line = new Apzon.Controls.Grid.GridColumnEx();
		this.gcupdateDate_line = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOcrName_line = new Apzon.Controls.Grid.GridColumnEx();
		this.bt_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_Code = new Apzon.Controls.Editor.TextEditEx();
		this.lb_Code = new Apzon.Controls.Controls.LabelLine();
		this.txt_Name = new Apzon.Controls.Editor.TextEditEx();
		this.lb_Description = new Apzon.Controls.Controls.LabelLine();
		this.lb_Dimension = new Apzon.Controls.Controls.LabelLine();
		this.luk_Dimension = new DevExpress.XtraEditors.LookUpEdit();
		this.bt_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_Refresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbl_Centr_z = new System.Windows.Forms.Label();
		this.lbl_Centr_z_Name = new Apzon.Controls.Controls.LabelLine();
		this.label2 = new System.Windows.Forms.Label();
		this.lb_TotalLine = new System.Windows.Forms.Label();
		this.lb_GeneralCenter = new System.Windows.Forms.Label();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_OOCR).BeginInit();
		this.ctm_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_OOCR).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_Dimesion).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_OCR1).BeginInit();
		this.ctm_Line.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_OCR1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_OcrCode_line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit4.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Dimension.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 97);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_OOCR);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_OCR1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(910, 342);
		this.splitContainerControl1.SplitterPosition = 154;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_OOCR.AllowPasteExcel = true;
		this.grd_OOCR.ClipboardData = null;
		this.grd_OOCR.ContextMenuStrip = this.ctm_Doc;
		this.grd_OOCR.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_OOCR.Location = new System.Drawing.Point(0, 0);
		this.grd_OOCR.MainView = this.gv_OOCR;
		this.grd_OOCR.Name = "grd_OOCR";
		this.grd_OOCR.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryItemCheckEdit1, this.rep_luk_Dimesion, this.repositoryItemDateEdit1 });
		this.grd_OOCR.Size = new System.Drawing.Size(910, 154);
		this.grd_OOCR.TabIndex = 0;
		this.grd_OOCR.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_OOCR });
		this.ctm_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.removeToolStripMenuItem });
		this.ctm_Doc.Name = "contextMenuStrip1";
		this.ctm_Doc.Size = new System.Drawing.Size(124, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_OOCR.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_OOCR.ColumnKey = null;
		this.gv_OOCR.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[15]
		{
			this.gcOcrCode, this.gcOcrName, this.gcDimCode, this.gcOcrTotal, this.gcActive, this.gcDirect, this.gcLocked, this.gcDataSource, this.gcUserSign, this.gcAbsEntry,
			this.gclogInstanc, this.gcUserSign2, this.gcupdateDate, this.gcValidFrom_Doc, this.gcValidTo_Doc
		});
		this.gv_OOCR.DisableFormSetting = false;
		this.gv_OOCR.DisplayException = false;
		this.gv_OOCR.EditColumn = null;
		this.gv_OOCR.FieldCurrency = null;
		this.gv_OOCR.FieldRate = null;
		this.gv_OOCR.FormId = "oDistributionRuleSetup";
		this.gv_OOCR.GridControl = this.grd_OOCR;
		this.gv_OOCR.IgnoreFilter = false;
		this.gv_OOCR.IndexEditRow = -1;
		this.gv_OOCR.IndicatorWidth = 35;
		this.gv_OOCR.IsApzTable = false;
		this.gv_OOCR.IsInsertNewRow = false;
		this.gv_OOCR.IsLoadingData = false;
		this.gv_OOCR.IsMappingContextAction = false;
		this.gv_OOCR.ItemId = "OOCR";
		this.gv_OOCR.LstColumnAllowBatch = null;
		this.gv_OOCR.LstColumnNotAllowBatch = null;
		this.gv_OOCR.LstColumnNotEditable = null;
		this.gv_OOCR.LstColumNotSetting = null;
		this.gv_OOCR.Name = "gv_OOCR";
		this.gv_OOCR.OptionsView.ShowGroupPanel = false;
		this.gv_OOCR.UseBatchMod = false;
		this.gv_OOCR.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_OOCR_CustomDrawRowIndicator);
		this.gv_OOCR.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_OOCR_RowCellStyle);
		this.gv_OOCR.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_OOCR_ShowingEditor);
		this.gv_OOCR.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_OOCR_FocusedRowChanged);
		this.gv_OOCR.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_OOCR_FocusedRowObjectChanged);
		this.gv_OOCR.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_OOCR_CellValueChanged);
		this.gv_OOCR.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_OOCR_CellValueChanging);
		this.gv_OOCR.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_OOCR_BeforeLeaveRow);
		this.gcOcrCode.Caption = "Code";
		this.gcOcrCode.CurrencyDateField = null;
		this.gcOcrCode.CurrencyField = null;
		this.gcOcrCode.CurrencyFixed = null;
		this.gcOcrCode.CurrencyFixedRate = 0.0;
		this.gcOcrCode.CurrencyRateField = null;
		this.gcOcrCode.FieldName = "OcrCode";
		this.gcOcrCode.Name = "gcOcrCode";
		this.gcOcrCode.PrimaryKey = true;
		this.gcOcrCode.UnitField = null;
		this.gcOcrCode.Visible = true;
		this.gcOcrCode.VisibleIndex = 0;
		this.gcOcrCode.Width = 68;
		this.gcOcrName.Caption = "Description";
		this.gcOcrName.CurrencyDateField = null;
		this.gcOcrName.CurrencyField = null;
		this.gcOcrName.CurrencyFixed = null;
		this.gcOcrName.CurrencyFixedRate = 0.0;
		this.gcOcrName.CurrencyRateField = null;
		this.gcOcrName.FieldName = "OcrName";
		this.gcOcrName.Name = "gcOcrName";
		this.gcOcrName.PrimaryKey = false;
		this.gcOcrName.UnitField = null;
		this.gcOcrName.Visible = true;
		this.gcOcrName.VisibleIndex = 1;
		this.gcOcrName.Width = 68;
		this.gcDimCode.Caption = "Dimension";
		this.gcDimCode.ColumnEdit = this.rep_luk_Dimesion;
		this.gcDimCode.CurrencyDateField = null;
		this.gcDimCode.CurrencyField = null;
		this.gcDimCode.CurrencyFixed = null;
		this.gcDimCode.CurrencyFixedRate = 0.0;
		this.gcDimCode.CurrencyRateField = null;
		this.gcDimCode.FieldName = "DimCode";
		this.gcDimCode.Name = "gcDimCode";
		this.gcDimCode.PrimaryKey = false;
		this.gcDimCode.UnitField = null;
		this.gcDimCode.Visible = true;
		this.gcDimCode.VisibleIndex = 2;
		this.gcDimCode.Width = 68;
		this.rep_luk_Dimesion.AutoHeight = false;
		this.rep_luk_Dimesion.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_Dimesion.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DimCode", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DimDesc", "Name")
		});
		this.rep_luk_Dimesion.DisplayMember = "DimDesc";
		this.rep_luk_Dimesion.Name = "rep_luk_Dimesion";
		this.rep_luk_Dimesion.NullText = "";
		this.rep_luk_Dimesion.ValueMember = "DimCode";
		this.gcOcrTotal.Caption = "Total";
		this.gcOcrTotal.CurrencyDateField = null;
		this.gcOcrTotal.CurrencyField = null;
		this.gcOcrTotal.CurrencyFixed = null;
		this.gcOcrTotal.CurrencyFixedRate = 0.0;
		this.gcOcrTotal.CurrencyRateField = null;
		this.gcOcrTotal.FieldName = "OcrTotal";
		this.gcOcrTotal.Name = "gcOcrTotal";
		this.gcOcrTotal.PrimaryKey = false;
		this.gcOcrTotal.UnitField = null;
		this.gcOcrTotal.Visible = true;
		this.gcOcrTotal.VisibleIndex = 3;
		this.gcOcrTotal.Width = 68;
		this.gcActive.Caption = "Active";
		this.gcActive.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gcActive.CurrencyDateField = null;
		this.gcActive.CurrencyField = null;
		this.gcActive.CurrencyFixed = null;
		this.gcActive.CurrencyFixedRate = 0.0;
		this.gcActive.CurrencyRateField = null;
		this.gcActive.FieldName = "Active";
		this.gcActive.Name = "gcActive";
		this.gcActive.PrimaryKey = false;
		this.gcActive.UnitField = null;
		this.gcActive.Visible = true;
		this.gcActive.VisibleIndex = 4;
		this.gcActive.Width = 68;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.gcDirect.Caption = "Direct Allocation";
		this.gcDirect.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gcDirect.CurrencyDateField = null;
		this.gcDirect.CurrencyField = null;
		this.gcDirect.CurrencyFixed = null;
		this.gcDirect.CurrencyFixedRate = 0.0;
		this.gcDirect.CurrencyRateField = null;
		this.gcDirect.FieldName = "Direct";
		this.gcDirect.Name = "gcDirect";
		this.gcDirect.PrimaryKey = false;
		this.gcDirect.UnitField = null;
		this.gcDirect.Visible = true;
		this.gcDirect.VisibleIndex = 5;
		this.gcDirect.Width = 96;
		this.gcLocked.Caption = "Locked";
		this.gcLocked.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gcLocked.CurrencyDateField = null;
		this.gcLocked.CurrencyField = null;
		this.gcLocked.CurrencyFixed = null;
		this.gcLocked.CurrencyFixedRate = 0.0;
		this.gcLocked.CurrencyRateField = null;
		this.gcLocked.FieldName = "Locked";
		this.gcLocked.Name = "gcLocked";
		this.gcLocked.PrimaryKey = false;
		this.gcLocked.UnitField = null;
		this.gcLocked.Width = 64;
		this.gcDataSource.Caption = "DataSource";
		this.gcDataSource.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gcDataSource.CurrencyDateField = null;
		this.gcDataSource.CurrencyField = null;
		this.gcDataSource.CurrencyFixed = null;
		this.gcDataSource.CurrencyFixedRate = 0.0;
		this.gcDataSource.CurrencyRateField = null;
		this.gcDataSource.FieldName = "DataSource";
		this.gcDataSource.Name = "gcDataSource";
		this.gcDataSource.PrimaryKey = false;
		this.gcDataSource.UnitField = null;
		this.gcDataSource.Width = 64;
		this.gcUserSign.Caption = "UserSign";
		this.gcUserSign.CurrencyDateField = null;
		this.gcUserSign.CurrencyField = null;
		this.gcUserSign.CurrencyFixed = null;
		this.gcUserSign.CurrencyFixedRate = 0.0;
		this.gcUserSign.CurrencyRateField = null;
		this.gcUserSign.FieldName = "UserSign";
		this.gcUserSign.Name = "gcUserSign";
		this.gcUserSign.PrimaryKey = false;
		this.gcUserSign.UnitField = null;
		this.gcUserSign.Width = 64;
		this.gcAbsEntry.Caption = "AbsEntry";
		this.gcAbsEntry.CurrencyDateField = null;
		this.gcAbsEntry.CurrencyField = null;
		this.gcAbsEntry.CurrencyFixed = null;
		this.gcAbsEntry.CurrencyFixedRate = 0.0;
		this.gcAbsEntry.CurrencyRateField = null;
		this.gcAbsEntry.FieldName = "AbsEntry";
		this.gcAbsEntry.Name = "gcAbsEntry";
		this.gcAbsEntry.PrimaryKey = false;
		this.gcAbsEntry.UnitField = null;
		this.gcAbsEntry.Width = 64;
		this.gclogInstanc.Caption = "logInstanc";
		this.gclogInstanc.CurrencyDateField = null;
		this.gclogInstanc.CurrencyField = null;
		this.gclogInstanc.CurrencyFixed = null;
		this.gclogInstanc.CurrencyFixedRate = 0.0;
		this.gclogInstanc.CurrencyRateField = null;
		this.gclogInstanc.FieldName = "logInstanc";
		this.gclogInstanc.Name = "gclogInstanc";
		this.gclogInstanc.PrimaryKey = false;
		this.gclogInstanc.UnitField = null;
		this.gclogInstanc.Width = 64;
		this.gcUserSign2.Caption = "UserSign2";
		this.gcUserSign2.CurrencyDateField = null;
		this.gcUserSign2.CurrencyField = null;
		this.gcUserSign2.CurrencyFixed = null;
		this.gcUserSign2.CurrencyFixedRate = 0.0;
		this.gcUserSign2.CurrencyRateField = null;
		this.gcUserSign2.FieldName = "UserSign2";
		this.gcUserSign2.Name = "gcUserSign2";
		this.gcUserSign2.PrimaryKey = false;
		this.gcUserSign2.UnitField = null;
		this.gcUserSign2.Width = 64;
		this.gcupdateDate.Caption = "updateDate";
		this.gcupdateDate.ColumnEdit = this.repositoryItemDateEdit1;
		this.gcupdateDate.CurrencyDateField = null;
		this.gcupdateDate.CurrencyField = null;
		this.gcupdateDate.CurrencyFixed = null;
		this.gcupdateDate.CurrencyFixedRate = 0.0;
		this.gcupdateDate.CurrencyRateField = null;
		this.gcupdateDate.FieldName = "updateDate";
		this.gcupdateDate.Name = "gcupdateDate";
		this.gcupdateDate.PrimaryKey = false;
		this.gcupdateDate.UnitField = null;
		this.gcupdateDate.Width = 72;
		this.repositoryItemDateEdit1.AutoHeight = false;
		this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
		this.gcValidFrom_Doc.Caption = "ValidFrom";
		this.gcValidFrom_Doc.ColumnEdit = this.repositoryItemDateEdit1;
		this.gcValidFrom_Doc.CurrencyDateField = null;
		this.gcValidFrom_Doc.CurrencyField = null;
		this.gcValidFrom_Doc.CurrencyFixed = null;
		this.gcValidFrom_Doc.CurrencyFixedRate = 0.0;
		this.gcValidFrom_Doc.CurrencyRateField = null;
		this.gcValidFrom_Doc.FieldName = "ValidFrom";
		this.gcValidFrom_Doc.Name = "gcValidFrom_Doc";
		this.gcValidFrom_Doc.PrimaryKey = false;
		this.gcValidFrom_Doc.UnitField = null;
		this.gcValidFrom_Doc.Visible = true;
		this.gcValidFrom_Doc.VisibleIndex = 6;
		this.gcValidTo_Doc.Caption = "ValidTo";
		this.gcValidTo_Doc.ColumnEdit = this.repositoryItemDateEdit1;
		this.gcValidTo_Doc.CurrencyDateField = null;
		this.gcValidTo_Doc.CurrencyField = null;
		this.gcValidTo_Doc.CurrencyFixed = null;
		this.gcValidTo_Doc.CurrencyFixedRate = 0.0;
		this.gcValidTo_Doc.CurrencyRateField = null;
		this.gcValidTo_Doc.FieldName = "ValidTo";
		this.gcValidTo_Doc.Name = "gcValidTo_Doc";
		this.gcValidTo_Doc.PrimaryKey = false;
		this.gcValidTo_Doc.UnitField = null;
		this.gcValidTo_Doc.Visible = true;
		this.gcValidTo_Doc.VisibleIndex = 7;
		this.grd_OCR1.AllowPasteExcel = true;
		this.grd_OCR1.ClipboardData = null;
		this.grd_OCR1.ContextMenuStrip = this.ctm_Line;
		this.grd_OCR1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_OCR1.Location = new System.Drawing.Point(0, 0);
		this.grd_OCR1.MainView = this.gv_OCR1;
		this.grd_OCR1.Name = "grd_OCR1";
		this.grd_OCR1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_luk_OcrCode_line, this.repositoryItemDateEdit4 });
		this.grd_OCR1.Size = new System.Drawing.Size(910, 178);
		this.grd_OCR1.TabIndex = 0;
		this.grd_OCR1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_OCR1 });
		this.ctm_Line.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem1, this.removeToolStripMenuItem1 });
		this.ctm_Line.Name = "contextMenuStrip2";
		this.ctm_Line.Size = new System.Drawing.Size(124, 48);
		this.addNewToolStripMenuItem1.Name = "addNewToolStripMenuItem1";
		this.addNewToolStripMenuItem1.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem1.Text = "Add New";
		this.addNewToolStripMenuItem1.Click += new System.EventHandler(this.addNewToolStripMenuItem1_Click);
		this.removeToolStripMenuItem1.Name = "removeToolStripMenuItem1";
		this.removeToolStripMenuItem1.Size = new System.Drawing.Size(123, 22);
		this.removeToolStripMenuItem1.Text = "Remove";
		this.removeToolStripMenuItem1.Click += new System.EventHandler(this.removeToolStripMenuItem1_Click);
		this.gv_OCR1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_OCR1.ColumnKey = null;
		this.gv_OCR1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.gcOcrCode_line, this.gcPrcCode_line, this.gcPrcAmount_line, this.gcOcrTotal_line, this.gcDirect_line, this.gcUserSign_line, this.gcValidFrom_line, this.gcValidTo_line, this.gclogInstanc_line, this.gcUserSign2_line,
			this.gcupdateDate_line, this.gcOcrName_line
		});
		this.gv_OCR1.DisableFormSetting = false;
		this.gv_OCR1.DisplayException = false;
		this.gv_OCR1.EditColumn = null;
		this.gv_OCR1.FieldCurrency = null;
		this.gv_OCR1.FieldRate = null;
		this.gv_OCR1.FormId = "oDistributionRuleSetup";
		this.gv_OCR1.GridControl = this.grd_OCR1;
		this.gv_OCR1.IgnoreFilter = false;
		this.gv_OCR1.IndexEditRow = -1;
		this.gv_OCR1.IndicatorWidth = 35;
		this.gv_OCR1.IsApzTable = false;
		this.gv_OCR1.IsInsertNewRow = false;
		this.gv_OCR1.IsLoadingData = false;
		this.gv_OCR1.IsMappingContextAction = false;
		this.gv_OCR1.ItemId = "CCR1";
		this.gv_OCR1.LstColumnAllowBatch = null;
		this.gv_OCR1.LstColumnNotAllowBatch = null;
		this.gv_OCR1.LstColumnNotEditable = null;
		this.gv_OCR1.LstColumNotSetting = null;
		this.gv_OCR1.Name = "gv_OCR1";
		this.gv_OCR1.OptionsView.ShowGroupPanel = false;
		this.gv_OCR1.UseBatchMod = false;
		this.gv_OCR1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_OCR1_CustomDrawRowIndicator);
		this.gv_OCR1.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_OCR1_RowCellStyle);
		this.gv_OCR1.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_OCR1_ShowingEditor);
		this.gv_OCR1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_OCR1_CellValueChanged);
		this.gv_OCR1.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_OCR1_CellValueChanging);
		this.gcOcrCode_line.Caption = "Code";
		this.gcOcrCode_line.CurrencyDateField = null;
		this.gcOcrCode_line.CurrencyField = null;
		this.gcOcrCode_line.CurrencyFixed = null;
		this.gcOcrCode_line.CurrencyFixedRate = 0.0;
		this.gcOcrCode_line.CurrencyRateField = null;
		this.gcOcrCode_line.FieldName = "OcrCode";
		this.gcOcrCode_line.Name = "gcOcrCode_line";
		this.gcOcrCode_line.OptionsColumn.ReadOnly = true;
		this.gcOcrCode_line.PrimaryKey = false;
		this.gcOcrCode_line.UnitField = null;
		this.gcOcrCode_line.Width = 88;
		this.gcPrcCode_line.Caption = "Center Code";
		this.gcPrcCode_line.ColumnEdit = this.rep_luk_OcrCode_line;
		this.gcPrcCode_line.CurrencyDateField = null;
		this.gcPrcCode_line.CurrencyField = null;
		this.gcPrcCode_line.CurrencyFixed = null;
		this.gcPrcCode_line.CurrencyFixedRate = 0.0;
		this.gcPrcCode_line.CurrencyRateField = null;
		this.gcPrcCode_line.FieldName = "PrcCode";
		this.gcPrcCode_line.Name = "gcPrcCode_line";
		this.gcPrcCode_line.PrimaryKey = true;
		this.gcPrcCode_line.UnitField = null;
		this.gcPrcCode_line.Visible = true;
		this.gcPrcCode_line.VisibleIndex = 0;
		this.gcPrcCode_line.Width = 88;
		this.rep_luk_OcrCode_line.AutoHeight = false;
		this.rep_luk_OcrCode_line.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_luk_OcrCode_line.Name = "rep_luk_OcrCode_line";
		this.rep_luk_OcrCode_line.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rIB_OcrCode_line_ButtonClick);
		this.gcPrcAmount_line.Caption = "Value";
		this.gcPrcAmount_line.CurrencyDateField = null;
		this.gcPrcAmount_line.CurrencyField = null;
		this.gcPrcAmount_line.CurrencyFixed = null;
		this.gcPrcAmount_line.CurrencyFixedRate = 0.0;
		this.gcPrcAmount_line.CurrencyRateField = null;
		this.gcPrcAmount_line.FieldName = "PrcAmount";
		this.gcPrcAmount_line.Name = "gcPrcAmount_line";
		this.gcPrcAmount_line.PrimaryKey = false;
		this.gcPrcAmount_line.UnitField = null;
		this.gcPrcAmount_line.Visible = true;
		this.gcPrcAmount_line.VisibleIndex = 2;
		this.gcPrcAmount_line.Width = 79;
		this.gcOcrTotal_line.Caption = "OcrTotal";
		this.gcOcrTotal_line.CurrencyDateField = null;
		this.gcOcrTotal_line.CurrencyField = null;
		this.gcOcrTotal_line.CurrencyFixed = null;
		this.gcOcrTotal_line.CurrencyFixedRate = 0.0;
		this.gcOcrTotal_line.CurrencyRateField = null;
		this.gcOcrTotal_line.FieldName = "OcrTotal";
		this.gcOcrTotal_line.Name = "gcOcrTotal_line";
		this.gcOcrTotal_line.OptionsColumn.ReadOnly = true;
		this.gcOcrTotal_line.PrimaryKey = false;
		this.gcOcrTotal_line.UnitField = null;
		this.gcOcrTotal_line.Width = 79;
		this.gcDirect_line.Caption = "Direct";
		this.gcDirect_line.CurrencyDateField = null;
		this.gcDirect_line.CurrencyField = null;
		this.gcDirect_line.CurrencyFixed = null;
		this.gcDirect_line.CurrencyFixedRate = 0.0;
		this.gcDirect_line.CurrencyRateField = null;
		this.gcDirect_line.Name = "gcDirect_line";
		this.gcDirect_line.PrimaryKey = false;
		this.gcDirect_line.UnitField = null;
		this.gcDirect_line.Width = 79;
		this.gcUserSign_line.Caption = "UserSign";
		this.gcUserSign_line.CurrencyDateField = null;
		this.gcUserSign_line.CurrencyField = null;
		this.gcUserSign_line.CurrencyFixed = null;
		this.gcUserSign_line.CurrencyFixedRate = 0.0;
		this.gcUserSign_line.CurrencyRateField = null;
		this.gcUserSign_line.FieldName = "UserSign";
		this.gcUserSign_line.Name = "gcUserSign_line";
		this.gcUserSign_line.OptionsColumn.ReadOnly = true;
		this.gcUserSign_line.PrimaryKey = false;
		this.gcUserSign_line.UnitField = null;
		this.gcUserSign_line.Width = 79;
		this.gcValidFrom_line.Caption = "ValidFrom";
		this.gcValidFrom_line.ColumnEdit = this.repositoryItemDateEdit4;
		this.gcValidFrom_line.CurrencyDateField = null;
		this.gcValidFrom_line.CurrencyField = null;
		this.gcValidFrom_line.CurrencyFixed = null;
		this.gcValidFrom_line.CurrencyFixedRate = 0.0;
		this.gcValidFrom_line.CurrencyRateField = null;
		this.gcValidFrom_line.FieldName = "ValidFrom";
		this.gcValidFrom_line.Name = "gcValidFrom_line";
		this.gcValidFrom_line.OptionsColumn.ReadOnly = true;
		this.gcValidFrom_line.PrimaryKey = false;
		this.gcValidFrom_line.UnitField = null;
		this.gcValidFrom_line.Width = 79;
		this.repositoryItemDateEdit4.AutoHeight = false;
		this.repositoryItemDateEdit4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit4.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit4.Name = "repositoryItemDateEdit4";
		this.gcValidTo_line.Caption = "ValidTo";
		this.gcValidTo_line.ColumnEdit = this.repositoryItemDateEdit4;
		this.gcValidTo_line.CurrencyDateField = null;
		this.gcValidTo_line.CurrencyField = null;
		this.gcValidTo_line.CurrencyFixed = null;
		this.gcValidTo_line.CurrencyFixedRate = 0.0;
		this.gcValidTo_line.CurrencyRateField = null;
		this.gcValidTo_line.FieldName = "ValidTo";
		this.gcValidTo_line.Name = "gcValidTo_line";
		this.gcValidTo_line.OptionsColumn.ReadOnly = true;
		this.gcValidTo_line.PrimaryKey = false;
		this.gcValidTo_line.UnitField = null;
		this.gcValidTo_line.Width = 79;
		this.gclogInstanc_line.Caption = "logInstanc";
		this.gclogInstanc_line.CurrencyDateField = null;
		this.gclogInstanc_line.CurrencyField = null;
		this.gclogInstanc_line.CurrencyFixed = null;
		this.gclogInstanc_line.CurrencyFixedRate = 0.0;
		this.gclogInstanc_line.CurrencyRateField = null;
		this.gclogInstanc_line.FieldName = "logInstanc";
		this.gclogInstanc_line.Name = "gclogInstanc_line";
		this.gclogInstanc_line.PrimaryKey = false;
		this.gclogInstanc_line.UnitField = null;
		this.gclogInstanc_line.Width = 79;
		this.gcUserSign2_line.Caption = "UserSign2";
		this.gcUserSign2_line.CurrencyDateField = null;
		this.gcUserSign2_line.CurrencyField = null;
		this.gcUserSign2_line.CurrencyFixed = null;
		this.gcUserSign2_line.CurrencyFixedRate = 0.0;
		this.gcUserSign2_line.CurrencyRateField = null;
		this.gcUserSign2_line.FieldName = "UserSign2";
		this.gcUserSign2_line.Name = "gcUserSign2_line";
		this.gcUserSign2_line.PrimaryKey = false;
		this.gcUserSign2_line.UnitField = null;
		this.gcUserSign2_line.Width = 79;
		this.gcupdateDate_line.Caption = "updateDate";
		this.gcupdateDate_line.ColumnEdit = this.repositoryItemDateEdit4;
		this.gcupdateDate_line.CurrencyDateField = null;
		this.gcupdateDate_line.CurrencyField = null;
		this.gcupdateDate_line.CurrencyFixed = null;
		this.gcupdateDate_line.CurrencyFixedRate = 0.0;
		this.gcupdateDate_line.CurrencyRateField = null;
		this.gcupdateDate_line.FieldName = "updateDate";
		this.gcupdateDate_line.Name = "gcupdateDate_line";
		this.gcupdateDate_line.PrimaryKey = false;
		this.gcupdateDate_line.UnitField = null;
		this.gcupdateDate_line.Width = 92;
		this.gcOcrName_line.Caption = "Center Name";
		this.gcOcrName_line.CurrencyDateField = null;
		this.gcOcrName_line.CurrencyField = null;
		this.gcOcrName_line.CurrencyFixed = null;
		this.gcOcrName_line.CurrencyFixedRate = 0.0;
		this.gcOcrName_line.CurrencyRateField = null;
		this.gcOcrName_line.FieldName = "PrcName";
		this.gcOcrName_line.Name = "gcOcrName_line";
		this.gcOcrName_line.PrimaryKey = false;
		this.gcOcrName_line.UnitField = null;
		this.gcOcrName_line.Visible = true;
		this.gcOcrName_line.VisibleIndex = 1;
		this.gcOcrName_line.Width = 162;
		this.bt_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_OK.EditControl = null;
		this.bt_OK.EditValue = "OK";
		this.bt_OK.FieldName = null;
		this.bt_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_OK.ImageOptions.Image");
		this.bt_OK.IsSearchControl = false;
		this.bt_OK.Location = new System.Drawing.Point(1, 498);
		this.bt_OK.Name = "bt_OK";
		this.bt_OK.ReadOnly = false;
		this.bt_OK.SettingInVisible = false;
		this.bt_OK.SettingReadonly = false;
		this.bt_OK.Size = new System.Drawing.Size(75, 23);
		this.bt_OK.TabIndex = 1;
		this.bt_OK.Text = "OK";
		this.bt_OK.Click += new System.EventHandler(this.bt_OK_Click);
		this.bt_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_Cancel.EditControl = null;
		this.bt_Cancel.EditValue = "Cancel";
		this.bt_Cancel.FieldName = null;
		this.bt_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_Cancel.ImageOptions.Image");
		this.bt_Cancel.IsSearchControl = false;
		this.bt_Cancel.Location = new System.Drawing.Point(157, 498);
		this.bt_Cancel.Name = "bt_Cancel";
		this.bt_Cancel.ReadOnly = false;
		this.bt_Cancel.SettingInVisible = false;
		this.bt_Cancel.SettingReadonly = false;
		this.bt_Cancel.Size = new System.Drawing.Size(75, 23);
		this.bt_Cancel.TabIndex = 1;
		this.bt_Cancel.Text = "Cancel";
		this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
		this.txt_Code.EditControl = null;
		this.txt_Code.FieldName = null;
		this.txt_Code.IsSearchControl = false;
		this.txt_Code.Location = new System.Drawing.Point(98, 9);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.SettingInVisible = false;
		this.txt_Code.SettingReadonly = false;
		this.txt_Code.Size = new System.Drawing.Size(121, 20);
		this.txt_Code.TabIndex = 2;
		this.lb_Code.EditControl = null;
		this.lb_Code.EditValue = "Code";
		this.lb_Code.FieldName = null;
		this.lb_Code.HideWhenEmpty = false;
		this.lb_Code.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Code.Location = new System.Drawing.Point(12, 12);
		this.lb_Code.Name = "lb_Code";
		this.lb_Code.ReadOnly = false;
		this.lb_Code.SettingInVisible = false;
		this.lb_Code.SettingReadonly = false;
		this.lb_Code.ShowUnderLine = true;
		this.lb_Code.Size = new System.Drawing.Size(89, 20);
		this.lb_Code.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Code.TabIndex = 4;
		this.lb_Code.Utext = "Code";
		this.txt_Name.EditControl = null;
		this.txt_Name.FieldName = null;
		this.txt_Name.IsSearchControl = false;
		this.txt_Name.Location = new System.Drawing.Point(331, 9);
		this.txt_Name.Name = "txt_Name";
		this.txt_Name.SettingInVisible = false;
		this.txt_Name.SettingReadonly = false;
		this.txt_Name.Size = new System.Drawing.Size(213, 20);
		this.txt_Name.TabIndex = 2;
		this.lb_Description.EditControl = null;
		this.lb_Description.EditValue = "Description";
		this.lb_Description.FieldName = null;
		this.lb_Description.HideWhenEmpty = false;
		this.lb_Description.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Description.Location = new System.Drawing.Point(243, 12);
		this.lb_Description.Name = "lb_Description";
		this.lb_Description.ReadOnly = false;
		this.lb_Description.SettingInVisible = false;
		this.lb_Description.SettingReadonly = false;
		this.lb_Description.ShowUnderLine = true;
		this.lb_Description.Size = new System.Drawing.Size(89, 20);
		this.lb_Description.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Description.TabIndex = 4;
		this.lb_Description.Utext = "Description";
		this.lb_Dimension.EditControl = null;
		this.lb_Dimension.EditValue = "Dimension";
		this.lb_Dimension.FieldName = null;
		this.lb_Dimension.HideWhenEmpty = false;
		this.lb_Dimension.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Dimension.Location = new System.Drawing.Point(12, 48);
		this.lb_Dimension.Name = "lb_Dimension";
		this.lb_Dimension.ReadOnly = false;
		this.lb_Dimension.SettingInVisible = false;
		this.lb_Dimension.SettingReadonly = false;
		this.lb_Dimension.ShowUnderLine = true;
		this.lb_Dimension.Size = new System.Drawing.Size(89, 20);
		this.lb_Dimension.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Dimension.TabIndex = 5;
		this.lb_Dimension.Utext = "Dimension";
		this.luk_Dimension.Location = new System.Drawing.Point(98, 45);
		this.luk_Dimension.Name = "luk_Dimension";
		this.luk_Dimension.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Dimension.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DimCode", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DimDesc", "Name")
		});
		this.luk_Dimension.Properties.DisplayMember = "DimDesc";
		this.luk_Dimension.Properties.NullText = "";
		this.luk_Dimension.Properties.ValueMember = "DimCode";
		this.luk_Dimension.Size = new System.Drawing.Size(121, 20);
		this.luk_Dimension.TabIndex = 6;
		this.luk_Dimension.EditValueChanged += new System.EventHandler(this.lookUpEdit_Dimension_EditValueChanged);
		this.bt_Search.EditControl = null;
		this.bt_Search.EditValue = "Search";
		this.bt_Search.FieldName = null;
		this.bt_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_Search.ImageOptions.Image");
		this.bt_Search.IsSearchControl = false;
		this.bt_Search.Location = new System.Drawing.Point(243, 43);
		this.bt_Search.Name = "bt_Search";
		this.bt_Search.ReadOnly = false;
		this.bt_Search.SettingInVisible = false;
		this.bt_Search.SettingReadonly = false;
		this.bt_Search.Size = new System.Drawing.Size(89, 23);
		this.bt_Search.TabIndex = 1;
		this.bt_Search.Text = "Search";
		this.bt_Search.Click += new System.EventHandler(this.bt_Search_Click);
		this.bt_Refresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_Refresh.EditControl = null;
		this.bt_Refresh.EditValue = "Refresh";
		this.bt_Refresh.FieldName = null;
		this.bt_Refresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_Refresh.ImageOptions.Image");
		this.bt_Refresh.IsSearchControl = false;
		this.bt_Refresh.Location = new System.Drawing.Point(79, 498);
		this.bt_Refresh.Name = "bt_Refresh";
		this.bt_Refresh.ReadOnly = false;
		this.bt_Refresh.SettingInVisible = false;
		this.bt_Refresh.SettingReadonly = false;
		this.bt_Refresh.Size = new System.Drawing.Size(75, 23);
		this.bt_Refresh.TabIndex = 7;
		this.bt_Refresh.Text = "Refresh";
		this.bt_Refresh.Click += new System.EventHandler(this.bt_Refresh_Click);
		this.lbl_Centr_z.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbl_Centr_z.AutoSize = true;
		this.lbl_Centr_z.Location = new System.Drawing.Point(95, 455);
		this.lbl_Centr_z.Name = "lbl_Centr_z";
		this.lbl_Centr_z.Size = new System.Drawing.Size(51, 13);
		this.lbl_Centr_z.TabIndex = 8;
		this.lbl_Centr_z.Text = "Center_z";
		this.lbl_Centr_z_Name.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbl_Centr_z_Name.EditControl = null;
		this.lbl_Centr_z_Name.EditValue = "lbl_Centr_z_Name";
		this.lbl_Centr_z_Name.FieldName = null;
		this.lbl_Centr_z_Name.HideWhenEmpty = false;
		this.lbl_Centr_z_Name.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Centr_z_Name.Location = new System.Drawing.Point(219, 448);
		this.lbl_Centr_z_Name.Name = "lbl_Centr_z_Name";
		this.lbl_Centr_z_Name.ReadOnly = false;
		this.lbl_Centr_z_Name.SettingInVisible = false;
		this.lbl_Centr_z_Name.SettingReadonly = false;
		this.lbl_Centr_z_Name.ShowUnderLine = true;
		this.lbl_Centr_z_Name.Size = new System.Drawing.Size(222, 20);
		this.lbl_Centr_z_Name.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Centr_z_Name.TabIndex = 9;
		this.lbl_Centr_z_Name.Utext = "lbl_Centr_z_Name";
		this.label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(216, 471);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(60, 13);
		this.label2.TabIndex = 10;
		this.label2.Text = "Table Total";
		this.lb_TotalLine.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lb_TotalLine.AutoSize = true;
		this.lb_TotalLine.Location = new System.Drawing.Point(363, 471);
		this.lb_TotalLine.Name = "lb_TotalLine";
		this.lb_TotalLine.Size = new System.Drawing.Size(78, 13);
		this.lb_TotalLine.TabIndex = 11;
		this.lb_TotalLine.Text = "label_TotalLine";
		this.lb_GeneralCenter.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lb_GeneralCenter.AutoSize = true;
		this.lb_GeneralCenter.Location = new System.Drawing.Point(363, 448);
		this.lb_GeneralCenter.Name = "lb_GeneralCenter";
		this.lb_GeneralCenter.Size = new System.Drawing.Size(91, 13);
		this.lb_GeneralCenter.TabIndex = 12;
		this.lb_GeneralCenter.Text = "lb_GeneralCenter";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(910, 524);
		base.Controls.Add(this.txt_Name);
		base.Controls.Add(this.txt_Code);
		base.Controls.Add(this.lb_GeneralCenter);
		base.Controls.Add(this.lb_TotalLine);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.lbl_Centr_z_Name);
		base.Controls.Add(this.lbl_Centr_z);
		base.Controls.Add(this.bt_Refresh);
		base.Controls.Add(this.luk_Dimension);
		base.Controls.Add(this.lb_Dimension);
		base.Controls.Add(this.lb_Description);
		base.Controls.Add(this.lb_Code);
		base.Controls.Add(this.bt_Cancel);
		base.Controls.Add(this.bt_Search);
		base.Controls.Add(this.bt_OK);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oDistributionRuleSetup";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_DistributeRule";
		this.Text = "Distribution Rule - Setup";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_DistributeRule_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_OOCR).EndInit();
		this.ctm_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_OOCR).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_Dimesion).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_OCR1).EndInit();
		this.ctm_Line.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_OCR1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_OcrCode_line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit4.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Dimension.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
