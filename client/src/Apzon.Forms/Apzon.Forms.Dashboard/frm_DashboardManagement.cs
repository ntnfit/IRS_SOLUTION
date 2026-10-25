using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Common;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;

namespace Apzon.Forms.Dashboard;

public class frm_DashboardManagement : XtraFormEx
{
	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string AddString = "Add";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataTable _dtQuery;

	private bool _isLoad;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private TreeList tree_List;

	private LabelLine lab_Name;

	private LabelLine labelLine4;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private LabelLine lab_File;

	private SimpleButtonEx btn_Update;

	private SimpleButtonEx btn_ExportLayout;

	private SimpleButtonEx btn_Browse;

	private CheckBox chk_Valid;

	private TextEditEx txt_Name;

	private TextEditEx txt_Description;

	private TextEditEx txt_File;

	private TreeListColumnEx tc_Description;

	private TreeListColumnEx tc_Id;

	private TreeListColumnEx tc_Name;

	private TreeListColumnEx tc_ParentId;

	private TreeListColumnEx tc_DetailId;

	private LookUpEdit luk_Category;

	private TextEditEx txt_layout;

	private TextEditEx txt_query;

	private TextEditEx txt_sqlType;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem removeToolStripMenuItem;

	private TextEditEx txt_layoutDefault;

	private TreeListColumnEx tc_ChildId;

