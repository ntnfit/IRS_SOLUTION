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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace Apzon.Forms.DMS;

public class frm_SaleTerritory : XtraFormEx
{
	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isDuplicate;

	private bool _isAddStore;

	private DataTable _dtDetail = new DataTable();

	private List<Common.ParameterValue> _lstParams = new List<Common.ParameterValue>();

	private IContainer components;

	private LabelLine lbl_TerritoryCode;

	private LabelLine lbl_Status;

	private LabelLine lbl_Location;

	private TextEditEx txt_Location;

	private LookUpEditEx lk_Status;

	private SimpleButtonEx btn_Search;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_SaleTerritory;

	private GridViewEx gv_SaleTerritory;

	private XtraTabControlEx tab_Main;

	private XtraTabPageEx tab_Structure;

	private SplitContainerControl splitContainerControl2;

	private XtraTabPageEx tab_Stores;

	private GridControlEx grd_Stores;

	private GridViewEx gv_Stores;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private GridColumnEx gc_TerritoryCode;

	private GridColumnEx gc_Name;

	private GridColumnEx gc_Type;

	private GridColumnEx gc_Status;

	private GroupControl grb_Detail;

	private GridControlEx grd_Detail;

	private GridViewEx gv_Detail;

	private GroupControl grb_Structure;

	private GridColumnEx gc_Remarks;

	private GridColumnEx gc_UserSign;

	private GridColumnEx gc_CreateDate;

	private GridColumnEx gc_CreateTime;

	private TreeEx tv_Structure;

	private TreeListColumnEx tlc_Name;

	private TreeListColumnEx tlc_FCode;

	private TreeListColumnEx tlc_Status;

	private TreeListColumnEx tlc_Remarks;

	private GridColumnEx gc_DTerritoryCode;

	private GridColumnEx gc_GgpRemarks;

	private GridColumnEx gc_Code;

	private GridColumnEx gc_StrName;

	private GridColumnEx gc_SRemarks;

	private ContextMenuStrip cms_Structure;

	private ContextMenuStrip cms_Territory;

	private ContextMenuStrip cms_Store;

	private ToolStripMenuItem menu_AddStructure;

	private ToolStripMenuItem menu_AddSameStructure;

	private ToolStripMenuItem menu_InsertStructure;

	private ToolStripMenuItem menu_RemoveStructure;

	private ToolStripMenuItem menu_AddStore;

	private ToolStripMenuItem menu_RemoveStore;

	private RepositoryItemLookUpEdit tlk_Status;

	private ToolStripMenuItem menu_AddTerritory;

	private ToolStripMenuItem menu_RemoveTerritory;

	private RepositoryItemButtonEdit btn_AddStructure;

	private GridColumnEx gc_Country;

	private GridColumnEx gc_StrStatus;

	private GridColumnEx gc_StrCardCode;

	private GridColumnEx gc_StrAddress;

	private RepositoryItemLookUpEdit glk_StrStatus;

	private RepositoryItemButtonEditCFL rep_CardCode;

	private GridColumnEx gc_DRegionCode;

	private GridColumnEx gc_StrCode;

	private TreeListColumnEx tlc_Id;

	private TreeListColumnEx tlc_FatherId;

	private RepositoryItemButtonEditCFL Rep_RegionCFL;

	private RepositoryItemButtonEditCFL rep_CountryCFL;

	private RepositoryItemLookUpEdit rep_Status;

	private RepositoryItemLookUpEdit glk_Status;

	private TreeListColumnEx tlc_Code;

	private TreeListColumnEx tlc_GgpCode;

	private RepositoryItemButtonEditCFL rep_Region;

	private RepositoryItemButtonEdit repositoryItemButtonEdit1;

	private TreeListColumnEx tlc_RegionCode;

	private ContextMenuStrip cms_Detail;

	private ToolStripMenuItem menu_AddDetail;

	private ToolStripMenuItem menu_RemoveDetail;

	private RepositoryItemButtonEditCFL rep_DetailCFL;

	private RepositoryItemButtonEditCFL rep_StoreCFL;

	private TextEditEx txt_TerritoryCode;

	private GridColumnEx gc_DGgpCode;

	private GridColumnEx gc_DGgpName;

	private RepositoryItemButtonEditCFL rep_CountriesCFL;

	private RepositoryItemButtonEditCFL rep_StoresCFL;

	private ToolStripMenuItem menu_DuplicateTerritory;

	private ToolStripMenuItem menu_DuplicateStore;

	private GridColumnEx gc_RegionCode;

	private RepositoryItemButtonEditCFL rep_StoreRegion;

	private GridColumnEx gc_City;

	private GridColumnEx gc_District;

	private GridColumnEx gc_Ward;

	private GridColumnEx gc_Town;

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
			menu_AddTerritory.Enabled = false;
			menu_RemoveTerritory.Enabled = false;
			menu_DuplicateTerritory.Enabled = true;
			menu_AddStructure.Enabled = ((grd_SaleTerritory.DataSourceTable.IsNotNull() && tv_Structure.DataSourceTable.IsNotNull()) ? true : false);
			menu_AddSameStructure.Enabled = (tv_Structure.DataSourceTable.IsNotNull() ? true : false);
			menu_InsertStructure.Enabled = (tv_Structure.DataSourceTable.IsNotNull() ? true : false);
			menu_RemoveStructure.Enabled = (tv_Structure.DataSourceTable.IsNotNull() ? true : false);
			menu_AddDetail.Enabled = true;
			menu_RemoveDetail.Enabled = false;
			menu_AddStore.Enabled = true;
			menu_RemoveStore.Enabled = false;
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			menu_AddTerritory.Enabled = true;
			menu_RemoveTerritory.Enabled = (grd_SaleTerritory.DataSourceTable.IsNotNull() ? true : false);
			menu_DuplicateTerritory.Enabled = (grd_SaleTerritory.DataSourceTable.IsNotNull() ? true : false);
			menu_AddStructure.Enabled = ((grd_SaleTerritory.DataSourceTable.IsNotNull() && tv_Structure.DataSourceTable.IsNotNull()) ? true : false);
			menu_AddSameStructure.Enabled = (tv_Structure.DataSourceTable.IsNotNull() ? true : false);
			menu_InsertStructure.Enabled = (tv_Structure.DataSourceTable.IsNotNull() ? true : false);
			menu_RemoveStructure.Enabled = (tv_Structure.DataSourceTable.IsNotNull() ? true : false);
			menu_AddDetail.Enabled = (tv_Structure.DataSourceTable.IsNotNull() ? true : false);
			menu_RemoveDetail.Enabled = (grd_Detail.DataSourceTable.IsNotNull() ? true : false);
			menu_RemoveStore.Enabled = (grd_Stores.DataSourceTable.IsNotNull() ? true : false);
			menu_DuplicateStore.Enabled = (grd_Stores.DataSourceTable.IsNotNull() ? true : false);
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		default:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_SaleTerritory()
	{
		InitializeComponent();
		OnLoadAsync += frm_SaleTerritory_Load;
	}

