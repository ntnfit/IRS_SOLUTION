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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources;

public class frm_Absence : XtraFormEx
{
	private enum AutoCompleteFlag
	{
		Auto = 1,
		BackToOldText = 2,
		SetEmpty = 3,
		None = 0
	}

	private const string FormController = "Absence";

	private bool isCancel;

	private bool _isLoading;

	private bool isTab;

	private bool isPressed;

	private int lineMax;

	private bool fromCFL;

	private string autoName = "";

	private int autoNumber;

	private string idField = "line";

	private AutoCompleteFlag _autoCompleteFlag;

	private DataTable dtAbsence;

	private DataTable dtOHEM;

	private Dictionary<int, string> dicOHEM;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private Dictionary<int, string> Inputs = new Dictionary<int, string>();

	public Action<DataTable> SendFromChildDelegate;

	private IContainer components;

	private GridControlEx gridControl1;

	private GridViewEx gridView1;

	private GridColumnEx line;

	private GridColumnEx fromDate;

	private GridColumnEx toDate;

	private GridColumnEx reason;

	private GridColumnEx approvedBy;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private RepositoryItemDateEditEx dateRep;

	private RepositoryItemButtonEdit bteApproved;

	private GridColumnEx cnfrmrNum;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btnOK;

	private GridColumnEx gridColumn1;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btnOK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btnOK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_Absence()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			FormId = "oAbsence";
			gridView1.FormId = FormId;
			gridView1.ItemId = "HEM1";
			return;
		}
		throw new Exception(Apzon.Commons.Properties.Resources.Msg_FormAccessDenied);
	}

	public void GetFromParentMethod(DataTable dt)
	{
		try
		{
			_isLoading = true;
			if (dt.IsNotNull())
			{
				foreach (DataRow row in dt.Rows)
				{
					if (Function.ToString(row["cnfrmrNum"]) == "")
					{
						row["cnfrmrNum"] = 0;
					}
				}
				DataView defaultView = dt.DefaultView;
				defaultView.Sort = "fromDate asc";
				dt = defaultView.ToTable();
				lineMax = Function.ParseInt((from s in dt.AsEnumerable()
					where Function.ToString(s.Field<short>("line")) != ""
					select s).Max((DataRow s) => s.Field<short>("line")));
			}
			if (!dt.ExistsColumn("IsNewRow"))
			{
				dt.Columns.Add("IsNewRow", typeof(string));
			}
			foreach (DataRow row2 in dt.Rows)
			{
				row2["IsNewRow"] = "N";
			}
			dtAbsence = dt.Copy();
			gridControl1.DataSource = dt;
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.No)
			{
				_isLoading = true;
				gridControl1.DataSource = dtAbsence.Copy();
				AddNewRow();
				_isLoading = false;
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CheckSaveState()
	{
		if (!_isLoading)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void AddNewRow()
	{
		try
		{
			gridView1.BeginInsertRow();
			DataTable dataTable = gridControl1.DataSource as DataTable;
			DataRow dataRow = dataTable.NewRow();
			dataRow["line"] = 0;
			dataRow["fromDate"] = DBNull.Value;
			dataRow["toDate"] = DBNull.Value;
			dataRow["reason"] = "";
			dataRow["approvedBy"] = "";
			dataRow["cnfrmrNum"] = 0;
			dataRow["IsNewRow"] = "Y";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			gridControl1.DataSource = dataTable;
			gridView1.MakeRowVisible(gridView1.DataRowCount - 1);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gridView1.EndInsertRow();
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		isCancel = true;
		Close();
	}

	private void frm_Absence_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && !isCancel)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
				{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					btnOK_Click(null, null);
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetEmployeeList()
	{
		try
		{
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadEmployees();
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure("OHEM");
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async void frm_Absence_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			_isLoading = true;
			Dictionary<string, GridViewEx> dictionary = new Dictionary<string, GridViewEx> { ["HEM1"] = gridView1 };
			dictionary["HEM1"] = gridView1;
			await WinFuntions.LoadUdfColumns(gridView1, "HEM1", dictionary);
			dtOHEM = await GetEmployeeList();
			dicOHEM = dtOHEM.AsEnumerable().ToDictionary((DataRow s) => Function.ParseInt(s["empID"]), (DataRow s) => s["lastName"]?.ToString() + ", " + s["firstName"]);
			RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
			repositoryItemTextEdit.MaxLength = 100;
			gridView1.Columns["reason"].ColumnEdit = repositoryItemTextEdit;
			gridView1.OptionsBehavior.KeepFocusedRowOnUpdate = true;
			DataTable dataTable = gridControl1.DataSource as DataTable;
			foreach (DataRow item in dataTable.AsEnumerable())
			{
				if (Function.ToString(item["cnfrmrNum"]) != "0")
				{
					Inputs[dataTable.Rows.IndexOf(item)] = "CFL";
				}
			}
			AddNewRow();
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void UpdateFieldList(out List<string> requiredFields, out List<string> normalFields)
	{
		requiredFields = new List<string>();
		requiredFields.Add("fromDate");
		requiredFields.Add("toDate");
		normalFields = new List<string>();
		foreach (GridColumn column in gridView1.Columns)
		{
			if (!requiredFields.Contains(column.FieldName) && column.FieldName != idField)
			{
				normalFields.Add(column.FieldName);
			}
		}
	}

	private bool ValidateField(DataTable dt, List<string> requiredFields, List<string> normalFields, string additionMessage = "")
	{
		try
		{
			string text = "";
			foreach (string requiredField in requiredFields)
			{
				text = text + gridView1.Columns[requiredField].Caption + ", ";
			}
			text = text.Remove(text.LastIndexOf(','), 2);
			text = text + " can't be null. " + additionMessage;
			foreach (DataRow item in dt.AsEnumerable())
			{
				if (dt.Rows[dt.Rows.Count - 1] == item)
				{
					continue;
				}
				bool flag = false;
				foreach (string requiredField2 in requiredFields)
				{
					if (Function.ToString(item[requiredField2]) == "" || Function.ToString(item[requiredField2]) == "0")
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					continue;
				}
				if (Function.ParseInt(item[idField]) == 0)
				{
					bool flag2 = false;
					foreach (string requiredField3 in requiredFields)
					{
						if (Function.ToString(item[requiredField3]) != "" && Function.ToString(item[requiredField3]) != "0")
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						foreach (string normalField in normalFields)
						{
							if (Function.ToString(item[normalField]) != "" && Function.ToString(item[normalField]) != "0")
							{
								flag2 = true;
								break;
							}
						}
					}
					if (flag2)
					{
						WinCommons.Error(Enums.MessageCode.None, text);
						return false;
					}
					continue;
				}
				WinCommons.Error(Enums.MessageCode.None, text);
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return false;
		}
	}

	private void UpdateData()
	{
		try
		{
			gridView1.ClearSorting();
			string requiredField1 = "fromDate";
			string requiredField2 = "toDate";
			DataTable dataTable = gridControl1.DataSource as DataTable;
			dataTable.AcceptChanges();
			List<string> requiredFields = new List<string>();
			List<string> normalFields = new List<string>();
			UpdateFieldList(out requiredFields, out normalFields);
			if (dataTable.IsNotNull())
			{
				if (!ValidateField(dataTable, requiredFields, normalFields))
				{
					return;
				}
				foreach (DataRow item in dataTable.AsEnumerable())
				{
					if (Function.ToString(item["fromDate"]) != "" && Function.ToString(item["toDate"]) != "" && Function.ParseDateTime(item["fromDate"]) > Function.ParseDateTime(item["toDate"]))
					{
						WinCommons.Error(Enums.MessageCode.FromDateMustBeBefore2Date, "");
						return;
					}
				}
				if (Inputs.ContainsValue("User"))
				{
					WinCommons.Error(Enums.MessageCode.PleaseChooseConfirmerFromEmployeeList, "", Function.ToString(Inputs.First((KeyValuePair<int, string> s) => s.Value == "User").Key + 1));
					return;
				}
			}
			DataRow[] array = (from s in dataTable.AsEnumerable()
				where Function.ToString(s.Field<DateTime?>(requiredField1)) != "" && Function.ToString(s.Field<DateTime?>(requiredField2)) != ""
				select s).ToArray();
			if (array.Length == 0)
			{
				gridControl1.DataSource = dataTable.Clone();
				SendFromChildDelegate(dataTable.Clone());
			}
			else
			{
				dataTable = array.CopyToDataTable();
				dataTable.AcceptChanges();
				foreach (DataRow item2 in dataTable.AsEnumerable())
				{
					if (Function.ToString(item2["line"]) == "0" && Function.ToString(item2[requiredField1]) != "" && Function.ToString(item2[requiredField2]) != "")
					{
						item2["line"] = lineMax + 1;
						lineMax++;
					}
					if (Function.ParseInt(item2["cnfrmrNum"]) == 0)
					{
						item2["cnfrmrNum"] = DBNull.Value;
					}
				}
				dataTable.AcceptChanges();
				dtAbsence = dataTable.Copy();
				gridControl1.DataSource = dataTable.Copy();
				if (dataTable.ExistsColumn("IsNewRow"))
				{
					dataTable.Columns.Remove("IsNewRow");
				}
				SendFromChildDelegate(dataTable.Copy());
			}
			FormMode = Enums.FormViewModeType.View;
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnOK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				UpdateData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			string text = Function.ToString(gridView1.GetRowCellValue(e.RowHandle, "IsNewRow"));
			string text2 = Function.ToString(gridView1.GetRowCellValue(e.RowHandle, "fromDate"));
			string text3 = Function.ToString(gridView1.GetRowCellValue(e.RowHandle, "toDate"));
			if (text == "Y" && (e.Column.FieldName == "fromDate" || e.Column.FieldName == "toDate") && text2 != "" && text3 != "")
			{
				gridView1.SetRowCellValue(e.RowHandle, "IsNewRow", "N");
				AddNewRow();
			}
			if (fromCFL || isTab || isPressed)
			{
				return;
			}
			if (_autoCompleteFlag != AutoCompleteFlag.None)
			{
				SetAutoComplete();
				return;
			}
			if (e.Column.FieldName == "approvedBy" && !string.IsNullOrEmpty(Function.ToString(e.Value)))
			{
				FindConfirmerNameCFL();
			}
			if (e.Column.FieldName == "approvedBy" && string.IsNullOrEmpty(Function.ToString(e.Value)))
			{
				Inputs[e.RowHandle] = "Empty";
			}
			CheckSaveState();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridView1_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		if (!_isLoading && !fromCFL && !isTab && !isPressed)
		{
			CheckSaveState();
		}
	}

	private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (Inputs.ContainsKey(gridView1.FocusedRowHandle))
			{
				Inputs.Remove(gridView1.FocusedRowHandle);
			}
			Dictionary<int, string> dictionary = Inputs.ToDictionary((KeyValuePair<int, string> d) => (d.Key >= gridView1.FocusedRowHandle) ? (d.Key - 1) : d.Key, (KeyValuePair<int, string> d) => d.Value);
			Inputs.Clear();
			foreach (KeyValuePair<int, string> item in dictionary)
			{
				Inputs.Add(item.Key, item.Value);
			}
			gridView1.BeginUpdate();
			gridView1.DeleteSelectedRows();
			gridView1.EndUpdate();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void FindConfirmerNameCFL()
	{
		try
		{
			string text = Function.ToString(gridView1.GetFocusedRowCellValue("approvedBy"));
			int num = Function.ParseInt(gridView1.GetFocusedRowCellValue("cnfrmrNum"));
			string value = "";
			if (num != 0)
			{
				value = dicOHEM[num];
			}
			bool flag = text.Equals(value);
			frm_CFL frm_CFL2;
			if (Inputs.ContainsKey(gridView1.FocusedRowHandle))
			{
				if ((Inputs[gridView1.FocusedRowHandle] == "CFL") & flag)
				{
					frm_CFL2 = new frm_CFL(Enums.TypeCFL.EmployeeListForSalesEmpSetup, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
				}
				else
				{
					gridView1.SetFocusedRowCellValue("cnfrmrNum", 0);
					frm_CFL2 = new frm_CFL(Enums.TypeCFL.EmployeeListForSalesEmpSetup, false, text, "", Enums.ItemObjectType.NoItemType, null, "");
				}
			}
			else
			{
				gridView1.SetFocusedRowCellValue("cnfrmrNum", 0);
				frm_CFL2 = new frm_CFL(Enums.TypeCFL.EmployeeListForSalesEmpSetup, false, text, "", Enums.ItemObjectType.NoItemType, null, "");
			}
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				int num2 = Function.ParseInt(frm_CFL2.ChooseItem["empID"]);
				fromCFL = true;
				gridView1.SetFocusedRowCellValue("approvedBy", Function.ToString(frm_CFL2.ChooseItem["lastName"]) + ", " + Function.ToString(frm_CFL2.ChooseItem["firstName"]));
				gridView1.SetFocusedRowCellValue("cnfrmrNum", num2);
				fromCFL = false;
				Inputs[gridView1.FocusedRowHandle] = "CFL";
				return;
			}
			string text2 = Function.ToString(gridView1.GetFocusedRowCellValue("approvedBy"));
			int num3 = Function.ParseInt(gridView1.GetFocusedRowCellValue("cnfrmrNum"));
			if (text2 != "" && num3 == 0)
			{
				Inputs[gridView1.FocusedRowHandle] = "User";
			}
			else if (text2 == "" && num3 == 0)
			{
				Inputs[gridView1.FocusedRowHandle] = "Empty";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bteApproved_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				if (_autoCompleteFlag != AutoCompleteFlag.None)
				{
					SetAutoComplete();
					return;
				}
				isPressed = true;
				gridView1.PostEditor();
				FindConfirmerNameCFL();
				isPressed = false;
			}
			if (e.Button.Index != 1)
			{
				return;
			}
			if (_autoCompleteFlag != AutoCompleteFlag.None)
			{
				SetAutoComplete();
				return;
			}
			string text = Function.ToString(gridView1.GetFocusedRowCellValue("cnfrmrNum"));
			if (!(text == "0"))
			{
				this.DrillDown(ApzObjectTypes.oEmployeesInfo, ApzObjectTypes.oEmployeesInfo, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gridControl1_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				gridView1.PostEditor();
				UpdateData();
			}
			else
			{
				if (e.KeyCode != Keys.Tab || !(gridView1.FocusedColumn.FieldName == "approvedBy") || !(Function.ToString(gridView1.GetFocusedRowCellValue("cnfrmrNum")) == "0"))
				{
					return;
				}
				if (_autoCompleteFlag != AutoCompleteFlag.None)
				{
					SetAutoComplete();
					return;
				}
				isTab = true;
				gridView1.PostEditor();
				if (Function.ToString(gridView1.GetFocusedRowCellValue("approvedBy")) == "")
				{
					Inputs[gridView1.FocusedRowHandle] = "Empty";
				}
				FindConfirmerNameCFL();
				isTab = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetAutoComplete()
	{
		switch (_autoCompleteFlag)
		{
		case AutoCompleteFlag.Auto:
			_isLoading = true;
			gridView1.SetFocusedRowCellValue("approvedBy", autoName);
			gridView1.SetFocusedRowCellValue("cnfrmrNum", autoNumber);
			Inputs[gridView1.FocusedRowHandle] = "CFL";
			_isLoading = false;
			_autoCompleteFlag = AutoCompleteFlag.None;
			break;
		case AutoCompleteFlag.BackToOldText:
		{
			_isLoading = true;
			int key = Function.ParseInt(gridView1.GetFocusedRowCellValue("cnfrmrNum"));
			string value = dicOHEM[key];
			gridView1.SetFocusedRowCellValue("approvedBy", value);
			_isLoading = false;
			_autoCompleteFlag = AutoCompleteFlag.None;
			break;
		}
		case AutoCompleteFlag.SetEmpty:
			_isLoading = true;
			gridView1.SetFocusedRowCellValue("cnfrmrNum", 0);
			Inputs[gridView1.FocusedRowHandle] = "Empty";
			_isLoading = false;
			_autoCompleteFlag = AutoCompleteFlag.None;
			break;
		default:
			_autoCompleteFlag = AutoCompleteFlag.None;
			break;
		}
	}

	private void bteApproved_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gridView1.GetFocusedRowCellValue("cnfrmrNum"));
			string text = "";
			if (num != 0)
			{
				text = dicOHEM[num];
			}
			DataRow[] array = (from s in dtOHEM.AsEnumerable()
				where Function.ToString(s["lastName"]).ToLower().Contains(Function.ToString(e.NewValue).ToLower()) && Function.ToString(e.NewValue) != ""
				select s).ToArray();
			switch (array.Length)
			{
			case 0:
				if (num != 0)
				{
					if (Function.ToString(e.NewValue) == "")
					{
						_autoCompleteFlag = AutoCompleteFlag.SetEmpty;
						break;
					}
					if (text.Contains(Function.ToString(e.NewValue)))
					{
						_autoCompleteFlag = AutoCompleteFlag.BackToOldText;
						break;
					}
				}
				_autoCompleteFlag = AutoCompleteFlag.None;
				break;
			case 1:
				_autoCompleteFlag = AutoCompleteFlag.Auto;
				autoNumber = Function.ParseInt(array[0]["empID"]);
				autoName = dicOHEM[autoNumber];
				break;
			default:
				_autoCompleteFlag = AutoCompleteFlag.None;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_Absence));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.gridControl1 = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.line = new Apzon.Controls.Grid.GridColumnEx();
		this.fromDate = new Apzon.Controls.Grid.GridColumnEx();
		this.dateRep = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.toDate = new Apzon.Controls.Grid.GridColumnEx();
		this.reason = new Apzon.Controls.Grid.GridColumnEx();
		this.approvedBy = new Apzon.Controls.Grid.GridColumnEx();
		this.bteApproved = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.cnfrmrNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateRep).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateRep.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bteApproved).BeginInit();
		base.SuspendLayout();
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(81, 350);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 14;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.EditControl = null;
		this.btnOK.EditValue = "OK";
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(3, 350);
		this.btnOK.Name = "btnOK";
		this.btnOK.ReadOnly = false;
		this.btnOK.SettingInVisible = false;
		this.btnOK.SettingReadonly = false;
		this.btnOK.Size = new System.Drawing.Size(75, 23);
		this.btnOK.TabIndex = 13;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		this.gridControl1.AllowPasteExcel = true;
		this.gridControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControl1.ClipboardData = null;
		this.gridControl1.ContextMenuStrip = this.contextMenuStrip1;
		this.gridControl1.Location = new System.Drawing.Point(3, 3);
		this.gridControl1.MainView = this.gridView1;
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.dateRep, this.bteApproved });
		this.gridControl1.Size = new System.Drawing.Size(602, 343);
		this.gridControl1.TabIndex = 12;
		this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView1 });
		this.gridControl1.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.gridControl1_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(144, 26);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = null;
		this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.line, this.fromDate, this.toDate, this.reason, this.approvedBy, this.cnfrmrNum, this.gridColumn1 });
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = "FormId";
		this.gridView1.GridControl = this.gridControl1;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 30;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = "HEM1";
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.OptionsEditForm.ShowOnEnterKey = DevExpress.Utils.DefaultBoolean.False;
		this.gridView1.OptionsView.ShowGroupPanel = false;
		this.gridView1.UseBatchMod = false;
		this.gridView1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator);
		this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
		this.gridView1.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanging);
		this.line.Caption = "line";
		this.line.CurrencyDateField = null;
		this.line.CurrencyField = null;
		this.line.CurrencyFixed = null;
		this.line.CurrencyFixedRate = 0.0;
		this.line.CurrencyRateField = null;
		this.line.FieldName = "line";
		this.line.Name = "line";
		this.line.PrimaryKey = false;
		this.line.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
		this.line.UnitField = null;
		this.line.Width = 21;
		this.fromDate.Caption = "From Date";
		this.fromDate.ColumnEdit = this.dateRep;
		this.fromDate.CurrencyDateField = null;
		this.fromDate.CurrencyField = null;
		this.fromDate.CurrencyFixed = null;
		this.fromDate.CurrencyFixedRate = 0.0;
		this.fromDate.CurrencyRateField = null;
		this.fromDate.FieldName = "fromDate";
		this.fromDate.Name = "fromDate";
		this.fromDate.PrimaryKey = false;
		this.fromDate.UnitField = null;
		this.fromDate.Visible = true;
		this.fromDate.VisibleIndex = 0;
		this.fromDate.Width = 73;
		this.dateRep.AutoHeight = false;
		this.dateRep.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateRep.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateRep.IsSearchControl = false;
		this.dateRep.Name = "dateRep";
		this.toDate.Caption = "To Date";
		this.toDate.ColumnEdit = this.dateRep;
		this.toDate.CurrencyDateField = null;
		this.toDate.CurrencyField = null;
		this.toDate.CurrencyFixed = null;
		this.toDate.CurrencyFixedRate = 0.0;
		this.toDate.CurrencyRateField = null;
		this.toDate.FieldName = "toDate";
		this.toDate.Name = "toDate";
		this.toDate.PrimaryKey = false;
		this.toDate.UnitField = null;
		this.toDate.Visible = true;
		this.toDate.VisibleIndex = 1;
		this.toDate.Width = 69;
		this.reason.Caption = "Reason";
		this.reason.CurrencyDateField = null;
		this.reason.CurrencyField = null;
		this.reason.CurrencyFixed = null;
		this.reason.CurrencyFixedRate = 0.0;
		this.reason.CurrencyRateField = null;
		this.reason.FieldName = "reason";
		this.reason.Name = "reason";
		this.reason.PrimaryKey = false;
		this.reason.UnitField = null;
		this.reason.Visible = true;
		this.reason.VisibleIndex = 2;
		this.reason.Width = 97;
		this.approvedBy.Caption = "Approved By";
		this.approvedBy.ColumnEdit = this.bteApproved;
		this.approvedBy.CurrencyDateField = null;
		this.approvedBy.CurrencyField = null;
		this.approvedBy.CurrencyFixed = null;
		this.approvedBy.CurrencyFixedRate = 0.0;
		this.approvedBy.CurrencyRateField = null;
		this.approvedBy.FieldName = "approvedBy";
		this.approvedBy.Name = "approvedBy";
		this.approvedBy.PrimaryKey = false;
		this.approvedBy.UnitField = null;
		this.approvedBy.Visible = true;
		this.approvedBy.VisibleIndex = 3;
		this.bteApproved.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.bteApproved.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bteApproved.Name = "bteApproved";
		this.bteApproved.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bteApproved_ButtonPressed);
		this.bteApproved.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.bteApproved_EditValueChanging);
		this.cnfrmrNum.Caption = "Confirmer Number";
		this.cnfrmrNum.CurrencyDateField = null;
		this.cnfrmrNum.CurrencyField = null;
		this.cnfrmrNum.CurrencyFixed = null;
		this.cnfrmrNum.CurrencyFixedRate = 0.0;
		this.cnfrmrNum.CurrencyRateField = null;
		this.cnfrmrNum.FieldName = "cnfrmrNum";
		this.cnfrmrNum.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.cnfrmrNum.Name = "cnfrmrNum";
		this.cnfrmrNum.PrimaryKey = false;
		this.cnfrmrNum.UnitField = null;
		this.gridColumn1.Caption = "gridColumn1";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "IsNewRow";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		base.AcceptButton = this.btnOK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(608, 378);
		this.ContextMenuStrip = this.contextMenuStrip1;
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.gridControl1);
		base.FormId = "FormId";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Absence";
		base.ShowInTaskbar = false;
		this.Text = "Absence";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Absence_FormClosing);
		base.Load += new System.EventHandler(this.frm_Absence_Load);
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateRep.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateRep).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bteApproved).EndInit();
		base.ResumeLayout(false);
	}
}
