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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Tree;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace Apzon.Forms.Administrator;

public class frm_Authorization : XtraFormEx
{
	private string _user = "";

	private string _formController = "Auth";

	private DataTable _dtAuth;

	public RefreshMain RefreshMain;

	private TreeListViewState treeListViewState;

	public bool isAuthorizationDMS;

	private DataTable _dtLooupEditAuth;

	private static frm_Authorization _instance;

	private static readonly object PadLock = new object();

	private bool _loadingAuth;

	private IContainer components;

	private GridControlEx grdUlist;

	private GridColumnEx gcU_NAME;

	private GridColumnEx gcUSER_CODE;

	private PanelControl panelControl1;

	private SplitContainerControl splitContainerControl1;

	private SimpleButtonEx btUpdate;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx bt_FullAuth;

	private SimpleButtonEx bt_NoAuth;

	private SimpleButtonEx bt_ReadOnly;

	private SimpleButtonEx bt_CopyAuth;

	private TreeEx trl_Doc;

	private GridViewEx gvUList;

	private TreeListColumnEx tc_ID;

	private TreeListColumnEx tc_FMenuName;

	private TreeListColumnEx tc_MenuName;

	private TreeListColumnEx tc_authoriza;

	private TreeListColumnEx USER_CODE;

	private RepositoryItemLookUpEdit rep_tl_authoriza;

	private ContextMenuStrip ctm_Author;

	private ToolStripMenuItem setAsDefaultToolStripMenuItem;

	private TreeListColumnEx tc_isDefault;

	private ToolStripMenuItem removeAsDefaultToolStripMenuItem;

