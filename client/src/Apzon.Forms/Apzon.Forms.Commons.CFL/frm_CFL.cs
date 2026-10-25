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
using Apzon.Forms.Administrator;
using Apzon.Forms.Administrator.ApprovalProcess;
using Apzon.Forms.Administrator.SetUp;
using Apzon.Forms.Administrator.SetUp.Inventory;
using Apzon.Forms.Administrator.SetUp.Inventory.Bin_Location;
using Apzon.Forms.Administrator.SetUp.SalesEmployeeBuyers;
using Apzon.Forms.Administrator.SetUp.Service;
using Apzon.Forms.Administrator.SystemInitialization;
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons.UDO;
using Apzon.Forms.DMS;
using Apzon.Forms.Financials;
using Apzon.Forms.Helper;
using Apzon.Forms.HumanResources;
using Apzon.Forms.HumanResources.TimeSheeting;
using Apzon.Forms.Interfaces.FormDocument;
using Apzon.Forms.Inventory.InventoryTransaction;
using Apzon.Forms.Inventory.Item_Management;
using Apzon.Forms.Inventory.Pick_and_Pack;
using Apzon.Forms.ItemsMaster;
using Apzon.Forms.KPIs;
using Apzon.Forms.MTS_Manufacturing;
using Apzon.Forms.Mrps;
using Apzon.Forms.PM;
using Apzon.Forms.Production;
using Apzon.Forms.Promotions;
using Apzon.Forms.Purchase;
using Apzon.Forms.RoutingAndResource;
using Apzon.Forms.Sales;
using Apzon.Forms.SalesOppotunity;
using Apzon.Forms.Service;
using Apzon.Forms.Service.MarketingCampaign;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace Apzon.Forms.Commons.CFL;

public class frm_CFL : XtraFormEx
{
	public bool CflMutiSelectMode;

	public Enums.TypeCFL CflType;

	public DataRow ChooseItem;

	private readonly Enums.ItemObjectType _itemObjectType;

	public List<DataRow> ChooseItems;

	public string StrSearch;

	private string _storeProc;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public List<string> Params = new List<string>();

	public string ResourceType = "";

	public int MrpLineBplid = -1;

	private const string Controller = "CFL";

	public string BomFather = string.Empty;

	public new DataSet DataSource;

	public string objtype;

	private List<Common.ParameterValue> lstpr = new List<Common.ParameterValue>();

	public string _tableType = "1";

	public TreeEx tree;

	public int UomGroup;

	public string ProductCode;

	public string DataString1;

	public string DataString2;

	public DataTable DataTable1;

	public string ScheduleCode = "";

	public string MrpCode = "";

	public string MrpStatus = "";

	public string ScheduleStatus = "";

	public string CapacityStatus = "";

	public string MrpType = "";

	public string ScheduleType = "";

	public string ForecastCode = "";

	public string IsGenCapacity = "";

	public string BpCode = "";

	public string SubCatNum = "";

	public string AtpType = "";

	public string AtpStatus = "";

	public string OutputAtpType = "";

	public string AtpScheduleType = "";

	public string AtpScheduleCode = "";

	public string AtpCode = "";

	public string AtpScheduleStatus = "";

	public int MaterialShortageCode;

	public string MaterialShortageType = "";

	public int MaterialShortageBranch = -1;

	public string MaterialShortageStatus = "";

	public int Branch;

	public string period;

	public int MachineNo;

	public DataTable dtSource = new DataTable();

	public bool ButtonEditMultiSelect;

	public Action AfterLoadForm;

	public DataTable Search = new DataTable();

	public string ResourceWorkScheduleStatus = "";

	public DataTable dtBPCode = new DataTable();

	public string ListMandatoryField;

	private List<Enums.TypeCFL> lstPreventAddNewCfl = new List<Enums.TypeCFL>
	{
		Enums.TypeCFL.UserDefinedProcedure,
		Enums.TypeCFL.UserDefinedQuery,
		Enums.TypeCFL.UserDefinedCfl,
		Enums.TypeCFL.UserDefinedData,
		Enums.TypeCFL.BatchNumbers,
		Enums.TypeCFL.BatchSerialNumber,
		Enums.TypeCFL.DashBoardCtrl,
		Enums.TypeCFL.Banks,
		Enums.TypeCFL.FactoringIndicator,
		Enums.TypeCFL.EmployeesTechnician,
		Enums.TypeCFL.AcctBPDownPayment,
		Enums.TypeCFL.AcctCusBPOpenDebts,
		Enums.TypeCFL.AcctVenBPOpenDebts,
		Enums.TypeCFL.ListUoMNameItemMaster,
		Enums.TypeCFL.ListUoMForUoMPrices,
		Enums.TypeCFL.ListAlterUoMs,
		Enums.TypeCFL.BOMCFLOfProduct,
		Enums.TypeCFL.ListBOMActive,
		Enums.TypeCFL.ListOfBinLocationByWareHouse,
		Enums.TypeCFL.TransferRequestCopyFrom,
		Enums.TypeCFL.InventoryCountingBatchSerial,
		Enums.TypeCFL.EmployeeContract,
		Enums.TypeCFL.ItemForSaleOrder,
		Enums.TypeCFL.BOMSaleOrder,
		Enums.TypeCFL.WbsInTabItem,
		Enums.TypeCFL.ItemInTabItem,
		Enums.TypeCFL.UserDefinedReport,
		Enums.TypeCFL.Languages,
		Enums.TypeCFL.BinOfItemGroupsWarehouse,
		Enums.TypeCFL.DefaultForUser,
		Enums.TypeCFL.ListOperation,
		Enums.TypeCFL.ListSemiOfProduc,
		Enums.TypeCFL.ListWorkCenter,
		Enums.TypeCFL.ListAdvanceRouting,
		Enums.TypeCFL.GetTaskList,
		Enums.TypeCFL.PreDecisionTask,
		Enums.TypeCFL.ListItemByCardCode,
		Enums.TypeCFL.ItemForServiceCall,
		Enums.TypeCFL.SerialNum,
		Enums.TypeCFL.ListResourceWorkSchedule,
		Enums.TypeCFL.GetPOforCopy,
		Enums.TypeCFL.GetPOforWrap,
		Enums.TypeCFL.GetCase,
		Enums.TypeCFL.GetTypeCase,
		Enums.TypeCFL.GetItem,
		Enums.TypeCFL.GetTC,
		Enums.TypeCFL.DraftNo,
		Enums.TypeCFL.DraftType,
		Enums.TypeCFL.WorkingProcessRequest,
		Enums.TypeCFL.SeriesOutlet,
		Enums.TypeCFL.PriceList
	};

	protected List<DataRow> selection;

	private IContainer components;

	public GridControlEx grd_CFL;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit_Type;

	private ButtonEditEx txt_Search;

	private LabelLine lb_Search;

	private RepositoryItemTextEdit rtxt;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnChoose;

	private SimpleButtonEx btn_Export;

	public GridViewEx gv_CFL;

	private SimpleButtonEx btn_Collapse;

	private SimpleButtonEx btn_New;

	private PanelControl panel_Data;

	public int campaignId { get; set; }

	public bool HasNew { get; set; }

	public string ItemCode { get; set; }

	public int DataInt1 { get; set; }

	public int DataInt2 { get; set; }

	public int UgpEntry { get; set; }

	public string WhsCode { get; set; }

	public string ObjType { get; set; }

	public int UomEntry { get; set; }

	public string LstUoMEntry { get; set; }

	public DataTable DataBatchAndSerial { get; set; }

	public string Type { get; set; }

	public string CountDate { get; set; }

	public int CountTime { get; set; }

	public string Status { get; set; }

	public int Series { get; set; }

	public int DocNum { get; set; }

	public string Ref2 { get; set; }

	public string WareHouse { get; set; }

	public string StatusPeriod { get; set; }

	public string CodePeriod { get; set; }

	public string NamePeriod { get; set; }

	public string Vendor { get; set; }

	public int ObjectType { get; set; }

	public int projectId { get; set; }

	public string wbsCode { get; set; }

	public int orderId { get; set; }

	public string FromWhsCode { get; set; }

	public string ToWhsCode { get; set; }

	public frm_CFL(Enums.TypeCFL type, bool multiSelect, string txtSeach, string storeProcedure = "", Enums.ItemObjectType itemObjectType = Enums.ItemObjectType.NoItemType, List<Common.ParameterValue> lstpr = null, string itemId = "", params string[] prms)
	{
		InitializeComponent();
		gv_CFL.OptionsView.ShowColumnHeaders = false;
		gv_CFL.NotLoadFormsettingAtLoad = true;
		FormId = "CFL";
		gv_CFL.FormId = FormId;
		GridViewEx gridViewEx = gv_CFL;
		int num = (int)type;
		gridViewEx.ItemId = num.ToString();
		CflType = type;
		if (CflType.ToString().StartsWith("UserDefined"))
		{
			if (CflType != Enums.TypeCFL.UserDefinedQuery)
			{
				gv_CFL.ItemId = storeProcedure;
			}
			else if (!string.IsNullOrWhiteSpace(itemId))
			{
				gv_CFL.ItemId = itemId;
			}
		}
		else if (!CflType.ToString().StartsWith("UserDefined"))
		{
			if (CflType == Enums.TypeCFL.LandedCostGodInvoice)
			{
				gv_CFL.ItemId += Function.ToString(ObjectType);
			}
			else
			{
				GridViewEx gridViewEx2 = gv_CFL;
				num = (int)CflType;
				gridViewEx2.ItemId = num.ToString();
			}
		}
		SettingLanguage();
		CflMutiSelectMode = multiSelect;
		StrSearch = txtSeach;
		ChooseItems = new List<DataRow>();
		_itemObjectType = itemObjectType;
		_storeProc = storeProcedure;
		if (prms.Length != 0)
		{
			Params = prms.ToList();
		}
		if (prms.Length != 0)
		{
			objtype = Function.ToString(prms.FirstOrDefault());
		}
		if (lstpr != null)
		{
			this.lstpr = lstpr;
		}
		OnLoadAsync += frm_ChooseFromList_Load;
	}

	public frm_CFL(bool buttonEditSource, Enums.TypeCFL type, bool multiSelect, string txtSeach, string storeProcedure = "", Enums.ItemObjectType itemObjectType = Enums.ItemObjectType.NoItemType, List<Common.ParameterValue> lstpr = null, params string[] prms)
	{
		if (buttonEditSource)
		{
			grd_CFL = new GridControlEx();
			gv_CFL = new GridViewEx();
			gv_CFL.GridControl = grd_CFL;
			grd_CFL.MainView = gv_CFL;
			btn_New = new SimpleButtonEx();
			btn_Collapse = new SimpleButtonEx();
			FormId = "CFL";
			gv_CFL.FormId = FormId;
			GridViewEx gridViewEx = gv_CFL;
			int num = (int)type;
			gridViewEx.ItemId = num.ToString();
			CflType = type;
			CflMutiSelectMode = multiSelect;
			StrSearch = txtSeach;
			ChooseItems = new List<DataRow>();
			_itemObjectType = itemObjectType;
			_storeProc = storeProcedure;
			if (prms.Length != 0)
			{
				Params = prms.ToList();
			}
			if (prms.Length != 0)
			{
				objtype = Function.ToString(prms.FirstOrDefault());
			}
			if (lstpr != null)
			{
				this.lstpr = lstpr;
			}
		}
		OnLoadAsync += frm_ChooseFromList_Load;
	}

	private void SettingLanguage()
	{
		lb_Search.Utext = Resources.ButtonSearch;
		btnChoose.Text = Resources.ButtonChoose;
		btnCancel.Text = Resources.ButtonCancel;
	}