	public frm_DashboardManagement()
	{
		_dtQuery = new DataTable();
		_dtQuery.Columns.Add("DetailId", typeof(string));
		_dtQuery.Columns.Add("Name", typeof(string));
		_dtQuery.Columns.Add("Type", typeof(string));
		_dtQuery.Columns.Add("Value", typeof(string));
		InitializeComponent();
		OnLoadAsync += frm_DashboardManagement_Load;
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				btn_Update.Text = "Add";
				btn_ExportLayout.Visible = false;
				break;
			case Enums.FormViewModeType.Update:
				btn_Update.Text = "Update";
				btn_ExportLayout.Visible = true;
				break;
			case Enums.FormViewModeType.View:
				btn_Update.Text = "OK";
				btn_ExportLayout.Visible = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_Valid_CheckedChanged(object sender, EventArgs e)
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

	private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
	{
	}

	private void tree_List_BeforeFocusNode(object sender, BeforeFocusNodeEventArgs e)
	{
		if (e.Node.Id == (sender as TreeList).Nodes[0].Id)
		{
			removeToolStripMenuItem.Visible = false;
		}
		else
		{
			removeToolStripMenuItem.Visible = true;
		}
	}

	private async Task frm_DashboardManagement_Load(object sender, EventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			InitChangeDataEvent();
			await LoadListCategories();
			LoadLookUpCategories();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void tree_List_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		try
		{
			_isLoad = true;
			int num = Function.ParseInt(tree_List.GetFocusedRowCellValue(tc_DetailId));
			if (Function.ParseInt(tree_List.GetFocusedRowCellValue(tc_ChildId)) == -1)
			{
				removeToolStripMenuItem.Visible = false;
			}
			else
			{
				removeToolStripMenuItem.Visible = true;
			}
			if (num > 0)
			{
				DataSet dataSet = await _httpClient.CategoriesDABClient.GetDetailByDasId(num, GlobalData.UserID);
				if (dataSet != null && dataSet.Tables.Count > 0)
				{
					DataTable dataTable = dataSet.Tables[0];
					if (dataTable.Rows.Count > 0 && dataTable != null)
					{
						txt_File.Text = Path.GetFileName(Function.ToString(dataTable.Rows[0]["PatchFile"]));
						txt_Name.Text = Function.ToString(dataTable.Rows[0]["Name"]);
						txt_Description.EditValue = Function.ToString(dataTable.Rows[0]["Description"]);
						chk_Valid.Checked = Function.ParseBool(dataTable.Rows[0]["ValidFor"]);
						luk_Category.EditValue = Function.ParseInt(dataTable.Rows[0]["CatId"]);
						txt_query.EditValue = Function.ToString(dataTable.Rows[0]["Query"]);
						txt_sqlType.EditValue = Function.ToString(dataTable.Rows[0]["SqlType"]);
						txt_layout.EditValue = Function.ToString(dataTable.Rows[0]["Layout"]);
						txt_layoutDefault.EditValue = Function.ToString(dataTable.Rows[0]["LayoutDefault"]);
					}
					_dtQuery = dataSet.Tables[1];
				}
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				REFRESH();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoad = false;
	}

	private void btn_Browse_Click(object sender, EventArgs e)
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				DefaultExt = ".xml",
				Multiselect = false,
				Filter = "Dashboard Layout (.xml)|*.xml"
			};
			openFileDialog.ShowDialog(this);
			if (!string.IsNullOrEmpty(openFileDialog.FileName))
			{
				txt_File.EditValue = openFileDialog.FileName;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Update_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Add:
				await Add();
				break;
			case Enums.FormViewModeType.Update:
				await UpdateDashboard();
				break;
			case Enums.FormViewModeType.View:
				Close();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void txt_File_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (_isLoad)
			{
				return;
			}
			string text = Function.ToString(txt_File.EditValue);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			XDocument xDocument = XDocument.Parse(File.ReadAllText(text ?? ""));
			txt_layout.EditValue = xDocument.ToString();
			XDocument xDocument2 = XDocument.Parse(xDocument.ToString());
			string text2 = "";
			if (xDocument2.Element("Dashboard").Element("Title") != null)
			{
				txt_Name.EditValue = xDocument2.Element("Dashboard").Element("Title").Attribute("Text")
					.Value;
			}
			if (xDocument2.Element("Dashboard").Element("DataSources").Element("SqlDataSource") == null)
			{
				return;
			}
			txt_layoutDefault.EditValue = xDocument2.Element("Dashboard").Element("DataSources").Element("SqlDataSource");
			string value = xDocument2.Element("Dashboard").Element("DataSources").Element("SqlDataSource")
				.Element("Query")
				.Attribute("Type")
				.Value;
			if (string.Equals(value, "SelectQuery"))
			{
				string value2 = xDocument2.Element("Dashboard").Element("DataSources").Element("SqlDataSource")
					.Element("Query")
					.Element("Tables")
					.Element("Table")
					.Attribute("Name")
					.Value;
				IEnumerable<XElement> enumerable = xDocument2.Descendants("Columns").Elements("Column");
				string text3 = "";
				List<string> list = new List<string>();
				foreach (XElement item in enumerable)
				{
					list.Add(Function.ToString(item.Attribute("Name").Value));
				}
				text3 = string.Join(",", list.ToArray());
				text2 = "select " + text3 + " from " + value2;
				foreach (XElement item2 in xDocument2.Descendants("Query").Elements("Parameter"))
				{
					DataRow dataRow = _dtQuery.NewRow();
					dataRow["Name"] = Function.ToString(item2.Attribute("Name").Value).Substring(1);
					dataRow["Value"] = Function.ToString(item2.Value);
					dataRow["Type"] = Function.ToString(item2.Attribute("Type").Value);
					_dtQuery.Rows.Add(dataRow);
				}
				_dtQuery.AcceptChanges();
				txt_query.EditValue = text2;
				txt_sqlType.EditValue = "T";
			}
			if (!string.Equals(value, "StoredProcQuery"))
			{
				return;
			}
			string value3 = xDocument2.Element("Dashboard").Element("DataSources").Element("SqlDataSource")
				.Element("Query")
				.Element("ProcName")
				.Value;
			foreach (XElement item3 in xDocument2.Descendants("Query").Elements("Parameter"))
			{
				DataRow dataRow2 = _dtQuery.NewRow();
				dataRow2["DetailId"] = Function.ToString(tree_List.GetFocusedRowCellValue(tc_DetailId));
				dataRow2["Name"] = Function.ToString(item3.Attribute("Name").Value).Substring(1);
				dataRow2["Value"] = Function.ToString(item3.Value);
				dataRow2["Type"] = Function.ToString(item3.Attribute("Type").Value);
				_dtQuery.Rows.Add(dataRow2);
			}
			_dtQuery.AcceptChanges();
			txt_query.EditValue = value3;
			txt_sqlType.EditValue = "S";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_ExportLayout_Click(object sender, EventArgs e)
	{
		try
		{
			Function.ToString(tree_List.GetFocusedRowCellValue(tc_DetailId));
			XDocument xDocument = XDocument.Parse(Function.ToString(txt_layoutDefault.EditValue));
			string text = string.Empty;
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = string.Format("Files (*.{0}*)|*.{0}", "xml"),
				FilterIndex = 1
			};
			if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				text = saveFileDialog.FileName;
			}
			if (FileIO.FileExist(text))
			{
				FileIO.DeleteFile(text);
			}
			xDocument.Save(text);
			if (!string.IsNullOrEmpty(text) && FileIO.FileExist(text))
			{
				try
				{
					Process.Start(text);
					return;
				}
				catch
				{
					WinCommons.Error(Enums.MessageCode.TheFileCouldNotBeOpened, "", Environment.NewLine, text);
					return;
				}
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
		try
		{
			REFRESH();
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
			int num = Function.ParseInt(tree_List.GetFocusedRowCellValue(tc_DetailId));
			Function.ToString(tree_List.GetFocusedRowCellValue(tc_Id));
			if (num > 0)
			{
				Remove(num, GlobalData.UserID);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_Name_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string value = Function.ToString(txt_layout.EditValue);
			if (!string.IsNullOrEmpty(value))
			{
				XDocument xDocument = XDocument.Parse(value);
				if (xDocument.Element("Dashboard").Element("Title") != null)
				{
					xDocument.Element("Dashboard").Element("Title").Attribute("Text")
						.Value = Function.ToString(txt_Name.EditValue);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadListCategories()
	{
		try
		{
			DataTable dataSource = await _httpClient.CategoriesDABClient.GetTreeListCategory("D", GlobalData.UserID);
			tree_List.DataSource = dataSource;
			tree_List.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void CheckChangedData()
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
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Control_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			CheckChangedData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetChangedEvent(Control control)
	{
		try
		{
			if (control is BaseEdit)
			{
				((BaseEdit)control).EditValueChanging += Control_EditValueChanged;
				return;
			}
			if (control is GridControl)
			{
				BaseView defaultView = ((GridControl)control).DefaultView;
				if (defaultView is GridView)
				{
					((GridView)defaultView).CellValueChanging += Control_EditValueChanged;
				}
				return;
			}
			foreach (Control control2 in control.Controls)
			{
				SetChangedEvent(control2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void InitChangeDataEvent()
	{
		try
		{
			foreach (Control control in Controls)
			{
				SetChangedEvent(control);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadLookUpCategories()
	{
		try
		{
			DataTable dataSource = await _httpClient.CategoriesDABClient.GetLookupCategories();
			luk_Category.Properties.DataSource = dataSource;
			luk_Category.Properties.DisplayMember = "Name";
			luk_Category.Properties.ValueMember = "Id";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			RefeshForm();
			luk_Category.EditValue = Function.ParseInt(tree_List.GetFocusedRowCellValue(tc_Id));
			chk_Valid.Checked = true;
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
			FormMode = Enums.FormViewModeType.View;
			await LoadListCategories();
			LoadLookUpCategories();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RefeshForm()
	{
		try
		{
			txt_Description.EditValue = null;
			txt_File.EditValue = null;
			txt_layout.EditValue = null;
			txt_Name.EditValue = null;
			txt_query.EditValue = null;
			txt_sqlType.EditValue = null;
			luk_Category.EditValue = null;
			_dtQuery.Rows.Clear();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> Add()
	{
		_ = 1;
		try
		{
			DataSet dataSet = new DataSet();
			WinCommons._OpenProcessing();
			string empty = string.Empty;
			string value = Function.ToString(txt_File.EditValue);
			string value2 = Function.ToString(txt_Name.EditValue);
			string value3 = Function.ToString(txt_Description.EditValue);
			bool flag = Function.ParseBool(chk_Valid.Checked);
			string value4 = Function.ToString(txt_layout.EditValue);
			int num = Function.ParseInt(luk_Category.EditValue);
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Id", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Columns.Add("CatId", typeof(int));
			dataTable.Columns.Add("UserSign", typeof(int));
			dataTable.Columns.Add("Layout", typeof(string));
			dataTable.Columns.Add("ValidFor", typeof(bool));
			dataTable.Columns.Add("SqlType", typeof(string));
			dataTable.Columns.Add("Query", typeof(string));
			dataTable.Columns.Add("PatchFile", typeof(string));
			dataTable.Columns.Add("Description", typeof(string));
			dataTable.Columns.Add("LayoutDefault", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Id"] = empty;
			dataRow["Name"] = value2;
			dataRow["CatId"] = num;
			dataRow["UserSign"] = GlobalData.UserID;
			dataRow["Layout"] = value4;
			dataRow["ValidFor"] = flag;
			dataRow["SqlType"] = Function.ToString(txt_sqlType.EditValue);
			dataRow["Query"] = Function.ToString(txt_query.EditValue);
			dataRow["PatchFile"] = value;
			dataRow["Description"] = value3;
			dataRow["LayoutDefault"] = Function.ToString(txt_layoutDefault.EditValue);
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			dataTable.TableName = "detail";
			_dtQuery.TableName = "query";
			dataSet.Tables.Add(dataTable.Copy());
			dataSet.Tables.Add(_dtQuery.Copy());
			HttpResult httpResult = await _httpClient.DashboardDetail.AddDetailInForm(dataSet);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				await LoadListCategories();
				btn_ExportLayout.Visible = true;
				FormMode = Enums.FormViewModeType.View;
				return 0;
			}
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			return 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
	}

	private async Task<int> UpdateDashboard()
	{
		_ = 1;
		try
		{
			DataSet dataSet = new DataSet();
			WinCommons._OpenProcessing();
			string value = Function.ToString(tree_List.GetFocusedRowCellValue(tc_DetailId));
			string value2 = Function.ToString(txt_File.EditValue);
			string value3 = Function.ToString(txt_Name.EditValue);
			string value4 = Function.ToString(txt_Description.EditValue);
			bool flag = Function.ParseBool(chk_Valid.Checked);
			string value5 = Function.ToString(txt_layout.EditValue);
			int num = Function.ParseInt(luk_Category.EditValue);
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Id", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Columns.Add("CatId", typeof(int));
			dataTable.Columns.Add("UserSign", typeof(int));
			dataTable.Columns.Add("Layout", typeof(string));
			dataTable.Columns.Add("ValidFor", typeof(bool));
			dataTable.Columns.Add("SqlType", typeof(string));
			dataTable.Columns.Add("Query", typeof(string));
			dataTable.Columns.Add("PatchFile", typeof(string));
			dataTable.Columns.Add("Description", typeof(string));
			dataTable.Columns.Add("LayoutDefault", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Id"] = value;
			dataRow["Name"] = value3;
			dataRow["CatId"] = num;
			dataRow["UserSign"] = GlobalData.UserID;
			dataRow["Layout"] = value5;
			dataRow["ValidFor"] = flag;
			dataRow["SqlType"] = Function.ToString(txt_sqlType.EditValue);
			dataRow["Query"] = Function.ToString(txt_query.EditValue);
			dataRow["PatchFile"] = value2;
			dataRow["Description"] = value4;
			dataRow["LayoutDefault"] = Function.ToString(txt_layoutDefault.EditValue);
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			dataTable.TableName = "detail";
			_dtQuery.TableName = "query";
			dataSet.Tables.Add(dataTable.Copy());
			dataSet.Tables.Add(_dtQuery.Copy());
			HttpResult httpResult = await _httpClient.DashboardDetail.UpdateDetailInForm(dataSet);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.UpdateSuccess, "");
				await LoadListCategories();
				btn_ExportLayout.Visible = true;
				FormMode = Enums.FormViewModeType.View;
				return 0;
			}
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			return 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
	}

	private async void Remove(int dashId, int userId)
	{
		_ = 1;
		try
		{
			string text = await _httpClient.CategoriesDABClient.Remove(dashId, userId);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				await LoadListCategories();
				RefeshForm();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Dashboard.frm_DashboardManagement));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.tree_List = new DevExpress.XtraTreeList.TreeList();
		this.tc_Description = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_Id = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_Name = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_ParentId = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_DetailId = new Apzon.Controls.Tree.TreeListColumnEx();
		this.txt_layoutDefault = new Apzon.Controls.Editor.TextEditEx();
		this.txt_query = new Apzon.Controls.Editor.TextEditEx();
		this.txt_sqlType = new Apzon.Controls.Editor.TextEditEx();
		this.txt_layout = new Apzon.Controls.Editor.TextEditEx();
		this.luk_Category = new DevExpress.XtraEditors.LookUpEdit();
		this.txt_Name = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Description = new Apzon.Controls.Editor.TextEditEx();
		this.txt_File = new Apzon.Controls.Editor.TextEditEx();
		this.btn_Update = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ExportLayout = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Browse = new Apzon.Controls.Controls.SimpleButtonEx();
		this.chk_Valid = new System.Windows.Forms.CheckBox();
		this.lab_Name = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.lab_File = new Apzon.Controls.Controls.LabelLine();
		this.tc_ChildId = new Apzon.Controls.Tree.TreeListColumnEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tree_List).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_layoutDefault.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_query.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_sqlType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_layout.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Category.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Description.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_File.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.ContextMenuStrip = this.contextMenuStrip1;
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.tree_List);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.txt_layoutDefault);
		this.splitContainerControl1.Panel2.Controls.Add(this.txt_query);
		this.splitContainerControl1.Panel2.Controls.Add(this.txt_sqlType);
		this.splitContainerControl1.Panel2.Controls.Add(this.txt_layout);
		this.splitContainerControl1.Panel2.Controls.Add(this.luk_Category);
		this.splitContainerControl1.Panel2.Controls.Add(this.txt_Name);
		this.splitContainerControl1.Panel2.Controls.Add(this.txt_Description);
		this.splitContainerControl1.Panel2.Controls.Add(this.txt_File);
		this.splitContainerControl1.Panel2.Controls.Add(this.btn_Update);
		this.splitContainerControl1.Panel2.Controls.Add(this.btn_ExportLayout);
		this.splitContainerControl1.Panel2.Controls.Add(this.btn_Browse);
		this.splitContainerControl1.Panel2.Controls.Add(this.chk_Valid);
		this.splitContainerControl1.Panel2.Controls.Add(this.lab_Name);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine4);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine3);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine2);
		this.splitContainerControl1.Panel2.Controls.Add(this.lab_File);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(865, 424);
		this.splitContainerControl1.SplitterPosition = 426;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(124, 48);
		this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		DevExpress.XtraTreeList.Columns.TreeListColumnCollection columns = this.tree_List.Columns;
		DevExpress.XtraTreeList.Columns.TreeListColumn[] columns2 = new Apzon.Controls.Tree.TreeListColumnEx[6] { this.tc_Description, this.tc_Id, this.tc_Name, this.tc_ParentId, this.tc_DetailId, this.tc_ChildId };
		columns.AddRange(columns2);
		this.tree_List.Cursor = System.Windows.Forms.Cursors.Default;
		this.tree_List.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tree_List.KeyFieldName = "TreeId";
		this.tree_List.Location = new System.Drawing.Point(0, 0);
		this.tree_List.Name = "tree_List";
		this.tree_List.ParentFieldName = "ChildId";
		this.tree_List.Size = new System.Drawing.Size(426, 424);
		this.tree_List.TabIndex = 0;
		this.tree_List.BeforeFocusNode += new DevExpress.XtraTreeList.BeforeFocusNodeEventHandler(this.tree_List_BeforeFocusNode);
		this.tree_List.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tree_List_FocusedNodeChanged);
		this.tc_Description.Caption = "Description";
		this.tc_Description.FieldName = "Description";
		this.tc_Description.Name = "tc_Description";
		this.tc_Description.OptionsColumn.AllowEdit = false;
		this.tc_Description.OptionsColumn.ReadOnly = true;
		this.tc_Description.Visible = true;
		this.tc_Description.VisibleIndex = 1;
		this.tc_Id.Caption = "treeListColumn1";
		this.tc_Id.FieldName = "Id";
		this.tc_Id.Name = "tc_Id";
		this.tc_Name.Caption = "Name";
		this.tc_Name.FieldName = "Name";
		this.tc_Name.Name = "tc_Name";
		this.tc_Name.OptionsColumn.AllowEdit = false;
		this.tc_Name.OptionsColumn.ReadOnly = true;
		this.tc_Name.Visible = true;
		this.tc_Name.VisibleIndex = 0;
		this.tc_ParentId.Caption = "treeListColumn3";
		this.tc_ParentId.FieldName = "ParentId";
		this.tc_ParentId.Name = "tc_ParentId";
		this.tc_DetailId.Caption = "treeListColumn1";
		this.tc_DetailId.FieldName = "DetailId";
		this.tc_DetailId.Name = "tc_DetailId";
		this.txt_layoutDefault.Location = new System.Drawing.Point(82, 151);
		this.txt_layoutDefault.Name = "txt_layoutDefault";
		this.txt_layoutDefault.Size = new System.Drawing.Size(171, 20);
		this.txt_layoutDefault.TabIndex = 21;
		this.txt_layoutDefault.Visible = false;
		this.txt_query.Location = new System.Drawing.Point(82, 226);
		this.txt_query.Name = "txt_query";
		this.txt_query.Size = new System.Drawing.Size(171, 20);
		this.txt_query.TabIndex = 20;
		this.txt_query.Visible = false;
		this.txt_sqlType.Location = new System.Drawing.Point(82, 200);
		this.txt_sqlType.Name = "txt_sqlType";
		this.txt_sqlType.Size = new System.Drawing.Size(171, 20);
		this.txt_sqlType.TabIndex = 19;
		this.txt_sqlType.Visible = false;
		this.txt_layout.Location = new System.Drawing.Point(82, 177);
		this.txt_layout.Name = "txt_layout";
		this.txt_layout.Size = new System.Drawing.Size(171, 20);
		this.txt_layout.TabIndex = 18;
		this.txt_layout.Visible = false;
		this.luk_Category.Location = new System.Drawing.Point(82, 99);
		this.luk_Category.Name = "luk_Category";
		this.luk_Category.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Category.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Id", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default)
		});
		this.luk_Category.Properties.DisplayMember = "Name";
		this.luk_Category.Properties.NullText = "";
		this.luk_Category.Properties.ValueMember = "Id";
		this.luk_Category.Size = new System.Drawing.Size(171, 20);
		this.luk_Category.TabIndex = 17;
		this.txt_Name.Location = new System.Drawing.Point(82, 47);
		this.txt_Name.Name = "txt_Name";
		this.txt_Name.Size = new System.Drawing.Size(171, 20);
		this.txt_Name.TabIndex = 16;
		this.txt_Name.EditValueChanged += new System.EventHandler(this.txt_Name_EditValueChanged);
		this.txt_Description.Location = new System.Drawing.Point(82, 73);
		this.txt_Description.Name = "txt_Description";
		this.txt_Description.Size = new System.Drawing.Size(171, 20);
		this.txt_Description.TabIndex = 15;
		this.txt_File.Location = new System.Drawing.Point(82, 21);
		this.txt_File.Name = "txt_File";
		this.txt_File.Size = new System.Drawing.Size(171, 20);
		this.txt_File.TabIndex = 13;
		this.txt_File.EditValueChanged += new System.EventHandler(this.txt_File_EditValueChanged);
		this.btn_Update.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Update.ImageOptions.Image");
		this.btn_Update.Location = new System.Drawing.Point(22, 252);
		this.btn_Update.Name = "btn_Update";
		this.btn_Update.Size = new System.Drawing.Size(75, 23);
		this.btn_Update.TabIndex = 12;
		this.btn_Update.Text = "Ok";
		this.btn_Update.Click += new System.EventHandler(this.btn_Update_Click);
		this.btn_ExportLayout.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ExportLayout.ImageOptions.Image");
		this.btn_ExportLayout.Location = new System.Drawing.Point(120, 252);
		this.btn_ExportLayout.Name = "btn_ExportLayout";
		this.btn_ExportLayout.Size = new System.Drawing.Size(100, 23);
		this.btn_ExportLayout.TabIndex = 11;
		this.btn_ExportLayout.Text = "Export Layout";
		this.btn_ExportLayout.Visible = false;
		this.btn_ExportLayout.Click += new System.EventHandler(this.btn_ExportLayout_Click);
		this.btn_Browse.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Browse.ImageOptions.Image");
		this.btn_Browse.Location = new System.Drawing.Point(259, 17);
		this.btn_Browse.Name = "btn_Browse";
		this.btn_Browse.Size = new System.Drawing.Size(75, 23);
		this.btn_Browse.TabIndex = 10;
		this.btn_Browse.Text = "Browse";
		this.btn_Browse.Click += new System.EventHandler(this.btn_Browse_Click);
		this.chk_Valid.AutoSize = true;
		this.chk_Valid.Location = new System.Drawing.Point(82, 131);
		this.chk_Valid.Name = "chk_Valid";
		this.chk_Valid.Size = new System.Drawing.Size(15, 14);
		this.chk_Valid.TabIndex = 9;
		this.chk_Valid.UseVisualStyleBackColor = true;
		this.chk_Valid.CheckedChanged += new System.EventHandler(this.chk_Valid_CheckedChanged);
		this.lab_Name.Location = new System.Drawing.Point(14, 47);
		this.lab_Name.Name = "lab_Name";
		this.lab_Name.Size = new System.Drawing.Size(80, 20);
		this.lab_Name.TabIndex = 4;
		this.lab_Name.Utext = "Name";
		this.labelLine4.Location = new System.Drawing.Point(14, 73);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.TabIndex = 3;
		this.labelLine4.Utext = "Description";
		this.labelLine3.Location = new System.Drawing.Point(14, 99);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.TabIndex = 2;
		this.labelLine3.Utext = "Category";
		this.labelLine2.Location = new System.Drawing.Point(17, 125);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.Size = new System.Drawing.Size(66, 20);
		this.labelLine2.TabIndex = 1;
		this.labelLine2.Utext = "Valid";
		this.lab_File.Location = new System.Drawing.Point(17, 21);
		this.lab_File.Name = "lab_File";
		this.lab_File.Size = new System.Drawing.Size(80, 20);
		this.lab_File.TabIndex = 0;
		this.lab_File.Utext = "File";
		this.tc_ChildId.Caption = "treeListColumn1";
		this.tc_ChildId.FieldName = "ChildId";
		this.tc_ChildId.Name = "tc_ChildId";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(865, 424);
		base.Controls.Add(this.splitContainerControl1);
		base.Name = "frm_DashboardManagement";
		base.ShowIcon = false;
		this.Text = "Dashboard Management";
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tree_List).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_layoutDefault.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_query.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_sqlType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_layout.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Category.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Description.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_File.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
