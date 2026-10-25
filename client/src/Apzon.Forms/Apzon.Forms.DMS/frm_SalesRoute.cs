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
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.HumanResources;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace Apzon.Forms.DMS;

public class frm_SalesRoute : XtraFormEx
{
	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isAddSame;

	private int _levelSameGeography;

	private DataTable EmployeesData = new DataTable();

	private DataTable OutletsData = new DataTable();

	private DataTable DistributorsData = new DataTable();

	private IContainer components;

	private LabelLine lbl_RCode;

	private ButtonEditEx btn_RouteCode;

	private LabelLine lbl_RouteName;

	private ButtonEditEx btn_RouteName;

	private LabelLine lbl_Status;

	private LookUpEditEx lk_Status;

	private SimpleButtonEx btn_Search;

	private SplitContainerControl split_Main;

	private GridControlEx grd_Routes;

	private GridViewEx gv_Routes;

	private SplitContainerControl split_Details;

	private GroupControl groupControl1;

	private TreeEx tv_Details;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private ContextMenuStrip ctm_Routes;

	private ContextMenuStrip ctm_Details;

	private ContextMenuStrip ctm_Outlet;

	private GridColumnEx gc_RouteCode;

	private GridColumnEx gc_RouteName;

	private GridColumnEx gc_CountryCode;

	private RepositoryItemButtonEdit rep_CountryCode;

	private GridColumnEx gc_DocStatus;

	private RepositoryItemLookUpEdit rep_DocStatus;

	private GridColumnEx gc_RouteRmarks;

	private GridColumnEx gc_ChannelCode;

	private GridColumnEx gc_UserSign;

	private GridColumnEx gc_CreateDate;

	private GridColumnEx gc_CreateTime;

	private GridColumnEx gc_UpdateDate;

	private GridColumnEx gc_UpdateTime;

	private TreeListColumnEx treeListColumnEx2;

	private TreeListColumnEx treeListColumnEx3;

	private TreeListColumnEx treeListColumnEx4;

	private TreeListColumnEx treeListColumnEx5;

	private TreeListColumnEx treeListColumnEx7;

	private TreeListColumnEx treeListColumnEx1;

	private TreeListColumnEx treeListColumnEx6;

	private ToolStripMenuItem menu_AddRoute;

	private ToolStripMenuItem menu_RemoveRoute;

	private ToolStripMenuItem menu_DuplicateRoute;

	private ToolStripMenuItem menu_AddDetail;

	private ToolStripMenuItem menu_AddSame;

	private ToolStripMenuItem menu_Insert;

	private ToolStripMenuItem menu_Remove;

	private ToolStripMenuItem menu_AddOutlet;

	private ToolStripMenuItem menu_RemoveOutlet;

	private RepositoryItemButtonEdit rep_Channel;

	private TreeListColumnEx tc_Code;

	private TreeListColumnEx tc_Name;

	private TreeListColumnEx tc_RouteCode;

	private TreeListColumnEx tc_CountryCode;

	private TreeListColumnEx tc_SuperVisor;

	private RepositoryItemButtonEditCFL rep_Employee;

	private RepositoryItemButtonEdit rep_Geography;

	private RepositoryItemLookUpEdit rep_Status1;

	private RepositoryItemButtonEdit btn_AddTree;

	private RepositoryItemLookUpEdit rep_Type;

	private RepositoryItemButtonEdit rep_BP;

	private RepositoryItemButtonEdit rep_BPCode;

	private TreeListColumnEx tc_Status1;

	private TreeListColumnEx tc_Remarks1;

	private TreeListColumnEx tc_Id;

	private TreeListColumnEx treeListColumnEx12;

	private TreeListColumnEx tc_LevelGgp;

	private TreeListColumnEx tc_Type;

	private XtraTabControlEx xtraTabControlEx1;

	private XtraTabPageEx xtraTabPageEx2;

	private GridControlEx grd_Outlets;

	private GridViewEx gv_Outlets;

	private GridColumnEx gc_Id;

	private GridColumnEx gc_RouteCode1;

	private GridColumnEx gc_EmpId;

	private GridColumnEx gc_OutletCode;

	private RepositoryItemButtonEdit rep_OutletCode;

	private GridColumnEx gc_OutletName;

	private GridColumnEx gc_Address;

	private GridColumnEx gc_Remarks;

	private RepositoryItemButtonEdit rep_StoreCode;

	private RepositoryItemLookUpEdit rep_StoreStatus;

	private XtraTabPageEx xtraTabPageEx1;

	private GridControlEx grd_Distributors;

	private GridViewEx gv_Distributors;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gc_CardCode;

	private RepositoryItemButtonEdit rep_CardCode;

	private GridColumnEx gc_CardName;

	private GridColumnEx gridColumnEx4;

	private RepositoryItemCheckEdit chk_Active;

	private ContextMenuStrip ctm_Distributors;

	private ToolStripMenuItem menu_AddDistributor;

	private ToolStripMenuItem menu_RemoveDistributor;

	private RepositoryItemButtonEdit rep_ChooseType;

	private GridColumnEx gc_Remarks1;

	private TreeListColumnEx tc_BaseId;

	private SplitContainerControl split_Employee;

	private GridControlEx grd_Emp;

	private GridViewEx gv_Emp;

	private GridColumnEx gridColumnEx5;

	private RepositoryItemButtonEditCFL rep_SalesEmployee;

	private ContextMenuStrip ctm_Emp;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private ToolStripMenuItem removeToolStripMenuItem;

	private TreeListColumnEx treeListColumnEx8;

	private RepositoryItemButtonEditCFL rep_SalesAdmin;

	private GridColumnEx gridColumnEx6;

	private RepositoryItemButtonEditCFL rep_PriceList;

	private GridColumnEx gridColumnEx7;

