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
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Helper;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.StyleFormatConditions;

namespace Apzon.Forms.Commons;

public class frm_ContextActionPublishService : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private LabelLine labelLine1;

	private TextEditEx txt_Controller;

	private TextEditEx txt_Action;

	private LabelLine lbl_Action;

	private TextEditEx txt_Uri;

	private LabelLine lbl_Uri;

	private SimpleButton btn_Copy;

	private XtraTabControlEx tabControl_Main;

	private XtraTabPageEx xtraTabPageEx1;

	private XtraTabPageEx tab_MarkDown;

	private SimpleButton btn_OK;

	private SimpleButton btn_Cancel;

	private SimpleButton btn_ModifyAction;

	private TreeEx tree_Input;

	private TreeListColumnEx treeListColumnEx1;

	private TreeListColumnEx treeCol_Father;

	private TreeListColumnEx treeListColumnEx3;

	private RepositoryItemButtonEdit rep_btnAddChild;

	private TreeListColumnEx treeListColumnEx4;

	private TreeListColumnEx treeListColumnEx5;

	private TreeListColumnEx treeListColumnEx7;

	private TreeListColumnEx treeListColumnEx8;

	private RepositoryItemLookUpEdit rep_JsonType;

	private RepositoryItemLookUpEdit rep_ConfigDataType;

	private TextEditEx txt_Description;

	private LabelLine labelLine3;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private ContextMenuStrip contextMenuStripContent;

	private ToolStripMenuItem removeFieldToolStripMenuItem;

	private ToolStripMenuItem insertFieldToolStripMenuItem;

	private XtraTabControlEx tabControl_InputData;

	private XtraTabPageEx tabControl_OutputData;

	private XtraTabPageEx xtraTabPageInputData;

	private MemoExEditEx txtOurtPutQuery;

	private TextEditEx txtOutputParamater;

	private LabelLine lbOutPutQueryParamater;

	private LabelLine lbOutPutQuery;

	private LookUpEditEx txtOutputType;

	private LabelLine lbOutputReturnType;

	public int AbsEntry { get; set; }

	public DataTable InputTable { get; set; }

	public int ActionAbsEntry { get; set; }

	public string ActionFormId { get; set; }

	public string ActionItemId { get; set; }

	public DataTable Source { get; set; }

	public bool ModifyFormContextAction { get; set; }

	public frm_ContextActionPublishService()
	{
		InitializeComponent();
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			_formMode = ((value == 0) ? Enums.FormViewModeType.View : value);
			switch (_formMode)
			{
			case Enums.FormViewModeType.Update:
			case Enums.FormViewModeType.Add:
			case Enums.FormViewModeType.View:
			case Enums.FormViewModeType.Busy:
				switch (_formMode)
				{
				case Enums.FormViewModeType.Add:
				{
					SimpleButton simpleButton5 = btn_OK;
					bool enabled = (btn_Cancel.Enabled = true);
					simpleButton5.Enabled = enabled;
					btn_OK.Text = Resources.ButtonAdd;
					break;
				}
				case Enums.FormViewModeType.Update:
				{
					SimpleButton simpleButton4 = btn_OK;
					bool enabled = (btn_Cancel.Enabled = true);
					simpleButton4.Enabled = enabled;
					btn_OK.Text = Resources.ButtonUpdate;
					break;
				}
				case Enums.FormViewModeType.View:
				{
					SimpleButton simpleButton3 = btn_OK;
					bool enabled = (btn_Cancel.Enabled = true);
					simpleButton3.Enabled = enabled;
					btn_OK.Text = Resources.ButtonOK;
					break;
				}
				case Enums.FormViewModeType.Busy:
				{
					SimpleButton simpleButton2 = btn_OK;
					bool enabled = (btn_Cancel.Enabled = false);
					simpleButton2.Enabled = enabled;
					break;
				}
				}
				lbl_Uri.Visible = true;
				txt_Uri.Visible = true;
				btn_Copy.Visible = true;
				break;
			case Enums.FormViewModeType.Find:
			{
				SimpleButton simpleButton = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButton.Enabled = enabled;
				btn_OK.Text = Resources.ButtonFind;
				lbl_Uri.Visible = false;
				txt_Uri.Visible = false;
				btn_Copy.Visible = false;
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
			{
				FormMode = Enums.FormViewModeType.Add;
				InitDoc();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MODE_FIND()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
			{
				this.SetFindMode();
				tree_Input.DataSource = InputTable.Clone();
				tree_Input.RefreshDataSource();
				FormMode = Enums.FormViewModeType.Find;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_FIRST()
	{
		_ = 1;
		try
		{
			_ = 1;
			try
			{
				if (FormMode == Enums.FormViewModeType.View || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
				{
					await LoadData(await _httpClient.PublishService.Navigate(AbsEntry, Enums.NavigateType.First));
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex);
				WinCommons.ErrorLineNumber(ex);
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		_ = 1;
		try
		{
			_ = 1;
			try
			{
				if (FormMode == Enums.FormViewModeType.View || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
				{
					await LoadData(await _httpClient.PublishService.Navigate(AbsEntry, Enums.NavigateType.Previous));
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex);
				WinCommons.ErrorLineNumber(ex);
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	public async Task MOVE_NEXT()
	{
		_ = 1;
		try
		{
			_ = 1;
			try
			{
				if (FormMode == Enums.FormViewModeType.View || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
				{
					await LoadData(await _httpClient.PublishService.Navigate(AbsEntry, Enums.NavigateType.Next));
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex);
				WinCommons.ErrorLineNumber(ex);
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	public async Task MOVE_LAST()
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.View || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
			{
				await LoadData(await _httpClient.PublishService.Navigate(AbsEntry, Enums.NavigateType.Last));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
			{
				await LoadData(AbsEntry);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_ContextActionPublishService_Load(object sender, EventArgs e)
	{
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "O", "Object" },
				{ "A", "Array" },
				{ "F", "Field" }
			};
			rep_JsonType.SetDataSource(dictionary);
			txtOutputType.Properties.DataSource = dictionary;
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			foreach (Enums.DisplayFormatType value2 in Enum.GetValues(typeof(Enums.DisplayFormatType)))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = (int)value2;
				string value = Function.ToString(value2);
				if (value2 == Enums.DisplayFormatType.None)
				{
					value = "String";
				}
				dataRow["Name"] = Function.SpaceBeforeCapital(value);
				dataTable.Rows.Add(dataRow);
			}
			rep_ConfigDataType.DataSource = dataTable;
			rep_ConfigDataType.DisplayMember = "Name";
			rep_ConfigDataType.ValueMember = "Code";
			InputTable = new DataTable();
			InputTable.Columns.Add("AbsEntry", typeof(int));
			InputTable.Columns.Add("ConfigID", typeof(int));
			InputTable.Columns.Add("FatherNum", typeof(int));
			InputTable.Columns.Add("Name", typeof(string));
			InputTable.Columns.Add("JsonType", typeof(string));
			InputTable.Columns.Add("DataType", typeof(int));
			InputTable.Columns.Add("DfltValue", typeof(string));
			InputTable.Columns.Add("Dscription", typeof(string));
			InputTable.Columns["ConfigID"].AutoIncrement = true;
			InputTable.Columns["ConfigID"].AutoIncrementSeed = (InputTable.IsNotNull() ? (InputTable.AsEnumerable().Max((DataRow t) => Function.ParseLong(t["ConfigID"])) + 1) : 1);
			InputTable.Columns["ConfigID"].AutoIncrementStep = 1L;
			InitDoc();
			int num = 2;
			if (Source != null)
			{
				foreach (DataColumn column in Source.Columns)
				{
					DataRow dataRow2 = InputTable.NewRow();
					dataRow2["ConfigID"] = num;
					dataRow2["FatherNum"] = 1;
					dataRow2["Name"] = column.ColumnName;
					dataRow2["JsonType"] = "F";
					dataRow2["Dscription"] = column.Caption;
					InputTable.Rows.Add(dataRow2);
					num++;
				}
				tree_Input.DataSource = InputTable;
				tree_Input.RefreshDataSource();
			}
			btn_ModifyAction.Visible = !ModifyFormContextAction && AbsEntry != 0;
			txt_Controller.Focus();
			NewDoc();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void InitDoc()
	{
		try
		{
			this.ResetData();
			AbsEntry = 0;
			ActionFormId = "PublishService";
			ActionItemId = "";
			ActionAbsEntry = 0;
			txt_Controller.EditValue = string.Empty;
			txt_Action.EditValue = string.Empty;
			txt_Description.EditValue = string.Empty;
			txtOutputType.EditValue = "A";
			txtOurtPutQuery.EditValue = string.Empty;
			txtOutputParamater.EditValue = "objCode";
			InputTable.Rows.Clear();
			DataRow dataRow = InputTable.NewRow();
			dataRow["ConfigID"] = 1;
			dataRow["FatherNum"] = -1;
			dataRow["Name"] = "Content";
			dataRow["JsonType"] = "A";
			InputTable.Rows.Add(dataRow);
			tree_Input.DataSource = InputTable;
			btn_ModifyAction.Visible = !ModifyFormContextAction && AbsEntry != 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Copy_Click(object sender, EventArgs e)
	{
		try
		{
			Clipboard.SetText(Function.ToString(txt_Uri.EditValue));
			WinCommons.Success(Enums.MessageCode.DataCopiedToClipboad, "");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Controller_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeUri();
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChangeUri()
	{
		try
		{
			string arg = Function.ToString(txt_Controller.EditValue);
			string arg2 = Function.ToString(txt_Action.EditValue);
			txt_Uri.EditValue = $"{Configuration.ApiServer}h/0/{arg}/{arg2}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Action_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			ChangeUri();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (FormMode == Enums.FormViewModeType.Find)
			{
				DataTable dataTable = await _httpClient.PublishService.Search(Function.ToString(txt_Controller.EditValue), Function.ToString(txt_Action.EditValue), Function.ToString(txt_Description.EditValue));
				if (dataTable.IsNotNull() && dataTable.Rows.Count == 1)
				{
					int absEntry = Function.ParseInt(dataTable.Rows[0]["AbsEntry"]);
					await LoadData(absEntry);
					return;
				}
				frm_ContextActionPublishServiceListOf frm_ContextActionPublishServiceListOf2 = new frm_ContextActionPublishServiceListOf();
				frm_ContextActionPublishServiceListOf2.SetSearchedResult(dataTable);
				if (frm_ContextActionPublishServiceListOf2.ShowDialog(this) == DialogResult.Yes)
				{
					int choosedEntry = frm_ContextActionPublishServiceListOf2.GetChoosedEntry();
					await LoadData(choosedEntry);
				}
				return;
			}
			if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(this, Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
				{
					return;
				}
				WinCommons._OpenProcessing();
				DataSet dataSet = new DataSet();
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add("AbsEntry", typeof(int));
				dataTable2.Columns.Add("ControllerName", typeof(string));
				dataTable2.Columns.Add("ActionName", typeof(string));
				dataTable2.Columns.Add("ActionEntry", typeof(int));
				dataTable2.Columns.Add("ActionFormId", typeof(string));
				dataTable2.Columns.Add("ActionItemId", typeof(string));
				dataTable2.Columns.Add("Description", typeof(string));
				dataTable2.Columns.Add("OputQuery", typeof(string));
				dataTable2.Columns.Add("OputType", typeof(string));
				dataTable2.TableName = "APZ_OCPS";
				DataRow dataRow = dataTable2.NewRow();
				dataRow["AbsEntry"] = AbsEntry;
				dataRow["ControllerName"] = Function.ToString(txt_Controller.EditValue);
				dataRow["ActionName"] = Function.ToString(txt_Action.EditValue);
				dataRow["ActionEntry"] = ActionAbsEntry;
				dataRow["ActionFormId"] = ActionFormId;
				dataRow["ActionItemId"] = ActionItemId;
				dataRow["Description"] = Function.ToString(txt_Description.EditValue);
				dataRow["OputQuery"] = Function.ToString(txtOurtPutQuery.EditValue);
				dataRow["OputType"] = Function.ToString(txtOutputType.EditValue);
				dataTable2.Rows.Add(dataRow);
				DataTable dataSourceTable = tree_Input.DataSourceTable;
				dataSourceTable.TableName = "APZ_CPS1";
				dataSet.Tables.Add(dataTable2.Copy());
				dataSet.Tables.Add(dataSourceTable.Copy());
				HttpResult httpResult = await _httpClient.PublishService.Save(dataSet);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				else
				{
					AbsEntry = Function.ParseInt(httpResult.Content);
					btn_ModifyAction.Visible = !ModifyFormContextAction && AbsEntry != 0;
					FormMode = Enums.FormViewModeType.View;
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private async Task LoadData(int absEntry)
	{
		DataSet dataSet = await _httpClient.PublishService.GetById(absEntry);
		if (dataSet.IsNotNull())
		{
			DataTable dataTable = dataSet.Tables[0];
			if (dataTable.IsNotNull())
			{
				this.ResetData();
				txt_Controller.EditValue = Function.ToString(dataTable.Rows[0]["ControllerName"]);
				txt_Action.EditValue = Function.ToString(dataTable.Rows[0]["ActionName"]);
				txt_Description.EditValue = Function.ToString(dataTable.Rows[0]["Description"]);
				txtOurtPutQuery.EditValue = Function.ToString(dataTable.Rows[0]["OputQuery"]);
				txtOutputType.EditValue = Function.ToString(dataTable.Rows[0]["OputType"]);
				txtOutputParamater.EditValue = "objCode";
				AbsEntry = absEntry;
				ActionFormId = Function.ToString(dataTable.Rows[0]["ActionFormId"]);
				ActionItemId = Function.ToString(dataTable.Rows[0]["ActionItemId"]);
				ActionAbsEntry = Function.ParseInt(dataTable.Rows[0]["ActionEntry"]);
				FormMode = Enums.FormViewModeType.View;
				btn_ModifyAction.Visible = !ModifyFormContextAction && AbsEntry != 0;
			}
			DataTable dataTable2 = dataSet.Tables[1];
			dataTable2.Columns["ConfigID"].AutoIncrement = true;
			dataTable2.Columns["ConfigID"].AutoIncrementSeed = (dataTable2.IsNotNull() ? (dataTable2.AsEnumerable().Max((DataRow t) => Function.ParseLong(t["ConfigID"])) + 1) : 1);
			dataTable2.Columns["ConfigID"].AutoIncrementStep = 1L;
			tree_Input.DataSource = dataTable2;
			tree_Input.RefreshDataSource();
			NewDoc();
		}
		else
		{
			WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
		}
	}

	private void rep_btnAddChild_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int fatherId = Function.ParseInt(tree_Input.FocusedNode.GetValue("ConfigID"));
			NewDoc(fatherId);
			tree_Input.FocusedNode.Expand();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void NewDoc(int fatherId = -1)
	{
		try
		{
			if (tree_Input.DataSource is DataTable dataTable && dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Name"])) && Function.ParseInt(t["FatherNum"]) == fatherId) == null)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["FatherNum"] = fatherId;
				dataTable.Rows.Add(dataRow);
				tree_Input.DataSource = dataTable;
				tree_Input.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tree_Input_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column != null)
			{
				if (e.Column.FieldName == "Name")
				{
					int fatherId = Function.ParseInt(e.Node.GetValue("FatherNum"));
					NewDoc(fatherId);
				}
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_ModifyAction_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = new DataTable();
			DataTable dataSourceTable = tree_Input.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				DataRow dataRow = dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["FatherNum"]) == -1 && !Function.ToString(t["JsonType"]).Equals("F"));
				if (dataRow != null)
				{
					GetDataTableStructureFromObjectConfig(dataTable, dataSourceTable, Function.ParseInt(dataRow["ConfigID"]), "");
				}
			}
			string text = Function.ToString(txt_Description.EditValue);
			frm_ContextAction frm_ContextAction2 = new frm_ContextAction("PublishService", AbsEntry.ToString(), dataTable, (!string.IsNullOrEmpty(text)) ? text : Function.ToString(txt_Action.EditValue));
			frm_ContextAction2.CloseFormAfterSave = true;
			if (frm_ContextAction2.ShowDialog(this) == DialogResult.Yes)
			{
				ActionAbsEntry = frm_ContextAction2.FirstEntry;
				ActionFormId = "PublishService";
				ActionItemId = AbsEntry.ToString();
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetDataTableStructureFromObjectConfig(DataTable source, DataTable configObj, int fatherId, string fatherName)
	{
		EnumerableRowCollection<DataRow> enumerableRowCollection = from t in configObj.AsEnumerable()
			where Function.ParseInt(t["FatherNum"]) == fatherId
			select t;
		if (!enumerableRowCollection.Any())
		{
			return;
		}
		foreach (DataRow item in enumerableRowCollection)
		{
			string text = (string.IsNullOrEmpty(fatherName) ? Function.ToString(item["Name"]) : string.Format("{0}.{1}", fatherName, Function.ToString(item["Name"])));
			if (Function.ToString(item["JsonType"]).Equals("F"))
			{
				if (!source.ExistsColumn(text))
				{
					switch ((Enums.DisplayFormatType)Function.ParseInt(item["DataType"]))
					{
					case Enums.DisplayFormatType.Qty:
					case Enums.DisplayFormatType.Price:
					case Enums.DisplayFormatType.Amount:
					case Enums.DisplayFormatType.Rate:
					case Enums.DisplayFormatType.Percent:
					case Enums.DisplayFormatType.Unit:
						source.Columns.Add(text, typeof(double));
						break;
					case Enums.DisplayFormatType.DateTime:
						source.Columns.Add(text, typeof(DateTime));
						break;
					case Enums.DisplayFormatType.Integer:
						source.Columns.Add(text, typeof(long));
						break;
					default:
						source.Columns.Add(text, typeof(string));
						break;
					}
				}
			}
			else
			{
				GetDataTableStructureFromObjectConfig(source, configObj, Function.ParseInt(item["ConfigID"]), text);
			}
		}
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemovePublishService, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			WinCommons._OpenProcessing();
			HttpResult httpResult = await _httpClient.PublishService.Remove(AbsEntry);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				await REFRESH();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			removeToolStripMenuItem.Visible = AbsEntry != 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void frm_ContextActionPublishService_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
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

	private void removeFieldToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			TreeListNode focusedNode = tree_Input.FocusedNode;
			if (focusedNode.ParentNode != null)
			{
				focusedNode.ParentNode.Nodes.Remove(focusedNode);
			}
			else
			{
				tree_Input.Nodes.Remove(focusedNode);
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void insertFieldToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = tree_Input.DataSourceTable;
			if (dataSourceTable == null || dataSourceTable.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.CannotGetData, "");
				return;
			}
			int num = Function.ParseInt(tree_Input.FocusedNode.GetValue("FatherNum"));
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["FatherNum"] = num;
			dataSourceTable.Rows.InsertAt(dataRow, tree_Input.FocusedNode.Id);
			dataSourceTable.AcceptChanges();
			TreeListNode focusedNode = tree_Input.FindNodeByKeyID(Function.ParseInt(dataRow["ConfigID"]));
			tree_Input.FocusedNode = focusedNode;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_ContextActionPublishService_OnControlValueChanged(object sender, ControlChangedEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.frm_ContextActionPublishService));
		DevExpress.XtraTreeList.StyleFormatConditions.TreeListFormatRule treeListFormatRule = new DevExpress.XtraTreeList.StyleFormatConditions.TreeListFormatRule();
		DevExpress.XtraEditors.FormatConditionRuleIconSet formatConditionRuleIconSet = new DevExpress.XtraEditors.FormatConditionRuleIconSet();
		DevExpress.XtraEditors.FormatConditionIconSet formatConditionIconSet = new DevExpress.XtraEditors.FormatConditionIconSet();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon2 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon3 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon4 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Controller = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Action = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_Action = new Apzon.Controls.Controls.LabelLine();
		this.txt_Uri = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_Uri = new Apzon.Controls.Controls.LabelLine();
		this.btn_Copy = new DevExpress.XtraEditors.SimpleButton();
		this.tabControl_Main = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPageEx1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.tree_Input = new Apzon.Controls.Tree.TreeEx();
		this.treeListColumnEx1 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeCol_Father = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx3 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_btnAddChild = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.treeListColumnEx4 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_JsonType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.treeListColumnEx5 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_ConfigDataType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.treeListColumnEx7 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx8 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.contextMenuStripContent = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeFieldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.insertFieldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.tab_MarkDown = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_OK = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Cancel = new DevExpress.XtraEditors.SimpleButton();
		this.btn_ModifyAction = new DevExpress.XtraEditors.SimpleButton();
		this.txt_Description = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.tabControl_InputData = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabControl_OutputData = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txtOutputType = new Apzon.Controls.Editor.LookUpEditEx();
		this.lbOutputReturnType = new Apzon.Controls.Controls.LabelLine();
		this.txtOurtPutQuery = new Apzon.Controls.Editor.MemoExEditEx();
		this.txtOutputParamater = new Apzon.Controls.Editor.TextEditEx();
		this.lbOutPutQueryParamater = new Apzon.Controls.Controls.LabelLine();
		this.lbOutPutQuery = new Apzon.Controls.Controls.LabelLine();
		this.xtraTabPageInputData = new Apzon.Controls.Controls.XtraTabPageEx();
		((System.ComponentModel.ISupportInitialize)this.txt_Controller.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Action.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Uri.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl_Main).BeginInit();
		this.tabControl_Main.SuspendLayout();
		this.xtraTabPageEx1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tree_Input).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnAddChild).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_JsonType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ConfigDataType).BeginInit();
		this.contextMenuStripContent.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Description.Properties).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tabControl_InputData).BeginInit();
		this.tabControl_InputData.SuspendLayout();
		this.tabControl_OutputData.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtOutputType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtOurtPutQuery.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtOutputParamater.Properties).BeginInit();
		this.xtraTabPageInputData.SuspendLayout();
		base.SuspendLayout();
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(13, 13);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(120, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Controller";
		this.txt_Controller.FieldName = null;
		this.txt_Controller.IsSearchControl = false;
		this.txt_Controller.Location = new System.Drawing.Point(133, 10);
		this.txt_Controller.Name = "txt_Controller";
		this.txt_Controller.Size = new System.Drawing.Size(200, 20);
		this.txt_Controller.TabIndex = 1;
		this.txt_Controller.EditValueChanged += new System.EventHandler(this.txt_Controller_EditValueChanged);
		this.txt_Action.FieldName = null;
		this.txt_Action.IsSearchControl = false;
		this.txt_Action.Location = new System.Drawing.Point(133, 36);
		this.txt_Action.Name = "txt_Action";
		this.txt_Action.Size = new System.Drawing.Size(200, 20);
		this.txt_Action.TabIndex = 2;
		this.txt_Action.EditValueChanged += new System.EventHandler(this.txt_Action_EditValueChanged);
		this.lbl_Action.FieldName = null;
		this.lbl_Action.HideWhenEmpty = false;
		this.lbl_Action.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Action.Location = new System.Drawing.Point(13, 39);
		this.lbl_Action.Name = "lbl_Action";
		this.lbl_Action.ShowUnderLine = true;
		this.lbl_Action.Size = new System.Drawing.Size(120, 20);
		this.lbl_Action.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Action.TabIndex = 4;
		this.lbl_Action.Utext = "Action";
		this.txt_Uri.FieldName = null;
		this.txt_Uri.IsSearchControl = false;
		this.txt_Uri.Location = new System.Drawing.Point(133, 88);
		this.txt_Uri.Name = "txt_Uri";
		this.txt_Uri.Properties.ReadOnly = true;
		this.txt_Uri.Size = new System.Drawing.Size(361, 20);
		this.txt_Uri.TabIndex = 4;
		this.lbl_Uri.FieldName = null;
		this.lbl_Uri.HideWhenEmpty = false;
		this.lbl_Uri.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Uri.Location = new System.Drawing.Point(13, 91);
		this.lbl_Uri.Name = "lbl_Uri";
		this.lbl_Uri.ShowUnderLine = true;
		this.lbl_Uri.Size = new System.Drawing.Size(120, 20);
		this.lbl_Uri.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Uri.TabIndex = 6;
		this.lbl_Uri.Utext = "Uri";
		this.btn_Copy.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_Copy.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Copy.ImageOptions.Image");
		this.btn_Copy.Location = new System.Drawing.Point(500, 85);
		this.btn_Copy.Name = "btn_Copy";
		this.btn_Copy.Size = new System.Drawing.Size(75, 23);
		this.btn_Copy.TabIndex = 5;
		this.btn_Copy.Text = "Copy Uri";
		this.btn_Copy.Click += new System.EventHandler(this.btn_Copy_Click);
		this.tabControl_Main.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tabControl_Main.IsSearchControl = false;
		this.tabControl_Main.Location = new System.Drawing.Point(0, 0);
		this.tabControl_Main.Name = "tabControl_Main";
		this.tabControl_Main.SelectedTabPage = this.xtraTabPageEx1;
		this.tabControl_Main.Size = new System.Drawing.Size(968, 561);
		this.tabControl_Main.TabIndex = 7;
		this.tabControl_Main.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPageEx1, this.tab_MarkDown });
		this.xtraTabPageEx1.Controls.Add(this.tree_Input);
		this.xtraTabPageEx1.FieldName = "";
		this.xtraTabPageEx1.IsSearchControl = false;
		this.xtraTabPageEx1.Name = "xtraTabPageEx1";
		this.xtraTabPageEx1.Size = new System.Drawing.Size(966, 536);
		this.xtraTabPageEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageEx1.Text = "Input Object";
		this.tree_Input.AllowDrop = true;
		this.tree_Input.AllowPasteExcel = true;
		this.tree_Input.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tree_Input.ColumnKey = null;
		this.tree_Input.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[7] { this.treeListColumnEx1, this.treeCol_Father, this.treeListColumnEx3, this.treeListColumnEx4, this.treeListColumnEx5, this.treeListColumnEx7, this.treeListColumnEx8 });
		this.tree_Input.ContextMenuStrip = this.contextMenuStripContent;
		this.tree_Input.Cursor = System.Windows.Forms.Cursors.Default;
		this.tree_Input.DisableFormSetting = false;
		this.tree_Input.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tree_Input.EditColumn = null;
		this.tree_Input.FieldCurrency = null;
		this.tree_Input.FieldRate = null;
		treeListFormatRule.Name = "Format0";
		formatConditionIconSet.CategoryName = "Shapes";
		formatConditionIconSetIcon.PredefinedName = "TrafficLights4_3.png";
		formatConditionIconSetIcon.Value = new decimal(new int[4] { 4, 0, 0, 0 });
		formatConditionIconSetIcon.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon2.PredefinedName = "TrafficLights23_2.png";
		formatConditionIconSetIcon2.Value = new decimal(new int[4] { 3, 0, 0, 0 });
		formatConditionIconSetIcon2.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon3.PredefinedName = "TrafficLights4_4.png";
		formatConditionIconSetIcon3.Value = new decimal(new int[4] { 2, 0, 0, 0 });
		formatConditionIconSetIcon3.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon4.PredefinedName = "TrafficLights4_1.png";
		formatConditionIconSetIcon4.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon2);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon3);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon4);
		formatConditionIconSet.Name = "RedToBlack";
		formatConditionRuleIconSet.IconSet = formatConditionIconSet;
		treeListFormatRule.Rule = formatConditionRuleIconSet;
		this.tree_Input.FormatRules.Add(treeListFormatRule);
		this.tree_Input.FormId = "oUriForIncomming";
		this.tree_Input.IndexEditRow = 0;
		this.tree_Input.IndicatorWidth = 40;
		this.tree_Input.IsApzTable = false;
		this.tree_Input.IsLoadingData = false;
		this.tree_Input.ItemId = "ODATA";
		this.tree_Input.KeyFieldName = "ConfigID";
		this.tree_Input.Location = new System.Drawing.Point(0, 0);
		this.tree_Input.LstColumnNotEditable = null;
		this.tree_Input.LstColumNotSetting = null;
		this.tree_Input.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.tree_Input.Name = "tree_Input";
		this.tree_Input.OptionsBehavior.AutoChangeParent = false;
		this.tree_Input.OptionsBehavior.CloseEditorOnLostFocus = false;
		this.tree_Input.OptionsBehavior.PopulateServiceColumns = true;
		this.tree_Input.OptionsBehavior.ShowToolTips = false;
		this.tree_Input.OptionsBehavior.SmartMouseHover = false;
		this.tree_Input.OptionsDragAndDrop.AcceptOuterNodes = true;
		this.tree_Input.OptionsDragAndDrop.CanCloneNodesOnDrop = true;
		this.tree_Input.OptionsDragAndDrop.DragNodesMode = DevExpress.XtraTreeList.DragNodesMode.Multiple;
		this.tree_Input.OptionsDragAndDrop.DropNodesMode = DevExpress.XtraTreeList.DropNodesMode.Advanced;
		this.tree_Input.OptionsMenu.ShowExpandCollapseItems = false;
		this.tree_Input.OptionsSelection.KeepSelectedOnClick = false;
		this.tree_Input.OptionsSelection.SelectNodesOnRightClick = true;
		this.tree_Input.OptionsView.AutoWidth = false;
		this.tree_Input.OptionsView.EnableAppearanceEvenRow = true;
		this.tree_Input.OptionsView.EnableAppearanceOddRow = true;
		this.tree_Input.ParentFieldName = "FatherNum";
		this.tree_Input.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_btnAddChild, this.rep_JsonType, this.rep_ConfigDataType });
		this.tree_Input.Size = new System.Drawing.Size(966, 536);
		this.tree_Input.TabIndex = 2;
		this.tree_Input.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tree_Input_CellValueChanged);
		this.treeListColumnEx1.Caption = "ID";
		this.treeListColumnEx1.CurrencyDateField = null;
		this.treeListColumnEx1.CurrencyField = null;
		this.treeListColumnEx1.CurrencyFixed = null;
		this.treeListColumnEx1.CurrencyFixedRate = 0.0;
		this.treeListColumnEx1.CurrencyRateField = null;
		this.treeListColumnEx1.FieldName = "ConfigID";
		this.treeListColumnEx1.Name = "treeListColumnEx1";
		this.treeCol_Father.Caption = "Father Num";
		this.treeCol_Father.CurrencyDateField = null;
		this.treeCol_Father.CurrencyField = null;
		this.treeCol_Father.CurrencyFixed = null;
		this.treeCol_Father.CurrencyFixedRate = 0.0;
		this.treeCol_Father.CurrencyRateField = null;
		this.treeCol_Father.FieldName = "FatherNum";
		this.treeCol_Father.Name = "treeCol_Father";
		this.treeListColumnEx3.Caption = "Name";
		this.treeListColumnEx3.ColumnEdit = this.rep_btnAddChild;
		this.treeListColumnEx3.CurrencyDateField = null;
		this.treeListColumnEx3.CurrencyField = null;
		this.treeListColumnEx3.CurrencyFixed = null;
		this.treeListColumnEx3.CurrencyFixedRate = 0.0;
		this.treeListColumnEx3.CurrencyRateField = null;
		this.treeListColumnEx3.FieldName = "Name";
		this.treeListColumnEx3.Name = "treeListColumnEx3";
		this.treeListColumnEx3.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.treeListColumnEx3.Visible = true;
		this.treeListColumnEx3.VisibleIndex = 0;
		this.treeListColumnEx3.Width = 150;
		this.rep_btnAddChild.AutoHeight = false;
		this.rep_btnAddChild.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
		});
		this.rep_btnAddChild.Name = "rep_btnAddChild";
		this.rep_btnAddChild.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_btnAddChild_ButtonClick);
		this.treeListColumnEx4.Caption = "Type";
		this.treeListColumnEx4.ColumnEdit = this.rep_JsonType;
		this.treeListColumnEx4.CurrencyDateField = null;
		this.treeListColumnEx4.CurrencyField = null;
		this.treeListColumnEx4.CurrencyFixed = null;
		this.treeListColumnEx4.CurrencyFixedRate = 0.0;
		this.treeListColumnEx4.CurrencyRateField = null;
		this.treeListColumnEx4.FieldName = "JsonType";
		this.treeListColumnEx4.Name = "treeListColumnEx4";
		this.treeListColumnEx4.Visible = true;
		this.treeListColumnEx4.VisibleIndex = 1;
		this.treeListColumnEx4.Width = 102;
		this.rep_JsonType.AutoHeight = false;
		this.rep_JsonType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_JsonType.DisplayMember = "Name";
		this.rep_JsonType.Name = "rep_JsonType";
		this.rep_JsonType.NullText = "";
		this.rep_JsonType.ValueMember = "Code";
		this.treeListColumnEx5.Caption = "Data Type";
		this.treeListColumnEx5.ColumnEdit = this.rep_ConfigDataType;
		this.treeListColumnEx5.CurrencyDateField = null;
		this.treeListColumnEx5.CurrencyField = null;
		this.treeListColumnEx5.CurrencyFixed = null;
		this.treeListColumnEx5.CurrencyFixedRate = 0.0;
		this.treeListColumnEx5.CurrencyRateField = null;
		this.treeListColumnEx5.FieldName = "DataType";
		this.treeListColumnEx5.Name = "treeListColumnEx5";
		this.treeListColumnEx5.Visible = true;
		this.treeListColumnEx5.VisibleIndex = 2;
		this.treeListColumnEx5.Width = 121;
		this.rep_ConfigDataType.AutoHeight = false;
		this.rep_ConfigDataType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ConfigDataType.Name = "rep_ConfigDataType";
		this.rep_ConfigDataType.NullText = "";
		this.treeListColumnEx7.Caption = "Default Value";
		this.treeListColumnEx7.CurrencyDateField = null;
		this.treeListColumnEx7.CurrencyField = null;
		this.treeListColumnEx7.CurrencyFixed = null;
		this.treeListColumnEx7.CurrencyFixedRate = 0.0;
		this.treeListColumnEx7.CurrencyRateField = null;
		this.treeListColumnEx7.FieldName = "DfltValue";
		this.treeListColumnEx7.Name = "treeListColumnEx7";
		this.treeListColumnEx7.Visible = true;
		this.treeListColumnEx7.VisibleIndex = 3;
		this.treeListColumnEx7.Width = 172;
		this.treeListColumnEx8.Caption = "Description";
		this.treeListColumnEx8.CurrencyDateField = null;
		this.treeListColumnEx8.CurrencyField = null;
		this.treeListColumnEx8.CurrencyFixed = null;
		this.treeListColumnEx8.CurrencyFixedRate = 0.0;
		this.treeListColumnEx8.CurrencyRateField = null;
		this.treeListColumnEx8.FieldName = "Dscription";
		this.treeListColumnEx8.Name = "treeListColumnEx8";
		this.treeListColumnEx8.Visible = true;
		this.treeListColumnEx8.VisibleIndex = 4;
		this.treeListColumnEx8.Width = 250;
		this.contextMenuStripContent.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.removeFieldToolStripMenuItem, this.insertFieldToolStripMenuItem });
		this.contextMenuStripContent.Name = "contextMenuStripContent";
		this.contextMenuStripContent.Size = new System.Drawing.Size(146, 48);
		this.removeFieldToolStripMenuItem.Name = "removeFieldToolStripMenuItem";
		this.removeFieldToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
		this.removeFieldToolStripMenuItem.Text = "Remove Field";
		this.removeFieldToolStripMenuItem.Click += new System.EventHandler(this.removeFieldToolStripMenuItem_Click);
		this.insertFieldToolStripMenuItem.Name = "insertFieldToolStripMenuItem";
		this.insertFieldToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
		this.insertFieldToolStripMenuItem.Text = "Insert Field";
		this.insertFieldToolStripMenuItem.Click += new System.EventHandler(this.insertFieldToolStripMenuItem_Click);
		this.tab_MarkDown.FieldName = null;
		this.tab_MarkDown.IsSearchControl = false;
		this.tab_MarkDown.Name = "tab_MarkDown";
		this.tab_MarkDown.Size = new System.Drawing.Size(966, 536);
		this.tab_MarkDown.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_MarkDown.Text = "Mark Down";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.Location = new System.Drawing.Point(14, 695);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 8;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(95, 695);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 9;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_ModifyAction.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_ModifyAction.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ModifyAction.ImageOptions.Image");
		this.btn_ModifyAction.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
		this.btn_ModifyAction.Location = new System.Drawing.Point(895, 11);
		this.btn_ModifyAction.Name = "btn_ModifyAction";
		this.btn_ModifyAction.Size = new System.Drawing.Size(94, 71);
		this.btn_ModifyAction.TabIndex = 10;
		this.btn_ModifyAction.Text = "Modify Action";
		this.btn_ModifyAction.Click += new System.EventHandler(this.btn_ModifyAction_Click);
		this.txt_Description.FieldName = null;
		this.txt_Description.IsSearchControl = false;
		this.txt_Description.Location = new System.Drawing.Point(133, 62);
		this.txt_Description.Name = "txt_Description";
		this.txt_Description.Size = new System.Drawing.Size(200, 20);
		this.txt_Description.TabIndex = 3;
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(13, 65);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(120, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 12;
		this.labelLine3.Utext = "Description";
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.tabControl_InputData.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tabControl_InputData.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Left;
		this.tabControl_InputData.HeaderOrientation = DevExpress.XtraTab.TabOrientation.Vertical;
		this.tabControl_InputData.IsSearchControl = false;
		this.tabControl_InputData.Location = new System.Drawing.Point(5, 126);
		this.tabControl_InputData.Name = "tabControl_InputData";
		this.tabControl_InputData.SelectedTabPage = this.xtraTabPageInputData;
		this.tabControl_InputData.Size = new System.Drawing.Size(994, 563);
		this.tabControl_InputData.TabIndex = 13;
		this.tabControl_InputData.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPageInputData, this.tabControl_OutputData });
		this.tabControl_OutputData.Controls.Add(this.txtOutputType);
		this.tabControl_OutputData.Controls.Add(this.lbOutputReturnType);
		this.tabControl_OutputData.Controls.Add(this.txtOurtPutQuery);
		this.tabControl_OutputData.Controls.Add(this.txtOutputParamater);
		this.tabControl_OutputData.Controls.Add(this.lbOutPutQueryParamater);
		this.tabControl_OutputData.Controls.Add(this.lbOutPutQuery);
		this.tabControl_OutputData.FieldName = "";
		this.tabControl_OutputData.IsSearchControl = false;
		this.tabControl_OutputData.Name = "tabControl_OutputData";
		this.tabControl_OutputData.Size = new System.Drawing.Size(968, 561);
		this.tabControl_OutputData.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabControl_OutputData.Text = "Output Data";
		this.txtOutputType.FieldName = null;
		this.txtOutputType.IsSearchControl = false;
		this.txtOutputType.Location = new System.Drawing.Point(348, 151);
		this.txtOutputType.Name = "txtOutputType";
		this.txtOutputType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtOutputType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Key", "Key"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Value")
		});
		this.txtOutputType.Properties.DisplayMember = "Value";
		this.txtOutputType.Properties.NullText = "";
		this.txtOutputType.Properties.ShowHeader = false;
		this.txtOutputType.Properties.ValueMember = "Key";
		this.txtOutputType.Size = new System.Drawing.Size(310, 20);
		this.txtOutputType.TabIndex = 7;
		this.lbOutputReturnType.FieldName = null;
		this.lbOutputReturnType.HideWhenEmpty = false;
		this.lbOutputReturnType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbOutputReturnType.Location = new System.Drawing.Point(227, 154);
		this.lbOutputReturnType.Name = "lbOutputReturnType";
		this.lbOutputReturnType.ShowUnderLine = true;
		this.lbOutputReturnType.Size = new System.Drawing.Size(120, 20);
		this.lbOutputReturnType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbOutputReturnType.TabIndex = 0;
		this.lbOutputReturnType.Utext = "Output Type";
		this.txtOurtPutQuery.FieldName = null;
		this.txtOurtPutQuery.IsSearchControl = false;
		this.txtOurtPutQuery.Location = new System.Drawing.Point(348, 226);
		this.txtOurtPutQuery.Name = "txtOurtPutQuery";
		this.txtOurtPutQuery.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtOurtPutQuery.Size = new System.Drawing.Size(310, 20);
		this.txtOurtPutQuery.TabIndex = 5;
		this.txtOutputParamater.FieldName = null;
		this.txtOutputParamater.IsSearchControl = false;
		this.txtOutputParamater.Location = new System.Drawing.Point(348, 189);
		this.txtOutputParamater.Name = "txtOutputParamater";
		this.txtOutputParamater.Properties.ReadOnly = true;
		this.txtOutputParamater.Size = new System.Drawing.Size(310, 20);
		this.txtOutputParamater.TabIndex = 4;
		this.lbOutPutQueryParamater.FieldName = null;
		this.lbOutPutQueryParamater.HideWhenEmpty = false;
		this.lbOutPutQueryParamater.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbOutPutQueryParamater.Location = new System.Drawing.Point(227, 192);
		this.lbOutPutQueryParamater.Name = "lbOutPutQueryParamater";
		this.lbOutPutQueryParamater.ShowUnderLine = true;
		this.lbOutPutQueryParamater.Size = new System.Drawing.Size(120, 20);
		this.lbOutPutQueryParamater.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbOutPutQueryParamater.TabIndex = 0;
		this.lbOutPutQueryParamater.Utext = "Paramater Name";
		this.lbOutPutQuery.FieldName = null;
		this.lbOutPutQuery.HideWhenEmpty = false;
		this.lbOutPutQuery.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbOutPutQuery.Location = new System.Drawing.Point(227, 229);
		this.lbOutPutQuery.Name = "lbOutPutQuery";
		this.lbOutPutQuery.ShowUnderLine = true;
		this.lbOutPutQuery.Size = new System.Drawing.Size(120, 20);
		this.lbOutPutQuery.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbOutPutQuery.TabIndex = 0;
		this.lbOutPutQuery.Utext = "Output Query";
		this.xtraTabPageInputData.Controls.Add(this.tabControl_Main);
		this.xtraTabPageInputData.FieldName = "";
		this.xtraTabPageInputData.IsSearchControl = false;
		this.xtraTabPageInputData.Name = "xtraTabPageInputData";
		this.xtraTabPageInputData.Size = new System.Drawing.Size(968, 561);
		this.xtraTabPageInputData.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageInputData.Text = "Input Data";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1001, 724);
		this.ContextMenuStrip = this.contextMenuStrip1;
		base.Controls.Add(this.tabControl_InputData);
		base.Controls.Add(this.txt_Description);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.btn_ModifyAction);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.btn_Copy);
		base.Controls.Add(this.txt_Uri);
		base.Controls.Add(this.lbl_Uri);
		base.Controls.Add(this.txt_Action);
		base.Controls.Add(this.lbl_Action);
		base.Controls.Add(this.txt_Controller);
		base.Controls.Add(this.labelLine1);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oUriForIncomming";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ContextActionPublishService.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ContextActionPublishService";
		base.ShowInTaskbar = false;
		this.Text = "URI for Incomming Data";
		base.OnControlValueChanged += new Apzon.Controls.Form.ControlValueChanged(this.frm_ContextActionPublishService_OnControlValueChanged);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_ContextActionPublishService_FormClosing);
		base.Load += new System.EventHandler(this.frm_ContextActionPublishService_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_Controller.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Action.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Uri.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl_Main).EndInit();
		this.tabControl_Main.ResumeLayout(false);
		this.xtraTabPageEx1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tree_Input).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnAddChild).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_JsonType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ConfigDataType).EndInit();
		this.contextMenuStripContent.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_Description.Properties).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tabControl_InputData).EndInit();
		this.tabControl_InputData.ResumeLayout(false);
		this.tabControl_OutputData.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtOutputType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtOurtPutQuery.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtOutputParamater.Properties).EndInit();
		this.xtraTabPageInputData.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
