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
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace Apzon.Forms.DMS;

public class frm_Geography : XtraFormEx
{
	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isDuplicate;

	private IContainer components;

	private ContextMenuStrip cms_CountryMenu;

	private ToolStripMenuItem menu_AddNewDoc;

	private ToolStripMenuItem menu_RemoveDoc;

	private ContextMenuStrip cms_CountryDetailsMenu;

	private ToolStripMenuItem menu_AddDetails;

	private ToolStripMenuItem menu_RemoveDetails;

	private ToolStripMenuItem menu_AddSame;

	private ToolStripMenuItem menu_Insert;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Country;

	private GridViewEx gv_Country;

	private GridColumnEx gc_CountryCode;

	private GridColumnEx gc_CountryName;

	private GridColumnEx gc_Region;

	private GridColumnEx gc_Status;

	private RepositoryItemLookUpEdit glk_Status;

	private GridColumnEx gc_Remarks;

	private GridColumnEx gc_UserSign;

	private GridColumnEx gc_CreateDate;

	private GridColumnEx gc_CreateTime;

	private TreeEx tv_CountryDetails;

	private TreeListColumnEx tlc_Code;

	private RepositoryItemButtonEdit btn_NewDetails;

	private TreeListColumnEx tlc_Name;

	private TreeListColumnEx tlc_CountryCode;

	private TreeListColumnEx tlc_FatherCode;

	private TreeListColumnEx tlc_Status;

	private RepositoryItemLookUpEdit tlk_Status;

	private TreeListColumnEx tlc_Remarks;

	private TreeListColumnEx tlc_Id;

	private TreeListColumnEx tlc_FatherId;

	private RepositoryItemButtonEdit btn_New;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private ToolStripMenuItem menu_Duplicate;

	private RepositoryItemDateEditEx rep_CreateTime;

	private GridColumnEx gc_UpdateDate;

	private GridColumnEx gc_UpdateTime;

	private TreeListColumnEx tc_UserSign;

	private TreeListColumnEx tc_CreateDate;

	private TreeListColumnEx tc_CreateTime;

	private TreeListColumnEx tc_UpdateDate;

	private TreeListColumnEx tc_UpdateTime;

	private RepositoryItemDateEditEx repositoryItemDateEditEx3;

	private RepositoryItemHourEdit repositoryItemHourEdit1;

	private RepositoryItemDateEditEx repositoryItemDateEditEx4;

	private RepositoryItemHourEdit repositoryItemHourEdit2;

	private RepositoryItemDateEditEx repositoryItemDateEditEx1;

	private RepositoryItemHourEdit repositoryItemHourEdit3;

	private RepositoryItemDateEditEx repositoryItemDateEditEx2;

	private RepositoryItemHourEdit repositoryItemHourEdit4;

	private RepositoryItemDateEdit repositoryItemDateEdit1;

	private TreeListColumnEx treeListColumnEx1;

	private TreeListColumnEx treeListColumnEx2;

	private TreeListColumnEx treeListColumnEx3;

	private TreeListColumnEx treeListColumnEx4;

	public string CountryCode { get; set; }