	public async Task REFRESH()
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			await AddNewDoc();
			await AddNewStructure("R", isNew: true);
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
			GetRecord(Enums.NavigateType.First, Function.ToString(gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "Code")));
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
			GetRecord(Enums.NavigateType.Previous, Function.ToString(gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "Code")));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.Next, Function.ToString(gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "Code")));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.Last, Function.ToString(gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "Code")));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			WinCommons._OpenProcessing();
			await LoadData();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task LoadInitForm()
	{
		try
		{
			if (!CheckTableExists())
			{
				WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
				return;
			}
			Dictionary<string, string> dic = new Dictionary<string, string>
			{
				{ "A", "Active" },
				{ "I", "Inactive" }
			};
			lk_Status.SetDataSource(dic);
			rep_Status.SetDataSource(dic);
			tlk_Status.SetDataSource(dic);
			glk_StrStatus.SetDataSource(dic);
			rep_CountriesCFL.ButtonClick += Rep_CountryCFL_ButtonClick;
			btn_AddStructure.ButtonClick += Btn_AddGgpDetails_ButtonClick;
			rep_Region.ButtonClick += Rep_Region_ButtonClick;
			rep_DetailCFL.ButtonClick += Rep_AddDetail_ButtonClick;
			rep_StoresCFL.ButtonClick += Rep_StoreCFL_ButtonClick;
			_dtDetail = (await _httpClient.SaleTerritoryClient.GetStructureDetails("", "", "")).Clone();
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
			if (WinFuntions.GetTableStructure("APZ_D_OSTR").Columns.Count > 0 || WinFuntions.GetTableStructure("APZ_D_STR1").Columns.Count > 0 || WinFuntions.GetTableStructure("APZ_D_STR2").Columns.Count > 0 || WinFuntions.GetTableStructure("APZ_D_STR3").Columns.Count > 0)
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

	public void LoadGeography(string countryCode, string ggpCode)
	{
		try
		{
			if (string.IsNullOrEmpty(countryCode))
			{
				return;
			}
			WinCommons._OpenProcessing();
			frm_Geography frm_Geography2 = new frm_Geography(countryCode, ggpCode);
			frm_Geography2.MdiParent = MdiParent;
			frm_Geography2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task LoadTreeView(string territoryCode)
	{
		_ = 2;
		try
		{
			DataTable dataSource = await _httpClient.SaleTerritoryClient.GetBranchStructure(territoryCode);
			tv_Structure.DataSource = dataSource;
			tv_Structure.RefreshDataSource();
			if (tv_Structure.DataSourceTable.IsNotNull())
			{
				tv_Structure.ExpandAll();
				return;
			}
			GridControlEx gridControlEx = grd_Detail;
			gridControlEx.DataSource = await _httpClient.SaleTerritoryClient.GetStructureDetails("", "", "");
			grd_Detail.RefreshDataSource();
			gridControlEx = grd_Stores;
			gridControlEx.DataSource = await _httpClient.SaleTerritoryClient.GetStore("", "");
			grd_Stores.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task LoadData(string territoryCode = "", string location = "", string status = "")
	{
		_ = 1;
		try
		{
			DataTable dataSource = await _httpClient.SaleTerritoryClient.GetSaleTerritory(territoryCode, location, status);
			grd_SaleTerritory.DataSource = dataSource;
			grd_SaleTerritory.RefreshDataSource();
			if (grd_SaleTerritory.DataSourceTable.IsNotNull())
			{
				territoryCode = Function.ToString(gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "Code"));
			}
			else
			{
				menu_RemoveTerritory.Enabled = false;
				menu_DuplicateTerritory.Enabled = false;
			}
			await LoadTreeView(territoryCode);
			_isDuplicate = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task AddNewDoc()
	{
		try
		{
			DataTable dataTable = grd_SaleTerritory.DataSourceTable.Clone();
			DataRow dataRow = dataTable.NewRow();
			dataRow["Status"] = "A";
			dataRow["UserSign"] = GlobalData.UserName;
			dataRow["CreateDate"] = DateTime.Now.Date;
			dataRow["CreateTime"] = Function.ParseInt(DateTime.Now.ToString("HHmmss"));
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_SaleTerritory.DataSource = dataTable;
			grd_SaleTerritory.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task AddNewStructure(string level, bool isNew)
	{
		try
		{
			DataTable dataTable = ((!isNew) ? tv_Structure.DataSourceTable : tv_Structure.DataSourceTable.Clone());
			if (dataTable.Columns.Count <= 0)
			{
				return;
			}
			int num = Function.ParseInt(dataTable.AsEnumerable().Max((DataRow r) => r["Id"]));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = Function.ToString(gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "Code"));
			dataRow["Status"] = "A";
			switch (level.ToUpper())
			{
			case "R":
				dataRow["FatherCode"] = string.Empty;
				dataRow["Id"] = num + 1;
				dataRow["FatherId"] = -1;
				break;
			case "S":
				dataRow["FatherCode"] = Function.ToString(tv_Structure.FocusedNode.GetValue("RegionCode"));
				dataRow["Id"] = num + 1;
				dataRow["FatherId"] = Function.ParseInt(tv_Structure.FocusedNode.GetValue("FatherId"));
				break;
			case "I":
				if (tv_Structure.FocusedNode.Level == 3)
				{
					return;
				}
				dataRow["FatherCode"] = Function.ToString(tv_Structure.FocusedNode.GetValue("RegionCode"));
				dataRow["Id"] = num + 1;
				dataRow["FatherId"] = Function.ParseInt(tv_Structure.FocusedNode.GetValue("Id"));
				break;
			}
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			tv_Structure.DataSource = dataTable;
			tv_Structure.RefreshDataSource();
			if (tv_Structure.FocusedNode.Nodes != null)
			{
				tv_Structure.FocusedNode.Expand();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task AddNewDetails(string territoryCode, string regionCode, string ggpCode, bool isNew)
	{
		try
		{
			DataTable dataTable = (isNew ? grd_Detail.DataSourceTable.Clone() : grd_Detail.DataSourceTable);
			if (dataTable.Columns.Count != 0)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = territoryCode;
				dataRow["RegionCode"] = regionCode;
				dataRow["GgpCode1"] = ggpCode;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_Detail.DataSource = dataTable;
				grd_Detail.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void AddNewStore(string territoryCode)
	{
		try
		{
			DataTable dataTable = grd_Stores.DataSourceTable.Clone();
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = territoryCode;
			dataRow["Status"] = "A";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_Stores.DataSource = dataTable;
			grd_Stores.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task SaveData(DataSet ds, string type)
	{
		_ = 1;
		try
		{
			if (ds == null || ds.Tables.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
				return;
			}
			if (!string.IsNullOrEmpty(Function.ToString(gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "CountryCode"))))
			{
				foreach (DataRow row in ds.Tables[0].Rows)
				{
					if (!CheckGeography(Function.ToString(row["CountryCode"]), 1))
					{
						return;
					}
				}
				foreach (DataRow row2 in ds.Tables[1].Rows)
				{
					if (!CheckGeography(Function.ToString(row2["GgpCode"]), 0))
					{
						return;
					}
				}
				foreach (DataRow row3 in ds.Tables[2].Rows)
				{
					if (!CheckGeography(Function.ToString(row3["GgpCode"]), 0))
					{
						return;
					}
				}
			}
			Tuple<DataSet, string> tuple = new Tuple<DataSet, string>(ds, type);
			WinCommons._OpenProcessing();
			HttpResult httpResult = await _httpClient.SaleTerritoryClient.SaveData(tuple);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				await LoadData();
				tab_Main.SelectedTabPageIndex = 0;
				_isDuplicate = false;
				_isDuplicate = false;
				_isAddStore = false;
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async void GetRecord(Enums.NavigateType type, string territoryCode)
	{
		_ = 4;
		try
		{
			DataTable dataTable = new DataTable();
			switch (type)
			{
			case Enums.NavigateType.First:
				dataTable = await _httpClient.SaleTerritoryClient.GetRecord("F", territoryCode);
				break;
			case Enums.NavigateType.Last:
				dataTable = await _httpClient.SaleTerritoryClient.GetRecord("L", territoryCode);
				break;
			case Enums.NavigateType.Previous:
				dataTable = await _httpClient.SaleTerritoryClient.GetRecord("P", territoryCode);
				break;
			case Enums.NavigateType.Next:
				dataTable = await _httpClient.SaleTerritoryClient.GetRecord("N", territoryCode);
				break;
			}
			if (dataTable.IsNotNull())
			{
				grd_SaleTerritory.DataSource = dataTable;
				grd_SaleTerritory.RefreshDataSource();
				await LoadTreeView(Function.ToString(dataTable.Rows[0]["Code"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string GetListGeographyCode(string ggpCode)
	{
		try
		{
			string text = string.Empty;
			IEnumerable<TreeListNode> enumerable = from r in tv_Structure.GetNodeList().AsEnumerable()
				where ggpCode == Function.ToString(r["GgpCode"])
				select r;
			EnumerableRowCollection<DataRow> enumerableRowCollection = from r in grd_Detail.DataSourceTable.AsEnumerable()
				where !string.IsNullOrEmpty(Function.ToString(r["GgpCode"]))
				select r;
			if (enumerable != null)
			{
				foreach (TreeListNode item in enumerable)
				{
					text = text + Function.ToString(item["GgpCode"]) + ",";
				}
				if (enumerableRowCollection != null)
				{
					foreach (DataRow item2 in enumerableRowCollection)
					{
						text = text + Function.ToString(item2["GgpCode"]) + ",";
					}
				}
			}
			return (text.Length > 0) ? text.Remove(text.Length - 1, 1) : string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return string.Empty;
		}
	}

	public async Task LoadDataLines(string territoryCode, string regionCode, string ggpCode)
	{
		try
		{
			if (_dtDetail.Columns.Count == 0)
			{
				return;
			}
			DataTable dataTable = await _httpClient.SaleTerritoryClient.GetStructureDetails(territoryCode, regionCode, ggpCode);
			if (_dtDetail.AsEnumerable().FirstOrDefault((DataRow dataRow) => Function.ToString(dataRow["Code"]) == territoryCode && Function.ToString(dataRow["RegionCode"]) == regionCode && Function.ToString(dataRow["GgpCode1"]) == ggpCode) == null)
			{
				foreach (DataRow row in dataTable.Rows)
				{
					_dtDetail.ImportRow(row);
				}
			}
			_dtDetail.AcceptChanges();
			DataRow[] array = _dtDetail.Select("Code = '" + territoryCode + "' and RegionCode = '" + regionCode + "' and GgpCode1 = '" + ggpCode + "'");
			if (array.Length == 0)
			{
				await AddNewDetails(territoryCode, regionCode, ggpCode, isNew: true);
			}
			else
			{
				grd_Detail.DataSource = array.CopyToDataTable();
				grd_Detail.RefreshDataSource();
			}
			string listGeographyCode = GetListGeographyCode(ggpCode);
			GridControlEx gridControlEx = grd_Stores;
			gridControlEx.DataSource = await _httpClient.SaleTerritoryClient.GetStore(territoryCode, listGeographyCode);
			grd_Stores.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task SaveTemporaryData()
	{
		try
		{
			if (_dtDetail.Columns.Count == 0)
			{
				return;
			}
			DataTable dataTable = grd_Detail.DataSourceTable.Copy();
			if (!dataTable.IsNotNull())
			{
				return;
			}
			foreach (DataRow row in dataTable.Rows)
			{
				if (_dtDetail.AsEnumerable().FirstOrDefault((DataRow drDoc) => Function.ToString(drDoc["Code"]) == Function.ToString(row["Code"]) && Function.ToString(drDoc["RegionCode"]) == Function.ToString(row["RegionCode"]) && Function.ToString(drDoc["GgpCode"]) == Function.ToString(row["GgpCode"])) == null)
				{
					_dtDetail.ImportRow(row);
				}
			}
			_dtDetail.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task LoadCfl(Enums.TypeCFL type, string countryCode, string ggpCode, bool isDetail)
	{
		try
		{
			_lstParams = new List<Common.ParameterValue>();
			switch (type)
			{
			case Enums.TypeCFL.Geography:
				_lstParams.Add(new Common.ParameterValue("CountryCode", countryCode));
				break;
			case Enums.TypeCFL.GeographyDetails:
				_lstParams.Add(new Common.ParameterValue("CountryCode", countryCode));
				if (isDetail)
				{
					_lstParams.Add(new Common.ParameterValue("FatherCode", Function.ToString(tv_Structure.FocusedNode.GetValue("GgpCode"))));
				}
				else
				{
					_lstParams.Add(new Common.ParameterValue("FatherCode", (tv_Structure.FocusedNode.ParentNode != null) ? Function.ToString(tv_Structure.FocusedNode.ParentNode.GetValue("GgpCode")) : ""));
				}
				break;
			default:
			{
				EnumerableRowCollection<DataRow> enumerableRowCollection = from r in tv_Structure.DataSourceTable.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(r["GgpCode"]))
					select r;
				EnumerableRowCollection<DataRow> enumerableRowCollection2 = from r in _dtDetail.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(r["GgpCode"]))
					select r;
				string text = string.Empty;
				foreach (DataRow item in enumerableRowCollection)
				{
					text = text + Function.ToString(item["GgpCode"]) + ",";
				}
				foreach (DataRow item2 in enumerableRowCollection2)
				{
					text = text + Function.ToString(item2["GgpCode"]) + ",";
				}
				text = text.Remove(text.Length - 1, 1);
				_lstParams.Add(new Common.ParameterValue("GgpCode", text));
				break;
			}
			}
			using frm_CFL frm_CFL2 = new frm_CFL(type, false, "", "", Enums.ItemObjectType.NoItemType, _lstParams, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem == null)
			{
				return;
			}
			switch (type)
			{
			case Enums.TypeCFL.Geography:
				gv_SaleTerritory.SetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "CountryCode", Function.ToString(frm_CFL2.ChooseItem["Code"]));
				break;
			case Enums.TypeCFL.GeographyDetails:
				if (!isDetail)
				{
					tv_Structure.SetRowCellValue(tv_Structure.FocusedNode, tlc_GgpCode, Function.ToString(frm_CFL2.ChooseItem["Code"]));
					break;
				}
				gv_Detail.SetRowCellValue(gv_Detail.FocusedRowHandle, "GgpCode", Function.ToString(frm_CFL2.ChooseItem["Code"]));
				gv_Detail.SetRowCellValue(gv_Detail.FocusedRowHandle, "GgpName", Function.ToString(frm_CFL2.ChooseItem["Name"]));
				break;
			case Enums.TypeCFL.Store:
				gv_Stores.SetRowCellValue(gv_Stores.FocusedRowHandle, "StrCode", Function.ToString(frm_CFL2.ChooseItem["Code"]));
				gv_Stores.SetRowCellValue(gv_Stores.FocusedRowHandle, "StrName", Function.ToString(frm_CFL2.ChooseItem["Name"]));
				gv_Stores.SetRowCellValue(gv_Stores.FocusedRowHandle, "CardCode", Function.ToString(frm_CFL2.ChooseItem["CardCode"]));
				gv_Stores.SetRowCellValue(gv_Stores.FocusedRowHandle, "Region", Function.ToString(frm_CFL2.ChooseItem["Region"]));
				gv_Stores.SetRowCellValue(gv_Stores.FocusedRowHandle, "City", Function.ToString(frm_CFL2.ChooseItem["City"]));
				gv_Stores.SetRowCellValue(gv_Stores.FocusedRowHandle, "District", Function.ToString(frm_CFL2.ChooseItem["District"]));
				gv_Stores.SetRowCellValue(gv_Stores.FocusedRowHandle, "Ward", Function.ToString(frm_CFL2.ChooseItem["Ward"]));
				gv_Stores.SetRowCellValue(gv_Stores.FocusedRowHandle, "Town", Function.ToString(frm_CFL2.ChooseItem["Town"]));
				gv_Stores.SetRowCellValue(gv_Stores.FocusedRowHandle, "Status", Function.ToString(frm_CFL2.ChooseItem["Status"]));
				gv_Stores.SetRowCellValue(gv_Stores.FocusedRowHandle, "Address", Function.ToString(frm_CFL2.ChooseItem["Address"]));
				gv_Stores.SetRowCellValue(gv_Stores.FocusedRowHandle, "Remarks", Function.ToString(frm_CFL2.ChooseItem["Remarks"]));
				break;
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

	public bool CheckGeography(string code, int isCountry)
	{
		try
		{
			DataTable dataTable = _httpClient.SaleTerritoryClient.CheckExistsGeography(code, isCountry);
			if (dataTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.None, Function.ToString(dataTable.Rows[0]["ErrorMessage"]));
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private async Task frm_SaleTerritory_Load(object sender, EventArgs e)
	{
		await LoadInitForm();
		await LoadData();
		FormMode = Enums.FormViewModeType.View;
	}

	private void gv_SaleTerritory_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = Function.ToString(e.RowHandle + 1);
		}
	}

	private void gv_Detail_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = Function.ToString(e.RowHandle + 1);
		}
	}

	private void gv_Stores_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = Function.ToString(e.RowHandle + 1);
		}
	}

	private async void addNewToolStripMenuItem2_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				WinCommons._OpenProcessing();
				await AddNewDoc();
				await AddNewStructure("R", isNew: true);
				if (grd_Detail.DataSourceTable.IsNotNull())
				{
					_dtDetail = _dtDetail.Clone();
					grd_Detail.DataSource = _httpClient.SaleTerritoryClient.GetStructureDetails("", "", "");
					grd_Detail.RefreshDataSource();
				}
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private async void tv_Structure_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		try
		{
			if (tv_Structure.FocusedNode != null)
			{
				string territoryCode = gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "Code").ToString();
				string regionCode = Function.ToString(tv_Structure.FocusedNode.GetValue("RegionCode"));
				string ggpCode = Function.ToString(tv_Structure.GetRowCellValue(tv_Structure.FocusedNode, tlc_GgpCode));
				await LoadDataLines(territoryCode, regionCode, ggpCode);
				if (FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update)
				{
					FormMode = Enums.FormViewModeType.View;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			await AddNewStructure("R", isNew: false);
			await AddNewDetails(Function.ToString(gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "Code")), "", Function.ToString(tv_Structure.GetRowCellValue(tv_Structure.FocusedNode, tlc_GgpCode)), isNew: true);
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

	private async void addSameToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (tv_Structure.DataSourceTable.IsNotNull())
		{
			await AddNewStructure("S", isNew: false);
		}
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private async void insertToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (tv_Structure.DataSourceTable.IsNotNull())
		{
			await AddNewStructure("I", isNew: false);
		}
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void frm_SaleTerritory_FormClosing(object sender, FormClosingEventArgs e)
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
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
			DataSet dataSet = new DataSet();
			grd_SaleTerritory.DataSourceTable.AcceptChanges();
			tv_Structure.DataSourceTable.AcceptChanges();
			grd_Detail.DataSourceTable.AcceptChanges();
			grd_Stores.DataSourceTable.AcceptChanges();
			if (dataSet.Tables.Count > 0)
			{
				dataSet.Tables.Clear();
			}
			if (!_dtDetail.IsNotNull())
			{
				_dtDetail = grd_Detail.DataSourceTable;
				_dtDetail.AcceptChanges();
			}
			DataRow dataRow = _dtDetail.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrEmpty(Function.ToString(r["GgpCode"])) && string.IsNullOrEmpty(Function.ToString(r["GgpName"])));
			if (dataRow != null)
			{
				_dtDetail.Rows.Remove(dataRow);
				_dtDetail.AcceptChanges();
			}
			dataSet.Tables.Add(gv_SaleTerritory.GetListDataSourceSelectedRow().CopyToDataTable());
			dataSet.Tables.Add(tv_Structure.DataSourceTable.Copy());
			dataSet.Tables.Add(_dtDetail.Copy());
			dataSet.Tables.Add(grd_Stores.DataSourceTable.Copy());
			dataSet.AcceptChanges();
			await SaveData(dataSet, (FormMode == Enums.FormViewModeType.Add) ? "A" : "U");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void Btn_AddGgpDetails_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			await AddNewStructure("I", isNew: false);
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

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			DataTable dataSourceTable = tv_Structure.DataSourceTable;
			if (!dataSourceTable.IsNotNull())
			{
				return;
			}
			TreeListMultiSelection selection = tv_Structure.Selection;
			if (selection != null)
			{
				foreach (TreeListNode item in selection.ToList())
				{
					if (item.Nodes != null)
					{
						foreach (TreeListNode item2 in item.Nodes.ToList())
						{
							tv_Structure.DeleteNode(item2);
						}
					}
					tv_Structure.DeleteNode(item);
				}
				dataSourceTable.AcceptChanges();
				tv_Structure.DataSource = dataSourceTable;
				tv_Structure.RefreshDataSource();
				if (!tv_Structure.DataSourceTable.IsNotNull())
				{
					await AddNewStructure("R", isNew: true);
					await AddNewDetails(Function.ToString(gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "Code")), "", "", isNew: true);
				}
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

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void addNewToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			string territoryCode = Function.ToString(gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "Code"));
			if (string.IsNullOrEmpty(GetListGeographyCode(Function.ToString(tv_Structure.GetRowCellValue(tv_Structure.FocusedNode, tlc_GgpCode)))))
			{
				WinCommons.Error(Enums.MessageCode.GeographyCodeCannotBeEmpty, "");
				return;
			}
			AddNewStore(territoryCode);
			if (grd_Stores.DataSourceTable.IsNotNull())
			{
				menu_RemoveStore.Enabled = true;
			}
			menu_DuplicateStore.Enabled = true;
			_isAddStore = true;
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

	private void gv_SaleTerritory_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (string.Equals(e.Column.Name, gc_TerritoryCode.Name))
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

	private void gv_SaleTerritory_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add && string.Equals(gv_SaleTerritory.FocusedColumn.Name, gc_TerritoryCode.Name))
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

	private async void gv_SaleTerritory_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update)
			{
				string territoryCode = Function.ToString(gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "Code"));
				await LoadTreeView(territoryCode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void Rep_Region_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		_ = 1;
		try
		{
			string territoryCode = Function.ToString(gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "Code"));
			string text = Function.ToString(gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "CountryCode"));
			if (e.Button.Index == 0)
			{
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.CountryCannotBeEmpty, "");
					return;
				}
				await LoadCfl(Enums.TypeCFL.GeographyDetails, text, "", isDetail: false);
				await LoadDataLines(territoryCode, Function.ToString(tv_Structure.GetRowCellValue(tv_Structure.FocusedNode, tlc_RegionCode)), Function.ToString(tv_Structure.GetRowCellValue(tv_Structure.FocusedNode, tlc_GgpCode)));
				if (FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
			else if (CheckGeography(Function.ToString(tv_Structure.FocusedNode.GetValue("GgpCode")), 0))
			{
				LoadGeography(text, Function.ToString(tv_Structure.FocusedNode.GetValue("GgpCode")));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Rep_CountryCFL_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				gv_SaleTerritory.PostEditor();
				LoadGeography(Function.ToString(gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "CountryCode")), "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_SaleTerritory_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == gc_Country.FieldName && !string.IsNullOrEmpty(Function.ToString(gv_SaleTerritory.GetRowCellValue(e.RowHandle, "CountryCode"))) && !CheckGeography(Function.ToString(gv_SaleTerritory.GetRowCellValue(e.RowHandle, "CountryCode")), 1))
			{
				return;
			}
			if (e.Column.FieldName == gc_TerritoryCode.FieldName && !string.IsNullOrEmpty(Function.ToString(gv_SaleTerritory.GetRowCellValue(e.RowHandle, "Code"))))
			{
				DataTable dataSourceTable = tv_Structure.DataSourceTable;
				if (dataSourceTable.IsNotNull())
				{
					foreach (DataRow item in from r in dataSourceTable.AsEnumerable()
						where string.IsNullOrEmpty(Function.ToString(r["Code"]))
						select r)
					{
						item["Code"] = Function.ToString(gv_SaleTerritory.GetRowCellValue(e.RowHandle, "Code"));
					}
					dataSourceTable.AcceptChanges();
				}
			}
			if (e.Column.FieldName == gc_Status.FieldName)
			{
				DataTable dataSourceTable2 = tv_Structure.DataSourceTable;
				if (dataSourceTable2.IsNotNull())
				{
					foreach (DataRow row in dataSourceTable2.Rows)
					{
						row["Status"] = Function.ToString(gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "Status"));
					}
					dataSourceTable2.AcceptChanges();
					tv_Structure.DataSource = dataSourceTable2;
					tv_Structure.RefreshDataSource();
				}
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

	private void tv_Structure_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		try
		{
			string text = Function.ToString(tv_Structure.GetRowCellValue(e.Node, tlc_GgpCode));
			if (e.Column.FieldName == tlc_GgpCode.FieldName && !string.IsNullOrEmpty(text) && !CheckGeography(text, 0))
			{
				return;
			}
			if (e.Column.Name == tlc_Status.Name && e.Node.Nodes != null)
			{
				tv_Structure.BeginUpdate();
				foreach (TreeListNode item in e.Node.Nodes.ToList())
				{
					item.SetValue("Status", Function.ToString(e.Node["Status"]));
				}
				tv_Structure.EndUpdate();
			}
			if (e.Column.Name == tlc_RegionCode.Name)
			{
				DataTable dataSourceTable = grd_Detail.DataSourceTable;
				if (!dataSourceTable.IsNotNull())
				{
					return;
				}
				foreach (DataRow row in dataSourceTable.Rows)
				{
					row["RegionCode"] = Function.ToString(tv_Structure.FocusedNode.GetValue("RegionCode"));
				}
				dataSourceTable.AcceptChanges();
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

	private async void gv_Detail_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == gc_DGgpCode.FieldName && !string.IsNullOrEmpty(Function.ToString(gv_Detail.GetRowCellValue(gv_Detail.FocusedRowHandle, "GgpCode"))) && !CheckGeography(Function.ToString(gv_Detail.GetRowCellValue(gv_Detail.FocusedRowHandle, "GgpCode")), 0))
			{
				return;
			}
			if (e.Column.Name == tlc_GgpCode.Name && !string.IsNullOrEmpty(Function.ToString(tv_Structure.GetRowCellValue(tv_Structure.FocusedNode, tlc_GgpCode))))
			{
				DataTable dataSourceTable = grd_Detail.DataSourceTable;
				if (!dataSourceTable.IsNotNull())
				{
					return;
				}
				foreach (DataRow row in dataSourceTable.Rows)
				{
					row["GgpCode1"] = Function.ToString(tv_Structure.FocusedNode.GetValue("GgpCode"));
				}
				dataSourceTable.AcceptChanges();
				grd_Detail.DataSource = dataSourceTable;
				grd_Detail.RefreshDataSource();
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

	private void gv_Stores_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void addNewToolStripMenuItem3_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			await AddNewDetails(Function.ToString(gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "Code")), Function.ToString(tv_Structure.GetRowCellValue(tv_Structure.FocusedNode, tlc_RegionCode)), Function.ToString(tv_Structure.GetRowCellValue(tv_Structure.FocusedNode, tlc_GgpCode)), isNew: false);
			if (grd_Detail.DataSourceTable.IsNotNull())
			{
				menu_RemoveDetail.Enabled = true;
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
		WinCommons._CloseProcessing();
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				FormMode = Enums.FormViewModeType.View;
			}
			WinCommons._OpenProcessing();
			await LoadData(txt_TerritoryCode.Text, txt_Location.Text, Function.ToString(lk_Status.EditValue));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private async void Rep_AddDetail_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				if (tv_Structure.FocusedNode != null)
				{
					if (string.IsNullOrEmpty(Function.ToString(tv_Structure.FocusedNode.GetValue("GgpCode"))))
					{
						WinCommons.Error(Enums.MessageCode.RegionOfStructureCannotBeEmpty, "");
						return;
					}
					_lstParams = new List<Common.ParameterValue>();
					_lstParams.Add(new Common.ParameterValue("CountryCode", Function.ToString(gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "CountryCode"))));
					_lstParams.Add(new Common.ParameterValue("FatherCode", Function.ToString(tv_Structure.FocusedNode.GetValue("GgpCode"))));
					await LoadCfl(Enums.TypeCFL.GeographyDetails, Function.ToString(gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "CountryCode")), "", isDetail: true);
				}
			}
			else if (string.IsNullOrEmpty(Function.ToString(gv_Detail.GetRowCellValue(gv_Detail.FocusedRowHandle, "GgpCode"))))
			{
				WinCommons.Error(Enums.MessageCode.DetailIsEmpty, "");
			}
			else
			{
				CheckGeography(Function.ToString(gv_Detail.GetRowCellValue(gv_Detail.FocusedRowHandle, "GgpCode")), 0);
				LoadGeography(Function.ToString(gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "CountryCode")), Function.ToString(gv_Detail.GetRowCellValue(gv_Detail.FocusedRowHandle, "GgpCode")));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void Rep_StoreCFL_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				if (!tv_Structure.DataSourceTable.IsNotNull() && !grd_Detail.DataSourceTable.IsNotNull())
				{
					WinCommons.Error(Enums.MessageCode.PleaseChooseRegionOfStructure, "");
					return;
				}
				string listGeographyCode = GetListGeographyCode(Function.ToString(tv_Structure.GetRowCellValue(tv_Structure.FocusedNode, tlc_GgpCode)));
				_lstParams = new List<Common.ParameterValue>();
				_lstParams.Add(new Common.ParameterValue("GgpCode", listGeographyCode));
				await LoadCfl(Enums.TypeCFL.Store, "", listGeographyCode, isDetail: false);
			}
			else if (!string.IsNullOrEmpty(Function.ToString(gv_Stores.GetRowCellValue(gv_Stores.FocusedRowHandle, "StrCode"))))
			{
				frm_Store frm_Store2 = new frm_Store(Function.ToString(gv_Stores.GetRowCellValue(gv_Stores.FocusedRowHandle, "StrCode")));
				frm_Store2.MdiParent = MdiParent;
				frm_Store2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void removeToolStripMenuItem2_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveItem, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				List<DataRow> listDataSourceSelectedRow = gv_SaleTerritory.GetListDataSourceSelectedRow();
				if (listDataSourceSelectedRow != null)
				{
					foreach (DataRow item in listDataSourceSelectedRow)
					{
						_ = item;
						Tuple<string> territoryCode = new Tuple<string>(Function.ToString(gv_SaleTerritory.GetRowCellValue(gv_SaleTerritory.FocusedRowHandle, "Code")));
						WinCommons._OpenProcessing();
						HttpResult httpResult = await _httpClient.SaleTerritoryClient.Remove(territoryCode);
						if (!string.IsNullOrEmpty(httpResult.Message))
						{
							WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
							continue;
						}
						WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
						await LoadData();
						FormMode = Enums.FormViewModeType.View;
					}
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

	private void menu_DuplicateTerritory_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			DataTable dataTable = grd_SaleTerritory.DataSourceTable.Copy().Clone();
			DataRow focusedDataRow = gv_SaleTerritory.GetFocusedDataRow();
			if (dataTable.Columns.Count == 0 || focusedDataRow == null)
			{
				return;
			}
			focusedDataRow["Code"] = DBNull.Value;
			focusedDataRow["Name"] = DBNull.Value;
			dataTable.Rows.Add(focusedDataRow.ItemArray);
			dataTable.AcceptChanges();
			_isDuplicate = true;
			grd_SaleTerritory.DataSource = dataTable;
			grd_SaleTerritory.RefreshDataSource();
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

	private void menu_DuplicateStore_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			DataTable dataTable = grd_Stores.DataSourceTable.Copy().Clone();
			DataRow focusedDataRow = gv_Stores.GetFocusedDataRow();
			if (dataTable.Columns.Count == 0 || focusedDataRow == null)
			{
				return;
			}
			_isDuplicate = true;
			_isAddStore = true;
			focusedDataRow["StrCode"] = string.Empty;
			focusedDataRow["StrName"] = string.Empty;
			dataTable.Rows.Add(focusedDataRow.ItemArray);
			dataTable.AcceptChanges();
			grd_Stores.DataSource = dataTable;
			grd_Stores.RefreshDataSource();
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

	private void menu_RemoveDetail_Click(object sender, EventArgs e)
	{
		try
		{
			if (_dtDetail.IsNotNull())
			{
				List<DataRow> list = new List<DataRow>();
				DataRow focusedDataRow = gv_Detail.GetFocusedDataRow();
				foreach (DataRow row in _dtDetail.Rows)
				{
					if (Function.ToString(row["GgpCode"]) == Function.ToString(focusedDataRow["GgpCode"]))
					{
						list.Add(row);
					}
				}
				if (list.Count > 0)
				{
					foreach (DataRow item in list)
					{
						_dtDetail.Rows.Remove(item);
					}
					_dtDetail.AcceptChanges();
				}
			}
			gv_Detail.DeleteDataSourceRowSelected();
			grd_SaleTerritory.RefreshDataSource();
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

	private void menu_RemoveStore_Click(object sender, EventArgs e)
	{
		try
		{
			gv_Stores.DeleteDataSourceRowSelected();
			grd_Stores.RefreshDataSource();
			if (!grd_Stores.DataSourceTable.IsNotNull())
			{
				menu_RemoveStore.Enabled = false;
				menu_DuplicateStore.Enabled = false;
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

	private void gv_Stores_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gv_Stores.FocusedColumn.Name == gc_RegionCode.Name || gv_Stores.FocusedColumn.Name == gc_City.Name || gv_Stores.FocusedColumn.Name == gc_District.Name || gv_Stores.FocusedColumn.Name == gc_Ward.Name || gv_Stores.FocusedColumn.Name == gc_Town.Name)
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

	private void gv_Stores_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (gv_Stores.FocusedColumn.Name == gc_RegionCode.Name)
			{
				e.Appearance.BackColor = Color.FromArgb(231, 231, 231);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Detail_RowUpdated(object sender, RowObjectEventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(gv_Detail.GetRowCellValue(e.RowHandle, "GgpCode"))) && !string.IsNullOrEmpty(Function.ToString(gv_Detail.GetRowCellValue(e.RowHandle, "GgpName"))))
			{
				await SaveTemporaryData();
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.DMS.frm_SaleTerritory));
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions10 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions11 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled18 = new DevExpress.Utils.SerializableAppearanceObject();
		this.rep_CountryCFL = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.glk_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.lbl_TerritoryCode = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Status = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Location = new Apzon.Controls.Controls.LabelLine();
		this.txt_Location = new Apzon.Controls.Editor.TextEditEx();
		this.lk_Status = new Apzon.Controls.Editor.LookUpEditEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_SaleTerritory = new Apzon.Controls.Grid.GridControlEx();
		this.cms_Territory = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menu_AddTerritory = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_RemoveTerritory = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_DuplicateTerritory = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_SaleTerritory = new Apzon.Controls.Grid.GridViewEx();
		this.gc_TerritoryCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Name = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Country = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CountriesCFL = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_Type = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Status = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CreateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CreateTime = new Apzon.Controls.Grid.GridColumnEx();
		this.tab_Main = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Structure = new Apzon.Controls.Controls.XtraTabPageEx();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grb_Structure = new DevExpress.XtraEditors.GroupControl();
		this.tv_Structure = new Apzon.Controls.Tree.TreeEx();
		this.tlc_RegionCode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_AddStructure = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tlc_Name = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_GgpCode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_Region = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.tlc_FCode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_Status = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlk_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.tlc_Remarks = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_Id = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_FatherId = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlc_Code = new Apzon.Controls.Tree.TreeListColumnEx();
		this.cms_Structure = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menu_AddStructure = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_AddSameStructure = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_InsertStructure = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_RemoveStructure = new System.Windows.Forms.ToolStripMenuItem();
		this.Rep_RegionCFL = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.grb_Detail = new DevExpress.XtraEditors.GroupControl();
		this.grd_Detail = new Apzon.Controls.Grid.GridControlEx();
		this.cms_Detail = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menu_AddDetail = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_RemoveDetail = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Detail = new Apzon.Controls.Grid.GridViewEx();
		this.gc_DTerritoryCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_DRegionCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_DGgpCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DetailCFL = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_DGgpName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_GgpRemarks = new Apzon.Controls.Grid.GridColumnEx();
		this.tab_Stores = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Stores = new Apzon.Controls.Grid.GridControlEx();
		this.cms_Store = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menu_AddStore = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_RemoveStore = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_DuplicateStore = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Stores = new Apzon.Controls.Grid.GridViewEx();
		this.gc_Code = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_StrCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_StoresCFL = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_StrStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.glk_StrStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_RegionCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_City = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_District = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Ward = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Town = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_StrCardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CardCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_StrAddress = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_StrName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_SRemarks = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_StoreCFL = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.rep_StoreRegion = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_TerritoryCode = new Apzon.Controls.Editor.TextEditEx();
		((System.ComponentModel.ISupportInitialize)this.rep_CountryCFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.glk_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Location.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lk_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_SaleTerritory).BeginInit();
		this.cms_Territory.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_SaleTerritory).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CountriesCFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tab_Main).BeginInit();
		this.tab_Main.SuspendLayout();
		this.tab_Structure.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grb_Structure).BeginInit();
		this.grb_Structure.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tv_Structure).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_AddStructure).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Region).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tlk_Status).BeginInit();
		this.cms_Structure.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.Rep_RegionCFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grb_Detail).BeginInit();
		this.grb_Detail.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Detail).BeginInit();
		this.cms_Detail.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Detail).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DetailCFL).BeginInit();
		this.tab_Stores.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Stores).BeginInit();
		this.cms_Store.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Stores).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_StoresCFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.glk_StrStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CardCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_StoreCFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_StoreRegion).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TerritoryCode.Properties).BeginInit();
		base.SuspendLayout();
		this.rep_CountryCFL.AutoHeight = false;
		this.rep_CountryCFL.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_CountryCFL.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_CountryCFL.ContainerControl = this;
		this.rep_CountryCFL.DisplayMember = "CountryCode";
		this.rep_CountryCFL.DisplayMember2 = "";
		this.rep_CountryCFL.LoadDataSourceWhenEnter = false;
		this.rep_CountryCFL.Name = "rep_CountryCFL";
		this.rep_CountryCFL.SeparateCharacterForMulti = ';';
		this.rep_CountryCFL.SeparateString = ", ";
		this.rep_CountryCFL.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Geography;
		this.rep_CountryCFL.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oGeography;
		this.rep_CountryCFL.UsingAutoCompleteBox = false;
		this.rep_CountryCFL.UsingDataSource = true;
		this.rep_CountryCFL.ValidateWhenNoDataSource = false;
		this.rep_CountryCFL.ValueMember = "CountryCode";
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
		this.lbl_TerritoryCode.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_TerritoryCode.Appearance.Options.UseFont = true;
		this.lbl_TerritoryCode.EditControl = null;
		this.lbl_TerritoryCode.EditValue = "Territory Code";
		this.lbl_TerritoryCode.FieldName = null;
		this.lbl_TerritoryCode.HideWhenEmpty = false;
		this.lbl_TerritoryCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_TerritoryCode.Location = new System.Drawing.Point(7, 8);
		this.lbl_TerritoryCode.Name = "lbl_TerritoryCode";
		this.lbl_TerritoryCode.ReadOnly = false;
		this.lbl_TerritoryCode.SettingInVisible = false;
		this.lbl_TerritoryCode.SettingReadonly = false;
		this.lbl_TerritoryCode.ShowUnderLine = true;
		this.lbl_TerritoryCode.Size = new System.Drawing.Size(100, 20);
		this.lbl_TerritoryCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_TerritoryCode.TabIndex = 0;
		this.lbl_TerritoryCode.Utext = "Territory Code";
		this.lbl_Status.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_Status.Appearance.Options.UseFont = true;
		this.lbl_Status.EditControl = null;
		this.lbl_Status.EditValue = "Status";
		this.lbl_Status.FieldName = null;
		this.lbl_Status.HideWhenEmpty = false;
		this.lbl_Status.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Status.Location = new System.Drawing.Point(7, 30);
		this.lbl_Status.Name = "lbl_Status";
		this.lbl_Status.ReadOnly = false;
		this.lbl_Status.SettingInVisible = false;
		this.lbl_Status.SettingReadonly = false;
		this.lbl_Status.ShowUnderLine = true;
		this.lbl_Status.Size = new System.Drawing.Size(100, 20);
		this.lbl_Status.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Status.TabIndex = 0;
		this.lbl_Status.Utext = "Status";
		this.lbl_Location.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_Location.Appearance.Options.UseFont = true;
		this.lbl_Location.EditControl = null;
		this.lbl_Location.EditValue = "Location";
		this.lbl_Location.FieldName = null;
		this.lbl_Location.HideWhenEmpty = false;
		this.lbl_Location.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Location.Location = new System.Drawing.Point(264, 8);
		this.lbl_Location.Name = "lbl_Location";
		this.lbl_Location.ReadOnly = false;
		this.lbl_Location.SettingInVisible = false;
		this.lbl_Location.SettingReadonly = false;
		this.lbl_Location.ShowUnderLine = true;
		this.lbl_Location.Size = new System.Drawing.Size(98, 17);
		this.lbl_Location.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Location.TabIndex = 0;
		this.lbl_Location.Utext = "Location";
		this.txt_Location.EditControl = null;
		this.txt_Location.FieldName = null;
		this.txt_Location.IsSearchControl = false;
		this.txt_Location.Location = new System.Drawing.Point(350, 5);
		this.txt_Location.Name = "txt_Location";
		this.txt_Location.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txt_Location.Properties.Appearance.Options.UseFont = true;
		this.txt_Location.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.txt_Location.Properties.AppearanceFocused.BackColor2 = System.Drawing.Color.FromArgb(250, 249, 215);
		this.txt_Location.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txt_Location.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_Location.Properties.AppearanceFocused.Options.UseFont = true;
		this.txt_Location.SettingInVisible = false;
		this.txt_Location.SettingReadonly = false;
		this.txt_Location.Size = new System.Drawing.Size(150, 20);
		this.txt_Location.TabIndex = 2;
		this.lk_Status.EditControl = null;
		this.lk_Status.FieldName = null;
		this.lk_Status.IsSearchControl = false;
		this.lk_Status.Location = new System.Drawing.Point(98, 27);
		this.lk_Status.Name = "lk_Status";
		this.lk_Status.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lk_Status.Properties.Appearance.Options.UseFont = true;
		this.lk_Status.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.lk_Status.Properties.AppearanceFocused.BackColor2 = System.Drawing.Color.FromArgb(250, 249, 215);
		this.lk_Status.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.lk_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lk_Status.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.lk_Status.Properties.DisplayMember = "Name";
		this.lk_Status.Properties.NullText = "";
		this.lk_Status.Properties.ShowHeader = false;
		this.lk_Status.Properties.ValueMember = "Code";
		this.lk_Status.SettingInVisible = false;
		this.lk_Status.SettingReadonly = false;
		this.lk_Status.Size = new System.Drawing.Size(150, 20);
		this.lk_Status.TabIndex = 3;
		this.btn_Search.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_Search.Appearance.Options.UseFont = true;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(264, 28);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 20);
		this.btn_Search.TabIndex = 4;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(7, 55);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_SaleTerritory);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.tab_Main);
		this.splitContainerControl1.Panel2.ShowCaption = true;
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.ShowCaption = true;
		this.splitContainerControl1.Size = new System.Drawing.Size(1076, 467);
		this.splitContainerControl1.SplitterPosition = 146;
		this.splitContainerControl1.TabIndex = 5;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_SaleTerritory.AllowPasteExcel = true;
		this.grd_SaleTerritory.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_SaleTerritory.ClipboardData = null;
		this.grd_SaleTerritory.ContextMenuStrip = this.cms_Territory;
		this.grd_SaleTerritory.Location = new System.Drawing.Point(0, 0);
		this.grd_SaleTerritory.MainView = this.gv_SaleTerritory;
		this.grd_SaleTerritory.Name = "grd_SaleTerritory";
		this.grd_SaleTerritory.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_Status, this.rep_CountriesCFL });
		this.grd_SaleTerritory.Size = new System.Drawing.Size(1206, 189);
		this.grd_SaleTerritory.TabIndex = 5;
		this.grd_SaleTerritory.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_SaleTerritory });
		this.cms_Territory.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.menu_AddTerritory, this.menu_RemoveTerritory, this.menu_DuplicateTerritory });
		this.cms_Territory.Name = "contextMenuStrip2";
		this.cms_Territory.Size = new System.Drawing.Size(125, 70);
		this.menu_AddTerritory.Name = "menu_AddTerritory";
		this.menu_AddTerritory.Size = new System.Drawing.Size(124, 22);
		this.menu_AddTerritory.Text = "Add New";
		this.menu_AddTerritory.Click += new System.EventHandler(this.addNewToolStripMenuItem2_Click);
		this.menu_RemoveTerritory.Name = "menu_RemoveTerritory";
		this.menu_RemoveTerritory.Size = new System.Drawing.Size(124, 22);
		this.menu_RemoveTerritory.Text = "Remove";
		this.menu_RemoveTerritory.Click += new System.EventHandler(this.removeToolStripMenuItem2_Click);
		this.menu_DuplicateTerritory.Name = "menu_DuplicateTerritory";
		this.menu_DuplicateTerritory.Size = new System.Drawing.Size(124, 22);
		this.menu_DuplicateTerritory.Text = "Duplicate";
		this.menu_DuplicateTerritory.Click += new System.EventHandler(this.menu_DuplicateTerritory_Click);
		this.gv_SaleTerritory.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_SaleTerritory.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_SaleTerritory.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_SaleTerritory.ColumnKey = null;
		this.gv_SaleTerritory.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gc_TerritoryCode, this.gc_Name, this.gc_Country, this.gc_Type, this.gc_Status, this.gc_Remarks, this.gc_UserSign, this.gc_CreateDate, this.gc_CreateTime });
		this.gv_SaleTerritory.DisableFormSetting = false;
		this.gv_SaleTerritory.DisplayException = false;
		this.gv_SaleTerritory.EditColumn = null;
		this.gv_SaleTerritory.FieldCurrency = null;
		this.gv_SaleTerritory.FieldRate = null;
		this.gv_SaleTerritory.FormId = "oSaleTerritory";
		this.gv_SaleTerritory.GridControl = this.grd_SaleTerritory;
		this.gv_SaleTerritory.IgnoreFilter = false;
		this.gv_SaleTerritory.IndexEditRow = -1;
		this.gv_SaleTerritory.IndicatorWidth = 35;
		this.gv_SaleTerritory.IsApzTable = false;
		this.gv_SaleTerritory.IsInsertNewRow = false;
		this.gv_SaleTerritory.IsLoadingData = false;
		this.gv_SaleTerritory.IsMappingContextAction = false;
		this.gv_SaleTerritory.ItemId = "APZ_D_OSTR";
		this.gv_SaleTerritory.LstColumnAllowBatch = null;
		this.gv_SaleTerritory.LstColumnNotAllowBatch = null;
		this.gv_SaleTerritory.LstColumnNotEditable = null;
		this.gv_SaleTerritory.LstColumNotSetting = null;
		this.gv_SaleTerritory.Name = "gv_SaleTerritory";
		this.gv_SaleTerritory.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_SaleTerritory.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_SaleTerritory.OptionsPrint.AutoWidth = false;
		this.gv_SaleTerritory.OptionsPrint.UsePrintStyles = false;
		this.gv_SaleTerritory.OptionsView.BestFitMaxRowCount = 100;
		this.gv_SaleTerritory.OptionsView.ColumnAutoWidth = false;
		this.gv_SaleTerritory.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_SaleTerritory.OptionsView.ShowGroupedColumns = true;
		this.gv_SaleTerritory.OptionsView.ShowGroupPanel = false;
		this.gv_SaleTerritory.UseBatchMod = false;
		this.gv_SaleTerritory.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_SaleTerritory_CustomDrawRowIndicator);
		this.gv_SaleTerritory.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_SaleTerritory_RowCellStyle);
		this.gv_SaleTerritory.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_SaleTerritory_ShowingEditor);
		this.gv_SaleTerritory.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_SaleTerritory_FocusedRowChanged);
		this.gv_SaleTerritory.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_SaleTerritory_CellValueChanged);
		this.gc_TerritoryCode.Caption = "Territory Code";
		this.gc_TerritoryCode.CurrencyDateField = null;
		this.gc_TerritoryCode.CurrencyField = null;
		this.gc_TerritoryCode.CurrencyFixed = null;
		this.gc_TerritoryCode.CurrencyFixedRate = 0.0;
		this.gc_TerritoryCode.CurrencyRateField = null;
		this.gc_TerritoryCode.FieldName = "Code";
		this.gc_TerritoryCode.Name = "gc_TerritoryCode";
		this.gc_TerritoryCode.PrimaryKey = false;
		this.gc_TerritoryCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_TerritoryCode.UnitField = null;
		this.gc_TerritoryCode.Visible = true;
		this.gc_TerritoryCode.VisibleIndex = 0;
		this.gc_TerritoryCode.Width = 80;
		this.gc_Name.Caption = "Name";
		this.gc_Name.CurrencyDateField = null;
		this.gc_Name.CurrencyField = null;
		this.gc_Name.CurrencyFixed = null;
		this.gc_Name.CurrencyFixedRate = 0.0;
		this.gc_Name.CurrencyRateField = null;
		this.gc_Name.FieldName = "Name";
		this.gc_Name.Name = "gc_Name";
		this.gc_Name.PrimaryKey = false;
		this.gc_Name.UnitField = null;
		this.gc_Name.Visible = true;
		this.gc_Name.VisibleIndex = 1;
		this.gc_Country.Caption = "Country";
		this.gc_Country.ColumnEdit = this.rep_CountriesCFL;
		this.gc_Country.CurrencyDateField = null;
		this.gc_Country.CurrencyField = null;
		this.gc_Country.CurrencyFixed = null;
		this.gc_Country.CurrencyFixedRate = 0.0;
		this.gc_Country.CurrencyRateField = null;
		this.gc_Country.FieldName = "CountryCode";
		this.gc_Country.Name = "gc_Country";
		this.gc_Country.PrimaryKey = false;
		this.gc_Country.UnitField = null;
		this.gc_Country.Visible = true;
		this.gc_Country.VisibleIndex = 2;
		this.rep_CountriesCFL.AutoHeight = false;
		this.rep_CountriesCFL.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.rep_CountriesCFL.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_CountriesCFL.ContainerControl = this;
		this.rep_CountriesCFL.DisplayMember = "CountryCode";
		this.rep_CountriesCFL.DisplayMember2 = "";
		this.rep_CountriesCFL.LoadDataSourceWhenEnter = false;
		this.rep_CountriesCFL.Name = "rep_CountriesCFL";
		this.rep_CountriesCFL.SeparateCharacterForMulti = ';';
		this.rep_CountriesCFL.SeparateString = ", ";
		this.rep_CountriesCFL.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Geography;
		this.rep_CountriesCFL.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oGeography;
		this.rep_CountriesCFL.UsingAutoCompleteBox = false;
		this.rep_CountriesCFL.UsingDataSource = true;
		this.rep_CountriesCFL.ValidateWhenNoDataSource = false;
		this.rep_CountriesCFL.ValueMember = "CountryCode";
		this.gc_Type.Caption = "Type";
		this.gc_Type.CurrencyDateField = null;
		this.gc_Type.CurrencyField = null;
		this.gc_Type.CurrencyFixed = null;
		this.gc_Type.CurrencyFixedRate = 0.0;
		this.gc_Type.CurrencyRateField = null;
		this.gc_Type.FieldName = "Type";
		this.gc_Type.Name = "gc_Type";
		this.gc_Type.PrimaryKey = false;
		this.gc_Type.UnitField = null;
		this.gc_Type.Visible = true;
		this.gc_Type.VisibleIndex = 3;
		this.gc_Status.Caption = "Status";
		this.gc_Status.ColumnEdit = this.rep_Status;
		this.gc_Status.CurrencyDateField = null;
		this.gc_Status.CurrencyField = null;
		this.gc_Status.CurrencyFixed = null;
		this.gc_Status.CurrencyFixedRate = 0.0;
		this.gc_Status.CurrencyRateField = null;
		this.gc_Status.FieldName = "Status";
		this.gc_Status.Name = "gc_Status";
		this.gc_Status.PrimaryKey = false;
		this.gc_Status.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_Status.UnitField = null;
		this.gc_Status.Visible = true;
		this.gc_Status.VisibleIndex = 4;
		this.rep_Status.AutoHeight = false;
		this.rep_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_Status.DisplayMember = "Name";
		this.rep_Status.Name = "rep_Status";
		this.rep_Status.NullText = "";
		this.rep_Status.ShowHeader = false;
		this.rep_Status.ValueMember = "Code";
		this.gc_Remarks.Caption = "Remarks";
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
		this.gc_Remarks.VisibleIndex = 5;
		this.gc_UserSign.Caption = "User Sign";
		this.gc_UserSign.CurrencyDateField = null;
		this.gc_UserSign.CurrencyField = null;
		this.gc_UserSign.CurrencyFixed = null;
		this.gc_UserSign.CurrencyFixedRate = 0.0;
		this.gc_UserSign.CurrencyRateField = null;
		this.gc_UserSign.FieldName = "UserSign";
		this.gc_UserSign.Name = "gc_UserSign";
		this.gc_UserSign.PrimaryKey = false;
		this.gc_UserSign.UnitField = null;
		this.gc_CreateDate.Caption = "Create Date";
		this.gc_CreateDate.CurrencyDateField = null;
		this.gc_CreateDate.CurrencyField = null;
		this.gc_CreateDate.CurrencyFixed = null;
		this.gc_CreateDate.CurrencyFixedRate = 0.0;
		this.gc_CreateDate.CurrencyRateField = null;
		this.gc_CreateDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gc_CreateDate.FieldName = "CreateDate";
		this.gc_CreateDate.Name = "gc_CreateDate";
		this.gc_CreateDate.PrimaryKey = false;
		this.gc_CreateDate.UnitField = null;
		this.gc_CreateTime.Caption = "Create Time";
		this.gc_CreateTime.CurrencyDateField = null;
		this.gc_CreateTime.CurrencyField = null;
		this.gc_CreateTime.CurrencyFixed = null;
		this.gc_CreateTime.CurrencyFixedRate = 0.0;
		this.gc_CreateTime.CurrencyRateField = null;
		this.gc_CreateTime.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Time;
		this.gc_CreateTime.FieldName = "CreateTime";
		this.gc_CreateTime.Name = "gc_CreateTime";
		this.gc_CreateTime.PrimaryKey = false;
		this.gc_CreateTime.UnitField = null;
		this.tab_Main.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tab_Main.IsSearchControl = false;
		this.tab_Main.Location = new System.Drawing.Point(0, 0);
		this.tab_Main.Name = "tab_Main";
		this.tab_Main.SelectedTabPage = this.tab_Structure;
		this.tab_Main.Size = new System.Drawing.Size(1076, 311);
		this.tab_Main.TabIndex = 6;
		this.tab_Main.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.tab_Structure, this.tab_Stores });
		this.tab_Structure.Appearance.Header.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.tab_Structure.Appearance.Header.Options.UseFont = true;
		this.tab_Structure.Controls.Add(this.splitContainerControl2);
		this.tab_Structure.EditControl = null;
		this.tab_Structure.EditValue = "Structure";
		this.tab_Structure.FieldName = "";
		this.tab_Structure.IsSearchControl = false;
		this.tab_Structure.Name = "tab_Structure";
		this.tab_Structure.ReadOnly = false;
		this.tab_Structure.SettingInVisible = false;
		this.tab_Structure.SettingReadonly = false;
		this.tab_Structure.Size = new System.Drawing.Size(1074, 286);
		this.tab_Structure.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Structure.Text = "Structure";
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.grb_Structure);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.grb_Detail);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(1074, 286);
		this.splitContainerControl2.SplitterPosition = 539;
		this.splitContainerControl2.TabIndex = 0;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.grb_Structure.Controls.Add(this.tv_Structure);
		this.grb_Structure.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grb_Structure.Location = new System.Drawing.Point(0, 0);
		this.grb_Structure.Name = "grb_Structure";
		this.grb_Structure.Size = new System.Drawing.Size(539, 286);
		this.grb_Structure.TabIndex = 0;
		this.grb_Structure.Text = "Structure";
		this.tv_Structure.AllowPasteExcel = true;
		this.tv_Structure.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tv_Structure.ColumnKey = null;
		this.tv_Structure.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[9] { this.tlc_RegionCode, this.tlc_Name, this.tlc_GgpCode, this.tlc_FCode, this.tlc_Status, this.tlc_Remarks, this.tlc_Id, this.tlc_FatherId, this.tlc_Code });
		this.tv_Structure.ContextMenuStrip = this.cms_Structure;
		this.tv_Structure.Cursor = System.Windows.Forms.Cursors.Default;
		this.tv_Structure.DisableFormSetting = false;
		this.tv_Structure.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tv_Structure.EditColumn = null;
		this.tv_Structure.FieldCurrency = null;
		this.tv_Structure.FieldRate = null;
		this.tv_Structure.FormId = "oSaleTerritory";
		this.tv_Structure.IndexEditRow = -1;
		this.tv_Structure.IndicatorWidth = 40;
		this.tv_Structure.IsApzTable = true;
		this.tv_Structure.IsLoadingData = true;
		this.tv_Structure.ItemId = "APZ_D_STR1";
		this.tv_Structure.KeyFieldName = "Id";
		this.tv_Structure.Location = new System.Drawing.Point(2, 23);
		this.tv_Structure.LstColumnNotEditable = null;
		this.tv_Structure.LstColumNotSetting = null;
		this.tv_Structure.Name = "tv_Structure";
		this.tv_Structure.OptionsNavigation.AutoFocusNewNode = true;
		this.tv_Structure.OptionsNavigation.AutoMoveRowFocus = true;
		this.tv_Structure.OptionsSelection.MultiSelect = true;
		this.tv_Structure.ParentFieldName = "FatherId";
		this.tv_Structure.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.tlk_Status, this.btn_AddStructure, this.Rep_RegionCFL, this.repositoryItemButtonEdit1, this.rep_Region });
		this.tv_Structure.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tv_Structure.Size = new System.Drawing.Size(535, 261);
		this.tv_Structure.TabIndex = 7;
		this.tv_Structure.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_Structure_CellValueChanged);
		this.tv_Structure.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tv_Structure_FocusedNodeChanged);
		this.tlc_RegionCode.Caption = "Code";
		this.tlc_RegionCode.ColumnEdit = this.btn_AddStructure;
		this.tlc_RegionCode.CurrencyDateField = null;
		this.tlc_RegionCode.CurrencyField = null;
		this.tlc_RegionCode.CurrencyFixed = null;
		this.tlc_RegionCode.CurrencyFixedRate = 0.0;
		this.tlc_RegionCode.CurrencyRateField = null;
		this.tlc_RegionCode.FieldName = "RegionCode";
		this.tlc_RegionCode.Name = "tlc_RegionCode";
		this.tlc_RegionCode.Visible = true;
		this.tlc_RegionCode.VisibleIndex = 0;
		this.btn_AddStructure.AutoHeight = false;
		this.btn_AddStructure.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
		});
		this.btn_AddStructure.Name = "btn_AddStructure";
		this.tlc_Name.Caption = "Name";
		this.tlc_Name.CurrencyDateField = null;
		this.tlc_Name.CurrencyField = null;
		this.tlc_Name.CurrencyFixed = null;
		this.tlc_Name.CurrencyFixedRate = 0.0;
		this.tlc_Name.CurrencyRateField = null;
		this.tlc_Name.FieldName = "RegionName";
		this.tlc_Name.Name = "tlc_Name";
		this.tlc_Name.Visible = true;
		this.tlc_Name.VisibleIndex = 1;
		this.tlc_GgpCode.Caption = "Region";
		this.tlc_GgpCode.ColumnEdit = this.rep_Region;
		this.tlc_GgpCode.CurrencyDateField = null;
		this.tlc_GgpCode.CurrencyField = null;
		this.tlc_GgpCode.CurrencyFixed = null;
		this.tlc_GgpCode.CurrencyFixedRate = 0.0;
		this.tlc_GgpCode.CurrencyRateField = null;
		this.tlc_GgpCode.FieldName = "GgpCode";
		this.tlc_GgpCode.Name = "tlc_GgpCode";
		this.tlc_GgpCode.Visible = true;
		this.tlc_GgpCode.VisibleIndex = 2;
		this.rep_Region.AutoHeight = false;
		this.rep_Region.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.rep_Region.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Region.ContainerControl = this;
		this.rep_Region.DisplayMember = "GgpCode";
		this.rep_Region.DisplayMember2 = "";
		this.rep_Region.LoadDataSourceWhenEnter = false;
		this.rep_Region.Name = "rep_Region";
		this.rep_Region.SeparateCharacterForMulti = ';';
		this.rep_Region.SeparateString = ", ";
		this.rep_Region.UsingAutoCompleteBox = false;
		this.rep_Region.UsingDataSource = true;
		this.rep_Region.ValidateWhenNoDataSource = false;
		this.rep_Region.ValueMember = "";
		this.tlc_FCode.Caption = "FCode";
		this.tlc_FCode.CurrencyDateField = null;
		this.tlc_FCode.CurrencyField = null;
		this.tlc_FCode.CurrencyFixed = null;
		this.tlc_FCode.CurrencyFixedRate = 0.0;
		this.tlc_FCode.CurrencyRateField = null;
		this.tlc_FCode.FieldName = "FatherCode";
		this.tlc_FCode.Name = "tlc_FCode";
		this.tlc_Status.Caption = "Status";
		this.tlc_Status.ColumnEdit = this.tlk_Status;
		this.tlc_Status.CurrencyDateField = null;
		this.tlc_Status.CurrencyField = null;
		this.tlc_Status.CurrencyFixed = null;
		this.tlc_Status.CurrencyFixedRate = 0.0;
		this.tlc_Status.CurrencyRateField = null;
		this.tlc_Status.FieldName = "Status";
		this.tlc_Status.Name = "tlc_Status";
		this.tlc_Status.Visible = true;
		this.tlc_Status.VisibleIndex = 3;
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
		this.tlk_Status.NullText = "";
		this.tlk_Status.ShowHeader = false;
		this.tlk_Status.ValueMember = "Code";
		this.tlc_Remarks.Caption = "Remarks";
		this.tlc_Remarks.CurrencyDateField = null;
		this.tlc_Remarks.CurrencyField = null;
		this.tlc_Remarks.CurrencyFixed = null;
		this.tlc_Remarks.CurrencyFixedRate = 0.0;
		this.tlc_Remarks.CurrencyRateField = null;
		this.tlc_Remarks.FieldName = "Remarks";
		this.tlc_Remarks.Name = "tlc_Remarks";
		this.tlc_Remarks.Visible = true;
		this.tlc_Remarks.VisibleIndex = 4;
		this.tlc_Id.Caption = "Id";
		this.tlc_Id.CurrencyDateField = null;
		this.tlc_Id.CurrencyField = null;
		this.tlc_Id.CurrencyFixed = null;
		this.tlc_Id.CurrencyFixedRate = 0.0;
		this.tlc_Id.CurrencyRateField = null;
		this.tlc_Id.FieldName = "Id";
		this.tlc_Id.Name = "tlc_Id";
		this.tlc_FatherId.Caption = "Father Id";
		this.tlc_FatherId.CurrencyDateField = null;
		this.tlc_FatherId.CurrencyField = null;
		this.tlc_FatherId.CurrencyFixed = null;
		this.tlc_FatherId.CurrencyFixedRate = 0.0;
		this.tlc_FatherId.CurrencyRateField = null;
		this.tlc_FatherId.FieldName = "FatherId";
		this.tlc_FatherId.Name = "tlc_FatherId";
		this.tlc_Code.Caption = "Territory Code";
		this.tlc_Code.CurrencyDateField = null;
		this.tlc_Code.CurrencyField = null;
		this.tlc_Code.CurrencyFixed = null;
		this.tlc_Code.CurrencyFixedRate = 0.0;
		this.tlc_Code.CurrencyRateField = null;
		this.tlc_Code.FieldName = "Code";
		this.tlc_Code.Name = "tlc_Code";
		this.cms_Structure.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.menu_AddStructure, this.menu_AddSameStructure, this.menu_InsertStructure, this.menu_RemoveStructure });
		this.cms_Structure.Name = "contextMenuStrip1";
		this.cms_Structure.Size = new System.Drawing.Size(129, 92);
		this.menu_AddStructure.Name = "menu_AddStructure";
		this.menu_AddStructure.Size = new System.Drawing.Size(128, 22);
		this.menu_AddStructure.Text = "Add New";
		this.menu_AddStructure.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.menu_AddSameStructure.Name = "menu_AddSameStructure";
		this.menu_AddSameStructure.Size = new System.Drawing.Size(128, 22);
		this.menu_AddSameStructure.Text = "Add Same";
		this.menu_AddSameStructure.Click += new System.EventHandler(this.addSameToolStripMenuItem_Click);
		this.menu_InsertStructure.Name = "menu_InsertStructure";
		this.menu_InsertStructure.Size = new System.Drawing.Size(128, 22);
		this.menu_InsertStructure.Text = "Insert";
		this.menu_InsertStructure.Click += new System.EventHandler(this.insertToolStripMenuItem_Click);
		this.menu_RemoveStructure.Name = "menu_RemoveStructure";
		this.menu_RemoveStructure.Size = new System.Drawing.Size(128, 22);
		this.menu_RemoveStructure.Text = "Remove";
		this.menu_RemoveStructure.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.Rep_RegionCFL.AutoHeight = false;
		this.Rep_RegionCFL.AutoLoadDataSource = true;
		this.Rep_RegionCFL.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, imageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.Rep_RegionCFL.ContainerControl = this;
		this.Rep_RegionCFL.DisplayMember = "GgpCode";
		this.Rep_RegionCFL.DisplayMember2 = "";
		this.Rep_RegionCFL.LoadDataSourceWhenEnter = false;
		this.Rep_RegionCFL.Name = "Rep_RegionCFL";
		this.Rep_RegionCFL.SeparateCharacterForMulti = ';';
		this.Rep_RegionCFL.SeparateString = ", ";
		this.Rep_RegionCFL.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.GeographyDetails;
		this.Rep_RegionCFL.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oSpecialPrices;
		this.Rep_RegionCFL.UsingAutoCompleteBox = false;
		this.Rep_RegionCFL.UsingDataSource = true;
		this.Rep_RegionCFL.ValidateWhenNoDataSource = false;
		this.Rep_RegionCFL.ValueMember = "GgpCode";
		this.repositoryItemButtonEdit1.AutoHeight = false;
		this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
		this.grb_Detail.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.grb_Detail.Appearance.Options.UseFont = true;
		this.grb_Detail.Controls.Add(this.grd_Detail);
		this.grb_Detail.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grb_Detail.Location = new System.Drawing.Point(0, 0);
		this.grb_Detail.Name = "grb_Detail";
		this.grb_Detail.Size = new System.Drawing.Size(525, 286);
		this.grb_Detail.TabIndex = 0;
		this.grb_Detail.Text = "Detail";
		this.grd_Detail.AllowPasteExcel = true;
		this.grd_Detail.ClipboardData = null;
		this.grd_Detail.ContextMenuStrip = this.cms_Detail;
		this.grd_Detail.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Detail.Location = new System.Drawing.Point(2, 23);
		this.grd_Detail.MainView = this.gv_Detail;
		this.grd_Detail.Name = "grd_Detail";
		this.grd_Detail.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_DetailCFL });
		this.grd_Detail.Size = new System.Drawing.Size(521, 261);
		this.grd_Detail.TabIndex = 8;
		this.grd_Detail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Detail });
		this.cms_Detail.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.menu_AddDetail, this.menu_RemoveDetail });
		this.cms_Detail.Name = "cms_Detail";
		this.cms_Detail.Size = new System.Drawing.Size(124, 48);
		this.menu_AddDetail.Name = "menu_AddDetail";
		this.menu_AddDetail.Size = new System.Drawing.Size(123, 22);
		this.menu_AddDetail.Text = "Add New";
		this.menu_AddDetail.Click += new System.EventHandler(this.addNewToolStripMenuItem3_Click);
		this.menu_RemoveDetail.Name = "menu_RemoveDetail";
		this.menu_RemoveDetail.Size = new System.Drawing.Size(123, 22);
		this.menu_RemoveDetail.Text = "Remove";
		this.menu_RemoveDetail.Click += new System.EventHandler(this.menu_RemoveDetail_Click);
		this.gv_Detail.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Detail.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Detail.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Detail.ColumnKey = null;
		this.gv_Detail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gc_DTerritoryCode, this.gc_DRegionCode, this.gc_DGgpCode, this.gc_DGgpName, this.gc_GgpRemarks });
		this.gv_Detail.DisableFormSetting = false;
		this.gv_Detail.DisplayException = false;
		this.gv_Detail.EditColumn = null;
		this.gv_Detail.FieldCurrency = null;
		this.gv_Detail.FieldRate = null;
		this.gv_Detail.FormId = "oSaleTerritory";
		this.gv_Detail.GridControl = this.grd_Detail;
		this.gv_Detail.IgnoreFilter = false;
		this.gv_Detail.IndexEditRow = -1;
		this.gv_Detail.IndicatorWidth = 35;
		this.gv_Detail.IsApzTable = true;
		this.gv_Detail.IsInsertNewRow = false;
		this.gv_Detail.IsLoadingData = false;
		this.gv_Detail.IsMappingContextAction = false;
		this.gv_Detail.ItemId = "APZ_D_STR2";
		this.gv_Detail.LstColumnAllowBatch = null;
		this.gv_Detail.LstColumnNotAllowBatch = null;
		this.gv_Detail.LstColumnNotEditable = null;
		this.gv_Detail.LstColumNotSetting = null;
		this.gv_Detail.Name = "gv_Detail";
		this.gv_Detail.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Detail.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Detail.OptionsPrint.AutoWidth = false;
		this.gv_Detail.OptionsPrint.UsePrintStyles = false;
		this.gv_Detail.OptionsSelection.MultiSelect = true;
		this.gv_Detail.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Detail.OptionsView.ColumnAutoWidth = false;
		this.gv_Detail.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Detail.OptionsView.ShowGroupedColumns = true;
		this.gv_Detail.OptionsView.ShowGroupPanel = false;
		this.gv_Detail.UseBatchMod = false;
		this.gv_Detail.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Detail_CustomDrawRowIndicator);
		this.gv_Detail.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Detail_CellValueChanged);
		this.gv_Detail.RowUpdated += new DevExpress.XtraGrid.Views.Base.RowObjectEventHandler(this.gv_Detail_RowUpdated);
		this.gc_DTerritoryCode.Caption = "Code";
		this.gc_DTerritoryCode.CurrencyDateField = null;
		this.gc_DTerritoryCode.CurrencyField = null;
		this.gc_DTerritoryCode.CurrencyFixed = null;
		this.gc_DTerritoryCode.CurrencyFixedRate = 0.0;
		this.gc_DTerritoryCode.CurrencyRateField = null;
		this.gc_DTerritoryCode.FieldName = "Code";
		this.gc_DTerritoryCode.Name = "gc_DTerritoryCode";
		this.gc_DTerritoryCode.PrimaryKey = true;
		this.gc_DTerritoryCode.UnitField = null;
		this.gc_DRegionCode.Caption = "Region Code";
		this.gc_DRegionCode.CurrencyDateField = null;
		this.gc_DRegionCode.CurrencyField = null;
		this.gc_DRegionCode.CurrencyFixed = null;
		this.gc_DRegionCode.CurrencyFixedRate = 0.0;
		this.gc_DRegionCode.CurrencyRateField = null;
		this.gc_DRegionCode.FieldName = "RegionCode";
		this.gc_DRegionCode.Name = "gc_DRegionCode";
		this.gc_DRegionCode.PrimaryKey = false;
		this.gc_DRegionCode.UnitField = null;
		this.gc_DGgpCode.Caption = "Code";
		this.gc_DGgpCode.ColumnEdit = this.rep_DetailCFL;
		this.gc_DGgpCode.CurrencyDateField = null;
		this.gc_DGgpCode.CurrencyField = null;
		this.gc_DGgpCode.CurrencyFixed = null;
		this.gc_DGgpCode.CurrencyFixedRate = 0.0;
		this.gc_DGgpCode.CurrencyRateField = null;
		this.gc_DGgpCode.FieldName = "GgpCode";
		this.gc_DGgpCode.Name = "gc_DGgpCode";
		this.gc_DGgpCode.PrimaryKey = false;
		this.gc_DGgpCode.UnitField = null;
		this.gc_DGgpCode.Visible = true;
		this.gc_DGgpCode.VisibleIndex = 0;
		this.rep_DetailCFL.AutoHeight = false;
		this.rep_DetailCFL.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions10.Image");
		this.rep_DetailCFL.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_DetailCFL.ContainerControl = this;
		this.rep_DetailCFL.DisplayMember = "";
		this.rep_DetailCFL.DisplayMember2 = "";
		this.rep_DetailCFL.LoadDataSourceWhenEnter = false;
		this.rep_DetailCFL.Name = "rep_DetailCFL";
		this.rep_DetailCFL.SeparateCharacterForMulti = ';';
		this.rep_DetailCFL.SeparateString = ", ";
		this.rep_DetailCFL.UsingAutoCompleteBox = false;
		this.rep_DetailCFL.UsingDataSource = true;
		this.rep_DetailCFL.ValidateWhenNoDataSource = false;
		this.rep_DetailCFL.ValueMember = "";
		this.gc_DGgpName.Caption = "Name";
		this.gc_DGgpName.CurrencyDateField = null;
		this.gc_DGgpName.CurrencyField = null;
		this.gc_DGgpName.CurrencyFixed = null;
		this.gc_DGgpName.CurrencyFixedRate = 0.0;
		this.gc_DGgpName.CurrencyRateField = null;
		this.gc_DGgpName.FieldName = "GgpName";
		this.gc_DGgpName.Name = "gc_DGgpName";
		this.gc_DGgpName.PrimaryKey = false;
		this.gc_DGgpName.UnitField = null;
		this.gc_DGgpName.Visible = true;
		this.gc_DGgpName.VisibleIndex = 1;
		this.gc_GgpRemarks.Caption = "Remarks";
		this.gc_GgpRemarks.CurrencyDateField = null;
		this.gc_GgpRemarks.CurrencyField = null;
		this.gc_GgpRemarks.CurrencyFixed = null;
		this.gc_GgpRemarks.CurrencyFixedRate = 0.0;
		this.gc_GgpRemarks.CurrencyRateField = null;
		this.gc_GgpRemarks.FieldName = "Remarks";
		this.gc_GgpRemarks.Name = "gc_GgpRemarks";
		this.gc_GgpRemarks.PrimaryKey = false;
		this.gc_GgpRemarks.UnitField = null;
		this.tab_Stores.Appearance.Header.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.tab_Stores.Appearance.Header.Options.UseFont = true;
		this.tab_Stores.Controls.Add(this.grd_Stores);
		this.tab_Stores.EditControl = null;
		this.tab_Stores.EditValue = "Stores";
		this.tab_Stores.FieldName = "";
		this.tab_Stores.IsSearchControl = false;
		this.tab_Stores.Name = "tab_Stores";
		this.tab_Stores.ReadOnly = false;
		this.tab_Stores.SettingInVisible = false;
		this.tab_Stores.SettingReadonly = false;
		this.tab_Stores.Size = new System.Drawing.Size(1074, 286);
		this.tab_Stores.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Stores.Text = "Stores";
		this.grd_Stores.AllowPasteExcel = true;
		this.grd_Stores.ClipboardData = null;
		this.grd_Stores.ContextMenuStrip = this.cms_Store;
		this.grd_Stores.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Stores.Location = new System.Drawing.Point(0, 0);
		this.grd_Stores.MainView = this.gv_Stores;
		this.grd_Stores.Name = "grd_Stores";
		this.grd_Stores.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.glk_StrStatus, this.rep_CardCode, this.rep_StoreCFL, this.rep_StoresCFL, this.rep_StoreRegion });
		this.grd_Stores.Size = new System.Drawing.Size(1074, 286);
		this.grd_Stores.TabIndex = 9;
		this.grd_Stores.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Stores });
		this.cms_Store.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.menu_AddStore, this.menu_RemoveStore, this.menu_DuplicateStore });
		this.cms_Store.Name = "contextMenuStrip3";
		this.cms_Store.Size = new System.Drawing.Size(125, 70);
		this.menu_AddStore.Name = "menu_AddStore";
		this.menu_AddStore.Size = new System.Drawing.Size(124, 22);
		this.menu_AddStore.Text = "Add New";
		this.menu_AddStore.Click += new System.EventHandler(this.addNewToolStripMenuItem1_Click);
		this.menu_RemoveStore.Name = "menu_RemoveStore";
		this.menu_RemoveStore.Size = new System.Drawing.Size(124, 22);
		this.menu_RemoveStore.Text = "Remove";
		this.menu_RemoveStore.Click += new System.EventHandler(this.menu_RemoveStore_Click);
		this.menu_DuplicateStore.Name = "menu_DuplicateStore";
		this.menu_DuplicateStore.Size = new System.Drawing.Size(124, 22);
		this.menu_DuplicateStore.Text = "Duplicate";
		this.menu_DuplicateStore.Click += new System.EventHandler(this.menu_DuplicateStore_Click);
		this.gv_Stores.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Stores.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Stores.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Stores.ColumnKey = null;
		this.gv_Stores.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.gc_Code, this.gc_StrCode, this.gc_StrStatus, this.gc_RegionCode, this.gc_City, this.gc_District, this.gc_Ward, this.gc_Town, this.gc_StrCardCode, this.gc_StrAddress,
			this.gc_StrName, this.gc_SRemarks
		});
		this.gv_Stores.DisableFormSetting = false;
		this.gv_Stores.DisplayException = false;
		this.gv_Stores.EditColumn = null;
		this.gv_Stores.FieldCurrency = null;
		this.gv_Stores.FieldRate = null;
		this.gv_Stores.FormId = "oSaleTerritory";
		this.gv_Stores.GridControl = this.grd_Stores;
		this.gv_Stores.IgnoreFilter = false;
		this.gv_Stores.IndexEditRow = -1;
		this.gv_Stores.IndicatorWidth = 35;
		this.gv_Stores.IsApzTable = true;
		this.gv_Stores.IsInsertNewRow = false;
		this.gv_Stores.IsLoadingData = false;
		this.gv_Stores.IsMappingContextAction = false;
		this.gv_Stores.ItemId = "APZ_D_STR3";
		this.gv_Stores.LstColumnAllowBatch = null;
		this.gv_Stores.LstColumnNotAllowBatch = null;
		this.gv_Stores.LstColumnNotEditable = null;
		this.gv_Stores.LstColumNotSetting = null;
		this.gv_Stores.Name = "gv_Stores";
		this.gv_Stores.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Stores.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Stores.OptionsPrint.AutoWidth = false;
		this.gv_Stores.OptionsPrint.UsePrintStyles = false;
		this.gv_Stores.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Stores.OptionsView.ColumnAutoWidth = false;
		this.gv_Stores.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Stores.OptionsView.ShowGroupedColumns = true;
		this.gv_Stores.OptionsView.ShowGroupPanel = false;
		this.gv_Stores.UseBatchMod = false;
		this.gv_Stores.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Stores_CustomDrawRowIndicator);
		this.gv_Stores.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Stores_RowCellStyle);
		this.gv_Stores.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Stores_ShowingEditor);
		this.gv_Stores.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Stores_CellValueChanged);
		this.gc_Code.Caption = "Code";
		this.gc_Code.CurrencyDateField = null;
		this.gc_Code.CurrencyField = null;
		this.gc_Code.CurrencyFixed = null;
		this.gc_Code.CurrencyFixedRate = 0.0;
		this.gc_Code.CurrencyRateField = null;
		this.gc_Code.FieldName = "Code";
		this.gc_Code.Name = "gc_Code";
		this.gc_Code.PrimaryKey = false;
		this.gc_Code.UnitField = null;
		this.gc_StrCode.Caption = "Code";
		this.gc_StrCode.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Store;
		this.gc_StrCode.ColumnEdit = this.rep_StoresCFL;
		this.gc_StrCode.CurrencyDateField = null;
		this.gc_StrCode.CurrencyField = null;
		this.gc_StrCode.CurrencyFixed = null;
		this.gc_StrCode.CurrencyFixedRate = 0.0;
		this.gc_StrCode.CurrencyRateField = null;
		this.gc_StrCode.FieldName = "StrCode";
		this.gc_StrCode.Name = "gc_StrCode";
		this.gc_StrCode.PrimaryKey = false;
		this.gc_StrCode.UnitField = null;
		this.gc_StrCode.Visible = true;
		this.gc_StrCode.VisibleIndex = 0;
		this.rep_StoresCFL.AutoHeight = false;
		this.rep_StoresCFL.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions12.Image");
		this.rep_StoresCFL.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance11, appearanceHovered11, appearancePressed11, appearanceDisabled11, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance12, appearanceHovered12, appearancePressed12, appearanceDisabled12, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_StoresCFL.ContainerControl = this;
		this.rep_StoresCFL.DisplayMember = "StrCode";
		this.rep_StoresCFL.DisplayMember2 = "";
		this.rep_StoresCFL.LoadDataSourceWhenEnter = false;
		this.rep_StoresCFL.Name = "rep_StoresCFL";
		this.rep_StoresCFL.SeparateCharacterForMulti = ';';
		this.rep_StoresCFL.SeparateString = ", ";
		this.rep_StoresCFL.UsingAutoCompleteBox = false;
		this.rep_StoresCFL.UsingDataSource = true;
		this.rep_StoresCFL.ValidateWhenNoDataSource = false;
		this.rep_StoresCFL.ValueMember = "StrCode";
		this.gc_StrStatus.Caption = "Status";
		this.gc_StrStatus.ColumnEdit = this.glk_StrStatus;
		this.gc_StrStatus.CurrencyDateField = null;
		this.gc_StrStatus.CurrencyField = null;
		this.gc_StrStatus.CurrencyFixed = null;
		this.gc_StrStatus.CurrencyFixedRate = 0.0;
		this.gc_StrStatus.CurrencyRateField = null;
		this.gc_StrStatus.FieldName = "Status";
		this.gc_StrStatus.Name = "gc_StrStatus";
		this.gc_StrStatus.PrimaryKey = false;
		this.gc_StrStatus.UnitField = null;
		this.gc_StrStatus.Visible = true;
		this.gc_StrStatus.VisibleIndex = 2;
		this.glk_StrStatus.AutoHeight = false;
		this.glk_StrStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.glk_StrStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.glk_StrStatus.DisplayMember = "Name";
		this.glk_StrStatus.Name = "glk_StrStatus";
		this.glk_StrStatus.NullText = "";
		this.glk_StrStatus.ShowHeader = false;
		this.glk_StrStatus.ValueMember = "Code";
		this.gc_RegionCode.Caption = "Region";
		this.gc_RegionCode.CurrencyDateField = null;
		this.gc_RegionCode.CurrencyField = null;
		this.gc_RegionCode.CurrencyFixed = null;
		this.gc_RegionCode.CurrencyFixedRate = 0.0;
		this.gc_RegionCode.CurrencyRateField = null;
		this.gc_RegionCode.FieldName = "Region";
		this.gc_RegionCode.Name = "gc_RegionCode";
		this.gc_RegionCode.PrimaryKey = false;
		this.gc_RegionCode.UnitField = null;
		this.gc_RegionCode.Visible = true;
		this.gc_RegionCode.VisibleIndex = 3;
		this.gc_City.Caption = "City";
		this.gc_City.CurrencyDateField = null;
		this.gc_City.CurrencyField = null;
		this.gc_City.CurrencyFixed = null;
		this.gc_City.CurrencyFixedRate = 0.0;
		this.gc_City.CurrencyRateField = null;
		this.gc_City.FieldName = "City";
		this.gc_City.Name = "gc_City";
		this.gc_City.PrimaryKey = false;
		this.gc_City.UnitField = null;
		this.gc_City.Visible = true;
		this.gc_City.VisibleIndex = 4;
		this.gc_District.Caption = "District";
		this.gc_District.CurrencyDateField = null;
		this.gc_District.CurrencyField = null;
		this.gc_District.CurrencyFixed = null;
		this.gc_District.CurrencyFixedRate = 0.0;
		this.gc_District.CurrencyRateField = null;
		this.gc_District.FieldName = "District";
		this.gc_District.Name = "gc_District";
		this.gc_District.PrimaryKey = false;
		this.gc_District.UnitField = null;
		this.gc_District.Visible = true;
		this.gc_District.VisibleIndex = 5;
		this.gc_Ward.Caption = "Ward";
		this.gc_Ward.CurrencyDateField = null;
		this.gc_Ward.CurrencyField = null;
		this.gc_Ward.CurrencyFixed = null;
		this.gc_Ward.CurrencyFixedRate = 0.0;
		this.gc_Ward.CurrencyRateField = null;
		this.gc_Ward.FieldName = "Ward";
		this.gc_Ward.Name = "gc_Ward";
		this.gc_Ward.PrimaryKey = false;
		this.gc_Ward.UnitField = null;
		this.gc_Ward.Visible = true;
		this.gc_Ward.VisibleIndex = 6;
		this.gc_Town.Caption = "Town";
		this.gc_Town.CurrencyDateField = null;
		this.gc_Town.CurrencyField = null;
		this.gc_Town.CurrencyFixed = null;
		this.gc_Town.CurrencyFixedRate = 0.0;
		this.gc_Town.CurrencyRateField = null;
		this.gc_Town.FieldName = "Town";
		this.gc_Town.Name = "gc_Town";
		this.gc_Town.PrimaryKey = false;
		this.gc_Town.UnitField = null;
		this.gc_Town.Visible = true;
		this.gc_Town.VisibleIndex = 7;
		this.gc_StrCardCode.Caption = "Card Code";
		this.gc_StrCardCode.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Bp;
		this.gc_StrCardCode.ColumnEdit = this.rep_CardCode;
		this.gc_StrCardCode.CurrencyDateField = null;
		this.gc_StrCardCode.CurrencyField = null;
		this.gc_StrCardCode.CurrencyFixed = null;
		this.gc_StrCardCode.CurrencyFixedRate = 0.0;
		this.gc_StrCardCode.CurrencyRateField = null;
		this.gc_StrCardCode.FieldName = "CardCode";
		this.gc_StrCardCode.Name = "gc_StrCardCode";
		this.gc_StrCardCode.PrimaryKey = false;
		this.gc_StrCardCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_StrCardCode.UnitField = null;
		this.gc_StrCardCode.Visible = true;
		this.gc_StrCardCode.VisibleIndex = 8;
		this.rep_CardCode.AutoHeight = false;
		this.rep_CardCode.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions14.Image");
		this.rep_CardCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance13, appearanceHovered13, appearancePressed13, appearanceDisabled13, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance14, appearanceHovered14, appearancePressed14, appearanceDisabled14, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_CardCode.ContainerControl = this;
		this.rep_CardCode.DisplayMember = "CardCode";
		this.rep_CardCode.DisplayMember2 = "";
		this.rep_CardCode.LoadDataSourceWhenEnter = false;
		this.rep_CardCode.Name = "rep_CardCode";
		this.rep_CardCode.SeparateCharacterForMulti = ';';
		this.rep_CardCode.SeparateString = ", ";
		this.rep_CardCode.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Bp;
		this.rep_CardCode.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oBusinessPartners;
		this.rep_CardCode.UsingAutoCompleteBox = false;
		this.rep_CardCode.UsingDataSource = true;
		this.rep_CardCode.ValidateWhenNoDataSource = false;
		this.rep_CardCode.ValueMember = "CardCode";
		this.gc_StrAddress.Caption = "Address";
		this.gc_StrAddress.CurrencyDateField = null;
		this.gc_StrAddress.CurrencyField = null;
		this.gc_StrAddress.CurrencyFixed = null;
		this.gc_StrAddress.CurrencyFixedRate = 0.0;
		this.gc_StrAddress.CurrencyRateField = null;
		this.gc_StrAddress.FieldName = "Address";
		this.gc_StrAddress.Name = "gc_StrAddress";
		this.gc_StrAddress.PrimaryKey = false;
		this.gc_StrAddress.UnitField = null;
		this.gc_StrAddress.Visible = true;
		this.gc_StrAddress.VisibleIndex = 9;
		this.gc_StrName.Caption = "Name";
		this.gc_StrName.CurrencyDateField = null;
		this.gc_StrName.CurrencyField = null;
		this.gc_StrName.CurrencyFixed = null;
		this.gc_StrName.CurrencyFixedRate = 0.0;
		this.gc_StrName.CurrencyRateField = null;
		this.gc_StrName.FieldName = "StrName";
		this.gc_StrName.Name = "gc_StrName";
		this.gc_StrName.PrimaryKey = false;
		this.gc_StrName.UnitField = null;
		this.gc_StrName.Visible = true;
		this.gc_StrName.VisibleIndex = 1;
		this.gc_SRemarks.Caption = "Remarks";
		this.gc_SRemarks.CurrencyDateField = null;
		this.gc_SRemarks.CurrencyField = null;
		this.gc_SRemarks.CurrencyFixed = null;
		this.gc_SRemarks.CurrencyFixedRate = 0.0;
		this.gc_SRemarks.CurrencyRateField = null;
		this.gc_SRemarks.FieldName = "Remarks";
		this.gc_SRemarks.Name = "gc_SRemarks";
		this.gc_SRemarks.PrimaryKey = false;
		this.gc_SRemarks.UnitField = null;
		this.gc_SRemarks.Visible = true;
		this.gc_SRemarks.VisibleIndex = 10;
		this.rep_StoreCFL.AutoHeight = false;
		this.rep_StoreCFL.AutoLoadDataSource = true;
		this.rep_StoreCFL.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance15, appearanceHovered15, appearancePressed15, appearanceDisabled15, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, imageOptions10, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance16, appearanceHovered16, appearancePressed16, appearanceDisabled16, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_StoreCFL.ContainerControl = this;
		this.rep_StoreCFL.DisplayMember = "GgpCode";
		this.rep_StoreCFL.DisplayMember2 = "";
		this.rep_StoreCFL.LoadDataSourceWhenEnter = false;
		this.rep_StoreCFL.Name = "rep_StoreCFL";
		this.rep_StoreCFL.SeparateCharacterForMulti = ';';
		this.rep_StoreCFL.SeparateString = ", ";
		this.rep_StoreCFL.UsingAutoCompleteBox = false;
		this.rep_StoreCFL.UsingDataSource = true;
		this.rep_StoreCFL.ValidateWhenNoDataSource = false;
		this.rep_StoreCFL.ValueMember = "GgpCode";
		this.rep_StoreRegion.AutoHeight = false;
		this.rep_StoreRegion.AutoLoadDataSource = true;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions18.Image");
		this.rep_StoreRegion.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions11, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance17, appearanceHovered17, appearancePressed17, appearanceDisabled17, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance18, appearanceHovered18, appearancePressed18, appearanceDisabled18, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_StoreRegion.ContainerControl = this;
		this.rep_StoreRegion.DisplayMember = "StrGgpCode";
		this.rep_StoreRegion.DisplayMember2 = "";
		this.rep_StoreRegion.LoadDataSourceWhenEnter = false;
		this.rep_StoreRegion.Name = "rep_StoreRegion";
		this.rep_StoreRegion.SeparateCharacterForMulti = ';';
		this.rep_StoreRegion.SeparateString = ", ";
		this.rep_StoreRegion.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.GeographyDetails;
		this.rep_StoreRegion.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oGeographyDetail;
		this.rep_StoreRegion.UsingAutoCompleteBox = false;
		this.rep_StoreRegion.UsingDataSource = true;
		this.rep_StoreRegion.ValidateWhenNoDataSource = false;
		this.rep_StoreRegion.ValueMember = "StrGgpCode";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_OK.Appearance.Options.UseFont = true;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(7, 523);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 20);
		this.btn_OK.TabIndex = 10;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_Cancel.Appearance.Options.UseFont = true;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(88, 523);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 11;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.txt_TerritoryCode.EditControl = null;
		this.txt_TerritoryCode.FieldName = null;
		this.txt_TerritoryCode.IsSearchControl = false;
		this.txt_TerritoryCode.Location = new System.Drawing.Point(98, 5);
		this.txt_TerritoryCode.Name = "txt_TerritoryCode";
		this.txt_TerritoryCode.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txt_TerritoryCode.Properties.Appearance.Options.UseFont = true;
		this.txt_TerritoryCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.txt_TerritoryCode.Properties.AppearanceFocused.BackColor2 = System.Drawing.Color.FromArgb(250, 249, 215);
		this.txt_TerritoryCode.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txt_TerritoryCode.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_TerritoryCode.Properties.AppearanceFocused.Options.UseFont = true;
		this.txt_TerritoryCode.SettingInVisible = false;
		this.txt_TerritoryCode.SettingReadonly = false;
		this.txt_TerritoryCode.Size = new System.Drawing.Size(150, 20);
		this.txt_TerritoryCode.TabIndex = 1;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1028, 551);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.lk_Status);
		base.Controls.Add(this.txt_TerritoryCode);
		base.Controls.Add(this.txt_Location);
		base.Controls.Add(this.lbl_Status);
		base.Controls.Add(this.lbl_Location);
		base.Controls.Add(this.lbl_TerritoryCode);
		base.FormId = "oSaleTerritory";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SaleTerritory";
		this.Text = "Sale Territory";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_SaleTerritory_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.rep_CountryCFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.glk_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Location.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lk_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_SaleTerritory).EndInit();
		this.cms_Territory.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_SaleTerritory).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CountriesCFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tab_Main).EndInit();
		this.tab_Main.ResumeLayout(false);
		this.tab_Structure.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grb_Structure).EndInit();
		this.grb_Structure.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tv_Structure).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_AddStructure).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Region).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tlk_Status).EndInit();
		this.cms_Structure.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.Rep_RegionCFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grb_Detail).EndInit();
		this.grb_Detail.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Detail).EndInit();
		this.cms_Detail.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Detail).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DetailCFL).EndInit();
		this.tab_Stores.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Stores).EndInit();
		this.cms_Store.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Stores).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_StoresCFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.glk_StrStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CardCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_StoreCFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_StoreRegion).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TerritoryCode.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
