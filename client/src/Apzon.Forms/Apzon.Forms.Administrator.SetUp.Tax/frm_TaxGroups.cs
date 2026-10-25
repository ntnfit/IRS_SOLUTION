using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using SAPbobsCOM;

namespace Apzon.Forms.Administrator.SetUp.Tax;

public class frm_TaxGroups : XtraFormEx
{
	public DataTable DtLineVtg1 = new DataTable();

	public string VatType = "";

	private string _oldCodeValue = "";

	private string _Code = string.Empty;

	private Dictionary<string, string> _lstCategogry = new Dictionary<string, string>
	{
		{ "I", "Input Tax" },
		{ "O", "Output Tax" }
	};

	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gcCode;

	private GridColumnEx gcInactive;

	private RepositoryItemCheckEdit rep_Inactive;

	private GridColumnEx gcName;

	private GridColumnEx gcCategory;

	private GridColumnEx gcAcqstnRvrs;

	private GridColumnEx gcEffecDate;

	private GridColumnEx gcRate;

	private GridColumnEx gcNonDedct;

	private GridColumnEx gcAccount;

	private GridColumnEx gcAcqsTax;

	private GridColumnEx gcDeferrAcc;

	private GridColumnEx gcNonDedAcc;

	private GridColumnEx gcReportCode;

	private SimpleButtonEx bt_OK;

	private SimpleButtonEx bt_Cancel;

	private SimpleButtonEx bt_TaxDefinition;

	private RepositoryItemLookUpEdit rILUE_Category;

	private RepositoryItemButtonEdit rIBE_Account;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private GridColumnEx gCLocked;

	private GridColumnEx gCDataSource;

	private GridColumnEx gCUserSign;

	private GridColumnEx gCIsEC;

	private GridColumnEx gcIndicator;

	private GridColumnEx gcGoddsShip;

	private GridColumnEx gcEquVatPr;

	private GridColumnEx gcFixdAssts;

	private GridColumnEx gcCalcMethod;

	private GridColumnEx gcTaxType;

	private GridColumnEx gcFixedAmnt;

	private GridColumnEx gcExtCode;

	private GridColumnEx gcCorrection;

	private GridColumnEx gcVatCrctn;

	private GridColumnEx gcRetVatCode;

	private GridColumnEx gcRepType;

	private GridColumnEx gcLogInstanc;

	private GridColumnEx gcUpdateDate;

	private GridColumnEx gcTaxCtgr;

	private GridColumnEx gcEquAccount;

	private GridColumnEx gcUserSign2;

	private GridColumnEx gcIsIGIC;

	private GridColumnEx gcServSupply;

	private GridColumnEx gcTaxCtgrBL;

	private GridColumnEx gcR349Code;

	private GridColumnEx gcVatRevAcc;

	private GridColumnEx gcCashDisAcc;

	private GridColumnEx gcDpmTaxOAcc;

	private GridColumnEx gcVatDedAcc;

	private GridColumnEx gcCstmExpAcc;

	private GridColumnEx gcCstmAlcAcc;

	private GridColumnEx gcTaxRegion;

	private GridColumnEx gcExemReason;

	private GridColumnEx gcAgent;

	private GridColumnEx gcOpCode;

	private GridColumnEx gcExport;

	private GridColumnEx gcSection;

	private RepositoryItemDateEdit rep_Effective;

	private RepositoryItemCalcEdit rep_Deduct;

	private RepositoryItemButtonEdit rep_AcqsTax;

	private RepositoryItemButtonEdit rep_DeferrAcc;

	private RepositoryItemButtonEdit rep_NonDedAcc;

	private RepositoryItemCheckEdit rep_AcqstnRvrs;