	private async Task frm_ChooseFromList_Load(object sender, EventArgs e)
	{
		try
		{
			txt_Search.Enabled = false;
			btnChoose.Enabled = false;
			btn_Export.Enabled = false;
			btn_New.Enabled = false;
			await LoadForm();
			if (!string.IsNullOrEmpty(StrSearch) && gv_CFL.OptionsSelection.MultiSelectMode != GridMultiSelectMode.CheckBoxRowSelect)
			{
				txt_Search.Text = StrSearch;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			txt_Search.Enabled = true;
			btnChoose.Enabled = true;
			btn_Export.Enabled = true;
			btn_New.Enabled = true;
			txt_Search.Focus();
		}
	}

	private void gv_CFL_DoubleClick(object sender, EventArgs e)
	{
		DXMouseEventArgs e2 = e as DXMouseEventArgs;
		GridHitInfo gridHitInfo = gv_CFL.CalcHitInfo(e2.Location);
		if (!ButtonEditMultiSelect && gridHitInfo.InRow)
		{
			AddChooseItemsGrid();
			DialogResult = DialogResult.OK;
			Close();
		}
	}

	private void gv_CFL_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				AddChooseItemsGrid();
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void Tree_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (tree.CalcHitInfo(tree.PointToClient(Control.MousePosition)).InRow)
			{
				AddChooseItemsGrid();
				DialogResult = DialogResult.OK;
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Tree_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				AddChooseItemsGrid();
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void txt_Search_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (_tableType.Equals("1"))
			{
				string text = "";
				foreach (GridColumn visibleColumn in gv_CFL.VisibleColumns)
				{
					text += $"OR [{visibleColumn.FieldName}] LIKE '%{txt_Search.Text}%' ";
				}
				text = ((text.Length > 2) ? text.Substring(2) : "");
				gv_CFL.ActiveFilterString = text;
				return;
			}
			string text2 = "";
			foreach (TreeListColumn item in (IEnumerable<TreeListColumn>)tree.VisibleColumns)
			{
				text2 += $"OR [{item.FieldName}] LIKE '%{txt_Search.Text}%' ";
			}
			text2 = ((text2.Length > 2) ? text2.Substring(2) : "");
			tree.ActiveFilterString = text2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void btnChoose_Click(object sender, EventArgs e)
	{
		AddChooseItemsGrid();
		DialogResult = DialogResult.OK;
		Close();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.Cancel;
		Close();
	}

	public async Task LoadForm(bool buttonEditSource = false)
	{
		gv_CFL.ShowLoadingPanel();
		try
		{
			HasNew = true;
			btn_New.Visible = HasNew;
			if (CflMutiSelectMode)
			{
				gv_CFL.OptionsSelection.MultiSelect = true;
				gv_CFL.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
			}
			DataSet ds = new DataSet();
			switch (CflType)
			{
			case Enums.TypeCFL.UserDefinedQuery:
				if (!string.IsNullOrEmpty(_storeProc))
				{
					ds = await _httpClient.Cfl.GetDataByQuery(_storeProc);
				}
				break;
			case Enums.TypeCFL.UserDefinedCfl:
				if (!string.IsNullOrEmpty(_storeProc))
				{
					ds = await _httpClient.Cfl.GetDataByStore(_storeProc, "", GlobalData.UserName, GlobalData.DefaultBranch.ToString());
				}
				break;
			case Enums.TypeCFL.UserDefinedProcedure:
				if (!string.IsNullOrEmpty(_storeProc))
				{
					DataSetResult dataSetResult = await _httpClient.Commons.ExecuteStore(_storeProc, lstpr);
					ds = dataSetResult.Data;
					if (!string.IsNullOrEmpty(dataSetResult.Message))
					{
						WinCommons.Error(Enums.MessageCode.None, dataSetResult.Message);
					}
				}
				break;
			case Enums.TypeCFL.UserDefinedData:
				_storeProc = "UserDefinedData";
				if (DataSource.IsNotNull())
				{
					ds = DataSource;
				}
				break;
			case Enums.TypeCFL.Bp:
				Text = "List of Business Partners";
				ds = await _httpClient.Cfl.LoadBusinessPartner("", GlobalData.UserName, GlobalData.DefaultBranch.ToString());
				break;
			case Enums.TypeCFL.Vendor:
				Text = "List of Business Partners";
				ds = await _httpClient.Cfl.LoadBusinessPartner("S", GlobalData.UserName, GlobalData.DefaultBranch.ToString());
				break;
			case Enums.TypeCFL.Lead:
				Text = "List of Business Partners";
				ds = await _httpClient.Cfl.LoadBusinessPartner("L", GlobalData.UserName, GlobalData.DefaultBranch.ToString());
				break;
			case Enums.TypeCFL.Customer:
				Text = "List of Business Partners";
				ds = await _httpClient.Cfl.LoadBusinessPartner("C", GlobalData.UserName, GlobalData.DefaultBranch.ToString());
				break;
			case Enums.TypeCFL.CustomerLead:
				Text = "List of Business Partners";
				ds = await _httpClient.Cfl.LoadBusinessPartner("L,C", GlobalData.UserName, GlobalData.DefaultBranch.ToString());
				break;
			case Enums.TypeCFL.CustomerVendor:
				Text = "List of Business Partners";
				ds = await _httpClient.Cfl.LoadBusinessPartner("C,S", GlobalData.UserName, GlobalData.DefaultBranch.ToString());
				break;
			case Enums.TypeCFL.VendorTypeEmployee:
				Text = "List of Business Partners";
				ds = await _httpClient.Cfl.LoadBusinessPartner("S", GlobalData.UserName, GlobalData.DefaultBranch.ToString(), "E");
				break;
			case Enums.TypeCFL.Items:
			case Enums.TypeCFL.ListItems:
				Text = "List of Items";
				ds = await GetListItem("", ((int)_itemObjectType).ToString(), GlobalData.UserName, GlobalData.DefaultBranch.ToString());
				break;
			case Enums.TypeCFL.Resource:
				Text = "List of Resources";
				ds = await GetResource("", GlobalData.UserName, GlobalData.DefaultBranch.ToString());
				break;
			case Enums.TypeCFL.Warehouse:
			case Enums.TypeCFL.GetListWareHouse:
				Text = "List of Warehouses";
				ds = await GetListWareHouse("", "", GlobalData.DefaultBranch.ToString());
				break;
			case Enums.TypeCFL.Project:
				Text = "List of Projects";
				ds = await GetProject("", GlobalData.UserName, GlobalData.DefaultBranch.ToString());
				break;
			case Enums.TypeCFL.Employees:
				Text = "List of Employees";
				if (Params != null && Params.Count > 0)
				{
					ds = (string.IsNullOrWhiteSpace(_storeProc) ? (await _httpClient.Cfl.GetEmployees(Branch, Params[0])) : (await _httpClient.Cfl.GetDataByStore(_storeProc, Params[0], GlobalData.UserName, GlobalData.DefaultBranch.ToString())));
				}
				break;
			case Enums.TypeCFL.EmployeeRelatives:
				Text = "List of Employee's Relatives";
				if (Params != null && Params.Count > 0)
				{
					ds = await _httpClient.Cfl.GetDataByStore(_storeProc, Params[0], GlobalData.UserName, GlobalData.DefaultBranch.ToString());
				}
				break;
			case Enums.TypeCFL.ListOfInstallBase:
				if (Params != null && Params.Count > 1)
				{
					ds = await GetListOfInstallBase(Params[0], Params[1], GlobalData.UserName, GlobalData.DefaultBranch.ToString());
				}
				break;
			case Enums.TypeCFL.ListMasterResource:
				ds = await GetListMasterResource();
				break;
			case Enums.TypeCFL.ListOfCostCenter:
				ds = await GetListCostCenter();
				break;
			case Enums.TypeCFL.AdvanceForecast:
				ds = await GetListAdvanceForecast();
				break;
			case Enums.TypeCFL.MRP:
				ds = await GetListMrp();
				break;
			case Enums.TypeCFL.Schedule:
				ds = await GetListSchedule();
				break;
			case Enums.TypeCFL.DefinedShift:
				ds = await GetListDefinedShift();
				break;
			case Enums.TypeCFL.ListItemsMRPLines:
				ds = await GetListItem("", ((int)_itemObjectType).ToString(), GlobalData.UserName, GlobalData.DefaultBranch.ToString());
				FilterMRPItemCode(ds);
				break;
			case Enums.TypeCFL.ListItemsMRPUsers:
				ds = GetListMRPUsers();
				break;
			case Enums.TypeCFL.Lines:
				ds = await GetListDataLines();
				break;
			case Enums.TypeCFL.BoMItem:
				ds = await GetListBomItem();
				break;
			case Enums.TypeCFL.BatchNumbers:
				ds = await GetBatchSerialBinNumbers();
				break;
			case Enums.TypeCFL.BatchSerialNumber:
				ds = await GetBatchSerialNumbers();
				break;
			case Enums.TypeCFL.BatchSerialForReturnComponents:
				ds = await GetBatchSerialForReturnComponents();
				break;
			case Enums.TypeCFL.DashBoardCtrl:
				ds = await GetDashBoardController();
				break;
			case Enums.TypeCFL.Banks:
				ds = GetDataBanks();
				break;
			case Enums.TypeCFL.SalesOder:
				ds = await GetDataSalesOder();
				break;
			case Enums.TypeCFL.ListOfCostCenterByDimCode:
				ds = GetDataListOfCostCenterByDimCode();
				break;
			case Enums.TypeCFL.FactoringIndicator:
				ds = GetDataFactoringIndicator();
				break;
			case Enums.TypeCFL.EmployeesTechnician:
				ds = GetDataEmployeesTechnician();
				break;
			case Enums.TypeCFL.Territory:
				ds = GetDataTerritory();
				break;
			case Enums.TypeCFL.AcctBPDownPayment:
				Text = "List of Accounts";
				ds = GetDataBPControlAccounts("", "D");
				break;
			case Enums.TypeCFL.AcctCusBPOpenDebts:
				Text = "List of Accounts";
				ds = GetDataBPControlAccounts("C", "O");
				break;
			case Enums.TypeCFL.AcctVenBPOpenDebts:
				Text = "List of Accounts";
				ds = GetDataBPControlAccounts("S", "O");
				break;
			case Enums.TypeCFL.ListUoMByUoMGroupItemMaster:
				ds = GetDataListUoMbyUoMGroup();
				break;
			case Enums.TypeCFL.ListUoMNameItemMaster:
				ds = GetDataListUoMNameItemMaster();
				break;
			case Enums.TypeCFL.ListUoMForUoMPrices:
				ds = await GetDataListUoMForUoMPrices();
				break;
			case Enums.TypeCFL.ItemWarrantyTemplate:
				ds = GetItemWarrantyTemplates();
				break;
			case Enums.TypeCFL.ListUoMs:
				ds = GetDataListUoMs();
				break;
			case Enums.TypeCFL.ListAlterUoMs:
				ds = GetDataListAlterUoMs();
				break;
			case Enums.TypeCFL.BOMCFLOfProduct:
				ds = await GetDataBOMCFLOfProduct();
				break;
			case Enums.TypeCFL.ListBOMActive:
				ds = await GetDataListBOMActive();
				break;
			case Enums.TypeCFL.BillOfMaterForProduction:
				ds = GetDataBOMForProduction();
				break;
			case Enums.TypeCFL.ChartOfAccount:
				ds = GetDataChartOfAccount();
				break;
			case Enums.TypeCFL.ListOfBinLocationByWareHouse:
				ds = ListOfBinLocationByWareHouse();
				break;
			case Enums.TypeCFL.TransferRequestCopyFrom:
				ds = await GetDataTransferRequestCopyFrom();
				break;
			case Enums.TypeCFL.ListUomCode:
				ds = await GetDataUomCode();
				break;
			case Enums.TypeCFL.ProductionOrder:
				ds = await GetProductionOrder();
				break;
			case Enums.TypeCFL.StandardProductionOrder:
				ds = await GetProductionOrder();
				break;
			case Enums.TypeCFL.DisassemblyProductionOrder:
				ds = await GetProductionOrder("D");
				break;
			case Enums.TypeCFL.IssueProductionOrder:
				ds = await GetProductionOrder("I");
				break;
			case Enums.TypeCFL.ReceiptProductionOrder:
				ds = await GetProductionOrder("R");
				break;
			case Enums.TypeCFL.ReturnComponents:
				ds = await GetProductionOrder("C");
				break;
			case Enums.TypeCFL.SaleEmployee:
				ds = await GetDataSalesPerson();
				break;
			case Enums.TypeCFL.EmployeeListForSalesEmpSetup:
				ds = GetEmployeeListForSalesEmpSetup();
				break;
			case Enums.TypeCFL.BpCatalog:
				Text = "List Of BP catalog";
				ds = await GetDataBpCatalogItem(BpCode, SubCatNum, GlobalData.UserName, "", GlobalData.DefaultBranch.ToString(), ItemCode);
				break;
			case Enums.TypeCFL.StockCounting:
				ds = await GetListInventoryCounting();
				break;
			case Enums.TypeCFL.InventoryCountingBatchSerial:
				ds = await _httpClient.Cfl.GetBatchSerial(ItemCode, WhsCode);
				break;
			case Enums.TypeCFL.GetDataBin:
				ds = await _httpClient.Cfl.GetDataBin(WhsCode, ItemCode, DataInt1);
				break;
			case Enums.TypeCFL.InventoryCyles:
				ds = await GetListInventoryCyles();
				break;
			case Enums.TypeCFL.GetQualificationList:
				ds = GetQualificationList();
				break;
			case Enums.TypeCFL.GetApplicantList:
				ds = GetApplicantList();
				break;
			case Enums.TypeCFL.GetListQuestion:
				ds = GetListQuestion();
				break;
			case Enums.TypeCFL.ListAtp:
				ds = await GetListAtp();
				break;
			case Enums.TypeCFL.WorkPeriod:
				ds = await FindWorkPeriod();
				break;
			case Enums.TypeCFL.ListScheduleAtp:
				ds = await GetListScheduleAtp();
				break;
			case Enums.TypeCFL.Queries:
				Text = "List of Queries";
				ds = await GetQueries();
				break;
			case Enums.TypeCFL.MaterialShortageReport:
				Text = "List Material Shortage Report";
				ds = await GetListMaterialShortageReport();
				break;
			case Enums.TypeCFL.GetLanedCostsList:
				ds = GetLanedCostsList();
				break;
			case Enums.TypeCFL.LanedCostsCopyForm:
				ds = LandedCostCopyFrom();
				break;
			case Enums.TypeCFL.LandedCostGodInvoice:
				ds = LandedCostGodInvoice();
				break;
			case Enums.TypeCFL.GetTableNameUDO:
				ds = GetTableNameORW(objtype);
				break;
			case Enums.TypeCFL.PickList:
				ds = ((!DataSource.IsNotNull()) ? (await _httpClient.Packing.ChoosePickList(BpCode)) : DataSource);
				break;
			case Enums.TypeCFL.Packing:
				ds = ((!DataSource.IsNotNull()) ? (await _httpClient.Packing.ChoosePacking()) : DataSource);
				break;
			case Enums.TypeCFL.PickingBatch:
				ds = ((!DataSource.IsNotNull()) ? (await _httpClient.Packing.ChooseBatch()) : DataSource);
				break;
			case Enums.TypeCFL.GetSalesOppPartner:
				ds = GetSalesOppPartner();
				break;
			case Enums.TypeCFL.ListOfPick:
				ds = await _httpClient.Shipping.ChooseList(ApzObjectTypes.oPickLists, BpCode);
				break;
			case Enums.TypeCFL.ListOfPack:
				ds = await _httpClient.Shipping.ChooseList(ApzObjectTypes.oPacking, BpCode);
				break;
			case Enums.TypeCFL.Shipping:
				ds = await _httpClient.Shipping.ChooseList(ApzObjectTypes.oShipping, BpCode);
				break;
			case Enums.TypeCFL.Vehicle:
			{
				DataTable dataTable2 = await _httpClient.Vehicle.GetList();
				ds = new DataSet
				{
					Tables = { dataTable2.Copy() }
				};
				break;
			}
			case Enums.TypeCFL.EmployeeContract:
				ds = await _httpClient.Cfl.GetEmployeeContract(Branch);
				break;
			case Enums.TypeCFL.GetUserList:
				ds = await _httpClient.Cfl.GetUserList();
				break;
			case Enums.TypeCFL.KnowledgeBaseSolutions:
				ds = await _httpClient.Cfl.GetSolution();
				break;
			case Enums.TypeCFL.UserDefinedTexts:
				ds = await _httpClient.Cfl.UserDefinedTexts();
				break;
			case Enums.TypeCFL.ListContractTemplate:
				ds = await _httpClient.Cfl.GetListContractTemplate();
				break;
			case Enums.TypeCFL.ProjectTask:
				ds = await _httpClient.Cfl.GetDataProjectTask(projectId);
				break;
			case Enums.TypeCFL.ProjectManager:
				ds = await _httpClient.Cfl.GetDataProjectManager(projectId);
				_tableType = "2";
				break;
			case Enums.TypeCFL.ItemForSaleOrder:
				ds = await _httpClient.Cfl.GetItemBySaleOrder(orderId);
				break;
			case Enums.TypeCFL.BOMSaleOrder:
				ds = await _httpClient.Cfl.GetBOMTemplateBySaleOrder();
				break;
			case Enums.TypeCFL.ProjectReportCompletion:
				ds = await _httpClient.Cfl.GetDataProjectReportCompletion(projectId);
				break;
			case Enums.TypeCFL.WbsInTabItem:
				ds = await _httpClient.Cfl.GetDataWBSInTabItem(projectId);
				break;
			case Enums.TypeCFL.ItemInTabItem:
				ds = await _httpClient.Cfl.GetDataItemInTabItem(projectId, wbsCode);
				break;
			case Enums.TypeCFL.UserDefinedReport:
				ds = await _httpClient.Cfl.UserDefinedReport();
				break;
			case Enums.TypeCFL.DeliveryTypes:
				ds = await _httpClient.Cfl.GetDataDeliveryTypes();
				break;
			case Enums.TypeCFL.Languages:
				ds = await _httpClient.Cfl.GetDataLanguages();
				break;
			case Enums.TypeCFL.BinOfItemGroupsWarehouse:
				ds = DataSource;
				break;
			case Enums.TypeCFL.DefaultForUser:
				ds = await _httpClient.Cfl.GetDataDefaultForUser();
				break;
			case Enums.TypeCFL.InventoryCountingRequest:
				ds = await _httpClient.Cfl.GetDataInventoryCountingRequest();
				break;
			case Enums.TypeCFL.ListOperation:
				ds = await _httpClient.Cfl.GetListOperation();
				break;
			case Enums.TypeCFL.ListSemiOfProduc:
				ds = await _httpClient.Cfl.GetListSemiOfProduct(ProductCode);
				break;
			case Enums.TypeCFL.WorkSchedule:
			{
				DataTableCollection tables = ds.Tables;
				tables.Add(await _httpClient.Cfl.LoadDataWorkSchedule());
				gv_CFL.OptionsBehavior.Editable = false;
				break;
			}
			case Enums.TypeCFL.ListWorkCenter:
				ds = await _httpClient.Cfl.GetListWorkCenter();
				break;
			case Enums.TypeCFL.ListAdvanceRouting:
				ds = await _httpClient.Cfl.GetListAdvanceRouting(ProductCode);
				break;
			case Enums.TypeCFL.GetTaskList:
				ds = await _httpClient.Cfl.GetListTask();
				break;
			case Enums.TypeCFL.PreDecisionTask:
				ds = await _httpClient.Cfl.PreDecisionTask(orderId);
				break;
			case Enums.TypeCFL.ListItemByCardCode:
				ds = await _httpClient.Cfl.ListItemByCardCode(BpCode, Type);
				break;
			case Enums.TypeCFL.ListServiceContractTemplate:
				ds = await _httpClient.Cfl.ListServiceContractTemplate(Type);
				break;
			case Enums.TypeCFL.ServiceLevelAgreement:
			{
				DataTableCollection tables = ds.Tables;
				tables.Add(await _httpClient.Cfl.GetDataServiceLevelAgreement());
				gv_CFL.OptionsBehavior.Editable = false;
				break;
			}
			case Enums.TypeCFL.ItemForServiceCall:
				ds = await _httpClient.Cfl.GetListItemForServiceCall(BpCode);
				break;
			case Enums.TypeCFL.SerialNum:
				ds = await _httpClient.Cfl.GetListSerialNum(DataString2);
				break;
			case Enums.TypeCFL.MarketingCampaign:
				ds = await _httpClient.Cfl.GetMarketingCampaign(campaignId);
				break;
			case Enums.TypeCFL.WbsTemplate:
				ds = await _httpClient.Cfl.GetWbsTemplate();
				break;
			case Enums.TypeCFL.ProjectManagerAll:
				ds = await _httpClient.Cfl.GetAllProjectManager();
				_tableType = "2";
				break;
			case Enums.TypeCFL.Opportunity:
				ds = await _httpClient.Cfl.GetAllOpportunities();
				break;
			case Enums.TypeCFL.ListCapacity:
				ds = await _httpClient.Cfl.GetListCapacity(CapacityStatus);
				break;
			case Enums.TypeCFL.ListResourceWorkSchedule:
				ds = await _httpClient.Cfl.GetListResourceWorkSchedule(ResourceWorkScheduleStatus);
				break;
			case Enums.TypeCFL.BioDataList:
				ds = await _httpClient.Cfl.GetBioDataList(Branch, period);
				break;
			case Enums.TypeCFL.TerritoryAll:
				ds = await _httpClient.Cfl.GetAllTerritory();
				_tableType = "2";
				break;
			case Enums.TypeCFL.BioMapping:
				ds = await _httpClient.Cfl.GetBioMapping(MachineNo);
				break;
			case Enums.TypeCFL.Geography:
				Text = "List of Geography";
				ds = await _httpClient.Cfl.GetAllGeography();
				break;
			case Enums.TypeCFL.Events:
				ds = await _httpClient.Cfl.GetAllEvents();
				break;
			case Enums.TypeCFL.GeographyDetails:
				ds = ((lstpr.Count <= 0) ? (await _httpClient.Cfl.GetGeoGraphyDetails("", "")) : (await _httpClient.Cfl.GetGeoGraphyDetails(Function.ToString(lstpr[0].Value), Function.ToString(lstpr[1].Value))));
				break;
			case Enums.TypeCFL.Store:
				ds = ((lstpr.Count <= 0) ? (await _httpClient.Cfl.GetStore("")) : (await _httpClient.Cfl.GetStore(Function.ToString(lstpr[0].Value))));
				break;
			case Enums.TypeCFL.StoreGeography:
				ds = await _httpClient.Cfl.GetStoreGeography(Function.ToString(lstpr[0].Value));
				_tableType = "2";
				break;
			case Enums.TypeCFL.GetPO:
				ds = await _httpClient.Cfl.LoadDataGetPO();
				_tableType = "2";
				break;
			case Enums.TypeCFL.GetPOforCopy:
				ds = await _httpClient.Cfl.LoadDataGetPOforCopy();
				_tableType = "2";
				break;
			case Enums.TypeCFL.GetPOforWrap:
				ds = await _httpClient.Cfl.LoadDataGetPOforWrap();
				_tableType = "2";
				break;
			case Enums.TypeCFL.GetCase:
				ds = await _httpClient.Cfl.LoadDataGetCase();
				_tableType = "2";
				break;
			case Enums.TypeCFL.GetTypeCase:
				ds = await _httpClient.Cfl.LoadDataGetTypeCase();
				_tableType = "2";
				break;
			case Enums.TypeCFL.GetItem:
				ds = await _httpClient.Cfl.LoadDataGetItem();
				_tableType = "2";
				break;
			case Enums.TypeCFL.GetTC:
				ds = await _httpClient.Cfl.LoadDataGetTC();
				_tableType = "2";
				break;
			case Enums.TypeCFL.GetProductionOrder:
				ds = await _httpClient.Cfl.LoadDataGetProductionOrder();
				_tableType = "2";
				break;
			case Enums.TypeCFL.AlternativeItems:
				ds = DataSource;
				break;
			case Enums.TypeCFL.Users:
				ds = await _httpClient.Cfl.GetUsers();
				break;
			case Enums.TypeCFL.DraftNo:
				ds = await _httpClient.Cfl.GetDraftNo();
				break;
			case Enums.TypeCFL.DraftType:
				ds = await _httpClient.Cfl.GetDraftType();
				break;
			case Enums.TypeCFL.WorkingProcessRequest:
				ds = await _httpClient.Cfl.GetWorkingProcessRequest(Function.ParseInt(lstpr[0].Value));
				break;
			case Enums.TypeCFL.BinLocAttribute:
				Text = "List of Bin Location Attribute Codes";
				ds = await _httpClient.Cfl.GetBinLocAttribute(Function.ParseInt(lstpr[0].Value));
				break;
			case Enums.TypeCFL.WarehouseSublevel:
				Text = "List of Warehouse Sublevel Codes";
				ds = await _httpClient.Cfl.GetWarehouseSublevel(Function.ParseInt(lstpr[0].Value));
				break;
			case Enums.TypeCFL.WarehouseBinActivated:
				Text = "List of Warehouses";
				ds = await _httpClient.Cfl.GetWarehouseBinActivated();
				break;
			case Enums.TypeCFL.GetAllReportDMS:
				Text = "List Report";
				ds = await _httpClient.Cfl.GetAllReportDMS();
				break;
			case Enums.TypeCFL.Channel:
				Text = "List of Channel";
				ds = await _httpClient.Cfl.GetChannel();
				break;
			case Enums.TypeCFL.SalesRoute:
				ds = await _httpClient.Cfl.GetSalesRoute();
				break;
			case Enums.TypeCFL.SalesRouteEmployee:
				Text = "List of Employees";
				ds = await _httpClient.Cfl.GetSalesRouteEmployees();
				break;
			case Enums.TypeCFL.SeriesOutlet:
				Text = "List of Series";
				ds = await _httpClient.Cfl.GetSeriesOutlet();
				break;
			case Enums.TypeCFL.ListOfBusinessPartner:
			{
				Text = "List of Business Partner";
				ds = _httpClient.Cfl.GetBusinessPartner();
				string text4 = string.Empty;
				foreach (DataRow row in dtBPCode.Rows)
				{
					text4 = text4 + "'" + row["CardCode"]?.ToString() + "',";
				}
				if (!string.IsNullOrEmpty(text4))
				{
					DataRow[] array3 = ds.Tables[1].Select("CardCode not in(" + text4 + ")");
					if (!string.IsNullOrEmpty(text4) && array3.Length != 0)
					{
						ds.Tables.RemoveAt(1);
						ds.Tables.Add(array3.CopyToDataTable());
					}
				}
				break;
			}
			case Enums.TypeCFL.Distributors:
				ds = await _httpClient.Cfl.GetDistributors();
				break;
			case Enums.TypeCFL.WorkingProcessTooltips:
			{
				ds = await _httpClient.Cfl.GetWorkingProcessTooltips();
				if (!dtSource.IsNotNull())
				{
					break;
				}
				string text3 = string.Empty;
				foreach (DataRow row2 in dtSource.Rows)
				{
					if (!string.IsNullOrEmpty(Function.ToString(row2["ColId"])))
					{
						text3 = text3 + "'" + Function.ToString(row2["ColId"]) + "',";
					}
				}
				if (!string.IsNullOrEmpty(text3))
				{
					string filterExpression = $"ColumnName not in({text3.Remove(text3.Length - 1, 1)})";
					List<DataRow> list = ds.Tables[1].Select(filterExpression).ToList();
					if (list.Count > 0)
					{
						ds.Tables.RemoveAt(1);
						ds.Tables.Add(list.CopyToDataTable());
					}
					else
					{
						DataTable table = ds.Tables[1].Copy().Clone();
						ds.Tables.RemoveAt(1);
						ds.Tables.Add(table);
					}
					ds.AcceptChanges();
				}
				break;
			}
			case Enums.TypeCFL.KPIsMaster:
				ds = await _httpClient.Cfl.GetKPIsMaster();
				break;
			case Enums.TypeCFL.ApprovalUsers:
			{
				Text = "List of User";
				ds = await _httpClient.Cfl.GetApprovalUsers(Function.ParseInt(lstpr[0].Value));
				string text2 = string.Empty;
				foreach (DataRow row3 in dtSource.Rows)
				{
					text2 = text2 + row3["UserId"]?.ToString() + ",";
				}
				if (!string.IsNullOrEmpty(text2))
				{
					DataRow[] array2 = ds.Tables[1].Select("UserId not in(" + text2 + ")");
					if (!string.IsNullOrEmpty(text2) && array2.Length != 0)
					{
						ds.Tables.RemoveAt(1);
						ds.Tables.Add(array2.CopyToDataTable());
					}
				}
				break;
			}
			case Enums.TypeCFL.Departments:
				Text = "List of Department";
				ds = await _httpClient.Cfl.GetDepartments();
				break;
			case Enums.TypeCFL.ApprovalStages:
				ds = await _httpClient.Cfl.GetApprovalStages();
				break;
			case Enums.TypeCFL.ApprovalTemplateStages:
			{
				Text = "List of Approval Stages";
				ds = await _httpClient.Cfl.GetApprovalTemplateStages();
				string text = string.Empty;
				foreach (DataRow row4 in dtSource.Rows)
				{
					text = text + row4["WstCode"]?.ToString() + ",";
				}
				if (!string.IsNullOrEmpty(text))
				{
					DataRow[] array = ds.Tables[1].Select("WstCode not in(" + text + ")");
					if (!string.IsNullOrEmpty(text) && array.Length != 0)
					{
						ds.Tables.RemoveAt(1);
						ds.Tables.Add(array.CopyToDataTable());
					}
				}
				break;
			}
			case Enums.TypeCFL.GetListKPIs:
				ds = await _httpClient.Cfl.GetListKPIs();
				break;
			case Enums.TypeCFL.KPIsSettings:
				ds = await _httpClient.Cfl.GetKPIsSettings();
				break;
			case Enums.TypeCFL.PromotionCondition:
				ds = await _httpClient.Cfl.GetPromotionCondition((lstpr.Count > 0) ? Function.ToString(lstpr[0].Value) : "");
				break;
			case Enums.TypeCFL.PromotionBenefit:
				ds = await _httpClient.Cfl.GetPromotionBenefit((lstpr.Count > 0) ? Function.ToString(lstpr[0].Value) : "");
				break;
			case Enums.TypeCFL.PriceList:
				ds = await _httpClient.Cfl.GetPriceList();
				break;
			case Enums.TypeCFL.ApprovalObjectType:
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("Code", typeof(string));
				dataTable.Columns.Add("Name", typeof(string));
				dataTable.Rows.Add("23", "Sales Quotation");
				dataTable.Rows.Add("17", "Sales Order");
				dataTable.Rows.Add("15", "Delivery");
				dataTable.Rows.Add("16", "Return");
				dataTable.Rows.Add("203", "A/R Down Payment");
				dataTable.Rows.Add("13", "A/R Invoice");
				dataTable.Rows.Add("165", "A/R Correction Invoice");
				dataTable.Rows.Add("166", "A/R Correction Invoice Reversal");
				dataTable.Rows.Add("14", "A/R Credit Memo");
				dataTable.Rows.Add("132", "Correction Invoice");
				dataTable.Rows.Add("1470000113", "Purchase Request");
				dataTable.Rows.Add("540000006", "Purchase Quotation");
				dataTable.Rows.Add("22", "Purchase Order");
				dataTable.Rows.Add("20", "Goods Receipt PO");
				dataTable.Rows.Add("21", "Goods Returns");
				dataTable.Rows.Add("204", "A/P Down Payment");
				dataTable.Rows.Add("18", "A/P Invoice");
				dataTable.Rows.Add("163", "A/P Correction Invoice");
				dataTable.Rows.Add("164", "A/P Correction Invoice Reversal");
				dataTable.Rows.Add("19", "A/P Credit Memo");
				dataTable.Rows.Add("59", "Goods Receipt");
				dataTable.Rows.Add("60", "Goods Issue");
				dataTable.Rows.Add("1250000001", "Inventory Transfer Request");
				dataTable.Rows.Add("67", "Inventory Transfer");
				dataTable.Rows.Add("310000001", "Inventory Opening Balance");
				dataTable.Rows.Add("46", "Outgoing Payment");
				dataTable.Rows.Add("1470000065", "Inventory Counting");
				dataTable.Rows.Add("10000071", "Inventory Posting");
				dataTable.Rows.Add("112", "Documents - Drafts");
				dataTable.Rows.Add("140", "Payment Drafts");
				dataTable.Rows.Add("1470000109", "Inventory Counting - Drafts");
				dataTable.Rows.Add("1470000136", "Inventory Posting - Drafts");
				dataTable.Rows.Add("1470000131", "Inventory Opening Balance - Drafts");
				ds = new DataSet
				{
					Tables = { dataTable }
				};
				break;
			}
			case Enums.TypeCFL.WorkCodeAll:
				Text = "List of Work Code";
				ds = DataSource;
				break;
			case Enums.TypeCFL.GoodsIssueOpen:
				Text = "List of Goods Issue";
				ds = await _httpClient.Cfl.GetGoodsIssueOpen();
				break;
			}
			btn_New.Enabled = !lstPreventAddNewCfl.Contains(CflType);
			tree = new TreeEx();
			tree.NotLoadFormsettingAtLoad = true;
			if (ds.IsNotNull())
			{
				if (buttonEditSource)
				{
					if (ds.IsHasStructure())
					{
						grd_CFL.DataSource = ds.Tables[1];
					}
					else
					{
						grd_CFL.DataSource = ds.Tables[0];
					}
					gv_CFL.HideLoadingPanel();
					if (AfterLoadForm != null)
					{
						AfterLoadForm();
					}
					return;
				}
				gv_CFL.OptionsView.ColumnAutoWidth = false;
				if (_tableType.Equals("2"))
				{
					panel_Data.Controls.Clear();
					tree.Dock = DockStyle.Fill;
					tree.OptionsView.ShowFilterPanelMode = DevExpress.XtraTreeList.ShowFilterPanelMode.Never;
					tree.OptionsView.ShowColumns = false;
					panel_Data.Controls.Add(tree);
					await TreeListHelper.GenerateTreeList(tree, ds, null, expand: true);
					tree.OptionsSelection.MultiSelectMode = TreeListMultiSelectMode.RowSelect;
					tree.OptionsSelection.MultiSelect = CflMutiSelectMode;
					tree.DoubleClick += Tree_DoubleClick;
					tree.KeyDown += Tree_KeyDown;
					grd_CFL.DataSource = tree.DataSourceTable();
					if (tree.VisibleColumns.Count > 5)
					{
						Size = new Size(800, 600);
					}
					tree.OptionsView.AutoWidth = false;
				}
				else
				{
					await GridViewHelper.GenerateGrid(grd_CFL, gv_CFL, ds, null, expand: true);
					int num = gv_CFL.VisibleColumns.Sum((GridColumn c) => c.Width);
					if (num > Size.Width && num <= 800)
					{
						Size = new Size(num + gv_CFL.IndicatorWidth + 50, 600);
					}
					if (gv_CFL.RowCount > 0)
					{
						gv_CFL.ExpandAllGroups();
						btn_Collapse.Visible = gv_CFL.GetRowExpanded(0);
					}
				}
			}
			ds.Tables.Clear();
			ds.Dispose();
			gv_CFL.OptionsView.ColumnAutoWidth = false;
			if (AfterLoadForm != null)
			{
				AfterLoadForm();
			}
			if (gv_CFL.OptionsSelection.MultiSelectMode == GridMultiSelectMode.CheckBoxRowSelect)
			{
				selection = new List<DataRow>();
				int[] selectedRows = gv_CFL.GetSelectedRows();
				GridColumnEx gridColumnEx = new GridColumnEx();
				gv_CFL.Columns.Add(gridColumnEx);
				gridColumnEx.Visible = false;
				gridColumnEx.FieldName = "_CheckBoxSelect_";
				if (gv_CFL.LstColumNotSetting != null)
				{
					gv_CFL.LstColumNotSetting.Add(gridColumnEx.FieldName);
				}
				else
				{
					gv_CFL.LstColumNotSetting = new List<string> { gridColumnEx.FieldName };
				}
				gridColumnEx.UnboundType = UnboundColumnType.Boolean;
				gv_CFL.OptionsSelection.CheckBoxSelectorField = gridColumnEx.FieldName;
				gv_CFL.CustomUnboundColumnData += Gv_CFL_CustomUnboundColumnData;
				for (int num2 = 0; num2 < selectedRows.Length; num2++)
				{
					gv_CFL.SelectRow(selectedRows[num2]);
				}
			}
			tree.DataSource = grd_CFL.DataSourceTable;
			await ReloadFormSetting(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
		gv_CFL.HideLoadingPanel();
	}

	private void Gv_CFL_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		if (e.Column.FieldName == gv_CFL.OptionsSelection.CheckBoxSelectorField)
		{
			if (e.IsGetData)
			{
				e.Value = IsRowSelected(gv_CFL.GetRowHandle(e.ListSourceRowIndex));
			}
			else
			{
				SelectRow(gv_CFL.GetRowHandle(e.ListSourceRowIndex), (bool)e.Value);
			}
		}
	}

	public int GetSelectedIndex(DataRow row)
	{
		return selection.IndexOf(row);
	}

	public void SelectGroup(int rowHandle, bool select)
	{
		if (IsGroupRowSelected(rowHandle) & select)
		{
			return;
		}
		for (int i = 0; i < gv_CFL.GetChildRowCount(rowHandle); i++)
		{
			int childRowHandle = gv_CFL.GetChildRowHandle(rowHandle, i);
			if (gv_CFL.IsGroupRow(childRowHandle))
			{
				SelectGroup(childRowHandle, select);
			}
			else
			{
				SelectRow(childRowHandle, select);
			}
		}
	}

	public bool IsGroupRowSelected(int rowHandle)
	{
		for (int i = 0; i < gv_CFL.GetChildRowCount(rowHandle); i++)
		{
			int childRowHandle = gv_CFL.GetChildRowHandle(rowHandle, i);
			if (gv_CFL.IsGroupRow(childRowHandle))
			{
				if (!IsGroupRowSelected(childRowHandle))
				{
					return false;
				}
			}
			else if (!IsRowSelected(childRowHandle))
			{
				return false;
			}
		}
		return true;
	}

	public bool IsRowSelected(int rowHandle)
	{
		if (gv_CFL.IsGroupRow(rowHandle))
		{
			return IsGroupRowSelected(rowHandle);
		}
		DataRow dataRow = gv_CFL.GetDataRow(rowHandle);
		return GetSelectedIndex(dataRow) != -1;
	}

	private void SelectRow(int rowHandle, bool select)
	{
		if (IsRowSelected(rowHandle) != select)
		{
			DataRow dataRow = gv_CFL.GetDataRow(rowHandle);
			if (select)
			{
				selection.Add(dataRow);
			}
			else
			{
				selection.Remove(dataRow);
			}
		}
	}

	private DataSet GetItemWarrantyTemplates()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("GetItemWarrantyTemplates", new
			{
				type = "S"
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	private DataSet GetDataBOMForProduction()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("GetDataBOMForProduction");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	private DataSet GetDataBPControlAccounts(string cardType, string acctType)
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("GetDataBPControlAccounts", new { cardType, acctType });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	private async Task<DataSet> GetDataListBOMActive()
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await http.GetAsync("GetDataListBOMActive");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	private async Task<DataSet> GetListMaterialShortageReport()
	{
		try
		{
			return await _httpClient.Cfl.GetListMaterialShortageReport(MaterialShortageBranch, MaterialShortageType, MaterialShortageStatus);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	private async Task<DataSet> GetQueries()
	{
		try
		{
			return await _httpClient.Queries.GetListQueries(GlobalData.UserName, GlobalData.BranchLogin);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetListScheduleAtp()
	{
		try
		{
			return await _httpClient.Cfl.GetListScheduleAtp(AtpCode, AtpScheduleType, AtpScheduleStatus, AtpScheduleCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetListAtp()
	{
		try
		{
			return await _httpClient.Cfl.GetListAtp(AtpType, AtpStatus, OutputAtpType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetDataBOMCFLOfProduct()
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await http.GetAsync("GetDataBOMCFLOfProduct");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetDataBpCatalogItem(string bpCode, string subCatNum, string userName, string docdate, string branch, string itemCode)
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await http.GetAsync("GetDataBpCatalogItem", new { bpCode, subCatNum, userName, docdate, branch, itemCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private DataSet GetEmployeeListForSalesEmpSetup()
	{
		try
		{
			return _httpClient.Cfl.GetEmployeeListForSalesEmpSetup();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	private DataSet GetDataListOfCostCenterByDimCode()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("GetListOfCostCenterByDimCode", new
			{
				dimCode = DataInt1
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	private async Task<DataSet> GetProductionOrder(string type = "S")
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await http.GetAsync("GetProductionOrder", new { type });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	private async Task<DataSet> GetDataUomCode()
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await http.GetAsync("GetDataUomCode", new
			{
				uomGroup = UomGroup
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	private async Task<DataSet> GetDataTransferRequestCopyFrom()
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await http.GetAsync("GetDataTransferRequestCopyFrom", new
			{
				fromWhsCode = FromWhsCode,
				toWhsCode = ToWhsCode
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	private async Task<DataSet> GetDataSalesOder()
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await http.GetAsync("GetDataSalesOder");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private DataSet GetDataChartOfAccount()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("GetDataChartOfAccount");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private DataSet ListOfBinLocationByWareHouse()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("ListOfBinLocationByWareHouse", new
			{
				whsCode = DataString1
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private DataSet GetQualificationList()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("GetQualificationList");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	private DataSet GetApplicantList()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("GetApplicantList");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	private DataSet GetListQuestion()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("GetListQuestion");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	private void AddChooseItemsGrid()
	{
		try
		{
			if (ButtonEditMultiSelect)
			{
				ChooseItems = selection ?? gv_CFL.GetListDataSourceSelectedRow();
			}
			else if (_tableType.Equals("1"))
			{
				if (gv_CFL.GetSelectedRows() == null || gv_CFL.GetSelectedRows().Length == 0)
				{
					return;
				}
				int[] selectedRows = gv_CFL.GetSelectedRows();
				foreach (int rowHandle in selectedRows)
				{
					DataRow dataRow = gv_CFL.GetDataRow(rowHandle);
					if (CflMutiSelectMode)
					{
						ChooseItems.Add(dataRow);
						continue;
					}
					ChooseItem = dataRow;
					break;
				}
			}
			else
			{
				if (tree.FocusedNode == null || tree.DataSourceTable().Rows.Count == 0)
				{
					return;
				}
				if (tree.Selection.Count > 0 || tree.DataSourceTable().Rows.Count <= 0)
				{
					foreach (TreeListNode item in tree.Selection)
					{
						DataRow dataRow2 = tree.GetDataRow(item.Id);
						if (!CflMutiSelectMode)
						{
							ChooseItem = dataRow2;
							break;
						}
						ChooseItems.Add(dataRow2);
					}
					return;
				}
				DataRow dataRow3 = tree.GetDataRow(tree.FocusedNode.Id);
				ChooseItem = dataRow3;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataSet GetLanedCostsList()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("GetLanedCostsList");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	private DataSet LandedCostCopyFrom()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("LandedCostCopyFrom", new
			{
				vendor = Vendor
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	private DataSet LandedCostGodInvoice()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("LandedCostGodInvoice", new
			{
				vendor = Vendor,
				objectType = ObjectType
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	private DataSet GetEmployees(int branch)
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("GetEmployees", new { branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private DataSet GetTableNameORW(string objtype)
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("GetTableNameORW", new { objtype });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	private DataSet GetSalesOppPartner()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("GetSalesOppPartner");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	private DataSet GetDataBanks()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("GetDataBanks");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	private async Task<DataSet> GetBatchSerialBinNumbers()
	{
		try
		{
			DataSet dataSet;
			using (HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL"))
			{
				dataSet = await http.GetAsync("GetBatchSerialBinNumbers", new
				{
					itemCode = ItemCode,
					whsCode = WhsCode,
					objtype = ObjType
				});
			}
			if (dataSet.Tables.Count < 2 || !dataSet.Tables[1].IsNotNull())
			{
				return dataSet;
			}
			DataTable dataTable = dataSet.Tables[1];
			if (DataBatchAndSerial.IsNotNull())
			{
				for (int i = 0; i < DataBatchAndSerial.Rows.Count; i++)
				{
					if (Function.ParseInt(DataBatchAndSerial.Rows[i]["AbsEntry"]) != 0)
					{
						DataRow[] array = dataTable.Select(string.Format("AbsEntry = {0} and SysNumber = {1}", Function.ParseInt(DataBatchAndSerial.Rows[i]["AbsEntry"]), Function.ParseInt(DataBatchAndSerial.Rows[i]["SysNumber"])));
						if (array.Length != 0)
						{
							array[0]["CommitQty"] = Function.ParseDouble(array[0]["CommitQty"]) + Function.ParseDouble(DataBatchAndSerial.Rows[i]["LogQty"]);
							dataTable.AcceptChanges();
						}
					}
				}
			}
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	private async Task<DataSet> GetBatchSerialNumbers()
	{
		try
		{
			DataSet dataSet;
			using (HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL"))
			{
				dataSet = await http.GetAsync("GetBatchSerialNumbers", new
				{
					itemCode = ItemCode,
					whsCode = WhsCode,
					objtype = ObjType,
					bpCode = BpCode
				});
			}
			if (dataSet.Tables.Count < 2 || !dataSet.Tables[1].IsNotNull())
			{
				return dataSet;
			}
			DataTable dataTable = dataSet.Tables[1];
			if (DataBatchAndSerial.IsNotNull())
			{
				for (int i = 0; i < DataBatchAndSerial.Rows.Count; i++)
				{
					if (Function.ParseInt(DataBatchAndSerial.Rows[i]["AbsEntry"]) != 0)
					{
						DataRow[] array = dataTable.Select(string.Format("AbsEntry = {0} and SysNumber = {1}", Function.ParseInt(DataBatchAndSerial.Rows[i]["AbsEntry"]), Function.ParseInt(DataBatchAndSerial.Rows[i]["SysNumber"])));
						if (array.Length != 0)
						{
							array[0]["CommitQty"] = Function.ParseDouble(array[0]["CommitQty"]) + Function.ParseDouble(DataBatchAndSerial.Rows[i]["LogQty"]);
							dataTable.AcceptChanges();
						}
					}
				}
			}
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	private async Task<DataSet> GetBatchSerialForReturnComponents()
	{
		try
		{
			DataSet dataSet;
			using (HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL"))
			{
				dataSet = await http.GetAsync("GetBatchSerialForReturnComponents", new
				{
					baseEntry = DataInt1,
					baseLine = DataInt2,
					whsCode = WhsCode
				});
			}
			if (dataSet.Tables.Count < 2 || !dataSet.Tables[1].IsNotNull())
			{
				return dataSet;
			}
			DataTable dataTable = dataSet.Tables[1];
			if (DataBatchAndSerial.IsNotNull())
			{
				for (int i = 0; i < DataBatchAndSerial.Rows.Count; i++)
				{
					if (Function.ParseInt(DataBatchAndSerial.Rows[i]["AbsEntry"]) != 0)
					{
						DataRow[] array = dataTable.Select(string.Format("AbsEntry = {0} and SysNumber = {1}", Function.ParseInt(DataBatchAndSerial.Rows[i]["AbsEntry"]), Function.ParseInt(DataBatchAndSerial.Rows[i]["SysNumber"])));
						if (array.Length != 0)
						{
							array[0]["CommitQty"] = Function.ParseDouble(array[0]["CommitQty"]) + Function.ParseDouble(DataBatchAndSerial.Rows[i]["LogQty"]);
							dataTable.AcceptChanges();
						}
					}
				}
			}
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	private DataSet GetDataListUoMbyUoMGroup()
	{
		DataSet dataSet = new DataSet();
		try
		{
			if (DataString2 == "I")
			{
				using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
				dataSet = httpClientHelper.Get("GetDataListUoMByUoMGroup", new
				{
					entry = DataInt1
				});
			}
			else
			{
				using (HttpClientHelper<DataSet> httpClientHelper2 = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL"))
				{
					dataSet = httpClientHelper2.Get("GetDataListUoMByUoMGroup", new
					{
						entry = DataInt1
					});
				}
				if (dataSet != null && dataSet.Tables.Count == 2)
				{
					dataSet.Tables.Remove(dataSet.Tables[1]);
					dataSet.AcceptChanges();
					dataSet.Tables.Add(DataTable1);
					dataSet.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
		return dataSet;
	}

	private DataSet GetDataListUoMNameItemMaster()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("GetDataListUoMNameItemMaster");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	private async Task<DataSet> GetDataListUoMForUoMPrices()
	{
		try
		{
			using (new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL"))
			{
				DataSet dataSet = await _httpClient.Cfl.GetDataListUoMForUoMPrices(DataString1, DataInt1, UgpEntry);
				if (dataSet != null && dataSet.Tables.Count == 2 && dataSet.Tables[1] != null)
				{
					for (int num = dataSet.Tables[1].Rows.Count - 1; num >= 0; num--)
					{
						DataRow dataRow = dataSet.Tables[1].Rows[num];
						if (DataString2.Contains(Function.ToString(dataRow["UomEntry"])))
						{
							dataSet.Tables[1].Rows.Remove(dataRow);
						}
						dataSet.Tables[1].AcceptChanges();
						dataSet.AcceptChanges();
					}
				}
				return dataSet;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	private DataSet GetDataListUoMs()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("GetDataListUoMs");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	private DataSet GetDataListAlterUoMs()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("GetDataListAlterUoMs", new
			{
				lstUoMEntry = LstUoMEntry
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	private async Task<DataSet> GetListBomItem()
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await http.GetAsync("GetListBomItem", new
			{
				code = BomFather
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	private async Task<DataSet> GetListDataLines()
	{
		try
		{
			if (Params == null || Params.Count == 0)
			{
				return null;
			}
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await http.GetAsync("GetListDataLines", new
			{
				branch = Function.ParseInt(Params[0])
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return null;
		}
	}

	private DataSet GetListMRPUsers()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("GetListMRPUsers");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	private void FilterMRPItemCode(DataSet ds)
	{
		try
		{
			int mrpLineBplid = MrpLineBplid;
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CFL");
			DataTable dataTable = httpClientHelper.Get("GetListItemsMRPItemCode", new
			{
				BPLId = mrpLineBplid
			});
			if (dataTable == null || dataTable.Rows.Count <= 0)
			{
				return;
			}
			string text = "";
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				text = text + "{" + Function.ToString(dataTable.Rows[i][0]) + "}";
			}
			for (int num = ds.Tables[1].Rows.Count - 1; num >= 0; num--)
			{
				DataRow dataRow = ds.Tables[1].Rows[num];
				if (text.Contains(dataRow["ItemCode"].ToString()))
				{
					dataRow.Delete();
				}
			}
			ds.Tables[1].AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataSet> GetListDefinedShift()
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await http.GetAsync("GetListDefinedShift", new
			{
				branch = Function.ParseInt(Params[0])
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetListMasterResource()
	{
		try
		{
			string resourceType = ResourceType;
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await helper.GetAsync("GetListMasterResource", new
			{
				resType = resourceType
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetProject(string prjCode, string userName, string branch)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await helper.GetAsync("GetProject", new { prjCode, userName, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetListOfInstallBase(string customer, string itemCode, string userName, string branch)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await helper.GetAsync("GetListOfInstallBase", new { customer, itemCode, userName, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetListCostCenter()
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await helper.GetAsync("GetListCostCenter");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetListWareHouse(string wareHouseCode, string wareHouseName, string branch)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await helper.GetAsync("GetListWareHouse", new { wareHouseCode, wareHouseName, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetListItem(string itemCode, string itemType, string userName, string branch)
	{
		_ = 1;
		try
		{
			string listField = ListMandatoryField;
			if (GlobalData.FormSettings.ContainsKey(gv_CFL.FormId) && GlobalData.FormSettings[gv_CFL.FormId].Tables.Contains(gv_CFL.ItemId))
			{
				EnumerableRowCollection<string> values = from r in GlobalData.FormSettings[gv_CFL.FormId].Tables[gv_CFL.ItemId].AsEnumerable()
					where Function.ParseBool(r["Visible"])
					select Function.ToString(r["ColumnId"]);
				listField = listField + "," + string.Join(",", values);
			}
			else
			{
				if (gv_CFL.FirstLoadFormSetting == null)
				{
					DataTable dataTable = await _httpClient.FormSettingClient.GetFormSetting(FormId, gv_CFL.ItemId, GlobalData.UserName, GlobalData.CURRENTLANGUAGE);
					if (dataTable != null && dataTable.Columns.Count != 0)
					{
						if (!GlobalData.FormSettings.ContainsKey(FormId))
						{
							GlobalData.FormSettings.Add(FormId, new DataSet());
						}
						if (!GlobalData.FormSettings[FormId].Tables.Contains(gv_CFL.ItemId))
						{
							dataTable.TableName = gv_CFL.ItemId;
							GlobalData.FormSettings[FormId].Tables.Add(dataTable);
						}
					}
				}
				if (GlobalData.FormSettings.ContainsKey(gv_CFL.FormId) && GlobalData.FormSettings[gv_CFL.FormId].Tables.Contains(gv_CFL.ItemId))
				{
					EnumerableRowCollection<string> values2 = from r in GlobalData.FormSettings[gv_CFL.FormId].Tables[gv_CFL.ItemId].AsEnumerable()
						where Function.ParseBool(r["Visible"])
						select Function.ToString(r["ColumnId"]);
					listField = listField + "," + string.Join(",", values2);
				}
			}
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			string text = string.Join(";", Params.ToArray());
			if (string.IsNullOrEmpty(text))
			{
				text = "";
			}
			return await helper.GetAsync("GetListItem", new
			{
				itemCode = itemCode,
				itemType = itemType,
				userName = userName,
				branch = branch,
				pr = text,
				listField = listField
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetResource(string rescode, string userName, string branch)
	{
		try
		{
			return await _httpClient.Cfl.GetResource(rescode, userName, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetListAdvanceForecast()
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await http.GetAsync("GetListAdvanceForecast");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetListMrp()
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await http.GetAsync("GetListMrp", new
			{
				mrpCode = MrpCode,
				mrpStatus = MrpStatus,
				forecast = ForecastCode,
				mrpType = MrpType
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetListSchedule()
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await http.GetAsync("GetListSchedule", new
			{
				scheduleCode = ScheduleCode,
				mrpCode = MrpCode,
				status = ScheduleStatus,
				mrpType = MrpType,
				scheduleType = ScheduleType,
				isGenCapacity = IsGenCapacity
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetDashBoardController()
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await http.GetAsync("GetListDBController");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetDataSalesPerson()
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await http.GetAsync("GetListSalesPerson");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetListInventoryCounting()
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await http.GetAsync("GetListInventoryCounting", new
			{
				countDate = CountDate,
				countTime = CountTime,
				status = Status,
				series = Series,
				ref2 = Ref2,
				docNum = DocNum,
				wareHouse = WareHouse
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetListInventoryCyles()
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await http.PostAsync("GetListInventoryCyles", Search);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private async Task<DataSet> FindWorkPeriod()
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await http.GetAsync("FindWorkPeriod", new
			{
				status = StatusPeriod,
				code = CodePeriod,
				name = NamePeriod
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private DataSet GetDataFactoringIndicator()
	{
		try
		{
			return _httpClient.Cfl.GetDataFactoringIndicator();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private DataSet GetDataEmployeesTechnician()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("GetDataEmployeesTechnician");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private DataSet GetDataTerritory()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return httpClientHelper.Get("GetDataTerritory");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private void btn_Export_Click(object sender, EventArgs e)
	{
		try
		{
			gv_CFL.Export();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Collapse_Click(object sender, EventArgs e)
	{
		try
		{
			if (btn_Collapse.Text.Equals("Collapse"))
			{
				btn_Collapse.Text = "Expand";
				gv_CFL.CollapseAllGroups();
			}
			else if (btn_Collapse.Text.Equals("Expand"))
			{
				btn_Collapse.Text = "Collapse";
				gv_CFL.ExpandAllGroups();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_New_Click(object sender, EventArgs e)
	{
		try
		{
			XtraFormEx xtraFormEx = new XtraFormEx();
			WinCommons._OpenProcessing();
			switch (CflType)
			{
			case Enums.TypeCFL.UserDefinedQuery:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.UserDefinedCfl:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.UserDefinedProcedure:
				xtraFormEx = new frm_MenuStructure();
				break;
			case Enums.TypeCFL.UserDefinedData:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.Bp:
			case Enums.TypeCFL.Customer:
			case Enums.TypeCFL.Vendor:
			case Enums.TypeCFL.Lead:
			case Enums.TypeCFL.CustomerVendor:
			case Enums.TypeCFL.CustomerLead:
			case Enums.TypeCFL.VendorTypeEmployee:
			case Enums.TypeCFL.GetSalesOppPartner:
			case Enums.TypeCFL.ListOfBusinessPartner:
				xtraFormEx = new frm_BusinessMasterData();
				break;
			case Enums.TypeCFL.Items:
			case Enums.TypeCFL.ListItems:
				xtraFormEx = new frm_ItemMasterData();
				break;
			case Enums.TypeCFL.Resource:
				xtraFormEx = new frm_Resources();
				break;
			case Enums.TypeCFL.Warehouse:
			case Enums.TypeCFL.GetListWareHouse:
			case Enums.TypeCFL.WarehouseBinActivated:
				xtraFormEx = new frm_WareHouses();
				break;
			case Enums.TypeCFL.Project:
				xtraFormEx = new frm_Project();
				break;
			case Enums.TypeCFL.Employees:
			case Enums.TypeCFL.SalesRouteEmployee:
				xtraFormEx = new frm_HumanResources();
				break;
			case Enums.TypeCFL.ListOfCostCenter:
			case Enums.TypeCFL.ListOfCostCenterByDimCode:
				xtraFormEx = new frm_CostCenters();
				break;
			case Enums.TypeCFL.AdvanceForecast:
				xtraFormEx = new frm_AdvanceForecast();
				break;
			case Enums.TypeCFL.MRP:
				xtraFormEx = new frm_MRP();
				break;
			case Enums.TypeCFL.Schedule:
				xtraFormEx = new frm_Schedule();
				break;
			case Enums.TypeCFL.DefinedShift:
				xtraFormEx = new FrmDefinedShift();
				break;
			case Enums.TypeCFL.ListItemsMRPUsers:
				xtraFormEx = new frm_Users();
				break;
			case Enums.TypeCFL.BoMItem:
				xtraFormEx = new frm_ItemMasterData();
				break;
			case Enums.TypeCFL.BatchNumbers:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.BatchSerialNumber:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.DashBoardCtrl:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.Banks:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.SalesOder:
				xtraFormEx = new frm_SalesOrder();
				break;
			case Enums.TypeCFL.FactoringIndicator:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.EmployeesTechnician:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.Territory:
			case Enums.TypeCFL.TerritoryAll:
				xtraFormEx = new frm_Territory();
				break;
			case Enums.TypeCFL.AcctBPDownPayment:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.AcctCusBPOpenDebts:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.AcctVenBPOpenDebts:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.ListUoMByUoMGroupItemMaster:
				xtraFormEx = new frm_UnitOfMeasureGroups();
				break;
			case Enums.TypeCFL.ListUoMNameItemMaster:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.ListUoMForUoMPrices:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.ItemWarrantyTemplate:
				xtraFormEx = new frm_WarrantyCard();
				break;
			case Enums.TypeCFL.ListUoMs:
			case Enums.TypeCFL.ListUomCode:
				xtraFormEx = new frm_UnitOfMeasure();
				break;
			case Enums.TypeCFL.ListAlterUoMs:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.BOMCFLOfProduct:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.ListBOMActive:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.BillOfMaterForProduction:
				xtraFormEx = new frm_BillOfMaterials();
				break;
			case Enums.TypeCFL.ListOfBinLocationByWareHouse:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.TransferRequestCopyFrom:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.DisassemblyProductionOrder:
			case Enums.TypeCFL.StandardProductionOrder:
			case Enums.TypeCFL.ProductionOrder:
			case Enums.TypeCFL.GetProductionOrder:
				xtraFormEx = new frm_ProductionOrder();
				break;
			case Enums.TypeCFL.IssueProductionOrder:
				xtraFormEx = new frm_IssueForProduction();
				break;
			case Enums.TypeCFL.ReceiptProductionOrder:
				xtraFormEx = new frm_ReceiptFromProduction();
				break;
			case Enums.TypeCFL.SaleEmployee:
				xtraFormEx = new frm_SalesEmployeeBuyers();
				break;
			case Enums.TypeCFL.EmployeeListForSalesEmpSetup:
				xtraFormEx = new frm_HumanResources();
				break;
			case Enums.TypeCFL.BpCatalog:
				xtraFormEx = new frm_BusinessPartnerCatalogNumbers();
				break;
			case Enums.TypeCFL.StockCounting:
				xtraFormEx = new frm_StockCounting();
				break;
			case Enums.TypeCFL.InventoryCountingBatchSerial:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.GetDataBin:
				xtraFormEx = new frm_BinLocMasterData();
				break;
			case Enums.TypeCFL.InventoryCyles:
				xtraFormEx = new frm_InventoryCycles();
				break;
			case Enums.TypeCFL.GetQualificationList:
				xtraFormEx = new frm_Qualifications();
				break;
			case Enums.TypeCFL.GetApplicantList:
				xtraFormEx = new frm_JVApplicants();
				break;
			case Enums.TypeCFL.GetListQuestion:
				xtraFormEx = new frm_JVQuestion();
				break;
			case Enums.TypeCFL.ListAtp:
				xtraFormEx = new frm_Atp();
				break;
			case Enums.TypeCFL.WorkPeriod:
				xtraFormEx = new frm_WorkPeriod();
				break;
			case Enums.TypeCFL.ListScheduleAtp:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.Queries:
				xtraFormEx = new frm_ListQueries();
				break;
			case Enums.TypeCFL.MaterialShortageReport:
				xtraFormEx = new frm_MaterialShortageReport();
				break;
			case Enums.TypeCFL.GetLanedCostsList:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.LanedCostsCopyForm:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.LandedCostGodInvoice:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.GetTableNameUDO:
				xtraFormEx = new frm_UDO();
				break;
			case Enums.TypeCFL.PickList:
				xtraFormEx = new frm_Picking();
				break;
			case Enums.TypeCFL.Packing:
				xtraFormEx = new frm_Packing();
				break;
			case Enums.TypeCFL.PickingBatch:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.ListOfPick:
				xtraFormEx = new frm_Picking();
				break;
			case Enums.TypeCFL.ListOfPack:
				xtraFormEx = new frm_Packing();
				break;
			case Enums.TypeCFL.Shipping:
				xtraFormEx = new frm_Shipping();
				break;
			case Enums.TypeCFL.Vehicle:
				xtraFormEx = new frm_Vehicle();
				break;
			case Enums.TypeCFL.EmployeeContract:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.GetUserList:
			case Enums.TypeCFL.Users:
			case Enums.TypeCFL.ApprovalUsers:
				xtraFormEx = new frm_Users();
				break;
			case Enums.TypeCFL.KnowledgeBaseSolutions:
				xtraFormEx = new frm_Solution();
				break;
			case Enums.TypeCFL.UserDefinedTexts:
				xtraFormEx = new frm_UserDefinedTexts();
				break;
			case Enums.TypeCFL.ListContractTemplate:
				xtraFormEx = new frm_ContractTemplate();
				break;
			case Enums.TypeCFL.ProjectTask:
			case Enums.TypeCFL.ProjectManager:
			case Enums.TypeCFL.ProjectManagerAll:
				xtraFormEx = new frm_WBSTemplate();
				break;
			case Enums.TypeCFL.ItemForSaleOrder:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.BOMSaleOrder:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.ProjectReportCompletion:
				xtraFormEx = new frm_ProjectReportCompletion();
				break;
			case Enums.TypeCFL.WbsInTabItem:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.ItemInTabItem:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.UserDefinedReport:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.DeliveryTypes:
				xtraFormEx = new frm_Shipping();
				break;
			case Enums.TypeCFL.Languages:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.BinOfItemGroupsWarehouse:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.DefaultForUser:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.InventoryCountingRequest:
				xtraFormEx = new frm_InventoryCountingRequest();
				break;
			case Enums.TypeCFL.ListOperation:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.ListSemiOfProduc:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.WorkSchedule:
				xtraFormEx = new frm_WorkScheduleSetup();
				break;
			case Enums.TypeCFL.ListWorkCenter:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.ListAdvanceRouting:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.GetTaskList:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.PreDecisionTask:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.ListItemByCardCode:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.ListServiceContractTemplate:
				xtraFormEx = new frm_ServiceContractTemplate();
				break;
			case Enums.TypeCFL.ServiceLevelAgreement:
				xtraFormEx = new frm_ServiceLevelAgreement();
				break;
			case Enums.TypeCFL.ItemForServiceCall:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.SerialNum:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.MarketingCampaign:
				xtraFormEx = new frm_MarketingCampaign();
				break;
			case Enums.TypeCFL.WbsTemplate:
				xtraFormEx = new frm_WBSTemplate();
				break;
			case Enums.TypeCFL.Opportunity:
				xtraFormEx = new frm_SalesOpportunity();
				break;
			case Enums.TypeCFL.ListCapacity:
				xtraFormEx = new frm_CapacityGantt();
				break;
			case Enums.TypeCFL.ListResourceWorkSchedule:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.BioDataList:
				xtraFormEx = new frm_BiometricData();
				break;
			case Enums.TypeCFL.BioMapping:
				xtraFormEx = new frm_BiometricEmpMapping();
				break;
			case Enums.TypeCFL.Geography:
			case Enums.TypeCFL.GeographyDetails:
				xtraFormEx = new frm_Geography();
				break;
			case Enums.TypeCFL.Events:
				xtraFormEx = new frm_EventManagement();
				break;
			case Enums.TypeCFL.GetPO:
				xtraFormEx = new frm_PurchaseOder();
				break;
			case Enums.TypeCFL.GetPOforCopy:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.GetPOforWrap:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.GetCase:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.GetTypeCase:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.GetItem:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.GetTC:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.AlternativeItems:
				xtraFormEx = new frm_AlternativeItems();
				break;
			case Enums.TypeCFL.DraftNo:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.DraftType:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.WorkingProcessRequest:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.BinLocAttribute:
				xtraFormEx = new frm_AttrCodeSetup();
				break;
			case Enums.TypeCFL.WarehouseSublevel:
				xtraFormEx = new frm_WHSubManager();
				break;
			case Enums.TypeCFL.GetAllReportDMS:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.Channel:
				xtraFormEx = new frm_Channel();
				break;
			case Enums.TypeCFL.SalesRoute:
				xtraFormEx = new frm_SalesRoute();
				break;
			case Enums.TypeCFL.SeriesOutlet:
				xtraFormEx = null;
				break;
			case Enums.TypeCFL.KPIsMaster:
				xtraFormEx = new frm_KPIsMasterData();
				break;
			case Enums.TypeCFL.Departments:
				xtraFormEx = new frm_Department();
				break;
			case Enums.TypeCFL.ApprovalStages:
			case Enums.TypeCFL.ApprovalTemplateStages:
				xtraFormEx = new frm_ApprovalStages();
				break;
			case Enums.TypeCFL.GetListKPIs:
				xtraFormEx = new frm_KPIsMasterData();
				break;
			case Enums.TypeCFL.KPIsSettings:
				xtraFormEx = new frm_KPIsSetting();
				break;
			case Enums.TypeCFL.PromotionCondition:
				xtraFormEx = new frm_PromotionCondition();
				break;
			case Enums.TypeCFL.PromotionBenefit:
				xtraFormEx = new frm_PromotionBenefit();
				break;
			case Enums.TypeCFL.WorkCodeAll:
				xtraFormEx = new frm_WorkCode
				{
					MdiParent = MdiParent
				};
				break;
			}
			if (xtraFormEx != null && !string.IsNullOrEmpty(xtraFormEx.Text))
			{
				xtraFormEx.ShowDialog(this);
			}
			await LoadForm();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public override async void FORMSETTING()
	{
		try
		{
			if (CflType != Enums.TypeCFL.ListItems && CflType != Enums.TypeCFL.Items)
			{
				base.FORMSETTING();
				return;
			}
			if (GlobalData.ListCFLItemField == null || GlobalData.ListCFLItemField.Count == 0)
			{
				GlobalData.ListCFLItemField = await _httpClient.Cfl.GetListCFLItemField();
			}
			if (GlobalData.ListCFLItemField == null || GlobalData.ListCFLItemField.Count <= 0)
			{
				return;
			}
			string value = Function.ToString(gv_CFL.ItemId);
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("FormId", typeof(string));
			dataTable.Columns.Add("ItemId", typeof(string));
			dataTable.Columns.Add("ColumnId", typeof(string));
			dataTable.Columns.Add("ColumnName", typeof(string));
			dataTable.Columns.Add("UserSign", typeof(string));
			dataTable.Columns.Add("Width", typeof(int));
			dataTable.Columns.Add("Visible", typeof(bool));
			dataTable.Columns.Add("VisIndex", typeof(int));
			dataTable.Columns.Add("Editable", typeof(bool));
			dataTable.Columns.Add("IsReadOnly", typeof(bool));
			dataTable.Columns.Add("DataType", typeof(string));
			dataTable.Columns.Add("Fixed", typeof(string));
			dataTable.Columns.Add("AllowChSet", typeof(bool));
			dataTable.Columns.Add("VisSetting", typeof(bool));
			dataTable.Columns.Add("IsSystem", typeof(string));
			dataTable.Columns.Add("LngCode", typeof(string));
			dataTable.Columns.Add("NeverEditable", typeof(bool));
			dataTable.Columns.Add("GroupIndex", typeof(int));
			dataTable.Columns.Add("CurrField", typeof(string));
			foreach (GridColumnEx item in (from t in gv_CFL.Columns
				orderby t.Visible descending, t.VisibleIndex
				select t).Cast<GridColumnEx>())
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["FormId"] = FormId;
				dataRow["ItemId"] = value;
				dataRow["ColumnId"] = item.FieldName;
				dataRow["ColumnName"] = (string.IsNullOrEmpty(item.Caption) ? item.FieldName : item.Caption);
				dataRow["UserSign"] = GlobalData.UserName;
				dataRow["Width"] = item.Width;
				dataRow["Visible"] = item.Visible;
				dataRow["VisIndex"] = item.AbsoluteIndex;
				dataRow["Editable"] = item.Visible && !item.OptionsColumn.ReadOnly;
				dataRow["NeverEditable"] = false;
				dataRow["IsReadOnly"] = item.OptionsColumn.ReadOnly;
				dataRow["DataType"] = Function.ToString(item.ColumnType);
				DataRow dataRow2 = dataRow;
				string value2;
				if (item.Fixed == DevExpress.XtraGrid.Columns.FixedStyle.Left)
				{
					value2 = "L";
				}
				else
				{
					value2 = ((item.Fixed == DevExpress.XtraGrid.Columns.FixedStyle.Right) ? "R" : "N");
				}
				dataRow2["Fixed"] = value2;
				dataRow["AllowChSet"] = item.AllowChSet;
				dataRow["VisSetting"] = true;
				dataRow["IsSystem"] = "Y";
				dataRow["LngCode"] = "-";
				dataRow["GroupIndex"] = item.GroupIndex;
				dataRow["CurrField"] = ((item.Currency == Enums.CurrencyColumn.Custom) ? item.CurrencyField : string.Empty);
				dataTable.Rows.Add(dataRow);
			}
			foreach (string item2 in GlobalData.ListCFLItemField)
			{
				if (gv_CFL.Columns[item2] == null)
				{
					DataRow dataRow3 = dataTable.NewRow();
					dataRow3["FormId"] = FormId;
					dataRow3["ItemId"] = value;
					dataRow3["ColumnId"] = item2;
					dataRow3["ColumnName"] = item2;
					dataRow3["UserSign"] = GlobalData.UserName;
					dataRow3["Width"] = 100;
					dataRow3["Visible"] = false;
					dataRow3["VisIndex"] = -1;
					dataRow3["Editable"] = false;
					dataRow3["NeverEditable"] = false;
					dataRow3["IsReadOnly"] = true;
					dataRow3["Fixed"] = "N";
					dataRow3["AllowChSet"] = true;
					dataRow3["VisSetting"] = true;
					dataRow3["IsSystem"] = "Y";
					dataRow3["LngCode"] = "-";
					dataRow3["GroupIndex"] = -1;
					dataTable.Rows.Add(dataRow3);
				}
			}
			frm_FormSetting frm_FormSetting2 = new frm_FormSetting();
			frm_FormSetting2.DtSource = dataTable;
			frm_FormSetting2.Grid = grd_CFL;
			frm_FormSetting2.Item = new Tuple<GridViewEx, string>(gv_CFL, gv_CFL.ItemId);
			frm_FormSetting2.FormSettingId = gv_CFL.FormId;
			frm_FormSetting2.ReloadAfterUpdate = false;
			frm_FormSetting2.ShowDialog();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_CFL_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (DialogResult == DialogResult.OK)
			{
				if (CflMutiSelectMode)
				{
					if (ChooseItems == null && ChooseItems.Count == 0)
					{
						e.Cancel = true;
					}
					return;
				}
				ChooseItem = ((_tableType == "1") ? gv_CFL.GetFocusedDataRow() : tree.GetFocusedDataRow());
				if (ChooseItem == null)
				{
					e.Cancel = true;
				}
			}
			else
			{
				ChooseItem = null;
				ChooseItems = null;
			}
		}
		catch (Exception)
		{
		}
	}

	private void gv_CFL_ShowingEditor(object sender, CancelEventArgs e)
	{
		e.Cancel = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.CFL.frm_CFL));
		this.grd_CFL = new Apzon.Controls.Grid.GridControlEx();
		this.gv_CFL = new Apzon.Controls.Grid.GridViewEx();
		this.repositoryItemLookUpEdit_Type = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rtxt = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.txt_Search = new Apzon.Controls.Editor.ButtonEditEx();
		this.lb_Search = new Apzon.Controls.Controls.LabelLine();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnChoose = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Export = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Collapse = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_New = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panel_Data = new DevExpress.XtraEditors.PanelControl();
		((System.ComponentModel.ISupportInitialize)this.grd_CFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_CFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rtxt).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Search.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panel_Data).BeginInit();
		this.panel_Data.SuspendLayout();
		base.SuspendLayout();
		this.grd_CFL.AllowPasteExcel = false;
		this.grd_CFL.ClipboardData = null;
		this.grd_CFL.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_CFL.Location = new System.Drawing.Point(2, 2);
		this.grd_CFL.MainView = this.gv_CFL;
		this.grd_CFL.Name = "grd_CFL";
		this.grd_CFL.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemLookUpEdit_Type, this.rtxt });
		this.grd_CFL.Size = new System.Drawing.Size(480, 464);
		this.grd_CFL.TabIndex = 1;
		this.grd_CFL.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_CFL });
		this.gv_CFL.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.gv_CFL.Appearance.SelectedRow.Options.UseBackColor = true;
		this.gv_CFL.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_CFL.ColumnKey = null;
		this.gv_CFL.DisableFormSetting = false;
		this.gv_CFL.DisplayException = false;
		this.gv_CFL.FieldCurrency = null;
		this.gv_CFL.FieldRate = null;
		this.gv_CFL.FormId = "oCFL";
		this.gv_CFL.GridControl = this.grd_CFL;
		this.gv_CFL.IndicatorWidth = 60;
		this.gv_CFL.IsApzTable = false;
		this.gv_CFL.IsInsertNewRow = false;
		this.gv_CFL.IsLoadingData = false;
		this.gv_CFL.ItemId = "ODATA";
		this.gv_CFL.LstColumnAllowBatch = null;
		this.gv_CFL.LstColumnNotAllowBatch = null;
		this.gv_CFL.LstColumnNotEditable = null;
		this.gv_CFL.LstColumNotSetting = null;
		this.gv_CFL.Name = "gv_CFL";
		this.gv_CFL.OptionsView.ColumnAutoWidth = false;
		this.gv_CFL.OptionsView.EnableAppearanceEvenRow = true;
		this.gv_CFL.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.gv_CFL.UseBatchMod = false;
		this.gv_CFL.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_CFL_ShowingEditor);
		this.gv_CFL.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gv_CFL_KeyDown);
		this.gv_CFL.DoubleClick += new System.EventHandler(this.gv_CFL_DoubleClick);
		this.repositoryItemLookUpEdit_Type.AutoHeight = false;
		this.repositoryItemLookUpEdit_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit_Type.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Text", " ")
		});
		this.repositoryItemLookUpEdit_Type.Name = "repositoryItemLookUpEdit_Type";
		this.repositoryItemLookUpEdit_Type.NullText = "";
		this.rtxt.AutoHeight = false;
		this.rtxt.Name = "rtxt";
		this.txt_Search.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Search.EditControl = null;
		this.txt_Search.FieldName = null;
		this.txt_Search.IsSearchControl = false;
		this.txt_Search.Location = new System.Drawing.Point(75, 3);
		this.txt_Search.Name = "txt_Search";
		this.txt_Search.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)
		});
		this.txt_Search.SettingInVisible = false;
		this.txt_Search.SettingReadonly = false;
		this.txt_Search.Size = new System.Drawing.Size(337, 20);
		this.txt_Search.TabIndex = 0;
		this.txt_Search.ToolTip = "Search by Item code";
		this.txt_Search.EditValueChanged += new System.EventHandler(this.txt_Search_EditValueChanged);
		this.lb_Search.EditControl = null;
		this.lb_Search.EditValue = "Search";
		this.lb_Search.FieldName = null;
		this.lb_Search.HideWhenEmpty = false;
		this.lb_Search.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Search.Location = new System.Drawing.Point(0, 8);
		this.lb_Search.Name = "lb_Search";
		this.lb_Search.ReadOnly = false;
		this.lb_Search.SettingInVisible = false;
		this.lb_Search.SettingReadonly = false;
		this.lb_Search.ShowUnderLine = true;
		this.lb_Search.Size = new System.Drawing.Size(80, 20);
		this.lb_Search.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Search.TabIndex = 46;
		this.lb_Search.Utext = "Search";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(81, 505);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 3;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnChoose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnChoose.EditControl = null;
		this.btnChoose.EditValue = "Choose";
		this.btnChoose.FieldName = null;
		this.btnChoose.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnChoose.ImageOptions.Image");
		this.btnChoose.IsSearchControl = false;
		this.btnChoose.Location = new System.Drawing.Point(0, 505);
		this.btnChoose.Name = "btnChoose";
		this.btnChoose.ReadOnly = false;
		this.btnChoose.SettingInVisible = false;
		this.btnChoose.SettingReadonly = false;
		this.btnChoose.Size = new System.Drawing.Size(75, 23);
		this.btnChoose.TabIndex = 2;
		this.btnChoose.Text = "Choose";
		this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
		this.btn_Export.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Export.EditControl = null;
		this.btn_Export.EditValue = "Export";
		this.btn_Export.FieldName = null;
		this.btn_Export.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Export.ImageOptions.Image");
		this.btn_Export.IsSearchControl = false;
		this.btn_Export.Location = new System.Drawing.Point(411, 505);
		this.btn_Export.Name = "btn_Export";
		this.btn_Export.ReadOnly = false;
		this.btn_Export.SettingInVisible = false;
		this.btn_Export.SettingReadonly = false;
		this.btn_Export.Size = new System.Drawing.Size(61, 23);
		this.btn_Export.TabIndex = 47;
		this.btn_Export.Text = "Export";
		this.btn_Export.Click += new System.EventHandler(this.btn_Export_Click);
		this.btn_Collapse.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Collapse.EditControl = null;
		this.btn_Collapse.EditValue = "Collapse";
		this.btn_Collapse.FieldName = null;
		this.btn_Collapse.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Collapse.ImageOptions.Image");
		this.btn_Collapse.IsSearchControl = false;
		this.btn_Collapse.Location = new System.Drawing.Point(310, 505);
		this.btn_Collapse.Name = "btn_Collapse";
		this.btn_Collapse.ReadOnly = false;
		this.btn_Collapse.SettingInVisible = false;
		this.btn_Collapse.SettingReadonly = false;
		this.btn_Collapse.Size = new System.Drawing.Size(83, 23);
		this.btn_Collapse.TabIndex = 48;
		this.btn_Collapse.Text = "Collapse";
		this.btn_Collapse.Visible = false;
		this.btn_Collapse.Click += new System.EventHandler(this.btn_Collapse_Click);
		this.btn_New.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_New.EditControl = null;
		this.btn_New.EditValue = "New";
		this.btn_New.FieldName = null;
		this.btn_New.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_New.ImageOptions.Image");
		this.btn_New.IsSearchControl = false;
		this.btn_New.Location = new System.Drawing.Point(162, 505);
		this.btn_New.Name = "btn_New";
		this.btn_New.ReadOnly = false;
		this.btn_New.SettingInVisible = false;
		this.btn_New.SettingReadonly = false;
		this.btn_New.Size = new System.Drawing.Size(75, 23);
		this.btn_New.TabIndex = 49;
		this.btn_New.Text = "New";
		this.btn_New.Click += new System.EventHandler(this.btn_New_Click);
		this.panel_Data.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel_Data.Controls.Add(this.grd_CFL);
		this.panel_Data.Location = new System.Drawing.Point(0, 31);
		this.panel_Data.Name = "panel_Data";
		this.panel_Data.Size = new System.Drawing.Size(484, 468);
		this.panel_Data.TabIndex = 50;
		base.AcceptButton = this.btnChoose;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(484, 532);
		base.Controls.Add(this.panel_Data);
		base.Controls.Add(this.btn_New);
		base.Controls.Add(this.btn_Collapse);
		base.Controls.Add(this.btn_Export);
		base.Controls.Add(this.txt_Search);
		base.Controls.Add(this.lb_Search);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnChoose);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oCFL";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_CFL";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Choose From List";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_CFL_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_CFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_CFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rtxt).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Search.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panel_Data).EndInit();
		this.panel_Data.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
