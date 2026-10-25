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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources;

public class frm_JVQuestion : XtraFormEx
{
	private List<int> changedCells = new List<int>();

	private const string FormController = "JVQuestion";

	private bool isLoading;

	private bool isCancel;

	private bool isInserted;

	private bool isSorting;

	private bool isUpdating;

	private bool isDuplicate;

	private bool userClick;

	private bool isChangingRow;

	private int focusedRow;

	private DataTable _dtAnswer;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private int idMax;

	public Action<int> childSend;

	private IContainer components;

	private GridControlEx gridControl1;

	private GridViewEx gridView1;

	private GridColumnEx QuesCode;

	private GridColumnEx name;

	private GridColumnEx descriptio;

	private GridColumnEx QuesGroup;

	private RepositoryItemLookUpEditDefineNew quesGroupRep;

	private RepositoryItemTextEdit repCode;

	private RepositoryItemTextEdit repName;

	private GridColumnEx gridColumn5;

	private GridControlEx grd_Answer;

	private GridViewEx gv_Answer;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private RepositoryItemLookUpEdit answerType_Rep;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private RepositoryItemCheckEdit repositoryItemCheckEdit2;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem toolStripMenuItem1;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btnOK;

	private GridColumnEx gridColumn8;

	private RadioGroupEx radioGroup1;

	private LabelControl labelControl1;

	private GridColumnEx gridColumn9;

	private ToolStripMenuItem duplicateToolStripMenuItem;

