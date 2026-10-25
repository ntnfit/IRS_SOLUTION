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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Forms.Commons;

public class frm_DefineNew : XtraFormEx
{
	private List<int> changedCells = new List<int>();

	private bool isLoading;

	private bool isCancel;

	private bool isInserted;

	private int focusedRow;

	private int idMax;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	protected string FormController = "DefineNew";

	protected string loadActionName = "Load";

	protected string saveActionName = "Save";

	protected string checkActionName = "Check";

	public Action<int> childSend;

	public Func<DataTable> CustomLoad;

	public Func<DataTable, HttpResult> CustomSave;

	public Func<int, DataTable> CustomDelete;

	public Action<GridView> CustomGridView;

	private string tableName = "";

	private List<string> requiredFields = new List<string>();

	private string idField = "";

	private string derivedForm = "";

	private bool hideFirstColumn = true;

	private string rowFilter = "";

	private bool useFormSetting;

	private Dictionary<string, RepositoryItem> dictionRepository = new Dictionary<string, RepositoryItem>();

	private List<string> visibleColumns = new List<string>();

	private List<string> columnCaptions = new List<string>();

	private List<string> ignoredFields = new List<string>();

	private GridHitInfo downHitInfo;

	private IContainer components;

	public SimpleButtonEx btCancel;

	public SimpleButtonEx btnOK;

	public GridControlEx gridControl1;

	public GridViewEx gridView1;

	public GridColumnEx edType;

	public GridColumnEx name;

	public GridColumnEx descriptio;

	public ContextMenuStrip contextMenuStrip1;

	public ToolStripMenuItem removeRowToolStripMenuItem;

	private RepositoryItemColorPickEdit clo_Status;

	public string TableName
	{
		get
		{
			return tableName;
		}
		set
		{
			tableName = value;
			gridView1.ItemId = value;
		}
	}

	public string IdField
	{
		get
		{
			return idField;
		}
		set
		{
			idField = value;
		}
	}

	public string DerivedForm
	{
		get
		{
			return derivedForm;
		}
		set
		{
			derivedForm = value;
		}
	}

	public GridViewEx View => gridView1;

	public bool HideFirstColumn
	{
		get
		{
			return hideFirstColumn;
		}
		set
		{
			hideFirstColumn = value;
		}
	}

	public string CheckTables { get; set; }

	public string CheckFields { get; set; }

	public string RowFilter
	{
		get
		{
			return rowFilter;
		}
		set
		{
			rowFilter = value;
		}
	}

	public bool UseFormSetting
	{
		get
		{
			return useFormSetting;
		}
		set
		{
			useFormSetting = value;
		}
	}

	public Dictionary<string, RepositoryItem> DictionRepository
	{
		get
		{
			return dictionRepository;
		}
		set
		{
			dictionRepository = value;
		}
	}

	public List<string> IgnoredFields
	{
		get
		{
			return ignoredFields;
		}
		set
		{
			ignoredFields = value;
		}
	}

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

	public void RequiredFields(params string[] fields)
	{
		foreach (string item in fields)
		{
			requiredFields.Add(item);
		}
	}

	public void VisibleColumns(params string[] columns)
	{
		foreach (string item in columns)
		{
			visibleColumns.Add(item);
		}
	}

	public void ColumnCaptions(params string[] captions)
	{
		foreach (string item in captions)
		{
			columnCaptions.Add(item);
		}
	}