	public static frm_Authorization Instance
	{
		get
		{
			if (_instance == null || _instance.IsDisposed)
			{
				lock (PadLock)
				{
					if (_instance == null || _instance.IsDisposed)
					{
						_instance = new frm_Authorization();
					}
				}
			}
			return _instance;
		}
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btUpdate.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btUpdate.Text = Resources.ButtonOK;
			break;
		}
	}

	public static bool IsOpening()
	{
		if (_instance != null)
		{
			return !_instance.IsDisposed;
		}
		return false;
	}

	public frm_Authorization()
	{
		if (GlobalData.UserLicense == null || GlobalData.UserLicense.Rule == LicenseRule.Service)
		{
			throw new Exception("System does not have a license, or License does not have permission to access any form");
		}
		GlobalData.UserName.ToLower().Equals(GlobalData.SuperUser);
		InitializeComponent();
		OnLoadAsync += frm_Authorization_Load;
	}

	private async Task frm_Authorization_Load(object sender, EventArgs e)
	{
		bt_ReadOnly.Enabled = !isAuthorizationDMS;
		LoadRepLkUpEditDatasource();
		await LoadData();
		SettingLanguage();
	}

	private void bt_CopyAuth_Click(object sender, EventArgs e)
	{
		try
		{
			frm_AuthorizationCopy frm_AuthorizationCopy2 = new frm_AuthorizationCopy(_isFromAuthForm: true, isAuthorizationDMS);
			frm_AuthorizationCopy2._user = _user;
			frm_AuthorizationCopy2.ShowDialog(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bt_FullAuth_Click(object sender, EventArgs e)
	{
		try
		{
			for (int i = 0; i < _dtAuth.Rows.Count; i++)
			{
				_dtAuth.Rows[i]["authoriza"] = "F";
			}
			trl_Doc.DataSource = _dtAuth;
			trl_Doc.RefreshDataSource();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bt_NoAuth_Click(object sender, EventArgs e)
	{
		try
		{
			for (int i = 0; i < _dtAuth.Rows.Count; i++)
			{
				_dtAuth.Rows[i]["authoriza"] = "N";
			}
			trl_Doc.DataSource = _dtAuth;
			trl_Doc.RefreshDataSource();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bt_ReadOnly_Click(object sender, EventArgs e)
	{
		try
		{
			for (int i = 0; i < _dtAuth.Rows.Count; i++)
			{
				_dtAuth.Rows[i]["authoriza"] = "R";
			}
			trl_Doc.DataSource = _dtAuth;
			trl_Doc.RefreshDataSource();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void rep_Auth_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			string text = Function.ToString(((BaseEdit)sender).EditValue);
			trl_Doc.FocusedNode.SetValue("authoriza", text);
			SetValueChildNodes(trl_Doc.FocusedNode, text);
			setValueAuthoriza(trl_Doc.FocusedNode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task setValueAuthoriza(TreeListNode node)
	{
		if (node.ParentNode != null)
		{
			TreeListNode parentNode = node.ParentNode;
			int id = Function.ParseInt(parentNode.GetValue("Id"));
			DataTable dataSourceTable = trl_Doc.DataSourceTable;
			DataRow dataRow = dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["FMenuName"]) == id && Function.ToString(t["authoriza"]) == "F");
			DataRow dataRow2 = dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["FMenuName"]) == id && Function.ToString(t["authoriza"]) == "R");
			DataRow dataRow3 = dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["FMenuName"]) == id && Function.ToString(t["authoriza"]) == "N");
			DataRow dataRow4 = dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["FMenuName"]) == id && Function.ToString(t["authoriza"]) == "H");
			if ((dataRow != null && dataRow3 != null) || (dataRow != null && dataRow2 != null) || (dataRow2 != null && dataRow3 != null) || dataRow4 != null)
			{
				parentNode.SetValue("authoriza", "H");
			}
			else if (dataRow != null && dataRow3 == null && dataRow4 == null)
			{
				parentNode.SetValue("authoriza", "F");
			}
			else if (dataRow == null && dataRow4 == null && dataRow3 != null)
			{
				parentNode.SetValue("authoriza", "N");
			}
			dataSourceTable.AcceptChanges();
			if (parentNode.ParentNode != null)
			{
				setValueAuthoriza(parentNode);
			}
		}
	}

	private void SetValueChildNodes(TreeListNode node, string value)
	{
		try
		{
			foreach (TreeListNode node2 in node.Nodes)
			{
				node2.SetValue("authoriza", value);
				SetValueChildNodes(node2, value);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public DataTable LoadAuthoriza(DataTable dt)
	{
		try
		{
			dt = (from r in dt.AsEnumerable()
				orderby Function.ParseInt(r["FMenuName"]) descending
				select r).CopyToDataTable();
			for (int num = 0; num < dt.Rows.Count; num++)
			{
				int id = Function.ParseInt(dt.Rows[num]["Id"]);
				if (!dt.AsEnumerable().Any((DataRow t) => Function.ParseInt(t["FMenuName"]) == id))
				{
					continue;
				}
				if ((from t in dt.AsEnumerable()
					where Function.ParseInt(t["FMenuName"]) == id
					select Function.ToString(t["authoriza"])).Distinct().Count() == 1)
				{
					DataRow dataRow = dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["FMenuName"]) == id);
					dt.Rows[num]["authoriza"] = Function.ToString(dataRow["authoriza"]);
				}
				else
				{
					dt.Rows[num]["authoriza"] = "H";
				}
			}
			dt.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return dt;
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void gvUList_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			_user = ((gvUList.GetFocusedRowCellValue("USER_CODE") == DBNull.Value || gvUList.GetFocusedRowCellValue("USER_CODE") == null) ? "" : gvUList.GetFocusedRowCellValue("USER_CODE").ToString());
			if (_user != "")
			{
				if (FormMode == Enums.FormViewModeType.Update)
				{
					FormMode = Enums.FormViewModeType.View;
				}
				await LoadAuthorities(_user);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void trl_Doc_FilterNode(object sender, FilterNodeEventArgs e)
	{
		try
		{
			if (trl_Doc.ActiveFilterCriteria is BinaryOperator { LeftOperand: OperandProperty leftOperand } && !(leftOperand.PropertyName != tc_MenuName.FieldName) && e.Node.Level != 0 && e.Node.ParentNode.Visible)
			{
				e.Node.Visible = e.Node.ParentNode.Visible;
				e.Handled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetEnableButtons(bool enable)
	{
		btUpdate.Enabled = enable;
		bt_CopyAuth.Enabled = enable;
		bt_FullAuth.Enabled = enable;
		bt_NoAuth.Enabled = enable;
		bt_ReadOnly.Enabled = enable;
	}

	private async void btUpdate_Click(object sender, EventArgs e)
	{
		try
		{
			SetEnableButtons(enable: false);
			if (FormMode == Enums.FormViewModeType.Update)
			{
				WinCommons._OpenProcessing();
				await SaveData();
			}
			else
			{
				Close();
			}
			SetEnableButtons(enable: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private async Task SaveData()
	{
		_ = 2;
		try
		{
			grdUlist.Enabled = false;
			trl_Doc.OptionsBehavior.Editable = false;
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, _formController);
			DataTable dataSourceTable = trl_Doc.DataSourceTable;
			if (!dataSourceTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
				return;
			}
			DataTable tableStructure = WinFuntions.GetTableStructure("OPRM");
			for (int i = 0; i < dataSourceTable.Rows.Count; i++)
			{
				string text = Function.ToString(dataSourceTable.Rows[i]["MenuType"]);
				if (!(text == "N") && !(text == "A"))
				{
					string value = Function.ToString(dataSourceTable.Rows[i]["authoriza"]);
					string value2 = Function.ToString(dataSourceTable.Rows[i]["menucode"]);
					string value3 = Function.ToString(dataSourceTable.Rows[i]["isDefault"]);
					DataRow dataRow = tableStructure.NewRow();
					dataRow["USER_CODE"] = Function.ToString(dataSourceTable.Rows[i]["USER_CODE"]);
					dataRow["menucode"] = value2;
					dataRow["authoriza"] = value;
					dataRow["isDefault"] = value3;
					tableStructure.Rows.Add(dataRow);
				}
			}
			tableStructure.AcceptChanges();
			string text2 = ((!isAuthorizationDMS) ? (await hp.PostAsync("SaveDocs", tableStructure)) : (await hp.PostAsync("SaveDMSAuthorization", tableStructure)));
			if (string.IsNullOrEmpty(text2))
			{
				WinCommons.Success(Enums.MessageCode.OperationCompletedSuccess, "");
				_user = ((gvUList.GetFocusedRowCellValue("USER_CODE") == DBNull.Value || gvUList.GetFocusedRowCellValue("USER_CODE") == null) ? "" : gvUList.GetFocusedRowCellValue("USER_CODE").ToString());
				if (_user != "")
				{
					await LoadAuthorities(_user);
				}
				if (RefreshMain != null && _user == GlobalData.UserCodeLogin)
				{
					RefreshMain();
				}
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			grdUlist.Enabled = true;
			trl_Doc.OptionsBehavior.Editable = true;
		}
	}

	private void LoadRepLkUpEditDatasource()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow[0] = "F";
			dataRow[1] = "Full Authorization";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2[0] = "N";
			dataRow2[1] = "";
			dataTable.Rows.Add(dataRow2);
			if (!isAuthorizationDMS)
			{
				DataRow dataRow3 = dataTable.NewRow();
				dataRow3[0] = "R";
				dataRow3[1] = "Read-Only";
				dataTable.Rows.Add(dataRow3);
			}
			dataTable.AcceptChanges();
			rep_tl_authoriza.DataSource = dataTable;
			treeListViewState = new TreeListViewState(trl_Doc);
			_dtLooupEditAuth = new DataTable();
			_dtLooupEditAuth.Columns.Add("Code", typeof(string));
			_dtLooupEditAuth.Columns.Add("Name", typeof(string));
			DataRow dataRow4 = _dtLooupEditAuth.NewRow();
			dataRow4[0] = "F";
			dataRow4[1] = "Full Authorization";
			_dtLooupEditAuth.Rows.Add(dataRow4);
			dataRow4 = _dtLooupEditAuth.NewRow();
			dataRow4[0] = "N";
			dataRow4[1] = "";
			_dtLooupEditAuth.Rows.Add(dataRow4);
			_dtLooupEditAuth.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SettingLanguage()
	{
		try
		{
			if (isAuthorizationDMS)
			{
				Text = "Phân quyền DMS";
			}
			btUpdate.Text = Resources.ButtonOK;
			btCancel.Text = Resources.ButtonCancel;
			bt_CopyAuth.Text = Resources.OPRM_CopyAuth;
			bt_FullAuth.Text = Resources.OPRM_FullAuth;
			bt_NoAuth.Text = Resources.OPRM_NoAuth;
			bt_ReadOnly.Text = Resources.OPRM_ReadOnly;
			gvUList.Columns["U_NAME"].Caption = Resources.OUSR_U_NAME;
			gvUList.Columns["USER_CODE"].Caption = Resources.OUSR_USER_CODE;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadData()
	{
		await LoadUsers();
		await LoadAuthorities(_user);
		FormMode = Enums.FormViewModeType.View;
	}

	private async Task LoadUsers()
	{
		gvUList.ShowLoadingPanel();
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Users");
			DataTable dataSource = await hp.GetAsync("ShowUsers");
			grdUlist.DataSource = dataSource;
			grdUlist.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			gvUList.HideLoadingPanel();
		}
	}

	private async Task LoadAuthorities(string user)
	{
		SetEnableButtons(enable: false);
		try
		{
			_loadingAuth = true;
			trl_Doc.ShowLoadingPanel();
			treeListViewState.SaveState();
			trl_Doc.DataSource = null;
			using (HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController))
			{
				if (isAuthorizationDMS)
				{
					_dtAuth = await hp.GetAsync("DMSGetAllAuth", new { user });
				}
				else
				{
					_dtAuth = await hp.GetAsync("GetAllAuth", new
					{
						user = user,
						lngCode = GlobalData.CURRENTLANGUAGE
					});
				}
				if (_dtAuth.IsNotNull())
				{
					List<DataRow> source = (from t in _dtAuth.AsEnumerable()
						where !CommonList.HiddenFunction.ContainsKey(Function.ToString(t["menucode"])) || (CommonList.HiddenFunction.ContainsKey(Function.ToString(t["menucode"])) && !CommonList.HiddenFunction[Function.ToString(t["menucode"])])
						select t).ToList();
					if (GlobalData.UserLicense.Rule != LicenseRule.Super)
					{
						string[] formTypeCheckLicense = new string[2] { "F", "L" };
						source = source.Where((DataRow t) => GlobalData.UserLicense.UserRule != LicenseRule.Service && (!formTypeCheckLicense.Contains(Function.ToString(t["MenuType"])) || (GlobalData.UserLicense != null && (GlobalData.UserLicense.UserRule == LicenseRule.Super || GlobalData.UserLicense.LstAccessFunctionOfUser.Contains(Function.ToString(t["menucode"])))))).ToList();
					}
					if (source.Any())
					{
						_dtAuth = ReloadFatherMenu(source.CopyToDataTable());
					}
					else
					{
						_dtAuth.Rows.Clear();
					}
				}
				trl_Doc.DataSource = LoadAuthoriza(_dtAuth);
				trl_Doc.RefreshDataSource();
				treeListViewState.LoadState();
			}
			SetEnableButtons(enable: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		trl_Doc.HideLoadingPanel();
		_loadingAuth = false;
	}

	private DataTable ReloadFatherMenu(DataTable dt)
	{
		if (dt.IsNotNull())
		{
			List<DataRow> list = (from t in dt.AsEnumerable()
				where Function.ToString(t["MenuType"]).Equals("N")
				select t).ToList();
			List<DataRow> list2 = list.Where((DataRow t) => dt.AsEnumerable().All((DataRow x) => Function.ParseInt(x["FMenuName"]) != Function.ParseInt(t["Id"]))).ToList();
			while (list2.Any())
			{
				for (int num = 0; num < list2.Count; num++)
				{
					list.Remove(list2[num]);
					dt.Rows.Remove(list2[num]);
				}
				list2 = list.Where((DataRow t) => dt.AsEnumerable().All((DataRow x) => Function.ParseInt(x["FMenuName"]) != Function.ParseInt(t["Id"]))).ToList();
			}
			dt.AcceptChanges();
		}
		return dt;
	}

	private async void setAsDefaultToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			DataTable dataSourceTable = trl_Doc.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				for (int i = 0; i < dataSourceTable.Rows.Count; i++)
				{
					dataSourceTable.Rows[i]["isDefault"] = "N";
				}
			}
			trl_Doc.SetFocusedRowCellValue("isDefault", "Y");
			await SaveData();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetStyleFather(DataTable dt, ref List<int> lst, int fatherId = -1)
	{
		try
		{
			DataRow dataRow = dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["Id"]) == fatherId);
			if (dataRow != null)
			{
				lst.Add(Function.ParseInt(dataRow["Id"]));
				SetStyleFather(dt, ref lst, Function.ParseInt(dataRow["FMenuName"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void trl_Doc_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		try
		{
			if (trl_Doc.DataSource != null)
			{
				DataTable dataSourceTable = trl_Doc.DataSourceTable;
				string text = Function.ToString(trl_Doc.GetRowCellValue(e.Node, tc_isDefault));
				int item = Function.ParseInt(trl_Doc.GetRowCellValue(e.Node, tc_ID));
				DataRow dataRow = dataSourceTable.Select($"isDefault='Y'").FirstOrDefault();
				int fatherId = 0;
				if (dataRow != null)
				{
					fatherId = Function.ParseInt(dataRow["FMenuName"]);
				}
				if (text.Equals("Y"))
				{
					e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);
				}
				List<int> lst = new List<int>();
				if (dataSourceTable.IsNotNull())
				{
					SetStyleFather(dataSourceTable, ref lst, fatherId);
				}
				if (lst.Contains(item))
				{
					e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void trl_Doc_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		try
		{
			DataTable dataSourceTable = trl_Doc.DataSourceTable;
			int fatherId = Function.ParseInt(trl_Doc.GetRowCellValue(e.Node, tc_ID));
			if (dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["FMenuName"]) == fatherId) != null)
			{
				setAsDefaultToolStripMenuItem.Enabled = false;
				removeAsDefaultToolStripMenuItem.Enabled = false;
			}
			else
			{
				setAsDefaultToolStripMenuItem.Enabled = true;
				removeAsDefaultToolStripMenuItem.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void removeAsDefaultToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			trl_Doc.SetFocusedRowCellValue("isDefault", "N");
			await SaveData();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void trl_Doc_CustomColumnDisplayText(object sender, DevExpress.XtraTreeList.CustomColumnDisplayTextEventArgs e)
	{
		if (e.Node != null)
		{
			string text = Function.ToString(e.Node.GetValue("authoriza"));
			if (e.Column.FieldName == "authoriza" && text == "H")
			{
				e.DisplayText = "Various Authorization";
			}
		}
	}

	private void trl_Doc_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
	{
		try
		{
			string text = Function.ToString(e.Node.GetValue("menucode"));
			if (e.Column.FieldName.Equals("authoriza") && text.Equals("frm_ChangeLogList"))
			{
				RepositoryItemLookUpEdit repositoryItemLookUpEdit = rep_tl_authoriza.Clone() as RepositoryItemLookUpEdit;
				repositoryItemLookUpEdit.DataSource = _dtLooupEditAuth;
				e.RepositoryItem = repositoryItemLookUpEdit;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void gvUList_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		if (_loadingAuth)
		{
			e.Allow = false;
		}
		else if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
		{
			e.Allow = false;
		}
	}

	private void gvUList_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		if (e.RowHandle == gvUList.FocusedRowHandle)
		{
			e.Appearance.BackColor = StyleColor.GridEditting;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_Authorization));
		this.grdUlist = new Apzon.Controls.Grid.GridControlEx();
		this.gvUList = new Apzon.Controls.Grid.GridViewEx();
		this.gcU_NAME = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUSER_CODE = new Apzon.Controls.Grid.GridColumnEx();
		this.btUpdate = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_FullAuth = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_NoAuth = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_ReadOnly = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_CopyAuth = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.trl_Doc = new Apzon.Controls.Tree.TreeEx();
		this.tc_ID = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_FMenuName = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_MenuName = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_authoriza = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_tl_authoriza = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.USER_CODE = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_isDefault = new Apzon.Controls.Tree.TreeListColumnEx();
		this.ctm_Author = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.setAsDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeAsDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)this.grdUlist).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvUList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.trl_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_tl_authoriza).BeginInit();
		this.ctm_Author.SuspendLayout();
		base.SuspendLayout();
		this.grdUlist.AllowPasteExcel = false;
		this.grdUlist.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdUlist.Location = new System.Drawing.Point(0, 0);
		this.grdUlist.MainView = this.gvUList;
		this.grdUlist.Name = "grdUlist";
		this.grdUlist.Size = new System.Drawing.Size(322, 475);
		this.grdUlist.TabIndex = 0;
		this.grdUlist.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvUList });
		this.gvUList.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvUList.ColumnKey = null;
		this.gvUList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gcU_NAME, this.gcUSER_CODE });
		this.gvUList.DisableFormSetting = false;
		this.gvUList.DisplayException = false;
		this.gvUList.FieldCurrency = null;
		this.gvUList.FieldRate = null;
		this.gvUList.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
		this.gvUList.FormId = "oAuthorization";
		this.gvUList.GridControl = this.grdUlist;
		this.gvUList.IndicatorWidth = 35;
		this.gvUList.IsApzTable = false;
		this.gvUList.IsInsertNewRow = false;
		this.gvUList.IsLoadingData = false;
		this.gvUList.ItemId = "OUSR";
		this.gvUList.LstColumnAllowBatch = null;
		this.gvUList.LstColumnNotAllowBatch = null;
		this.gvUList.LstColumnNotEditable = null;
		this.gvUList.LstColumNotSetting = null;
		this.gvUList.Name = "gvUList";
		this.gvUList.OptionsBehavior.Editable = false;
		this.gvUList.OptionsBehavior.ReadOnly = true;
		this.gvUList.OptionsView.ShowGroupPanel = false;
		this.gvUList.UseBatchMod = false;
		this.gvUList.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvUList_RowCellStyle);
		this.gvUList.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvUList_FocusedRowObjectChanged);
		this.gvUList.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gvUList_BeforeLeaveRow);
		this.gcU_NAME.Caption = "Full name";
		this.gcU_NAME.CurrencyDateField = null;
		this.gcU_NAME.CurrencyField = null;
		this.gcU_NAME.CurrencyFixed = null;
		this.gcU_NAME.CurrencyFixedRate = 0.0;
		this.gcU_NAME.CurrencyRateField = null;
		this.gcU_NAME.FieldName = "U_NAME";
		this.gcU_NAME.Name = "gcU_NAME";
		this.gcU_NAME.OptionsColumn.AllowEdit = false;
		this.gcU_NAME.OptionsColumn.ReadOnly = true;
		this.gcU_NAME.PrimaryKey = false;
		this.gcU_NAME.UnitField = null;
		this.gcU_NAME.Visible = true;
		this.gcU_NAME.VisibleIndex = 0;
		this.gcUSER_CODE.Caption = "User name";
		this.gcUSER_CODE.CurrencyDateField = null;
		this.gcUSER_CODE.CurrencyField = null;
		this.gcUSER_CODE.CurrencyFixed = null;
		this.gcUSER_CODE.CurrencyFixedRate = 0.0;
		this.gcUSER_CODE.CurrencyRateField = null;
		this.gcUSER_CODE.FieldName = "USER_CODE";
		this.gcUSER_CODE.Name = "gcUSER_CODE";
		this.gcUSER_CODE.OptionsColumn.AllowEdit = false;
		this.gcUSER_CODE.OptionsColumn.ReadOnly = true;
		this.gcUSER_CODE.PrimaryKey = false;
		this.gcUSER_CODE.UnitField = null;
		this.gcUSER_CODE.Visible = true;
		this.gcUSER_CODE.VisibleIndex = 1;
		this.btUpdate.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btUpdate.EditControl = null;
		this.btUpdate.EditValue = "OK";
		this.btUpdate.FieldName = null;
		this.btUpdate.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btUpdate.ImageOptions.Image");
		this.btUpdate.IsSearchControl = false;
		this.btUpdate.Location = new System.Drawing.Point(332, 8);
		this.btUpdate.Name = "btUpdate";
		this.btUpdate.ReadOnly = false;
		this.btUpdate.SettingInVisible = false;
		this.btUpdate.SettingReadonly = false;
		this.btUpdate.Size = new System.Drawing.Size(75, 23);
		this.btUpdate.TabIndex = 2;
		this.btUpdate.Text = "OK";
		this.btUpdate.Click += new System.EventHandler(this.btUpdate_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(409, 8);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 3;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.bt_FullAuth.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.bt_FullAuth.EditControl = null;
		this.bt_FullAuth.EditValue = "Full Authorization";
		this.bt_FullAuth.FieldName = null;
		this.bt_FullAuth.IsSearchControl = false;
		this.bt_FullAuth.Location = new System.Drawing.Point(527, 8);
		this.bt_FullAuth.Name = "bt_FullAuth";
		this.bt_FullAuth.ReadOnly = false;
		this.bt_FullAuth.SettingInVisible = false;
		this.bt_FullAuth.SettingReadonly = false;
		this.bt_FullAuth.Size = new System.Drawing.Size(92, 23);
		this.bt_FullAuth.TabIndex = 5;
		this.bt_FullAuth.Text = "Full Authorization";
		this.bt_FullAuth.Click += new System.EventHandler(this.bt_FullAuth_Click);
		this.bt_NoAuth.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.bt_NoAuth.EditControl = null;
		this.bt_NoAuth.EditValue = "No Authorization";
		this.bt_NoAuth.FieldName = null;
		this.bt_NoAuth.IsSearchControl = false;
		this.bt_NoAuth.Location = new System.Drawing.Point(622, 8);
		this.bt_NoAuth.Name = "bt_NoAuth";
		this.bt_NoAuth.ReadOnly = false;
		this.bt_NoAuth.SettingInVisible = false;
		this.bt_NoAuth.SettingReadonly = false;
		this.bt_NoAuth.Size = new System.Drawing.Size(90, 23);
		this.bt_NoAuth.TabIndex = 6;
		this.bt_NoAuth.Text = "No Authorization";
		this.bt_NoAuth.Click += new System.EventHandler(this.bt_NoAuth_Click);
		this.bt_ReadOnly.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.bt_ReadOnly.EditControl = null;
		this.bt_ReadOnly.EditValue = "Read-Only";
		this.bt_ReadOnly.FieldName = null;
		this.bt_ReadOnly.IsSearchControl = false;
		this.bt_ReadOnly.Location = new System.Drawing.Point(715, 8);
		this.bt_ReadOnly.Name = "bt_ReadOnly";
		this.bt_ReadOnly.ReadOnly = false;
		this.bt_ReadOnly.SettingInVisible = false;
		this.bt_ReadOnly.SettingReadonly = false;
		this.bt_ReadOnly.Size = new System.Drawing.Size(75, 23);
		this.bt_ReadOnly.TabIndex = 7;
		this.bt_ReadOnly.Text = "Read-Only";
		this.bt_ReadOnly.Click += new System.EventHandler(this.bt_ReadOnly_Click);
		this.bt_CopyAuth.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_CopyAuth.EditControl = null;
		this.bt_CopyAuth.EditValue = "Copy Author&izations";
		this.bt_CopyAuth.FieldName = null;
		this.bt_CopyAuth.IsSearchControl = false;
		this.bt_CopyAuth.Location = new System.Drawing.Point(4, 8);
		this.bt_CopyAuth.Name = "bt_CopyAuth";
		this.bt_CopyAuth.ReadOnly = false;
		this.bt_CopyAuth.SettingInVisible = false;
		this.bt_CopyAuth.SettingReadonly = false;
		this.bt_CopyAuth.Size = new System.Drawing.Size(115, 23);
		this.bt_CopyAuth.TabIndex = 8;
		this.bt_CopyAuth.Text = "Copy Author&izations";
		this.bt_CopyAuth.Click += new System.EventHandler(this.bt_CopyAuth_Click);
		this.panelControl1.Controls.Add(this.btUpdate);
		this.panelControl1.Controls.Add(this.bt_CopyAuth);
		this.panelControl1.Controls.Add(this.btCancel);
		this.panelControl1.Controls.Add(this.bt_ReadOnly);
		this.panelControl1.Controls.Add(this.bt_FullAuth);
		this.panelControl1.Controls.Add(this.bt_NoAuth);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl1.Location = new System.Drawing.Point(0, 475);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(794, 37);
		this.panelControl1.TabIndex = 9;
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grdUlist);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.trl_Doc);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(794, 475);
		this.splitContainerControl1.SplitterPosition = 322;
		this.splitContainerControl1.TabIndex = 10;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.trl_Doc.AllowPasteExcel = false;
		this.trl_Doc.Appearance.HotTrackedRow.BackColor = System.Drawing.Color.PaleGoldenrod;
		this.trl_Doc.Appearance.HotTrackedRow.Options.UseBackColor = true;
		this.trl_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.trl_Doc.ColumnKey = null;
		this.trl_Doc.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[6] { this.tc_ID, this.tc_FMenuName, this.tc_MenuName, this.tc_authoriza, this.USER_CODE, this.tc_isDefault });
		this.trl_Doc.ContextMenuStrip = this.ctm_Author;
		this.trl_Doc.Cursor = System.Windows.Forms.Cursors.Default;
		this.trl_Doc.DisableFormSetting = true;
		this.trl_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.trl_Doc.EditColumn = null;
		this.trl_Doc.FieldCurrency = null;
		this.trl_Doc.FieldRate = null;
		this.trl_Doc.FormId = "oAuthorization";
		this.trl_Doc.IndexEditRow = -1;
		this.trl_Doc.IndicatorWidth = 40;
		this.trl_Doc.IsApzTable = false;
		this.trl_Doc.IsLoadingData = false;
		this.trl_Doc.ItemId = "Authorization";
		this.trl_Doc.KeyFieldName = "Id";
		this.trl_Doc.Location = new System.Drawing.Point(0, 0);
		this.trl_Doc.LstColumnNotEditable = null;
		this.trl_Doc.LstColumNotSetting = null;
		this.trl_Doc.Name = "trl_Doc";
		this.trl_Doc.OptionsFilter.ExpandNodesOnFiltering = true;
		this.trl_Doc.OptionsFilter.FilterMode = DevExpress.XtraTreeList.FilterMode.EntireBranch;
		this.trl_Doc.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.trl_Doc.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.trl_Doc.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True;
		this.trl_Doc.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.RowFocus;
		this.trl_Doc.ParentFieldName = "FMenuName";
		this.trl_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_tl_authoriza });
		this.trl_Doc.Size = new System.Drawing.Size(462, 475);
		this.trl_Doc.TabIndex = 0;
		this.trl_Doc.FilterNode += new DevExpress.XtraTreeList.FilterNodeEventHandler(this.trl_Doc_FilterNode);
		this.trl_Doc.CustomNodeCellEdit += new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(this.trl_Doc_CustomNodeCellEdit);
		this.trl_Doc.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.trl_Doc_NodeCellStyle);
		this.trl_Doc.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.trl_Doc_FocusedNodeChanged);
		this.trl_Doc.CustomColumnDisplayText += new DevExpress.XtraTreeList.CustomColumnDisplayTextEventHandler(this.trl_Doc_CustomColumnDisplayText);
		this.tc_ID.Caption = "ID";
		this.tc_ID.CurrencyDateField = null;
		this.tc_ID.CurrencyField = null;
		this.tc_ID.CurrencyFixed = null;
		this.tc_ID.CurrencyFixedRate = 0.0;
		this.tc_ID.CurrencyRateField = null;
		this.tc_ID.FieldName = "Id";
		this.tc_ID.Name = "tc_ID";
		this.tc_ID.OptionsColumn.AllowEdit = false;
		this.tc_ID.OptionsColumn.ReadOnly = true;
		this.tc_FMenuName.Caption = "FMenuName";
		this.tc_FMenuName.CurrencyDateField = null;
		this.tc_FMenuName.CurrencyField = null;
		this.tc_FMenuName.CurrencyFixed = null;
		this.tc_FMenuName.CurrencyFixedRate = 0.0;
		this.tc_FMenuName.CurrencyRateField = null;
		this.tc_FMenuName.FieldName = "FMenuName";
		this.tc_FMenuName.Name = "tc_FMenuName";
		this.tc_FMenuName.OptionsColumn.AllowEdit = false;
		this.tc_FMenuName.OptionsColumn.ReadOnly = true;
		this.tc_MenuName.Caption = "Menu Name";
		this.tc_MenuName.CurrencyDateField = null;
		this.tc_MenuName.CurrencyField = null;
		this.tc_MenuName.CurrencyFixed = null;
		this.tc_MenuName.CurrencyFixedRate = 0.0;
		this.tc_MenuName.CurrencyRateField = null;
		this.tc_MenuName.FieldName = "MenuName";
		this.tc_MenuName.Name = "tc_MenuName";
		this.tc_MenuName.OptionsColumn.AllowEdit = false;
		this.tc_MenuName.OptionsColumn.ReadOnly = true;
		this.tc_MenuName.Visible = true;
		this.tc_MenuName.VisibleIndex = 0;
		this.tc_authoriza.Caption = "Authorization";
		this.tc_authoriza.ColumnEdit = this.rep_tl_authoriza;
		this.tc_authoriza.CurrencyDateField = null;
		this.tc_authoriza.CurrencyField = null;
		this.tc_authoriza.CurrencyFixed = null;
		this.tc_authoriza.CurrencyFixedRate = 0.0;
		this.tc_authoriza.CurrencyRateField = null;
		this.tc_authoriza.FieldName = "authoriza";
		this.tc_authoriza.Name = "tc_authoriza";
		this.tc_authoriza.Visible = true;
		this.tc_authoriza.VisibleIndex = 1;
		this.rep_tl_authoriza.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_tl_authoriza.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_tl_authoriza.DisplayMember = "Name";
		this.rep_tl_authoriza.DropDownRows = 3;
		this.rep_tl_authoriza.Name = "rep_tl_authoriza";
		this.rep_tl_authoriza.NullText = "";
		this.rep_tl_authoriza.PopupBorderStyle = DevExpress.XtraEditors.Controls.PopupBorderStyles.Flat;
		this.rep_tl_authoriza.ShowFooter = false;
		this.rep_tl_authoriza.ShowHeader = false;
		this.rep_tl_authoriza.ValueMember = "Code";
		this.rep_tl_authoriza.EditValueChanged += new System.EventHandler(this.rep_Auth_EditValueChanged);
		this.USER_CODE.Caption = "USER_CODE";
		this.USER_CODE.CurrencyDateField = null;
		this.USER_CODE.CurrencyField = null;
		this.USER_CODE.CurrencyFixed = null;
		this.USER_CODE.CurrencyFixedRate = 0.0;
		this.USER_CODE.CurrencyRateField = null;
		this.USER_CODE.FieldName = "USER_CODE";
		this.USER_CODE.Name = "USER_CODE";
		this.USER_CODE.OptionsColumn.AllowEdit = false;
		this.USER_CODE.OptionsColumn.ReadOnly = true;
		this.tc_isDefault.Caption = "isDefault";
		this.tc_isDefault.CurrencyDateField = null;
		this.tc_isDefault.CurrencyField = null;
		this.tc_isDefault.CurrencyFixed = null;
		this.tc_isDefault.CurrencyFixedRate = 0.0;
		this.tc_isDefault.CurrencyRateField = null;
		this.tc_isDefault.FieldName = "isDefault";
		this.tc_isDefault.Name = "tc_isDefault";
		this.tc_isDefault.OptionsColumn.AllowEdit = false;
		this.tc_isDefault.OptionsColumn.ReadOnly = true;
		this.ctm_Author.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.ctm_Author.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.setAsDefaultToolStripMenuItem, this.removeAsDefaultToolStripMenuItem });
		this.ctm_Author.Name = "ctm_Author";
		this.ctm_Author.Size = new System.Drawing.Size(172, 48);
		this.setAsDefaultToolStripMenuItem.Name = "setAsDefaultToolStripMenuItem";
		this.setAsDefaultToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
		this.setAsDefaultToolStripMenuItem.Text = "Set as default";
		this.setAsDefaultToolStripMenuItem.Click += new System.EventHandler(this.setAsDefaultToolStripMenuItem_Click);
		this.removeAsDefaultToolStripMenuItem.Name = "removeAsDefaultToolStripMenuItem";
		this.removeAsDefaultToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
		this.removeAsDefaultToolStripMenuItem.Text = "Remove as default";
		this.removeAsDefaultToolStripMenuItem.Click += new System.EventHandler(this.removeAsDefaultToolStripMenuItem_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(794, 512);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.panelControl1);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oAuthorization";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_Authorization.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.MinimizeBox = false;
		base.Name = "frm_Authorization";
		base.ShowInTaskbar = false;
		this.Text = "General Authorization";
		((System.ComponentModel.ISupportInitialize)this.grdUlist).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvUList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.trl_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_tl_authoriza).EndInit();
		this.ctm_Author.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