	private RepositoryItemButtonEditCFL rep_PriceListDistributor;

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
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_SalesRoute()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalesRoute"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalesRoute" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				ctm_Routes.Enabled = false;
				ctm_Details.Enabled = false;
				ctm_Outlet.Enabled = false;
				btn_Cancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Apzon.Commons.Properties.Resources.Msg_FormAccessDenied);
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
			menu_AddRoute_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.First, Function.ToString(gv_Routes.GetRowCellValue(gv_Routes.FocusedRowHandle, "RouteCode")));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.Previous, Function.ToString(gv_Routes.GetRowCellValue(gv_Routes.FocusedRowHandle, "RouteCode")));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.Next, Function.ToString(gv_Routes.GetRowCellValue(gv_Routes.FocusedRowHandle, "RouteCode")));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.Last, Function.ToString(gv_Routes.GetRowCellValue(gv_Routes.FocusedRowHandle, "RouteCode")));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			WinCommons._OpenProcessing();
			await LoadData("", "", "");
			tv_Details.DataSourceTable.Clear();
			grd_Outlets.DataSourceTable.Clear();
			OutletsData = grd_Outlets.DataSourceTable.Copy().Clone();
			DistributorsData = grd_Distributors.DataSourceTable.Copy().Clone();
			EmployeesData = grd_Distributors.DataSourceTable.Copy().Clone();
			btn_RouteCode.EditValue = null;
			btn_RouteName.EditValue = null;
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	private async Task InitData()
	{
		_ = 4;
		try
		{
			Dictionary<string, string> dic = new Dictionary<string, string>
			{
				{ "Y", "Hoạt động" },
				{ "N", "Không hoạt động" }
			};
			Dictionary<string, string> dic2 = new Dictionary<string, string>
			{
				{ "T", "Vùng" },
				{ "G", "Vị trí địa lý" }
			};
			lk_Status.SetDataSource(dic);
			rep_DocStatus.SetDataSource(dic);
			rep_Status1.SetDataSource(dic);
			rep_StoreStatus.SetDataSource(dic);
			rep_Type.SetDataSource(dic2);
			rep_Employee.TypeCFL = Enums.TypeCFL.Employees;
			rep_Employee.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			rep_Employee.ValueMember = "empID";
			rep_Employee.DisplayMember = "fullName";
			rep_Employee.SelectedFromCFL += employee_choosed;
			await rep_Employee.RefreshDataSourceAsync();
			await rep_SalesEmployee.RefreshDataSourceAsync();
			rep_SalesAdmin.ValueMember = "empID";
			rep_SalesAdmin.DisplayMember = "fullName";
			rep_SalesAdmin.DisplayMember2 = "";
			rep_SalesAdmin.TypeCFL = Enums.TypeCFL.Employees;
			rep_SalesAdmin.TypeDrillDown = ApzObjectTypes.oNone;
			rep_SalesAdmin.MultiSelectMode = true;
			rep_SalesAdmin.SelectedFromCFL += salesAdmin_SelectedFromCFL;
			await rep_SalesAdmin.RefreshDataSourceAsync();
			await rep_PriceList.RefreshDataSourceAsync();
			await rep_PriceListDistributor.RefreshDataSourceAsync();
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_D_OSTR");
			DataTable tableStructure2 = WinFuntions.GetTableStructure("APZ_D_STR1");
			DataTable tableStructure3 = WinFuntions.GetTableStructure("APZ_D_STR2");
			DataTable tableStructure4 = WinFuntions.GetTableStructure("APZ_D_STR3");
			if (tableStructure.Columns.Count == 0 || tableStructure2.Columns.Count == 0 || tableStructure3.Columns.Count == 0 || tableStructure4.Columns.Count == 0)
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Cancel.Enabled = true;
				ctm_Routes.Enabled = false;
				ctm_Details.Enabled = false;
				ctm_Outlet.Enabled = false;
				ctm_Distributors.Enabled = false;
				WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void salesAdmin_SelectedFromCFL(object sender, ButtonEditCFLEventArgs e)
	{
		try
		{
			_ = e.ChooseItems;
			if (e.ChooseItems.Count > 0)
			{
				string val = string.Join(";", e.ChooseItems.Select((DataRow t) => Function.ToString(t["empID"])));
				tv_Details.FocusedNode.SetValue("SalesAdmin", val);
				tv_Details.DataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void employee_choosed(object sender, ButtonEditCFLEventArgs e)
	{
		try
		{
			if (e.ChooseItem != null)
			{
				tv_Details.FocusedNode.SetValue("SuperVisor", Function.ParseInt(e.ChooseItem["empID"]));
				tv_Details.DataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private async Task LoadData(string routeCode, string routeName, string status)
	{
		try
		{
			GridControlEx gridControlEx = grd_Routes;
			gridControlEx.DataSource = await _httpClient.SalesRouteClient.GetDataDoc(routeCode, routeName, status);
			grd_Routes.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void LoadDetails(string routeCode)
	{
		try
		{
			EmployeesData = _httpClient.SalesRouteClient.GetEmployees(routeCode);
			OutletsData = _httpClient.SalesRouteClient.GetOutlets(routeCode);
			DistributorsData = _httpClient.SalesRouteClient.GetDistributors(routeCode);
			tv_Details.DataSource = _httpClient.SalesRouteClient.GetDataDetails(routeCode);
			tv_Details.RefreshDataSource();
			tv_Details.ExpandAll();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void GenerateBpStructure(DataTable dt)
	{
		try
		{
			dt.Columns.Add("Id", typeof(int));
			dt.Columns.Add("RouteCode", typeof(string));
			dt.Columns.Add("EmpId", typeof(int));
			dt.Columns.Add("CardCode", typeof(string));
			dt.Columns.Add("CardName", typeof(string));
			dt.Columns.Add("CardType", typeof(string));
			dt.Columns.Add("Address", typeof(string));
			dt.Columns.Add("Remarks", typeof(string));
			dt.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void AddNewDoc()
	{
		try
		{
			DataTable dataTable = grd_Routes.DataSourceTable.Clone();
			if (dataTable.Columns.Count == 0)
			{
				dataTable = WinFuntions.GetTableStructure("APZ_D_OSTR");
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["Status"] = "Y";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_Routes.DataSource = dataTable;
			grd_Routes.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddDetails(TreeLevel level, int fatherid = -1, string type = "T")
	{
		try
		{
			tv_Details.PostEditor();
			DataTable dataTable = tv_Details.DataSourceTable();
			if (dataTable.Columns.Count == 0)
			{
				dataTable = WinFuntions.GetTableStructure("APZ_D_STR1");
			}
			int num = Function.ParseInt(dataTable.AsEnumerable().Max((DataRow r) => r["Id"]));
			DataRow dataRow = dataTable.NewRow();
			switch (level)
			{
			default:
				dataRow["Id"] = num + 1;
				dataRow["FatherId"] = fatherid;
				break;
			case TreeLevel.Insert:
				if (Function.ToString(tv_Details.FocusedNode.GetValue("Type")) == "E")
				{
					return;
				}
				dataRow["Id"] = num + 1;
				dataRow["FatherId"] = Function.ParseInt(tv_Details.FocusedNode.GetValue("Id"));
				break;
			case TreeLevel.SameLevel:
				dataRow["Id"] = num + 1;
				dataRow["FatherId"] = Function.ParseInt(tv_Details.FocusedNode.GetValue("FatherId"));
				break;
			}
			dataRow["Type"] = type;
			dataRow["Status"] = "Y";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			tv_Details.DataSource = dataTable;
			tv_Details.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddSubDetails(int id, string routeCode, int empId, bool isOutlets, bool isNew)
	{
		try
		{
			DataTable dataTable = new DataTable();
			if (isOutlets)
			{
				dataTable = (isNew ? grd_Outlets.DataSourceTable.Clone() : grd_Outlets.DataSourceTable);
			}
			else
			{
				dataTable = (isNew ? grd_Distributors.DataSourceTable.Clone() : grd_Distributors.DataSourceTable);
			}
			if (dataTable.Columns.Count == 0)
			{
				GenerateBpStructure(dataTable);
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["Id"] = id;
			dataRow["RouteCode"] = routeCode;
			dataRow["EmpId"] = empId;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			if (isOutlets)
			{
				grd_Outlets.DataSource = dataTable;
				grd_Outlets.RefreshDataSource();
			}
			else
			{
				grd_Distributors.DataSource = dataTable;
				grd_Distributors.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ShowSalesRouteCfl(bool isRouteCode)
	{
		try
		{
			using frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.SalesRoute, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				if (isRouteCode)
				{
					btn_RouteCode.Text = Function.ToString(frm_CFL2.ChooseItem[0]);
				}
				else
				{
					btn_RouteName.Text = Function.ToString(frm_CFL2.ChooseItem[1]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void ShowGeographyForm(string countryCode, string ggpCode)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_Geography frm_Geography2 = new frm_Geography(countryCode, ggpCode);
			frm_Geography2.MdiParent = MdiParent;
			frm_Geography2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	private void ShowBpMasterData()
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
			frm_BusinessMasterData2.MdiParent = MdiParent;
			frm_BusinessMasterData2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	private void RemoveDetails()
	{
		try
		{
			if (!tv_Details.DataSourceTable.IsNotNull())
			{
				return;
			}
			TreeListMultiSelection selection = tv_Details.Selection;
			if (selection != null)
			{
				int num = Function.ParseInt(tv_Details.FocusedNode.GetValue("Id"));
				if (OutletsData.IsNotNull())
				{
					DataRow[] array = OutletsData.Select("Id =" + num);
					for (int i = 0; i < array.Length; i++)
					{
						OutletsData.Rows.Remove(array[i]);
					}
					OutletsData.AcceptChanges();
				}
				if (grd_Outlets.DataSourceTable.IsNotNull())
				{
					grd_Outlets.DataSourceTable.Clear();
					grd_Outlets.DataSourceTable.AcceptChanges();
				}
				if (EmployeesData.IsNotNull())
				{
					DataRow[] array2 = EmployeesData.Select("Id =" + num);
					for (int j = 0; j < array2.Length; j++)
					{
						EmployeesData.Rows.Remove(array2[j]);
					}
					EmployeesData.AcceptChanges();
				}
				if (grd_Emp.DataSourceTable.IsNotNull())
				{
					grd_Emp.DataSourceTable.Clear();
					grd_Emp.DataSourceTable.AcceptChanges();
				}
				if (DistributorsData.IsNotNull())
				{
					DataRow[] array3 = DistributorsData.Select("Id =" + num);
					for (int k = 0; k < array3.Length; k++)
					{
						DistributorsData.Rows.Remove(array3[k]);
					}
					DistributorsData.AcceptChanges();
				}
				if (grd_Distributors.DataSourceTable.IsNotNull())
				{
					grd_Distributors.DataSourceTable.Clear();
					grd_Distributors.DataSourceTable.AcceptChanges();
				}
				foreach (TreeListNode item in selection.ToList())
				{
					if (item.Nodes != null)
					{
						foreach (TreeListNode item2 in item.Nodes.ToList())
						{
							tv_Details.DeleteNode(item2);
						}
					}
					tv_Details.DeleteNode(item);
				}
			}
			tv_Details.DataSourceTable.AcceptChanges();
			if (!tv_Details.DataSourceTable.IsNotNull())
			{
				AddDetails(TreeLevel.Root);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void GetRecord(Enums.NavigateType type, string routeCode)
	{
		try
		{
			DataTable dataTable = new DataTable();
			switch (type)
			{
			case Enums.NavigateType.First:
				dataTable = _httpClient.SalesRouteClient.GetRecord("F", routeCode);
				break;
			case Enums.NavigateType.Last:
				dataTable = _httpClient.SalesRouteClient.GetRecord("L", routeCode);
				break;
			case Enums.NavigateType.Previous:
				dataTable = _httpClient.SalesRouteClient.GetRecord("P", routeCode);
				break;
			case Enums.NavigateType.Next:
				dataTable = _httpClient.SalesRouteClient.GetRecord("N", routeCode);
				break;
			}
			if (dataTable.IsNotNull())
			{
				grd_Routes.DataSource = dataTable;
				grd_Routes.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SaveTemporaryData(int id, DataTable dtSource, ref DataTable dtTarget)
	{
		try
		{
			if (dtTarget.Columns.Count == 0)
			{
				GenerateBpStructure(dtTarget);
			}
			if (!dtSource.IsNotNull())
			{
				return;
			}
			if (dtTarget.AsEnumerable().FirstOrDefault((DataRow row) => Function.ParseInt(row["Id"]) == id || string.IsNullOrEmpty(Function.ToString(row["CardCode"]))) != null)
			{
				(from row1 in dtTarget.AsEnumerable()
					where Function.ParseInt(row1["Id"]) == id || string.IsNullOrEmpty(Function.ToString(row1["CardCode"]))
					select row1).ToList().ForEach((DataRow r) =>
				{
					r.Delete();
				});
				dtTarget.AcceptChanges();
			}
			foreach (DataRow row in dtSource.Rows)
			{
				dtTarget.Rows.Add(row.ItemArray);
			}
			dtTarget.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void UpdateFormView()
	{
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private TreeListNode FindParentNodeGeography(TreeListNode focusedNode)
	{
		try
		{
			if (focusedNode.ParentNode != null)
			{
				return (Function.ToString(focusedNode.ParentNode["Type"]) == "G") ? focusedNode.ParentNode : FindParentNodeGeography(focusedNode.ParentNode);
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	private void UpdateEmployee(bool isOutlet, int oldEmpId, int newEmpId)
	{
		try
		{
			DataTable dataTable = (isOutlet ? OutletsData : DistributorsData);
			if (dataTable.IsNotNull())
			{
				foreach (DataRow row in dataTable.Rows)
				{
					if (Function.ParseInt(row["EmpId"]) == oldEmpId)
					{
						row["EmpId"] = newEmpId;
					}
				}
			}
			dataTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GenerateBpMasterInput(bool isOutlet, int id, string routeCode, int empId)
	{
		try
		{
			string empty = string.Empty;
			DataTable dataTable = new DataTable();
			if (isOutlet)
			{
				empty = Function.ToString(gv_Outlets.GetRowCellValue(gv_Outlets.FocusedRowHandle, "CardCode"));
				dataTable = _httpClient.SalesRouteClient.GetBpMasterData("S", empty);
				if (dataTable.IsNotNull())
				{
					gv_Outlets.SetRowCellValue(gv_Outlets.FocusedRowHandle, "Id", id);
					gv_Outlets.SetRowCellValue(gv_Outlets.FocusedRowHandle, "RouteCode", routeCode);
					gv_Outlets.SetRowCellValue(gv_Outlets.FocusedRowHandle, "EmpId", empId);
					gv_Outlets.SetRowCellValue(gv_Outlets.FocusedRowHandle, "CardName", Function.ToString(dataTable.Rows[0]["CardName"]));
					gv_Outlets.SetRowCellValue(gv_Outlets.FocusedRowHandle, "CardType", Function.ToString(dataTable.Rows[0]["CardType"]));
					gv_Outlets.SetRowCellValue(gv_Outlets.FocusedRowHandle, "Address", Function.ToString(dataTable.Rows[0]["Address"]));
					grd_Outlets.DataSourceTable.AcceptChanges();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, "Outlet code is not valid");
				}
			}
			else
			{
				empty = Function.ToString(gv_Distributors.GetRowCellValue(gv_Distributors.FocusedRowHandle, "CardCode"));
				dataTable = _httpClient.SalesRouteClient.GetBpMasterData("D", empty);
				if (dataTable.IsNotNull())
				{
					gv_Distributors.SetRowCellValue(gv_Distributors.FocusedRowHandle, "Id", id);
					gv_Distributors.SetRowCellValue(gv_Distributors.FocusedRowHandle, "RouteCode", routeCode);
					gv_Distributors.SetRowCellValue(gv_Distributors.FocusedRowHandle, "EmpId", empId);
					gv_Distributors.SetRowCellValue(gv_Distributors.FocusedRowHandle, "CardName", Function.ToString(dataTable.Rows[0]["CardName"]));
					gv_Distributors.SetRowCellValue(gv_Distributors.FocusedRowHandle, "CardType", Function.ToString(dataTable.Rows[0]["CardType"]));
					gv_Distributors.SetRowCellValue(gv_Distributors.FocusedRowHandle, "Address", Function.ToString(dataTable.Rows[0]["Address"]));
					grd_Distributors.DataSourceTable.AcceptChanges();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, "CardCode is not valid");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void GenerateBpMasterCfl(bool isOutlet, int id, string routeCode, int empId)
	{
		try
		{
			int type = (isOutlet ? 10002072 : 10002073);
			DataTable dataTable = (isOutlet ? grd_Outlets.DataSourceTable : grd_Distributors.DataSourceTable);
			using frm_CFL frm_CFL2 = new frm_CFL((Enums.TypeCFL)type, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
			{
				WinCommons._OpenProcessing();
				dataTable.Rows.RemoveAt(dataTable.Rows.Count - 1);
				foreach (DataRow chooseItem in frm_CFL2.ChooseItems)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["Id"] = id;
					dataRow["RouteCode"] = routeCode;
					dataRow["EmpId"] = empId;
					dataRow["CardCode"] = Function.ToString(chooseItem["CardCode"]);
					dataRow["CardName"] = Function.ToString(chooseItem["CardName"]);
					dataRow["Address"] = Function.ToString(chooseItem["Address"]);
					dataTable.Rows.Add(dataRow);
				}
				dataTable.AcceptChanges();
				if (isOutlet)
				{
					menu_AddOutlet_Click(null, null);
				}
				else
				{
					menu_AddDistributor_Click(null, null);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	private void GenerateDetailData(Enums.TypeCFL typeCfl, TreeListNode focusedNode, string empType)
	{
		try
		{
			List<Common.ParameterValue> lstpr = new List<Common.ParameterValue>();
			frm_CFL frm_CFL2 = new frm_CFL(typeCfl, false, "", "", Enums.ItemObjectType.NoItemType, lstpr, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem == null)
			{
				return;
			}
			if (typeCfl == Enums.TypeCFL.SalesRouteEmployee)
			{
				if (empType.Equals("S"))
				{
					focusedNode.SetValue(tc_SuperVisor, frm_CFL2.ChooseItem[0]);
					return;
				}
				Function.ParseInt(tv_Details.FocusedNode.GetValue("Code"));
				int num = Function.ParseInt(frm_CFL2.ChooseItem[0]);
				focusedNode.SetValue(tc_Code, num);
				focusedNode.SetValue(tc_Name, Function.ToString(frm_CFL2.ChooseItem[2]) + " " + Function.ToString(frm_CFL2.ChooseItem[1]));
			}
			else
			{
				focusedNode.SetValue("Code", frm_CFL2.ChooseItem["Code"]);
				focusedNode.SetValue("Name", frm_CFL2.ChooseItem["NamePath"]);
				focusedNode.SetValue("CountryCode", frm_CFL2.ChooseItem["CountryCode"]);
				gv_Routes.SetFocusedRowCellValue(tc_CountryCode.FieldName, frm_CFL2.ChooseItem["CountryCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void DisableControls(string type)
	{
		try
		{
			if (type.Equals("G") || type.Equals("E"))
			{
				ctm_Outlet.Enabled = true;
				ctm_Distributors.Enabled = true;
				tc_SuperVisor.OptionsColumn.ReadOnly = true;
			}
			else
			{
				ctm_Outlet.Enabled = false;
				ctm_Distributors.Enabled = false;
				tc_SuperVisor.OptionsColumn.ReadOnly = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private async void frm_SalesRoute_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await InitData();
			await LoadData("", "", "");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void gv_Routes_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				FormMode = Enums.FormViewModeType.View;
			}
			grd_Distributors.DataSourceTable.Clear();
			grd_Distributors.DataSourceTable.AcceptChanges();
			grd_Emp.DataSourceTable.Clear();
			grd_Emp.DataSourceTable.AcceptChanges();
			grd_Outlets.DataSourceTable.Clear();
			grd_Outlets.DataSourceTable.AcceptChanges();
			EmployeesData.Clear();
			OutletsData.Clear();
			DistributorsData.Clear();
			LoadDetails(Function.ToString(gv_Routes.GetRowCellValue(gv_Routes.FocusedRowHandle, gc_RouteCode.FieldName)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_AddRoute_Click(object sender, EventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			AddNewDoc();
			tv_Details.DataSource = _httpClient.SalesRouteClient.GetDataDetails("");
			if (grd_Outlets.DataSourceTable.IsNotNull())
			{
				grd_Outlets.DataSourceTable.Clear();
				grd_Outlets.DataSourceTable.AcceptChanges();
			}
			AddDetails(TreeLevel.Root);
			AddDetails(TreeLevel.Root, 1, "G");
			tv_Details.Nodes[0].ExpandAll();
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

	private void frm_SalesRoute_FormClosing(object sender, FormClosingEventArgs e)
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

	private void btn_RouteCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			ShowSalesRouteCfl(isRouteCode: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_RouteName_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			ShowSalesRouteCfl(isRouteCode: false);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_CountryCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0 && !string.IsNullOrEmpty(Function.ToString(gv_Routes.GetRowCellValue(gv_Routes.FocusedRowHandle, gc_CountryCode.FieldName))))
			{
				ShowGeographyForm(Function.ToString(gv_Routes.GetRowCellValue(gv_Routes.FocusedRowHandle, gc_CountryCode.FieldName)), "");
				return;
			}
			using frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Geography, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gv_Routes.SetRowCellValue(gv_Routes.FocusedRowHandle, gc_CountryCode.FieldName, frm_CFL2.ChooseItem[0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_AddDetail_Click(object sender, EventArgs e)
	{
		try
		{
			AddDetails(TreeLevel.Root);
			_isAddSame = false;
			UpdateFormView();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Channel_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0 && !string.IsNullOrEmpty(Function.ToString(gv_Routes.GetRowCellValue(gv_Routes.FocusedRowHandle, gc_ChannelCode.FieldName))))
			{
				frm_Channel frm_Channel2 = new frm_Channel();
				frm_Channel2.MdiParent = MdiParent;
				frm_Channel2.Show();
				return;
			}
			using frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Channel, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gv_Routes.SetRowCellValue(gv_Routes.FocusedRowHandle, gc_ChannelCode.FieldName, frm_CFL2.ChooseItem[0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_AddSame_Click(object sender, EventArgs e)
	{
		try
		{
			if (tv_Details.FocusedNode != null)
			{
				_levelSameGeography = Function.ParseInt(tv_Details.FocusedNode.GetValue("LevelGgp"));
			}
			AddDetails(TreeLevel.SameLevel);
			_isAddSame = true;
			UpdateFormView();
			tv_Details.FocusedNode.ExpandAll();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void menu_Insert_Click(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(tv_Details.FocusedNode.GetValue("Type"));
			if (!string.IsNullOrEmpty(text) && text != "E")
			{
				AddDetails(TreeLevel.Insert);
				AddDetails(TreeLevel.Insert);
				_isAddSame = false;
			}
			UpdateFormView();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Geography_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
	}

	private void menu_AddOutlet_Click(object sender, EventArgs e)
	{
		try
		{
			TreeListNode focusedNode = tv_Details.FocusedNode;
			if (string.IsNullOrEmpty(Function.ToString(focusedNode.GetValue("Code"))))
			{
				WinCommons.Error(Enums.MessageCode.None, "You must choose Sales Employee");
				return;
			}
			AddSubDetails(Function.ParseInt(focusedNode["Id"]), Function.ToString(focusedNode["RouteCode"]), Function.ParseInt(focusedNode["Code"]), isOutlets: true, isNew: false);
			UpdateFormView();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		Enums.FormViewModeType currMode = FormMode;
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (btn_OK.Text == Apzon.Commons.Properties.Resources.ButtonOK || FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				FormMode = currMode;
				return;
			}
			DataSet dataset = GetDataset((currMode == Enums.FormViewModeType.Add) ? "A" : "U");
			HttpResult httpResult = await _httpClient.SalesRouteClient.SaveData(new Tuple<DataSet, string>(dataset, (currMode == Enums.FormViewModeType.Add) ? "A" : "U"));
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				FormMode = currMode;
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				FormMode = Enums.FormViewModeType.View;
			}
			return;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = currMode;
	}

	private DataSet GetDataset(string type)
	{
		DataTable dataTable = gv_Routes.GetListDataSourceSelectedRow().CopyToDataTable().Copy();
		if (type == "A")
		{
			dataTable.Rows[0]["UserSign"] = GlobalData.UserID;
		}
		else
		{
			dataTable.Rows[0]["UserSign2"] = GlobalData.UserID;
		}
		dataTable.AcceptChanges();
		InsertDataToTableEmployee(grd_Emp.DataSourceTable.Copy());
		InsertDataToTableOutlets(grd_Outlets.DataSourceTable.Copy());
		InsertDataToTableDistributors(grd_Distributors.DataSourceTable.Copy());
		DataTable dataTable2 = EmployeesData.Copy();
		DataTable dataTable3 = OutletsData.Copy();
		DataTable dataTable4 = DistributorsData.Copy();
		DataTable dataTable5 = tv_Details.DataSourceTable.Copy();
		DataSet dataSet = new DataSet();
		dataSet.Tables.AddRange(new DataTable[5] { dataTable, dataTable5, dataTable2, dataTable3, dataTable4 });
		return dataSet;
	}

	private void menu_Remove_Click(object sender, EventArgs e)
	{
		try
		{
			RemoveDetails();
			UpdateFormView();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private async void menu_RemoveRoute_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.View && XtraMessageBox.Show("Do you want to remove this Route?", "System Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				Tuple<string> tupData = new Tuple<string>(Function.ToString(gv_Routes.GetRowCellValue(gv_Routes.FocusedRowHandle, gc_RouteCode.FieldName)));
				HttpResult httpResult = await _httpClient.SalesRouteClient.Remove(tupData);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					return;
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				await LoadData("", "", "");
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void menu_RemoveOutlet_Click(object sender, EventArgs e)
	{
		try
		{
			if (grd_Outlets.DataSourceTable.IsNotNull())
			{
				gv_Outlets.DeleteDataSourceRowSelected();
				grd_Outlets.DataSourceTable.AcceptChanges();
				UpdateFormView();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void tv_Details_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		try
		{
			if (tv_Details.FocusedNode != null)
			{
				int focusDetail = Function.ParseInt(tv_Details.FocusedNode.GetValue("Id"));
				Function.ParseInt(gv_Emp.GetFocusedRowCellValue("EmpId"));
				SetFocusDetail(focusDetail);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	private void SetFocusDetail(int detailId)
	{
		try
		{
			InsertDataToTableOutlets(grd_Outlets.DataSourceTable.Copy());
			grd_Outlets.DataSourceTable.Rows.Clear();
			grd_Outlets.DataSourceTable.AcceptChanges();
			InsertDataToTableDistributors(grd_Distributors.DataSourceTable.Copy());
			grd_Distributors.DataSourceTable.Rows.Clear();
			grd_Distributors.DataSourceTable.AcceptChanges();
			InsertDataToTableEmployee(grd_Emp.DataSourceTable.Copy());
			grd_Emp.DataSourceTable.Rows.Clear();
			grd_Emp.DataSourceTable.AcceptChanges();
			if (EmployeesData == null || EmployeesData.Columns.Count == 0)
			{
				return;
			}
			DataTable dataTable = EmployeesData.Clone();
			if (detailId == 0)
			{
				grd_Emp.DataSource = dataTable;
				grd_Emp.RefreshDataSource();
				NewEmployeeBlank(-1, detailId);
				return;
			}
			DataRow[] array = EmployeesData.Select($"Id = {detailId}");
			for (int i = 0; i < array.Length; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow.ItemArray = array[i].ItemArray;
				dataTable.Rows.Add(dataRow);
			}
			EmployeesData.AcceptChanges();
			dataTable.AcceptChanges();
			grd_Emp.DataSource = dataTable;
			grd_Emp.RefreshDataSource();
			NewEmployeeBlank(-1, detailId);
			int num = Function.ParseInt(gv_Emp.GetFocusedRowCellValue("EmpId"));
			if (OutletsData == null || OutletsData.Columns.Count == 0)
			{
				return;
			}
			DataTable dataTable2 = OutletsData.Clone();
			if (detailId == 0 || num == 0)
			{
				grd_Outlets.DataSource = dataTable2;
				grd_Outlets.RefreshDataSource();
				return;
			}
			DataRow[] array2 = OutletsData.Select($"Id = {detailId} AND EmpId={num}");
			for (int j = 0; j < array2.Length; j++)
			{
				DataRow dataRow2 = dataTable2.NewRow();
				dataRow2.ItemArray = array2[j].ItemArray;
				dataTable2.Rows.Add(dataRow2);
			}
			OutletsData.AcceptChanges();
			dataTable2.AcceptChanges();
			grd_Outlets.DataSource = dataTable2;
			grd_Outlets.RefreshDataSource();
			NewOutletBlank(-1, detailId, num);
			if (DistributorsData == null || DistributorsData.Columns.Count == 0)
			{
				return;
			}
			DataTable dataTable3 = DistributorsData.Clone();
			if (detailId == 0 || num == 0)
			{
				grd_Distributors.DataSource = dataTable;
				grd_Distributors.RefreshDataSource();
				return;
			}
			DataRow[] array3 = DistributorsData.Select($"Id = {detailId} AND EmpId={num}");
			for (int k = 0; k < array3.Length; k++)
			{
				DataRow dataRow3 = dataTable3.NewRow();
				dataRow3.ItemArray = array3[k].ItemArray;
				dataTable3.Rows.Add(dataRow3);
			}
			DistributorsData.AcceptChanges();
			dataTable3.AcceptChanges();
			grd_Distributors.DataSource = dataTable3;
			grd_Distributors.RefreshDataSource();
			NewDistributorBlank(-1, detailId, num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void NewEmployeeBlank(int position, int detailId)
	{
		try
		{
			DataTable dataSourceTable = grd_Emp.DataSourceTable;
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["Id"] = detailId;
			if (position == -1)
			{
				if (!dataSourceTable.AsEnumerable().Any((DataRow t) => Function.ParseInt(t["EmpId"]) == 0))
				{
					dataSourceTable.Rows.Add(dataRow);
				}
			}
			else
			{
				dataSourceTable.Rows.InsertAt(dataRow, position);
			}
			dataSourceTable.AcceptChanges();
			grd_Emp.DataSource = dataSourceTable;
			grd_Emp.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void InsertDataToTableEmployee(DataTable dt)
	{
		try
		{
			if (EmployeesData != null && EmployeesData.Columns.Count != 0 && dt != null && dt.Rows.Count != 0)
			{
				DataRow[] array = EmployeesData.Select("Id = " + Function.ParseInt(dt.Rows[0]["Id"]));
				for (int i = 0; i < array.Length; i++)
				{
					EmployeesData.Rows.Remove(array[i]);
				}
				for (int j = 0; j < dt.Rows.Count; j++)
				{
					DataRow dataRow = EmployeesData.NewRow();
					dataRow.ItemArray = dt.Rows[j].ItemArray;
					EmployeesData.Rows.Add(dataRow);
				}
				EmployeesData.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void gv_Routes_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			if (e.Button == MouseButtons.Right)
			{
				menu_AddRoute.Enabled = gv_Routes.Columns.Count > 0;
				ToolStripMenuItem toolStripMenuItem = menu_RemoveRoute;
				bool enabled = (menu_DuplicateRoute.Enabled = gv_Routes.RowCount > 0);
				toolStripMenuItem.Enabled = enabled;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void tv_Details_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			if (e.Button == MouseButtons.Right)
			{
				menu_AddDetail.Enabled = gv_Routes.RowCount > 0;
				ToolStripMenuItem toolStripMenuItem = menu_AddSame;
				ToolStripMenuItem toolStripMenuItem2 = menu_Insert;
				bool flag = (menu_Remove.Enabled = tv_Details.Nodes.Count > 0);
				bool enabled = (toolStripMenuItem2.Enabled = flag);
				toolStripMenuItem.Enabled = enabled;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
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
			btn_Search.Enabled = false;
			DataTable dataSource = await _httpClient.SalesRouteClient.GetDataDoc(Function.ToString(btn_RouteCode.EditValue), btn_RouteName.Text, Function.ToString(lk_Status.EditValue));
			grd_Routes.DataSource = dataSource;
			grd_Routes.RefreshDataSource();
			LoadDetails(Function.ToString(gv_Routes.GetRowCellValue(gv_Routes.FocusedRowHandle, gc_RouteCode.FieldName)));
			if (!grd_Routes.DataSourceTable.IsNotNull())
			{
				grd_Outlets.DataSourceTable.Clear();
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		btn_Search.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void gv_Routes_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		UpdateFormView();
	}

	private void tv_Details_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		UpdateFormView();
	}

	private void gv_Outlets_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.Name == gc_OutletCode.Name)
			{
				DataRow focusedDataRow = tv_Details.GetFocusedDataRow();
				GenerateBpMasterInput(isOutlet: true, Function.ParseInt(focusedDataRow["Id"]), Function.ToString(focusedDataRow["RouteCode"]), Function.ParseInt(gv_Emp.GetFocusedRowCellValue("EmpId")));
				menu_AddOutlet_Click(null, null);
			}
			UpdateFormView();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_AddTree_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			AddDetails(TreeLevel.Insert);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Outlets_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			if (e.Button == MouseButtons.Right)
			{
				if (tv_Details.Nodes.Count == 0)
				{
					menu_AddOutlet.Enabled = false;
					menu_RemoveOutlet.Enabled = false;
				}
				else if (tv_Details.Nodes.Count > 0 && Function.ToString(tv_Details.FocusedNode.GetValue("Type")) == "E")
				{
					menu_AddOutlet.Enabled = tv_Details.Nodes.Count > 0;
					menu_RemoveOutlet.Enabled = gv_Outlets.RowCount > 0;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void tv_Details_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			string text = Function.ToString(tv_Details.FocusedNode.GetValue("Type"));
			if (tv_Details.DataSourceTable.IsNotNull() && !string.IsNullOrEmpty(text))
			{
				if (text == "T")
				{
					tc_Code.ColumnEdit = new RepositoryItemTextEdit();
				}
				else
				{
					tc_Code.ColumnEdit = rep_ChooseType;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void rep_Type_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			tv_Details.PostEditor();
			TreeListNode focusedNode = tv_Details.FocusedNode;
			string text = Function.ToString(focusedNode.GetValue("Type"));
			DisableControls(text);
			if (text == "G")
			{
				WinCommons._OpenProcessing();
				int num = 0;
				TreeListNode treeListNode = FindParentNodeGeography(focusedNode);
				if (treeListNode != null)
				{
					num = (_isAddSame ? _levelSameGeography : (Function.ParseInt(treeListNode["LevelGgp"]) + 1));
				}
				tv_Details.SetRowCellValue(focusedNode, tc_LevelGgp, num);
				grd_Outlets.DataSourceTable.Clear();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	private void gv_Routes_ShowingEditor(object sender, CancelEventArgs e)
	{
	}

	private void menu_DuplicateRoute_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				DataRow dataRow = gv_Routes.GetDataRow(gv_Routes.FocusedRowHandle);
				if (dataRow != null)
				{
					DataTable dataTable = grd_Routes.DataSourceTable.Clone();
					dataTable.Rows.Add(dataRow.ItemArray);
					dataTable.AcceptChanges();
					grd_Routes.DataSource = dataTable;
					grd_Routes.RefreshDataSource();
					tv_Details.DataSourceTable.Clear();
					tv_Details.DataSourceTable.AcceptChanges();
					grd_Outlets.DataSourceTable.Clear();
					AddDetails(TreeLevel.Root);
					FormMode = Enums.FormViewModeType.Add;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_OutletCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				if (!string.IsNullOrEmpty(Function.ToString(gv_Outlets.GetRowCellValue(gv_Outlets.FocusedRowHandle, gc_OutletCode))))
				{
					ShowBpMasterData();
				}
			}
			else
			{
				if (string.IsNullOrEmpty(Function.ToString(tv_Details.FocusedNode.GetValue("Code"))))
				{
					WinCommons.Error(Enums.MessageCode.YouMustChooseSaleEmployee, "");
					return;
				}
				DataRow focusedDataRow = tv_Details.GetFocusedDataRow();
				GenerateBpMasterCfl(isOutlet: true, Function.ParseInt(focusedDataRow["Id"]), Function.ToString(gv_Routes.GetFocusedRowCellValue("RouteCode")), Function.ParseInt(gv_Emp.GetFocusedRowCellValue("EmpId")));
				UpdateFormView();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	private void tv_Details_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (tv_Details.DataSourceTable.IsNotNull())
			{
				switch (e.KeyCode)
				{
				case Keys.Insert:
					menu_Insert_Click(null, null);
					break;
				case Keys.Return:
					menu_AddSame_Click(null, null);
					break;
				case Keys.Delete:
					RemoveDetails();
					UpdateFormView();
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void gv_Doc_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			if (e.Button == MouseButtons.Right)
			{
				if (tv_Details.Nodes.Count == 0)
				{
					menu_AddDistributor.Enabled = false;
					menu_RemoveDistributor.Enabled = false;
				}
				else if (tv_Details.Nodes.Count > 0 && Function.ToString(tv_Details.FocusedNode.GetValue("Type")) == "E")
				{
					menu_AddDistributor.Enabled = tv_Details.Nodes.Count > 0;
					menu_RemoveDistributor.Enabled = gv_Distributors.RowCount > 0;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_CardCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				if (!string.IsNullOrEmpty(Function.ToString(gv_Distributors.GetRowCellValue(gv_Distributors.FocusedRowHandle, gc_CardCode))))
				{
					ShowBpMasterData();
				}
			}
			else
			{
				DataRow focusedDataRow = tv_Details.GetFocusedDataRow();
				GenerateBpMasterCfl(isOutlet: false, Function.ParseInt(focusedDataRow["Id"]), Function.ToString(gv_Routes.GetFocusedRowCellValue("RouteCode")), Function.ParseInt(gv_Emp.GetFocusedRowCellValue("EmpId")));
				UpdateFormView();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void menu_AddDistributor_Click(object sender, EventArgs e)
	{
		try
		{
			TreeListNode focusedNode = tv_Details.FocusedNode;
			if (string.IsNullOrEmpty(Function.ToString(focusedNode.GetValue("Code"))))
			{
				WinCommons.Error(Enums.MessageCode.None, "You must choose Sales Employee");
				return;
			}
			AddSubDetails(Function.ParseInt(focusedNode["Id"]), Function.ToString(focusedNode["RouteCode"]), Function.ParseInt(focusedNode["Code"]), isOutlets: false, isNew: false);
			gv_Distributors.FocusedRowHandle = gv_Distributors.RowCount - 1;
			UpdateFormView();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void gv_Distributors_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.Name == gc_CardCode.Name)
			{
				DataRow focusedDataRow = gv_Distributors.GetFocusedDataRow();
				GenerateBpMasterInput(isOutlet: false, Function.ParseInt(focusedDataRow["Id"]), Function.ToString(focusedDataRow["RouteCode"]), Function.ParseInt(gv_Emp.GetFocusedRowCellValue("EmpId")));
			}
			UpdateFormView();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_ChooseType_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(tv_Details.FocusedNode.GetValue("Type"));
			if (e.Button.Index == 0)
			{
				if (text.Equals("G"))
				{
					ShowGeographyForm(Function.ToString(gv_Routes.GetFocusedDataRow()["CountryCode"]), Function.ToString(tv_Details.FocusedNode.GetValue("Code")));
				}
				else
				{
					WinCommons._OpenProcessing();
					frm_HumanResources frm_HumanResources2 = new frm_HumanResources();
					frm_HumanResources2.MdiParent = MdiParent;
					frm_HumanResources2.Show();
				}
			}
			else
			{
				switch (text)
				{
				case "T":
					GenerateDetailData(Enums.TypeCFL.SalesRouteEmployee, tv_Details.FocusedNode, "S");
					break;
				case "G":
				{
					int fatherId = Function.ParseInt(tv_Details.FocusedNode.GetValue("Id"));
					List<Common.ParameterValue> list = new List<Common.ParameterValue>();
					string value = "-1";
					TreeListNode treeListNode = FindParentNodeGeography(tv_Details.FocusedNode);
					if (treeListNode != null)
					{
						value = Function.ToString(treeListNode["Code"]);
					}
					list.Add(new Common.ParameterValue("parentCode", value));
					frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.StoreGeography, false, "", "", Enums.ItemObjectType.NoItemType, list, "");
					frm_CFL2.ShowDialog(this);
					if (frm_CFL2.ChooseItem != null)
					{
						RefreshCode(fatherId);
						tv_Details.FocusedNode.SetValue("Code", frm_CFL2.ChooseItem["Code"]);
						tv_Details.FocusedNode.SetValue("Name", frm_CFL2.ChooseItem["Name"]);
						tv_Details.FocusedNode.SetValue("CountryCode", frm_CFL2.ChooseItem["CountryCode"]);
						gv_Routes.SetFocusedRowCellValue(tc_CountryCode.FieldName, frm_CFL2.ChooseItem["CountryCode"]);
					}
					break;
				}
				case "E":
					GenerateDetailData(Enums.TypeCFL.SalesRouteEmployee, tv_Details.FocusedNode, "E");
					break;
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

	private void RefreshCode(int fatherId = -1)
	{
		try
		{
			List<DataRow> list = (from t in tv_Details.DataSourceTable.AsEnumerable()
				where Function.ParseInt(t["FatherId"]) == fatherId
				select t).ToList();
			if (list.Count > 0)
			{
				for (int num = 0; num < list.Count; num++)
				{
					list[num]["Code"] = "";
					list[num]["Name"] = "";
					RefreshCode(Function.ParseInt(list[num]["Id"]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_Details_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
	{
	}

	private void menu_RemoveDistributor_Click(object sender, EventArgs e)
	{
		try
		{
			if (grd_Distributors.DataSourceTable.IsNotNull())
			{
				gv_Distributors.DeleteDataSourceRowSelected();
				grd_Distributors.DataSourceTable.AcceptChanges();
				UpdateFormView();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void tv_Details_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
	}

	private void gv_Emp_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			int detailId = Function.ParseInt(tv_Details.FocusedNode.GetValue("Id"));
			int empId = Function.ParseInt(gv_Emp.GetFocusedRowCellValue("EmpId"));
			SetFocusEmp(detailId, empId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetFocusEmp(int detailId, int empId)
	{
		try
		{
			InsertDataToTableOutlets(grd_Outlets.DataSourceTable.Copy());
			grd_Outlets.DataSourceTable.Rows.Clear();
			grd_Outlets.DataSourceTable.AcceptChanges();
			if (OutletsData == null || OutletsData.Columns.Count == 0)
			{
				return;
			}
			DataTable dataTable = OutletsData.Clone();
			if (detailId == 0 || empId == 0)
			{
				grd_Outlets.DataSource = dataTable;
				grd_Outlets.RefreshDataSource();
				return;
			}
			DataRow[] array = OutletsData.Select($"Id = {detailId} AND EmpId={empId}");
			for (int i = 0; i < array.Length; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow.ItemArray = array[i].ItemArray;
				dataTable.Rows.Add(dataRow);
			}
			OutletsData.AcceptChanges();
			dataTable.AcceptChanges();
			grd_Outlets.DataSource = dataTable;
			grd_Outlets.RefreshDataSource();
			NewOutletBlank(-1, detailId, empId);
			InsertDataToTableDistributors(grd_Distributors.DataSourceTable.Copy());
			grd_Distributors.DataSourceTable.Rows.Clear();
			grd_Distributors.DataSourceTable.AcceptChanges();
			if (DistributorsData == null || DistributorsData.Columns.Count == 0)
			{
				return;
			}
			DataTable dataTable2 = DistributorsData.Clone();
			if (detailId == 0 || empId == 0)
			{
				grd_Distributors.DataSource = dataTable;
				grd_Distributors.RefreshDataSource();
				return;
			}
			DataRow[] array2 = DistributorsData.Select($"Id = {detailId} AND EmpId={empId}");
			for (int j = 0; j < array2.Length; j++)
			{
				DataRow dataRow2 = dataTable2.NewRow();
				dataRow2.ItemArray = array2[j].ItemArray;
				dataTable2.Rows.Add(dataRow2);
			}
			DistributorsData.AcceptChanges();
			dataTable2.AcceptChanges();
			grd_Distributors.DataSource = dataTable2;
			grd_Distributors.RefreshDataSource();
			NewDistributorBlank(-1, detailId, empId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void NewDistributorBlank(int position, int detailId, int empId)
	{
		try
		{
			DataTable dataTable = grd_Distributors.DataSourceTable;
			if (!dataTable.IsNotNull())
			{
				dataTable = DistributorsData.Clone();
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["Id"] = detailId;
			dataRow["EmpId"] = empId;
			if (position == -1)
			{
				if (!dataTable.AsEnumerable().Any((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["CardCode"]))))
				{
					dataTable.Rows.Add(dataRow);
				}
			}
			else
			{
				dataTable.Rows.InsertAt(dataRow, position);
			}
			dataTable.AcceptChanges();
			grd_Distributors.DataSource = dataTable;
			grd_Distributors.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void InsertDataToTableDistributors(DataTable dt)
	{
		try
		{
			if (DistributorsData != null && DistributorsData.Columns.Count != 0 && dt != null && dt.Rows.Count != 0)
			{
				DataRow[] array = DistributorsData.Select("Id = " + Function.ParseInt(dt.Rows[0]["Id"]) + " AND EmpId=" + Function.ParseInt(dt.Rows[0]["EmpId"]));
				for (int i = 0; i < array.Length; i++)
				{
					DistributorsData.Rows.Remove(array[i]);
				}
				for (int j = 0; j < dt.Rows.Count; j++)
				{
					DataRow dataRow = DistributorsData.NewRow();
					dataRow.ItemArray = dt.Rows[j].ItemArray;
					DistributorsData.Rows.Add(dataRow);
				}
				DistributorsData.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void NewOutletBlank(int position, int detailId, int empId)
	{
		try
		{
			DataTable dataSourceTable = grd_Outlets.DataSourceTable;
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["Id"] = detailId;
			dataRow["EmpId"] = empId;
			if (position == -1)
			{
				if (!dataSourceTable.AsEnumerable().Any((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["CardCode"]))))
				{
					dataSourceTable.Rows.Add(dataRow);
				}
			}
			else
			{
				dataSourceTable.Rows.InsertAt(dataRow, position);
			}
			dataSourceTable.AcceptChanges();
			grd_Outlets.DataSource = dataSourceTable;
			grd_Outlets.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void InsertDataToTableOutlets(DataTable dt)
	{
		try
		{
			if (OutletsData != null && OutletsData.Columns.Count != 0 && dt != null && dt.Rows.Count != 0)
			{
				DataRow[] array = OutletsData.Select("Id = " + Function.ParseInt(dt.Rows[0]["Id"]) + " AND EmpId=" + Function.ParseInt(dt.Rows[0]["EmpId"]));
				for (int i = 0; i < array.Length; i++)
				{
					OutletsData.Rows.Remove(array[i]);
				}
				for (int j = 0; j < dt.Rows.Count; j++)
				{
					DataRow dataRow = OutletsData.NewRow();
					dataRow.ItemArray = dt.Rows[j].ItemArray;
					OutletsData.Rows.Add(dataRow);
				}
				OutletsData.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private async Task gv_Emp_CellValueChangedAsync(DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "EmpId")
			{
				int num = Function.ParseInt(gv_Emp.GetFocusedRowCellValue("EmpId"));
				foreach (DataRow row in grd_Outlets.DataSourceTable.Rows)
				{
					row["EmpId"] = num;
				}
				grd_Outlets.DataSourceTable.AcceptChanges();
				foreach (DataRow row2 in grd_Distributors.DataSourceTable.Rows)
				{
					row2["EmpId"] = num;
				}
				grd_Distributors.DataSourceTable.AcceptChanges();
				NewOutletBlank(-1, Function.ParseInt(tv_Details.FocusedNode.GetValue("Id")), num);
				NewDistributorBlank(-1, Function.ParseInt(tv_Details.FocusedNode.GetValue("Id")), num);
			}
			NewEmployeeBlank(-1, Function.ParseInt(tv_Details.FocusedNode.GetValue("Id")));
			UpdateFormView();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Emp.GetFocusedRowCellValue("Id"));
			int num2 = Function.ParseInt(gv_Emp.GetFocusedRowCellValue("EmpId"));
			DataRow[] array = OutletsData.Select("Id = " + num + " AND EmpId=" + num2);
			for (int i = 0; i < array.Length; i++)
			{
				OutletsData.Rows.Remove(array[i]);
			}
			array = DistributorsData.Select("Id = " + num + " AND EmpId=" + num2);
			for (int j = 0; j < array.Length; j++)
			{
				DistributorsData.Rows.Remove(array[j]);
			}
			grd_Outlets.DataSourceTable.Rows.Clear();
			grd_Outlets.DataSourceTable.AcceptChanges();
			grd_Distributors.DataSourceTable.Rows.Clear();
			grd_Distributors.DataSourceTable.AcceptChanges();
			gv_Emp.DeleteRow(gv_Emp.FocusedRowHandle);
			grd_Emp.DataSourceTable.AcceptChanges();
			UpdateFormView();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.DMS.frm_SalesRoute));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions10 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions11 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions12 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions13 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled17 = new DevExpress.Utils.SerializableAppearanceObject();
		this.lbl_RCode = new Apzon.Controls.Controls.LabelLine();
		this.btn_RouteCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.lbl_RouteName = new Apzon.Controls.Controls.LabelLine();
		this.btn_RouteName = new Apzon.Controls.Editor.ButtonEditEx();
		this.lbl_Status = new Apzon.Controls.Controls.LabelLine();
		this.lk_Status = new Apzon.Controls.Editor.LookUpEditEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.split_Main = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Routes = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Routes = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menu_AddRoute = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_RemoveRoute = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_DuplicateRoute = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Routes = new Apzon.Controls.Grid.GridViewEx();
		this.gc_UserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_RouteCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_RouteName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CountryCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CountryCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_DocStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DocStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_RouteRmarks = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ChannelCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Channel = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_PriceList = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_PriceListDistributor = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_CreateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CreateTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UpdateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UpdateTime = new Apzon.Controls.Grid.GridColumnEx();
		this.split_Details = new DevExpress.XtraEditors.SplitContainerControl();
		this.split_Employee = new DevExpress.XtraEditors.SplitContainerControl();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.tv_Details = new Apzon.Controls.Tree.TreeEx();
		this.tc_Id = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_RouteCode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_Type = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_Type = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.tc_Code = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_ChooseType = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tc_Name = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_CountryCode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_Status1 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_Status1 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.tc_SuperVisor = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_Employee = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.treeListColumnEx8 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_SalesAdmin = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.tc_Remarks1 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx12 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_LevelGgp = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_BaseId = new Apzon.Controls.Tree.TreeListColumnEx();
		this.ctm_Details = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menu_AddDetail = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_AddSame = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_Insert = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.rep_Geography = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.btn_AddTree = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_BP = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_BPCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.grd_Emp = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Emp = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Emp = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SalesEmployee = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.xtraTabControlEx1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPageEx2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Outlets = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Outlet = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menu_AddOutlet = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_RemoveOutlet = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Outlets = new Apzon.Controls.Grid.GridViewEx();
		this.gc_Id = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_RouteCode1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_EmpId = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_OutletCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_OutletCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_OutletName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Address = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_StoreCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_StoreStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.xtraTabPageEx1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Distributors = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Distributors = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menu_AddDistributor = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_RemoveDistributor = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Distributors = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CardCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_CardName = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Remarks1 = new Apzon.Controls.Grid.GridColumnEx();
		this.chk_Active = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.btn_RouteCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_RouteName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lk_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.split_Main).BeginInit();
		this.split_Main.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Routes).BeginInit();
		this.ctm_Routes.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Routes).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CountryCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DocStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Channel).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PriceList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PriceListDistributor).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.split_Details).BeginInit();
		this.split_Details.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.split_Employee).BeginInit();
		this.split_Employee.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tv_Details).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ChooseType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Employee).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SalesAdmin).BeginInit();
		this.ctm_Details.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.rep_Geography).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_AddTree).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BP).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BPCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Emp).BeginInit();
		this.ctm_Emp.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Emp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SalesEmployee).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).BeginInit();
		this.xtraTabControlEx1.SuspendLayout();
		this.xtraTabPageEx2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Outlets).BeginInit();
		this.ctm_Outlet.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Outlets).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_OutletCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_StoreCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_StoreStatus).BeginInit();
		this.xtraTabPageEx1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Distributors).BeginInit();
		this.ctm_Distributors.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Distributors).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CardCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active).BeginInit();
		base.SuspendLayout();
		this.lbl_RCode.EditControl = null;
		this.lbl_RCode.EditValue = "Mã tuyến";
		this.lbl_RCode.FieldName = null;
		this.lbl_RCode.HideWhenEmpty = false;
		this.lbl_RCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_RCode.Location = new System.Drawing.Point(12, 11);
		this.lbl_RCode.Name = "lbl_RCode";
		this.lbl_RCode.ReadOnly = false;
		this.lbl_RCode.SettingInVisible = false;
		this.lbl_RCode.SettingReadonly = false;
		this.lbl_RCode.ShowUnderLine = true;
		this.lbl_RCode.Size = new System.Drawing.Size(90, 20);
		this.lbl_RCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_RCode.TabIndex = 0;
		this.lbl_RCode.Utext = "Mã tuyến";
		this.btn_RouteCode.EditControl = null;
		this.btn_RouteCode.FieldName = null;
		this.btn_RouteCode.IsSearchControl = true;
		this.btn_RouteCode.Location = new System.Drawing.Point(97, 9);
		this.btn_RouteCode.Name = "btn_RouteCode";
		this.btn_RouteCode.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f);
		this.btn_RouteCode.Properties.Appearance.Options.UseFont = true;
		this.btn_RouteCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.btn_RouteCode.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_RouteCode.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.btn_RouteCode.Properties.AppearanceFocused.Options.UseFont = true;
		this.btn_RouteCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btn_RouteCode.SettingInVisible = false;
		this.btn_RouteCode.SettingReadonly = false;
		this.btn_RouteCode.Size = new System.Drawing.Size(140, 20);
		this.btn_RouteCode.TabIndex = 1;
		this.btn_RouteCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_RouteCode_ButtonClick);
		this.lbl_RouteName.EditControl = null;
		this.lbl_RouteName.EditValue = "Tên tuyến";
		this.lbl_RouteName.FieldName = null;
		this.lbl_RouteName.HideWhenEmpty = false;
		this.lbl_RouteName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_RouteName.Location = new System.Drawing.Point(248, 9);
		this.lbl_RouteName.Name = "lbl_RouteName";
		this.lbl_RouteName.ReadOnly = false;
		this.lbl_RouteName.SettingInVisible = false;
		this.lbl_RouteName.SettingReadonly = false;
		this.lbl_RouteName.ShowUnderLine = true;
		this.lbl_RouteName.Size = new System.Drawing.Size(92, 20);
		this.lbl_RouteName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_RouteName.TabIndex = 0;
		this.lbl_RouteName.Utext = "Tên tuyến";
		this.btn_RouteName.EditControl = null;
		this.btn_RouteName.FieldName = null;
		this.btn_RouteName.IsSearchControl = true;
		this.btn_RouteName.Location = new System.Drawing.Point(334, 6);
		this.btn_RouteName.Name = "btn_RouteName";
		this.btn_RouteName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_RouteName.Properties.Appearance.Options.UseFont = true;
		this.btn_RouteName.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.btn_RouteName.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_RouteName.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.btn_RouteName.Properties.AppearanceFocused.Options.UseFont = true;
		this.btn_RouteName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btn_RouteName.SettingInVisible = false;
		this.btn_RouteName.SettingReadonly = false;
		this.btn_RouteName.Size = new System.Drawing.Size(140, 20);
		this.btn_RouteName.TabIndex = 2;
		this.btn_RouteName.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_RouteName_ButtonClick);
		this.lbl_Status.EditControl = null;
		this.lbl_Status.EditValue = "Trạng thái";
		this.lbl_Status.FieldName = null;
		this.lbl_Status.HideWhenEmpty = false;
		this.lbl_Status.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Status.Location = new System.Drawing.Point(12, 34);
		this.lbl_Status.Name = "lbl_Status";
		this.lbl_Status.ReadOnly = false;
		this.lbl_Status.SettingInVisible = false;
		this.lbl_Status.SettingReadonly = false;
		this.lbl_Status.ShowUnderLine = true;
		this.lbl_Status.Size = new System.Drawing.Size(90, 20);
		this.lbl_Status.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Status.TabIndex = 0;
		this.lbl_Status.Utext = "Trạng thái";
		this.lk_Status.EditControl = null;
		this.lk_Status.FieldName = null;
		this.lk_Status.IsSearchControl = true;
		this.lk_Status.Location = new System.Drawing.Point(97, 32);
		this.lk_Status.Name = "lk_Status";
		this.lk_Status.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lk_Status.Properties.Appearance.Options.UseFont = true;
		this.lk_Status.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.lk_Status.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lk_Status.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.lk_Status.Properties.AppearanceFocused.Options.UseFont = true;
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
		this.lk_Status.Size = new System.Drawing.Size(140, 20);
		this.lk_Status.TabIndex = 3;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Tìm kiếm";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = true;
		this.btn_Search.Location = new System.Drawing.Point(399, 32);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 20);
		this.btn_Search.TabIndex = 4;
		this.btn_Search.Text = "Tìm kiếm";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.split_Main.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.split_Main.Horizontal = false;
		this.split_Main.Location = new System.Drawing.Point(5, 64);
		this.split_Main.Name = "split_Main";
		this.split_Main.Panel1.Controls.Add(this.grd_Routes);
		this.split_Main.Panel1.Text = "Panel1";
		this.split_Main.Panel2.Controls.Add(this.split_Details);
		this.split_Main.Panel2.Text = "Panel2";
		this.split_Main.Size = new System.Drawing.Size(1305, 598);
		this.split_Main.SplitterPosition = 152;
		this.split_Main.TabIndex = 5;
		this.split_Main.Text = "splitContainerControl1";
		this.grd_Routes.AllowPasteExcel = true;
		this.grd_Routes.ClipboardData = null;
		this.grd_Routes.ContextMenuStrip = this.ctm_Routes;
		this.grd_Routes.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Routes.Location = new System.Drawing.Point(0, 0);
		this.grd_Routes.MainView = this.gv_Routes;
		this.grd_Routes.Name = "grd_Routes";
		this.grd_Routes.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rep_DocStatus, this.rep_CountryCode, this.rep_Channel, this.rep_PriceList, this.rep_PriceListDistributor });
		this.grd_Routes.Size = new System.Drawing.Size(1305, 152);
		this.grd_Routes.TabIndex = 6;
		this.grd_Routes.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Routes });
		this.ctm_Routes.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.menu_AddRoute, this.menu_RemoveRoute, this.menu_DuplicateRoute });
		this.ctm_Routes.Name = "contextMenuStrip1";
		this.ctm_Routes.Size = new System.Drawing.Size(125, 70);
		this.menu_AddRoute.Name = "menu_AddRoute";
		this.menu_AddRoute.Size = new System.Drawing.Size(124, 22);
		this.menu_AddRoute.Text = "Add New";
		this.menu_AddRoute.Click += new System.EventHandler(this.menu_AddRoute_Click);
		this.menu_RemoveRoute.Name = "menu_RemoveRoute";
		this.menu_RemoveRoute.Size = new System.Drawing.Size(124, 22);
		this.menu_RemoveRoute.Text = "Remove";
		this.menu_RemoveRoute.Click += new System.EventHandler(this.menu_RemoveRoute_Click);
		this.menu_DuplicateRoute.Name = "menu_DuplicateRoute";
		this.menu_DuplicateRoute.Size = new System.Drawing.Size(124, 22);
		this.menu_DuplicateRoute.Text = "Duplicate";
		this.menu_DuplicateRoute.Click += new System.EventHandler(this.menu_DuplicateRoute_Click);
		this.gv_Routes.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Routes.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Routes.AutoFillColumn = this.gc_UserSign;
		this.gv_Routes.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clSalesRoute;
		this.gv_Routes.ColumnKey = null;
		this.gv_Routes.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[13]
		{
			this.gc_RouteCode, this.gc_RouteName, this.gc_CountryCode, this.gc_DocStatus, this.gc_RouteRmarks, this.gc_ChannelCode, this.gridColumnEx6, this.gridColumnEx7, this.gc_UserSign, this.gc_CreateDate,
			this.gc_CreateTime, this.gc_UpdateDate, this.gc_UpdateTime
		});
		this.gv_Routes.DisableFormSetting = false;
		this.gv_Routes.DisplayException = false;
		this.gv_Routes.EditColumn = null;
		this.gv_Routes.FieldCurrency = null;
		this.gv_Routes.FieldRate = null;
		this.gv_Routes.FormId = "oSalesRoute";
		this.gv_Routes.GridControl = this.grd_Routes;
		this.gv_Routes.IgnoreFilter = false;
		this.gv_Routes.IndexEditRow = -1;
		this.gv_Routes.IndicatorWidth = 35;
		this.gv_Routes.IsApzTable = true;
		this.gv_Routes.IsInsertNewRow = false;
		this.gv_Routes.IsLoadingData = false;
		this.gv_Routes.IsMappingContextAction = false;
		this.gv_Routes.ItemId = "APZ_D_OSTR";
		this.gv_Routes.LstColumnAllowBatch = null;
		this.gv_Routes.LstColumnNotAllowBatch = null;
		this.gv_Routes.LstColumnNotEditable = null;
		this.gv_Routes.LstColumNotSetting = null;
		this.gv_Routes.Name = "gv_Routes";
		this.gv_Routes.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Routes.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Routes.OptionsPrint.AutoWidth = false;
		this.gv_Routes.OptionsPrint.UsePrintStyles = false;
		this.gv_Routes.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Routes.OptionsView.ColumnAutoWidth = false;
		this.gv_Routes.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Routes.OptionsView.ShowGroupedColumns = true;
		this.gv_Routes.OptionsView.ShowGroupPanel = false;
		this.gv_Routes.UseBatchMod = false;
		this.gv_Routes.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Routes_ShowingEditor);
		this.gv_Routes.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Routes_FocusedRowObjectChanged);
		this.gv_Routes.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Routes_CellValueChanged);
		this.gv_Routes.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gv_Routes_MouseDown);
		this.gc_UserSign.Caption = "User Sign";
		this.gc_UserSign.CurrencyDateField = null;
		this.gc_UserSign.CurrencyField = null;
		this.gc_UserSign.CurrencyFixed = null;
		this.gc_UserSign.CurrencyFixedRate = 0.0;
		this.gc_UserSign.CurrencyRateField = null;
		this.gc_UserSign.FieldName = "UserSign";
		this.gc_UserSign.Name = "gc_UserSign";
		this.gc_UserSign.OptionsColumn.ReadOnly = true;
		this.gc_UserSign.PrimaryKey = false;
		this.gc_UserSign.UnitField = null;
		this.gc_RouteCode.Caption = "Mã";
		this.gc_RouteCode.CurrencyDateField = null;
		this.gc_RouteCode.CurrencyField = null;
		this.gc_RouteCode.CurrencyFixed = null;
		this.gc_RouteCode.CurrencyFixedRate = 0.0;
		this.gc_RouteCode.CurrencyRateField = null;
		this.gc_RouteCode.FieldName = "RouteCode";
		this.gc_RouteCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gc_RouteCode.Name = "gc_RouteCode";
		this.gc_RouteCode.PrimaryKey = true;
		this.gc_RouteCode.UnitField = null;
		this.gc_RouteCode.Visible = true;
		this.gc_RouteCode.VisibleIndex = 0;
		this.gc_RouteCode.Width = 83;
		this.gc_RouteName.Caption = "Tên";
		this.gc_RouteName.CurrencyDateField = null;
		this.gc_RouteName.CurrencyField = null;
		this.gc_RouteName.CurrencyFixed = null;
		this.gc_RouteName.CurrencyFixedRate = 0.0;
		this.gc_RouteName.CurrencyRateField = null;
		this.gc_RouteName.FieldName = "RouteName";
		this.gc_RouteName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gc_RouteName.Name = "gc_RouteName";
		this.gc_RouteName.PrimaryKey = false;
		this.gc_RouteName.UnitField = null;
		this.gc_RouteName.Visible = true;
		this.gc_RouteName.VisibleIndex = 1;
		this.gc_RouteName.Width = 183;
		this.gc_CountryCode.Caption = "Mã quốc gia";
		this.gc_CountryCode.ColumnEdit = this.rep_CountryCode;
		this.gc_CountryCode.CurrencyDateField = null;
		this.gc_CountryCode.CurrencyField = null;
		this.gc_CountryCode.CurrencyFixed = null;
		this.gc_CountryCode.CurrencyFixedRate = 0.0;
		this.gc_CountryCode.CurrencyRateField = null;
		this.gc_CountryCode.FieldName = "CountryCode";
		this.gc_CountryCode.Name = "gc_CountryCode";
		this.gc_CountryCode.PrimaryKey = false;
		this.gc_CountryCode.UnitField = null;
		this.gc_CountryCode.Visible = true;
		this.gc_CountryCode.VisibleIndex = 2;
		this.gc_CountryCode.Width = 147;
		this.rep_CountryCode.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_CountryCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_CountryCode.Name = "rep_CountryCode";
		this.rep_CountryCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_CountryCode_ButtonClick);
		this.gc_DocStatus.Caption = "Trạng thái";
		this.gc_DocStatus.ColumnEdit = this.rep_DocStatus;
		this.gc_DocStatus.CurrencyDateField = null;
		this.gc_DocStatus.CurrencyField = null;
		this.gc_DocStatus.CurrencyFixed = null;
		this.gc_DocStatus.CurrencyFixedRate = 0.0;
		this.gc_DocStatus.CurrencyRateField = null;
		this.gc_DocStatus.FieldName = "Status";
		this.gc_DocStatus.Name = "gc_DocStatus";
		this.gc_DocStatus.PrimaryKey = false;
		this.gc_DocStatus.UnitField = null;
		this.gc_DocStatus.Visible = true;
		this.gc_DocStatus.VisibleIndex = 3;
		this.rep_DocStatus.AutoHeight = false;
		this.rep_DocStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_DocStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_DocStatus.DisplayMember = "Name";
		this.rep_DocStatus.Name = "rep_DocStatus";
		this.rep_DocStatus.NullText = "";
		this.rep_DocStatus.ShowHeader = false;
		this.rep_DocStatus.ValueMember = "Code";
		this.gc_RouteRmarks.Caption = "Diễn giải";
		this.gc_RouteRmarks.CurrencyDateField = null;
		this.gc_RouteRmarks.CurrencyField = null;
		this.gc_RouteRmarks.CurrencyFixed = null;
		this.gc_RouteRmarks.CurrencyFixedRate = 0.0;
		this.gc_RouteRmarks.CurrencyRateField = null;
		this.gc_RouteRmarks.FieldName = "Remarks";
		this.gc_RouteRmarks.Name = "gc_RouteRmarks";
		this.gc_RouteRmarks.PrimaryKey = false;
		this.gc_RouteRmarks.UnitField = null;
		this.gc_RouteRmarks.Visible = true;
		this.gc_RouteRmarks.VisibleIndex = 4;
		this.gc_RouteRmarks.Width = 252;
		this.gc_ChannelCode.Caption = "Kênh";
		this.gc_ChannelCode.ColumnEdit = this.rep_Channel;
		this.gc_ChannelCode.CurrencyDateField = null;
		this.gc_ChannelCode.CurrencyField = null;
		this.gc_ChannelCode.CurrencyFixed = null;
		this.gc_ChannelCode.CurrencyFixedRate = 0.0;
		this.gc_ChannelCode.CurrencyRateField = null;
		this.gc_ChannelCode.FieldName = "ChannelCode";
		this.gc_ChannelCode.Name = "gc_ChannelCode";
		this.gc_ChannelCode.PrimaryKey = false;
		this.gc_ChannelCode.UnitField = null;
		this.gc_ChannelCode.Visible = true;
		this.gc_ChannelCode.VisibleIndex = 5;
		this.gc_ChannelCode.Width = 125;
		this.rep_Channel.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Channel.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Channel.Name = "rep_Channel";
		this.rep_Channel.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Channel_ButtonClick);
		this.gridColumnEx6.Caption = "Bảng giá đại lý";
		this.gridColumnEx6.ColumnEdit = this.rep_PriceList;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "PriceList";
		this.gridColumnEx6.GroupInterval = DevExpress.XtraGrid.ColumnGroupInterval.Date;
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 6;
		this.gridColumnEx6.Width = 101;
		this.rep_PriceList.AutoHeight = false;
		this.rep_PriceList.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.rep_PriceList.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_PriceList.ContainerControl = this;
		this.rep_PriceList.DisplayMember = "ListName";
		this.rep_PriceList.DisplayMember2 = "";
		this.rep_PriceList.LoadDataSourceWhenEnter = false;
		this.rep_PriceList.Name = "rep_PriceList";
		this.rep_PriceList.SeparateCharacterForMulti = ';';
		this.rep_PriceList.SeparateString = ", ";
		this.rep_PriceList.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.PriceList;
		this.rep_PriceList.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oPriceLists;
		this.rep_PriceList.UsingAutoCompleteBox = false;
		this.rep_PriceList.UsingDataSource = true;
		this.rep_PriceList.ValidateWhenNoDataSource = false;
		this.rep_PriceList.ValueMember = "ListNum";
		this.gridColumnEx7.Caption = "Bảng giá nhà phân phối";
		this.gridColumnEx7.ColumnEdit = this.rep_PriceListDistributor;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "PriceListDistributor";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 7;
		this.gridColumnEx7.Width = 125;
		this.rep_PriceListDistributor.AutoHeight = false;
		this.rep_PriceListDistributor.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.rep_PriceListDistributor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_PriceListDistributor.ContainerControl = this;
		this.rep_PriceListDistributor.DisplayMember = "ListName";
		this.rep_PriceListDistributor.DisplayMember2 = "";
		this.rep_PriceListDistributor.LoadDataSourceWhenEnter = false;
		this.rep_PriceListDistributor.Name = "rep_PriceListDistributor";
		this.rep_PriceListDistributor.SeparateCharacterForMulti = ';';
		this.rep_PriceListDistributor.SeparateString = ", ";
		this.rep_PriceListDistributor.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.PriceList;
		this.rep_PriceListDistributor.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oPriceLists;
		this.rep_PriceListDistributor.UsingAutoCompleteBox = false;
		this.rep_PriceListDistributor.UsingDataSource = true;
		this.rep_PriceListDistributor.ValidateWhenNoDataSource = false;
		this.rep_PriceListDistributor.ValueMember = "ListNum";
		this.gc_CreateDate.Caption = "Create Date";
		this.gc_CreateDate.CurrencyDateField = null;
		this.gc_CreateDate.CurrencyField = null;
		this.gc_CreateDate.CurrencyFixed = null;
		this.gc_CreateDate.CurrencyFixedRate = 0.0;
		this.gc_CreateDate.CurrencyRateField = null;
		this.gc_CreateDate.FieldName = "CreateDate";
		this.gc_CreateDate.Name = "gc_CreateDate";
		this.gc_CreateDate.OptionsColumn.ReadOnly = true;
		this.gc_CreateDate.PrimaryKey = false;
		this.gc_CreateDate.UnitField = null;
		this.gc_CreateTime.Caption = "Create Time";
		this.gc_CreateTime.CurrencyDateField = null;
		this.gc_CreateTime.CurrencyField = null;
		this.gc_CreateTime.CurrencyFixed = null;
		this.gc_CreateTime.CurrencyFixedRate = 0.0;
		this.gc_CreateTime.CurrencyRateField = null;
		this.gc_CreateTime.FieldName = "CreateTime";
		this.gc_CreateTime.Name = "gc_CreateTime";
		this.gc_CreateTime.OptionsColumn.ReadOnly = true;
		this.gc_CreateTime.PrimaryKey = false;
		this.gc_CreateTime.UnitField = null;
		this.gc_UpdateDate.Caption = "Update Date";
		this.gc_UpdateDate.CurrencyDateField = null;
		this.gc_UpdateDate.CurrencyField = null;
		this.gc_UpdateDate.CurrencyFixed = null;
		this.gc_UpdateDate.CurrencyFixedRate = 0.0;
		this.gc_UpdateDate.CurrencyRateField = null;
		this.gc_UpdateDate.FieldName = "UpdateDate";
		this.gc_UpdateDate.Name = "gc_UpdateDate";
		this.gc_UpdateDate.OptionsColumn.ReadOnly = true;
		this.gc_UpdateDate.PrimaryKey = false;
		this.gc_UpdateDate.UnitField = null;
		this.gc_UpdateTime.Caption = "Update Time";
		this.gc_UpdateTime.CurrencyDateField = null;
		this.gc_UpdateTime.CurrencyField = null;
		this.gc_UpdateTime.CurrencyFixed = null;
		this.gc_UpdateTime.CurrencyFixedRate = 0.0;
		this.gc_UpdateTime.CurrencyRateField = null;
		this.gc_UpdateTime.FieldName = "UpdateTime";
		this.gc_UpdateTime.Name = "gc_UpdateTime";
		this.gc_UpdateTime.OptionsColumn.ReadOnly = true;
		this.gc_UpdateTime.PrimaryKey = false;
		this.gc_UpdateTime.UnitField = null;
		this.split_Details.Dock = System.Windows.Forms.DockStyle.Fill;
		this.split_Details.Location = new System.Drawing.Point(0, 0);
		this.split_Details.Name = "split_Details";
		this.split_Details.Panel1.Controls.Add(this.split_Employee);
		this.split_Details.Panel1.Text = "Panel1";
		this.split_Details.Panel2.Controls.Add(this.xtraTabControlEx1);
		this.split_Details.Panel2.Text = "Panel2";
		this.split_Details.Size = new System.Drawing.Size(1305, 436);
		this.split_Details.SplitterPosition = 923;
		this.split_Details.TabIndex = 7;
		this.split_Details.Text = "splitContainerControl2";
		this.split_Employee.Dock = System.Windows.Forms.DockStyle.Fill;
		this.split_Employee.Location = new System.Drawing.Point(0, 0);
		this.split_Employee.Name = "split_Employee";
		this.split_Employee.Panel1.Controls.Add(this.groupControl1);
		this.split_Employee.Panel1.Text = "Panel1";
		this.split_Employee.Panel2.Controls.Add(this.grd_Emp);
		this.split_Employee.Panel2.Text = "Panel2";
		this.split_Employee.Size = new System.Drawing.Size(923, 436);
		this.split_Employee.SplitterPosition = 568;
		this.split_Employee.TabIndex = 9;
		this.groupControl1.Controls.Add(this.tv_Details);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(568, 436);
		this.groupControl1.TabIndex = 8;
		this.groupControl1.Text = "Chi tiết";
		this.tv_Details.AllowPasteExcel = false;
		this.tv_Details.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clSalesRoute;
		this.tv_Details.ColumnKey = null;
		this.tv_Details.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[13]
		{
			this.tc_Id, this.tc_RouteCode, this.tc_Type, this.tc_Code, this.tc_Name, this.tc_CountryCode, this.tc_Status1, this.tc_SuperVisor, this.treeListColumnEx8, this.tc_Remarks1,
			this.treeListColumnEx12, this.tc_LevelGgp, this.tc_BaseId
		});
		this.tv_Details.ContextMenuStrip = this.ctm_Details;
		this.tv_Details.Cursor = System.Windows.Forms.Cursors.Default;
		this.tv_Details.DisableFormSetting = false;
		this.tv_Details.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tv_Details.EditColumn = null;
		this.tv_Details.FieldCurrency = null;
		this.tv_Details.FieldRate = null;
		this.tv_Details.FormId = "oSalesRoute";
		this.tv_Details.HorzScrollVisibility = DevExpress.XtraTreeList.ScrollVisibility.Always;
		this.tv_Details.IndexEditRow = -1;
		this.tv_Details.IndicatorWidth = 40;
		this.tv_Details.IsApzTable = true;
		this.tv_Details.IsLoadingData = false;
		this.tv_Details.ItemId = "APZ_D_STR1";
		this.tv_Details.KeyFieldName = "Id";
		this.tv_Details.Location = new System.Drawing.Point(2, 23);
		this.tv_Details.LstColumnNotEditable = null;
		this.tv_Details.LstColumNotSetting = null;
		this.tv_Details.Name = "tv_Details";
		this.tv_Details.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
		this.tv_Details.OptionsMenu.ShowExpandCollapseItems = false;
		this.tv_Details.OptionsNavigation.AutoMoveRowFocus = true;
		this.tv_Details.OptionsView.AutoWidth = false;
		this.tv_Details.ParentFieldName = "FatherId";
		this.tv_Details.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[9] { this.rep_Employee, this.rep_Geography, this.rep_Status1, this.btn_AddTree, this.rep_Type, this.rep_BP, this.rep_BPCode, this.rep_ChooseType, this.rep_SalesAdmin });
		this.tv_Details.Size = new System.Drawing.Size(564, 411);
		this.tv_Details.TabIndex = 9;
		this.tv_Details.VertScrollVisibility = DevExpress.XtraTreeList.ScrollVisibility.Always;
		this.tv_Details.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_Details_CellValueChanged);
		this.tv_Details.CustomNodeCellEdit += new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(this.tv_Details_CustomNodeCellEdit);
		this.tv_Details.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tv_Details_FocusedNodeChanged);
		this.tv_Details.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(this.tv_Details_PopupMenuShowing);
		this.tv_Details.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.tv_Details_ShowingEditor);
		this.tv_Details.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tv_Details_KeyDown);
		this.tv_Details.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tv_Details_MouseDown);
		this.tc_Id.Caption = "Id";
		this.tc_Id.CurrencyDateField = null;
		this.tc_Id.CurrencyField = null;
		this.tc_Id.CurrencyFixed = null;
		this.tc_Id.CurrencyFixedRate = 0.0;
		this.tc_Id.CurrencyRateField = null;
		this.tc_Id.FieldName = "Id";
		this.tc_Id.Name = "tc_Id";
		this.tc_Id.OptionsColumn.AllowEdit = false;
		this.tc_Id.OptionsColumn.ReadOnly = true;
		this.tc_RouteCode.Caption = "Route Code";
		this.tc_RouteCode.CurrencyDateField = null;
		this.tc_RouteCode.CurrencyField = null;
		this.tc_RouteCode.CurrencyFixed = null;
		this.tc_RouteCode.CurrencyFixedRate = 0.0;
		this.tc_RouteCode.CurrencyRateField = null;
		this.tc_RouteCode.FieldName = "RouteCode";
		this.tc_RouteCode.Name = "tc_RouteCode";
		this.tc_RouteCode.OptionsColumn.ReadOnly = true;
		this.tc_Type.Caption = "Loại";
		this.tc_Type.ColumnEdit = this.rep_Type;
		this.tc_Type.CurrencyDateField = null;
		this.tc_Type.CurrencyField = null;
		this.tc_Type.CurrencyFixed = null;
		this.tc_Type.CurrencyFixedRate = 0.0;
		this.tc_Type.CurrencyRateField = null;
		this.tc_Type.FieldName = "Type";
		this.tc_Type.Name = "tc_Type";
		this.tc_Type.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tc_Type.Visible = true;
		this.tc_Type.VisibleIndex = 0;
		this.tc_Type.Width = 199;
		this.rep_Type.AppearanceDropDown.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.rep_Type.AppearanceDropDown.Options.UseBackColor = true;
		this.rep_Type.AutoHeight = false;
		this.rep_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Type.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_Type.DisplayMember = "Name";
		this.rep_Type.Name = "rep_Type";
		this.rep_Type.NullText = "";
		this.rep_Type.ShowHeader = false;
		this.rep_Type.ValueMember = "Code";
		this.rep_Type.EditValueChanged += new System.EventHandler(this.rep_Type_EditValueChanged);
		this.tc_Code.Caption = "Mã";
		this.tc_Code.ColumnEdit = this.rep_ChooseType;
		this.tc_Code.CurrencyDateField = null;
		this.tc_Code.CurrencyField = null;
		this.tc_Code.CurrencyFixed = null;
		this.tc_Code.CurrencyFixedRate = 0.0;
		this.tc_Code.CurrencyRateField = null;
		this.tc_Code.FieldName = "Code";
		this.tc_Code.Name = "tc_Code";
		this.tc_Code.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowForFocusedRow;
		this.tc_Code.Visible = true;
		this.tc_Code.VisibleIndex = 1;
		this.tc_Code.Width = 138;
		this.rep_ChooseType.AutoHeight = false;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_ChooseType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_ChooseType.Name = "rep_ChooseType";
		this.rep_ChooseType.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_ChooseType_ButtonClick);
		this.tc_Name.Caption = "Tên";
		this.tc_Name.CurrencyDateField = null;
		this.tc_Name.CurrencyField = null;
		this.tc_Name.CurrencyFixed = null;
		this.tc_Name.CurrencyFixedRate = 0.0;
		this.tc_Name.CurrencyRateField = null;
		this.tc_Name.FieldName = "Name";
		this.tc_Name.Name = "tc_Name";
		this.tc_Name.Visible = true;
		this.tc_Name.VisibleIndex = 2;
		this.tc_Name.Width = 197;
		this.tc_CountryCode.Caption = "Country Code";
		this.tc_CountryCode.CurrencyDateField = null;
		this.tc_CountryCode.CurrencyField = null;
		this.tc_CountryCode.CurrencyFixed = null;
		this.tc_CountryCode.CurrencyFixedRate = 0.0;
		this.tc_CountryCode.CurrencyRateField = null;
		this.tc_CountryCode.FieldName = "CountryCode";
		this.tc_CountryCode.Name = "tc_CountryCode";
		this.tc_CountryCode.OptionsColumn.ReadOnly = true;
		this.tc_CountryCode.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tc_Status1.Caption = "Trạng thái";
		this.tc_Status1.ColumnEdit = this.rep_Status1;
		this.tc_Status1.CurrencyDateField = null;
		this.tc_Status1.CurrencyField = null;
		this.tc_Status1.CurrencyFixed = null;
		this.tc_Status1.CurrencyFixedRate = 0.0;
		this.tc_Status1.CurrencyRateField = null;
		this.tc_Status1.FieldName = "Status";
		this.tc_Status1.Name = "tc_Status1";
		this.tc_Status1.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tc_Status1.Visible = true;
		this.tc_Status1.VisibleIndex = 4;
		this.tc_Status1.Width = 106;
		this.rep_Status1.AutoHeight = false;
		this.rep_Status1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status1.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_Status1.DisplayMember = "Name";
		this.rep_Status1.Name = "rep_Status1";
		this.rep_Status1.NullText = "";
		this.rep_Status1.ShowHeader = false;
		this.rep_Status1.ValueMember = "Code";
		this.tc_SuperVisor.Caption = "Giám sát kinh doanh";
		this.tc_SuperVisor.ColumnEdit = this.rep_Employee;
		this.tc_SuperVisor.CurrencyDateField = null;
		this.tc_SuperVisor.CurrencyField = null;
		this.tc_SuperVisor.CurrencyFixed = null;
		this.tc_SuperVisor.CurrencyFixedRate = 0.0;
		this.tc_SuperVisor.CurrencyRateField = null;
		this.tc_SuperVisor.FieldName = "SuperVisor";
		this.tc_SuperVisor.Name = "tc_SuperVisor";
		this.tc_SuperVisor.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tc_SuperVisor.Visible = true;
		this.tc_SuperVisor.VisibleIndex = 3;
		this.tc_SuperVisor.Width = 130;
		this.rep_Employee.AutoHeight = false;
		this.rep_Employee.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Employee.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Employee.ContainerControl = this;
		this.rep_Employee.DisplayMember = "";
		this.rep_Employee.DisplayMember2 = "";
		this.rep_Employee.LoadDataSourceWhenEnter = false;
		this.rep_Employee.Name = "rep_Employee";
		this.rep_Employee.SeparateCharacterForMulti = ';';
		this.rep_Employee.SeparateString = ", ";
		this.rep_Employee.UsingAutoCompleteBox = false;
		this.rep_Employee.UsingDataSource = true;
		this.rep_Employee.ValidateWhenNoDataSource = false;
		this.rep_Employee.ValueMember = "";
		this.treeListColumnEx8.Caption = "Trợ lý kinh doanh";
		this.treeListColumnEx8.ColumnEdit = this.rep_SalesAdmin;
		this.treeListColumnEx8.CurrencyDateField = null;
		this.treeListColumnEx8.CurrencyField = null;
		this.treeListColumnEx8.CurrencyFixed = null;
		this.treeListColumnEx8.CurrencyFixedRate = 0.0;
		this.treeListColumnEx8.CurrencyRateField = null;
		this.treeListColumnEx8.FieldName = "SalesAdmin";
		this.treeListColumnEx8.Name = "treeListColumnEx8";
		this.treeListColumnEx8.Visible = true;
		this.treeListColumnEx8.VisibleIndex = 5;
		this.rep_SalesAdmin.AutoHeight = false;
		this.rep_SalesAdmin.AutoLoadDataSource = true;
		this.rep_SalesAdmin.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_SalesAdmin.ContainerControl = this;
		this.rep_SalesAdmin.DisplayMember = "fullName";
		this.rep_SalesAdmin.DisplayMember2 = "";
		this.rep_SalesAdmin.LoadDataSourceWhenEnter = false;
		this.rep_SalesAdmin.Name = "rep_SalesAdmin";
		this.rep_SalesAdmin.SeparateCharacterForMulti = ';';
		this.rep_SalesAdmin.SeparateString = ", ";
		this.rep_SalesAdmin.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Employees;
		this.rep_SalesAdmin.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oEmployeesInfo;
		this.rep_SalesAdmin.UsingAutoCompleteBox = false;
		this.rep_SalesAdmin.UsingDataSource = true;
		this.rep_SalesAdmin.ValidateWhenNoDataSource = false;
		this.rep_SalesAdmin.ValueMember = "empID";
		this.tc_Remarks1.Caption = "Diễn giải";
		this.tc_Remarks1.CurrencyDateField = null;
		this.tc_Remarks1.CurrencyField = null;
		this.tc_Remarks1.CurrencyFixed = null;
		this.tc_Remarks1.CurrencyFixedRate = 0.0;
		this.tc_Remarks1.CurrencyRateField = null;
		this.tc_Remarks1.FieldName = "Remarks";
		this.tc_Remarks1.Name = "tc_Remarks1";
		this.tc_Remarks1.Visible = true;
		this.tc_Remarks1.VisibleIndex = 6;
		this.tc_Remarks1.Width = 196;
		this.treeListColumnEx12.Caption = "FatherId";
		this.treeListColumnEx12.CurrencyDateField = null;
		this.treeListColumnEx12.CurrencyField = null;
		this.treeListColumnEx12.CurrencyFixed = null;
		this.treeListColumnEx12.CurrencyFixedRate = 0.0;
		this.treeListColumnEx12.CurrencyRateField = null;
		this.treeListColumnEx12.FieldName = "FatherId";
		this.treeListColumnEx12.Name = "treeListColumnEx12";
		this.treeListColumnEx12.OptionsColumn.AllowEdit = false;
		this.treeListColumnEx12.OptionsColumn.ReadOnly = true;
		this.tc_LevelGgp.Caption = "LevelGgp";
		this.tc_LevelGgp.CurrencyDateField = null;
		this.tc_LevelGgp.CurrencyField = null;
		this.tc_LevelGgp.CurrencyFixed = null;
		this.tc_LevelGgp.CurrencyFixedRate = 0.0;
		this.tc_LevelGgp.CurrencyRateField = null;
		this.tc_LevelGgp.FieldName = "LevelGgp";
		this.tc_LevelGgp.Name = "tc_LevelGgp";
		this.tc_LevelGgp.OptionsColumn.AllowEdit = false;
		this.tc_LevelGgp.OptionsColumn.ReadOnly = true;
		this.tc_BaseId.Caption = "treeListColumnEx8";
		this.tc_BaseId.CurrencyDateField = null;
		this.tc_BaseId.CurrencyField = null;
		this.tc_BaseId.CurrencyFixed = null;
		this.tc_BaseId.CurrencyFixedRate = 0.0;
		this.tc_BaseId.CurrencyRateField = null;
		this.tc_BaseId.FieldName = "BaseId";
		this.tc_BaseId.Name = "tc_BaseId";
		this.ctm_Details.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.menu_AddDetail, this.menu_AddSame, this.menu_Insert, this.menu_Remove });
		this.ctm_Details.Name = "contextMenuStrip2";
		this.ctm_Details.Size = new System.Drawing.Size(142, 92);
		this.menu_AddDetail.Name = "menu_AddDetail";
		this.menu_AddDetail.Size = new System.Drawing.Size(141, 22);
		this.menu_AddDetail.Text = "Add New";
		this.menu_AddDetail.Click += new System.EventHandler(this.menu_AddDetail_Click);
		this.menu_AddSame.Name = "menu_AddSame";
		this.menu_AddSame.Size = new System.Drawing.Size(141, 22);
		this.menu_AddSame.Text = "Add Same";
		this.menu_AddSame.Click += new System.EventHandler(this.menu_AddSame_Click);
		this.menu_Insert.Name = "menu_Insert";
		this.menu_Insert.ShortcutKeys = System.Windows.Forms.Keys.Insert;
		this.menu_Insert.Size = new System.Drawing.Size(141, 22);
		this.menu_Insert.Text = "Insert";
		this.menu_Insert.Click += new System.EventHandler(this.menu_Insert_Click);
		this.menu_Remove.Name = "menu_Remove";
		this.menu_Remove.ShortcutKeys = System.Windows.Forms.Keys.Delete;
		this.menu_Remove.Size = new System.Drawing.Size(141, 22);
		this.menu_Remove.Text = "Remove";
		this.menu_Remove.Click += new System.EventHandler(this.menu_Remove_Click);
		this.rep_Geography.AutoHeight = false;
		editorButtonImageOptions7.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Geography.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Geography.Name = "rep_Geography";
		this.rep_Geography.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Geography_ButtonClick);
		this.btn_AddTree.AutoHeight = false;
		this.btn_AddTree.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
		});
		this.btn_AddTree.Name = "btn_AddTree";
		this.btn_AddTree.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_AddTree_ButtonClick);
		this.rep_BP.AutoHeight = false;
		editorButtonImageOptions8.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_BP.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance11, appearanceHovered11, appearancePressed11, appearanceDisabled11, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_BP.Name = "rep_BP";
		this.rep_BPCode.AutoHeight = false;
		editorButtonImageOptions9.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_BPCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis, "", -1, true, true, true, editorButtonImageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance12, appearanceHovered12, appearancePressed12, appearanceDisabled12, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_BPCode.Name = "rep_BPCode";
		this.grd_Emp.AllowPasteExcel = false;
		this.grd_Emp.ClipboardData = null;
		this.grd_Emp.ContextMenuStrip = this.ctm_Emp;
		this.grd_Emp.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Emp.Location = new System.Drawing.Point(0, 0);
		this.grd_Emp.MainView = this.gv_Emp;
		this.grd_Emp.Name = "grd_Emp";
		this.grd_Emp.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_SalesEmployee });
		this.grd_Emp.Size = new System.Drawing.Size(345, 436);
		this.grd_Emp.TabIndex = 0;
		this.grd_Emp.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Emp });
		this.ctm_Emp.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRowToolStripMenuItem, this.removeToolStripMenuItem });
		this.ctm_Emp.Name = "contextMenuStrip5";
		this.ctm_Emp.Size = new System.Drawing.Size(123, 48);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_Emp.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Emp.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Emp.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clSalesRoute;
		this.gv_Emp.ColumnKey = null;
		this.gv_Emp.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this.gridColumnEx5 });
		this.gv_Emp.DisableFormSetting = false;
		this.gv_Emp.DisplayException = false;
		this.gv_Emp.EditColumn = null;
		this.gv_Emp.FieldCurrency = null;
		this.gv_Emp.FieldRate = null;
		this.gv_Emp.FormId = null;
		this.gv_Emp.GridControl = this.grd_Emp;
		this.gv_Emp.IgnoreFilter = false;
		this.gv_Emp.IndexEditRow = -1;
		this.gv_Emp.IndicatorWidth = 35;
		this.gv_Emp.IsApzTable = true;
		this.gv_Emp.IsInsertNewRow = false;
		this.gv_Emp.IsLoadingData = false;
		this.gv_Emp.IsMappingContextAction = false;
		this.gv_Emp.ItemId = null;
		this.gv_Emp.LstColumnAllowBatch = null;
		this.gv_Emp.LstColumnNotAllowBatch = null;
		this.gv_Emp.LstColumnNotEditable = null;
		this.gv_Emp.LstColumNotSetting = null;
		this.gv_Emp.Name = "gv_Emp";
		this.gv_Emp.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Emp.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Emp.OptionsPrint.AutoWidth = false;
		this.gv_Emp.OptionsPrint.UsePrintStyles = false;
		this.gv_Emp.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Emp.OptionsView.ColumnAutoWidth = false;
		this.gv_Emp.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Emp.OptionsView.ShowGroupedColumns = true;
		this.gv_Emp.OptionsView.ShowGroupPanel = false;
		this.gv_Emp.UseBatchMod = false;
		this.gv_Emp.CellValueChangedAsync += new Apzon.Controls.Grid.CellValueChangedAsync(this.gv_Emp_CellValueChangedAsync);
		this.gv_Emp.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Emp_FocusedRowObjectChanged);
		this.gridColumnEx5.Caption = "Nhân viên kinh doanh";
		this.gridColumnEx5.ColumnEdit = this.rep_SalesEmployee;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "EmpId";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 0;
		this.gridColumnEx5.Width = 284;
		this.rep_SalesEmployee.AutoHeight = false;
		this.rep_SalesEmployee.AutoLoadDataSource = true;
		editorButtonImageOptions10.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_SalesEmployee.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance13, appearanceHovered13, appearancePressed13, appearanceDisabled13, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions10, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance14, appearanceHovered14, appearancePressed14, appearanceDisabled14, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_SalesEmployee.ContainerControl = this;
		this.rep_SalesEmployee.DisplayMember = "fullName";
		this.rep_SalesEmployee.DisplayMember2 = "";
		this.rep_SalesEmployee.LoadDataSourceWhenEnter = false;
		this.rep_SalesEmployee.Name = "rep_SalesEmployee";
		this.rep_SalesEmployee.SeparateCharacterForMulti = ';';
		this.rep_SalesEmployee.SeparateString = ", ";
		this.rep_SalesEmployee.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Employees;
		this.rep_SalesEmployee.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oEmployeesInfo;
		this.rep_SalesEmployee.UsingAutoCompleteBox = false;
		this.rep_SalesEmployee.UsingDataSource = true;
		this.rep_SalesEmployee.ValidateWhenNoDataSource = false;
		this.rep_SalesEmployee.ValueMember = "empID";
		this.xtraTabControlEx1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControlEx1.IsSearchControl = false;
		this.xtraTabControlEx1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControlEx1.Name = "xtraTabControlEx1";
		this.xtraTabControlEx1.SelectedTabPage = this.xtraTabPageEx2;
		this.xtraTabControlEx1.Size = new System.Drawing.Size(372, 436);
		this.xtraTabControlEx1.TabIndex = 0;
		this.xtraTabControlEx1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPageEx2, this.xtraTabPageEx1 });
		this.xtraTabPageEx2.Controls.Add(this.grd_Outlets);
		this.xtraTabPageEx2.EditControl = null;
		this.xtraTabPageEx2.EditValue = "Cửa hàng/ Đại lý";
		this.xtraTabPageEx2.FieldName = "";
		this.xtraTabPageEx2.IsSearchControl = false;
		this.xtraTabPageEx2.Name = "xtraTabPageEx2";
		this.xtraTabPageEx2.ReadOnly = false;
		this.xtraTabPageEx2.SettingInVisible = false;
		this.xtraTabPageEx2.SettingReadonly = false;
		this.xtraTabPageEx2.Size = new System.Drawing.Size(370, 411);
		this.xtraTabPageEx2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageEx2.Text = "Cửa hàng/ Đại lý";
		this.grd_Outlets.AllowPasteExcel = true;
		this.grd_Outlets.ClipboardData = null;
		this.grd_Outlets.ContextMenuStrip = this.ctm_Outlet;
		this.grd_Outlets.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Outlets.Location = new System.Drawing.Point(0, 0);
		this.grd_Outlets.MainView = this.gv_Outlets;
		this.grd_Outlets.Name = "grd_Outlets";
		this.grd_Outlets.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_StoreCode, this.rep_StoreStatus, this.rep_OutletCode });
		this.grd_Outlets.Size = new System.Drawing.Size(370, 411);
		this.grd_Outlets.TabIndex = 13;
		this.grd_Outlets.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Outlets });
		this.ctm_Outlet.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.menu_AddOutlet, this.menu_RemoveOutlet });
		this.ctm_Outlet.Name = "contextMenuStrip3";
		this.ctm_Outlet.Size = new System.Drawing.Size(124, 48);
		this.menu_AddOutlet.Name = "menu_AddOutlet";
		this.menu_AddOutlet.Size = new System.Drawing.Size(123, 22);
		this.menu_AddOutlet.Text = "Add New";
		this.menu_AddOutlet.Click += new System.EventHandler(this.menu_AddOutlet_Click);
		this.menu_RemoveOutlet.Name = "menu_RemoveOutlet";
		this.menu_RemoveOutlet.Size = new System.Drawing.Size(123, 22);
		this.menu_RemoveOutlet.Text = "Remove";
		this.menu_RemoveOutlet.Click += new System.EventHandler(this.menu_RemoveOutlet_Click);
		this.gv_Outlets.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Outlets.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Outlets.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clSalesRoute;
		this.gv_Outlets.ColumnKey = null;
		this.gv_Outlets.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gc_Id, this.gc_RouteCode1, this.gc_EmpId, this.gc_OutletCode, this.gc_OutletName, this.gc_Address, this.gc_Remarks });
		this.gv_Outlets.DisableFormSetting = false;
		this.gv_Outlets.DisplayException = false;
		this.gv_Outlets.EditColumn = null;
		this.gv_Outlets.FieldCurrency = null;
		this.gv_Outlets.FieldRate = null;
		this.gv_Outlets.FormId = "oSalesRoute";
		this.gv_Outlets.GridControl = this.grd_Outlets;
		this.gv_Outlets.IgnoreFilter = false;
		this.gv_Outlets.IndexEditRow = -1;
		this.gv_Outlets.IndicatorWidth = 35;
		this.gv_Outlets.IsApzTable = true;
		this.gv_Outlets.IsInsertNewRow = false;
		this.gv_Outlets.IsLoadingData = false;
		this.gv_Outlets.IsMappingContextAction = false;
		this.gv_Outlets.ItemId = "APZ_D_STR2";
		this.gv_Outlets.LstColumnAllowBatch = null;
		this.gv_Outlets.LstColumnNotAllowBatch = null;
		this.gv_Outlets.LstColumnNotEditable = null;
		this.gv_Outlets.LstColumNotSetting = null;
		this.gv_Outlets.Name = "gv_Outlets";
		this.gv_Outlets.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Outlets.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Outlets.OptionsNavigation.AutoFocusNewRow = true;
		this.gv_Outlets.OptionsPrint.AutoWidth = false;
		this.gv_Outlets.OptionsPrint.UsePrintStyles = false;
		this.gv_Outlets.OptionsSelection.MultiSelect = true;
		this.gv_Outlets.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Outlets.OptionsView.ColumnAutoWidth = false;
		this.gv_Outlets.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Outlets.OptionsView.ShowGroupedColumns = true;
		this.gv_Outlets.OptionsView.ShowGroupPanel = false;
		this.gv_Outlets.UseBatchMod = false;
		this.gv_Outlets.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Outlets_CellValueChanged);
		this.gv_Outlets.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gv_Outlets_MouseDown);
		this.gc_Id.Caption = "Id";
		this.gc_Id.CurrencyDateField = null;
		this.gc_Id.CurrencyField = null;
		this.gc_Id.CurrencyFixed = null;
		this.gc_Id.CurrencyFixedRate = 0.0;
		this.gc_Id.CurrencyRateField = null;
		this.gc_Id.FieldName = "Id";
		this.gc_Id.Name = "gc_Id";
		this.gc_Id.OptionsColumn.AllowEdit = false;
		this.gc_Id.OptionsColumn.ReadOnly = true;
		this.gc_Id.PrimaryKey = true;
		this.gc_Id.UnitField = null;
		this.gc_RouteCode1.Caption = "Route Code";
		this.gc_RouteCode1.CurrencyDateField = null;
		this.gc_RouteCode1.CurrencyField = null;
		this.gc_RouteCode1.CurrencyFixed = null;
		this.gc_RouteCode1.CurrencyFixedRate = 0.0;
		this.gc_RouteCode1.CurrencyRateField = null;
		this.gc_RouteCode1.FieldName = "RouteCode";
		this.gc_RouteCode1.Name = "gc_RouteCode1";
		this.gc_RouteCode1.OptionsColumn.AllowEdit = false;
		this.gc_RouteCode1.OptionsColumn.ReadOnly = true;
		this.gc_RouteCode1.PrimaryKey = false;
		this.gc_RouteCode1.UnitField = null;
		this.gc_EmpId.Caption = "Employee Id";
		this.gc_EmpId.CurrencyDateField = null;
		this.gc_EmpId.CurrencyField = null;
		this.gc_EmpId.CurrencyFixed = null;
		this.gc_EmpId.CurrencyFixedRate = 0.0;
		this.gc_EmpId.CurrencyRateField = null;
		this.gc_EmpId.FieldName = "EmpId";
		this.gc_EmpId.Name = "gc_EmpId";
		this.gc_EmpId.OptionsColumn.AllowEdit = false;
		this.gc_EmpId.OptionsColumn.ReadOnly = true;
		this.gc_EmpId.PrimaryKey = false;
		this.gc_EmpId.UnitField = null;
		this.gc_OutletCode.Caption = "Mã cửa hàng";
		this.gc_OutletCode.ColumnEdit = this.rep_OutletCode;
		this.gc_OutletCode.CurrencyDateField = null;
		this.gc_OutletCode.CurrencyField = null;
		this.gc_OutletCode.CurrencyFixed = null;
		this.gc_OutletCode.CurrencyFixedRate = 0.0;
		this.gc_OutletCode.CurrencyRateField = null;
		this.gc_OutletCode.FieldName = "CardCode";
		this.gc_OutletCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gc_OutletCode.Name = "gc_OutletCode";
		this.gc_OutletCode.PrimaryKey = false;
		this.gc_OutletCode.UnitField = null;
		this.gc_OutletCode.Visible = true;
		this.gc_OutletCode.VisibleIndex = 0;
		this.gc_OutletCode.Width = 100;
		this.rep_OutletCode.AutoHeight = false;
		editorButtonImageOptions11.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_OutletCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions11, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance15, appearanceHovered15, appearancePressed15, appearanceDisabled15, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_OutletCode.Name = "rep_OutletCode";
		this.rep_OutletCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_OutletCode_ButtonClick);
		this.gc_OutletName.Caption = "Tên cửa hàng";
		this.gc_OutletName.CurrencyDateField = null;
		this.gc_OutletName.CurrencyField = null;
		this.gc_OutletName.CurrencyFixed = null;
		this.gc_OutletName.CurrencyFixedRate = 0.0;
		this.gc_OutletName.CurrencyRateField = null;
		this.gc_OutletName.FieldName = "CardName";
		this.gc_OutletName.Name = "gc_OutletName";
		this.gc_OutletName.PrimaryKey = false;
		this.gc_OutletName.UnitField = null;
		this.gc_OutletName.Visible = true;
		this.gc_OutletName.VisibleIndex = 1;
		this.gc_OutletName.Width = 125;
		this.gc_Address.Caption = "Địa chỉ";
		this.gc_Address.CurrencyDateField = null;
		this.gc_Address.CurrencyField = null;
		this.gc_Address.CurrencyFixed = null;
		this.gc_Address.CurrencyFixedRate = 0.0;
		this.gc_Address.CurrencyRateField = null;
		this.gc_Address.FieldName = "Address";
		this.gc_Address.Name = "gc_Address";
		this.gc_Address.PrimaryKey = false;
		this.gc_Address.UnitField = null;
		this.gc_Address.Visible = true;
		this.gc_Address.VisibleIndex = 2;
		this.gc_Address.Width = 95;
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
		this.gc_Remarks.Width = 98;
		this.rep_StoreCode.AutoHeight = false;
		editorButtonImageOptions12.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_StoreCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions12, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance16, appearanceHovered16, appearancePressed16, appearanceDisabled16, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_StoreCode.Name = "rep_StoreCode";
		this.rep_StoreStatus.AutoHeight = false;
		this.rep_StoreStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_StoreStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_StoreStatus.DisplayMember = "Name";
		this.rep_StoreStatus.Name = "rep_StoreStatus";
		this.rep_StoreStatus.NullText = "";
		this.rep_StoreStatus.ShowHeader = false;
		this.rep_StoreStatus.ValueMember = "Code";
		this.xtraTabPageEx1.Controls.Add(this.grd_Distributors);
		this.xtraTabPageEx1.EditControl = null;
		this.xtraTabPageEx1.EditValue = "Nhà phân phối";
		this.xtraTabPageEx1.FieldName = "";
		this.xtraTabPageEx1.IsSearchControl = false;
		this.xtraTabPageEx1.Name = "xtraTabPageEx1";
		this.xtraTabPageEx1.ReadOnly = false;
		this.xtraTabPageEx1.SettingInVisible = false;
		this.xtraTabPageEx1.SettingReadonly = false;
		this.xtraTabPageEx1.Size = new System.Drawing.Size(370, 411);
		this.xtraTabPageEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageEx1.Text = "Nhà phân phối";
		this.grd_Distributors.AllowPasteExcel = false;
		this.grd_Distributors.ClipboardData = null;
		this.grd_Distributors.ContextMenuStrip = this.ctm_Distributors;
		this.grd_Distributors.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Distributors.Location = new System.Drawing.Point(0, 0);
		this.grd_Distributors.MainView = this.gv_Distributors;
		this.grd_Distributors.Name = "grd_Distributors";
		this.grd_Distributors.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.chk_Active, this.rep_CardCode });
		this.grd_Distributors.Size = new System.Drawing.Size(370, 411);
		this.grd_Distributors.TabIndex = 4;
		this.grd_Distributors.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Distributors });
		this.ctm_Distributors.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.menu_AddDistributor, this.menu_RemoveDistributor });
		this.ctm_Distributors.Name = "contextMenuStrip4";
		this.ctm_Distributors.Size = new System.Drawing.Size(124, 48);
		this.menu_AddDistributor.Name = "menu_AddDistributor";
		this.menu_AddDistributor.Size = new System.Drawing.Size(123, 22);
		this.menu_AddDistributor.Text = "Add New";
		this.menu_AddDistributor.Click += new System.EventHandler(this.menu_AddDistributor_Click);
		this.menu_RemoveDistributor.Name = "menu_RemoveDistributor";
		this.menu_RemoveDistributor.Size = new System.Drawing.Size(123, 22);
		this.menu_RemoveDistributor.Text = "Remove";
		this.menu_RemoveDistributor.Click += new System.EventHandler(this.menu_RemoveDistributor_Click);
		this.gv_Distributors.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Distributors.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Distributors.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clSalesRoute;
		this.gv_Distributors.ColumnKey = null;
		this.gv_Distributors.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gc_CardCode, this.gc_CardName, this.gridColumnEx4, this.gc_Remarks1 });
		this.gv_Distributors.DisableFormSetting = false;
		this.gv_Distributors.DisplayException = false;
		this.gv_Distributors.EditColumn = null;
		this.gv_Distributors.FieldCurrency = null;
		this.gv_Distributors.FieldRate = null;
		this.gv_Distributors.FormId = null;
		this.gv_Distributors.GridControl = this.grd_Distributors;
		this.gv_Distributors.IgnoreFilter = false;
		this.gv_Distributors.IndexEditRow = -1;
		this.gv_Distributors.IndicatorWidth = 35;
		this.gv_Distributors.IsApzTable = true;
		this.gv_Distributors.IsInsertNewRow = false;
		this.gv_Distributors.IsLoadingData = false;
		this.gv_Distributors.IsMappingContextAction = false;
		this.gv_Distributors.ItemId = null;
		this.gv_Distributors.LstColumnAllowBatch = null;
		this.gv_Distributors.LstColumnNotAllowBatch = null;
		this.gv_Distributors.LstColumnNotEditable = null;
		this.gv_Distributors.LstColumNotSetting = null;
		this.gv_Distributors.Name = "gv_Distributors";
		this.gv_Distributors.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Distributors.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Distributors.OptionsNavigation.AutoFocusNewRow = true;
		this.gv_Distributors.OptionsPrint.AutoWidth = false;
		this.gv_Distributors.OptionsPrint.UsePrintStyles = false;
		this.gv_Distributors.OptionsSelection.MultiSelect = true;
		this.gv_Distributors.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Distributors.OptionsView.ColumnAutoWidth = false;
		this.gv_Distributors.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Distributors.OptionsView.ShowGroupedColumns = true;
		this.gv_Distributors.OptionsView.ShowGroupPanel = false;
		this.gv_Distributors.UseBatchMod = false;
		this.gv_Distributors.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Distributors_CellValueChanged);
		this.gv_Distributors.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gv_Doc_MouseDown);
		this.gridColumnEx1.Caption = "Id";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "Id";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.AllowEdit = false;
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx2.Caption = "Route Code";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "RouteCode";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx3.Caption = "Employee Id";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "EmpId";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.AllowEdit = false;
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gc_CardCode.Caption = "Mã NPP";
		this.gc_CardCode.ColumnEdit = this.rep_CardCode;
		this.gc_CardCode.CurrencyDateField = null;
		this.gc_CardCode.CurrencyField = null;
		this.gc_CardCode.CurrencyFixed = null;
		this.gc_CardCode.CurrencyFixedRate = 0.0;
		this.gc_CardCode.CurrencyRateField = null;
		this.gc_CardCode.FieldName = "CardCode";
		this.gc_CardCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gc_CardCode.Name = "gc_CardCode";
		this.gc_CardCode.PrimaryKey = false;
		this.gc_CardCode.UnitField = null;
		this.gc_CardCode.Visible = true;
		this.gc_CardCode.VisibleIndex = 0;
		this.gc_CardCode.Width = 100;
		this.rep_CardCode.AutoHeight = false;
		editorButtonImageOptions13.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_CardCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions13, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance17, appearanceHovered17, appearancePressed17, appearanceDisabled17, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_CardCode.Name = "rep_CardCode";
		this.rep_CardCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_CardCode_ButtonClick);
		this.gc_CardName.Caption = "Tên NPP";
		this.gc_CardName.CurrencyDateField = null;
		this.gc_CardName.CurrencyField = null;
		this.gc_CardName.CurrencyFixed = null;
		this.gc_CardName.CurrencyFixedRate = 0.0;
		this.gc_CardName.CurrencyRateField = null;
		this.gc_CardName.FieldName = "CardName";
		this.gc_CardName.Name = "gc_CardName";
		this.gc_CardName.OptionsColumn.ReadOnly = true;
		this.gc_CardName.PrimaryKey = false;
		this.gc_CardName.UnitField = null;
		this.gc_CardName.Visible = true;
		this.gc_CardName.VisibleIndex = 1;
		this.gc_CardName.Width = 125;
		this.gridColumnEx4.Caption = "Địa chỉ";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "Address";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.ReadOnly = true;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 2;
		this.gridColumnEx4.Width = 85;
		this.gc_Remarks1.Caption = "Diễn giải";
		this.gc_Remarks1.CurrencyDateField = null;
		this.gc_Remarks1.CurrencyField = null;
		this.gc_Remarks1.CurrencyFixed = null;
		this.gc_Remarks1.CurrencyFixedRate = 0.0;
		this.gc_Remarks1.CurrencyRateField = null;
		this.gc_Remarks1.FieldName = "Remarks";
		this.gc_Remarks1.Name = "gc_Remarks1";
		this.gc_Remarks1.PrimaryKey = false;
		this.gc_Remarks1.UnitField = null;
		this.gc_Remarks1.Visible = true;
		this.gc_Remarks1.VisibleIndex = 3;
		this.gc_Remarks1.Width = 85;
		this.chk_Active.AutoHeight = false;
		this.chk_Active.DisplayValueChecked = "Y";
		this.chk_Active.DisplayValueUnchecked = "N";
		this.chk_Active.Name = "chk_Active";
		this.chk_Active.ValueChecked = "Y";
		this.chk_Active.ValueUnchecked = "N";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(5, 668);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 20);
		this.btn_OK.TabIndex = 12;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = true;
		this.btn_Cancel.Location = new System.Drawing.Point(83, 668);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 13;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1314, 695);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.split_Main);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.lk_Status);
		base.Controls.Add(this.btn_RouteName);
		base.Controls.Add(this.lbl_RouteName);
		base.Controls.Add(this.lbl_Status);
		base.Controls.Add(this.btn_RouteCode);
		base.Controls.Add(this.lbl_RCode);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SalesRoute";
		base.ShowInTaskbar = false;
		this.Text = "Tuyến bán hàng";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_SalesRoute_FormClosing);
		base.Load += new System.EventHandler(this.frm_SalesRoute_Load);
		((System.ComponentModel.ISupportInitialize)this.btn_RouteCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_RouteName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lk_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.split_Main).EndInit();
		this.split_Main.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Routes).EndInit();
		this.ctm_Routes.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Routes).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CountryCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DocStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Channel).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PriceList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PriceListDistributor).EndInit();
		((System.ComponentModel.ISupportInitialize)this.split_Details).EndInit();
		this.split_Details.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.split_Employee).EndInit();
		this.split_Employee.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tv_Details).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ChooseType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Employee).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SalesAdmin).EndInit();
		this.ctm_Details.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.rep_Geography).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_AddTree).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BP).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BPCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Emp).EndInit();
		this.ctm_Emp.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Emp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SalesEmployee).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).EndInit();
		this.xtraTabControlEx1.ResumeLayout(false);
		this.xtraTabPageEx2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Outlets).EndInit();
		this.ctm_Outlet.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Outlets).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_OutletCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_StoreCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_StoreStatus).EndInit();
		this.xtraTabPageEx1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Distributors).EndInit();
		this.ctm_Distributors.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Distributors).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CardCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active).EndInit();
		base.ResumeLayout(false);
	}
}