	public string GgpCode { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btn_Cancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			menu_AddNewDoc.Enabled = false;
			menu_RemoveDoc.Enabled = false;
			menu_Duplicate.Enabled = true;
			menu_AddDetails.Enabled = true;
			menu_AddSame.Enabled = true;
			menu_Insert.Enabled = true;
			menu_RemoveDetails.Enabled = true;
			menu_Duplicate.Enabled = true;
			btn_OK.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			menu_AddDetails.Enabled = true;
			menu_AddSame.Enabled = true;
			menu_Insert.Enabled = true;
			menu_RemoveDetails.Enabled = true;
			menu_Duplicate.Enabled = true;
			btn_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			menu_AddNewDoc.Enabled = grd_Country.DataSourceTable.Columns.Count > 0;
			menu_RemoveDoc.Enabled = (grd_Country.DataSourceTable.IsNotNull() ? true : false);
			menu_Duplicate.Enabled = (grd_Country.DataSourceTable.IsNotNull() ? true : false);
			menu_AddDetails.Enabled = ((tv_CountryDetails.DataSourceTable.Columns.Count > 0 && grd_Country.DataSourceTable.IsNotNull()) ? true : false);
			menu_AddSame.Enabled = tv_CountryDetails.DataSourceTable.Rows.Count > 0;
			menu_Insert.Enabled = tv_CountryDetails.DataSourceTable.Rows.Count > 0;
			menu_RemoveDetails.Enabled = tv_CountryDetails.DataSourceTable.Rows.Count > 0;
			btn_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_Geography()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Geography"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Geography" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				cms_CountryMenu.Enabled = false;
				cms_CountryDetailsMenu.Enabled = false;
				btn_Cancel.Enabled = true;
			}
			else
			{
				OnLoadAsync += frm_Geography_Load;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	public frm_Geography(string countryCode, string ggpCode)
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Geography"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Geography" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				cms_CountryMenu.Enabled = false;
				cms_CountryDetailsMenu.Enabled = false;
				btn_Cancel.Enabled = true;
			}
			else
			{
				CountryCode = countryCode;
				GgpCode = ggpCode;
				OnLoadAsync += frm_Geography_Load;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	public async Task REFRESH()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			AddNewData();
			FormMode = Enums.FormViewModeType.Add;
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
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.First, Function.ToString(gv_Country.GetRowCellValue(gv_Country.FocusedRowHandle, "CountryCode")));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.Previous, Function.ToString(gv_Country.GetRowCellValue(gv_Country.FocusedRowHandle, "CountryCode")));
			WinCommons._CloseProcessing();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.Next, Function.ToString(gv_Country.GetRowCellValue(gv_Country.FocusedRowHandle, "CountryCode")));
			WinCommons._CloseProcessing();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_LAST()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.Last, Function.ToString(gv_Country.GetRowCellValue(gv_Country.FocusedRowHandle, "CountryCode")));
			WinCommons._CloseProcessing();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await LoadData();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void LoadInitForm()
	{
		try
		{
			if (!CheckTableExists())
			{
				WinFuntions.DisableAllControl(Controls);
				cms_CountryMenu.Enabled = false;
				cms_CountryDetailsMenu.Enabled = false;
				btn_Cancel.Enabled = false;
				WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
			}
			else
			{
				Dictionary<string, string> dic = new Dictionary<string, string>
				{
					{ "Y", "Hoạt động" },
					{ "N", "Không hoạt động" }
				};
				glk_Status.SetDataSource(dic);
				tlk_Status.SetDataSource(dic);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public bool CheckTableExists()
	{
		try
		{
			if (WinFuntions.GetTableStructure("APZ_D_OGGP").Columns.Count > 0 && WinFuntions.GetTableStructure("APZ_D_GGP1").Columns.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	public async Task LoadData(string countryCode = "")
	{
		try
		{
			grd_Country.DataSource = _httpClient.GeographyClient.GetGeography(countryCode, "", "");
			grd_Country.RefreshDataSource();
			if (grd_Country.DataSourceTable.IsNotNull())
			{
				countryCode = Function.ToString(gv_Country.GetRowCellValue(gv_Country.FocusedRowHandle, "CountryCode"));
				tv_CountryDetails.DataSource = _httpClient.GeographyClient.GetGeographyDetails(countryCode);
				tv_CountryDetails.RefreshDataSource();
			}
			else
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_D_GGP1");
				tv_CountryDetails.DataSource = tableStructure;
				tv_CountryDetails.RefreshDataSource();
			}
			_isDuplicate = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void AddNewDoc()
	{
		try
		{
			DataTable dataTable = grd_Country.DataSourceTable.Clone();
			DataRow dataRow = dataTable.NewRow();
			dataRow["Status"] = "Y";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_Country.DataSource = dataTable;
			grd_Country.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void AddGeographyDetails(TreeLevel level, bool isNew)
	{
		try
		{
			tv_CountryDetails.PostEditor();
			DataTable dataTable = (isNew ? tv_CountryDetails.DataSourceTable.Clone() : tv_CountryDetails.DataSourceTable);
			int num = Function.ParseInt(dataTable.AsEnumerable().Max((DataRow r) => r["Id"]));
			DataRow dataRow = dataTable.NewRow();
			switch (level)
			{
			case TreeLevel.Root:
				dataRow["Id"] = num + 1;
				dataRow["FatherId"] = -1;
				dataRow["CountryCode"] = Function.ToString(gv_Country.GetRowCellDisplayText(gv_Country.FocusedRowHandle, "CountryCode"));
				dataRow["Status"] = "Y";
				break;
			case TreeLevel.Insert:
				if (tv_CountryDetails.FocusedNode.Level == 3)
				{
					return;
				}
				dataRow["Id"] = num + 1;
				dataRow["FatherId"] = Function.ParseInt(tv_CountryDetails.FocusedNode.GetValue("Id"));
				dataRow["CountryCode"] = Function.ToString(gv_Country.GetRowCellDisplayText(gv_Country.FocusedRowHandle, "CountryCode"));
				dataRow["FatherCode"] = Function.ToString(tv_CountryDetails.FocusedNode.GetValue("Code"));
				dataRow["Status"] = "Y";
				break;
			case TreeLevel.SameLevel:
				dataRow["Id"] = num + 1;
				dataRow["FatherId"] = Function.ParseInt(tv_CountryDetails.FocusedNode.GetValue("FatherId"));
				dataRow["CountryCode"] = Function.ToString(gv_Country.GetRowCellDisplayText(gv_Country.FocusedRowHandle, "CountryCode"));
				dataRow["FatherCode"] = Function.ToString(tv_CountryDetails.FocusedNode.GetValue("FatherCode"));
				dataRow["Status"] = "Y";
				break;
			}
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			tv_CountryDetails.DataSource = dataTable;
			tv_CountryDetails.RefreshDataSource();
			if (tv_CountryDetails.FocusedNode.Selected)
			{
				tv_CountryDetails.FocusedNode.Expand();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void RemoveGeographyDetails()
	{
		try
		{
			if (!tv_CountryDetails.DataSourceTable.IsNotNull())
			{
				return;
			}
			TreeListMultiSelection selection = tv_CountryDetails.Selection;
			if (selection != null)
			{
				foreach (TreeListNode item in selection.ToList())
				{
					HttpResult httpResult = _httpClient.GeographyClient.CheckRemoveDetails(Function.ToString(item["Code"]));
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
						return;
					}
				}
				foreach (TreeListNode item2 in selection.ToList())
				{
					if (item2.Nodes != null)
					{
						foreach (TreeListNode item3 in item2.Nodes.ToList())
						{
							tv_CountryDetails.DeleteNode(item3);
						}
					}
					tv_CountryDetails.DeleteNode(item2);
				}
			}
			tv_CountryDetails.DataSourceTable.AcceptChanges();
			if (!tv_CountryDetails.DataSourceTable.IsNotNull())
			{
				AddGeographyDetails(TreeLevel.Root, isNew: true);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string SaveData(Tuple<DataSet, string> tupData)
	{
		try
		{
			HttpResult httpResult = _httpClient.GeographyClient.SaveData(tupData);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
	}

	public void LoadGeographyDetails(string countryCode)
	{
		try
		{
			DataTable geographyDetails = _httpClient.GeographyClient.GetGeographyDetails(countryCode);
			tv_CountryDetails.DataSource = geographyDetails;
			tv_CountryDetails.RefreshDataSource();
			if (geographyDetails.IsNotNull())
			{
				tv_CountryDetails.ExpandAll();
			}
			else if (grd_Country.DataSourceTable.IsNotNull() && !geographyDetails.IsNotNull())
			{
				AddGeographyDetails(TreeLevel.Root, isNew: true);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void GetRecord(Enums.NavigateType type, string countryCode)
	{
		try
		{
			DataTable dataTable = new DataTable();
			switch (type)
			{
			case Enums.NavigateType.First:
				dataTable = _httpClient.GeographyClient.GetRecord("F", countryCode);
				break;
			case Enums.NavigateType.Last:
				dataTable = _httpClient.GeographyClient.GetRecord("L", countryCode);
				break;
			case Enums.NavigateType.Previous:
				dataTable = _httpClient.GeographyClient.GetRecord("P", countryCode);
				break;
			case Enums.NavigateType.Next:
				dataTable = _httpClient.GeographyClient.GetRecord("N", countryCode);
				break;
			}
			if (dataTable.IsNotNull())
			{
				grd_Country.DataSource = dataTable;
				grd_Country.RefreshDataSource();
				LoadGeographyDetails(Function.ToString(dataTable.Rows[0]["CountryCode"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void AddNewData()
	{
		try
		{
			AddNewDoc();
			AddGeographyDetails(TreeLevel.Root, isNew: true);
			AddGeographyDetails(TreeLevel.Root, isNew: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_Geography_Load(object sender, EventArgs e)
	{
		try
		{
			LoadInitForm();
			await LoadData();
			if (grd_Country.DataSourceTable.IsNotNull() && tv_CountryDetails.DataSourceTable.IsNotNull())
			{
				if (!string.IsNullOrEmpty(CountryCode))
				{
					DataRow dataRow = grd_Country.DataSourceTable.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["CountryCode"]) == CountryCode);
					if (dataRow != null)
					{
						gv_Country.FocusedRowHandle = grd_Country.DataSourceTable.Rows.IndexOf(dataRow);
					}
				}
				if (!string.IsNullOrEmpty(GgpCode))
				{
					TreeListNode focusedNode = tv_CountryDetails.FindNodeByFieldValue("Code", GgpCode);
					tv_CountryDetails.FocusedNode = focusedNode;
				}
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		AddNewData();
		FormMode = Enums.FormViewModeType.Add;
		WinCommons._CloseProcessing();
	}

	private void addSameToolStripMenuItem_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		AddGeographyDetails(TreeLevel.SameLevel, isNew: false);
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
		WinCommons._CloseProcessing();
	}

	private void addRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		AddGeographyDetails(TreeLevel.Root, isNew: false);
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
		WinCommons._CloseProcessing();
	}

	private void addSubToolStripMenuItem_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		AddGeographyDetails(TreeLevel.Insert, isNew: false);
		AddGeographyDetails(TreeLevel.Insert, isNew: false);
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
		WinCommons._CloseProcessing();
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show((FormMode == Enums.FormViewModeType.Add) ? Enums.MessageCode.ConfirmSave : Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				btn_OK.Enabled = false;
				DataSet dataSet = new DataSet();
				DataTable dataTable = gv_Country.GetListDataSourceSelectedRow().CopyToDataTable();
				DataTable dataSourceTable = tv_CountryDetails.DataSourceTable;
				dataSet.Tables.Add(dataTable.Copy());
				dataSet.Tables.Add(dataSourceTable.Copy());
				dataSet.AcceptChanges();
				Tuple<DataSet, string> tupData = new Tuple<DataSet, string>(dataSet, (FormMode == Enums.FormViewModeType.Add) ? "A" : "U");
				string text = SaveData(tupData);
				if (!string.IsNullOrEmpty(text))
				{
					btn_OK.Enabled = true;
					WinCommons.Error(Enums.MessageCode.None, text);
					return;
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				await LoadData();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btn_OK.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void btn_Search_Click(object sender, EventArgs e)
	{
	}

	private void tv_CountryDetails_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		try
		{
			if ((e.Column.Name == tlc_Code.Name || e.Column.Name == tlc_Status.Name) && e.Node.Nodes != null)
			{
				tv_CountryDetails.BeginUpdate();
				foreach (TreeListNode item in e.Node.Nodes.ToList())
				{
					item.SetValue("FatherCode", Function.ToString(e.Node["Code"]));
					item.SetValue("Status", Function.ToString(e.Node["Status"]));
				}
				tv_CountryDetails.EndUpdate();
			}
			if (FormMode != Enums.FormViewModeType.Add)
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

	private void removeToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		RemoveGeographyDetails();
		FormMode = Enums.FormViewModeType.Update;
	}

	private void Btn_New_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		WinCommons._OpenProcessing();
		AddGeographyDetails(TreeLevel.Insert, isNew: false);
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
		WinCommons._CloseProcessing();
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void tv_CountryDetails_KeyDown(object sender, KeyEventArgs e)
	{
		KeysEvent(sender, e);
	}

	private void gv_Country_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			string countryCode = Function.ToString(gv_Country.GetRowCellValue(gv_Country.FocusedRowHandle, "CountryCode"));
			grd_Country.RefreshDataSource();
			LoadGeographyDetails(countryCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveItem, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				Tuple<string> tupParams = new Tuple<string>(Function.ToString(gv_Country.GetRowCellValue(gv_Country.FocusedRowHandle, "CountryCode")));
				WinCommons._OpenProcessing();
				HttpResult httpResult = _httpClient.GeographyClient.Remove(tupParams);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.OperationCompletedSuccess, "");
					await LoadData();
					FormMode = Enums.FormViewModeType.View;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
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

	private void gv_Country_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		if (e.Column.Name == "gc_Status" && Function.ToString(e.Value) == "N")
		{
			DataTable dataSourceTable = tv_CountryDetails.DataSourceTable;
			foreach (DataRow row in dataSourceTable.Rows)
			{
				row["Status"] = "N";
			}
			dataSourceTable.AcceptChanges();
		}
		if (e.Column.Name == gc_CountryCode.Name && !string.IsNullOrEmpty(Function.ToString(e.Value)))
		{
			DataTable dataSourceTable2 = tv_CountryDetails.DataSourceTable;
			if (dataSourceTable2.IsNotNull())
			{
				foreach (DataRow row2 in dataSourceTable2.Rows)
				{
					row2["CountryCode"] = Function.ToString(e.Value);
				}
				dataSourceTable2.AcceptChanges();
				tv_CountryDetails.DataSource = dataSourceTable2;
				tv_CountryDetails.RefreshDataSource();
			}
		}
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void frm_Geography_FormClosing(object sender, FormClosingEventArgs e)
	{
		if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
		{
			e.Cancel = true;
		}
	}

	private void gv_Country_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (string.Equals(e.Column.Name, gc_CountryCode.Name))
			{
				if (FormMode == Enums.FormViewModeType.Add)
				{
					e.Appearance.BackColor = Color.Empty;
				}
				else
				{
					e.Appearance.BackColor = Color.FromArgb(231, 231, 231);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Country_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add && string.Equals(gv_Country.FocusedColumn.Name, gc_CountryCode.Name))
			{
				if (_isDuplicate)
				{
					e.Cancel = false;
				}
				else
				{
					e.Cancel = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_Duplicate_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			DataTable dataTable = grd_Country.DataSourceTable.Clone();
			DataRow focusedDataRow = gv_Country.GetFocusedDataRow();
			if (dataTable.Columns.Count == 0 || focusedDataRow == null)
			{
				return;
			}
			_isDuplicate = true;
			focusedDataRow["CountryCode"] = string.Empty;
			focusedDataRow["CountryName"] = string.Empty;
			dataTable.Rows.Add(focusedDataRow.ItemArray);
			dataTable.AcceptChanges();
			grd_Country.DataSource = dataTable;
			grd_Country.RefreshDataSource();
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void tv_CountryDetails_EditorKeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Insert || e.KeyCode == Keys.Return || e.KeyCode == Keys.Delete)
		{
			KeysEvent(sender, e);
		}
	}

	private void KeysEvent(object sender, KeyEventArgs e)
	{
		try
		{
			if (!CheckTableExists())
			{
				WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
				return;
			}
			switch (e.KeyCode)
			{
			case Keys.Insert:
				AddGeographyDetails(TreeLevel.Insert, isNew: false);
				FormMode = Enums.FormViewModeType.Update;
				break;
			case Keys.Return:
				AddGeographyDetails(TreeLevel.SameLevel, isNew: false);
				FormMode = Enums.FormViewModeType.Update;
				break;
			case Keys.Delete:
				RemoveGeographyDetails();
				FormMode = Enums.FormViewModeType.Update;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Country_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Country.DataSourceTable;
			menu_AddNewDoc.Enabled = dataSourceTable.Columns.Count > 0;
			menu_RemoveDoc.Enabled = dataSourceTable.IsNotNull();
			menu_Duplicate.Enabled = dataSourceTable.IsNotNull();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void tv_CountryDetails_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Country.DataSourceTable;
			DataTable dataSourceTable2 = tv_CountryDetails.DataSourceTable;
			if (!dataSourceTable.IsNotNull())
			{
				menu_AddDetails.Enabled = false;
				menu_Insert.Enabled = false;
				menu_AddSame.Enabled = false;
				menu_RemoveDetails.Enabled = false;
			}
			else if (dataSourceTable.IsNotNull() && dataSourceTable2.Columns.Count > 0)
			{
				menu_AddDetails.Enabled = true;
				menu_Insert.Enabled = dataSourceTable2.IsNotNull();
				menu_AddSame.Enabled = dataSourceTable2.IsNotNull();
				menu_RemoveDetails.Enabled = dataSourceTable2.IsNotNull();
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.DMS.frm_Geography));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		this.cms_CountryMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menu_AddNewDoc = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_RemoveDoc = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_Duplicate = new System.Windows.Forms.ToolStripMenuItem();
		this.cms_CountryDetailsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menu_AddDetails = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_AddSame = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_Insert = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_RemoveDetails = new System.Windows.Forms.ToolStripMenuItem();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Country = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Country = new Apzon.Controls.Grid.GridViewEx();
		this.gc_CountryCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CountryName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Region = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Status = new Apzon.Controls.Grid.GridColumnEx();
		this.glk_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CreateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx3 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gc_CreateTime = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemHourEdit1 = new Apzon.Controls.Controls.RepositoryItemHourEdit();
		this.gc_UpdateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx4 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gc_UpdateTime = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemHourEdit2 = new Apzon.Controls.Controls.RepositoryItemHourEdit();
		this.rep_CreateTime = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.tv_CountryDetails = new Apzon.Controls.Tree.TreeEx();
		this.tlc_Code = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_NewDetails = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tlc_Name = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_CountryCode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_FatherCode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_Status = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlk_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.tlc_Remarks = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_Id = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_FatherId = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_UserSign = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_CreateDate = new Apzon.Controls.Tree.TreeListColumnEx();
		this.repositoryItemDateEditEx1 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.tc_CreateTime = new Apzon.Controls.Tree.TreeListColumnEx();
		this.repositoryItemHourEdit3 = new Apzon.Controls.Controls.RepositoryItemHourEdit();
		this.tc_UpdateDate = new Apzon.Controls.Tree.TreeListColumnEx();
		this.repositoryItemDateEditEx2 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.tc_UpdateTime = new Apzon.Controls.Tree.TreeListColumnEx();
		this.repositoryItemHourEdit4 = new Apzon.Controls.Controls.RepositoryItemHourEdit();
		this.treeListColumnEx1 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx2 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx3 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx4 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_New = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.repositoryItemDateEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.cms_CountryMenu.SuspendLayout();
		this.cms_CountryDetailsMenu.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Country).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Country).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.glk_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx3.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx4.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CreateTime).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CreateTime.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tv_CountryDetails).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_NewDetails).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tlk_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_New).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).BeginInit();
		base.SuspendLayout();
		this.cms_CountryMenu.ImageScalingSize = new System.Drawing.Size(28, 28);
		this.cms_CountryMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.menu_AddNewDoc, this.menu_RemoveDoc, this.menu_Duplicate });
		this.cms_CountryMenu.Name = "cms_CountryMenu";
		this.cms_CountryMenu.Size = new System.Drawing.Size(125, 70);
		this.menu_AddNewDoc.Name = "menu_AddNewDoc";
		this.menu_AddNewDoc.Size = new System.Drawing.Size(124, 22);
		this.menu_AddNewDoc.Text = "Add Row";
		this.menu_AddNewDoc.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.menu_RemoveDoc.Name = "menu_RemoveDoc";
		this.menu_RemoveDoc.Size = new System.Drawing.Size(124, 22);
		this.menu_RemoveDoc.Text = "Remove";
		this.menu_RemoveDoc.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.menu_Duplicate.Name = "menu_Duplicate";
		this.menu_Duplicate.Size = new System.Drawing.Size(124, 22);
		this.menu_Duplicate.Text = "Duplicate";
		this.menu_Duplicate.Click += new System.EventHandler(this.menu_Duplicate_Click);
		this.cms_CountryDetailsMenu.ImageScalingSize = new System.Drawing.Size(28, 28);
		this.cms_CountryDetailsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.menu_AddDetails, this.menu_AddSame, this.menu_Insert, this.menu_RemoveDetails });
		this.cms_CountryDetailsMenu.Name = "cms_CountryDetailsMenu";
		this.cms_CountryDetailsMenu.Size = new System.Drawing.Size(159, 92);
		this.menu_AddDetails.Enabled = false;
		this.menu_AddDetails.Name = "menu_AddDetails";
		this.menu_AddDetails.Size = new System.Drawing.Size(158, 22);
		this.menu_AddDetails.Text = "Add New";
		this.menu_AddDetails.Click += new System.EventHandler(this.addRowToolStripMenuItem1_Click);
		this.menu_AddSame.Enabled = false;
		this.menu_AddSame.Name = "menu_AddSame";
		this.menu_AddSame.Size = new System.Drawing.Size(158, 22);
		this.menu_AddSame.Text = "Add Same Level";
		this.menu_AddSame.Click += new System.EventHandler(this.addSameToolStripMenuItem_Click);
		this.menu_Insert.Enabled = false;
		this.menu_Insert.Name = "menu_Insert";
		this.menu_Insert.Size = new System.Drawing.Size(158, 22);
		this.menu_Insert.Text = "Insert";
		this.menu_Insert.Click += new System.EventHandler(this.addSubToolStripMenuItem_Click);
		this.menu_RemoveDetails.Enabled = false;
		this.menu_RemoveDetails.Name = "menu_RemoveDetails";
		this.menu_RemoveDetails.Size = new System.Drawing.Size(158, 22);
		this.menu_RemoveDetails.Text = "Remove";
		this.menu_RemoveDetails.Click += new System.EventHandler(this.removeToolStripMenuItem1_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(-1, 1);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Country);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.tv_CountryDetails);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(893, 457);
		this.splitContainerControl1.SplitterPosition = 176;
		this.splitContainerControl1.TabIndex = 12;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Country.AllowPasteExcel = true;
		this.grd_Country.ClipboardData = null;
		this.grd_Country.ContextMenuStrip = this.cms_CountryMenu;
		this.grd_Country.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Country.Location = new System.Drawing.Point(0, 0);
		this.grd_Country.MainView = this.gv_Country;
		this.grd_Country.Name = "grd_Country";
		this.grd_Country.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.glk_Status, this.rep_CreateTime, this.repositoryItemHourEdit1, this.repositoryItemHourEdit2, this.repositoryItemDateEditEx3, this.repositoryItemDateEditEx4 });
		this.grd_Country.Size = new System.Drawing.Size(893, 176);
		this.grd_Country.TabIndex = 5;
		this.grd_Country.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Country });
		this.gv_Country.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Country.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Country.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Country.ColumnKey = null;
		this.gv_Country.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.gc_CountryCode, this.gc_CountryName, this.gc_Region, this.gc_Status, this.gc_Remarks, this.gc_UserSign, this.gc_CreateDate, this.gc_CreateTime, this.gc_UpdateDate, this.gc_UpdateTime });
		this.gv_Country.DisableFormSetting = false;
		this.gv_Country.DisplayException = false;
		this.gv_Country.EditColumn = null;
		this.gv_Country.FieldCurrency = null;
		this.gv_Country.FieldRate = null;
		this.gv_Country.FormId = "oGeography";
		this.gv_Country.GridControl = this.grd_Country;
		this.gv_Country.IgnoreFilter = false;
		this.gv_Country.IndexEditRow = -1;
		this.gv_Country.IndicatorWidth = 35;
		this.gv_Country.IsApzTable = false;
		this.gv_Country.IsInsertNewRow = false;
		this.gv_Country.IsLoadingData = false;
		this.gv_Country.IsMappingContextAction = false;
		this.gv_Country.ItemId = "APZ_D_OGGP";
		this.gv_Country.LstColumnAllowBatch = null;
		this.gv_Country.LstColumnNotAllowBatch = null;
		this.gv_Country.LstColumnNotEditable = null;
		this.gv_Country.LstColumNotSetting = null;
		this.gv_Country.Name = "gv_Country";
		this.gv_Country.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Country.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Country.OptionsPrint.AutoWidth = false;
		this.gv_Country.OptionsPrint.UsePrintStyles = false;
		this.gv_Country.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Country.OptionsView.ColumnAutoWidth = false;
		this.gv_Country.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Country.OptionsView.ShowGroupedColumns = true;
		this.gv_Country.OptionsView.ShowGroupPanel = false;
		this.gv_Country.UseBatchMod = false;
		this.gv_Country.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Country_RowCellStyle);
		this.gv_Country.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Country_ShowingEditor);
		this.gv_Country.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Country_FocusedRowChanged);
		this.gv_Country.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Country_CellValueChanged);
		this.gv_Country.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gv_Country_MouseDown);
		this.gc_CountryCode.Caption = "Mã quốc gia";
		this.gc_CountryCode.CurrencyDateField = null;
		this.gc_CountryCode.CurrencyField = null;
		this.gc_CountryCode.CurrencyFixed = null;
		this.gc_CountryCode.CurrencyFixedRate = 0.0;
		this.gc_CountryCode.CurrencyRateField = null;
		this.gc_CountryCode.FieldName = "CountryCode";
		this.gc_CountryCode.Name = "gc_CountryCode";
		this.gc_CountryCode.PrimaryKey = true;
		this.gc_CountryCode.UnitField = null;
		this.gc_CountryCode.Visible = true;
		this.gc_CountryCode.VisibleIndex = 0;
		this.gc_CountryCode.Width = 80;
		this.gc_CountryName.Caption = "Tên";
		this.gc_CountryName.CurrencyDateField = null;
		this.gc_CountryName.CurrencyField = null;
		this.gc_CountryName.CurrencyFixed = null;
		this.gc_CountryName.CurrencyFixedRate = 0.0;
		this.gc_CountryName.CurrencyRateField = null;
		this.gc_CountryName.FieldName = "CountryName";
		this.gc_CountryName.Name = "gc_CountryName";
		this.gc_CountryName.PrimaryKey = false;
		this.gc_CountryName.UnitField = null;
		this.gc_CountryName.Visible = true;
		this.gc_CountryName.VisibleIndex = 1;
		this.gc_CountryName.Width = 120;
		this.gc_Region.Caption = "Region";
		this.gc_Region.CurrencyDateField = null;
		this.gc_Region.CurrencyField = null;
		this.gc_Region.CurrencyFixed = null;
		this.gc_Region.CurrencyFixedRate = 0.0;
		this.gc_Region.CurrencyRateField = null;
		this.gc_Region.FieldName = "Location";
		this.gc_Region.Name = "gc_Region";
		this.gc_Region.PrimaryKey = false;
		this.gc_Region.UnitField = null;
		this.gc_Status.Caption = "Trạng thái";
		this.gc_Status.ColumnEdit = this.glk_Status;
		this.gc_Status.CurrencyDateField = null;
		this.gc_Status.CurrencyField = null;
		this.gc_Status.CurrencyFixed = null;
		this.gc_Status.CurrencyFixedRate = 0.0;
		this.gc_Status.CurrencyRateField = null;
		this.gc_Status.FieldName = "Status";
		this.gc_Status.Name = "gc_Status";
		this.gc_Status.PrimaryKey = false;
		this.gc_Status.UnitField = null;
		this.gc_Status.Visible = true;
		this.gc_Status.VisibleIndex = 2;
		this.glk_Status.AutoHeight = false;
		this.glk_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.glk_Status.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.glk_Status.DisplayMember = "Name";
		this.glk_Status.Name = "glk_Status";
		this.glk_Status.NullText = "";
		this.glk_Status.ShowHeader = false;
		this.glk_Status.ValueMember = "Code";
		this.gc_Remarks.Caption = "Diễn giải";
		this.gc_Remarks.CurrencyDateField = null;
		this.gc_Remarks.CurrencyField = null;
		this.gc_Remarks.CurrencyFixed = null;
		this.gc_Remarks.CurrencyFixedRate = 0.0;
		this.gc_Remarks.CurrencyRateField = null;
		this.gc_Remarks.FieldName = "Remarks";
		this.gc_Remarks.Name = "gc_Remarks";
		this.gc_Remarks.PrimaryKey = false;
		this.gc_Remarks.UnitField = null;
		this.gc_Remarks.Visible = true;
		this.gc_Remarks.VisibleIndex = 3;
		this.gc_Remarks.Width = 322;
		this.gc_UserSign.Caption = "User Sign";
		this.gc_UserSign.CurrencyDateField = null;
		this.gc_UserSign.CurrencyField = null;
		this.gc_UserSign.CurrencyFixed = null;
		this.gc_UserSign.CurrencyFixedRate = 0.0;
		this.gc_UserSign.CurrencyRateField = null;
		this.gc_UserSign.FieldName = "UserSign";
		this.gc_UserSign.Name = "gc_UserSign";
		this.gc_UserSign.OptionsColumn.AllowEdit = false;
		this.gc_UserSign.OptionsColumn.ReadOnly = true;
		this.gc_UserSign.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
		this.gc_UserSign.PrimaryKey = false;
		this.gc_UserSign.UnitField = null;
		this.gc_CreateDate.Caption = "Create Date";
		this.gc_CreateDate.ColumnEdit = this.repositoryItemDateEditEx3;
		this.gc_CreateDate.CurrencyDateField = null;
		this.gc_CreateDate.CurrencyField = null;
		this.gc_CreateDate.CurrencyFixed = null;
		this.gc_CreateDate.CurrencyFixedRate = 0.0;
		this.gc_CreateDate.CurrencyRateField = null;
		this.gc_CreateDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gc_CreateDate.FieldName = "CreateDate";
		this.gc_CreateDate.Name = "gc_CreateDate";
		this.gc_CreateDate.OptionsColumn.AllowEdit = false;
		this.gc_CreateDate.OptionsColumn.ReadOnly = true;
		this.gc_CreateDate.PrimaryKey = false;
		this.gc_CreateDate.UnitField = null;
		this.repositoryItemDateEditEx3.AutoHeight = false;
		this.repositoryItemDateEditEx3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx3.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx3.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx3.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx3.EditFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx3.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx3.IsSearchControl = false;
		this.repositoryItemDateEditEx3.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx3.Name = "repositoryItemDateEditEx3";
		this.gc_CreateTime.Caption = "Create Time";
		this.gc_CreateTime.ColumnEdit = this.repositoryItemHourEdit1;
		this.gc_CreateTime.CurrencyDateField = null;
		this.gc_CreateTime.CurrencyField = null;
		this.gc_CreateTime.CurrencyFixed = null;
		this.gc_CreateTime.CurrencyFixedRate = 0.0;
		this.gc_CreateTime.CurrencyRateField = null;
		this.gc_CreateTime.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Time;
		this.gc_CreateTime.FieldName = "CreateTime";
		this.gc_CreateTime.Name = "gc_CreateTime";
		this.gc_CreateTime.OptionsColumn.AllowEdit = false;
		this.gc_CreateTime.OptionsColumn.ReadOnly = true;
		this.gc_CreateTime.PrimaryKey = false;
		this.gc_CreateTime.UnitField = null;
		this.repositoryItemHourEdit1.AutoHeight = false;
		this.repositoryItemHourEdit1.IsSearchControl = false;
		this.repositoryItemHourEdit1.MaxLength = 5;
		this.repositoryItemHourEdit1.Name = "repositoryItemHourEdit1";
		this.repositoryItemHourEdit1.ReadOnly = true;
		this.gc_UpdateDate.Caption = "Update Date";
		this.gc_UpdateDate.ColumnEdit = this.repositoryItemDateEditEx4;
		this.gc_UpdateDate.CurrencyDateField = null;
		this.gc_UpdateDate.CurrencyField = null;
		this.gc_UpdateDate.CurrencyFixed = null;
		this.gc_UpdateDate.CurrencyFixedRate = 0.0;
		this.gc_UpdateDate.CurrencyRateField = null;
		this.gc_UpdateDate.FieldName = "UpdateDate";
		this.gc_UpdateDate.Name = "gc_UpdateDate";
		this.gc_UpdateDate.OptionsColumn.AllowEdit = false;
		this.gc_UpdateDate.OptionsColumn.ReadOnly = true;
		this.gc_UpdateDate.PrimaryKey = false;
		this.gc_UpdateDate.UnitField = null;
		this.repositoryItemDateEditEx4.AutoHeight = false;
		this.repositoryItemDateEditEx4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx4.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx4.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx4.EditFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx4.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx4.IsSearchControl = false;
		this.repositoryItemDateEditEx4.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx4.Name = "repositoryItemDateEditEx4";
		this.gc_UpdateTime.Caption = "Update Time";
		this.gc_UpdateTime.ColumnEdit = this.repositoryItemHourEdit2;
		this.gc_UpdateTime.CurrencyDateField = null;
		this.gc_UpdateTime.CurrencyField = null;
		this.gc_UpdateTime.CurrencyFixed = null;
		this.gc_UpdateTime.CurrencyFixedRate = 0.0;
		this.gc_UpdateTime.CurrencyRateField = null;
		this.gc_UpdateTime.FieldName = "UpdateTime";
		this.gc_UpdateTime.Name = "gc_UpdateTime";
		this.gc_UpdateTime.OptionsColumn.AllowEdit = false;
		this.gc_UpdateTime.OptionsColumn.ReadOnly = true;
		this.gc_UpdateTime.PrimaryKey = false;
		this.gc_UpdateTime.UnitField = null;
		this.repositoryItemHourEdit2.AutoHeight = false;
		this.repositoryItemHourEdit2.IsSearchControl = false;
		this.repositoryItemHourEdit2.MaxLength = 5;
		this.repositoryItemHourEdit2.Name = "repositoryItemHourEdit2";
		this.repositoryItemHourEdit2.ReadOnly = true;
		this.rep_CreateTime.AutoHeight = false;
		this.rep_CreateTime.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_CreateTime.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_CreateTime.DisplayFormat.FormatString = "dd-MM-yyyy";
		this.rep_CreateTime.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rep_CreateTime.EditFormat.FormatString = "dd-MM-yyyy";
		this.rep_CreateTime.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rep_CreateTime.IsSearchControl = false;
		this.rep_CreateTime.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.rep_CreateTime.Name = "rep_CreateTime";
		this.tv_CountryDetails.AllowPasteExcel = true;
		this.tv_CountryDetails.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tv_CountryDetails.ColumnKey = null;
		this.tv_CountryDetails.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[17]
		{
			this.tlc_Code, this.tlc_Name, this.tlc_CountryCode, this.tlc_FatherCode, this.tlc_Status, this.tlc_Remarks, this.tlc_Id, this.tlc_FatherId, this.tc_UserSign, this.tc_CreateDate,
			this.tc_CreateTime, this.tc_UpdateDate, this.tc_UpdateTime, this.treeListColumnEx1, this.treeListColumnEx2, this.treeListColumnEx3, this.treeListColumnEx4
		});
		this.tv_CountryDetails.ContextMenuStrip = this.cms_CountryDetailsMenu;
		this.tv_CountryDetails.Cursor = System.Windows.Forms.Cursors.Default;
		this.tv_CountryDetails.DisableFormSetting = false;
		this.tv_CountryDetails.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tv_CountryDetails.EditColumn = null;
		this.tv_CountryDetails.FieldCurrency = null;
		this.tv_CountryDetails.FieldRate = null;
		this.tv_CountryDetails.FormId = "oGeography";
		this.tv_CountryDetails.IndexEditRow = -1;
		this.tv_CountryDetails.IndicatorWidth = 40;
		this.tv_CountryDetails.IsApzTable = false;
		this.tv_CountryDetails.IsLoadingData = false;
		this.tv_CountryDetails.ItemId = "APZ_D_GGP1";
		this.tv_CountryDetails.KeyFieldName = "Id";
		this.tv_CountryDetails.Location = new System.Drawing.Point(0, 0);
		this.tv_CountryDetails.LstColumnNotEditable = null;
		this.tv_CountryDetails.LstColumNotSetting = null;
		this.tv_CountryDetails.Name = "tv_CountryDetails";
		this.tv_CountryDetails.OptionsBehavior.CloseEditorOnLostFocus = false;
		this.tv_CountryDetails.OptionsBehavior.SmartMouseHover = false;
		this.tv_CountryDetails.OptionsDragAndDrop.AcceptOuterNodes = true;
		this.tv_CountryDetails.OptionsDragAndDrop.DragNodesMode = DevExpress.XtraTreeList.DragNodesMode.Multiple;
		this.tv_CountryDetails.OptionsDragAndDrop.DropNodesMode = DevExpress.XtraTreeList.DropNodesMode.Advanced;
		this.tv_CountryDetails.OptionsSelection.MultiSelect = true;
		this.tv_CountryDetails.OptionsView.AutoWidth = false;
		this.tv_CountryDetails.OptionsView.EnableAppearanceEvenRow = true;
		this.tv_CountryDetails.OptionsView.EnableAppearanceOddRow = true;
		this.tv_CountryDetails.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.RowFocus;
		this.tv_CountryDetails.ParentFieldName = "FatherId";
		this.tv_CountryDetails.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.tlk_Status, this.btn_New, this.btn_NewDetails, this.repositoryItemHourEdit3, this.repositoryItemHourEdit4, this.repositoryItemDateEdit1, this.repositoryItemDateEditEx1, this.repositoryItemDateEditEx2 });
		this.tv_CountryDetails.Size = new System.Drawing.Size(893, 271);
		this.tv_CountryDetails.TabIndex = 6;
		this.tv_CountryDetails.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_CountryDetails_CellValueChanged);
		this.tv_CountryDetails.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.tv_CountryDetails_EditorKeyDown);
		this.tv_CountryDetails.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tv_CountryDetails_KeyDown);
		this.tv_CountryDetails.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tv_CountryDetails_MouseDown);
		this.tlc_Code.Caption = "Mã";
		this.tlc_Code.ColumnEdit = this.btn_NewDetails;
		this.tlc_Code.CurrencyDateField = null;
		this.tlc_Code.CurrencyField = null;
		this.tlc_Code.CurrencyFixed = null;
		this.tlc_Code.CurrencyFixedRate = 0.0;
		this.tlc_Code.CurrencyRateField = null;
		this.tlc_Code.FieldName = "Code";
		this.tlc_Code.Name = "tlc_Code";
		this.tlc_Code.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tlc_Code.Visible = true;
		this.tlc_Code.VisibleIndex = 0;
		this.tlc_Code.Width = 120;
		this.btn_NewDetails.AutoHeight = false;
		this.btn_NewDetails.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Add Child", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_NewDetails.Name = "btn_NewDetails";
		this.btn_NewDetails.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.Btn_New_ButtonClick);
		this.tlc_Name.Caption = "Tên";
		this.tlc_Name.CurrencyDateField = null;
		this.tlc_Name.CurrencyField = null;
		this.tlc_Name.CurrencyFixed = null;
		this.tlc_Name.CurrencyFixedRate = 0.0;
		this.tlc_Name.CurrencyRateField = null;
		this.tlc_Name.FieldName = "Name";
		this.tlc_Name.Name = "tlc_Name";
		this.tlc_Name.Visible = true;
		this.tlc_Name.VisibleIndex = 1;
		this.tlc_Name.Width = 120;
		this.tlc_CountryCode.Caption = "Country Code";
		this.tlc_CountryCode.CurrencyDateField = null;
		this.tlc_CountryCode.CurrencyField = null;
		this.tlc_CountryCode.CurrencyFixed = null;
		this.tlc_CountryCode.CurrencyFixedRate = 0.0;
		this.tlc_CountryCode.CurrencyRateField = null;
		this.tlc_CountryCode.FieldName = "CountryCode";
		this.tlc_CountryCode.Name = "tlc_CountryCode";
		this.tlc_CountryCode.Width = 120;
		this.tlc_FatherCode.Caption = "FatherCode";
		this.tlc_FatherCode.CurrencyDateField = null;
		this.tlc_FatherCode.CurrencyField = null;
		this.tlc_FatherCode.CurrencyFixed = null;
		this.tlc_FatherCode.CurrencyFixedRate = 0.0;
		this.tlc_FatherCode.CurrencyRateField = null;
		this.tlc_FatherCode.FieldName = "FatherCode";
		this.tlc_FatherCode.Name = "tlc_FatherCode";
		this.tlc_FatherCode.Width = 120;
		this.tlc_Status.Caption = "Status";
		this.tlc_Status.ColumnEdit = this.tlk_Status;
		this.tlc_Status.CurrencyDateField = null;
		this.tlc_Status.CurrencyField = null;
		this.tlc_Status.CurrencyFixed = null;
		this.tlc_Status.CurrencyFixedRate = 0.0;
		this.tlc_Status.CurrencyRateField = null;
		this.tlc_Status.FieldName = "Status";
		this.tlc_Status.Name = "tlc_Status";
		this.tlc_Status.Width = 120;
		this.tlk_Status.AutoHeight = false;
		this.tlk_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.tlk_Status.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.tlk_Status.DisplayMember = "Name";
		this.tlk_Status.Name = "tlk_Status";
		this.tlk_Status.ShowHeader = false;
		this.tlk_Status.ValueMember = "Code";
		this.tlc_Remarks.Caption = "Diễn giải";
		this.tlc_Remarks.CurrencyDateField = null;
		this.tlc_Remarks.CurrencyField = null;
		this.tlc_Remarks.CurrencyFixed = null;
		this.tlc_Remarks.CurrencyFixedRate = 0.0;
		this.tlc_Remarks.CurrencyRateField = null;
		this.tlc_Remarks.FieldName = "Remarks";
		this.tlc_Remarks.Name = "tlc_Remarks";
		this.tlc_Remarks.Visible = true;
		this.tlc_Remarks.VisibleIndex = 2;
		this.tlc_Remarks.Width = 120;
		this.tlc_Id.Caption = "Id";
		this.tlc_Id.CurrencyDateField = null;
		this.tlc_Id.CurrencyField = null;
		this.tlc_Id.CurrencyFixed = null;
		this.tlc_Id.CurrencyFixedRate = 0.0;
		this.tlc_Id.CurrencyRateField = null;
		this.tlc_Id.FieldName = "Id";
		this.tlc_Id.Name = "tlc_Id";
		this.tlc_Id.Width = 120;
		this.tlc_FatherId.Caption = "FatherId";
		this.tlc_FatherId.CurrencyDateField = null;
		this.tlc_FatherId.CurrencyField = null;
		this.tlc_FatherId.CurrencyFixed = null;
		this.tlc_FatherId.CurrencyFixedRate = 0.0;
		this.tlc_FatherId.CurrencyRateField = null;
		this.tlc_FatherId.FieldName = "FatherId";
		this.tlc_FatherId.Name = "tlc_FatherId";
		this.tlc_FatherId.Width = 120;
		this.tc_UserSign.Caption = "UserSign";
		this.tc_UserSign.CurrencyDateField = null;
		this.tc_UserSign.CurrencyField = null;
		this.tc_UserSign.CurrencyFixed = null;
		this.tc_UserSign.CurrencyFixedRate = 0.0;
		this.tc_UserSign.CurrencyRateField = null;
		this.tc_UserSign.FieldName = "UserSign";
		this.tc_UserSign.Name = "tc_UserSign";
		this.tc_UserSign.Width = 120;
		this.tc_CreateDate.Caption = "Create Date";
		this.tc_CreateDate.ColumnEdit = this.repositoryItemDateEditEx1;
		this.tc_CreateDate.CurrencyDateField = null;
		this.tc_CreateDate.CurrencyField = null;
		this.tc_CreateDate.CurrencyFixed = null;
		this.tc_CreateDate.CurrencyFixedRate = 0.0;
		this.tc_CreateDate.CurrencyRateField = null;
		this.tc_CreateDate.FieldName = "Create Date";
		this.tc_CreateDate.Name = "tc_CreateDate";
		this.tc_CreateDate.OptionsColumn.AllowEdit = false;
		this.tc_CreateDate.OptionsColumn.ReadOnly = true;
		this.tc_CreateDate.Width = 120;
		this.repositoryItemDateEditEx1.AutoHeight = false;
		this.repositoryItemDateEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.EditFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx1.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.IsSearchControl = false;
		this.repositoryItemDateEditEx1.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx1.Name = "repositoryItemDateEditEx1";
		this.tc_CreateTime.Caption = "Create Time";
		this.tc_CreateTime.ColumnEdit = this.repositoryItemHourEdit3;
		this.tc_CreateTime.CurrencyDateField = null;
		this.tc_CreateTime.CurrencyField = null;
		this.tc_CreateTime.CurrencyFixed = null;
		this.tc_CreateTime.CurrencyFixedRate = 0.0;
		this.tc_CreateTime.CurrencyRateField = null;
		this.tc_CreateTime.FieldName = "CreateTime";
		this.tc_CreateTime.Name = "tc_CreateTime";
		this.tc_CreateTime.OptionsColumn.AllowEdit = false;
		this.tc_CreateTime.OptionsColumn.ReadOnly = true;
		this.tc_CreateTime.Width = 120;
		this.repositoryItemHourEdit3.AutoHeight = false;
		this.repositoryItemHourEdit3.IsSearchControl = false;
		this.repositoryItemHourEdit3.MaxLength = 5;
		this.repositoryItemHourEdit3.Name = "repositoryItemHourEdit3";
		this.repositoryItemHourEdit3.ReadOnly = true;
		this.tc_UpdateDate.Caption = "Update Date";
		this.tc_UpdateDate.ColumnEdit = this.repositoryItemDateEditEx2;
		this.tc_UpdateDate.CurrencyDateField = null;
		this.tc_UpdateDate.CurrencyField = null;
		this.tc_UpdateDate.CurrencyFixed = null;
		this.tc_UpdateDate.CurrencyFixedRate = 0.0;
		this.tc_UpdateDate.CurrencyRateField = null;
		this.tc_UpdateDate.FieldName = "UpdateDate";
		this.tc_UpdateDate.Name = "tc_UpdateDate";
		this.tc_UpdateDate.OptionsColumn.AllowEdit = false;
		this.tc_UpdateDate.OptionsColumn.ReadOnly = true;
		this.tc_UpdateDate.Width = 120;
		this.repositoryItemDateEditEx2.AutoHeight = false;
		this.repositoryItemDateEditEx2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx2.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx2.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx2.EditFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx2.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx2.IsSearchControl = false;
		this.repositoryItemDateEditEx2.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx2.Name = "repositoryItemDateEditEx2";
		this.tc_UpdateTime.Caption = "Update Time";
		this.tc_UpdateTime.ColumnEdit = this.repositoryItemHourEdit4;
		this.tc_UpdateTime.CurrencyDateField = null;
		this.tc_UpdateTime.CurrencyField = null;
		this.tc_UpdateTime.CurrencyFixed = null;
		this.tc_UpdateTime.CurrencyFixedRate = 0.0;
		this.tc_UpdateTime.CurrencyRateField = null;
		this.tc_UpdateTime.FieldName = "UpdateTime";
		this.tc_UpdateTime.Name = "tc_UpdateTime";
		this.tc_UpdateTime.OptionsColumn.AllowEdit = false;
		this.tc_UpdateTime.OptionsColumn.ReadOnly = true;
		this.tc_UpdateTime.Width = 120;
		this.repositoryItemHourEdit4.AutoHeight = false;
		this.repositoryItemHourEdit4.IsSearchControl = false;
		this.repositoryItemHourEdit4.MaxLength = 5;
		this.repositoryItemHourEdit4.Name = "repositoryItemHourEdit4";
		this.repositoryItemHourEdit4.ReadOnly = true;
		this.treeListColumnEx1.Caption = "Cấp ( Tỉnh - Huyện - Xã )";
		this.treeListColumnEx1.CurrencyDateField = null;
		this.treeListColumnEx1.CurrencyField = null;
		this.treeListColumnEx1.CurrencyFixed = null;
		this.treeListColumnEx1.CurrencyFixedRate = 0.0;
		this.treeListColumnEx1.CurrencyRateField = null;
		this.treeListColumnEx1.FieldName = "Type";
		this.treeListColumnEx1.Name = "treeListColumnEx1";
		this.treeListColumnEx1.Visible = true;
		this.treeListColumnEx1.VisibleIndex = 3;
		this.treeListColumnEx1.Width = 141;
		this.treeListColumnEx2.Caption = "Tên bao gồm cấp";
		this.treeListColumnEx2.CurrencyDateField = null;
		this.treeListColumnEx2.CurrencyField = null;
		this.treeListColumnEx2.CurrencyFixed = null;
		this.treeListColumnEx2.CurrencyFixedRate = 0.0;
		this.treeListColumnEx2.CurrencyRateField = null;
		this.treeListColumnEx2.FieldName = "NameWithType";
		this.treeListColumnEx2.Name = "treeListColumnEx2";
		this.treeListColumnEx2.Visible = true;
		this.treeListColumnEx2.VisibleIndex = 4;
		this.treeListColumnEx2.Width = 120;
		this.treeListColumnEx3.Caption = "Đường dẫn";
		this.treeListColumnEx3.CurrencyDateField = null;
		this.treeListColumnEx3.CurrencyField = null;
		this.treeListColumnEx3.CurrencyFixed = null;
		this.treeListColumnEx3.CurrencyFixedRate = 0.0;
		this.treeListColumnEx3.CurrencyRateField = null;
		this.treeListColumnEx3.FieldName = "NamePath";
		this.treeListColumnEx3.Name = "treeListColumnEx3";
		this.treeListColumnEx3.Visible = true;
		this.treeListColumnEx3.VisibleIndex = 5;
		this.treeListColumnEx3.Width = 250;
		this.treeListColumnEx4.Caption = "Tên đầy đủ";
		this.treeListColumnEx4.CurrencyDateField = null;
		this.treeListColumnEx4.CurrencyField = null;
		this.treeListColumnEx4.CurrencyFixed = null;
		this.treeListColumnEx4.CurrencyFixedRate = 0.0;
		this.treeListColumnEx4.CurrencyRateField = null;
		this.treeListColumnEx4.FieldName = "NamePathWithType";
		this.treeListColumnEx4.Name = "treeListColumnEx4";
		this.treeListColumnEx4.Visible = true;
		this.treeListColumnEx4.VisibleIndex = 6;
		this.treeListColumnEx4.Width = 300;
		this.btn_New.Appearance.Image = (System.Drawing.Image)resources.GetObject("btn_New.Appearance.Image");
		this.btn_New.Appearance.Options.UseImage = true;
		this.btn_New.AutoHeight = false;
		this.btn_New.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "Add Child", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_New.Name = "btn_New";
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
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_Cancel.Appearance.Options.UseFont = true;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(83, 464);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 8;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_OK.Appearance.Options.UseFont = true;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(6, 464);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 20);
		this.btn_OK.TabIndex = 7;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(896, 491);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oGeography";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Geography";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
		this.Text = "Vị trị địa lý";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Geography_FormClosing);
		this.cms_CountryMenu.ResumeLayout(false);
		this.cms_CountryDetailsMenu.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Country).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Country).EndInit();
		((System.ComponentModel.ISupportInitialize)this.glk_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx3.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx4.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CreateTime.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CreateTime).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tv_CountryDetails).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_NewDetails).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tlk_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_New).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).EndInit();
		base.ResumeLayout(false);
	}
}