	private RepositoryItemButtonEdit rCode;

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
		case Enums.FormViewModeType.Update:
			bt_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			bt_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_TaxGroups()
	{
		try
		{
			OnLoadAsync += frm_TaxGroups_Load;
			InitializeComponent();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			if (e.Column.FieldName == "Code" || e.Column.FieldName == "Inactive" || e.Column.FieldName == "Name" || e.Column.FieldName == "Category" || e.Column.FieldName == "AcqstnRvrs" || e.Column.FieldName == "EffecDate" || e.Column.FieldName == "Rate" || e.Column.FieldName == "NonDedct" || e.Column.FieldName == "Account" || e.Column.FieldName == "AcqsTax" || e.Column.FieldName == "DeferrAcc" || e.Column.FieldName == "NonDedAcc" || e.Column.FieldName == "ReportCode")
			{
				if (e.RowHandle == dataSourceTable.Rows.Count - 1)
				{
					AddNewRow();
				}
				FormMode = Enums.FormViewModeType.Update;
			}
			if (e.Column.FieldName == "Code")
			{
				DataRow[] array = DtLineVtg1.Select($"Code = '{_oldCodeValue}'");
				for (int i = 0; i < array.Length; i++)
				{
					array[i]["Code"] = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "Code"));
				}
				DtLineVtg1.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Code")
			{
				_oldCodeValue = Function.ToString(gv_Doc.ActiveEditor.OldEditValue);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (gv_Doc.FocusedColumn.FieldName != "Code")
			{
				return;
			}
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				string code = Function.ToString(e.Value);
				if (dataTable.AsEnumerable().Any((DataRow r) => string.Compare(Function.ToString(r["Code"]), code, ignoreCase: true) == 0))
				{
					e.Valid = false;
					e.ErrorText = "Duplicated code";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		e.ExceptionMode = ExceptionMode.DisplayError;
	}

	private void rCode_EditValueChanging(object sender, ChangingEventArgs e)
	{
	}

	private void rCode_EditValueChanged(object sender, EventArgs e)
	{
	}

	private async Task frm_TaxGroups_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			GetCategory();
			await LoadData();
			await LoadVTG1();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if ((gv_Doc.FocusedColumn.FieldName == "Code" || gv_Doc.FocusedColumn.FieldName == "Category") && Function.ToString(gv_Doc.GetFocusedRowCellValue("IsExist")).Equals("Y"))
			{
				e.Cancel = true;
			}
			if (gv_Doc.FocusedColumn.FieldName == "EffecDate" || gv_Doc.FocusedColumn.FieldName == "Rate")
			{
				e.Cancel = true;
			}
			if ((gv_Doc.FocusedColumn.FieldName == "AcqstnRvrs" || gv_Doc.FocusedColumn.FieldName == "AcqsTax") && Function.ToString(gv_Doc.GetFocusedRowCellValue("Category")) == "O")
			{
				e.Cancel = true;
			}
			if (gv_Doc.FocusedColumn.FieldName == "NonDedct")
			{
				if (Function.ToString(gv_Doc.GetFocusedRowCellValue("Category")) == "O")
				{
					e.Cancel = true;
				}
				else if (Function.ToString(gv_Doc.GetFocusedRowCellValue("AcqstnRvrs")) == "Y")
				{
					e.Cancel = true;
				}
			}
			if (gv_Doc.FocusedColumn.FieldName == "NonDedAcc" && Function.ParseDecimal(gv_Doc.GetFocusedRowCellValue("NonDedct")) == 0m)
			{
				e.Cancel = true;
			}
			if (gv_Doc.FocusedColumn.FieldName == "Category" && !string.IsNullOrEmpty(VatType))
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

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			if (e.Column.FieldName == "EffecDate" || e.Column.FieldName == "Rate")
			{
				e.Appearance.BackColor = Color.LightGray;
			}
			if (e.Column.FieldName == "AcqstnRvrs" || e.Column.FieldName == "AcqsTax")
			{
				if (Function.ToString(gridView.GetRowCellValue(e.RowHandle, gcCategory)) == "O")
				{
					e.Appearance.BackColor = Color.LightGray;
				}
				else
				{
					e.Appearance.BackColor = SystemColors.Window;
				}
			}
			if (e.Column.FieldName == "NonDedct")
			{
				if (Function.ToString(gridView.GetRowCellValue(e.RowHandle, gcCategory)) == "O")
				{
					e.Appearance.BackColor = Color.LightGray;
				}
				else if (Function.ToString(gridView.GetRowCellValue(e.RowHandle, gcAcqstnRvrs)) == "Y")
				{
					e.Appearance.BackColor = Color.LightGray;
				}
				else
				{
					e.Appearance.BackColor = SystemColors.Window;
				}
			}
			if (e.Column.FieldName == "NonDedAcc")
			{
				if (Function.ParseDecimal(gridView.GetRowCellValue(e.RowHandle, gcNonDedct)) == 0m)
				{
					e.Appearance.BackColor = Color.LightGray;
				}
				else
				{
					e.Appearance.BackColor = SystemColors.Window;
				}
			}
			if (e.Column.FieldName == "Category")
			{
				if (!string.IsNullOrEmpty(VatType) || Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "IsExist")) == "Y")
				{
					e.Appearance.BackColor = Color.LightGray;
				}
				else
				{
					e.Appearance.BackColor = SystemColors.Window;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_AcqstnRvrs_EditValueChanged(object sender, EventArgs e)
	{
		gv_Doc.PostEditor();
	}

	private void rILUE_Category_EditValueChanged(object sender, EventArgs e)
	{
		gv_Doc.PostEditor();
	}

	private void rep_Deduct_EditValueChanged(object sender, EventArgs e)
	{
		gv_Doc.PostEditor();
	}

	private void gv_Doc_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			Point pt = gridView.GridControl.PointToClient(Control.MousePosition);
			GridHitInfo gridHitInfo = gridView.CalcHitInfo(pt);
			if ((gridHitInfo.InRow || gridHitInfo.InRowCell) && gridHitInfo.Column == null && !string.IsNullOrEmpty(Function.ToString(gridView.GetFocusedRowCellValue("Code"))))
			{
				ShowForm_TaxDefinitionSetup();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_AcqsTax_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.AcctCode, multiSelect: false, null);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				AcctCodeImpl acctCodeImpl = (AcctCodeImpl)frm_ChooseFromList2.ChooseItem;
				if (!string.IsNullOrEmpty(acctCodeImpl.AcctCode))
				{
					gv_Doc.SetFocusedRowCellValue("AcqsTax", acctCodeImpl.AcctCode);
				}
				grd_Doc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_DeferrAcc_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.AcctCode, multiSelect: false, null);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				AcctCodeImpl acctCodeImpl = (AcctCodeImpl)frm_ChooseFromList2.ChooseItem;
				if (!string.IsNullOrEmpty(acctCodeImpl.AcctCode))
				{
					gv_Doc.SetFocusedRowCellValue("DeferrAcc", acctCodeImpl.AcctCode);
				}
				grd_Doc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_NonDedAcc_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.AcctCode, multiSelect: false, null);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				AcctCodeImpl acctCodeImpl = (AcctCodeImpl)frm_ChooseFromList2.ChooseItem;
				if (!string.IsNullOrEmpty(acctCodeImpl.AcctCode))
				{
					gv_Doc.SetFocusedRowCellValue("NonDedAcc", acctCodeImpl.AcctCode);
				}
				grd_Doc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void grd_Doc_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				await PasteFromExcelToDataTable(grd_Doc, gv_Doc, new List<string> { "Code" });
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rIBE_Account_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.AcctCode, multiSelect: false, null);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				AcctCodeImpl acctCodeImpl = (AcctCodeImpl)frm_ChooseFromList2.ChooseItem;
				if (!string.IsNullOrEmpty(acctCodeImpl.AcctCode))
				{
					gv_Doc.SetFocusedRowCellValue("Account", acctCodeImpl.AcctCode);
				}
				grd_Doc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bt_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				Save();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bt_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void bt_TaxDefinition_Click(object sender, EventArgs e)
	{
		try
		{
			ShowForm_TaxDefinitionSetup();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
			if (_httpClient.TaxGroupsClient.Exists(text))
			{
				string text2 = _httpClient.TaxGroupsClient.CheckCode(text);
				if (!string.IsNullOrEmpty(text2))
				{
					WinCommons.Error(Enums.MessageCode.None, text2);
					return;
				}
				text2 = _httpClient.TaxGroupsClient.Delete(text);
				if (!string.IsNullOrEmpty(text2))
				{
					WinCommons.Error(Enums.MessageCode.None, text2);
					return;
				}
				WinCommons.Success(Enums.MessageCode.OperationCompletedSuccess, "");
			}
			gv_Doc.BeginUpdate();
			gv_Doc.DeleteSelectedRows();
			gv_Doc.EndUpdate();
			DataTable dataTable = (DataTable)grd_Doc.DataSource;
			dataTable.AcceptChanges();
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
			for (int i = 0; i < DtLineVtg1.Rows.Count; i++)
			{
				if (Function.ToString(DtLineVtg1.Rows[i]["Code"]) == text)
				{
					DtLineVtg1.Rows.RemoveAt(i);
					DtLineVtg1.AcceptChanges();
				}
			}
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
			DataTable dataSource = await _httpClient.TaxGroupsClient.GetDataOVTG(VatType);
			grd_Doc.DataSource = dataSource;
			AddNewRow();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewRow()
	{
		try
		{
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["Category"] = VatType;
			dataRow["Locked"] = "N";
			dataRow["DataSource"] = "N";
			dataRow["IsEC"] = "N";
			dataRow["AcqstnRvrs"] = "N";
			dataRow["FixdAssts"] = "N";
			dataRow["CalcMethod"] = "R";
			dataRow["TaxType"] = "V";
			dataRow["Correction"] = "N";
			dataRow["LogInstanc"] = 0;
			dataRow["TaxCtgr"] = "E";
			dataRow["IsIGIC"] = "N";
			dataRow["TaxCtgrBL"] = "E";
			dataRow["R349Code"] = 0;
			dataRow["TaxRegion"] = "PT";
			dataRow["Agent"] = "N";
			dataRow["Export"] = "N";
			dataRow["Inactive"] = "N";
			dataRow["IsExist"] = "N";
			dataSourceTable.Rows.Add(dataRow);
			dataSourceTable.AcceptChanges();
			grd_Doc.DataSource = dataSourceTable;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void SettingTable(DataTable dt, string code)
	{
		try
		{
			if (dt.IsNotNull())
			{
				DataTable dataTable = DtLineVtg1.Copy();
				DtLineVtg1 = DtLineVtg1.Clone();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					if (Function.ToString(dataTable.Rows[i]["Code"]) != code)
					{
						DataRow dataRow = DtLineVtg1.NewRow();
						for (int j = 0; j < dataTable.Columns.Count; j++)
						{
							string columnName = dataTable.Columns[j].ColumnName;
							dataRow[columnName] = dataTable.Rows[i][columnName];
						}
						DtLineVtg1.Rows.Add(dataRow);
					}
				}
				DtLineVtg1.AcceptChanges();
				for (int k = 0; k < dt.Rows.Count; k++)
				{
					if (!DtLineVtg1.IsNotNull())
					{
						DtLineVtg1 = dt.Clone();
					}
					DataRow dataRow2 = DtLineVtg1.NewRow();
					for (int l = 0; l < dt.Columns.Count; l++)
					{
						string columnName2 = dt.Columns[l].ColumnName;
						dataRow2[columnName2] = dt.Rows[k][columnName2];
					}
					DtLineVtg1.Rows.Add(dataRow2);
				}
				DtLineVtg1.AcceptChanges();
			}
			DateTime dateTime = default;
			double num = 0.0;
			for (int m = 0; m < DtLineVtg1.Rows.Count; m++)
			{
				DataRow[] array = DtLineVtg1.Select("[Code]= '" + _Code + "'");
				for (int n = 0; n < array.Length; n++)
				{
					if (array[n]["EffecDate"] != null && array[n]["EffecDate"] != DBNull.Value && Function.ParseDateTime(array[n]["EffecDate"]).Value.CompareTo(dateTime) > 0)
					{
						dateTime = Function.ParseDateTime(array[n]["EffecDate"]).Value;
						num = Function.ParseDouble(array[n]["Rate"]);
					}
					gv_Doc.SetFocusedRowCellValue("EffecDate", dateTime);
					gv_Doc.SetFocusedRowCellValue("Rate", num);
					FormMode = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ShowForm_TaxDefinitionSetup()
	{
		try
		{
			_Code = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
			if (string.IsNullOrEmpty(_Code))
			{
				WinCommons.Error(Enums.MessageCode.ValueCannotBeEmptyTaxDefinitionCode, "");
				return;
			}
			DataTable dataTable = DtLineVtg1.Clone();
			if (DtLineVtg1.IsNotNull())
			{
				for (int i = 0; i < DtLineVtg1.Rows.Count; i++)
				{
					if (Function.ToString(DtLineVtg1.Rows[i]["Code"]) == _Code.Trim())
					{
						DataRow dataRow = dataTable.NewRow();
						for (int j = 0; j < dataTable.Columns.Count; j++)
						{
							dataRow[j] = DtLineVtg1.Rows[i][j];
						}
						dataTable.Rows.Add(dataRow);
						dataTable.AcceptChanges();
					}
				}
			}
			frm_TaxDefinitionSetup frm_TaxDefinitionSetup2 = new frm_TaxDefinitionSetup(dataTable, _Code);
			frm_TaxDefinitionSetup2.GetTable = SettingTable;
			frm_TaxDefinitionSetup2._CodeVat = _Code;
			frm_TaxDefinitionSetup2.MdiParent = MdiParent;
			frm_TaxDefinitionSetup2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Save()
	{
		_ = 4;
		try
		{
			WinCommons._OpenProcessing();
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				dataTable.AcceptChanges();
				DataTable dataTable2 = dataTable.Copy();
				DataRow[] array = (from t in dataTable2.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["Code"]))
					select t).ToArray();
				if (array.Length != 0)
				{
					dataTable2 = array.CopyToDataTable();
				}
				List<IGrouping<string, DataRow>> list = (from i in dataTable2.AsEnumerable()
					group i by Function.ToString(i.Field<string>("Code")).ToUpper() into gr
					where gr.Count() > 1
					select gr).ToList();
				if (list != null && list.Count > 0)
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExsitsInFollowingTableTaxDefinition, "");
					return;
				}
				if ((from t in dataTable2.AsEnumerable()
					where string.IsNullOrEmpty(Function.ToString(t["EffecDate"]))
					select t).ToArray().Length != 0)
				{
					WinCommons.Error(Enums.MessageCode.DefineEffectiveDate, "");
					return;
				}
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(dataTable2.Copy());
				dataSet.Tables.Add(DtLineVtg1.Copy());
				if (GlobalData.LoginSap)
				{
					List<VatGroupsImpl> lst = ConvertDataTableToEntity<VatGroupsImpl>(dataTable2);
					int j;
					for (j = 0; j < lst.Count; j++)
					{
						List<DataRow> list2 = (from t in DtLineVtg1.Copy().AsEnumerable()
							where Function.ToString(t["Code"]) == Function.ToString(lst[j].Code)
							select t).ToList();
						if (list2.Count > 0)
						{
							DataTable dt = list2.CopyToDataTable();
							lst[j].VatGroups_Lines = ConvertDataTableToEntity<VatGroups_LinesImpl>(dt);
						}
					}
					lst = lst.Where((VatGroupsImpl x) => x.Code != null && x.Code.Length > 0).ToList();
					lst.RemoveAll((VatGroupsImpl x) => string.IsNullOrEmpty(x.Code));
					using HttpClientHelper<List<VatGroupsImpl>> client = new HttpClientHelper<List<VatGroupsImpl>>(Configuration.ApiServer, "VatGroups");
					_ = grd_Doc.DataSource;
					WinCommons._OpenProcessing();
					if (string.IsNullOrWhiteSpace(await client.PostAsJsonAsync("SaveSap", lst)))
					{
						WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
						await LoadData();
						await LoadVTG1();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.SaveError, "");
					}
				}
				else
				{
					HttpResult httpResult = _httpClient.TaxGroupsClient.SaveVat(dataSet);
					if (string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Success(Enums.MessageCode.OperationCompletedSuccess, "");
						await LoadData();
						await LoadVTG1();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async Task LoadVTG1()
	{
		try
		{
			DtLineVtg1 = await _httpClient.TaxGroupsClient.GetAllLine_VTG1();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetCategory()
	{
		try
		{
			List<Common.Table_Code> dataSource = _lstCategogry.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			rILUE_Category.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		await frm_TaxGroups_Load(null, null);
	}

	private async Task PasteFromExcelToDataTable(GridControl grd, GridView gv, List<string> key)
	{
		try
		{
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			string clipboardData = dataObject.GetData(DataFormats.UnicodeText).ToString();
			if (!dataObject.GetDataPresent(DataFormats.Text))
			{
				return;
			}
			DataTable dataTable = Function.PasteFromExcel(clipboardData);
			if (dataTable != null && dataTable.Rows.Count == 1 && dataTable.Columns.Count > 0)
			{
				int num = gv.FocusedRowHandle;
				GridColumn focusedColumn = gv.FocusedColumn;
				if (key.Contains(focusedColumn.FieldName))
				{
					int focusedRowHandle = gv.FocusedRowHandle;
					for (int num2 = dataTable.Rows.Count - 1; num2 >= 0; num2--)
					{
						DataRow row = grd.DataSourceTable().NewRow();
						grd.DataSourceTable().Rows.InsertAt(row, focusedRowHandle);
						grd.DataSourceTable().AcceptChanges();
						int colunmIndex = focusedColumn.VisibleIndex;
						for (int i = 0; i < dataTable.Columns.Count; i++)
						{
							gv.FocusedRowHandle = focusedRowHandle;
							GridColumn gridColumn = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colunmIndex);
							if (gridColumn == null)
							{
								break;
							}
							gv.SetRowCellValue(num, gridColumn, dataTable.Rows[num2][i]);
							colunmIndex++;
						}
					}
				}
				else
				{
					int focusedRowHandle2 = gv.FocusedRowHandle;
					for (int num3 = focusedRowHandle2; num3 < gv.RowCount; num3++)
					{
						int colIndex = focusedColumn.VisibleIndex;
						bool flag = true;
						for (int num4 = 0; num4 < key.Count; num4++)
						{
							if (string.IsNullOrEmpty(Function.ToString(gv.GetRowCellValue(num3, key[num4]))))
							{
								flag = false;
								break;
							}
						}
						if (!flag || num3 - focusedRowHandle2 >= dataTable.Rows.Count)
						{
							break;
						}
						for (int num5 = 0; num5 < dataTable.Columns.Count; num5++)
						{
							GridColumn gridColumn2 = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colIndex);
							if (gridColumn2 == null)
							{
								break;
							}
							gv.SetRowCellValue(num, gridColumn2, dataTable.Rows[num3 - focusedRowHandle2][num5]);
							colIndex++;
						}
						num++;
					}
					grd.RefreshDataSource();
				}
				Clipboard.Clear();
				gv_Doc.SetFocusedRowCellValue("Locked", "N");
				gv_Doc.SetFocusedRowCellValue("DataSource", "N");
				gv_Doc.SetFocusedRowCellValue("IsEC", "N");
				gv_Doc.SetFocusedRowCellValue("FixdAssts", "N");
				gv_Doc.SetFocusedRowCellValue("CalcMethod", "R");
				gv_Doc.SetFocusedRowCellValue("TaxType", "V");
				gv_Doc.SetFocusedRowCellValue("Correction", "N");
				gv_Doc.SetFocusedRowCellValue("LogInstanc", 0);
				gv_Doc.SetFocusedRowCellValue("TaxCtgr", "E");
				gv_Doc.SetFocusedRowCellValue("IsIGIC", "N");
				gv_Doc.SetFocusedRowCellValue("TaxCtgrBL", "E");
				gv_Doc.SetFocusedRowCellValue("R349Code", 0);
				gv_Doc.SetFocusedRowCellValue("TaxRegion", "PT");
				gv_Doc.SetFocusedRowCellValue("Agent", "N");
				gv_Doc.SetFocusedRowCellValue("Export", "N");
				grd.RefreshDataSource();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.TheTargetTableCannotAddNewRowAutomaticallyContentBelowRowNotCopied, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
	{
		try
		{
			if (msg.Msg == 256 || msg.Msg == 260)
			{
				switch (keyData)
				{
				case Keys.Return:
					bt_OK_Click(null, null);
					break;
				case Keys.Tab:
					EventTabkey();
					break;
				}
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return false;
	}

	private void EventTabkey()
	{
		try
		{
			if (gv_Doc.FocusedColumn.FieldName == "Account" && string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue("Account"))))
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.AcctCode, multiSelect: false, null);
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					AcctCodeImpl acctCodeImpl = (AcctCodeImpl)frm_ChooseFromList2.ChooseItem;
					if (!string.IsNullOrEmpty(acctCodeImpl.AcctCode))
					{
						gv_Doc.SetFocusedRowCellValue("Account", acctCodeImpl.AcctCode);
					}
				}
			}
			if (gv_Doc.FocusedColumn.FieldName == "AcqsTax" && Function.ToString(gv_Doc.GetFocusedRowCellValue("Category")) == "I" && string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue("AcqsTax"))))
			{
				frm_ChooseFromList frm_ChooseFromList3 = new frm_ChooseFromList(Enums.TypeCFL.AcctCode, multiSelect: false, null);
				frm_ChooseFromList3.ShowDialog(this);
				if (frm_ChooseFromList3.ChooseItem != null)
				{
					AcctCodeImpl acctCodeImpl2 = (AcctCodeImpl)frm_ChooseFromList3.ChooseItem;
					if (!string.IsNullOrEmpty(acctCodeImpl2.AcctCode))
					{
						gv_Doc.SetFocusedRowCellValue("AcqsTax", acctCodeImpl2.AcctCode);
					}
				}
			}
			if (gv_Doc.FocusedColumn.FieldName == "DeferrAcc" && string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue("DeferrAcc"))))
			{
				frm_ChooseFromList frm_ChooseFromList4 = new frm_ChooseFromList(Enums.TypeCFL.AcctCode, multiSelect: false, null);
				frm_ChooseFromList4.ShowDialog(this);
				if (frm_ChooseFromList4.ChooseItem != null)
				{
					AcctCodeImpl acctCodeImpl3 = (AcctCodeImpl)frm_ChooseFromList4.ChooseItem;
					if (!string.IsNullOrEmpty(acctCodeImpl3.AcctCode))
					{
						gv_Doc.SetFocusedRowCellValue("DeferrAcc", acctCodeImpl3.AcctCode);
					}
				}
			}
			if (gv_Doc.FocusedColumn.FieldName == "NonDedAcc" && Function.ParseDouble(gv_Doc.GetFocusedRowCellValue("NonDedct")) > 0.0 && string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue("NonDedAcc"))))
			{
				frm_ChooseFromList frm_ChooseFromList5 = new frm_ChooseFromList(Enums.TypeCFL.AcctCode, multiSelect: false, null);
				frm_ChooseFromList5.ShowDialog(this);
				if (frm_ChooseFromList5.ChooseItem != null)
				{
					AcctCodeImpl acctCodeImpl4 = (AcctCodeImpl)frm_ChooseFromList5.ChooseItem;
					if (!string.IsNullOrEmpty(acctCodeImpl4.AcctCode))
					{
						gv_Doc.SetFocusedRowCellValue("NonDedAcc", acctCodeImpl4.AcctCode);
					}
				}
			}
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static List<T> ConvertDataTableToEntity<T>(DataTable dt) where T : new()
	{
		List<T> list = new List<T>();
		try
		{
			List<string> list2 = (from t in (from DataColumn c in dt.Columns
					select c.ColumnName).ToList()
				where t.StartsWith("U_")
				select t).ToList();
			PropertyInfo[] properties = typeof(T).GetProperties();
			for (int num = 0; num < dt.Rows.Count; num++)
			{
				DataRow dataRow = dt.Rows[num];
				T val = new T();
				val = Function.DataRowToEntity<T>(dt.Rows[num]);
				for (int num2 = 0; num2 < dataRow.Table.Columns.Count; num2++)
				{
					string columnName = dataRow.Table.Columns[num2].ColumnName;
					PropertyInfo property = val.GetType().GetProperty(columnName);
					PropertyInfo[] array = properties;
					foreach (PropertyInfo propertyInfo in array)
					{
						if (!(propertyInfo.Name == "UserFields") || list2.Count <= 0)
						{
							continue;
						}
						List<UserFieldsImpl> list3 = new List<UserFieldsImpl>();
						for (int num4 = 0; num4 < list2.ToList().Count; num4++)
						{
							UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
							if (dataRow.Table.Columns[list2.ToList()[num4]].DataType == typeof(DateTime))
							{
								userFieldsImpl.Value = Function.ToString(Function.ParseDateTime(dataRow[list2.ToList()[num4]]));
								userFieldsImpl.Type = BoFieldTypes.db_Date;
							}
							else if (dataRow.Table.Columns[list2.ToList()[num4]].DataType == typeof(double) || dataRow.Table.Columns[list2.ToList()[num4]].DataType == typeof(int) || dataRow.Table.Columns[list2.ToList()[num4]].DataType == typeof(short) || dataRow.Table.Columns[list2.ToList()[num4]].DataType == typeof(decimal))
							{
								userFieldsImpl.Value = Function.ParseDouble(dataRow[list2.ToList()[num4]]);
								userFieldsImpl.Type = BoFieldTypes.db_Numeric;
							}
							else
							{
								userFieldsImpl.Value = Function.ToString(Function.ToString(dataRow[list2.ToList()[num4]]));
								userFieldsImpl.Type = BoFieldTypes.db_Alpha;
							}
							userFieldsImpl.Name = list2.ToList()[num4];
							list3.Add(userFieldsImpl);
						}
						if (list3.Count > 0)
						{
							propertyInfo.SetValue(property, list3);
						}
					}
				}
				list.Add(val);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return list;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SetUp.Tax.frm_TaxGroups));
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gcCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcInactive = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Inactive = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCategory = new Apzon.Controls.Grid.GridColumnEx();
		this.rILUE_Category = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcAcqstnRvrs = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_AcqstnRvrs = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcEffecDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Effective = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcRate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcNonDedct = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Deduct = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcAccount = new Apzon.Controls.Grid.GridColumnEx();
		this.rIBE_Account = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcAcqsTax = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_AcqsTax = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcDeferrAcc = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DeferrAcc = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcNonDedAcc = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_NonDedAcc = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcReportCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gCLocked = new Apzon.Controls.Grid.GridColumnEx();
		this.gCDataSource = new Apzon.Controls.Grid.GridColumnEx();
		this.gCUserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.gCIsEC = new Apzon.Controls.Grid.GridColumnEx();
		this.gcIndicator = new Apzon.Controls.Grid.GridColumnEx();
		this.gcGoddsShip = new Apzon.Controls.Grid.GridColumnEx();
		this.gcEquVatPr = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFixdAssts = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCalcMethod = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTaxType = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFixedAmnt = new Apzon.Controls.Grid.GridColumnEx();
		this.gcExtCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCorrection = new Apzon.Controls.Grid.GridColumnEx();
		this.gcVatCrctn = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRetVatCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRepType = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLogInstanc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUpdateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTaxCtgr = new Apzon.Controls.Grid.GridColumnEx();
		this.gcEquAccount = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUserSign2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcIsIGIC = new Apzon.Controls.Grid.GridColumnEx();
		this.gcServSupply = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTaxCtgrBL = new Apzon.Controls.Grid.GridColumnEx();
		this.gcR349Code = new Apzon.Controls.Grid.GridColumnEx();
		this.gcVatRevAcc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCashDisAcc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDpmTaxOAcc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcVatDedAcc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCstmExpAcc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCstmAlcAcc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTaxRegion = new Apzon.Controls.Grid.GridColumnEx();
		this.gcExemReason = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAgent = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOpCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcExport = new Apzon.Controls.Grid.GridColumnEx();
		this.gcSection = new Apzon.Controls.Grid.GridColumnEx();
		this.bt_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_TaxDefinition = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Inactive).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rILUE_Category).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AcqstnRvrs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Effective).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Effective.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Deduct).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rIBE_Account).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AcqsTax).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DeferrAcc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_NonDedAcc).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Location = new System.Drawing.Point(1, 1);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[10] { this.rep_Inactive, this.rILUE_Category, this.rIBE_Account, this.rep_Effective, this.rep_Deduct, this.rep_AcqsTax, this.rep_DeferrAcc, this.rep_NonDedAcc, this.rep_AcqstnRvrs, this.rCode });
		this.grd_Doc.Size = new System.Drawing.Size(661, 428);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.grd_Doc.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_Doc_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[50]
		{
			this.gcCode, this.gcInactive, this.gcName, this.gcCategory, this.gcAcqstnRvrs, this.gcEffecDate, this.gcRate, this.gcNonDedct, this.gcAccount, this.gcAcqsTax,
			this.gcDeferrAcc, this.gcNonDedAcc, this.gcReportCode, this.gCLocked, this.gCDataSource, this.gCUserSign, this.gCIsEC, this.gcIndicator, this.gcGoddsShip, this.gcEquVatPr,
			this.gcFixdAssts, this.gcCalcMethod, this.gcTaxType, this.gcFixedAmnt, this.gcExtCode, this.gcCorrection, this.gcVatCrctn, this.gcRetVatCode, this.gcRepType, this.gcLogInstanc,
			this.gcUpdateDate, this.gcTaxCtgr, this.gcEquAccount, this.gcUserSign2, this.gcIsIGIC, this.gcServSupply, this.gcTaxCtgrBL, this.gcR349Code, this.gcVatRevAcc, this.gcCashDisAcc,
			this.gcDpmTaxOAcc, this.gcVatDedAcc, this.gcCstmExpAcc, this.gcCstmAlcAcc, this.gcTaxRegion, this.gcExemReason, this.gcAgent, this.gcOpCode, this.gcExport, this.gcSection
		});
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oTaxGroups";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OVTG";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Doc_CustomDrawRowIndicator);
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanging);
		this.gv_Doc.DoubleClick += new System.EventHandler(this.gv_Doc_DoubleClick);
		this.gv_Doc.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Doc_ValidatingEditor);
		this.gv_Doc.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gv_Doc_InvalidValueException);
		this.gcCode.Caption = "Code";
		this.gcCode.ColumnEdit = this.rCode;
		this.gcCode.CurrencyDateField = null;
		this.gcCode.CurrencyField = null;
		this.gcCode.CurrencyFixed = null;
		this.gcCode.CurrencyFixedRate = 0.0;
		this.gcCode.CurrencyRateField = null;
		this.gcCode.FieldName = "Code";
		this.gcCode.Name = "gcCode";
		this.gcCode.PrimaryKey = true;
		this.gcCode.UnitField = null;
		this.gcCode.Visible = true;
		this.gcCode.VisibleIndex = 0;
		this.gcCode.Width = 66;
		this.rCode.AutoHeight = false;
		this.rCode.Name = "rCode";
		this.rCode.EditValueChanged += new System.EventHandler(this.rCode_EditValueChanged);
		this.rCode.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rCode_EditValueChanging);
		this.gcInactive.Caption = "Inactive";
		this.gcInactive.ColumnEdit = this.rep_Inactive;
		this.gcInactive.CurrencyDateField = null;
		this.gcInactive.CurrencyField = null;
		this.gcInactive.CurrencyFixed = null;
		this.gcInactive.CurrencyFixedRate = 0.0;
		this.gcInactive.CurrencyRateField = null;
		this.gcInactive.FieldName = "Inactive";
		this.gcInactive.Name = "gcInactive";
		this.gcInactive.PrimaryKey = false;
		this.gcInactive.UnitField = null;
		this.gcInactive.Visible = true;
		this.gcInactive.VisibleIndex = 1;
		this.gcInactive.Width = 48;
		this.rep_Inactive.AutoHeight = false;
		this.rep_Inactive.Name = "rep_Inactive";
		this.rep_Inactive.ValueChecked = "Y";
		this.rep_Inactive.ValueUnchecked = "N";
		this.gcName.Caption = "Name";
		this.gcName.CurrencyDateField = null;
		this.gcName.CurrencyField = null;
		this.gcName.CurrencyFixed = null;
		this.gcName.CurrencyFixedRate = 0.0;
		this.gcName.CurrencyRateField = null;
		this.gcName.FieldName = "Name";
		this.gcName.Name = "gcName";
		this.gcName.PrimaryKey = false;
		this.gcName.UnitField = null;
		this.gcName.Visible = true;
		this.gcName.VisibleIndex = 2;
		this.gcName.Width = 176;
		this.gcCategory.Caption = "Category";
		this.gcCategory.ColumnEdit = this.rILUE_Category;
		this.gcCategory.CurrencyDateField = null;
		this.gcCategory.CurrencyField = null;
		this.gcCategory.CurrencyFixed = null;
		this.gcCategory.CurrencyFixedRate = 0.0;
		this.gcCategory.CurrencyRateField = null;
		this.gcCategory.FieldName = "Category";
		this.gcCategory.Name = "gcCategory";
		this.gcCategory.PrimaryKey = false;
		this.gcCategory.UnitField = null;
		this.gcCategory.Visible = true;
		this.gcCategory.VisibleIndex = 3;
		this.gcCategory.Width = 67;
		this.rILUE_Category.AutoHeight = false;
		this.rILUE_Category.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rILUE_Category.DisplayMember = "Name";
		this.rILUE_Category.Name = "rILUE_Category";
		this.rILUE_Category.NullText = "";
		this.rILUE_Category.ValueMember = "Code";
		this.rILUE_Category.EditValueChanged += new System.EventHandler(this.rILUE_Category_EditValueChanged);
		this.gcAcqstnRvrs.Caption = "Acquitions/ Reverse";
		this.gcAcqstnRvrs.ColumnEdit = this.rep_AcqstnRvrs;
		this.gcAcqstnRvrs.CurrencyDateField = null;
		this.gcAcqstnRvrs.CurrencyField = null;
		this.gcAcqstnRvrs.CurrencyFixed = null;
		this.gcAcqstnRvrs.CurrencyFixedRate = 0.0;
		this.gcAcqstnRvrs.CurrencyRateField = null;
		this.gcAcqstnRvrs.FieldName = "AcqstnRvrs";
		this.gcAcqstnRvrs.Name = "gcAcqstnRvrs";
		this.gcAcqstnRvrs.PrimaryKey = false;
		this.gcAcqstnRvrs.UnitField = null;
		this.gcAcqstnRvrs.Width = 105;
		this.rep_AcqstnRvrs.AutoHeight = false;
		this.rep_AcqstnRvrs.Name = "rep_AcqstnRvrs";
		this.rep_AcqstnRvrs.ValueChecked = "Y";
		this.rep_AcqstnRvrs.ValueUnchecked = "N";
		this.rep_AcqstnRvrs.EditValueChanged += new System.EventHandler(this.rep_AcqstnRvrs_EditValueChanged);
		this.gcEffecDate.Caption = "Effective from";
		this.gcEffecDate.ColumnEdit = this.rep_Effective;
		this.gcEffecDate.CurrencyDateField = null;
		this.gcEffecDate.CurrencyField = null;
		this.gcEffecDate.CurrencyFixed = null;
		this.gcEffecDate.CurrencyFixedRate = 0.0;
		this.gcEffecDate.CurrencyRateField = null;
		this.gcEffecDate.DisplayFormat.FormatString = "d";
		this.gcEffecDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcEffecDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcEffecDate.FieldName = "EffecDate";
		this.gcEffecDate.Name = "gcEffecDate";
		this.gcEffecDate.PrimaryKey = false;
		this.gcEffecDate.UnitField = null;
		this.gcEffecDate.Visible = true;
		this.gcEffecDate.VisibleIndex = 4;
		this.gcEffecDate.Width = 77;
		this.rep_Effective.AutoHeight = false;
		this.rep_Effective.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Effective.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Effective.Name = "rep_Effective";
		this.gcRate.Caption = "Rate %";
		this.gcRate.CurrencyDateField = null;
		this.gcRate.CurrencyField = null;
		this.gcRate.CurrencyFixed = null;
		this.gcRate.CurrencyFixedRate = 0.0;
		this.gcRate.CurrencyRateField = null;
		this.gcRate.FieldName = "Rate";
		this.gcRate.Name = "gcRate";
		this.gcRate.PrimaryKey = false;
		this.gcRate.UnitField = null;
		this.gcRate.Visible = true;
		this.gcRate.VisibleIndex = 5;
		this.gcRate.Width = 47;
		this.gcNonDedct.Caption = "Non Deduct %";
		this.gcNonDedct.ColumnEdit = this.rep_Deduct;
		this.gcNonDedct.CurrencyDateField = null;
		this.gcNonDedct.CurrencyField = null;
		this.gcNonDedct.CurrencyFixed = null;
		this.gcNonDedct.CurrencyFixedRate = 0.0;
		this.gcNonDedct.CurrencyRateField = null;
		this.gcNonDedct.FieldName = "NonDedct";
		this.gcNonDedct.Name = "gcNonDedct";
		this.gcNonDedct.PrimaryKey = false;
		this.gcNonDedct.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
		this.gcNonDedct.UnitField = null;
		this.gcNonDedct.Width = 79;
		this.rep_Deduct.AutoHeight = false;
		this.rep_Deduct.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Deduct.Name = "rep_Deduct";
		this.rep_Deduct.EditValueChanged += new System.EventHandler(this.rep_Deduct_EditValueChanged);
		this.gcAccount.Caption = "Tax Account";
		this.gcAccount.ColumnEdit = this.rIBE_Account;
		this.gcAccount.CurrencyDateField = null;
		this.gcAccount.CurrencyField = null;
		this.gcAccount.CurrencyFixed = null;
		this.gcAccount.CurrencyFixedRate = 0.0;
		this.gcAccount.CurrencyRateField = null;
		this.gcAccount.FieldName = "Account";
		this.gcAccount.Name = "gcAccount";
		this.gcAccount.PrimaryKey = false;
		this.gcAccount.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
		this.gcAccount.UnitField = null;
		this.gcAccount.Width = 70;
		this.rIBE_Account.AutoHeight = false;
		this.rIBE_Account.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rIBE_Account.Name = "rIBE_Account";
		this.rIBE_Account.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rIBE_Account_ButtonClick);
		this.gcAcqsTax.Caption = "Acquisition Tax Account";
		this.gcAcqsTax.ColumnEdit = this.rep_AcqsTax;
		this.gcAcqsTax.CurrencyDateField = null;
		this.gcAcqsTax.CurrencyField = null;
		this.gcAcqsTax.CurrencyFixed = null;
		this.gcAcqsTax.CurrencyFixedRate = 0.0;
		this.gcAcqsTax.CurrencyRateField = null;
		this.gcAcqsTax.FieldName = "AcqsTax";
		this.gcAcqsTax.Name = "gcAcqsTax";
		this.gcAcqsTax.PrimaryKey = false;
		this.gcAcqsTax.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
		this.gcAcqsTax.UnitField = null;
		this.gcAcqsTax.Width = 124;
		this.rep_AcqsTax.AutoHeight = false;
		this.rep_AcqsTax.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_AcqsTax.Name = "rep_AcqsTax";
		this.rep_AcqsTax.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_AcqsTax_ButtonClick);
		this.gcDeferrAcc.Caption = "Deferred Tax Account";
		this.gcDeferrAcc.ColumnEdit = this.rep_DeferrAcc;
		this.gcDeferrAcc.CurrencyDateField = null;
		this.gcDeferrAcc.CurrencyField = null;
		this.gcDeferrAcc.CurrencyFixed = null;
		this.gcDeferrAcc.CurrencyFixedRate = 0.0;
		this.gcDeferrAcc.CurrencyRateField = null;
		this.gcDeferrAcc.FieldName = "DeferrAcc";
		this.gcDeferrAcc.Name = "gcDeferrAcc";
		this.gcDeferrAcc.PrimaryKey = false;
		this.gcDeferrAcc.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
		this.gcDeferrAcc.UnitField = null;
		this.gcDeferrAcc.Width = 116;
		this.rep_DeferrAcc.AutoHeight = false;
		this.rep_DeferrAcc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_DeferrAcc.Name = "rep_DeferrAcc";
		this.rep_DeferrAcc.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_DeferrAcc_ButtonClick);
		this.gcNonDedAcc.Caption = "Non Deduct. Acct";
		this.gcNonDedAcc.ColumnEdit = this.rep_NonDedAcc;
		this.gcNonDedAcc.CurrencyDateField = null;
		this.gcNonDedAcc.CurrencyField = null;
		this.gcNonDedAcc.CurrencyFixed = null;
		this.gcNonDedAcc.CurrencyFixedRate = 0.0;
		this.gcNonDedAcc.CurrencyRateField = null;
		this.gcNonDedAcc.FieldName = "NonDedAcc";
		this.gcNonDedAcc.Name = "gcNonDedAcc";
		this.gcNonDedAcc.PrimaryKey = false;
		this.gcNonDedAcc.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
		this.gcNonDedAcc.UnitField = null;
		this.gcNonDedAcc.Width = 93;
		this.rep_NonDedAcc.AutoHeight = false;
		this.rep_NonDedAcc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_NonDedAcc.Name = "rep_NonDedAcc";
		this.rep_NonDedAcc.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_NonDedAcc_ButtonClick);
		this.gcReportCode.Caption = "Group Description";
		this.gcReportCode.CurrencyDateField = null;
		this.gcReportCode.CurrencyField = null;
		this.gcReportCode.CurrencyFixed = null;
		this.gcReportCode.CurrencyFixedRate = 0.0;
		this.gcReportCode.CurrencyRateField = null;
		this.gcReportCode.FieldName = "ReportCode";
		this.gcReportCode.Name = "gcReportCode";
		this.gcReportCode.PrimaryKey = false;
		this.gcReportCode.UnitField = null;
		this.gcReportCode.Visible = true;
		this.gcReportCode.VisibleIndex = 6;
		this.gcReportCode.Width = 132;
		this.gCLocked.Caption = "Locked";
		this.gCLocked.CurrencyDateField = null;
		this.gCLocked.CurrencyField = null;
		this.gCLocked.CurrencyFixed = null;
		this.gCLocked.CurrencyFixedRate = 0.0;
		this.gCLocked.CurrencyRateField = null;
		this.gCLocked.FieldName = "Locked";
		this.gCLocked.Name = "gCLocked";
		this.gCLocked.PrimaryKey = false;
		this.gCLocked.UnitField = null;
		this.gCDataSource.Caption = "DataSource";
		this.gCDataSource.CurrencyDateField = null;
		this.gCDataSource.CurrencyField = null;
		this.gCDataSource.CurrencyFixed = null;
		this.gCDataSource.CurrencyFixedRate = 0.0;
		this.gCDataSource.CurrencyRateField = null;
		this.gCDataSource.FieldName = "DataSource";
		this.gCDataSource.Name = "gCDataSource";
		this.gCDataSource.PrimaryKey = false;
		this.gCDataSource.UnitField = null;
		this.gCUserSign.Caption = "UserSign";
		this.gCUserSign.CurrencyDateField = null;
		this.gCUserSign.CurrencyField = null;
		this.gCUserSign.CurrencyFixed = null;
		this.gCUserSign.CurrencyFixedRate = 0.0;
		this.gCUserSign.CurrencyRateField = null;
		this.gCUserSign.FieldName = "UserSign";
		this.gCUserSign.Name = "gCUserSign";
		this.gCUserSign.PrimaryKey = false;
		this.gCUserSign.UnitField = null;
		this.gCIsEC.Caption = "IsEC";
		this.gCIsEC.CurrencyDateField = null;
		this.gCIsEC.CurrencyField = null;
		this.gCIsEC.CurrencyFixed = null;
		this.gCIsEC.CurrencyFixedRate = 0.0;
		this.gCIsEC.CurrencyRateField = null;
		this.gCIsEC.FieldName = "IsEC";
		this.gCIsEC.Name = "gCIsEC";
		this.gCIsEC.PrimaryKey = false;
		this.gCIsEC.UnitField = null;
		this.gcIndicator.Caption = "Indicator";
		this.gcIndicator.CurrencyDateField = null;
		this.gcIndicator.CurrencyField = null;
		this.gcIndicator.CurrencyFixed = null;
		this.gcIndicator.CurrencyFixedRate = 0.0;
		this.gcIndicator.CurrencyRateField = null;
		this.gcIndicator.FieldName = "Indicator";
		this.gcIndicator.Name = "gcIndicator";
		this.gcIndicator.PrimaryKey = false;
		this.gcIndicator.UnitField = null;
		this.gcGoddsShip.Caption = "GoddsShip";
		this.gcGoddsShip.CurrencyDateField = null;
		this.gcGoddsShip.CurrencyField = null;
		this.gcGoddsShip.CurrencyFixed = null;
		this.gcGoddsShip.CurrencyFixedRate = 0.0;
		this.gcGoddsShip.CurrencyRateField = null;
		this.gcGoddsShip.FieldName = "GoddsShip";
		this.gcGoddsShip.Name = "gcGoddsShip";
		this.gcGoddsShip.PrimaryKey = false;
		this.gcGoddsShip.UnitField = null;
		this.gcEquVatPr.Caption = "EquVatPr";
		this.gcEquVatPr.CurrencyDateField = null;
		this.gcEquVatPr.CurrencyField = null;
		this.gcEquVatPr.CurrencyFixed = null;
		this.gcEquVatPr.CurrencyFixedRate = 0.0;
		this.gcEquVatPr.CurrencyRateField = null;
		this.gcEquVatPr.FieldName = "EquVatPr";
		this.gcEquVatPr.Name = "gcEquVatPr";
		this.gcEquVatPr.PrimaryKey = false;
		this.gcEquVatPr.UnitField = null;
		this.gcFixdAssts.Caption = "FixdAssts";
		this.gcFixdAssts.CurrencyDateField = null;
		this.gcFixdAssts.CurrencyField = null;
		this.gcFixdAssts.CurrencyFixed = null;
		this.gcFixdAssts.CurrencyFixedRate = 0.0;
		this.gcFixdAssts.CurrencyRateField = null;
		this.gcFixdAssts.FieldName = "FixdAssts";
		this.gcFixdAssts.Name = "gcFixdAssts";
		this.gcFixdAssts.PrimaryKey = false;
		this.gcFixdAssts.UnitField = null;
		this.gcCalcMethod.Caption = "CalcMethod";
		this.gcCalcMethod.CurrencyDateField = null;
		this.gcCalcMethod.CurrencyField = null;
		this.gcCalcMethod.CurrencyFixed = null;
		this.gcCalcMethod.CurrencyFixedRate = 0.0;
		this.gcCalcMethod.CurrencyRateField = null;
		this.gcCalcMethod.FieldName = "CalcMethod";
		this.gcCalcMethod.Name = "gcCalcMethod";
		this.gcCalcMethod.PrimaryKey = false;
		this.gcCalcMethod.UnitField = null;
		this.gcTaxType.Caption = "TaxType";
		this.gcTaxType.CurrencyDateField = null;
		this.gcTaxType.CurrencyField = null;
		this.gcTaxType.CurrencyFixed = null;
		this.gcTaxType.CurrencyFixedRate = 0.0;
		this.gcTaxType.CurrencyRateField = null;
		this.gcTaxType.FieldName = "TaxType";
		this.gcTaxType.Name = "gcTaxType";
		this.gcTaxType.PrimaryKey = false;
		this.gcTaxType.UnitField = null;
		this.gcFixedAmnt.Caption = "FixedAmnt";
		this.gcFixedAmnt.CurrencyDateField = null;
		this.gcFixedAmnt.CurrencyField = null;
		this.gcFixedAmnt.CurrencyFixed = null;
		this.gcFixedAmnt.CurrencyFixedRate = 0.0;
		this.gcFixedAmnt.CurrencyRateField = null;
		this.gcFixedAmnt.FieldName = "FixedAmnt";
		this.gcFixedAmnt.Name = "gcFixedAmnt";
		this.gcFixedAmnt.PrimaryKey = false;
		this.gcFixedAmnt.UnitField = null;
		this.gcExtCode.Caption = "ExtCode";
		this.gcExtCode.CurrencyDateField = null;
		this.gcExtCode.CurrencyField = null;
		this.gcExtCode.CurrencyFixed = null;
		this.gcExtCode.CurrencyFixedRate = 0.0;
		this.gcExtCode.CurrencyRateField = null;
		this.gcExtCode.FieldName = "ExtCode";
		this.gcExtCode.Name = "gcExtCode";
		this.gcExtCode.PrimaryKey = false;
		this.gcExtCode.UnitField = null;
		this.gcCorrection.Caption = "Correction";
		this.gcCorrection.CurrencyDateField = null;
		this.gcCorrection.CurrencyField = null;
		this.gcCorrection.CurrencyFixed = null;
		this.gcCorrection.CurrencyFixedRate = 0.0;
		this.gcCorrection.CurrencyRateField = null;
		this.gcCorrection.FieldName = "Correction";
		this.gcCorrection.Name = "gcCorrection";
		this.gcCorrection.PrimaryKey = false;
		this.gcCorrection.UnitField = null;
		this.gcVatCrctn.Caption = "VatCrctn";
		this.gcVatCrctn.CurrencyDateField = null;
		this.gcVatCrctn.CurrencyField = null;
		this.gcVatCrctn.CurrencyFixed = null;
		this.gcVatCrctn.CurrencyFixedRate = 0.0;
		this.gcVatCrctn.CurrencyRateField = null;
		this.gcVatCrctn.FieldName = "VatCrctn";
		this.gcVatCrctn.Name = "gcVatCrctn";
		this.gcVatCrctn.PrimaryKey = false;
		this.gcVatCrctn.UnitField = null;
		this.gcRetVatCode.Caption = "RetVatCode";
		this.gcRetVatCode.CurrencyDateField = null;
		this.gcRetVatCode.CurrencyField = null;
		this.gcRetVatCode.CurrencyFixed = null;
		this.gcRetVatCode.CurrencyFixedRate = 0.0;
		this.gcRetVatCode.CurrencyRateField = null;
		this.gcRetVatCode.FieldName = "RetVatCode";
		this.gcRetVatCode.Name = "gcRetVatCode";
		this.gcRetVatCode.PrimaryKey = false;
		this.gcRetVatCode.UnitField = null;
		this.gcRepType.Caption = "RepType";
		this.gcRepType.CurrencyDateField = null;
		this.gcRepType.CurrencyField = null;
		this.gcRepType.CurrencyFixed = null;
		this.gcRepType.CurrencyFixedRate = 0.0;
		this.gcRepType.CurrencyRateField = null;
		this.gcRepType.FieldName = "RepType";
		this.gcRepType.Name = "gcRepType";
		this.gcRepType.PrimaryKey = false;
		this.gcRepType.UnitField = null;
		this.gcLogInstanc.Caption = "LogInstanc";
		this.gcLogInstanc.CurrencyDateField = null;
		this.gcLogInstanc.CurrencyField = null;
		this.gcLogInstanc.CurrencyFixed = null;
		this.gcLogInstanc.CurrencyFixedRate = 0.0;
		this.gcLogInstanc.CurrencyRateField = null;
		this.gcLogInstanc.FieldName = "LogInstanc";
		this.gcLogInstanc.Name = "gcLogInstanc";
		this.gcLogInstanc.PrimaryKey = false;
		this.gcLogInstanc.UnitField = null;
		this.gcUpdateDate.Caption = "UpdateDate";
		this.gcUpdateDate.CurrencyDateField = null;
		this.gcUpdateDate.CurrencyField = null;
		this.gcUpdateDate.CurrencyFixed = null;
		this.gcUpdateDate.CurrencyFixedRate = 0.0;
		this.gcUpdateDate.CurrencyRateField = null;
		this.gcUpdateDate.FieldName = "UpdateDate";
		this.gcUpdateDate.Name = "gcUpdateDate";
		this.gcUpdateDate.PrimaryKey = false;
		this.gcUpdateDate.UnitField = null;
		this.gcTaxCtgr.Caption = "TaxCtgr";
		this.gcTaxCtgr.CurrencyDateField = null;
		this.gcTaxCtgr.CurrencyField = null;
		this.gcTaxCtgr.CurrencyFixed = null;
		this.gcTaxCtgr.CurrencyFixedRate = 0.0;
		this.gcTaxCtgr.CurrencyRateField = null;
		this.gcTaxCtgr.FieldName = "TaxCtgr";
		this.gcTaxCtgr.Name = "gcTaxCtgr";
		this.gcTaxCtgr.PrimaryKey = false;
		this.gcTaxCtgr.UnitField = null;
		this.gcEquAccount.Caption = "EquAccount";
		this.gcEquAccount.CurrencyDateField = null;
		this.gcEquAccount.CurrencyField = null;
		this.gcEquAccount.CurrencyFixed = null;
		this.gcEquAccount.CurrencyFixedRate = 0.0;
		this.gcEquAccount.CurrencyRateField = null;
		this.gcEquAccount.FieldName = "EquAccount";
		this.gcEquAccount.Name = "gcEquAccount";
		this.gcEquAccount.PrimaryKey = false;
		this.gcEquAccount.UnitField = null;
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
		this.gcIsIGIC.Caption = "IsIGIC";
		this.gcIsIGIC.CurrencyDateField = null;
		this.gcIsIGIC.CurrencyField = null;
		this.gcIsIGIC.CurrencyFixed = null;
		this.gcIsIGIC.CurrencyFixedRate = 0.0;
		this.gcIsIGIC.CurrencyRateField = null;
		this.gcIsIGIC.FieldName = "IsIGIC";
		this.gcIsIGIC.Name = "gcIsIGIC";
		this.gcIsIGIC.PrimaryKey = false;
		this.gcIsIGIC.UnitField = null;
		this.gcServSupply.Caption = "ServSupply";
		this.gcServSupply.CurrencyDateField = null;
		this.gcServSupply.CurrencyField = null;
		this.gcServSupply.CurrencyFixed = null;
		this.gcServSupply.CurrencyFixedRate = 0.0;
		this.gcServSupply.CurrencyRateField = null;
		this.gcServSupply.FieldName = "ServSupply";
		this.gcServSupply.Name = "gcServSupply";
		this.gcServSupply.PrimaryKey = false;
		this.gcServSupply.UnitField = null;
		this.gcTaxCtgrBL.Caption = "TaxCtgrBL";
		this.gcTaxCtgrBL.CurrencyDateField = null;
		this.gcTaxCtgrBL.CurrencyField = null;
		this.gcTaxCtgrBL.CurrencyFixed = null;
		this.gcTaxCtgrBL.CurrencyFixedRate = 0.0;
		this.gcTaxCtgrBL.CurrencyRateField = null;
		this.gcTaxCtgrBL.FieldName = "TaxCtgrBL";
		this.gcTaxCtgrBL.Name = "gcTaxCtgrBL";
		this.gcTaxCtgrBL.PrimaryKey = false;
		this.gcTaxCtgrBL.UnitField = null;
		this.gcR349Code.Caption = "R349Code";
		this.gcR349Code.CurrencyDateField = null;
		this.gcR349Code.CurrencyField = null;
		this.gcR349Code.CurrencyFixed = null;
		this.gcR349Code.CurrencyFixedRate = 0.0;
		this.gcR349Code.CurrencyRateField = null;
		this.gcR349Code.FieldName = "R349Code";
		this.gcR349Code.Name = "gcR349Code";
		this.gcR349Code.PrimaryKey = false;
		this.gcR349Code.UnitField = null;
		this.gcVatRevAcc.Caption = "VatRevAcc";
		this.gcVatRevAcc.CurrencyDateField = null;
		this.gcVatRevAcc.CurrencyField = null;
		this.gcVatRevAcc.CurrencyFixed = null;
		this.gcVatRevAcc.CurrencyFixedRate = 0.0;
		this.gcVatRevAcc.CurrencyRateField = null;
		this.gcVatRevAcc.FieldName = "VatRevAcc";
		this.gcVatRevAcc.Name = "gcVatRevAcc";
		this.gcVatRevAcc.PrimaryKey = false;
		this.gcVatRevAcc.UnitField = null;
		this.gcCashDisAcc.Caption = "CashDisAcc";
		this.gcCashDisAcc.CurrencyDateField = null;
		this.gcCashDisAcc.CurrencyField = null;
		this.gcCashDisAcc.CurrencyFixed = null;
		this.gcCashDisAcc.CurrencyFixedRate = 0.0;
		this.gcCashDisAcc.CurrencyRateField = null;
		this.gcCashDisAcc.FieldName = "CashDisAcc";
		this.gcCashDisAcc.Name = "gcCashDisAcc";
		this.gcCashDisAcc.PrimaryKey = false;
		this.gcCashDisAcc.UnitField = null;
		this.gcDpmTaxOAcc.Caption = "DpmTaxOAcc";
		this.gcDpmTaxOAcc.CurrencyDateField = null;
		this.gcDpmTaxOAcc.CurrencyField = null;
		this.gcDpmTaxOAcc.CurrencyFixed = null;
		this.gcDpmTaxOAcc.CurrencyFixedRate = 0.0;
		this.gcDpmTaxOAcc.CurrencyRateField = null;
		this.gcDpmTaxOAcc.FieldName = "DpmTaxOAcc";
		this.gcDpmTaxOAcc.Name = "gcDpmTaxOAcc";
		this.gcDpmTaxOAcc.PrimaryKey = false;
		this.gcDpmTaxOAcc.UnitField = null;
		this.gcVatDedAcc.Caption = "VatDedAcc";
		this.gcVatDedAcc.CurrencyDateField = null;
		this.gcVatDedAcc.CurrencyField = null;
		this.gcVatDedAcc.CurrencyFixed = null;
		this.gcVatDedAcc.CurrencyFixedRate = 0.0;
		this.gcVatDedAcc.CurrencyRateField = null;
		this.gcVatDedAcc.FieldName = "VatDedAcc";
		this.gcVatDedAcc.Name = "gcVatDedAcc";
		this.gcVatDedAcc.PrimaryKey = false;
		this.gcVatDedAcc.UnitField = null;
		this.gcCstmExpAcc.Caption = "CstmExpAcc";
		this.gcCstmExpAcc.CurrencyDateField = null;
		this.gcCstmExpAcc.CurrencyField = null;
		this.gcCstmExpAcc.CurrencyFixed = null;
		this.gcCstmExpAcc.CurrencyFixedRate = 0.0;
		this.gcCstmExpAcc.CurrencyRateField = null;
		this.gcCstmExpAcc.FieldName = "CstmExpAcc";
		this.gcCstmExpAcc.Name = "gcCstmExpAcc";
		this.gcCstmExpAcc.PrimaryKey = false;
		this.gcCstmExpAcc.UnitField = null;
		this.gcCstmAlcAcc.Caption = "CstmAlcAcc";
		this.gcCstmAlcAcc.CurrencyDateField = null;
		this.gcCstmAlcAcc.CurrencyField = null;
		this.gcCstmAlcAcc.CurrencyFixed = null;
		this.gcCstmAlcAcc.CurrencyFixedRate = 0.0;
		this.gcCstmAlcAcc.CurrencyRateField = null;
		this.gcCstmAlcAcc.FieldName = "CstmAlcAcc";
		this.gcCstmAlcAcc.Name = "gcCstmAlcAcc";
		this.gcCstmAlcAcc.PrimaryKey = false;
		this.gcCstmAlcAcc.UnitField = null;
		this.gcTaxRegion.Caption = "TaxRegion";
		this.gcTaxRegion.CurrencyDateField = null;
		this.gcTaxRegion.CurrencyField = null;
		this.gcTaxRegion.CurrencyFixed = null;
		this.gcTaxRegion.CurrencyFixedRate = 0.0;
		this.gcTaxRegion.CurrencyRateField = null;
		this.gcTaxRegion.FieldName = "TaxRegion";
		this.gcTaxRegion.Name = "gcTaxRegion";
		this.gcTaxRegion.PrimaryKey = false;
		this.gcTaxRegion.UnitField = null;
		this.gcExemReason.Caption = "ExemReason";
		this.gcExemReason.CurrencyDateField = null;
		this.gcExemReason.CurrencyField = null;
		this.gcExemReason.CurrencyFixed = null;
		this.gcExemReason.CurrencyFixedRate = 0.0;
		this.gcExemReason.CurrencyRateField = null;
		this.gcExemReason.FieldName = "ExemReason";
		this.gcExemReason.Name = "gcExemReason";
		this.gcExemReason.PrimaryKey = false;
		this.gcExemReason.UnitField = null;
		this.gcAgent.Caption = "Agent";
		this.gcAgent.CurrencyDateField = null;
		this.gcAgent.CurrencyField = null;
		this.gcAgent.CurrencyFixed = null;
		this.gcAgent.CurrencyFixedRate = 0.0;
		this.gcAgent.CurrencyRateField = null;
		this.gcAgent.FieldName = "Agent";
		this.gcAgent.Name = "gcAgent";
		this.gcAgent.PrimaryKey = false;
		this.gcAgent.UnitField = null;
		this.gcOpCode.Caption = "OpCode";
		this.gcOpCode.CurrencyDateField = null;
		this.gcOpCode.CurrencyField = null;
		this.gcOpCode.CurrencyFixed = null;
		this.gcOpCode.CurrencyFixedRate = 0.0;
		this.gcOpCode.CurrencyRateField = null;
		this.gcOpCode.FieldName = "OpCode";
		this.gcOpCode.Name = "gcOpCode";
		this.gcOpCode.PrimaryKey = false;
		this.gcOpCode.UnitField = null;
		this.gcExport.Caption = "Export";
		this.gcExport.CurrencyDateField = null;
		this.gcExport.CurrencyField = null;
		this.gcExport.CurrencyFixed = null;
		this.gcExport.CurrencyFixedRate = 0.0;
		this.gcExport.CurrencyRateField = null;
		this.gcExport.FieldName = "Export";
		this.gcExport.Name = "gcExport";
		this.gcExport.PrimaryKey = false;
		this.gcExport.UnitField = null;
		this.gcSection.Caption = "Section";
		this.gcSection.CurrencyDateField = null;
		this.gcSection.CurrencyField = null;
		this.gcSection.CurrencyFixed = null;
		this.gcSection.CurrencyFixedRate = 0.0;
		this.gcSection.CurrencyRateField = null;
		this.gcSection.FieldName = "Section";
		this.gcSection.Name = "gcSection";
		this.gcSection.PrimaryKey = false;
		this.gcSection.UnitField = null;
		this.bt_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_OK.EditControl = null;
		this.bt_OK.EditValue = "OK";
		this.bt_OK.FieldName = null;
		this.bt_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_OK.ImageOptions.Image");
		this.bt_OK.IsSearchControl = false;
		this.bt_OK.Location = new System.Drawing.Point(2, 433);
		this.bt_OK.Name = "bt_OK";
		this.bt_OK.ReadOnly = false;
		this.bt_OK.SettingInVisible = false;
		this.bt_OK.SettingReadonly = false;
		this.bt_OK.Size = new System.Drawing.Size(99, 20);
		this.bt_OK.TabIndex = 1;
		this.bt_OK.Text = "OK";
		this.bt_OK.Click += new System.EventHandler(this.bt_OK_Click);
		this.bt_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_Cancel.EditControl = null;
		this.bt_Cancel.EditValue = "Cancel";
		this.bt_Cancel.FieldName = null;
		this.bt_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_Cancel.ImageOptions.Image");
		this.bt_Cancel.IsSearchControl = false;
		this.bt_Cancel.Location = new System.Drawing.Point(103, 433);
		this.bt_Cancel.Name = "bt_Cancel";
		this.bt_Cancel.ReadOnly = false;
		this.bt_Cancel.SettingInVisible = false;
		this.bt_Cancel.SettingReadonly = false;
		this.bt_Cancel.Size = new System.Drawing.Size(99, 20);
		this.bt_Cancel.TabIndex = 1;
		this.bt_Cancel.Text = "Cancel";
		this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
		this.bt_TaxDefinition.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.bt_TaxDefinition.EditControl = null;
		this.bt_TaxDefinition.EditValue = "Tax Definition";
		this.bt_TaxDefinition.FieldName = null;
		this.bt_TaxDefinition.ImageOptions.Image = Apzon.Forms.Properties.Resources.withtextwrapping_topright_16x16;
		this.bt_TaxDefinition.IsSearchControl = false;
		this.bt_TaxDefinition.Location = new System.Drawing.Point(513, 433);
		this.bt_TaxDefinition.Name = "bt_TaxDefinition";
		this.bt_TaxDefinition.ReadOnly = false;
		this.bt_TaxDefinition.SettingInVisible = false;
		this.bt_TaxDefinition.SettingReadonly = false;
		this.bt_TaxDefinition.Size = new System.Drawing.Size(149, 20);
		this.bt_TaxDefinition.TabIndex = 1;
		this.bt_TaxDefinition.Text = "Tax Definition";
		this.bt_TaxDefinition.Click += new System.EventHandler(this.bt_TaxDefinition_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(664, 457);
		base.Controls.Add(this.bt_TaxDefinition);
		base.Controls.Add(this.bt_Cancel);
		base.Controls.Add(this.bt_OK);
		base.Controls.Add(this.grd_Doc);
		base.FormId = "oTaxGroups";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_TaxGroups";
		this.Text = "Tax Groups - Setup";
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Inactive).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rILUE_Category).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AcqstnRvrs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Effective.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Effective).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Deduct).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rIBE_Account).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AcqsTax).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DeferrAcc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_NonDedAcc).EndInit();
		base.ResumeLayout(false);
	}
}