	public frm_DefineNew()
	{
		if (!string.IsNullOrEmpty(DerivedForm))
		{
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == DerivedForm))
			{
				throw new Exception(Resources.Msg_FormAccessDenied);
			}
			InitializeComponent();
			SetupFormControls();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == DerivedForm && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
		}
		else
		{
			InitializeComponent();
			SetupFormControls();
		}
		FormId = "oDefineNew";
		gridView1.OptionsBehavior.KeepFocusedRowOnUpdate = true;
		gridView1.OptionsView.ColumnAutoWidth = true;
	}

	public virtual void SetupFormControls()
	{
	}

	public virtual DataTable LoadData()
	{
		try
		{
			if (CustomLoad != null)
			{
				return CustomLoad();
			}
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			if (string.IsNullOrEmpty(TableName))
			{
				return new DataTable();
			}
			if (FormController.Equals("DefineNew"))
			{
				DataTable dataTable = _httpClient.DefineNewClient.Load(TableName);
				if (!dataTable.HasColumn())
				{
					dataTable = WinFuntions.GetTableStructure(TableName);
				}
				if (!string.IsNullOrEmpty(rowFilter))
				{
					DataRow[] array = dataTable.Select(rowFilter);
					dataTable = ((array.Length == 0) ? dataTable.Clone() : array.CopyToDataTable());
				}
				if (TableName == "APZ_OTIS")
				{
					dataTable.DefaultView.Sort = "VisOrder ASC";
				}
				return dataTable;
			}
			DataTable dataTable2 = httpClientHelper.Get(loadActionName);
			if (!dataTable2.HasColumn())
			{
				dataTable2 = WinFuntions.GetTableStructure(TableName);
			}
			if (!string.IsNullOrEmpty(rowFilter))
			{
				DataRow[] array2 = dataTable2.Select(rowFilter);
				dataTable2 = ((array2.Length == 0) ? dataTable2.Clone() : array2.CopyToDataTable());
			}
			return dataTable2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public virtual async Task<DataTable> LoadDataAsync()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			if (string.IsNullOrEmpty(TableName))
			{
				return new DataTable();
			}
			if (FormController.Equals("DefineNew"))
			{
				DataTable dataTable = await _httpClient.DefineNewClient.LoadAsync(TableName);
				if (!dataTable.HasColumn())
				{
					dataTable = WinFuntions.GetTableStructure(TableName);
				}
				if (!string.IsNullOrEmpty(rowFilter))
				{
					DataRow[] array = dataTable.Select(rowFilter);
					dataTable = ((array.Length == 0) ? dataTable.Clone() : array.CopyToDataTable());
				}
				if (TableName == "APZ_OTIS")
				{
					dataTable.DefaultView.Sort = "VisOrder ASC";
				}
				return dataTable;
			}
			DataTable dataTable2 = helper.Get(loadActionName);
			if (!dataTable2.HasColumn())
			{
				dataTable2 = WinFuntions.GetTableStructure(TableName);
			}
			if (!string.IsNullOrEmpty(rowFilter))
			{
				DataRow[] array2 = dataTable2.Select(rowFilter);
				dataTable2 = ((array2.Length == 0) ? dataTable2.Clone() : array2.CopyToDataTable());
			}
			return dataTable2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public virtual HttpResult SaveResult(DataTable table, string actionName)
	{
		try
		{
			using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
			if (string.IsNullOrEmpty(TableName))
			{
				return new HttpResult();
			}
			if (FormController.Equals("DefineNew"))
			{
				Tuple<DataTable, string, string> dt = new Tuple<DataTable, string, string>(table, TableName, IdField);
				return _httpClient.DefineNewClient.Save(dt);
			}
			return httpClientHelper.Post(actionName, new
			{
				dt = table
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new HttpResult(Enums.MessageCode.Error, "Error");
		}
	}

	public virtual DataTable CheckExists(int _id)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			if (string.IsNullOrEmpty(TableName))
			{
				return new DataTable();
			}
			if (FormController.Equals("DefineNew"))
			{
				return _httpClient.DefineNewClient.Check(_id, CheckTables, CheckFields);
			}
			return httpClientHelper.Get(checkActionName, new
			{
				Id = _id
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public virtual DataTable CheckExistsStausTrackingIssue(int _id)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			if (string.IsNullOrEmpty(TableName))
			{
				return new DataTable();
			}
			if (FormController.Equals("DefineNew"))
			{
				return _httpClient.DefineNewClient.CheckStatusTrackingIssue(_id, "APZ_OTIS");
			}
			return httpClientHelper.Get(checkActionName, new
			{
				Id = _id
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public virtual DataTable LoadDataAndSetField()
	{
		try
		{
			DataTable dataTable = LoadData();
			DefaultSettingFields(dataTable);
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private void DefaultSettingFields(DataTable dt)
	{
		try
		{
			if (dt.HasColumn())
			{
				if (string.IsNullOrEmpty(IdField))
				{
					string columnName = dt.Columns[0].ColumnName;
					IdField = columnName;
				}
				idMax = Function.ParseInt((from s in dt.AsEnumerable()
					where Function.ToString(s[IdField]) != ""
					select s).Max((DataRow s) => s[IdField]));
				if (requiredFields.Count == 0)
				{
					string columnName2 = dt.Columns[1].ColumnName;
					requiredFields.Add(columnName2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual string SaveData()
	{
		try
		{
			DataTable dataTable = gridControl1.DataSource as DataTable;
			dataTable.AcceptChanges();
			if (dataTable.IsNotNull())
			{
				foreach (DataRow item in dataTable.AsEnumerable())
				{
					if (Function.ToString(item[IdField]) == "" && Function.ToString(item[requiredFields[0]]) != "")
					{
						item[IdField] = 0;
						isInserted = true;
					}
				}
				DataRow[] array = (from s in dataTable.AsEnumerable()
					where Function.ToString(s[requiredFields[0]]) != ""
					select s).ToArray();
				if (array.Length == 0)
				{
					gridControl1.DataSource = dataTable.Clone();
					dataTable = dataTable.Clone();
					dataTable.AcceptChanges();
				}
				else
				{
					dataTable = array.CopyToDataTable();
					dataTable.AcceptChanges();
				}
				HttpResult httpResult = ((CustomSave == null) ? SaveResult(dataTable, saveActionName) : CustomSave(dataTable));
				if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
				{
					focusedRow = gridView1.FocusedRowHandle;
					gridControl1.DataSource = LoadData();
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					return "success";
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return "";
			}
			WinCommons.Error(Enums.MessageCode.ThisEntryIsNull, "");
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return "";
		}
	}

	private void CheckSaveState()
	{
		if (!isLoading)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	public virtual async Task REFRESH_DATA()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.No)
			{
				frm_DefineNew_Load(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual async void frm_DefineNew_Load(object sender, EventArgs e)
	{
		try
		{
			if (isLoading)
			{
				return;
			}
			isLoading = true;
			DataTable dataTable = LoadData();
			DefaultSettingFields(dataTable);
			gridControl1.DataSource = dataTable;
			gridView1.PopulateColumns();
			if (gridView1.Columns.Count > 0)
			{
				if (DictionRepository.Count > 0)
				{
					foreach (KeyValuePair<string, RepositoryItem> item in DictionRepository)
					{
						string key = item.Key;
						RepositoryItem value = item.Value;
						if (gridView1.Columns[key] != null)
						{
							gridView1.Columns[key].ColumnEdit = value;
						}
					}
				}
				if (visibleColumns.Count > 0)
				{
					foreach (GridColumn column in gridView1.Columns)
					{
						column.Visible = false;
					}
					for (int num = visibleColumns.Count - 1; num >= 0; num--)
					{
						if (gridView1.Columns[visibleColumns[num]] != null)
						{
							gridView1.Columns[visibleColumns[num]].Visible = true;
						}
					}
				}
				else if (HideFirstColumn)
				{
					gridView1.Columns[0].Visible = false;
				}
				if (TableName == "APZ_OTIS")
				{
					gridView1.Columns["VisOrder"].Visible = false;
				}
				if (columnCaptions.Count > 0 && columnCaptions.Count >= gridView1.Columns.Count)
				{
					for (int i = 0; i < columnCaptions.Count; i++)
					{
						gridView1.Columns[i].Caption = columnCaptions[i];
					}
				}
				gridView1.AddNewRow();
				if (CustomGridView != null)
				{
					CustomGridView(gridView1);
				}
			}
			isLoading = false;
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void btnOK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			gridView1.ClearSorting();
			if (!gridView1.ValidateFields(requiredFields, idField, ignoredFields) || SaveData() != "success")
			{
				return;
			}
			gridView1.FocusedRowHandle = focusedRow;
			int obj;
			if (changedCells.Count > 0)
			{
				if (isInserted)
				{
					obj = idMax + 1;
					isInserted = false;
				}
				else
				{
					for (int i = 0; i < changedCells.Count; i++)
					{
						int value = Function.ParseInt(gridView1.GetRowCellValue(changedCells[i], IdField));
						changedCells[i] = value;
					}
					obj = changedCells.Max();
				}
			}
			else
			{
				obj = Function.ParseInt(gridView1.GetFocusedRowCellValue(IdField));
			}
			if (childSend != null)
			{
				childSend(obj);
			}
			changedCells.Clear();
			gridView1.AddNewRow();
			gridView1.MakeRowVisible(gridView1.DataRowCount - 1);
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(checkActionName))
			{
				return;
			}
			if ((gridControl1.DataSource as DataTable).IsNotNull())
			{
				int num = Function.ParseInt(gridView1.GetFocusedRowCellValue(IdField));
				if (num > 0)
				{
					DataTable dataTable;
					if (CustomDelete != null)
					{
						dataTable = CustomDelete(num);
					}
					else if (TableName == "APZ_OTIS")
					{
						if (num == 1 || num == 8)
						{
							WinCommons.Error(Enums.MessageCode.CannotDeleteThisRecordIsBeingUsed, "");
							return;
						}
						dataTable = CheckExistsStausTrackingIssue(num);
					}
					else
					{
						dataTable = CheckExists(num);
					}
					if (dataTable.IsNotNull())
					{
						if (!dataTable.ExistsColumn("Cancel") || !(Function.ToString(dataTable.Rows[0]["Cancel"]) == "Y"))
						{
							if (dataTable.ExistsColumn("Error"))
							{
								string msg = Function.ToString(dataTable.Rows[0]["Error"]);
								WinCommons.Error(Enums.MessageCode.None, msg);
							}
							else
							{
								WinCommons.Error(Enums.MessageCode.CannotDeleteThisRecordIsBeingUsed, "");
							}
						}
						return;
					}
				}
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

	private void frm_HREducationTypes_FormClosing(object sender, FormClosingEventArgs e)
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

	private void btCancel_Click(object sender, EventArgs e)
	{
		isCancel = true;
		Close();
	}

	private void gridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (isLoading)
		{
			return;
		}
		bool flag = false;
		foreach (string requiredField in requiredFields)
		{
			if (Function.ToString(gridView1.GetRowCellValue(e.RowHandle, requiredField)) == "")
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			if (string.Equals(TableName, "APZ_OTIS") && e.Column.FieldName == "Name" && !string.IsNullOrEmpty(Function.ToString(e.Value)))
			{
				int num = Function.ParseInt(gridControl1.DataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["VisOrder"]))) + 1;
				gridView1.SetRowCellValue(e.RowHandle, "VisOrder", num);
			}
			changedCells.Add(e.RowHandle);
		}
		CheckSaveState();
	}

	private void gridView1_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		CheckSaveState();
	}

	private void gridControl1_EditorKeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			gridView1.PostEditor();
			btnOK_Click(null, null);
		}
	}

	private void gridControl1_DragDrop(object sender, DragEventArgs e)
	{
		if (!string.Equals(TableName, "APZ_OTIS"))
		{
			return;
		}
		try
		{
			if (!(sender is GridControlEx gridControlEx))
			{
				return;
			}
			DataTable sourceTable = gridControlEx.DataSource as DataTable;
			GridViewEx gridViewEx = gridControlEx.MainView as GridViewEx;
			GridHitInfo gridHitInfo = e.Data.GetData(typeof(GridHitInfo)) as GridHitInfo;
			if (gridViewEx != null)
			{
				GridHitInfo gridHitInfo2 = gridViewEx.CalcHitInfo(gridControlEx.PointToClient(new Point(e.X, e.Y)));
				if (gridHitInfo != null)
				{
					int rowHandle = gridHitInfo.RowHandle;
					int rowHandle2 = gridHitInfo2.RowHandle;
					MoveRow(rowHandle, rowHandle2, sourceTable);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridControl1_DragOver(object sender, DragEventArgs e)
	{
		if (!string.Equals(TableName, "APZ_OTIS"))
		{
			return;
		}
		try
		{
			if (e.Data.GetDataPresent(typeof(GridHitInfo)) && e.Data.GetData(typeof(GridHitInfo)) is GridHitInfo gridHitInfo && sender is GridControlEx { MainView: GridViewEx mainView } gridControlEx)
			{
				GridHitInfo gridHitInfo2 = mainView.CalcHitInfo(gridControlEx.PointToClient(new Point(e.X, e.Y)));
				if (gridHitInfo2.InRow && gridHitInfo2.RowHandle != gridHitInfo.RowHandle && gridHitInfo2.RowHandle != -2147483647)
				{
					e.Effect = DragDropEffects.Move;
				}
				else
				{
					e.Effect = DragDropEffects.None;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void MoveRow(int sourceRow, int targetRow, DataTable sourceTable)
	{
		if (!string.Equals(TableName, "APZ_OTIS"))
		{
			return;
		}
		try
		{
			if (sourceRow == targetRow)
			{
				return;
			}
			DataRow dataRow = sourceTable.Rows[sourceRow];
			DataRow dataRow2 = sourceTable.NewRow();
			dataRow2.ItemArray = dataRow.ItemArray;
			sourceTable.Rows.Remove(dataRow);
			sourceTable.Rows.InsertAt(dataRow2, targetRow);
			if (sourceTable.IsNotNull())
			{
				int num = 1;
				foreach (DataRow item in (from t in sourceTable.AsEnumerable()
					select (t)).ToList())
				{
					if (!string.IsNullOrEmpty(Function.ToString(item["Name"])))
					{
						item["VisOrder"] = num;
						num++;
					}
				}
			}
			sourceTable.AcceptChanges();
			gridView1.FocusedRowHandle = targetRow;
			CheckSaveState();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridView1_MouseDown(object sender, MouseEventArgs e)
	{
		if (!string.Equals(TableName, "APZ_OTIS"))
		{
			return;
		}
		try
		{
			GridViewEx gridViewEx = sender as GridViewEx;
			downHitInfo = null;
			if (gridViewEx != null)
			{
				GridHitInfo gridHitInfo = gridViewEx.CalcHitInfo(new Point(e.X, e.Y));
				if (Control.ModifierKeys == Keys.None && e.Button == MouseButtons.Left && gridHitInfo.InRow && gridHitInfo.RowHandle != -2147483647)
				{
					downHitInfo = gridHitInfo;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridView1_MouseMove(object sender, MouseEventArgs e)
	{
		if (!string.Equals(TableName, "APZ_OTIS"))
		{
			return;
		}
		try
		{
			GridViewEx gridViewEx = sender as GridViewEx;
			if (e.Button == MouseButtons.Left && downHitInfo != null)
			{
				Size dragSize = SystemInformation.DragSize;
				Rectangle rectangle = new Rectangle(new Point(downHitInfo.HitPoint.X - dragSize.Width / 2, downHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);
				if (!rectangle.Contains(new Point(e.X, e.Y)))
				{
					gridViewEx?.GridControl.DoDragDrop(downHitInfo, DragDropEffects.All);
					downHitInfo = null;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GridView1_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		if (!string.Equals(TableName, "APZ_OTIS"))
		{
			return;
		}
		try
		{
			if (e.Column.FieldName == "Color")
			{
				e.RepositoryItem = clo_Status;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.frm_DefineNew));
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.gridControl1 = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.edType = new Apzon.Controls.Grid.GridColumnEx();
		this.name = new Apzon.Controls.Grid.GridColumnEx();
		this.descriptio = new Apzon.Controls.Grid.GridColumnEx();
		this.clo_Status = new DevExpress.XtraEditors.Repository.RepositoryItemColorPickEdit();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.clo_Status).BeginInit();
		base.SuspendLayout();
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(93, 343);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 12;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.EditControl = null;
		this.btnOK.EditValue = "OK";
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(12, 343);
		this.btnOK.Name = "btnOK";
		this.btnOK.ReadOnly = false;
		this.btnOK.SettingInVisible = false;
		this.btnOK.SettingReadonly = false;
		this.btnOK.Size = new System.Drawing.Size(75, 23);
		this.btnOK.TabIndex = 11;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		this.gridControl1.AllowDrop = true;
		this.gridControl1.AllowPasteExcel = true;
		this.gridControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControl1.ClipboardData = null;
		this.gridControl1.ContextMenuStrip = this.contextMenuStrip1;
		this.gridControl1.Location = new System.Drawing.Point(0, 0);
		this.gridControl1.MainView = this.gridView1;
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.clo_Status });
		this.gridControl1.Size = new System.Drawing.Size(608, 337);
		this.gridControl1.TabIndex = 10;
		this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView1 });
		this.gridControl1.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.gridControl1_EditorKeyDown);
		this.gridControl1.DragDrop += new System.Windows.Forms.DragEventHandler(this.gridControl1_DragDrop);
		this.gridControl1.DragOver += new System.Windows.Forms.DragEventHandler(this.gridControl1_DragOver);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(144, 26);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = null;
		this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.edType, this.name, this.descriptio });
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = "oDefinedNew";
		this.gridView1.GridControl = this.gridControl1;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 30;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = "ODATA";
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.OptionsView.ShowGroupPanel = false;
		this.gridView1.UseBatchMod = false;
		this.gridView1.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.GridView1_CustomRowCellEdit);
		this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
		this.gridView1.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanging);
		this.gridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridView1_MouseDown);
		this.gridView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gridView1_MouseMove);
		this.edType.Caption = "edType";
		this.edType.CurrencyDateField = null;
		this.edType.CurrencyField = null;
		this.edType.CurrencyFixed = null;
		this.edType.CurrencyFixedRate = 0.0;
		this.edType.CurrencyRateField = null;
		this.edType.FieldName = "edType";
		this.edType.Name = "edType";
		this.edType.PrimaryKey = true;
		this.edType.UnitField = null;
		this.name.Caption = "Name";
		this.name.CurrencyDateField = null;
		this.name.CurrencyField = null;
		this.name.CurrencyFixed = null;
		this.name.CurrencyFixedRate = 0.0;
		this.name.CurrencyRateField = null;
		this.name.FieldName = "name";
		this.name.Name = "name";
		this.name.PrimaryKey = false;
		this.name.UnitField = null;
		this.name.Visible = true;
		this.name.VisibleIndex = 0;
		this.name.Width = 212;
		this.descriptio.Caption = "Description";
		this.descriptio.CurrencyDateField = null;
		this.descriptio.CurrencyField = null;
		this.descriptio.CurrencyFixed = null;
		this.descriptio.CurrencyFixedRate = 0.0;
		this.descriptio.CurrencyRateField = null;
		this.descriptio.FieldName = "descriptio";
		this.descriptio.Name = "descriptio";
		this.descriptio.PrimaryKey = false;
		this.descriptio.UnitField = null;
		this.descriptio.Visible = true;
		this.descriptio.VisibleIndex = 1;
		this.descriptio.Width = 370;
		this.clo_Status.AutoHeight = false;
		this.clo_Status.AutomaticColor = System.Drawing.Color.Black;
		this.clo_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.clo_Status.Name = "clo_Status";
		this.clo_Status.StoreColorAsInteger = true;
		base.AcceptButton = this.btnOK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btCancel;
		base.ClientSize = new System.Drawing.Size(608, 378);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.gridControl1);
		base.FormId = "oDefinedNew";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_DefineNew";
		base.ShowInTaskbar = false;
		this.Text = "Define New";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_HREducationTypes_FormClosing);
		base.Load += new System.EventHandler(this.frm_DefineNew_Load);
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.clo_Status).EndInit();
		base.ResumeLayout(false);
	}
}