	private GridColumnEx gridColumn10;

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
			btnOK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btnOK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_JVQuestion()
	{
		try
		{
			if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_JobVacancy"))
			{
				InitializeComponent();
				if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_JobVacancy" && x.authoriza == "F"))
				{
					WinFuntions.DisableAllControl(Controls);
					btCancel.Enabled = true;
				}
				quesGroupRep.Columns.Add(new LookUpColumnInfo("Id", 0, ""));
				quesGroupRep.Columns.Add(new LookUpColumnInfo("Name", 150, ""));
				quesGroupRep.NullText = "";
				quesGroupRep.ValueMember = "Id";
				quesGroupRep.DisplayMember = "Name";
				quesGroupRep.Columns["Id"].Visible = false;
				quesGroupRep.ShowHeader = false;
				quesGroupRep.FormProperties.TableName = "APZ_OQLG";
				FormId = "oJVQuestion";
				GridViewEx gridViewEx = gridView1;
				string formId = (gv_Answer.ItemId = FormId);
				gridViewEx.FormId = formId;
				gridView1.ItemId = "APZ_OQLQ";
				return;
			}
			throw new Exception(Resources.Msg_FormAccessDenied);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadQuestionGroup()
	{
		try
		{
			DataTable dataTable = _httpClient.JVQuestion.LoadQuestionGroup();
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure("APZ_OQLG");
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["Id"] = -1;
			dataRow["Name"] = "";
			dataTable.Rows.InsertAt(dataRow, 0);
			dataTable.AcceptChanges();
			quesGroupRep.DropDownRows = dataTable.Rows.Count;
			quesGroupRep.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadAnswer(int _id)
	{
		try
		{
			_dtAnswer = await _httpClient.JVQuestion.LoadAnswer(_id);
			if (!_dtAnswer.IsNotNull())
			{
				_dtAnswer = WinFuntions.GetTableStructure("APZ_OQLA");
			}
			if (!_dtAnswer.ExistsColumn("IsNewRow"))
			{
				_dtAnswer.Columns.Add("IsNewRow", typeof(string));
			}
			foreach (DataRow row in _dtAnswer.Rows)
			{
				row["IsNewRow"] = "N";
			}
			grd_Answer.DataSource = _dtAnswer.Copy();
			AddNewAnswer();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData()
	{
		try
		{
			int id = 0;
			DataTable dataTable = _httpClient.JVQuestion.LoadQuestion();
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure("APZ_OQLQ");
			}
			else
			{
				id = Function.ParseInt(dataTable.Rows[0]["QuesCode"]);
			}
			if (!dataTable.ExistsColumn("IsDuplicate"))
			{
				dataTable.Columns.Add("IsDuplicate", typeof(string));
			}
			gridControl1.DataSource = dataTable;
			dataTable.AcceptChanges();
			await LoadAnswer(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void UpdateData()
	{
		try
		{
			gv_Answer.ClearSorting();
			isUpdating = true;
			DataTable dataTable = (gridControl1.DataSource as DataTable).Clone();
			DataRow dataRow = dataTable.NewRow();
			dataRow["QuesCode"] = Function.ParseInt(gridView1.GetFocusedRowCellValue("QuesCode"));
			dataRow["Content"] = Function.ToString(gridView1.GetFocusedRowCellValue("Content"));
			dataRow["AnsType"] = Function.ToString(gridView1.GetFocusedRowCellValue("AnsType"));
			dataRow["QuesGroup"] = Function.ParseInt(gridView1.GetFocusedRowCellValue("QuesGroup"));
			dataRow["Note"] = Function.ToString(gridView1.GetFocusedRowCellValue("Note"));
			dataRow["AutoLabel"] = Function.ToString(radioGroup1.EditValue);
			dataRow["IsDuplicate"] = Function.ToString(gridView1.GetFocusedRowCellValue("IsDuplicate"));
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			if (dataTable.IsNotNull())
			{
				foreach (DataRow item in dataTable.AsEnumerable())
				{
					if (Function.ToString(item["Content"]) == "" || Function.ToString(item["QuesGroup"]) == "-1" || Function.ToString(item["QuesGroup"]) == "")
					{
						if (!(Function.ToString(item["QuesCode"]) == "") && !(Function.ToString(item["QuesCode"]) == "0"))
						{
							WinCommons.Error(Enums.MessageCode.QuestionContentOrQuestionGroupFieldCannotBeNull, "");
							return;
						}
						if (Function.ToString(item["Content"]) != "" || Function.ToString(item["QuesGroup"]) != "" || Function.ToString(item["AnsType"]) != "F" || Function.ToString(item["Note"]) != "")
						{
							WinCommons.Error(Enums.MessageCode.QuestionContentOrQuestionGroupFieldCannotBeNull, "");
							return;
						}
					}
				}
			}
			DataTable dataTable2 = grd_Answer.DataSource as DataTable;
			dataTable2.AcceptChanges();
			if (dataTable.IsNotNull())
			{
				foreach (DataRow item2 in dataTable2.AsEnumerable())
				{
					if (Function.ToString(item2["Content"]) == "")
					{
						if (!(Function.ToString(item2["IsNewRow"]) == "Y"))
						{
							WinCommons.Error(Enums.MessageCode.AnswerContentFieldCannotBeNull, "");
							return;
						}
						if (Function.ToString(item2["Correct"]) != "N" || Function.ToString(item2["Note"]) != "")
						{
							WinCommons.Error(Enums.MessageCode.AnswerContentFieldCannotBeNull, "");
							return;
						}
					}
				}
			}
			WinCommons._OpenProcessing();
			await SaveData(dataTable, dataTable2);
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			isUpdating = false;
		}
	}

	private async Task<string> SaveData(DataTable dtNew, DataTable dtA)
	{
		_ = 1;
		try
		{
			string requiredField = "Content";
			DataRow[] array = (from s in dtNew.AsEnumerable()
				where Function.ToString(s.Field<string>(requiredField)) != ""
				select s).ToArray();
			if (array.Length == 0)
			{
				gridControl1.DataSource = dtNew.Clone();
				dtNew = dtNew.Clone();
				dtNew.AcceptChanges();
			}
			else
			{
				dtNew = array.CopyToDataTable();
				dtNew.AcceptChanges();
			}
			if (dtA.IsNotNull() && Function.ToString(dtNew.Rows[0]["IsDuplicate"]) == "Y")
			{
				foreach (DataRow item in dtA.AsEnumerable())
				{
					item["AnsCode"] = 0;
				}
				dtA.AcceptChanges();
			}
			DataRow[] array2 = (from s in dtA.AsEnumerable()
				where Function.ToString(s.Field<string>(requiredField)) != ""
				select s).ToArray();
			if (array2.Length == 0)
			{
				grd_Answer.DataSource = dtA.Clone();
				dtA = dtA.Clone();
				dtA.AcceptChanges();
			}
			else
			{
				dtA = array2.CopyToDataTable();
				dtA.AcceptChanges();
			}
			if (dtA.ExistsColumn("IsNewRow"))
			{
				dtA.Columns.Remove("IsNewRow");
			}
			dtA.AcceptChanges();
			if (dtNew.ExistsColumn("IsDuplicate"))
			{
				dtNew.Columns.Remove("IsDuplicate");
			}
			dtNew.TableName = "APZ_OQLQ";
			new Tuple<DataTable, string>(dtNew, dtNew.TableName);
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dtNew);
			dataSet.Tables.Add(dtA);
			using (HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "JVAnswer"))
			{
				HttpResult httpResult = await helper.PostAsync("SaveQuestion", dataSet);
				if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
				{
					FormMode = Enums.FormViewModeType.View;
					int num = Function.ParseInt(httpResult.Content);
					foreach (DataRow item2 in (gridControl1.DataSource as DataTable).AsEnumerable())
					{
						if (Function.ParseInt(item2["QuesCode"]) == 0 && Function.ToString(item2[requiredField]) != "")
						{
							item2["QuesCode"] = num;
						}
					}
					await LoadAnswer(num);
					AddNewRow();
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					return "success";
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return "";
		}
	}

	private async void btnOK_Click(object sender, EventArgs e)
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		isCancel = true;
		Close();
	}

	private void frm_JVInterviewStatus_FormClosing(object sender, FormClosingEventArgs e)
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

	private void CheckSaveState()
	{
		if (!isLoading && !isDuplicate)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void SetupNewRow(ref DataRow dr)
	{
		try
		{
			foreach (GridColumn column in gridView1.Columns)
			{
				switch (column.ColumnType.ToString().ToLower())
				{
				case "system.string":
					dr[column.FieldName] = "";
					break;
				case "system.datetime":
					dr[column.FieldName] = DBNull.Value;
					break;
				case "system.decimal":
				case "system.double":
				case "system.int16":
				case "system.int32":
				case "system.int64":
					dr[column.FieldName] = DBNull.Value;
					break;
				default:
					dr[column.FieldName] = DBNull.Value;
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

	private void AddNewRow()
	{
		try
		{
			gridView1.BeginInsertRow();
			if (gridView1.DataRowCount <= 0 || !(Function.ToString(gridView1.GetRowCellValue(gridView1.DataRowCount - 1, "Content")) == ""))
			{
				DataTable dataTable = gridControl1.DataSource as DataTable;
				DataRow dr = dataTable.NewRow();
				SetupNewRow(ref dr);
				dr["QuesCode"] = DBNull.Value;
				dr["Content"] = "";
				dr["AnsType"] = "F";
				dr["QuesGroup"] = -1;
				dr["Note"] = DBNull.Value;
				dr["AutoLabel"] = "O";
				dataTable.Rows.Add(dr);
				dataTable.AcceptChanges();
				gridControl1.DataSource = dataTable;
				if (gridView1.RowCount > 0)
				{
					gridView1.MakeRowVisible(gridView1.RowCount - 1);
				}
			}
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

	private void DuplicateRow()
	{
		try
		{
			if (gridView1.DataRowCount < 0 || Function.ParseInt(gridView1.GetFocusedRowCellValue("QuesCode")) == 0)
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.Update)
			{
				WinCommons.Warning(Enums.MessageCode.DataMustBeSaveBeforeDuplicate, "");
				return;
			}
			DataTable dataTable = gridControl1.DataSource as DataTable;
			DataRow dataRow = dataTable.NewRow();
			dataRow["QuesCode"] = DBNull.Value;
			dataRow["Content"] = Function.ToString(gridView1.GetFocusedRowCellValue("Content"));
			dataRow["AnsType"] = Function.ToString(gridView1.GetFocusedRowCellValue("AnsType"));
			dataRow["QuesGroup"] = Function.ToString(gridView1.GetFocusedRowCellValue("QuesGroup"));
			dataRow["Note"] = Function.ToString(gridView1.GetFocusedRowCellValue("Note"));
			dataRow["AutoLabel"] = Function.ToString(radioGroup1.EditValue);
			dataRow["IsDuplicate"] = "Y";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				if (Function.ToString(dataTable.Rows[i]["Content"]) == "" && Function.ParseInt(dataTable.Rows[i]["QuesCode"]) == 0)
				{
					dataTable.Rows.RemoveAt(i);
					break;
				}
			}
			int num = gridView1.LocateByValue("IsDuplicate", "Y");
			gridView1.SetRowCellValue(num, "IsDuplicate", "N");
			gridView1.FocusedRowHandle = num;
			UpdateData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewAnswer()
	{
		try
		{
			if (gv_Answer.DataRowCount <= 0 || !(Function.ToString(gv_Answer.GetRowCellValue(gv_Answer.DataRowCount - 1, "Content")) == ""))
			{
				DataTable dataTable = grd_Answer.DataSource as DataTable;
				DataRow dataRow = dataTable.NewRow();
				dataRow["AnsCode"] = DBNull.Value;
				dataRow["QuesCode"] = DBNull.Value;
				string text = "";
				if (radioGroup1.Enabled)
				{
					string text2 = Function.ToString(radioGroup1.EditValue);
					if (text2 == "A")
					{
						text = Function.ToString(Convert.ToChar(gv_Answer.DataRowCount + 65));
					}
					else
					{
						text = ((!(text2 == "N")) ? "" : Function.ToString(gv_Answer.DataRowCount + 1));
					}
				}
				dataRow["AnsLabel"] = text.ToString();
				dataRow["Correct"] = "N";
				dataRow["Content"] = "";
				dataRow["Note"] = "";
				dataRow["IsNewRow"] = "Y";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_Answer.DataSource = dataTable;
				if (gv_Answer.RowCount > 0)
				{
					gv_Answer.MakeRowVisible(gv_Answer.RowCount - 1);
				}
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
		if (!isLoading)
		{
			if (Function.ToString(gridView1.GetRowCellValue(e.RowHandle, "Content")) != "")
			{
				changedCells.Add(e.RowHandle);
			}
			CheckSaveState();
		}
	}

	private void gridView1_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		CheckSaveState();
	}

	private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.No)
			{
				isLoading = true;
				await LoadData();
				AddNewRow();
				isLoading = false;
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AnswerTypeLookUp()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = "F";
			dataRow["Name"] = "Free";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "N";
			dataRow["Name"] = "Normal";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "M";
			dataRow["Name"] = "Multichoice";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			answerType_Rep.DropDownRows = dataTable.Rows.Count;
			answerType_Rep.DataSource = dataTable;
			answerType_Rep.BestFitMode = BestFitMode.BestFitResizePopup;
			answerType_Rep.NullText = "";
			answerType_Rep.ValueMember = "Code";
			answerType_Rep.DisplayMember = "Name";
			answerType_Rep.Columns["Code"].Visible = false;
			answerType_Rep.ShowHeader = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_JVInterviewStatus_Load(object sender, EventArgs e)
	{
		try
		{
			AnswerTypeLookUp();
			RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
			repositoryItemTextEdit.MaxLength = 20;
			gv_Answer.Columns["AnsLabel"].ColumnEdit = repositoryItemTextEdit;
			gridView1.OptionsBehavior.KeepFocusedRowOnUpdate = true;
			isLoading = true;
			quesGroupRep.RefreshDataSource();
			await LoadData();
			LoadQuestionGroup();
			AddNewRow();
			AddNewAnswer();
			SetRadioEnable();
			SetRadioValue();
			SetLabel();
			isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetRadioValue()
	{
		radioGroup1.EditValue = Function.ToString(gridView1.GetFocusedRowCellValue("AutoLabel"));
	}

	private void SetRadioEnable()
	{
		if (Function.ToString(gridView1.GetFocusedRowCellValue("AnsType")) == "M")
		{
			radioGroup1.Enabled = true;
		}
		else
		{
			radioGroup1.Enabled = false;
		}
	}

	private DataTable CheckExist(int ID, string checkTable, string checkField = "Id")
	{
		try
		{
			DataTable dataTable = _httpClient.JVQuestion.CheckExist(new Tuple<int, string, string>(ID, checkTable, checkField));
			if (dataTable.IsNotNull())
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private void DeleteSelect()
	{
		gridView1.BeginUpdate();
		gridView1.DeleteSelectedRows();
		gridView1.EndUpdate();
		gridControl1.RefreshDataSource();
		FormMode = Enums.FormViewModeType.View;
	}

	private async void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			isUpdating = true;
			if (gridView1.DataRowCount <= 0 || !(gridControl1.DataSource as DataTable).IsNotNull() || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
			{
				return;
			}
			int num = Function.ParseInt(gridView1.GetFocusedRowCellValue("QuesCode"));
			if (num > 0)
			{
				string text = await _httpClient.JVQuestion.DeleteQuestion(num);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					return;
				}
			}
			DeleteSelect();
			await LoadData();
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			isUpdating = false;
		}
	}

	private void grd_Answer_Click(object sender, EventArgs e)
	{
	}

	private void answerType_Rep_EditValueChanging(object sender, ChangingEventArgs e)
	{
		if (Function.ToString(e.NewValue) == "M")
		{
			radioGroup1.Enabled = true;
			if (Function.ToString(gridView1.GetFocusedRowCellValue("QuesCode")) == "")
			{
				radioGroup1.EditValue = "A";
			}
		}
		else
		{
			radioGroup1.Enabled = false;
		}
		SetLabel();
	}

	private async void gridView1_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			if (!isLoading && !isDuplicate && !isUpdating && !isChangingRow)
			{
				isChangingRow = true;
				int id = Function.ParseInt(gridView1.GetFocusedRowCellValue("QuesCode"));
				await LoadAnswer(id);
				SetRadioEnable();
				SetRadioValue();
				SetLabel();
				isChangingRow = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Answer_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (!isLoading)
		{
			string text = Function.ToString(gv_Answer.GetFocusedRowCellValue("IsNewRow"));
			Function.ParseInt(gv_Answer.GetFocusedRowCellValue("AnsCode"));
			string text2 = Function.ToString(gv_Answer.GetFocusedRowCellValue("Content"));
			if (text == "Y" && text2 != "" && e.Column.FieldName == "Content")
			{
				gv_Answer.SetFocusedRowCellValue("IsNewRow", "N");
				AddNewAnswer();
			}
		}
	}

	private void gv_Answer_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		CheckSaveState();
	}

	private void gv_Answer_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = Function.ToString(e.RowHandle + 1);
		}
	}

	private void gridView1_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		if (isLoading || isSorting || isUpdating || FormMode != Enums.FormViewModeType.Update)
		{
			return;
		}
		switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
		{
		case DialogResult.Yes:
			e.Allow = true;
			if (gridControl1.DataSource is DataTable dataTable)
			{
				dataTable.RejectChanges();
			}
			FormMode = Enums.FormViewModeType.View;
			break;
		case DialogResult.No:
			e.Allow = false;
			break;
		}
	}

	private void gridView1_StartSorting(object sender, EventArgs e)
	{
		isSorting = true;
	}

	private void gridView1_EndSorting(object sender, EventArgs e)
	{
		if (FormMode != Enums.FormViewModeType.View)
		{
			gridView1.ClearSorting();
		}
		isSorting = false;
	}

	private void toolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			isUpdating = true;
			if (gv_Answer.DataRowCount > 0 && (grd_Answer.DataSource as DataTable).IsNotNull())
			{
				gv_Answer.BeginUpdate();
				gv_Answer.DeleteSelectedRows();
				gv_Answer.EndUpdate();
				grd_Answer.RefreshDataSource();
				FormMode = Enums.FormViewModeType.Update;
				isUpdating = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridControl1_EditorKeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			gridView1.PostEditor();
			UpdateData();
		}
	}

	private void grd_Answer_EditorKeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			gv_Answer.PostEditor();
			UpdateData();
		}
	}

	private void SetLabel()
	{
		try
		{
			DataTable dataTable = grd_Answer.DataSource as DataTable;
			string text = "";
			if (radioGroup1.Enabled)
			{
				switch (Function.ToString(radioGroup1.EditValue))
				{
				case "A":
				{
					int num2 = 0;
					foreach (DataRow item in dataTable.AsEnumerable())
					{
						text = Function.ToString(Convert.ToChar(num2 + 65));
						item["AnsLabel"] = text.ToString();
						num2++;
					}
					break;
				}
				case "N":
				{
					int num = 0;
					foreach (DataRow item2 in dataTable.AsEnumerable())
					{
						text = Function.ToString(num + 1);
						item2["AnsLabel"] = text.ToString();
						num++;
					}
					break;
				}
				case "O":
					foreach (DataRow item3 in dataTable.AsEnumerable())
					{
						string text2 = Function.ToString(item3["AnsLabel"]);
						if (text2.Length == 1 && (char.IsDigit(text2[0]) || char.IsLetter(text2[0])))
						{
							item3["AnsLabel"] = "";
						}
					}
					break;
				}
				dataTable.AcceptChanges();
				grd_Answer.DataSource = dataTable;
				return;
			}
			foreach (DataRow item4 in dataTable.AsEnumerable())
			{
				string text3 = Function.ToString(item4["AnsLabel"]);
				if (text3.Length == 1 && (char.IsDigit(text3[0]) || char.IsLetter(text3[0])))
				{
					item4["AnsLabel"] = "";
				}
			}
			dataTable.AcceptChanges();
			grd_Answer.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void radioGroup1_SelectedIndexChanged(object sender, EventArgs e)
	{
		SetLabel();
		if (userClick)
		{
			FormMode = Enums.FormViewModeType.Update;
			userClick = false;
		}
	}

	private void radioGroup1_MouseDown(object sender, MouseEventArgs e)
	{
		userClick = true;
	}

	private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		isDuplicate = true;
		DuplicateRow();
		isDuplicate = false;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_JVQuestion));
		this.repositoryItemCheckEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.gridControl1 = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.QuesCode = new Apzon.Controls.Grid.GridColumnEx();
		this.name = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.answerType_Rep = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.QuesGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.quesGroupRep = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.descriptio = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.repCode = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.repName = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.grd_Answer = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Answer = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.radioGroup1 = new Apzon.Controls.Editor.RadioGroupEx();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.answerType_Rep).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.quesGroupRep).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Answer).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Answer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.radioGroup1.Properties).BeginInit();
		base.SuspendLayout();
		this.repositoryItemCheckEdit2.AutoHeight = false;
		this.repositoryItemCheckEdit2.Name = "repositoryItemCheckEdit2";
		this.repositoryItemCheckEdit2.ValueChecked = "Y";
		this.repositoryItemCheckEdit2.ValueUnchecked = "N";
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(84, 471);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 15;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.EditControl = null;
		this.btnOK.EditValue = "OK";
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(6, 471);
		this.btnOK.Name = "btnOK";
		this.btnOK.ReadOnly = false;
		this.btnOK.SettingInVisible = false;
		this.btnOK.SettingReadonly = false;
		this.btnOK.Size = new System.Drawing.Size(75, 23);
		this.btnOK.TabIndex = 14;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		this.gridControl1.AllowPasteExcel = true;
		this.gridControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControl1.ClipboardData = null;
		this.gridControl1.ContextMenuStrip = this.contextMenuStrip1;
		this.gridControl1.Location = new System.Drawing.Point(1, 31);
		this.gridControl1.MainView = this.gridView1;
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.quesGroupRep, this.repCode, this.repName, this.answerType_Rep });
		this.gridControl1.Size = new System.Drawing.Size(806, 262);
		this.gridControl1.TabIndex = 13;
		this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView1 });
		this.gridControl1.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.gridControl1_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.removeRowToolStripMenuItem, this.duplicateToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(144, 48);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
		this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.duplicateToolStripMenuItem.Text = "Duplicate";
		this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = null;
		this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.QuesCode, this.name, this.gridColumn5, this.QuesGroup, this.descriptio, this.gridColumn9, this.gridColumn10 });
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = "oJobVacancyQuestionLibrary";
		this.gridView1.GridControl = this.gridControl1;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 30;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = "APZ_OQLQ";
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.OptionsView.ShowGroupPanel = false;
		this.gridView1.UseBatchMod = false;
		this.gridView1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator);
		this.gridView1.StartSorting += new System.EventHandler(this.gridView1_StartSorting);
		this.gridView1.EndSorting += new System.EventHandler(this.gridView1_EndSorting);
		this.gridView1.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridView1_FocusedRowChanged);
		this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
		this.gridView1.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanging);
		this.gridView1.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gridView1_BeforeLeaveRow);
		this.QuesCode.Caption = "Code";
		this.QuesCode.CurrencyDateField = null;
		this.QuesCode.CurrencyField = null;
		this.QuesCode.CurrencyFixed = null;
		this.QuesCode.CurrencyFixedRate = 0.0;
		this.QuesCode.CurrencyRateField = null;
		this.QuesCode.FieldName = "QuesCode";
		this.QuesCode.Name = "QuesCode";
		this.QuesCode.OptionsColumn.ReadOnly = true;
		this.QuesCode.PrimaryKey = true;
		this.QuesCode.UnitField = null;
		this.QuesCode.Visible = true;
		this.QuesCode.VisibleIndex = 0;
		this.QuesCode.Width = 40;
		this.name.Caption = "Content";
		this.name.CurrencyDateField = null;
		this.name.CurrencyField = null;
		this.name.CurrencyFixed = null;
		this.name.CurrencyFixedRate = 0.0;
		this.name.CurrencyRateField = null;
		this.name.FieldName = "Content";
		this.name.Name = "name";
		this.name.PrimaryKey = false;
		this.name.UnitField = null;
		this.name.Visible = true;
		this.name.VisibleIndex = 1;
		this.name.Width = 319;
		this.gridColumn5.Caption = "Type ";
		this.gridColumn5.ColumnEdit = this.answerType_Rep;
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "AnsType";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 2;
		this.gridColumn5.Width = 83;
		this.answerType_Rep.AutoHeight = false;
		this.answerType_Rep.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.answerType_Rep.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name2")
		});
		this.answerType_Rep.Name = "answerType_Rep";
		this.answerType_Rep.NullText = "";
		this.answerType_Rep.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.answerType_Rep_EditValueChanging);
		this.QuesGroup.Caption = "Category";
		this.QuesGroup.ColumnEdit = this.quesGroupRep;
		this.QuesGroup.CurrencyDateField = null;
		this.QuesGroup.CurrencyField = null;
		this.QuesGroup.CurrencyFixed = null;
		this.QuesGroup.CurrencyFixedRate = 0.0;
		this.QuesGroup.CurrencyRateField = null;
		this.QuesGroup.FieldName = "QuesGroup";
		this.QuesGroup.Name = "QuesGroup";
		this.QuesGroup.PrimaryKey = false;
		this.QuesGroup.UnitField = null;
		this.QuesGroup.Visible = true;
		this.QuesGroup.VisibleIndex = 3;
		this.QuesGroup.Width = 165;
		this.quesGroupRep.AutoHeight = false;
		this.quesGroupRep.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.quesGroupRep.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.quesGroupRep.CheckFields = null;
		this.quesGroupRep.CheckTables = null;
		this.quesGroupRep.DisplayMember = "Name";
		this.quesGroupRep.Name = "quesGroupRep";
		this.quesGroupRep.NullText = "";
		this.quesGroupRep.ShowFooter = false;
		this.quesGroupRep.ShowHeader = false;
		this.quesGroupRep.ValueMember = "Id";
		this.descriptio.Caption = "Note";
		this.descriptio.CurrencyDateField = null;
		this.descriptio.CurrencyField = null;
		this.descriptio.CurrencyFixed = null;
		this.descriptio.CurrencyFixedRate = 0.0;
		this.descriptio.CurrencyRateField = null;
		this.descriptio.FieldName = "Note";
		this.descriptio.Name = "descriptio";
		this.descriptio.PrimaryKey = false;
		this.descriptio.UnitField = null;
		this.descriptio.Visible = true;
		this.descriptio.VisibleIndex = 4;
		this.descriptio.Width = 167;
		this.gridColumn9.Caption = "gridColumn9";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "AutoLabel";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn10.Caption = "gridColumn10";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "IsDuplicate";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.repCode.AutoHeight = false;
		this.repCode.MaxLength = 20;
		this.repCode.Name = "repCode";
		this.repName.AutoHeight = false;
		this.repName.MaxLength = 100;
		this.repName.Name = "repName";
		this.grd_Answer.AllowPasteExcel = true;
		this.grd_Answer.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Answer.ClipboardData = null;
		this.grd_Answer.ContextMenuStrip = this.contextMenuStrip2;
		this.grd_Answer.Location = new System.Drawing.Point(1, 325);
		this.grd_Answer.MainView = this.gv_Answer;
		this.grd_Answer.Name = "grd_Answer";
		this.grd_Answer.Size = new System.Drawing.Size(806, 140);
		this.grd_Answer.TabIndex = 16;
		this.grd_Answer.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Answer });
		this.grd_Answer.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_Answer_EditorKeyDown);
		this.grd_Answer.Click += new System.EventHandler(this.grd_Answer_Click);
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripMenuItem1 });
		this.contextMenuStrip2.Name = "contextMenuStrip1";
		this.contextMenuStrip2.Size = new System.Drawing.Size(144, 26);
		this.toolStripMenuItem1.Name = "toolStripMenuItem1";
		this.toolStripMenuItem1.Size = new System.Drawing.Size(143, 22);
		this.toolStripMenuItem1.Text = "Remove Row";
		this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
		this.gv_Answer.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Answer.ColumnKey = null;
		this.gv_Answer.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumn1, this.gridColumn6, this.gridColumn2, this.gridColumn3, this.gridColumn7, this.gridColumn4, this.gridColumn8 });
		this.gv_Answer.DisableFormSetting = false;
		this.gv_Answer.DisplayException = false;
		this.gv_Answer.EditColumn = null;
		this.gv_Answer.FieldCurrency = null;
		this.gv_Answer.FieldRate = null;
		this.gv_Answer.FormId = "oJobVacancyQuestionLibrary";
		this.gv_Answer.GridControl = this.grd_Answer;
		this.gv_Answer.IgnoreFilter = false;
		this.gv_Answer.IndexEditRow = -1;
		this.gv_Answer.IndicatorWidth = 30;
		this.gv_Answer.IsApzTable = false;
		this.gv_Answer.IsInsertNewRow = false;
		this.gv_Answer.IsLoadingData = false;
		this.gv_Answer.IsMappingContextAction = false;
		this.gv_Answer.ItemId = "APZ_OQLA";
		this.gv_Answer.LstColumnAllowBatch = null;
		this.gv_Answer.LstColumnNotAllowBatch = null;
		this.gv_Answer.LstColumnNotEditable = null;
		this.gv_Answer.LstColumNotSetting = null;
		this.gv_Answer.Name = "gv_Answer";
		this.gv_Answer.OptionsView.ShowGroupPanel = false;
		this.gv_Answer.UseBatchMod = false;
		this.gv_Answer.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Answer_CustomDrawRowIndicator);
		this.gv_Answer.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Answer_CellValueChanged);
		this.gv_Answer.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Answer_CellValueChanging);
		this.gridColumn1.Caption = "Code";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "AnsCode";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Width = 115;
		this.gridColumn6.Caption = "gridColumn6";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "QuesCode";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn2.Caption = "Label";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "AnsLabel";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 82;
		this.gridColumn3.Caption = "Correct";
		this.gridColumn3.ColumnEdit = this.repositoryItemCheckEdit2;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "Correct";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 2;
		this.gridColumn3.Width = 47;
		this.gridColumn7.Caption = "Content";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "Content";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 0;
		this.gridColumn7.Width = 390;
		this.gridColumn4.Caption = "Note";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "Note";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 3;
		this.gridColumn4.Width = 255;
		this.gridColumn8.Caption = "gridColumn8";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "IsNewRow";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Questions";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 5);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(54, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 17;
		this.labelLine1.Utext = "Questions";
		this.labelLine2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Answers";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(12, 299);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(46, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 17;
		this.labelLine2.Utext = "Answers";
		this.radioGroup1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.radioGroup1.EditControl = null;
		this.radioGroup1.EditValue = "O";
		this.radioGroup1.FieldName = null;
		this.radioGroup1.IsSearchControl = false;
		this.radioGroup1.Location = new System.Drawing.Point(139, 299);
		this.radioGroup1.Name = "radioGroup1";
		this.radioGroup1.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.radioGroup1.Properties.Appearance.Options.UseBackColor = true;
		this.radioGroup1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.radioGroup1.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[3]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("A", "Alphabet"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("N", "Number"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("O", "None")
		});
		this.radioGroup1.SettingInVisible = false;
		this.radioGroup1.SettingReadonly = false;
		this.radioGroup1.Size = new System.Drawing.Size(210, 20);
		this.radioGroup1.TabIndex = 19;
		this.radioGroup1.SelectedIndexChanged += new System.EventHandler(this.radioGroup1_SelectedIndexChanged);
		this.radioGroup1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.radioGroup1_MouseDown);
		this.labelControl1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelControl1.Location = new System.Drawing.Point(70, 302);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(63, 13);
		this.labelControl1.TabIndex = 20;
		this.labelControl1.Text = "Auto label by";
		base.AcceptButton = this.btnOK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(805, 498);
		base.Controls.Add(this.labelControl1);
		base.Controls.Add(this.radioGroup1);
		base.Controls.Add(this.gridControl1);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.grd_Answer);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btnOK);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_JVQuestion";
		base.ShowInTaskbar = false;
		this.Text = "Question Library";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_JVInterviewStatus_FormClosing);
		base.Load += new System.EventHandler(this.frm_JVInterviewStatus_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.answerType_Rep).EndInit();
		((System.ComponentModel.ISupportInitialize)this.quesGroupRep).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Answer).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Answer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.radioGroup1.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
