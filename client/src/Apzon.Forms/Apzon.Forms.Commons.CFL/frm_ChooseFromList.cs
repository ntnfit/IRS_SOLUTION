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
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.Payments;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.Production_Addon;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
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
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Forms.Commons.CFL;

public class frm_ChooseFromList : XtraFormEx
{
	public bool CFL_MutiSelectMode;

	public Enums.TypeCFL CFL_Type;

	public dynamic ChooseItem;

	private Enums.ItemObjectType _itemObjectType;

	public List<dynamic> ChooseItems;

	public string ItemCode = string.Empty;

	public string StartAccount = string.Empty;

	public string WhsCode = string.Empty;

	public int BatchSerialTrantype = -1;

	public int BatchSerialAbs;

	public int _objectType;

	public int _absEntry;

	public int _lineNum;

	public string BinCode = "";

	public DataTable dtItems = new DataTable();

	public string strSearch = string.Empty;

	public DataTable table = new DataTable();

	public string FormattedSearch = string.Empty;

	private string _vatType = "";

	private string _itemCode;

	public string _PriceListItemCode = "";

	public string _TranType = "";

	public bool _isBOMSearch = true;

	public DataTable _dtBOMSearch = new DataTable();

	public string Coutry;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public string _AccountType;

	public DataTable _dtSearchHoliday = new DataTable();

	public HolidayImpl _docSearch = new HolidayImpl();

	public DataTable _dtSource = new DataTable();

	public string LstForecast = string.Empty;

	public string LstMrpSalesOrder = string.Empty;

	public DataTable DataInventoryTransferCopy = new DataTable();

	public DataTable DataEmpJobVacancy = new DataTable();

	public DataTable DataApplicant = new DataTable();

	public DataTable DataApplicantInOffer = new DataTable();

	public DataTable DataTaskInTeam = new DataTable();

	public DataTable DataJetstarVendorPreview = new DataTable();

	public DataTable JetstarDataFltSort = new DataTable();

	public DataTable DataBarcodeCounting = new DataTable();

	private GridHitInfo downHitInfo;

	private List<Enums.TypeCFL> _listTypeDataRow = new List<Enums.TypeCFL> { Enums.TypeCFL.Warehouse };

	public DataTable DataSerialDetail;

	public string ListXcludeOrder;

	public string LstSchedule;

	private DataTable _activeFilterSource = new DataTable();

	public Action AfterLoadForm;

	private IContainer components;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit_Type;

	private ButtonEditEx txt_Search;

	private LabelLine lb_Search;

	private RepositoryItemTextEdit rtxt;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnChoose;

	public GridViewEx gv_CFL;

	public GridControlEx grd_CFL;

	public string cardCode { get; set; }

	public string LstProjectCode { get; set; }

	public DataTable FilterSourceValue { get; set; }

	public string ValueFilterSource { get; set; }

	public DateTime? PostingDate { get; set; }

	public string _strOcrDistRule { get; set; }

	public frm_ChooseFromList(Enums.TypeCFL type, bool multiSelect, string txtText, string vatType = "", Enums.ItemObjectType itemObjectType = Enums.ItemObjectType.NoItemType, string itemId = "")
	{
		InitializeComponent();
		SettingLanguage();
		CFL_Type = type;
		CFL_MutiSelectMode = multiSelect;
		strSearch = txtText;
		ChooseItems = new List<object>();
		_itemObjectType = itemObjectType;
		if (type == Enums.TypeCFL.BatchNumbers || type == Enums.TypeCFL.WareHouseByItemCode)
		{
			_itemCode = vatType;
		}
		else
		{
			_vatType = vatType;
		}
		gv_CFL.FormId = FormId;
		if (type == Enums.TypeCFL.FormattedSearch && !string.IsNullOrWhiteSpace(itemId))
		{
			gv_CFL.ItemId = itemId;
		}
		else
		{
			GridViewEx gridViewEx = gv_CFL;
			int num = (int)type;
			gridViewEx.ItemId = num.ToString();
		}
		OnLoadAsync += frm_ChooseFromList_Load;
	}

	private void SettingLanguage()
	{
		lb_Search.Utext = Apzon.Commons.Properties.Resources.ButtonSearch;
		btnChoose.Text = Apzon.Commons.Properties.Resources.ButtonChoose;
		btnCancel.Text = Apzon.Commons.Properties.Resources.ButtonCancel;
	}

	private async Task frm_ChooseFromList_Load(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		await LoadForm();
		if (AfterLoadForm != null)
		{
			AfterLoadForm();
		}
		if (!string.IsNullOrEmpty(strSearch))
		{
			txt_Search.Text = strSearch;
		}
		WinCommons._CloseProcessing();
	}

	private void gv_CFL_DoubleClick(object sender, EventArgs e)
	{
		if (CFL_Type != Enums.TypeCFL.CustomLineInventoryTrnasfer && CFL_Type != Enums.TypeCFL.JetstarFltSort)
		{
			DXMouseEventArgs e2 = e as DXMouseEventArgs;
			GridHitInfo gridHitInfo = gv_CFL.CalcHitInfo(e2.Location);
			if (!CFL_MutiSelectMode && gridHitInfo.InRow)
			{
				AddChooseItemsGrid();
				Close();
			}
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

	private void frm_ChooseFromList_FormClosed(object sender, FormClosedEventArgs e)
	{
	}

	private void txt_Search_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string text = "";
			if (!_activeFilterSource.IsNotNull())
			{
				foreach (GridColumn visibleColumn in gv_CFL.VisibleColumns)
				{
					text += $"OR [{visibleColumn.FieldName}] LIKE '%{txt_Search.Text}%' ";
				}
			}
			else
			{
				foreach (GridColumn visibleColumn2 in gv_CFL.VisibleColumns)
				{
					if (_activeFilterSource.ExistsColumn(visibleColumn2.FieldName))
					{
						text += $"OR [{visibleColumn2.FieldName}] LIKE '%{txt_Search.Text}%' ";
					}
				}
			}
			text = ((text.Length > 2) ? text.Substring(2) : "");
			gv_CFL.ActiveFilterString = text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void btnChoose_Click(object sender, EventArgs e)
	{
		try
		{
			if (CFL_Type == Enums.TypeCFL.CustomLineInventoryTrnasfer)
			{
				DataInventoryTransferCopy.Clear();
				List<DataRow> listDataSourceSelectedRow = gv_CFL.GetListDataSourceSelectedRow();
				if (listDataSourceSelectedRow != null && listDataSourceSelectedRow.Any())
				{
					DataInventoryTransferCopy = listDataSourceSelectedRow.CopyToDataTable();
					List<DataRow> list = (from t in DataInventoryTransferCopy.AsEnumerable()
						where Math.Abs(Function.ParseDouble(t["Quantity"])) < 1E-07
						select t).ToList();
					for (int num = 0; num < list.Count; num++)
					{
						DataInventoryTransferCopy.Rows.Remove(list[num]);
					}
					DataInventoryTransferCopy.AcceptChanges();
				}
			}
			if (CFL_Type == Enums.TypeCFL.JetstarFltSort)
			{
				grd_CFL.DataSourceTable.AcceptChanges();
				JetstarDataFltSort = grd_CFL.DataSourceTable;
				JetstarDataFltSort.AcceptChanges();
			}
			else
			{
				AddChooseItemsGrid();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
		Close();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async Task LoadForm()
	{
		gv_CFL.ShowLoadingPanel();
		try
		{
			if (CFL_MutiSelectMode)
			{
				gv_CFL.OptionsSelection.MultiSelect = true;
				gv_CFL.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
			}
			switch (CFL_Type)
			{
			case Enums.TypeCFL.None:
				return;
			case Enums.TypeCFL.ListItems:
				LoadColumnsListItem();
				await LoadDataListItems();
				break;
			case Enums.TypeCFL.Items:
				LoadColumnsItem();
				await LoadDataItems();
				break;
			case Enums.TypeCFL.ItemsInventory:
				LoadColumnsItemInventory();
				await LoadDataItemsInventory();
				break;
			case Enums.TypeCFL.ItemsTypeI:
				LoadColumnsItemType();
				await LoadDataItemsByItemType("I");
				break;
			case Enums.TypeCFL.ItemsTypeL:
				LoadColumnsItemType();
				await LoadDataItemsByItemType("L");
				break;
			case Enums.TypeCFL.ItemsTypeT:
				LoadColumnsItemType();
				await LoadDataItemsByItemType("T");
				break;
			case Enums.TypeCFL.ItemsByGroup:
				LoadColumnsItemType();
				await LoadDataItemsByGroup();
				break;
			case Enums.TypeCFL.Bp:
				LoadColumnsBp();
				await LoadDataBp();
				break;
			case Enums.TypeCFL.Vendor:
				LoadColumnsVendor();
				await LoadDataVendor();
				break;
			case Enums.TypeCFL.Lead:
				LoadColumnsLead();
				await LoadDataLead();
				break;
			case Enums.TypeCFL.Customer:
				LoadColumnsCustomer();
				await LoadDataCustomer();
				break;
			case Enums.TypeCFL.ContactPerson:
				LoadColumnsContactPerson();
				await LoadDataListContactPerson();
				break;
			case Enums.TypeCFL.SaleEmployee:
				LoadColumnsSaleEmployee();
				await LoadDataSaleEmployee();
				break;
			case Enums.TypeCFL.CustomerNew:
				LoadColumnsCustomer();
				await LoadDataCustomer("C");
				break;
			case Enums.TypeCFL.VendorNew:
				LoadColumnsCustomer();
				await LoadDataCustomer("S");
				break;
			case Enums.TypeCFL.LeadNew:
				LoadColumnsCustomer();
				await LoadDataCustomer("L");
				break;
			case Enums.TypeCFL.ChartOfAccount:
				LoadColumnsAccount();
				await LoadDataAccount();
				break;
			case Enums.TypeCFL.Shipping:
				LoadColumnsShipping();
				await LoadDataShipping();
				break;
			case Enums.TypeCFL.Manufacture:
				LoadColumnsManufacture();
				await LoadDataManufacture();
				break;
			case Enums.TypeCFL.ItemGroup:
				LoadColumnsItemGroup();
				await LoadDataItemGroup();
				break;
			case Enums.TypeCFL.Vat:
				LoadColumnsVatGroup();
				await LoadDataVatGroup();
				break;
			case Enums.TypeCFL.VatIn:
				LoadColumnsVatGroup();
				await LoadDataVatGroup("I");
				break;
			case Enums.TypeCFL.VatOut:
				LoadColumnsVatGroup();
				await LoadDataVatGroup("O");
				break;
			case Enums.TypeCFL.PriceList:
				LoadColumnsPriceList();
				await LoadDataPriceList();
				break;
			case Enums.TypeCFL.Warehouse:
			{
				Task task2 = LoadDataWhs();
				LoadColumnsWhs();
				await task2;
				await WinFuntions.LoadUdfColumns(gv_CFL, "OWHS");
				break;
			}
			case Enums.TypeCFL.WarehouseFromDocuments:
			{
				Task task = LoadDataWhsDocument();
				LoadColumnsWhsDocument();
				await task;
				await WinFuntions.LoadUdfColumns(gv_CFL, "OWHS");
				break;
			}
			case Enums.TypeCFL.Requester:
				LoadColumnsRequester();
				await LoadDataRequester();
				break;
			case Enums.TypeCFL.RequesterByUser:
				LoadColumnsRequesterByUser();
				await LoadDataRequesterByUser();
				break;
			case Enums.TypeCFL.RequesterByEmployee:
				LoadColumnsRequesterByEmployee();
				await LoadDataRequesterByEmployee();
				break;
			case Enums.TypeCFL.AcctCode:
				LoadColumnsAcctCode();
				await LoadDataAcctCode();
				break;
			case Enums.TypeCFL.ChartOfAccountPurpose:
				LoadColumnsAccountPurpose();
				await LoadDataAccountPurpose();
				break;
			case Enums.TypeCFL.BpCatalog:
				LoadColumnsBPCatalog();
				await LoadDataBpCatalog();
				break;
			case Enums.TypeCFL.BpCatalogByCardCode:
				LoadColumnsBPCatalogByCardCode();
				await LoadDataBpCatalogByCardCode();
				break;
			case Enums.TypeCFL.BOM:
				LoadColumnBOM();
				await LoadDataBOM();
				break;
			case Enums.TypeCFL.BillOfMater:
				LoadColumnsBillOfMater();
				await LoadDataBillOfMater();
				break;
			case Enums.TypeCFL.BoMItem:
				LoadColumnsBoMItem();
				await LoadDataBoMItem();
				break;
			case Enums.TypeCFL.BoMJobResource:
				LoadColumnsBoMJobResource();
				await LoadDataBoMJobResource();
				break;
			case Enums.TypeCFL.ItemProduction:
				LoadColumnsItemProduction();
				await LoadDataItemProduction();
				break;
			case Enums.TypeCFL.ItemManualProduction:
				txt_Search.Visible = false;
				lb_Search.Visible = false;
				LoadColumnsItemProduction();
				await LoadDataItemManualProduction();
				break;
			case Enums.TypeCFL.StandardProductionOrder:
				LoadColumnsStdProOrder();
				await LoadDataStdProOrder();
				break;
			case Enums.TypeCFL.SalesOder:
				LoadColumnsSalesOder();
				await LoadDataSalesOder();
				break;
			case Enums.TypeCFL.ListPurchaseRequest:
				LoadColumnsListPurchaseRequest();
				await LoadDataListPurchaseRequest();
				break;
			case Enums.TypeCFL.FormattedSearch:
				LoadDataFormatedSearch();
				break;
			case Enums.TypeCFL.BatchNumbers:
				LoadColumnsBatch();
				await LoadDataColumnsBatch(_itemCode);
				break;
			case Enums.TypeCFL.WareHouseByItemCode:
				LoadColumnsWareHouseByItemCode();
				await LoadDataColumnsWareHouseByItemCode(_itemCode);
				break;
			case Enums.TypeCFL.ForeCast:
				LoadColumnsForeCast();
				await LoadDataForeCast();
				break;
			case Enums.TypeCFL.MRP:
				LoadColumnsMRP();
				await LoadDataMRP();
				break;
			case Enums.TypeCFL.ProductionOrder:
				LoadColumnsProductionOrder();
				await LoadDataProductionOrder();
				break;
			case Enums.TypeCFL.CashFlow:
				LoadColumnsCashFlow();
				await LoadDataCashFlow();
				break;
			case Enums.TypeCFL.OrcCode:
				LoadColumnsOrcCode(1);
				await LoadDataOrcCode(1);
				break;
			case Enums.TypeCFL.OrcCode2:
				LoadColumnsOrcCode(2);
				await LoadDataOrcCode(2);
				break;
			case Enums.TypeCFL.OrcCode3:
				LoadColumnsOrcCode(3);
				await LoadDataOrcCode(3);
				break;
			case Enums.TypeCFL.OrcCode4:
				LoadColumnsOrcCode(4);
				await LoadDataOrcCode(4);
				break;
			case Enums.TypeCFL.OrcCode5:
				LoadColumnsOrcCode(5);
				await LoadDataOrcCode(5);
				break;
			case Enums.TypeCFL.ControlKey:
				LoadColumnsControlKey();
				await LoadDataControlKey();
				break;
			case Enums.TypeCFL.Resource:
				LoadColumnsResource();
				await LoadDataResource();
				break;
			case Enums.TypeCFL.GroupControlKey:
				LoadColumnsGroupControlKey();
				await LoadDataGroupControlKey();
				break;
			case Enums.TypeCFL.Operation:
				LoadColumnsOperation();
				await LoadDataOperation();
				break;
			case Enums.TypeCFL.OperationByStatus:
				LoadColumnsOperationByStatus();
				await LoadDataOperationByStatus();
				break;
			case Enums.TypeCFL.Routing:
				LoadColumnsRouting();
				await LoadDataRouting();
				break;
			case Enums.TypeCFL.Project:
				LoadColumnsProject();
				await LoadDataProject();
				break;
			case Enums.TypeCFL.Employees:
				await LoadDataEmployees();
				break;
			case Enums.TypeCFL.Bin:
				await LoadDataBin();
				break;
			case Enums.TypeCFL.RevennueAccount:
				LoadColumnsRevenueAccount();
				await LoadDataRevenueAccount();
				break;
			case Enums.TypeCFL.Owner:
				await LoadDataOwner();
				break;
			case Enums.TypeCFL.ListOfInstallBase:
				LoadColumnsCountry();
				await LoadDataCountry();
				break;
			case Enums.TypeCFL.State:
				LoadColumnsState();
				await LoadDataState();
				break;
			case Enums.TypeCFL.Holiday:
				LoadColumnsHoliday();
				await LoadDataHoliday();
				break;
			case Enums.TypeCFL.DistributeRule:
				await DistributeRule();
				break;
			case Enums.TypeCFL.AdvanceForecast:
				LoadColumnsAdvanceForecast();
				await LoadDataAdvaceForecast();
				break;
			case Enums.TypeCFL.Schedule:
				LoadColumnsSchedule();
				await LoadDataSchedule();
				break;
			case Enums.TypeCFL.MrpSalesOrder:
				LoadColumnsMrpSalesOrder();
				await LoadDataMrpSalesOrder();
				break;
			case Enums.TypeCFL.CustomLineInventoryTrnasfer:
				LoadCustomLineInventoryTrnasfer();
				break;
			case Enums.TypeCFL.SerialDetail:
				LoadColumnSerial();
				grd_CFL.DataSource = DataSerialDetail;
				grd_CFL.RefreshDataSource();
				break;
			case Enums.TypeCFL.GetListEmpId:
				LoadEmpJobVacancy();
				break;
			case Enums.TypeCFL.GetListAppNo:
				LoadApplicantNo();
				break;
			case Enums.TypeCFL.GetListApplicantInOffer:
				LoadApplicantInOffer();
				break;
			case Enums.TypeCFL.MaterialShortageReport:
				LoadColumnsMaterialShortageReport();
				await LoadDataMaterialShortageReport();
				break;
			case Enums.TypeCFL.ListBaseOnInputForecast:
				LoadColumnBaseOnInputForecast();
				await LoadDataBaseOnInputForecast();
				break;
			case Enums.TypeCFL.MrpProject:
				LoadColumnsMrpProject();
				await LoadDataMrpProject();
				break;
			case Enums.TypeCFL.GetTaskList:
				GetTaskInItem();
				break;
			case Enums.TypeCFL.DocumentByWbs:
				btnChoose.Text = "OK";
				await ProjectManagerItemDocument(_objectType, _absEntry, _lineNum);
				break;
			case Enums.TypeCFL.JetstarPreviewBp:
				LoadColumJetstarPreviewBp();
				break;
			case Enums.TypeCFL.JetstarFltSort:
				LoadColumnJetstarFltSort();
				break;
			case Enums.TypeCFL.ChooseFilterSourceValue:
				LoadFilterSourceValue();
				break;
			case Enums.TypeCFL.BarcodeInventoryCounting:
				LoadDataBarcodeInventoryCounting();
				break;
			case Enums.TypeCFL.MaterialRevaluation:
				LoadColumnRevaluation();
				await LoadDataRevaluation();
				break;
			case Enums.TypeCFL.OutgoingPayment:
				LoadColumnPayment(1);
				await LoadDataPayment(1);
				break;
			case Enums.TypeCFL.IncomingPayment:
				LoadColumnPayment(2);
				await LoadDataPayment(2);
				break;
			case Enums.TypeCFL.TrackingIssue:
				LoadColumnTracking();
				await LoadDataTracking();
				break;
			case Enums.TypeCFL.BlanketAgreement:
				LoadColumnBlanket();
				await LoadDataBlanket();
				break;
			}
			gv_CFL.ClearSelection();
			_activeFilterSource = grd_CFL.DataSource as DataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
		gv_CFL.HideLoadingPanel();
	}

	private void LoadColumJetstarPreviewBp()
	{
		try
		{
			Text = "Choose Vendor Preview";
			GridColumnEx gridColumnEx = new GridColumnEx();
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			gridColumnEx.Name = "cl_Code";
			gridColumnEx.Caption = "Vendor code";
			gridColumnEx.FieldName = "CardCode";
			gridColumnEx.Width = 150;
			gridColumnEx.Visible = true;
			gridColumnEx.OptionsColumn.ReadOnly = true;
			gridColumnEx2.Name = "cl_Name";
			gridColumnEx2.Caption = "Tên nhà CC viết tắt";
			gridColumnEx2.FieldName = "ShortName";
			gridColumnEx2.Width = 150;
			gridColumnEx2.Visible = true;
			gridColumnEx2.OptionsColumn.ReadOnly = true;
			gv_CFL.Columns.Clear();
			gv_CFL.Columns.AddRange(new GridColumn[2] { gridColumnEx2, gridColumnEx });
			gv_CFL.OptionsBehavior.ReadOnly = false;
			gv_CFL.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
			grd_CFL.DataSource = DataJetstarVendorPreview;
			grd_CFL.DataSourceTable.AcceptChanges();
			grd_CFL.RefreshDataSource();
			btnChoose.Text = "Ok";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnJetstarFltSort()
	{
		try
		{
			Text = "Manual sort flight";
			GridColumnEx gridColumnEx = new GridColumnEx();
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			GridColumnEx gridColumnEx3 = new GridColumnEx();
			GridColumnEx gridColumnEx4 = new GridColumnEx();
			gridColumnEx.Name = "cl_FltNo";
			gridColumnEx.Caption = "Mã chuyến bay";
			gridColumnEx.FieldName = "FltNo";
			gridColumnEx.Width = 100;
			gridColumnEx.Visible = true;
			gridColumnEx.OptionsColumn.ReadOnly = true;
			gridColumnEx2.Name = "cl_Sector";
			gridColumnEx2.Caption = "Chặng bay";
			gridColumnEx2.FieldName = "Sector";
			gridColumnEx2.Width = 80;
			gridColumnEx2.Visible = true;
			gridColumnEx2.OptionsColumn.ReadOnly = true;
			gridColumnEx3.Name = "cl_FltTime";
			gridColumnEx3.Caption = "Giờ bay";
			gridColumnEx3.FieldName = "FltTime";
			gridColumnEx3.Width = 80;
			gridColumnEx3.Visible = true;
			gridColumnEx3.OptionsColumn.ReadOnly = true;
			gridColumnEx3.DisplayFormatType = Enums.DisplayFormatType.Time;
			gridColumnEx4.Name = "cl_FltDate";
			gridColumnEx4.Caption = "Ngày bay";
			gridColumnEx4.FieldName = "FltDate";
			gridColumnEx4.Width = 80;
			gridColumnEx4.Visible = true;
			gridColumnEx4.OptionsColumn.ReadOnly = true;
			gridColumnEx4.DisplayFormatType = Enums.DisplayFormatType.DateTime;
			gv_CFL.Columns.Clear();
			gv_CFL.Columns.AddRange(new GridColumn[4] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4 });
			gv_CFL.OptionsBehavior.ReadOnly = true;
			grd_CFL.DataSource = JetstarDataFltSort;
			grd_CFL.DataSourceTable.AcceptChanges();
			grd_CFL.RefreshDataSource();
			btnChoose.Text = "Ok";
			grd_CFL.AllowDrop = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataBaseOnInputForecast()
	{
		try
		{
			DataTable dataSource = await _httpClient.Cfl.GetDataBaseOnInputForecast();
			grd_CFL.DataSource = dataSource;
			grd_CFL.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnBaseOnInputForecast()
	{
		try
		{
			Text = "Forecast Detail";
			GridColumnEx gridColumnEx = new GridColumnEx();
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			gridColumnEx.Name = "cl_Code";
			gridColumnEx.Caption = "Code";
			gridColumnEx.FieldName = "Code";
			gridColumnEx.Width = 150;
			gridColumnEx.Visible = true;
			gridColumnEx.OptionsColumn.ReadOnly = true;
			gridColumnEx2.Name = "cl_Name";
			gridColumnEx2.Caption = "Name";
			gridColumnEx2.FieldName = "Name";
			gridColumnEx2.Width = 150;
			gridColumnEx2.Visible = true;
			gridColumnEx2.OptionsColumn.ReadOnly = true;
			gv_CFL.Columns.Clear();
			GridColumnCollection columns = gv_CFL.Columns;
			GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
			columns.AddRange(columns2);
			gv_CFL.OptionsBehavior.ReadOnly = false;
			grd_CFL.DataSource = DataInventoryTransferCopy;
			grd_CFL.DataSourceTable.AcceptChanges();
			grd_CFL.RefreshDataSource();
			btnChoose.Text = "Ok";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadCustomLineInventoryTrnasfer()
	{
		try
		{
			gv_CFL.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
			Text = "Select Item to Copy";
			GridColumnEx gridColumnEx = new GridColumnEx();
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			GridColumnEx gridColumnEx3 = new GridColumnEx();
			GridColumnEx gridColumnEx4 = new GridColumnEx();
			GridColumnEx gridColumnEx5 = new GridColumnEx();
			GridColumnEx gridColumnEx6 = new GridColumnEx();
			GridColumnEx gridColumnEx7 = new GridColumnEx();
			GridColumnEx gridColumnEx8 = new GridColumnEx();
			gridColumnEx.Name = "cl_DocNum";
			gridColumnEx.Caption = "Base Document";
			gridColumnEx.FieldName = "DocNum";
			gridColumnEx.Width = 150;
			gridColumnEx.Visible = true;
			gridColumnEx.OptionsColumn.ReadOnly = true;
			gridColumnEx2.Name = "cl_ItemNo";
			gridColumnEx2.Caption = "Item No.";
			gridColumnEx2.FieldName = "ItemCode";
			gridColumnEx2.Width = 150;
			gridColumnEx2.Visible = true;
			gridColumnEx2.OptionsColumn.ReadOnly = true;
			gridColumnEx3.Name = "cl_ItemName";
			gridColumnEx3.Caption = "Item Description";
			gridColumnEx3.FieldName = "ItemName";
			gridColumnEx3.Width = 150;
			gridColumnEx3.Visible = true;
			gridColumnEx3.OptionsColumn.ReadOnly = true;
			gridColumnEx4.Name = "cl_ToWhs";
			gridColumnEx4.Caption = "To Warehouse";
			gridColumnEx4.FieldName = "WhsCode";
			gridColumnEx4.Width = 150;
			gridColumnEx4.Visible = true;
			gridColumnEx4.OptionsColumn.ReadOnly = true;
			gridColumnEx5.Name = "cl_FromWhs";
			gridColumnEx5.Caption = "From Warehouse";
			gridColumnEx5.FieldName = "FromWhsCod";
			gridColumnEx5.Width = 150;
			gridColumnEx5.Visible = true;
			gridColumnEx5.OptionsColumn.ReadOnly = true;
			gridColumnEx6.Name = "cl_Qty";
			gridColumnEx6.Caption = "Qty";
			gridColumnEx6.FieldName = "Quantity";
			gridColumnEx6.Width = 150;
			gridColumnEx6.Visible = true;
			gridColumnEx6.OptionsColumn.ReadOnly = true;
			gridColumnEx6.OptionsColumn.ReadOnly = false;
			gridColumnEx6.ColumnEdit = new RepositoryItemCalcEdit();
			gridColumnEx7.Name = "cl_UoMCode";
			gridColumnEx7.Caption = "UoM Code";
			gridColumnEx7.FieldName = "UomCode";
			gridColumnEx7.Width = 150;
			gridColumnEx7.Visible = true;
			gridColumnEx7.OptionsColumn.ReadOnly = true;
			gridColumnEx7.OptionsColumn.ReadOnly = true;
			gridColumnEx8.Name = "cl_unitMsr";
			gridColumnEx8.Caption = "Unit Name";
			gridColumnEx8.FieldName = "unitMsr";
			gridColumnEx8.Width = 150;
			gridColumnEx8.Visible = true;
			gridColumnEx8.OptionsColumn.ReadOnly = true;
			gridColumnEx8.OptionsColumn.ReadOnly = true;
			gv_CFL.Columns.Clear();
			GridColumnCollection columns = gv_CFL.Columns;
			GridColumn[] columns2 = new GridColumnEx[8] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5, gridColumnEx6, gridColumnEx7, gridColumnEx8 };
			columns.AddRange(columns2);
			gv_CFL.OptionsBehavior.ReadOnly = false;
			grd_CFL.DataSource = DataInventoryTransferCopy.Copy();
			grd_CFL.DataSourceTable.AcceptChanges();
			grd_CFL.RefreshDataSource();
			btnChoose.Text = "Ok";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsMrpSalesOrder()
	{
		try
		{
			Text = "Forecast Detail";
			GridColumnEx gridColumnEx = new GridColumnEx();
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			GridColumnEx gridColumnEx3 = new GridColumnEx();
			GridColumnEx gridColumnEx4 = new GridColumnEx();
			GridColumnEx gridColumnEx5 = new GridColumnEx();
			gridColumnEx.Name = "cl_DocEntry";
			gridColumnEx.Caption = "DocEntry";
			gridColumnEx.FieldName = "DocEntry";
			gridColumnEx.Width = 150;
			gridColumnEx.Visible = true;
			gridColumnEx2.Name = "cl_DocNum";
			gridColumnEx2.Caption = "DocNum";
			gridColumnEx2.FieldName = "DocNum";
			gridColumnEx2.Width = 150;
			gridColumnEx2.Visible = true;
			gridColumnEx3.Name = "cl_CardCode";
			gridColumnEx3.Caption = "Customer Code";
			gridColumnEx3.FieldName = "CardCode";
			gridColumnEx3.Width = 150;
			gridColumnEx3.Visible = true;
			gridColumnEx4.Name = "cl_CardName";
			gridColumnEx4.Caption = "Customer Name";
			gridColumnEx4.FieldName = "CardName";
			gridColumnEx4.Width = 150;
			gridColumnEx4.Visible = true;
			gridColumnEx5.Name = "cl_DocDate";
			gridColumnEx5.Caption = "Posting Date";
			gridColumnEx5.FieldName = "DocDate";
			gridColumnEx5.Width = 150;
			gridColumnEx5.Visible = true;
			gv_CFL.Columns.Clear();
			GridColumnCollection columns = gv_CFL.Columns;
			GridColumn[] columns2 = new GridColumnEx[5] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5 };
			columns.AddRange(columns2);
			btnChoose.Text = "Ok";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataMrpSalesOrder()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CFL");
			GridControlEx gridControlEx = grd_CFL;
			gridControlEx.DataSource = await http.GetAsync("LoadDataMrpSalesOrder", new { LstMrpSalesOrder });
			grd_CFL.RefreshDataSource();
			grd_CFL.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataAdvaceForecast()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CFL");
			GridControlEx gridControlEx = grd_CFL;
			gridControlEx.DataSource = await http.GetAsync("LoadDataAdvaceForecast", new { LstForecast });
			grd_CFL.RefreshDataSource();
			grd_CFL.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsAdvanceForecast()
	{
		try
		{
			Text = "Forecast Detail";
			GridColumnEx gridColumnEx = new GridColumnEx();
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			gridColumnEx.Name = "cl_Code";
			gridColumnEx.Caption = "Forecast Code";
			gridColumnEx.FieldName = "Code";
			gridColumnEx.Width = 200;
			gridColumnEx.Visible = true;
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit
			{
				AllowMouseWheel = false
			};
			repositoryItemButtonEdit.Buttons[0].IsLeft = true;
			repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Glyph;
			repositoryItemButtonEdit.Buttons[0].Image = Apzon.Forms.Properties.Resources.right;
			repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs ev) =>
			{
				ChooseItem = gv_CFL.GetFocusedDataRow();
				Close();
			};
			gridColumnEx.ColumnEdit = repositoryItemButtonEdit;
			gridColumnEx.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
			gridColumnEx2.Name = "cl_Name";
			gridColumnEx2.Caption = "Forecast Name";
			gridColumnEx2.FieldName = "Name";
			gridColumnEx2.Width = 150;
			gridColumnEx2.Visible = true;
			gv_CFL.Columns.Clear();
			GridColumnCollection columns = gv_CFL.Columns;
			GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
			columns.AddRange(columns2);
			btnChoose.Text = "Ok";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsMrpProject()
	{
		try
		{
			Text = "Project Detail";
			GridColumnEx gridColumnEx = new GridColumnEx();
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			gridColumnEx.Name = "cl_Code";
			gridColumnEx.Caption = "Project Code";
			gridColumnEx.FieldName = "PrjCode";
			gridColumnEx.Width = 200;
			gridColumnEx.Visible = true;
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit
			{
				AllowMouseWheel = false
			};
			repositoryItemButtonEdit.Buttons[0].IsLeft = true;
			repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Glyph;
			repositoryItemButtonEdit.Buttons[0].Image = Apzon.Forms.Properties.Resources.right;
			repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs ev) =>
			{
				ChooseItem = gv_CFL.GetFocusedDataRow();
				Close();
			};
			gridColumnEx.ColumnEdit = repositoryItemButtonEdit;
			gridColumnEx.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
			gridColumnEx2.Name = "cl_Name";
			gridColumnEx2.Caption = "Project Name";
			gridColumnEx2.FieldName = "PrjName";
			gridColumnEx2.Width = 150;
			gridColumnEx2.Visible = true;
			gv_CFL.Columns.Clear();
			GridColumnCollection columns = gv_CFL.Columns;
			GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
			columns.AddRange(columns2);
			btnChoose.Text = "Ok";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataMrpProject()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Project");
			GridControlEx gridControlEx = grd_CFL;
			gridControlEx.DataSource = await http.GetAsync("LadDataMrpProject", new { LstProjectCode });
			grd_CFL.RefreshDataSource();
			grd_CFL.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataMaterialShortageReport()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CFL");
			GridControlEx gridControlEx = grd_CFL;
			gridControlEx.DataSource = await http.GetAsync("LoadDataMaterialShortageReport", new { ListXcludeOrder });
			grd_CFL.RefreshDataSource();
			grd_CFL.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsMaterialShortageReport()
	{
		try
		{
			Text = "Exclude Production Order";
			GridColumnEx gridColumnEx = new GridColumnEx();
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			GridColumnEx gridColumnEx3 = new GridColumnEx();
			gridColumnEx.Name = "cl_Code";
			gridColumnEx.Caption = "DocEntry";
			gridColumnEx.FieldName = "DocEntry";
			gridColumnEx.Width = 200;
			gridColumnEx.Visible = true;
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit
			{
				AllowMouseWheel = false
			};
			repositoryItemButtonEdit.Buttons[0].IsLeft = true;
			repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Glyph;
			repositoryItemButtonEdit.Buttons[0].Image = Apzon.Forms.Properties.Resources.right;
			repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs ev) =>
			{
				ChooseItem = gv_CFL.GetFocusedDataRow();
				Close();
			};
			gridColumnEx.ColumnEdit = repositoryItemButtonEdit;
			gridColumnEx.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
			gridColumnEx2.Name = "cl_Name";
			gridColumnEx2.Caption = "DocNum";
			gridColumnEx2.FieldName = "DocNum";
			gridColumnEx2.Width = 150;
			gridColumnEx2.Visible = true;
			gridColumnEx3.Name = "cl_ItemCOde";
			gridColumnEx3.Caption = "ItemCode";
			gridColumnEx3.FieldName = "ItemCode";
			gridColumnEx3.Width = 150;
			gridColumnEx3.Visible = true;
			gv_CFL.Columns.Clear();
			GridColumnCollection columns = gv_CFL.Columns;
			GridColumn[] columns2 = new GridColumnEx[3] { gridColumnEx, gridColumnEx2, gridColumnEx3 };
			columns.AddRange(columns2);
			btnChoose.Text = "Ok";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task DistributeRule()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CFL");
			gv_CFL.OptionsBehavior.Editable = true;
			gv_CFL.OptionsView.ColumnAutoWidth = false;
			GridColumnEx gridColumnEx = new GridColumnEx();
			gridColumnEx.Caption = "Dimensions code";
			gridColumnEx.FieldName = "DimCode";
			gridColumnEx.Visible = false;
			gridColumnEx.OptionsColumn.AllowEdit = false;
			gridColumnEx.OptionsColumn.ReadOnly = true;
			gv_CFL.Columns.Add(gridColumnEx);
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			gridColumnEx2.Width = 200;
			gridColumnEx2.Caption = "Dimensions Name";
			gridColumnEx2.FieldName = "DimDesc";
			gridColumnEx2.Visible = true;
			gridColumnEx2.OptionsColumn.AllowEdit = false;
			gridColumnEx2.OptionsColumn.ReadOnly = true;
			gv_CFL.Columns.Add(gridColumnEx2);
			GridColumnEx gridColumnEx3 = new GridColumnEx();
			gridColumnEx3.Visible = true;
			gridColumnEx3.Width = 200;
			gridColumnEx3.Caption = "Cost centers";
			gridColumnEx3.FieldName = "OcrCode";
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit.Buttons.RemoveAt(0);
			EditorButton editorButton = new EditorButton();
			editorButton.Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit.Buttons.Add(editorButton);
			gridColumnEx3.ColumnEdit = repositoryItemButtonEdit;
			repositoryItemButtonEdit.ButtonPressed += (object s, ButtonPressedEventArgs e) =>
			{
				string text = Function.ToString(gv_CFL.GetFocusedRowCellValue("DimCode"));
				Enums.TypeCFL type = Enums.TypeCFL.OrcCode;
				switch (text)
				{
				case "2":
					type = Enums.TypeCFL.OrcCode2;
					break;
				case "3":
					type = Enums.TypeCFL.OrcCode3;
					break;
				case "4":
					type = Enums.TypeCFL.OrcCode4;
					break;
				case "5":
					type = Enums.TypeCFL.OrcCode5;
					break;
				}
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(type, multiSelect: false, "");
				frm_ChooseFromList2.ShowDialog();
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
					gv_CFL.SetFocusedRowCellValue("OcrCode", Function.ToString(row["OcrCode"]));
				}
			};
			gv_CFL.Columns.Add(gridColumnEx3);
			DataTable dataTable = await helper.GetAsync("GetAllOcrByDistRule", _docSearch);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				string[] array = WinFuntions.ValidateDistrRule(_strOcrDistRule).Split(';');
				for (int num = 0; num < array.Length; num++)
				{
					if (!string.IsNullOrWhiteSpace(array[num]))
					{
						if (dataTable.Rows.Count <= num)
						{
							break;
						}
						dataTable.Rows[num]["OcrCode"] = array[num];
					}
				}
			}
			grd_CFL.DataSource = dataTable;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataHoliday()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Holiday");
			DataTable dataSource = await helper.GetAsync("GetHoliday", _docSearch);
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
			gv_CFL.PopulateColumns();
			gv_CFL.OptionsView.ColumnAutoWidth = false;
			foreach (GridColumnEx column in gv_CFL.Columns)
			{
				column.Width = 100;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsHoliday()
	{
		Text = "List Of State";
		GridColumnEx gridColumnEx = new GridColumnEx();
		new GridColumnEx();
		gridColumnEx.Name = "HldCode";
		gridColumnEx.Caption = "Holidays Code";
		gridColumnEx.FieldName = "HldCode";
		gridColumnEx.Width = 50;
		gridColumnEx.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[1] { gridColumnEx };
		columns.AddRange(columns2);
	}

	private async Task LoadDataCustomer(string cardType)
	{
		try
		{
			gv_CFL.Columns["CardCode"].FilterMode = ColumnFilterMode.DisplayText;
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BusinessPartner");
			DataTable dataSource = await hp.GetAsync("ListBPByCardType", new
			{
				cardType = cardType,
				BPLId = GlobalData.DefaultBranch
			});
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataOwner()
	{
		try
		{
			Text = "List of Employees";
			GridColumnEx gridColumnEx = new GridColumnEx();
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			GridColumnEx gridColumnEx3 = new GridColumnEx();
			GridColumnEx gridColumnEx4 = new GridColumnEx();
			GridColumnEx gridColumnEx5 = new GridColumnEx();
			gridColumnEx.Name = "clfirstName";
			gridColumnEx.Caption = "First Name";
			gridColumnEx.FieldName = "firstName";
			gridColumnEx.Width = 150;
			gridColumnEx.Visible = true;
			gridColumnEx2.Name = "clmiddleName";
			gridColumnEx2.Caption = "Middle Name";
			gridColumnEx2.FieldName = "middleName";
			gridColumnEx2.Width = 150;
			gridColumnEx2.Visible = true;
			gridColumnEx3.Name = "cllastName";
			gridColumnEx3.Caption = "Last Name";
			gridColumnEx3.FieldName = "lastName";
			gridColumnEx3.Width = 150;
			gridColumnEx3.Visible = true;
			gridColumnEx4.Name = "clempID";
			gridColumnEx4.Caption = "Employee ID";
			gridColumnEx4.FieldName = "empID";
			gridColumnEx4.Width = 50;
			gridColumnEx4.Visible = true;
			gridColumnEx5.Name = "cluserId";
			gridColumnEx5.Caption = "User Id";
			gridColumnEx5.FieldName = "userId";
			gridColumnEx5.Width = 50;
			gridColumnEx5.Visible = true;
			gv_CFL.Columns.Clear();
			GridColumnCollection columns = gv_CFL.Columns;
			GridColumn[] columns2 = new GridColumnEx[5] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5 };
			columns.AddRange(columns2);
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CFL");
			DataTable dataSource = await hp.GetAsync("GetListEmployees");
			grd_CFL.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataBin()
	{
		try
		{
			DataTable dataSource = await _httpClient.InventoryCountingBin.LoadAllBin(ItemCode, WhsCode, BatchSerialAbs, BatchSerialTrantype, BinCode);
			grd_CFL.DataSource = dataSource;
			gv_CFL.PopulateColumns();
			foreach (GridColumnEx column in gv_CFL.Columns)
			{
				column.Width = 100;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadDataEmployees()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataSource = await helper.GetAsync("GetAllEmployees");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
			gv_CFL.PopulateColumns();
			gv_CFL.OptionsView.ColumnAutoWidth = false;
			foreach (GridColumnEx column in gv_CFL.Columns)
			{
				column.Width = 100;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataProject()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataSource = await helper.GetAsync("GetAllProject");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
			gv_CFL.PopulateColumns();
			gv_CFL.OptionsView.ColumnAutoWidth = false;
			foreach (GridColumnEx column in gv_CFL.Columns)
			{
				column.Width = 100;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataForeCast()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataSource = await helper.GetAsync("GetForeCast");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataMRP()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataSource = await helper.GetAsync("GetMRP");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataSchedule()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CFL");
			GridControlEx gridControlEx = grd_CFL;
			gridControlEx.DataSource = await http.GetAsync("LoadDataLstSchedule", new { LstSchedule });
			grd_CFL.RefreshDataSource();
			grd_CFL.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataProductionOrder()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataSource = await helper.GetAsync("GetProductionOrder");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
			grd_CFL.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataCashFlow()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataSource = await helper.GetAsync("LoadDataCashFlow");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataListItems()
	{
		try
		{
			using HttpClientHelper<ItemsImpl> helper = new HttpClientHelper<ItemsImpl>(Configuration.ApiServer, "Commons");
			List<ItemsImpl> dataSource = (await helper.GetListAsync("ListChooseItemMaster", new
			{
				itemCode = "",
				BPLId = GlobalData.DefaultBranch,
				itObjType = _itemObjectType
			})).ToList();
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsListItem()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfItem;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		GridColumnEx gridColumnEx5 = new GridColumnEx();
		GridColumnEx gridColumnEx6 = new GridColumnEx();
		GridColumnEx gridColumnEx7 = new GridColumnEx();
		GridColumnEx gridColumnEx8 = new GridColumnEx();
		GridColumnEx gridColumnEx9 = new GridColumnEx();
		GridColumnEx gridColumnEx10 = new GridColumnEx();
		GridColumnEx gridColumnEx11 = new GridColumnEx();
		GridColumnEx gridColumnEx12 = new GridColumnEx();
		GridColumnEx gridColumnEx13 = new GridColumnEx();
		GridColumnEx gridColumnEx14 = new GridColumnEx();
		GridColumnEx gridColumnEx15 = new GridColumnEx();
		GridColumnEx gridColumnEx16 = new GridColumnEx();
		GridColumnEx gridColumnEx17 = new GridColumnEx();
		GridColumnEx gridColumnEx18 = new GridColumnEx();
		GridColumnEx gridColumnEx19 = new GridColumnEx();
		GridColumnEx gridColumnEx20 = new GridColumnEx();
		GridColumnEx gridColumnEx21 = new GridColumnEx();
		GridColumnEx gridColumnEx22 = new GridColumnEx();
		GridColumnEx gridColumnEx23 = new GridColumnEx();
		GridColumnEx gridColumnEx24 = new GridColumnEx();
		GridColumnEx gridColumnEx25 = new GridColumnEx();
		GridColumnEx gridColumnEx26 = new GridColumnEx();
		GridColumnEx gridColumnEx27 = new GridColumnEx();
		GridColumnEx gridColumnEx28 = new GridColumnEx();
		GridColumnEx gridColumnEx29 = new GridColumnEx();
		GridColumnEx gridColumnEx30 = new GridColumnEx();
		GridColumnEx gridColumnEx31 = new GridColumnEx();
		GridColumnEx gridColumnEx32 = new GridColumnEx();
		GridColumnEx gridColumnEx33 = new GridColumnEx();
		GridColumnEx gridColumnEx34 = new GridColumnEx();
		GridColumnEx gridColumnEx35 = new GridColumnEx();
		GridColumnEx gridColumnEx36 = new GridColumnEx();
		GridColumnEx gridColumnEx37 = new GridColumnEx();
		GridColumnEx gridColumnEx38 = new GridColumnEx();
		GridColumnEx gridColumnEx39 = new GridColumnEx();
		GridColumnEx gridColumnEx40 = new GridColumnEx();
		GridColumnEx gridColumnEx41 = new GridColumnEx();
		GridColumnEx gridColumnEx42 = new GridColumnEx();
		GridColumnEx gridColumnEx43 = new GridColumnEx();
		GridColumnEx gridColumnEx44 = new GridColumnEx();
		GridColumnEx gridColumnEx45 = new GridColumnEx();
		GridColumnEx gridColumnEx46 = new GridColumnEx();
		GridColumnEx gridColumnEx47 = new GridColumnEx();
		GridColumnEx gridColumnEx48 = new GridColumnEx();
		GridColumnEx gridColumnEx49 = new GridColumnEx();
		GridColumnEx gridColumnEx50 = new GridColumnEx();
		GridColumnEx gridColumnEx51 = new GridColumnEx();
		GridColumnEx gridColumnEx52 = new GridColumnEx();
		GridColumnEx gridColumnEx53 = new GridColumnEx();
		GridColumnEx gridColumnEx54 = new GridColumnEx();
		GridColumnEx gridColumnEx55 = new GridColumnEx();
		GridColumnEx gridColumnEx56 = new GridColumnEx();
		new GridColumnEx();
		new GridColumnEx();
		new GridColumnEx();
		new GridColumnEx();
		gridColumnEx.Name = "clItemCode";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.OITM_ItemCode;
		gridColumnEx.FieldName = "ItemCode";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clItemName";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.OITM_ItemName;
		gridColumnEx2.FieldName = "ItemName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "clDfltWH";
		gridColumnEx3.Caption = Apzon.Commons.Properties.Resources.OWHS_WhsCode;
		gridColumnEx3.FieldName = "DefaultWarehouse";
		gridColumnEx3.Width = 120;
		gridColumnEx3.Visible = true;
		gridColumnEx4.Name = "clOnHand";
		gridColumnEx4.Caption = Apzon.Commons.Properties.Resources.OITW_gcOnHand;
		gridColumnEx4.FieldName = "QuantityOnStock";
		gridColumnEx4.Width = 120;
		gridColumnEx4.DisplayFormat.FormatType = FormatType.Numeric;
		gridColumnEx4.DisplayFormat.FormatString = "n0";
		gridColumnEx4.Visible = true;
		gridColumnEx5.Name = "InvntryUom";
		gridColumnEx5.Caption = "Invntry Uom";
		gridColumnEx5.FieldName = "InvntryUom";
		gridColumnEx5.Width = 120;
		gridColumnEx5.Visible = false;
		gridColumnEx6.Name = "CodeBars";
		gridColumnEx6.Caption = "Bar Code";
		gridColumnEx6.FieldName = "CodeBars";
		gridColumnEx6.Width = 120;
		gridColumnEx6.Visible = false;
		gridColumnEx7.Name = "FrgnName";
		gridColumnEx7.Caption = "Frgn Name";
		gridColumnEx7.FieldName = "FrgnName";
		gridColumnEx7.Width = 120;
		gridColumnEx7.Visible = false;
		gridColumnEx8.Name = "ItemType";
		gridColumnEx8.Caption = "Item Type";
		gridColumnEx8.FieldName = "ItemTypes";
		gridColumnEx8.Width = 120;
		gridColumnEx8.Visible = false;
		gridColumnEx9.Name = "ItemGroup";
		gridColumnEx9.Caption = "ItemGroup";
		gridColumnEx9.FieldName = "ItmsGrpCod";
		gridColumnEx9.Width = 120;
		gridColumnEx9.Visible = false;
		gridColumnEx10.Name = "InvntItem";
		gridColumnEx10.Caption = "InvntItem";
		gridColumnEx10.FieldName = "InvntItem";
		gridColumnEx10.Width = 120;
		gridColumnEx10.Visible = false;
		gridColumnEx11.Name = "PrchseItem";
		gridColumnEx11.Caption = "PrchseItem";
		gridColumnEx11.FieldName = "PrchseItem";
		gridColumnEx11.Width = 120;
		gridColumnEx11.Visible = false;
		gridColumnEx12.Name = "SellItem";
		gridColumnEx12.Caption = "SellItem";
		gridColumnEx12.FieldName = "SellItem";
		gridColumnEx12.Width = 120;
		gridColumnEx12.Visible = false;
		gridColumnEx13.Name = "FirmCode";
		gridColumnEx13.Caption = "FirmCode";
		gridColumnEx13.FieldName = "FirmCode";
		gridColumnEx13.Width = 120;
		gridColumnEx13.Visible = false;
		gridColumnEx14.Name = "ShipType";
		gridColumnEx14.Caption = "ShipType";
		gridColumnEx14.FieldName = "ShipType";
		gridColumnEx14.Width = 120;
		gridColumnEx14.Visible = false;
		gridColumnEx15.Name = "ManSerNum";
		gridColumnEx15.Caption = "ManSerNum";
		gridColumnEx15.FieldName = "ManSerNum";
		gridColumnEx15.Width = 120;
		gridColumnEx15.Visible = false;
		gridColumnEx16.Name = "validFor";
		gridColumnEx16.Caption = "validFor";
		gridColumnEx16.FieldName = "validFor";
		gridColumnEx16.Width = 120;
		gridColumnEx16.Visible = false;
		gridColumnEx17.Name = "CardCode";
		gridColumnEx17.Caption = "CardCode";
		gridColumnEx17.FieldName = "CardCode";
		gridColumnEx17.Width = 120;
		gridColumnEx17.Visible = false;
		gridColumnEx18.Name = "SuppCatNum";
		gridColumnEx18.Caption = "SuppCatNum";
		gridColumnEx18.FieldName = "SuppCatNum";
		gridColumnEx18.Width = 120;
		gridColumnEx18.Visible = false;
		gridColumnEx19.Name = "BuyUnitMsr";
		gridColumnEx19.Caption = "BuyUnitMsr";
		gridColumnEx19.FieldName = "BuyUnitMsr";
		gridColumnEx19.Width = 120;
		gridColumnEx19.Visible = false;
		gridColumnEx20.Name = "NumInBuy";
		gridColumnEx20.Caption = "NumInBuy";
		gridColumnEx20.FieldName = "NumInBuy";
		gridColumnEx20.Width = 120;
		gridColumnEx20.Visible = false;
		gridColumnEx21.Name = "PurPackMsr";
		gridColumnEx21.Caption = "PurPackMsr";
		gridColumnEx21.FieldName = "PurPackMsr";
		gridColumnEx21.Width = 120;
		gridColumnEx21.Visible = false;
		gridColumnEx22.Name = "PurPackUn";
		gridColumnEx22.Caption = "PurPackUn";
		gridColumnEx22.FieldName = "PurPackUn";
		gridColumnEx22.Width = 120;
		gridColumnEx22.Visible = false;
		gridColumnEx23.Name = "PurFactor1";
		gridColumnEx23.Caption = "PurFactor1";
		gridColumnEx23.FieldName = "PurFactor1";
		gridColumnEx23.Width = 120;
		gridColumnEx23.Visible = false;
		gridColumnEx24.Name = "PurFactor2";
		gridColumnEx24.Caption = "PurFactor2";
		gridColumnEx24.FieldName = "PurFactor2";
		gridColumnEx24.Width = 120;
		gridColumnEx24.Visible = false;
		gridColumnEx25.Name = "PurFactor3";
		gridColumnEx25.Caption = "PurFactor3";
		gridColumnEx25.FieldName = "PurFactor3";
		gridColumnEx25.Width = 120;
		gridColumnEx25.Visible = false;
		gridColumnEx26.Name = "PurFactor4";
		gridColumnEx26.Caption = "PurFactor4";
		gridColumnEx26.FieldName = "PurFactor4";
		gridColumnEx26.Width = 120;
		gridColumnEx26.Visible = false;
		gridColumnEx27.Name = "SalUnitMsr";
		gridColumnEx27.Caption = "SalUnitMsr";
		gridColumnEx27.FieldName = "SalUnitMsr";
		gridColumnEx27.Width = 120;
		gridColumnEx27.Visible = false;
		gridColumnEx28.Name = "NumInSale";
		gridColumnEx28.Caption = "NumInSale";
		gridColumnEx28.FieldName = "NumInSale";
		gridColumnEx28.Width = 120;
		gridColumnEx28.Visible = false;
		gridColumnEx29.Name = "SalPackMsr";
		gridColumnEx29.Caption = "SalPackMsr";
		gridColumnEx29.FieldName = "SalPackMsr";
		gridColumnEx29.Width = 120;
		gridColumnEx29.Visible = false;
		gridColumnEx30.Name = "SalPackUn";
		gridColumnEx30.Caption = "SalPackUn";
		gridColumnEx30.FieldName = "SalPackUn";
		gridColumnEx30.Width = 120;
		gridColumnEx30.Visible = false;
		gridColumnEx31.Name = "SalFactor1";
		gridColumnEx31.Caption = "SalFactor1";
		gridColumnEx31.FieldName = "SalFactor1";
		gridColumnEx31.Width = 120;
		gridColumnEx31.Visible = false;
		gridColumnEx32.Name = "SalFactor2";
		gridColumnEx32.Caption = "SalFactor2";
		gridColumnEx32.FieldName = "SalFactor2";
		gridColumnEx32.Width = 120;
		gridColumnEx32.Visible = false;
		gridColumnEx33.Name = "SalFactor3";
		gridColumnEx33.Caption = "SalFactor3";
		gridColumnEx33.FieldName = "SalFactor3";
		gridColumnEx33.Width = 120;
		gridColumnEx33.Visible = false;
		gridColumnEx34.Name = "SalFactor4";
		gridColumnEx34.Caption = "SalFactor4";
		gridColumnEx34.FieldName = "SalFactor4";
		gridColumnEx34.Width = 120;
		gridColumnEx34.Visible = false;
		gridColumnEx35.Name = "GLMethods";
		gridColumnEx35.Caption = "GLMethods";
		gridColumnEx35.FieldName = "GLMethods";
		gridColumnEx35.Width = 120;
		gridColumnEx35.Visible = false;
		gridColumnEx36.Name = "EvalSystem";
		gridColumnEx36.Caption = "EvalSystem";
		gridColumnEx36.FieldName = "EvalSystem";
		gridColumnEx36.Width = 120;
		gridColumnEx36.Visible = false;
		gridColumnEx37.Name = "AvgPrice";
		gridColumnEx37.Caption = "AvgPrice";
		gridColumnEx37.FieldName = "AvgPrice";
		gridColumnEx37.Width = 120;
		gridColumnEx37.Visible = false;
		gridColumnEx38.Name = "ByWh";
		gridColumnEx38.Caption = "ByWh";
		gridColumnEx38.FieldName = "ByWh";
		gridColumnEx38.Width = 120;
		gridColumnEx38.Visible = false;
		gridColumnEx39.Name = "ReorderQty";
		gridColumnEx39.Caption = "ReorderQty";
		gridColumnEx39.FieldName = "ReorderQty";
		gridColumnEx39.Width = 120;
		gridColumnEx39.Visible = false;
		gridColumnEx40.Name = "MinLevel";
		gridColumnEx40.Caption = "MinLevel";
		gridColumnEx40.FieldName = "MinLevel";
		gridColumnEx40.Width = 120;
		gridColumnEx40.Visible = false;
		gridColumnEx40.Name = "MaxLevel";
		gridColumnEx40.Caption = "MaxLevel";
		gridColumnEx40.FieldName = "MaxLevel";
		gridColumnEx40.Width = 120;
		gridColumnEx40.Visible = false;
		gridColumnEx41.Name = "VatGroupPu";
		gridColumnEx41.Caption = "VatGroupPu";
		gridColumnEx41.FieldName = "PurchaseVATGroup";
		gridColumnEx41.Width = 120;
		gridColumnEx41.Visible = false;
		gridColumnEx42.Name = "VatGourpSa";
		gridColumnEx42.Caption = "VatGourpSa";
		gridColumnEx42.FieldName = "SalesVATGroup";
		gridColumnEx42.Width = 120;
		gridColumnEx42.Visible = false;
		gridColumnEx43.Name = "BOMTreeType";
		gridColumnEx43.Caption = "BOMTreeType";
		gridColumnEx43.FieldName = "BOMTreeType";
		gridColumnEx43.Width = 120;
		gridColumnEx43.Visible = false;
		gridColumnEx44.Name = "OITMVatGroupPu";
		gridColumnEx44.Caption = "VatGroupPu";
		gridColumnEx44.FieldName = "VatGroupPu";
		gridColumnEx44.Width = 120;
		gridColumnEx44.Visible = false;
		gridColumnEx45.Name = "OITMVatGourpSa";
		gridColumnEx45.Caption = "VatGourpSa";
		gridColumnEx45.FieldName = "VatGourpSa";
		gridColumnEx45.Width = 120;
		gridColumnEx45.Visible = false;
		gridColumnEx46.Name = "ColVatPuInActive";
		gridColumnEx46.Caption = "VatPuInActive";
		gridColumnEx46.FieldName = "VatPuInActive";
		gridColumnEx46.Width = 120;
		gridColumnEx46.Visible = false;
		gridColumnEx47.Name = "ColVatSaInActive";
		gridColumnEx47.Caption = "VatSaInActive";
		gridColumnEx47.FieldName = "VatSaInActive";
		gridColumnEx47.Width = 120;
		gridColumnEx47.Visible = false;
		gridColumnEx48.Name = "ColValidTo";
		gridColumnEx48.Caption = "ValidTo";
		gridColumnEx48.FieldName = "ValidTo";
		gridColumnEx48.Width = 120;
		gridColumnEx48.Visible = false;
		gridColumnEx49.Name = "ColValidFrom";
		gridColumnEx49.Caption = "ValidFrom";
		gridColumnEx49.FieldName = "ValidFrom";
		gridColumnEx49.Width = 120;
		gridColumnEx49.Visible = false;
		gridColumnEx50.Name = "ColValidRemarks";
		gridColumnEx50.Caption = "ValidRemarks";
		gridColumnEx50.FieldName = "ValidRemarks";
		gridColumnEx50.Width = 120;
		gridColumnEx50.Visible = false;
		gridColumnEx51.Name = "ColFrozenTo";
		gridColumnEx51.Caption = "FrozenTo";
		gridColumnEx51.FieldName = "FrozenTo";
		gridColumnEx51.Width = 120;
		gridColumnEx51.Visible = false;
		gridColumnEx52.Name = "ColFrozenFrom";
		gridColumnEx52.Caption = "FrozenFrom";
		gridColumnEx52.FieldName = "FrozenFrom";
		gridColumnEx52.Width = 120;
		gridColumnEx52.Visible = false;
		gridColumnEx53.Name = "ColFrozenRemarks";
		gridColumnEx53.Caption = "FrozenRemarks";
		gridColumnEx53.FieldName = "FrozenRemarks";
		gridColumnEx53.Width = 120;
		gridColumnEx53.Visible = false;
		gridColumnEx54.Name = "ColMngMethod";
		gridColumnEx54.Caption = "MngMethod";
		gridColumnEx54.FieldName = "MngMethod";
		gridColumnEx54.Width = 120;
		gridColumnEx54.Visible = false;
		gridColumnEx55.Name = "ColManBtchNum";
		gridColumnEx55.Caption = "ManBtchNum";
		gridColumnEx55.FieldName = "ManBtchNum";
		gridColumnEx55.Width = 120;
		gridColumnEx55.Visible = false;
		gridColumnEx56.Name = "ColfrozenFor";
		gridColumnEx56.Caption = "frozenFor";
		gridColumnEx56.FieldName = "frozenFor";
		gridColumnEx56.Width = 120;
		gridColumnEx56.Visible = false;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[56]
		{
			gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5, gridColumnEx6, gridColumnEx7, gridColumnEx8, gridColumnEx9, gridColumnEx10,
			gridColumnEx11, gridColumnEx12, gridColumnEx13, gridColumnEx14, gridColumnEx15, gridColumnEx16, gridColumnEx17, gridColumnEx18, gridColumnEx19, gridColumnEx20,
			gridColumnEx21, gridColumnEx22, gridColumnEx23, gridColumnEx24, gridColumnEx25, gridColumnEx26, gridColumnEx27, gridColumnEx28, gridColumnEx29, gridColumnEx30,
			gridColumnEx31, gridColumnEx32, gridColumnEx33, gridColumnEx34, gridColumnEx35, gridColumnEx36, gridColumnEx37, gridColumnEx38, gridColumnEx39, gridColumnEx40,
			gridColumnEx41, gridColumnEx42, gridColumnEx43, gridColumnEx44, gridColumnEx45, gridColumnEx46, gridColumnEx47, gridColumnEx48, gridColumnEx49, gridColumnEx50,
			gridColumnEx51, gridColumnEx52, gridColumnEx53, gridColumnEx54, gridColumnEx55, gridColumnEx56
		};
		columns.AddRange(columns2);
	}

	private async Task LoadDataBpCatalogByCardCode()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BusinessPartnerCatalogNumbers");
			DataTable dataSource = await helper.GetAsync("GetBPByCardCode", new
			{
				cardCode = _vatType
			});
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
			gv_CFL.HideLoadingPanel();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsBPCatalogByCardCode()
	{
		try
		{
			Text = Apzon.Commons.Properties.Resources.Titel_ListOfBPCatalogNo;
			GridColumnEx gridColumnEx = new GridColumnEx();
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			GridColumnEx gridColumnEx3 = new GridColumnEx();
			GridColumnEx gridColumnEx4 = new GridColumnEx();
			GridColumnEx gridColumnEx5 = new GridColumnEx();
			GridColumnEx gridColumnEx6 = new GridColumnEx();
			GridColumnEx gridColumnEx7 = new GridColumnEx();
			gridColumnEx.Name = "Substitute";
			gridColumnEx.Caption = Apzon.Commons.Properties.Resources.lb_BPCatalogNo;
			gridColumnEx.FieldName = "Substitute";
			gridColumnEx.Width = 120;
			gridColumnEx.Visible = true;
			gridColumnEx2.Name = "ItemCode";
			gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.Line_gvLine_gcItemCode;
			gridColumnEx2.FieldName = "ItemCode";
			gridColumnEx2.Width = 120;
			gridColumnEx2.Visible = true;
			gridColumnEx3.Name = "Transfered";
			gridColumnEx3.Caption = Apzon.Commons.Properties.Resources.lb_BPCatalogNo;
			gridColumnEx3.FieldName = "Transfered";
			gridColumnEx3.Width = 120;
			gridColumnEx3.Visible = false;
			gridColumnEx4.Name = "DataSource";
			gridColumnEx4.Caption = Apzon.Commons.Properties.Resources.Line_gvLine_gcItemCode;
			gridColumnEx4.FieldName = "DataSource";
			gridColumnEx4.Width = 120;
			gridColumnEx4.Visible = false;
			gridColumnEx5.Name = "UserSign";
			gridColumnEx5.Caption = Apzon.Commons.Properties.Resources.lb_BPCatalogNo;
			gridColumnEx5.FieldName = "UserSign";
			gridColumnEx5.Width = 120;
			gridColumnEx5.Visible = false;
			gridColumnEx6.Name = "ShowSCN";
			gridColumnEx6.Caption = Apzon.Commons.Properties.Resources.Line_gvLine_gcItemCode;
			gridColumnEx6.FieldName = "ShowSCN";
			gridColumnEx6.Width = 120;
			gridColumnEx6.Visible = false;
			gridColumnEx7.Name = "CardCode";
			gridColumnEx7.Caption = Apzon.Commons.Properties.Resources.lb_BPCatalogNo;
			gridColumnEx7.FieldName = "CardCode";
			gridColumnEx7.Width = 120;
			gridColumnEx7.Visible = false;
			gv_CFL.Columns.Clear();
			GridColumnCollection columns = gv_CFL.Columns;
			GridColumn[] columns2 = new GridColumnEx[7] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5, gridColumnEx6, gridColumnEx7 };
			columns.AddRange(columns2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsRouting()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfRouting;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "RouCode";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.lb_RouCode;
		gridColumnEx.FieldName = "RouCode";
		gridColumnEx.Width = 100;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "RouName";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.lb_RouName;
		gridColumnEx2.FieldName = "RouName";
		gridColumnEx2.Width = 150;
		gridColumnEx2.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataRouting()
	{
		try
		{
			using HttpClientHelper<GetListRoutingImpl> helper = new HttpClientHelper<GetListRoutingImpl>(Configuration.ApiServer, "Routing");
			List<GetListRoutingImpl> dataSource = await helper.GetListAsync("GetListRouting");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadDataFormatedSearch()
	{
		try
		{
			DataSet dataSet = _httpClient.Commons.ExecuteSql(FormattedSearch);
			if (dataSet.Tables.Count > 1)
			{
				grd_CFL.DataSource = dataSet.Tables[1];
			}
			else
			{
				grd_CFL.DataSource = new DataTable();
			}
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadDataColumnsBatch(string itemcode)
	{
		try
		{
			using HttpClientHelper<DataTable> client = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionPlanning");
			DataTable dataSource = await client.PostDataTableAsync("GetBatchByItemCode", itemcode);
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadDataColumnsWareHouseByItemCode(string itemcode)
	{
		try
		{
			using HttpClientHelper<DataTable> client = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionPlanning");
			DataTable dataSource = await client.PostDataTableAsync("ChooseWareHouseByItemCode", itemcode);
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadDataCountry()
	{
		try
		{
			using HttpClientHelper<DataTable> client = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Administrator");
			DataTable dataSource = await client.GetAsync("GetListCountry");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadDataState()
	{
		try
		{
			using HttpClientHelper<DataTable> client = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Administrator");
			DataTable dataSource = await client.GetAsync("GetListState", new
			{
				country = Coutry
			});
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadDataOrcCode(int p)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Sales");
			DataTable dataSource = await helper.GetDataTableAsync("GetListOrcCode", new
			{
				p = p.ToString(),
				date = PostingDate
			});
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsOrcCode(int p)
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfDimensionCostCenter;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		gridColumnEx.Name = "cl_OrcCode";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.lb_Code;
		gridColumnEx.FieldName = "OcrCode";
		gridColumnEx.Width = 100;
		gridColumnEx.Visible = true;
		if (p == 1)
		{
			gridColumnEx.Tag = "OcrCode";
		}
		else
		{
			gridColumnEx.Tag = "OcrCode" + Function.ToString(p);
		}
		gridColumnEx2.Name = "cl_OrcName";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.lb_Name;
		gridColumnEx2.FieldName = "OcrName";
		gridColumnEx2.Width = 150;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "cl_OrcTotal";
		gridColumnEx3.Caption = Apzon.Commons.Properties.Resources.lb_Total;
		gridColumnEx3.FieldName = "OcrTotal";
		gridColumnEx3.Width = 150;
		gridColumnEx3.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[3] { gridColumnEx, gridColumnEx2, gridColumnEx3 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsGroupControlKey()
	{
		Text = "List of Group Control Key";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "Code";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.lb_Code;
		gridColumnEx.FieldName = "Code";
		gridColumnEx.Width = 100;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "Name";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.lb_Name;
		gridColumnEx2.FieldName = "Name";
		gridColumnEx2.Width = 150;
		gridColumnEx2.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsOperation()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfOperation;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "Code";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.lb_Code;
		gridColumnEx.FieldName = "Code";
		gridColumnEx.Width = 100;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "Name";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.lb_Name;
		gridColumnEx2.FieldName = "Name";
		gridColumnEx2.Width = 150;
		gridColumnEx2.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsOperationByStatus()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfOperation;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "Code";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.lb_Code;
		gridColumnEx.FieldName = "Code";
		gridColumnEx.Width = 100;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "Name";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.lb_Name;
		gridColumnEx2.FieldName = "Name";
		gridColumnEx2.Width = 150;
		gridColumnEx2.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataStdProOrder()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionOder");
			DataTable dataSource = await helper.GetAsync("GetListStandardOrders");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
			gv_CFL.PopulateColumns();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsStdProOrder()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListofStandardProductionOrders;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		GridColumnEx gridColumnEx5 = new GridColumnEx();
		GridColumnEx gridColumnEx6 = new GridColumnEx();
		GridColumnEx gridColumnEx7 = new GridColumnEx();
		gridColumnEx.Name = "cl_DocEntry";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.lb_DocumentNo;
		gridColumnEx.FieldName = "DocEntry";
		gridColumnEx.Width = 100;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "cl_Series";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.lb_SeriesName;
		gridColumnEx2.FieldName = "Series";
		gridColumnEx2.Width = 150;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "cl_Type";
		gridColumnEx3.Caption = Apzon.Commons.Properties.Resources.lb_ProductionOrdersType;
		gridColumnEx3.FieldName = "Type";
		gridColumnEx3.Width = 150;
		gridColumnEx3.Visible = true;
		gridColumnEx4.Name = "cl_DueDate";
		gridColumnEx4.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_gcDocDueDate;
		gridColumnEx4.FieldName = "DueDate";
		gridColumnEx4.Width = 100;
		gridColumnEx4.Visible = true;
		gridColumnEx5.Name = "cl_ItemCode";
		gridColumnEx5.Caption = Apzon.Commons.Properties.Resources.lb_ProductNo;
		gridColumnEx5.FieldName = "ItemCode";
		gridColumnEx5.Width = 150;
		gridColumnEx5.Visible = true;
		gridColumnEx6.Name = "cl_ItemName";
		gridColumnEx6.Caption = Apzon.Commons.Properties.Resources.Line_gvLine_gcItemName;
		gridColumnEx6.FieldName = "ItemName";
		gridColumnEx6.Width = 150;
		gridColumnEx6.Visible = true;
		gridColumnEx7.Name = "cl_Comments";
		gridColumnEx7.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_Remark;
		gridColumnEx7.FieldName = "Comments";
		gridColumnEx7.Width = 150;
		gridColumnEx7.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[7] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5, gridColumnEx6, gridColumnEx7 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataItemProduction()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionOder");
			DataTable dataSource = await helper.GetAsync("GetDataItemProduction", new { _TranType });
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsControlKey()
	{
		Text = "List of Control Keys";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "KeyCode";
		gridColumnEx.Caption = "KeyCode";
		gridColumnEx.FieldName = "KeyCode";
		gridColumnEx.Width = 100;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "KeyName";
		gridColumnEx2.Caption = "Key Name";
		gridColumnEx2.FieldName = "KeyName";
		gridColumnEx2.Width = 150;
		gridColumnEx2.Visible = true;
	}

	private void LoadColumnsResource()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfResource;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "ResCode";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.lb_ResCode;
		gridColumnEx.FieldName = "ResCode";
		gridColumnEx.Width = 100;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "ResName";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.lb_ResName;
		gridColumnEx2.FieldName = "ResName";
		gridColumnEx2.Width = 150;
		gridColumnEx2.Visible = true;
	}

	private async Task LoadDataControlKey()
	{
		try
		{
			using HttpClientHelper<ControlKeyImpl> helper = new HttpClientHelper<ControlKeyImpl>(Configuration.ApiServer, "ControlKey");
			List<ControlKeyImpl> dataSource = await helper.GetListAsync("GetListKeyCode");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataResource()
	{
		try
		{
			using HttpClientHelper<GetResCodeImpl> helper = new HttpClientHelper<GetResCodeImpl>(Configuration.ApiServer, "Resources");
			List<GetResCodeImpl> dataSource = await helper.GetListAsync("GetListResCode");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataItemManualProduction()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionOder");
			DataTable dataSource = await helper.GetAsync("GetDataItemManualProduction", new { strSearch });
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsItemProduction()
	{
		Text = "item Production:";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		GridColumnEx gridColumnEx5 = new GridColumnEx();
		GridColumnEx gridColumnEx6 = new GridColumnEx();
		GridColumnEx gridColumnEx7 = new GridColumnEx();
		GridColumnEx gridColumnEx8 = new GridColumnEx();
		GridColumnEx gridColumnEx9 = new GridColumnEx();
		GridColumnEx gridColumnEx10 = new GridColumnEx();
		GridColumnEx gridColumnEx11 = new GridColumnEx();
		gridColumnEx.Name = "cl_DocEntry";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.lb_DocumentNo;
		gridColumnEx.FieldName = "DocEntry";
		gridColumnEx.Width = 100;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "cl_Series";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.lb_SeriesName;
		gridColumnEx2.FieldName = "Series";
		gridColumnEx2.Width = 150;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "cl_Type";
		gridColumnEx3.Caption = Apzon.Commons.Properties.Resources.lb_ProductionOrdersType;
		gridColumnEx3.FieldName = "Type";
		gridColumnEx3.Width = 150;
		gridColumnEx3.Visible = true;
		gridColumnEx4.Name = "cl_DueDate";
		gridColumnEx4.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_gcDocDueDate;
		gridColumnEx4.FieldName = "DueDate";
		gridColumnEx4.Width = 100;
		gridColumnEx4.Visible = true;
		gridColumnEx5.Name = "cl_ItemCode";
		gridColumnEx5.Caption = Apzon.Commons.Properties.Resources.lb_ProductNo;
		gridColumnEx5.FieldName = "ItemCode";
		gridColumnEx5.Width = 150;
		gridColumnEx5.Visible = true;
		gridColumnEx6.Name = "cl_ItemName";
		gridColumnEx6.Caption = Apzon.Commons.Properties.Resources.Line_gvLine_gcItemName;
		gridColumnEx6.FieldName = "ItemName";
		gridColumnEx6.Width = 150;
		gridColumnEx6.Visible = true;
		gridColumnEx7.Name = "cl_Comments";
		gridColumnEx7.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_Remark;
		gridColumnEx7.FieldName = "Comments";
		gridColumnEx7.Width = 150;
		gridColumnEx7.Visible = true;
		gridColumnEx8.Name = "cl_DefaultWarehouse";
		gridColumnEx8.Caption = Apzon.Commons.Properties.Resources.lb_WarehouseCode;
		gridColumnEx8.FieldName = "DefaultWarehouse";
		gridColumnEx8.Width = 150;
		gridColumnEx8.Visible = true;
		gridColumnEx9.Name = "cl_UoMCode";
		gridColumnEx9.Caption = Apzon.Commons.Properties.Resources.lb_UomCode;
		gridColumnEx9.FieldName = "UoMCode";
		gridColumnEx9.Width = 150;
		gridColumnEx9.Visible = true;
		gridColumnEx10.Name = "cl_QuantityOnStock";
		gridColumnEx10.Caption = "QuantityOnStock";
		gridColumnEx10.FieldName = "QuantityOnStock";
		gridColumnEx10.Width = 150;
		gridColumnEx10.Visible = true;
		gridColumnEx11.Name = "cl_QuantityOnWareHouse";
		gridColumnEx11.Caption = "QuantityOnWareHouse";
		gridColumnEx11.FieldName = "QuantityOnWareHouse";
		gridColumnEx11.Width = 150;
		gridColumnEx11.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[11]
		{
			gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5, gridColumnEx6, gridColumnEx7, gridColumnEx8, gridColumnEx9, gridColumnEx10,
			gridColumnEx11
		};
		columns.AddRange(columns2);
	}

	private async Task LoadDataBoMJobResource()
	{
		try
		{
			using HttpClientHelper<GetListRoutingImpl> helper = new HttpClientHelper<GetListRoutingImpl>(Configuration.ApiServer, "BoMJob");
			List<GetListRoutingImpl> dataSource = await helper.GetListAsync("GetResourceCFL");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataOperation()
	{
		try
		{
			using HttpClientHelper<OperationImpl> helper = new HttpClientHelper<OperationImpl>(Configuration.ApiServer, "Operation");
			List<OperationImpl> dataSource = await helper.GetListAsync("LoadListOperation");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataOperationByStatus()
	{
		try
		{
			using HttpClientHelper<OperationImpl> helper = new HttpClientHelper<OperationImpl>(Configuration.ApiServer, "Operation");
			List<OperationImpl> dataSource = await helper.GetListAsync("LoadListOperationByStatus");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsBoMJobResource()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfBoMResource;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		gridColumnEx.Name = "cl_ResCode";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.lb_ResourceNo;
		gridColumnEx.FieldName = "ItemCode";
		gridColumnEx.Width = 100;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "cl_ResName";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.lb_ResourceName;
		gridColumnEx2.FieldName = "ItemName";
		gridColumnEx2.Width = 150;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "cl_warehouse";
		gridColumnEx3.Caption = Apzon.Commons.Properties.Resources.OWHS_WhsCode;
		gridColumnEx3.FieldName = "WhseCode";
		gridColumnEx3.Width = 150;
		gridColumnEx3.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[3] { gridColumnEx, gridColumnEx2, gridColumnEx3 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsListPurchaseRequest()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfPurchaseRequest;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		GridColumnEx gridColumnEx5 = new GridColumnEx();
		GridColumnEx gridColumnEx6 = new GridColumnEx();
		gridColumnEx.Name = "cl_DocEntry";
		gridColumnEx.Caption = "PRQ No.";
		gridColumnEx.FieldName = "DocNum";
		gridColumnEx.Width = 100;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "cl_DocDate";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_gcPostingDate;
		gridColumnEx2.FieldName = "DocDate";
		gridColumnEx2.Width = 150;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "cl_ReqType";
		gridColumnEx3.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_gcReqTypeName;
		gridColumnEx3.FieldName = "ReqTypeName";
		gridColumnEx3.Width = 200;
		gridColumnEx3.Visible = true;
		gridColumnEx4.Name = "cl_ReqName";
		gridColumnEx4.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_ReqName;
		gridColumnEx4.FieldName = "ReqName";
		gridColumnEx4.Width = 150;
		gridColumnEx4.Visible = true;
		gridColumnEx5.Name = "cl_Comments";
		gridColumnEx5.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_Remark;
		gridColumnEx5.FieldName = "Comments";
		gridColumnEx5.Width = 200;
		gridColumnEx5.Visible = true;
		gridColumnEx6.Name = "cl_DocDueDate";
		gridColumnEx6.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_gcValidUntil;
		gridColumnEx6.FieldName = "DocDueDate";
		gridColumnEx6.Width = 200;
		gridColumnEx6.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[6] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5, gridColumnEx6 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataListPurchaseRequest()
	{
		try
		{
			using HttpClientHelper<DocumentsImpl> helper = new HttpClientHelper<DocumentsImpl>(Configuration.ApiServer, "PurchaseRequest");
			List<DocumentsImpl> dataSource = await helper.GetListAsync("GetListPurchaseRequest");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataBpCatalog()
	{
		try
		{
			using HttpClientHelper<ItemsImpl> helper = new HttpClientHelper<ItemsImpl>(Configuration.ApiServer, "Sales");
			DataTable dataSource = await helper.GetDataTableAsync("GetListBPCatalog");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsContactPerson()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfContactPerson;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		gridColumnEx.Name = "clName";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.OCPR_Name;
		gridColumnEx.FieldName = "Name";
		gridColumnEx.Width = 150;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clPosition";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.OCPR_Position;
		gridColumnEx2.FieldName = "Position";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "clTel1";
		gridColumnEx3.Caption = Apzon.Commons.Properties.Resources.OCPR_Tel1;
		gridColumnEx3.FieldName = "Tel1";
		gridColumnEx3.Width = 120;
		gridColumnEx3.Visible = true;
		gridColumnEx4.Name = "clCntctCode";
		gridColumnEx4.Caption = Apzon.Commons.Properties.Resources.OCPR_CntctCode;
		gridColumnEx4.FieldName = "CntctCode";
		gridColumnEx4.Width = 120;
		gridColumnEx4.Visible = false;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[4] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataListContactPerson()
	{
		await GetListContactPerson(cardCode);
	}

	private async Task GetListContactPerson(string cardCode)
	{
		cardCode = GlobalData.CardCodeDK;
		try
		{
			using HttpClientHelper<BusinessPartnersImpl> helper = new HttpClientHelper<BusinessPartnersImpl>(Configuration.ApiServer, "BusinessPartner");
			List<BusinessPartnersImpl> dataSource = await helper.GetListAsync("GetListContactPerson", new { cardCode });
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsRequesterByEmployee()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfEmployee;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		GridColumnEx gridColumnEx5 = new GridColumnEx();
		gridColumnEx.Name = "clLastName";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.OHEM_LastName;
		gridColumnEx.FieldName = "LastName";
		gridColumnEx.Width = 100;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clFirstName";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.OHEM_FirstName;
		gridColumnEx2.FieldName = "FirstName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gridColumnEx4.Name = "clReqName";
		gridColumnEx4.Caption = Apzon.Commons.Properties.Resources.OPRQ_ReqName;
		gridColumnEx4.FieldName = "ReqName";
		gridColumnEx4.Width = 120;
		gridColumnEx4.Visible = false;
		gridColumnEx4.Name = "clEmpID";
		gridColumnEx4.Caption = Apzon.Commons.Properties.Resources.OHEM_EmpID;
		gridColumnEx4.FieldName = "EmpID";
		gridColumnEx4.Width = 120;
		gridColumnEx4.Visible = true;
		gridColumnEx5.Name = "UserSign";
		gridColumnEx5.Caption = Apzon.Commons.Properties.Resources.OUSR_UserSign;
		gridColumnEx5.FieldName = "UserSign";
		gridColumnEx5.Width = 120;
		gridColumnEx5.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[5] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsRequesterByUser()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfUser;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		GridColumnEx gridColumnEx5 = new GridColumnEx();
		GridColumnEx gridColumnEx6 = new GridColumnEx();
		GridColumnEx gridColumnEx7 = new GridColumnEx();
		gridColumnEx.Name = "clRequester";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.OPRQ_Requester;
		gridColumnEx.FieldName = "Requester";
		gridColumnEx.Width = 100;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clReqName";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.OPRQ_ReqName;
		gridColumnEx2.FieldName = "ReqName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "clDepartment";
		gridColumnEx3.Caption = Apzon.Commons.Properties.Resources.OPRQ_Department;
		gridColumnEx3.FieldName = "Department";
		gridColumnEx3.Width = 120;
		gridColumnEx3.Visible = false;
		gridColumnEx4.Name = "clDepartmentName";
		gridColumnEx4.Caption = Apzon.Commons.Properties.Resources.OPRQ_DepartmentName;
		gridColumnEx4.FieldName = "DepartmentName";
		gridColumnEx4.Width = 120;
		gridColumnEx4.Visible = true;
		gridColumnEx5.Name = "clBranch";
		gridColumnEx5.Caption = Apzon.Commons.Properties.Resources.OUBR_Branch;
		gridColumnEx5.FieldName = "Branch";
		gridColumnEx5.Width = 120;
		gridColumnEx5.Visible = false;
		gridColumnEx6.Name = "BranchName";
		gridColumnEx6.Caption = Apzon.Commons.Properties.Resources.OUBR_BranchName;
		gridColumnEx6.FieldName = "BranchName";
		gridColumnEx6.Width = 120;
		gridColumnEx6.Visible = true;
		gridColumnEx7.Name = "UserSign";
		gridColumnEx7.Caption = Apzon.Commons.Properties.Resources.OUSR_UserSign;
		gridColumnEx7.FieldName = "UserSign";
		gridColumnEx7.Width = 120;
		gridColumnEx7.Visible = false;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[7] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5, gridColumnEx6, gridColumnEx7 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataRequesterByEmployee()
	{
		try
		{
			using HttpClientHelper<RequesterImpl> helper = new HttpClientHelper<RequesterImpl>(Configuration.ApiServer, "Requester");
			DataTable dataSource = await helper.GetDataTableAsync("GetRequesterByEmployee");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataRequesterByUser()
	{
		try
		{
			using HttpClientHelper<RequesterImpl> helper = new HttpClientHelper<RequesterImpl>(Configuration.ApiServer, "Requester");
			DataTable dataSource = await helper.GetDataTableAsync("GetRequesterByUser");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsItemInventory()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfItem;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		GridColumnEx gridColumnEx5 = new GridColumnEx();
		GridColumnEx gridColumnEx6 = new GridColumnEx();
		GridColumnEx gridColumnEx7 = new GridColumnEx();
		GridColumnEx gridColumnEx8 = new GridColumnEx();
		GridColumnEx gridColumnEx9 = new GridColumnEx();
		GridColumnEx gridColumnEx10 = new GridColumnEx();
		GridColumnEx gridColumnEx11 = new GridColumnEx();
		GridColumnEx gridColumnEx12 = new GridColumnEx();
		GridColumnEx gridColumnEx13 = new GridColumnEx();
		GridColumnEx gridColumnEx14 = new GridColumnEx();
		GridColumnEx gridColumnEx15 = new GridColumnEx();
		GridColumnEx gridColumnEx16 = new GridColumnEx();
		GridColumnEx gridColumnEx17 = new GridColumnEx();
		GridColumnEx gridColumnEx18 = new GridColumnEx();
		GridColumnEx gridColumnEx19 = new GridColumnEx();
		GridColumnEx gridColumnEx20 = new GridColumnEx();
		GridColumnEx gridColumnEx21 = new GridColumnEx();
		GridColumnEx gridColumnEx22 = new GridColumnEx();
		GridColumnEx gridColumnEx23 = new GridColumnEx();
		GridColumnEx gridColumnEx24 = new GridColumnEx();
		GridColumnEx gridColumnEx25 = new GridColumnEx();
		GridColumnEx gridColumnEx26 = new GridColumnEx();
		GridColumnEx gridColumnEx27 = new GridColumnEx();
		GridColumnEx gridColumnEx28 = new GridColumnEx();
		GridColumnEx gridColumnEx29 = new GridColumnEx();
		GridColumnEx gridColumnEx30 = new GridColumnEx();
		GridColumnEx gridColumnEx31 = new GridColumnEx();
		GridColumnEx gridColumnEx32 = new GridColumnEx();
		GridColumnEx gridColumnEx33 = new GridColumnEx();
		GridColumnEx gridColumnEx34 = new GridColumnEx();
		GridColumnEx gridColumnEx35 = new GridColumnEx();
		GridColumnEx gridColumnEx36 = new GridColumnEx();
		GridColumnEx gridColumnEx37 = new GridColumnEx();
		GridColumnEx gridColumnEx38 = new GridColumnEx();
		GridColumnEx gridColumnEx39 = new GridColumnEx();
		GridColumnEx gridColumnEx40 = new GridColumnEx();
		GridColumnEx gridColumnEx41 = new GridColumnEx();
		GridColumnEx gridColumnEx42 = new GridColumnEx();
		GridColumnEx gridColumnEx43 = new GridColumnEx();
		new GridColumnEx();
		new GridColumnEx();
		new GridColumnEx();
		new GridColumnEx();
		new GridColumnEx();
		new GridColumnEx();
		new GridColumnEx();
		gridColumnEx.Name = "clItemCode";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.OITM_ItemCode;
		gridColumnEx.FieldName = "ItemCode";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clItemName";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.OITM_ItemName;
		gridColumnEx2.FieldName = "ItemName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "clDfltWH";
		gridColumnEx3.Caption = Apzon.Commons.Properties.Resources.OWHS_WhsCode;
		gridColumnEx3.FieldName = "DefaultWarehouse";
		gridColumnEx3.Width = 120;
		gridColumnEx3.Visible = false;
		gridColumnEx4.Name = "clOnHand";
		gridColumnEx4.Caption = Apzon.Commons.Properties.Resources.OITW_gcOnHand;
		gridColumnEx4.FieldName = "QuantityOnStock";
		gridColumnEx4.Width = 120;
		gridColumnEx4.DisplayFormat.FormatType = FormatType.Numeric;
		gridColumnEx4.DisplayFormat.FormatString = "n0";
		gridColumnEx4.Visible = true;
		gridColumnEx5.Name = "InvntryUom";
		gridColumnEx5.Caption = "Invntry Uom";
		gridColumnEx5.FieldName = "InvntryUom";
		gridColumnEx5.Width = 120;
		gridColumnEx5.Visible = false;
		gridColumnEx6.Name = "CodeBars";
		gridColumnEx6.Caption = Apzon.Commons.Properties.Resources.OITM_CodeBars;
		gridColumnEx6.FieldName = "CodeBars";
		gridColumnEx6.Width = 120;
		gridColumnEx6.Visible = false;
		gridColumnEx7.Name = "FrgnName";
		gridColumnEx7.Caption = Apzon.Commons.Properties.Resources.OITM_FrgnName;
		gridColumnEx7.FieldName = "FrgnName";
		gridColumnEx7.Width = 120;
		gridColumnEx7.Visible = false;
		gridColumnEx8.Name = "ItemType";
		gridColumnEx8.Caption = Apzon.Commons.Properties.Resources.OITM_ItemType;
		gridColumnEx8.FieldName = "ItemTypes";
		gridColumnEx8.Width = 120;
		gridColumnEx8.Visible = false;
		gridColumnEx9.Name = "ItemGroup";
		gridColumnEx9.Caption = Apzon.Commons.Properties.Resources.OITM_ItmsGrpCod;
		gridColumnEx9.FieldName = "ItmsGrpCod";
		gridColumnEx9.Width = 120;
		gridColumnEx9.Visible = false;
		gridColumnEx10.Name = "InvntItem";
		gridColumnEx10.Caption = "InvntItem";
		gridColumnEx10.FieldName = "InvntItem";
		gridColumnEx10.Width = 120;
		gridColumnEx10.Visible = false;
		gridColumnEx11.Name = "PrchseItem";
		gridColumnEx11.Caption = "PrchseItem";
		gridColumnEx11.FieldName = "PrchseItem";
		gridColumnEx11.Width = 120;
		gridColumnEx11.Visible = false;
		gridColumnEx12.Name = "SellItem";
		gridColumnEx12.Caption = "SellItem";
		gridColumnEx12.FieldName = "SellItem";
		gridColumnEx12.Width = 120;
		gridColumnEx12.Visible = false;
		gridColumnEx13.Name = "FirmCode";
		gridColumnEx13.Caption = "FirmCode";
		gridColumnEx13.FieldName = "FirmCode";
		gridColumnEx13.Width = 120;
		gridColumnEx13.Visible = false;
		gridColumnEx14.Name = "ShipType";
		gridColumnEx14.Caption = "ShipType";
		gridColumnEx14.FieldName = "ShipType";
		gridColumnEx14.Width = 120;
		gridColumnEx14.Visible = false;
		gridColumnEx15.Name = "ManSerNum";
		gridColumnEx15.Caption = "ManSerNum";
		gridColumnEx15.FieldName = "ManSerNum";
		gridColumnEx15.Width = 120;
		gridColumnEx15.Visible = false;
		gridColumnEx16.Name = "validFor";
		gridColumnEx16.Caption = "validFor";
		gridColumnEx16.FieldName = "validFor";
		gridColumnEx16.Width = 120;
		gridColumnEx16.Visible = false;
		gridColumnEx17.Name = "CardCode";
		gridColumnEx17.Caption = "CardCode";
		gridColumnEx17.FieldName = "CardCode";
		gridColumnEx17.Width = 120;
		gridColumnEx17.Visible = false;
		gridColumnEx18.Name = "SuppCatNum";
		gridColumnEx18.Caption = "SuppCatNum";
		gridColumnEx18.FieldName = "SuppCatNum";
		gridColumnEx18.Width = 120;
		gridColumnEx18.Visible = false;
		gridColumnEx19.Name = "BuyUnitMsr";
		gridColumnEx19.Caption = "BuyUnitMsr";
		gridColumnEx19.FieldName = "BuyUnitMsr";
		gridColumnEx19.Width = 120;
		gridColumnEx19.Visible = false;
		gridColumnEx20.Name = "NumInBuy";
		gridColumnEx20.Caption = "NumInBuy";
		gridColumnEx20.FieldName = "NumInBuy";
		gridColumnEx20.Width = 120;
		gridColumnEx20.Visible = false;
		gridColumnEx21.Name = "PurPackMsr";
		gridColumnEx21.Caption = "PurPackMsr";
		gridColumnEx21.FieldName = "PurPackMsr";
		gridColumnEx21.Width = 120;
		gridColumnEx21.Visible = false;
		gridColumnEx22.Name = "PurPackUn";
		gridColumnEx22.Caption = "PurPackUn";
		gridColumnEx22.FieldName = "PurPackUn";
		gridColumnEx22.Width = 120;
		gridColumnEx22.Visible = false;
		gridColumnEx23.Name = "PurFactor1";
		gridColumnEx23.Caption = "PurFactor1";
		gridColumnEx23.FieldName = "PurFactor1";
		gridColumnEx23.Width = 120;
		gridColumnEx23.Visible = false;
		gridColumnEx24.Name = "PurFactor2";
		gridColumnEx24.Caption = "PurFactor2";
		gridColumnEx24.FieldName = "PurFactor2";
		gridColumnEx24.Width = 120;
		gridColumnEx24.Visible = false;
		gridColumnEx25.Name = "PurFactor3";
		gridColumnEx25.Caption = "PurFactor3";
		gridColumnEx25.FieldName = "PurFactor3";
		gridColumnEx25.Width = 120;
		gridColumnEx25.Visible = false;
		gridColumnEx26.Name = "PurFactor4";
		gridColumnEx26.Caption = "PurFactor4";
		gridColumnEx26.FieldName = "PurFactor4";
		gridColumnEx26.Width = 120;
		gridColumnEx26.Visible = false;
		gridColumnEx27.Name = "SalUnitMsr";
		gridColumnEx27.Caption = "SalUnitMsr";
		gridColumnEx27.FieldName = "SalUnitMsr";
		gridColumnEx27.Width = 120;
		gridColumnEx27.Visible = false;
		gridColumnEx28.Name = "NumInSale";
		gridColumnEx28.Caption = "NumInSale";
		gridColumnEx28.FieldName = "NumInSale";
		gridColumnEx28.Width = 120;
		gridColumnEx28.Visible = false;
		gridColumnEx29.Name = "SalPackMsr";
		gridColumnEx29.Caption = "SalPackMsr";
		gridColumnEx29.FieldName = "SalPackMsr";
		gridColumnEx29.Width = 120;
		gridColumnEx29.Visible = false;
		gridColumnEx30.Name = "SalPackUn";
		gridColumnEx30.Caption = "SalPackUn";
		gridColumnEx30.FieldName = "SalPackUn";
		gridColumnEx30.Width = 120;
		gridColumnEx30.Visible = false;
		gridColumnEx31.Name = "SalFactor1";
		gridColumnEx31.Caption = "SalFactor1";
		gridColumnEx31.FieldName = "SalFactor1";
		gridColumnEx31.Width = 120;
		gridColumnEx31.Visible = false;
		gridColumnEx32.Name = "SalFactor2";
		gridColumnEx32.Caption = "SalFactor2";
		gridColumnEx32.FieldName = "SalFactor2";
		gridColumnEx32.Width = 120;
		gridColumnEx32.Visible = false;
		gridColumnEx33.Name = "SalFactor3";
		gridColumnEx33.Caption = "SalFactor3";
		gridColumnEx33.FieldName = "SalFactor3";
		gridColumnEx33.Width = 120;
		gridColumnEx33.Visible = false;
		gridColumnEx34.Name = "SalFactor4";
		gridColumnEx34.Caption = "SalFactor4";
		gridColumnEx34.FieldName = "SalFactor4";
		gridColumnEx34.Width = 120;
		gridColumnEx34.Visible = false;
		gridColumnEx35.Name = "GLMethods";
		gridColumnEx35.Caption = "GLMethods";
		gridColumnEx35.FieldName = "GLMethods";
		gridColumnEx35.Width = 120;
		gridColumnEx35.Visible = false;
		gridColumnEx36.Name = "EvalSystem";
		gridColumnEx36.Caption = "EvalSystem";
		gridColumnEx36.FieldName = "EvalSystem";
		gridColumnEx36.Width = 120;
		gridColumnEx36.Visible = false;
		gridColumnEx37.Name = "AvgPrice";
		gridColumnEx37.Caption = "AvgPrice";
		gridColumnEx37.FieldName = "AvgPrice";
		gridColumnEx37.Width = 120;
		gridColumnEx37.Visible = false;
		gridColumnEx38.Name = "ByWh";
		gridColumnEx38.Caption = "ByWh";
		gridColumnEx38.FieldName = "ByWh";
		gridColumnEx38.Width = 120;
		gridColumnEx38.Visible = false;
		gridColumnEx39.Name = "ReorderQty";
		gridColumnEx39.Caption = "ReorderQty";
		gridColumnEx39.FieldName = "ReorderQty";
		gridColumnEx39.Width = 120;
		gridColumnEx39.Visible = false;
		gridColumnEx40.Name = "MinLevel";
		gridColumnEx40.Caption = "MinLevel";
		gridColumnEx40.FieldName = "MinLevel";
		gridColumnEx40.Width = 120;
		gridColumnEx40.Visible = false;
		gridColumnEx40.Name = "MaxLevel";
		gridColumnEx40.Caption = "MaxLevel";
		gridColumnEx40.FieldName = "MaxLevel";
		gridColumnEx40.Width = 120;
		gridColumnEx40.Visible = false;
		gridColumnEx41.Name = "VatGroupPu";
		gridColumnEx41.Caption = "VatGroupPu";
		gridColumnEx41.FieldName = "PurchaseVATGroup";
		gridColumnEx41.Width = 120;
		gridColumnEx41.Visible = false;
		gridColumnEx42.Name = "VatGourpSa";
		gridColumnEx42.Caption = "VatGourpSa";
		gridColumnEx42.FieldName = "SalesVATGroup";
		gridColumnEx42.Width = 120;
		gridColumnEx42.Visible = false;
		gridColumnEx43.Name = "Series";
		gridColumnEx43.Caption = "Series";
		gridColumnEx43.FieldName = "Series";
		gridColumnEx43.Width = 120;
		gridColumnEx43.Visible = false;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[43]
		{
			gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5, gridColumnEx6, gridColumnEx7, gridColumnEx8, gridColumnEx9, gridColumnEx10,
			gridColumnEx11, gridColumnEx12, gridColumnEx13, gridColumnEx14, gridColumnEx15, gridColumnEx16, gridColumnEx17, gridColumnEx18, gridColumnEx19, gridColumnEx20,
			gridColumnEx21, gridColumnEx22, gridColumnEx23, gridColumnEx24, gridColumnEx25, gridColumnEx26, gridColumnEx27, gridColumnEx28, gridColumnEx29, gridColumnEx30,
			gridColumnEx31, gridColumnEx32, gridColumnEx33, gridColumnEx34, gridColumnEx35, gridColumnEx36, gridColumnEx37, gridColumnEx38, gridColumnEx39, gridColumnEx40,
			gridColumnEx41, gridColumnEx42, gridColumnEx43
		};
		columns.AddRange(columns2);
	}

	private async Task LoadDataItemsInventory()
	{
		try
		{
			using HttpClientHelper<ItemsImpl> helper = new HttpClientHelper<ItemsImpl>(Configuration.ApiServer, "Commons");
			List<ItemsImpl> dataSource = await helper.GetListAsync("ItemMaterInventory", new
			{
				BPLId = GlobalData.DefaultBranch
			});
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataSalesOder()
	{
		try
		{
			using HttpClientHelper<DocumentsImpl> helper = new HttpClientHelper<DocumentsImpl>(Configuration.ApiServer, "ProductionOder");
			List<DocumentsImpl> dataSource = await helper.GetListAsync("GetList_SalesOder");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataGroupControlKey()
	{
		try
		{
			using HttpClientHelper<GroupControlKeyImpl> helper = new HttpClientHelper<GroupControlKeyImpl>(Configuration.ApiServer, "GroupControlKey");
			List<GroupControlKeyImpl> dataSource = await helper.GetListAsync("GetListGroupControlKey");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsSalesOder()
	{
		Text = "List of Sales Oder";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		GridColumnEx gridColumnEx5 = new GridColumnEx();
		GridColumnEx gridColumnEx6 = new GridColumnEx();
		GridColumnEx gridColumnEx7 = new GridColumnEx();
		gridColumnEx.Name = "cl_DocNum";
		gridColumnEx.Caption = "Document Number";
		gridColumnEx.FieldName = "DocNum";
		gridColumnEx.Width = 100;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "cl_DocDate";
		gridColumnEx2.Caption = "Date";
		gridColumnEx2.FieldName = "DocDate";
		gridColumnEx2.Width = 100;
		gridColumnEx2.Visible = true;
		gridColumnEx7.Name = "cl_CardCode";
		gridColumnEx7.Caption = "Customer Code";
		gridColumnEx7.FieldName = "CardCode";
		gridColumnEx7.Width = 250;
		gridColumnEx7.Visible = true;
		gridColumnEx3.Name = "cl_CardName";
		gridColumnEx3.Caption = "Customer Name";
		gridColumnEx3.FieldName = "CardName";
		gridColumnEx3.Width = 250;
		gridColumnEx3.Visible = true;
		gridColumnEx4.Name = "cl_Comments";
		gridColumnEx4.Caption = "Remarks";
		gridColumnEx4.FieldName = "Comments";
		gridColumnEx4.Width = 150;
		gridColumnEx4.Visible = true;
		gridColumnEx5.Name = "cl_DocDueDate";
		gridColumnEx5.Caption = "Due Date";
		gridColumnEx5.FieldName = "DocDueDate";
		gridColumnEx5.Width = 100;
		gridColumnEx5.Visible = true;
		gridColumnEx6.Name = "cl_DocEntry";
		gridColumnEx6.Caption = "Document Entry";
		gridColumnEx6.FieldName = "DocEntry";
		gridColumnEx6.Width = 100;
		gridColumnEx6.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[7] { gridColumnEx, gridColumnEx7, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5, gridColumnEx6 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataBillOfMater()
	{
		try
		{
			if (_isBOMSearch)
			{
				grd_CFL.DataSource = _dtBOMSearch;
				grd_CFL.RefreshDataSource();
				return;
			}
			using HttpClientHelper<BillOfMasterials_Impl_OITT> helper = new HttpClientHelper<BillOfMasterials_Impl_OITT>(Configuration.ApiServer, "BillOfMasterials");
			List<BillOfMasterials_Impl_OITT> dataSource = await helper.GetListAsync("GetList_BillOfMster");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataBOM()
	{
		try
		{
			using HttpClientHelper<ApzBillOfMaterials> helper = new HttpClientHelper<ApzBillOfMaterials>(Configuration.ApiServer, "ApzBillOfMaterials");
			List<ApzBillOfMaterials> dataSource = await helper.GetListAsync("GetList_BOM");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsBillOfMater()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfBillOfMaterials;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		GridColumnEx gridColumnEx5 = new GridColumnEx();
		GridColumnEx gridColumnEx6 = new GridColumnEx();
		gridColumnEx.Name = "cl_ItemCode";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.lb_ItemNo;
		gridColumnEx.FieldName = "Code";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "cl_Qauntity";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.lb_Quantity;
		gridColumnEx2.FieldName = "Qauntity";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = false;
		gridColumnEx3.Name = "cl_ToWH";
		gridColumnEx3.Caption = Apzon.Commons.Properties.Resources.Line_gvLine_gcWhseCode;
		gridColumnEx3.FieldName = "ToWH";
		gridColumnEx3.Width = 120;
		gridColumnEx3.Visible = true;
		gridColumnEx4.Name = "cl_TreeType";
		gridColumnEx4.Caption = Apzon.Commons.Properties.Resources.lb_BomType;
		gridColumnEx4.FieldName = "TreeType";
		gridColumnEx4.Width = 120;
		gridColumnEx4.Visible = false;
		gridColumnEx5.Name = "cl_PriceList";
		gridColumnEx5.Caption = Apzon.Commons.Properties.Resources.ITM1_PriceList;
		gridColumnEx5.FieldName = "PriceList";
		gridColumnEx5.Width = 120;
		gridColumnEx5.Visible = false;
		gridColumnEx6.Name = "cl_OcrCode";
		gridColumnEx6.Caption = "OcrCode";
		gridColumnEx6.FieldName = "OcrCode";
		gridColumnEx6.Width = 120;
		gridColumnEx6.Visible = false;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[6] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5, gridColumnEx6 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataBoMItem()
	{
		try
		{
			using HttpClientHelper<ItemsImpl> helper = new HttpClientHelper<ItemsImpl>(Configuration.ApiServer, "BillOfMasterials");
			List<ItemsImpl> dataSource = await helper.GetListAsync("GetItemCFL");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsBoMItem()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfItem;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		gridColumnEx.Name = "cl_ItemCode";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.lb_ItemNo;
		gridColumnEx.FieldName = "ItemCode";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "cl_ItemName";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.Line_gvLine_gcItemName;
		gridColumnEx2.FieldName = "ItemName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "cl_OnHand";
		gridColumnEx3.Caption = Apzon.Commons.Properties.Resources.OITW_gcOnHand;
		gridColumnEx3.FieldName = "OnHand";
		gridColumnEx3.Width = 120;
		gridColumnEx3.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[3] { gridColumnEx, gridColumnEx2, gridColumnEx3 };
		columns.AddRange(columns2);
	}

	private void LoadColumnBOM()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfItem;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "cl_ItemCode";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.lb_ItemNo;
		gridColumnEx.FieldName = "Code";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "cl_ItemName";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.Line_gvLine_gcItemName;
		gridColumnEx2.FieldName = "ItemName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsItemType()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfItem;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		GridColumnEx gridColumnEx5 = new GridColumnEx();
		GridColumnEx gridColumnEx6 = new GridColumnEx();
		GridColumnEx gridColumnEx7 = new GridColumnEx();
		GridColumnEx gridColumnEx8 = new GridColumnEx();
		GridColumnEx gridColumnEx9 = new GridColumnEx();
		GridColumnEx gridColumnEx10 = new GridColumnEx();
		GridColumnEx gridColumnEx11 = new GridColumnEx();
		GridColumnEx gridColumnEx12 = new GridColumnEx();
		GridColumnEx gridColumnEx13 = new GridColumnEx();
		GridColumnEx gridColumnEx14 = new GridColumnEx();
		GridColumnEx gridColumnEx15 = new GridColumnEx();
		GridColumnEx gridColumnEx16 = new GridColumnEx();
		GridColumnEx gridColumnEx17 = new GridColumnEx();
		GridColumnEx gridColumnEx18 = new GridColumnEx();
		GridColumnEx gridColumnEx19 = new GridColumnEx();
		GridColumnEx gridColumnEx20 = new GridColumnEx();
		GridColumnEx gridColumnEx21 = new GridColumnEx();
		GridColumnEx gridColumnEx22 = new GridColumnEx();
		GridColumnEx gridColumnEx23 = new GridColumnEx();
		GridColumnEx gridColumnEx24 = new GridColumnEx();
		GridColumnEx gridColumnEx25 = new GridColumnEx();
		GridColumnEx gridColumnEx26 = new GridColumnEx();
		GridColumnEx gridColumnEx27 = new GridColumnEx();
		GridColumnEx gridColumnEx28 = new GridColumnEx();
		GridColumnEx gridColumnEx29 = new GridColumnEx();
		GridColumnEx gridColumnEx30 = new GridColumnEx();
		GridColumnEx gridColumnEx31 = new GridColumnEx();
		GridColumnEx gridColumnEx32 = new GridColumnEx();
		GridColumnEx gridColumnEx33 = new GridColumnEx();
		GridColumnEx gridColumnEx34 = new GridColumnEx();
		GridColumnEx gridColumnEx35 = new GridColumnEx();
		GridColumnEx gridColumnEx36 = new GridColumnEx();
		GridColumnEx gridColumnEx37 = new GridColumnEx();
		GridColumnEx gridColumnEx38 = new GridColumnEx();
		GridColumnEx gridColumnEx39 = new GridColumnEx();
		GridColumnEx gridColumnEx40 = new GridColumnEx();
		GridColumnEx gridColumnEx41 = new GridColumnEx();
		GridColumnEx gridColumnEx42 = new GridColumnEx();
		GridColumnEx gridColumnEx43 = new GridColumnEx();
		GridColumnEx gridColumnEx44 = new GridColumnEx();
		GridColumnEx gridColumnEx45 = new GridColumnEx();
		GridColumnEx gridColumnEx46 = new GridColumnEx();
		GridColumnEx gridColumnEx47 = new GridColumnEx();
		GridColumnEx gridColumnEx48 = new GridColumnEx();
		GridColumnEx gridColumnEx49 = new GridColumnEx();
		GridColumnEx gridColumnEx50 = new GridColumnEx();
		GridColumnEx gridColumnEx51 = new GridColumnEx();
		GridColumnEx gridColumnEx52 = new GridColumnEx();
		new GridColumnEx();
		new GridColumnEx();
		new GridColumnEx();
		new GridColumnEx();
		new GridColumnEx();
		new GridColumnEx();
		new GridColumnEx();
		new GridColumnEx();
		gridColumnEx.Name = "clItemCode";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.OITM_ItemCode;
		gridColumnEx.FieldName = "ItemCode";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clItemName";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.OITM_ItemName;
		gridColumnEx2.FieldName = "ItemName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "clDfltWH";
		gridColumnEx3.Caption = Apzon.Commons.Properties.Resources.OWHS_WhsCode;
		gridColumnEx3.FieldName = "DefaultWarehouse";
		gridColumnEx3.Width = 120;
		gridColumnEx3.Visible = true;
		gridColumnEx4.Name = "clOnHand";
		gridColumnEx4.Caption = Apzon.Commons.Properties.Resources.OITW_gcOnHand;
		gridColumnEx4.FieldName = "QuantityOnStock";
		gridColumnEx4.Width = 120;
		gridColumnEx4.DisplayFormat.FormatType = FormatType.Numeric;
		gridColumnEx4.DisplayFormat.FormatString = "n0";
		gridColumnEx4.Visible = true;
		gridColumnEx5.Name = "InvntryUom";
		gridColumnEx5.Caption = "Invntry Uom";
		gridColumnEx5.FieldName = "InvntryUom";
		gridColumnEx5.Width = 120;
		gridColumnEx5.Visible = true;
		gridColumnEx6.Name = "CodeBars";
		gridColumnEx6.Caption = "Bar Code";
		gridColumnEx6.FieldName = "CodeBars";
		gridColumnEx6.Width = 120;
		gridColumnEx6.Visible = false;
		gridColumnEx7.Name = "FrgnName";
		gridColumnEx7.Caption = Apzon.Commons.Properties.Resources.OITM_FrgnName;
		gridColumnEx7.FieldName = "FrgnName";
		gridColumnEx7.Width = 120;
		gridColumnEx7.Visible = true;
		gridColumnEx8.Name = "ItemType";
		gridColumnEx8.Caption = Apzon.Commons.Properties.Resources.OITM_ItemType;
		gridColumnEx8.FieldName = "ItemTypes";
		gridColumnEx8.Width = 120;
		gridColumnEx8.Visible = true;
		gridColumnEx9.Name = "ItemGroup";
		gridColumnEx9.Caption = Apzon.Commons.Properties.Resources.OITM_ItmsGrpCod;
		gridColumnEx9.FieldName = "ItmsGrpCod";
		gridColumnEx9.Width = 120;
		gridColumnEx9.Visible = false;
		gridColumnEx10.Name = "InvntItem";
		gridColumnEx10.Caption = "InvntItem";
		gridColumnEx10.FieldName = "InvntItem";
		gridColumnEx10.Width = 120;
		gridColumnEx10.Visible = false;
		gridColumnEx11.Name = "PrchseItem";
		gridColumnEx11.Caption = "PrchseItem";
		gridColumnEx11.FieldName = "PrchseItem";
		gridColumnEx11.Width = 120;
		gridColumnEx11.Visible = false;
		gridColumnEx12.Name = "SellItem";
		gridColumnEx12.Caption = "SellItem";
		gridColumnEx12.FieldName = "SellItem";
		gridColumnEx12.Width = 120;
		gridColumnEx12.Visible = false;
		gridColumnEx13.Name = "FirmCode";
		gridColumnEx13.Caption = "FirmCode";
		gridColumnEx13.FieldName = "FirmCode";
		gridColumnEx13.Width = 120;
		gridColumnEx13.Visible = false;
		gridColumnEx14.Name = "ShipType";
		gridColumnEx14.Caption = "ShipType";
		gridColumnEx14.FieldName = "ShipType";
		gridColumnEx14.Width = 120;
		gridColumnEx14.Visible = false;
		gridColumnEx15.Name = "ManSerNum";
		gridColumnEx15.Caption = "ManSerNum";
		gridColumnEx15.FieldName = "ManSerNum";
		gridColumnEx15.Width = 120;
		gridColumnEx15.Visible = false;
		gridColumnEx16.Name = "validFor";
		gridColumnEx16.Caption = "validFor";
		gridColumnEx16.FieldName = "validFor";
		gridColumnEx16.Width = 120;
		gridColumnEx16.Visible = false;
		gridColumnEx17.Name = "CardCode";
		gridColumnEx17.Caption = "CardCode";
		gridColumnEx17.FieldName = "CardCode";
		gridColumnEx17.Width = 120;
		gridColumnEx17.Visible = false;
		gridColumnEx18.Name = "SuppCatNum";
		gridColumnEx18.Caption = "SuppCatNum";
		gridColumnEx18.FieldName = "SuppCatNum";
		gridColumnEx18.Width = 120;
		gridColumnEx18.Visible = false;
		gridColumnEx19.Name = "BuyUnitMsr";
		gridColumnEx19.Caption = "BuyUnitMsr";
		gridColumnEx19.FieldName = "BuyUnitMsr";
		gridColumnEx19.Width = 120;
		gridColumnEx19.Visible = false;
		gridColumnEx20.Name = "NumInBuy";
		gridColumnEx20.Caption = "NumInBuy";
		gridColumnEx20.FieldName = "NumInBuy";
		gridColumnEx20.Width = 120;
		gridColumnEx20.Visible = false;
		gridColumnEx21.Name = "PurPackMsr";
		gridColumnEx21.Caption = "PurPackMsr";
		gridColumnEx21.FieldName = "PurPackMsr";
		gridColumnEx21.Width = 120;
		gridColumnEx21.Visible = false;
		gridColumnEx22.Name = "PurPackUn";
		gridColumnEx22.Caption = "PurPackUn";
		gridColumnEx22.FieldName = "PurPackUn";
		gridColumnEx22.Width = 120;
		gridColumnEx22.Visible = false;
		gridColumnEx23.Name = "PurFactor1";
		gridColumnEx23.Caption = "PurFactor1";
		gridColumnEx23.FieldName = "PurFactor1";
		gridColumnEx23.Width = 120;
		gridColumnEx23.Visible = false;
		gridColumnEx24.Name = "PurFactor2";
		gridColumnEx24.Caption = "PurFactor2";
		gridColumnEx24.FieldName = "PurFactor2";
		gridColumnEx24.Width = 120;
		gridColumnEx24.Visible = false;
		gridColumnEx25.Name = "PurFactor3";
		gridColumnEx25.Caption = "PurFactor3";
		gridColumnEx25.FieldName = "PurFactor3";
		gridColumnEx25.Width = 120;
		gridColumnEx25.Visible = false;
		gridColumnEx26.Name = "PurFactor4";
		gridColumnEx26.Caption = "PurFactor4";
		gridColumnEx26.FieldName = "PurFactor4";
		gridColumnEx26.Width = 120;
		gridColumnEx26.Visible = false;
		gridColumnEx27.Name = "SalUnitMsr";
		gridColumnEx27.Caption = "SalUnitMsr";
		gridColumnEx27.FieldName = "SalUnitMsr";
		gridColumnEx27.Width = 120;
		gridColumnEx27.Visible = false;
		gridColumnEx28.Name = "NumInSale";
		gridColumnEx28.Caption = "NumInSale";
		gridColumnEx28.FieldName = "NumInSale";
		gridColumnEx28.Width = 120;
		gridColumnEx28.Visible = false;
		gridColumnEx29.Name = "SalPackMsr";
		gridColumnEx29.Caption = "SalPackMsr";
		gridColumnEx29.FieldName = "SalPackMsr";
		gridColumnEx29.Width = 120;
		gridColumnEx29.Visible = false;
		gridColumnEx30.Name = "SalPackUn";
		gridColumnEx30.Caption = "SalPackUn";
		gridColumnEx30.FieldName = "SalPackUn";
		gridColumnEx30.Width = 120;
		gridColumnEx30.Visible = false;
		gridColumnEx31.Name = "SalFactor1";
		gridColumnEx31.Caption = "SalFactor1";
		gridColumnEx31.FieldName = "SalFactor1";
		gridColumnEx31.Width = 120;
		gridColumnEx31.Visible = false;
		gridColumnEx32.Name = "SalFactor2";
		gridColumnEx32.Caption = "SalFactor2";
		gridColumnEx32.FieldName = "SalFactor2";
		gridColumnEx32.Width = 120;
		gridColumnEx32.Visible = false;
		gridColumnEx33.Name = "SalFactor3";
		gridColumnEx33.Caption = "SalFactor3";
		gridColumnEx33.FieldName = "SalFactor3";
		gridColumnEx33.Width = 120;
		gridColumnEx33.Visible = false;
		gridColumnEx34.Name = "SalFactor4";
		gridColumnEx34.Caption = "SalFactor4";
		gridColumnEx34.FieldName = "SalFactor4";
		gridColumnEx34.Width = 120;
		gridColumnEx34.Visible = false;
		gridColumnEx35.Name = "GLMethods";
		gridColumnEx35.Caption = "GLMethods";
		gridColumnEx35.FieldName = "GLMethods";
		gridColumnEx35.Width = 120;
		gridColumnEx35.Visible = false;
		gridColumnEx36.Name = "EvalSystem";
		gridColumnEx36.Caption = "EvalSystem";
		gridColumnEx36.FieldName = "EvalSystem";
		gridColumnEx36.Width = 120;
		gridColumnEx36.Visible = false;
		gridColumnEx37.Name = "AvgPrice";
		gridColumnEx37.Caption = "AvgPrice";
		gridColumnEx37.FieldName = "AvgPrice";
		gridColumnEx37.Width = 120;
		gridColumnEx37.Visible = false;
		gridColumnEx38.Name = "ByWh";
		gridColumnEx38.Caption = "ByWh";
		gridColumnEx38.FieldName = "ByWh";
		gridColumnEx38.Width = 120;
		gridColumnEx38.Visible = false;
		gridColumnEx39.Name = "ReorderQty";
		gridColumnEx39.Caption = "ReorderQty";
		gridColumnEx39.FieldName = "ReorderQty";
		gridColumnEx39.Width = 120;
		gridColumnEx39.Visible = false;
		gridColumnEx40.Name = "MinLevel";
		gridColumnEx40.Caption = "MinLevel";
		gridColumnEx40.FieldName = "MinLevel";
		gridColumnEx40.Width = 120;
		gridColumnEx40.Visible = false;
		gridColumnEx40.Name = "MaxLevel";
		gridColumnEx40.Caption = "MaxLevel";
		gridColumnEx40.FieldName = "MaxLevel";
		gridColumnEx40.Width = 120;
		gridColumnEx40.Visible = false;
		gridColumnEx41.Name = "VatGroupPu";
		gridColumnEx41.Caption = "VatGroupPu";
		gridColumnEx41.FieldName = "PurchaseVATGroup";
		gridColumnEx41.Width = 120;
		gridColumnEx41.Visible = true;
		gridColumnEx42.Name = "VatGourpSa";
		gridColumnEx42.Caption = "VatGourpSa";
		gridColumnEx42.FieldName = "SalesVATGroup";
		gridColumnEx42.Width = 120;
		gridColumnEx42.Visible = true;
		gridColumnEx43.Name = "ColValidTo";
		gridColumnEx43.Caption = "ValidTo";
		gridColumnEx43.FieldName = "ValidTo";
		gridColumnEx43.Width = 120;
		gridColumnEx43.Visible = false;
		gridColumnEx44.Name = "ColValidFrom";
		gridColumnEx44.Caption = "ValidFrom";
		gridColumnEx44.FieldName = "ValidFrom";
		gridColumnEx44.Width = 120;
		gridColumnEx44.Visible = false;
		gridColumnEx45.Name = "ColValidRemarks";
		gridColumnEx45.Caption = "ValidRemarks";
		gridColumnEx45.FieldName = "ValidRemarks";
		gridColumnEx45.Width = 120;
		gridColumnEx45.Visible = false;
		gridColumnEx46.Name = "ColFrozenTo";
		gridColumnEx46.Caption = "FrozenTo";
		gridColumnEx46.FieldName = "FrozenTo";
		gridColumnEx46.Width = 120;
		gridColumnEx46.Visible = false;
		gridColumnEx47.Name = "ColFrozenFrom";
		gridColumnEx47.Caption = "FrozenFrom";
		gridColumnEx47.FieldName = "FrozenFrom";
		gridColumnEx47.Width = 120;
		gridColumnEx47.Visible = false;
		gridColumnEx48.Name = "ColFrozenRemarks";
		gridColumnEx48.Caption = "FrozenRemarks";
		gridColumnEx48.FieldName = "FrozenRemarks";
		gridColumnEx48.Width = 120;
		gridColumnEx48.Visible = false;
		gridColumnEx49.Name = "ColMngMethod";
		gridColumnEx49.Caption = "MngMethod";
		gridColumnEx49.FieldName = "MngMethod";
		gridColumnEx49.Width = 120;
		gridColumnEx49.Visible = false;
		gridColumnEx50.Name = "ColManBtchNum";
		gridColumnEx50.Caption = "ManBtchNum";
		gridColumnEx50.FieldName = "ManBtchNum";
		gridColumnEx50.Width = 120;
		gridColumnEx50.Visible = false;
		gridColumnEx51.Name = "ColfrozenFor";
		gridColumnEx51.Caption = "frozenFor";
		gridColumnEx51.FieldName = "frozenFor";
		gridColumnEx51.Width = 120;
		gridColumnEx51.Visible = false;
		gridColumnEx52.Name = "Series";
		gridColumnEx52.Caption = "Series";
		gridColumnEx52.FieldName = "Series";
		gridColumnEx52.Width = 120;
		gridColumnEx52.Visible = false;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[52]
		{
			gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5, gridColumnEx6, gridColumnEx7, gridColumnEx8, gridColumnEx9, gridColumnEx10,
			gridColumnEx11, gridColumnEx12, gridColumnEx13, gridColumnEx14, gridColumnEx15, gridColumnEx16, gridColumnEx17, gridColumnEx18, gridColumnEx19, gridColumnEx20,
			gridColumnEx21, gridColumnEx22, gridColumnEx23, gridColumnEx24, gridColumnEx25, gridColumnEx26, gridColumnEx27, gridColumnEx28, gridColumnEx29, gridColumnEx30,
			gridColumnEx31, gridColumnEx32, gridColumnEx33, gridColumnEx34, gridColumnEx35, gridColumnEx36, gridColumnEx37, gridColumnEx38, gridColumnEx39, gridColumnEx40,
			gridColumnEx41, gridColumnEx42, gridColumnEx43, gridColumnEx44, gridColumnEx45, gridColumnEx46, gridColumnEx47, gridColumnEx48, gridColumnEx49, gridColumnEx50,
			gridColumnEx51, gridColumnEx52
		};
		columns.AddRange(columns2);
	}

	private async Task LoadDataItemsByGroup()
	{
		try
		{
			using HttpClientHelper<ItemsImpl> helper = new HttpClientHelper<ItemsImpl>(Configuration.ApiServer, "Commons");
			List<ItemsImpl> dataSource = await helper.GetListAsync("GetItemMasterByGroup");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataItemsByItemType(string itemTypes)
	{
		try
		{
			using HttpClientHelper<ItemsImpl> helper = new HttpClientHelper<ItemsImpl>(Configuration.ApiServer, "Commons");
			List<ItemsImpl> dataSource = await helper.GetListAsync("GetItemMasterTypes", new
			{
				itemTypes = itemTypes,
				BPLId = GlobalData.DefaultBranch
			});
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataAccountPurpose()
	{
		try
		{
			using HttpClientHelper<Payments_AccountsImpl> helper = new HttpClientHelper<Payments_AccountsImpl>(Configuration.ApiServer, "ChartOfAccountPurpose");
			List<Payments_AccountsImpl> dataSource = await helper.GetListAsync("Cfl");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsAccountPurpose()
	{
		Text = "List of Payment Purpose";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "cl_AcctCode";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.lb_Code;
		gridColumnEx.FieldName = "AcctCode";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "cl_AcctName";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.lb_Name;
		gridColumnEx2.FieldName = "AcctName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsBPCatalog()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfBPCatalogNo;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "SubCatNum";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.lb_BPCatalogNo;
		gridColumnEx.FieldName = "SubCatNum";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "ItemCode";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.Line_gvLine_gcItemCode;
		gridColumnEx2.FieldName = "ItemCode";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataWhs()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Warehouse");
			DataTable dataSource = await helper.GetAsync("Cfl");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataWhsDocument()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Warehouse");
			DataTable dataSource = await helper.GetAsync("CflDocument");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataPriceList()
	{
		_ = 1;
		try
		{
			if (!string.IsNullOrWhiteSpace(GlobalData._ItemCode))
			{
				DataTable dataSource = await WinFuntions.GetPriceList(GlobalData._ItemCode);
				grd_CFL.DataSource = dataSource;
				grd_CFL.RefreshDataSource();
				return;
			}
			using HttpClientHelper<PriceListImpl> helper = new HttpClientHelper<PriceListImpl>(Configuration.ApiServer, "PriceList");
			List<PriceListImpl> dataSource2 = await helper.GetListAsync("Cfl");
			grd_CFL.DataSource = dataSource2;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsPriceList()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfPriceList;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		gridColumnEx.Name = "clCode";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.lb_PriceListNo;
		gridColumnEx.FieldName = "PriceListNo";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clName";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.lb_PriceListName;
		gridColumnEx2.FieldName = "PriceListName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "clPrice";
		gridColumnEx3.Caption = Apzon.Commons.Properties.Resources.lb_Price;
		gridColumnEx3.FieldName = "Price";
		gridColumnEx3.Width = 120;
		gridColumnEx3.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[3] { gridColumnEx, gridColumnEx2, gridColumnEx3 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataVatGroup(string catetory)
	{
		try
		{
			using HttpClientHelper<VatGroupsImpl> helper = new HttpClientHelper<VatGroupsImpl>(Configuration.ApiServer, "VatGroups");
			List<VatGroupsImpl> dataSource = await helper.GetListAsync("Cfl", new
			{
				Category = catetory
			});
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataVatGroup()
	{
		try
		{
			using HttpClientHelper<VatGroupsImpl> helper = new HttpClientHelper<VatGroupsImpl>(Configuration.ApiServer, "VatGroups");
			List<VatGroupsImpl> dataSource = await helper.GetListAsync("Cfl");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsVatGroup()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfVatGroups;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		GridColumnEx gridColumnEx5 = new GridColumnEx();
		gridColumnEx.Name = "clCode";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.lb_Code;
		gridColumnEx.FieldName = "Code";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clName";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.lb_Name;
		gridColumnEx2.FieldName = "Name";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "clCategory";
		gridColumnEx3.Caption = Apzon.Commons.Properties.Resources.OVTG_Category;
		gridColumnEx3.FieldName = "Category";
		gridColumnEx3.Width = 120;
		gridColumnEx3.Visible = true;
		gridColumnEx4.Name = "clTaxAccount";
		gridColumnEx4.Caption = Apzon.Commons.Properties.Resources.OVTG_TaxAccount;
		gridColumnEx4.FieldName = "TaxAccount";
		gridColumnEx4.Width = 120;
		gridColumnEx4.Visible = true;
		gridColumnEx5.Name = "clRate";
		gridColumnEx5.Caption = Apzon.Commons.Properties.Resources.OVTG_Rate;
		gridColumnEx5.FieldName = "Rate";
		gridColumnEx5.Width = 120;
		gridColumnEx5.Visible = true;
		gridColumnEx5.DisplayFormat.FormatType = FormatType.Numeric;
		gridColumnEx5.DisplayFormat.FormatString = "n3";
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[5] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataItemGroup()
	{
		try
		{
			using HttpClientHelper<ItemGroupsImpl> helper = new HttpClientHelper<ItemGroupsImpl>(Configuration.ApiServer, "ItemGroups");
			List<ItemGroupsImpl> dataSource = await helper.GetListAsync("Cfl");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsItemGroup()
	{
		Text = "List of Item Groups";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "clCode";
		gridColumnEx.Caption = "Code";
		gridColumnEx.FieldName = "Number";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clName";
		gridColumnEx2.Caption = "Group Name";
		gridColumnEx2.FieldName = "GroupName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataManufacture()
	{
		try
		{
			using HttpClientHelper<ChartOfAccountsImpl> helper = new HttpClientHelper<ChartOfAccountsImpl>(Configuration.ApiServer, "Manufacturers");
			List<ChartOfAccountsImpl> dataSource = await helper.GetListAsync("Cfl");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsManufacture()
	{
		Text = "List of Manufactures";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "clCode";
		gridColumnEx.Caption = "Code";
		gridColumnEx.FieldName = "Code";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clName";
		gridColumnEx2.Caption = "Name";
		gridColumnEx2.FieldName = "ManufacturerName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataShipping()
	{
		try
		{
			using HttpClientHelper<ChartOfAccountsImpl> helper = new HttpClientHelper<ChartOfAccountsImpl>(Configuration.ApiServer, "ShippingTypes");
			List<ChartOfAccountsImpl> dataSource = await helper.GetListAsync("Cfl");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsShipping()
	{
		Text = "List of Shipping Types";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		gridColumnEx.Name = "clCode";
		gridColumnEx.Caption = "Code";
		gridColumnEx.FieldName = "Code";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clName";
		gridColumnEx2.Caption = "Name";
		gridColumnEx2.FieldName = "Name";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "clWebsite";
		gridColumnEx3.Caption = "Website";
		gridColumnEx3.FieldName = "Website";
		gridColumnEx3.Width = 120;
		gridColumnEx3.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[3] { gridColumnEx, gridColumnEx2, gridColumnEx3 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataAccount()
	{
		try
		{
			using HttpClientHelper<ChartOfAccountsImpl> helper = new HttpClientHelper<ChartOfAccountsImpl>(Configuration.ApiServer, "ChartOfAccount");
			List<ChartOfAccountsImpl> dataSource = await helper.GetListAsync("Cfl");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsSaleEmployee()
	{
		Text = "List of Sales Employees";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "clCode";
		gridColumnEx.Caption = "Code";
		gridColumnEx.FieldName = "SalesEmployeeCode";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clName";
		gridColumnEx2.Caption = "Name";
		gridColumnEx2.FieldName = "SalesEmployeeName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataSaleEmployee()
	{
		try
		{
			using HttpClientHelper<SalesPersonsImpl> helper = new HttpClientHelper<SalesPersonsImpl>(Configuration.ApiServer, "SalesPersons");
			List<SalesPersonsImpl> dataSource = await helper.GetListAsync("Cfl");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataLead()
	{
		try
		{
			using HttpClientHelper<BusinessPartnersImpl> helper = new HttpClientHelper<BusinessPartnersImpl>(Configuration.ApiServer, "BusinessPartner");
			List<BusinessPartnersImpl> dataSource = await helper.GetListAsync("GetLead");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsLead()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfBusinessPartner;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		gridColumnEx.Name = "clCode";
		gridColumnEx.Caption = "Code";
		gridColumnEx.FieldName = "CardCode";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clName";
		gridColumnEx2.Caption = "Name";
		gridColumnEx2.FieldName = "CardName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "clBalance";
		gridColumnEx3.Caption = "Balance";
		gridColumnEx3.FieldName = "CurrentAccountBalance";
		gridColumnEx3.Width = 120;
		gridColumnEx3.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[3] { gridColumnEx, gridColumnEx2, gridColumnEx3 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataVendor()
	{
		try
		{
			using HttpClientHelper<BusinessPartnersImpl> helper = new HttpClientHelper<BusinessPartnersImpl>(Configuration.ApiServer, "BusinessPartner");
			List<BusinessPartnersImpl> dataSource = await helper.GetListAsync("GetVendor");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsVendor()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfBusinessVenDor;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		gridColumnEx.Name = "clCode";
		gridColumnEx.Caption = "Code";
		gridColumnEx.FieldName = "CardCode";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clName";
		gridColumnEx2.Caption = "Name";
		gridColumnEx2.FieldName = "CardName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "clBalance";
		gridColumnEx3.Caption = "Balance";
		gridColumnEx3.FieldName = "CurrentAccountBalance";
		gridColumnEx3.Width = 120;
		gridColumnEx3.Visible = true;
		gridColumnEx4.Name = "clCurrency";
		gridColumnEx4.Caption = "Currency";
		gridColumnEx4.FieldName = "Currency";
		gridColumnEx4.Width = 120;
		gridColumnEx4.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[4] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataBp()
	{
		try
		{
			using HttpClientHelper<BusinessPartnersImpl> bpClientHelper = new HttpClientHelper<BusinessPartnersImpl>(Configuration.ApiServer, "BusinessPartner");
			List<BusinessPartnersImpl> dataSource = (await bpClientHelper.GetListAsync("GetBp")).ToList();
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsBp()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfBusinessPartner;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		GridColumnEx gridColumnEx5 = new GridColumnEx();
		GridColumnEx gridColumnEx6 = new GridColumnEx();
		gridColumnEx.Name = "clCode";
		gridColumnEx.Caption = "Code";
		gridColumnEx.FieldName = "CardCode";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clName";
		gridColumnEx2.Caption = "Name";
		gridColumnEx2.FieldName = "CardName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "clBalance";
		gridColumnEx3.Caption = "Balance";
		gridColumnEx3.FieldName = "CurrentAccountBalance";
		gridColumnEx3.Width = 120;
		gridColumnEx3.Visible = true;
		gridColumnEx4.Name = "clCardType";
		gridColumnEx4.Caption = "CardType";
		gridColumnEx4.FieldName = "CardType";
		gridColumnEx4.Width = 120;
		gridColumnEx4.Visible = false;
		gridColumnEx5.Name = "clPhone1";
		gridColumnEx5.Caption = "Phone1";
		gridColumnEx5.FieldName = "Phone1";
		gridColumnEx5.Width = 120;
		gridColumnEx5.Visible = false;
		gridColumnEx6.Name = "clNameCntcPer";
		gridColumnEx6.Caption = "Name Contact Person";
		gridColumnEx6.FieldName = "Name";
		gridColumnEx6.Width = 120;
		gridColumnEx6.Visible = false;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[6] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5, gridColumnEx6 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataCustomer()
	{
		try
		{
			using HttpClientHelper<BusinessPartnersImpl> bpClientHelper = new HttpClientHelper<BusinessPartnersImpl>(Configuration.ApiServer, "BusinessPartner");
			List<BusinessPartnersImpl> dataSource = (await bpClientHelper.GetListAsync("ListCustomer")).ToList();
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataItems()
	{
		try
		{
			using HttpClientHelper<ItemsImpl> helper = new HttpClientHelper<ItemsImpl>(Configuration.ApiServer, "Commons");
			List<ItemsImpl> dataSource = (await helper.GetListAsync("ItemMasters", new
			{
				itemCode = "",
				BPLId = GlobalData.DefaultBranch,
				itObjType = _itemObjectType
			})).ToList();
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddChooseItemsGrid()
	{
		try
		{
			if (gv_CFL.GetSelectedRows() == null || gv_CFL.GetSelectedRows().Length == 0)
			{
				return;
			}
			int[] selectedRows = gv_CFL.GetSelectedRows();
			try
			{
				_dtSource = grd_CFL.DataSource as DataTable;
			}
			catch
			{
			}
			int[] array = selectedRows;
			foreach (int rowHandle in array)
			{
				dynamic val = (_listTypeDataRow.Contains(CFL_Type) ? gv_CFL.GetDataRow(rowHandle) : gv_CFL.GetRow(rowHandle));
				if (!CFL_MutiSelectMode)
				{
					ChooseItem = val;
					break;
				}
				((List<object>)ChooseItems).Add(val);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void AddChooseItems()
	{
		try
		{
			gv_CFL.SelectRows(0, gv_CFL.RowCount);
			int[] selectedRows = gv_CFL.GetSelectedRows();
			foreach (int rowHandle in selectedRows)
			{
				dynamic val = (_listTypeDataRow.Contains(CFL_Type) ? gv_CFL.GetDataRow(rowHandle) : gv_CFL.GetRow(rowHandle));
				if (!CFL_MutiSelectMode)
				{
					ChooseItem = val;
					break;
				}
				((List<object>)ChooseItems).Add(val);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void LoadColumnsCustomer()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfCustomers;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		gridColumnEx.Name = "clCode";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.lb_Code;
		gridColumnEx.FieldName = "CardCode";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clName";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.lb_Name;
		gridColumnEx2.FieldName = "CardName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "clBalance";
		gridColumnEx3.Caption = Apzon.Commons.Properties.Resources.lb_Balance;
		gridColumnEx3.FieldName = "CurrentAccountBalance";
		gridColumnEx3.Width = 120;
		gridColumnEx3.Visible = true;
		gridColumnEx4.Name = "clCurrency";
		gridColumnEx4.Caption = Apzon.Commons.Properties.Resources.Doc_gvDoc_gcDocCurrency;
		gridColumnEx4.FieldName = "Currency";
		gridColumnEx4.Width = 120;
		gridColumnEx4.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[4] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsWhs()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfWarehouses;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		GridColumnEx gridColumnEx5 = new GridColumnEx();
		GridColumnEx gridColumnEx6 = new GridColumnEx();
		GridColumnEx gridColumnEx7 = new GridColumnEx();
		GridColumnEx gridColumnEx8 = new GridColumnEx();
		GridColumnEx gridColumnEx9 = new GridColumnEx();
		GridColumnEx gridColumnEx10 = new GridColumnEx();
		GridColumnEx gridColumnEx11 = new GridColumnEx();
		GridColumnEx gridColumnEx12 = new GridColumnEx();
		GridColumnEx gridColumnEx13 = new GridColumnEx();
		GridColumnEx gridColumnEx14 = new GridColumnEx();
		gridColumnEx.Name = "clCode";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.OWHS_WhsCode;
		gridColumnEx.FieldName = "WhsCode";
		gridColumnEx.Width = 150;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clName";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.OWHS_WhsName;
		gridColumnEx2.FieldName = "WhsName";
		gridColumnEx2.Width = 150;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "clLocation";
		gridColumnEx3.Caption = Apzon.Commons.Properties.Resources.OWHS_Location;
		gridColumnEx3.FieldName = "Location";
		gridColumnEx3.Width = 150;
		gridColumnEx3.Visible = false;
		gridColumnEx4.Name = "clBuildingFloorRoom";
		gridColumnEx4.Caption = Apzon.Commons.Properties.Resources.OWHS_BuildingFloorRoom;
		gridColumnEx4.FieldName = "Building";
		gridColumnEx4.Width = 150;
		gridColumnEx4.Visible = false;
		gridColumnEx5.Name = "clStreet";
		gridColumnEx5.Caption = Apzon.Commons.Properties.Resources.OWHS_Street;
		gridColumnEx5.FieldName = "Street";
		gridColumnEx5.Width = 150;
		gridColumnEx5.Visible = false;
		gridColumnEx6.Name = "clBlock";
		gridColumnEx6.Caption = Apzon.Commons.Properties.Resources.OWHS_Block;
		gridColumnEx6.FieldName = "Block";
		gridColumnEx6.Width = 150;
		gridColumnEx6.Visible = false;
		gridColumnEx7.Name = "clZipCode";
		gridColumnEx7.Caption = Apzon.Commons.Properties.Resources.OWHS_ZipCode;
		gridColumnEx7.FieldName = "ZipCode";
		gridColumnEx7.Width = 150;
		gridColumnEx7.Visible = false;
		gridColumnEx8.Name = "clCity";
		gridColumnEx8.Caption = Apzon.Commons.Properties.Resources.OWHS_City;
		gridColumnEx8.FieldName = "City";
		gridColumnEx8.Width = 150;
		gridColumnEx8.Visible = false;
		gridColumnEx9.Name = "clCounty";
		gridColumnEx9.Caption = Apzon.Commons.Properties.Resources.OWHS_County;
		gridColumnEx9.FieldName = "County";
		gridColumnEx9.Width = 150;
		gridColumnEx9.Visible = false;
		gridColumnEx10.Name = "clFederalTaxID";
		gridColumnEx10.Caption = Apzon.Commons.Properties.Resources.OWHS_FederalTaxID;
		gridColumnEx10.FieldName = "FedTaxID";
		gridColumnEx10.Width = 150;
		gridColumnEx10.Visible = false;
		gridColumnEx11.Name = "clLocked";
		gridColumnEx11.Caption = Apzon.Commons.Properties.Resources.OWHS_Locked;
		gridColumnEx11.FieldName = "Locked";
		gridColumnEx11.Width = 150;
		gridColumnEx11.Visible = false;
		gridColumnEx12.Name = "clDropShip";
		gridColumnEx12.Caption = Apzon.Commons.Properties.Resources.OWHS_DropShip;
		gridColumnEx12.FieldName = "DropShip";
		gridColumnEx12.ColumnEdit = rtxt;
		gridColumnEx12.Width = 150;
		gridColumnEx12.Visible = false;
		gridColumnEx13.Name = "clNettable";
		gridColumnEx13.Caption = Apzon.Commons.Properties.Resources.OWHS_Nettable;
		gridColumnEx13.FieldName = "Nettable";
		gridColumnEx13.ColumnEdit = rtxt;
		gridColumnEx13.Width = 150;
		gridColumnEx13.Visible = true;
		gridColumnEx14.Name = "clObjTypeHistory";
		gridColumnEx14.Caption = Apzon.Commons.Properties.Resources.OWHS_ObjTypeHistory;
		gridColumnEx14.FieldName = "ObjTypeHistory";
		gridColumnEx14.Width = 150;
		gridColumnEx14.Visible = false;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[14]
		{
			gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5, gridColumnEx6, gridColumnEx7, gridColumnEx8, gridColumnEx9, gridColumnEx10,
			gridColumnEx11, gridColumnEx12, gridColumnEx13, gridColumnEx4
		};
		columns.AddRange(columns2);
	}

	private void LoadColumnsWhsDocument()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfWarehouses;
		gv_CFL.PopulateColumns();
		for (int i = 0; i < gv_CFL.Columns.Count; i++)
		{
			gv_CFL.Columns[i].Visible = false;
		}
		if (gv_CFL.Columns["WhsCode"] != null)
		{
			gv_CFL.Columns["WhsCode"].Visible = true;
			gv_CFL.Columns["WhsCode"].Caption = "Code";
			gv_CFL.Columns["WhsCode"].Width = 100;
		}
		if (gv_CFL.Columns["WhsName"] != null)
		{
			gv_CFL.Columns["WhsName"].Visible = true;
			gv_CFL.Columns["WhsName"].Caption = "Name";
			gv_CFL.Columns["WhsCode"].Width = 200;
		}
	}

	private void LoadColumnsItem()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfItem;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		GridColumnEx gridColumnEx5 = new GridColumnEx();
		GridColumnEx gridColumnEx6 = new GridColumnEx();
		GridColumnEx gridColumnEx7 = new GridColumnEx();
		GridColumnEx gridColumnEx8 = new GridColumnEx();
		GridColumnEx gridColumnEx9 = new GridColumnEx();
		GridColumnEx gridColumnEx10 = new GridColumnEx();
		GridColumnEx gridColumnEx11 = new GridColumnEx();
		GridColumnEx gridColumnEx12 = new GridColumnEx();
		GridColumnEx gridColumnEx13 = new GridColumnEx();
		GridColumnEx gridColumnEx14 = new GridColumnEx();
		GridColumnEx gridColumnEx15 = new GridColumnEx();
		GridColumnEx gridColumnEx16 = new GridColumnEx();
		GridColumnEx gridColumnEx17 = new GridColumnEx();
		GridColumnEx gridColumnEx18 = new GridColumnEx();
		GridColumnEx gridColumnEx19 = new GridColumnEx();
		GridColumnEx gridColumnEx20 = new GridColumnEx();
		GridColumnEx gridColumnEx21 = new GridColumnEx();
		GridColumnEx gridColumnEx22 = new GridColumnEx();
		GridColumnEx gridColumnEx23 = new GridColumnEx();
		GridColumnEx gridColumnEx24 = new GridColumnEx();
		GridColumnEx gridColumnEx25 = new GridColumnEx();
		GridColumnEx gridColumnEx26 = new GridColumnEx();
		GridColumnEx gridColumnEx27 = new GridColumnEx();
		GridColumnEx gridColumnEx28 = new GridColumnEx();
		GridColumnEx gridColumnEx29 = new GridColumnEx();
		GridColumnEx gridColumnEx30 = new GridColumnEx();
		GridColumnEx gridColumnEx31 = new GridColumnEx();
		GridColumnEx gridColumnEx32 = new GridColumnEx();
		GridColumnEx gridColumnEx33 = new GridColumnEx();
		GridColumnEx gridColumnEx34 = new GridColumnEx();
		GridColumnEx gridColumnEx35 = new GridColumnEx();
		GridColumnEx gridColumnEx36 = new GridColumnEx();
		GridColumnEx gridColumnEx37 = new GridColumnEx();
		GridColumnEx gridColumnEx38 = new GridColumnEx();
		GridColumnEx gridColumnEx39 = new GridColumnEx();
		GridColumnEx gridColumnEx40 = new GridColumnEx();
		GridColumnEx gridColumnEx41 = new GridColumnEx();
		GridColumnEx gridColumnEx42 = new GridColumnEx();
		GridColumnEx gridColumnEx43 = new GridColumnEx();
		GridColumnEx gridColumnEx44 = new GridColumnEx();
		GridColumnEx gridColumnEx45 = new GridColumnEx();
		GridColumnEx gridColumnEx46 = new GridColumnEx();
		GridColumnEx gridColumnEx47 = new GridColumnEx();
		GridColumnEx gridColumnEx48 = new GridColumnEx();
		GridColumnEx gridColumnEx49 = new GridColumnEx();
		GridColumnEx gridColumnEx50 = new GridColumnEx();
		GridColumnEx gridColumnEx51 = new GridColumnEx();
		GridColumnEx gridColumnEx52 = new GridColumnEx();
		GridColumnEx gridColumnEx53 = new GridColumnEx();
		GridColumnEx gridColumnEx54 = new GridColumnEx();
		GridColumnEx gridColumnEx55 = new GridColumnEx();
		GridColumnEx gridColumnEx56 = new GridColumnEx();
		new GridColumnEx();
		new GridColumnEx();
		new GridColumnEx();
		new GridColumnEx();
		gridColumnEx.Name = "clItemCode";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.OITM_ItemCode;
		gridColumnEx.FieldName = "ItemCode";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clItemName";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.OITM_ItemName;
		gridColumnEx2.FieldName = "ItemName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "clDfltWH";
		gridColumnEx3.Caption = Apzon.Commons.Properties.Resources.OWHS_WhsCode;
		gridColumnEx3.FieldName = "DefaultWarehouse";
		gridColumnEx3.Width = 120;
		gridColumnEx3.Visible = true;
		gridColumnEx4.Name = "clOnHand";
		gridColumnEx4.Caption = Apzon.Commons.Properties.Resources.OITW_gcOnHand;
		gridColumnEx4.FieldName = "QuantityOnStock";
		gridColumnEx4.Width = 120;
		gridColumnEx4.DisplayFormat.FormatType = FormatType.Numeric;
		gridColumnEx4.DisplayFormat.FormatString = "n0";
		gridColumnEx4.Visible = true;
		gridColumnEx5.Name = "InvntryUom";
		gridColumnEx5.Caption = "Invntry Uom";
		gridColumnEx5.FieldName = "InvntryUom";
		gridColumnEx5.Width = 120;
		gridColumnEx5.Visible = false;
		gridColumnEx6.Name = "CodeBars";
		gridColumnEx6.Caption = "Bar Code";
		gridColumnEx6.FieldName = "CodeBars";
		gridColumnEx6.Width = 120;
		gridColumnEx6.Visible = false;
		gridColumnEx7.Name = "FrgnName";
		gridColumnEx7.Caption = "Frgn Name";
		gridColumnEx7.FieldName = "FrgnName";
		gridColumnEx7.Width = 120;
		gridColumnEx7.Visible = false;
		gridColumnEx8.Name = "ItemType";
		gridColumnEx8.Caption = "Item Type";
		gridColumnEx8.FieldName = "ItemTypes";
		gridColumnEx8.Width = 120;
		gridColumnEx8.Visible = false;
		gridColumnEx9.Name = "ItemGroup";
		gridColumnEx9.Caption = "ItemGroup";
		gridColumnEx9.FieldName = "ItmsGrpCod";
		gridColumnEx9.Width = 120;
		gridColumnEx9.Visible = false;
		gridColumnEx10.Name = "InvntItem";
		gridColumnEx10.Caption = "InvntItem";
		gridColumnEx10.FieldName = "InvntItem";
		gridColumnEx10.Width = 120;
		gridColumnEx10.Visible = false;
		gridColumnEx11.Name = "PrchseItem";
		gridColumnEx11.Caption = "PrchseItem";
		gridColumnEx11.FieldName = "PrchseItem";
		gridColumnEx11.Width = 120;
		gridColumnEx11.Visible = false;
		gridColumnEx12.Name = "SellItem";
		gridColumnEx12.Caption = "SellItem";
		gridColumnEx12.FieldName = "SellItem";
		gridColumnEx12.Width = 120;
		gridColumnEx12.Visible = false;
		gridColumnEx13.Name = "FirmCode";
		gridColumnEx13.Caption = "FirmCode";
		gridColumnEx13.FieldName = "FirmCode";
		gridColumnEx13.Width = 120;
		gridColumnEx13.Visible = false;
		gridColumnEx14.Name = "ShipType";
		gridColumnEx14.Caption = "ShipType";
		gridColumnEx14.FieldName = "ShipType";
		gridColumnEx14.Width = 120;
		gridColumnEx14.Visible = false;
		gridColumnEx15.Name = "ManSerNum";
		gridColumnEx15.Caption = "ManSerNum";
		gridColumnEx15.FieldName = "ManSerNum";
		gridColumnEx15.Width = 120;
		gridColumnEx15.Visible = false;
		gridColumnEx16.Name = "validFor";
		gridColumnEx16.Caption = "validFor";
		gridColumnEx16.FieldName = "validFor";
		gridColumnEx16.Width = 120;
		gridColumnEx16.Visible = false;
		gridColumnEx17.Name = "CardCode";
		gridColumnEx17.Caption = "CardCode";
		gridColumnEx17.FieldName = "CardCode";
		gridColumnEx17.Width = 120;
		gridColumnEx17.Visible = false;
		gridColumnEx18.Name = "SuppCatNum";
		gridColumnEx18.Caption = "SuppCatNum";
		gridColumnEx18.FieldName = "SuppCatNum";
		gridColumnEx18.Width = 120;
		gridColumnEx18.Visible = false;
		gridColumnEx19.Name = "BuyUnitMsr";
		gridColumnEx19.Caption = "BuyUnitMsr";
		gridColumnEx19.FieldName = "BuyUnitMsr";
		gridColumnEx19.Width = 120;
		gridColumnEx19.Visible = false;
		gridColumnEx20.Name = "NumInBuy";
		gridColumnEx20.Caption = "NumInBuy";
		gridColumnEx20.FieldName = "NumInBuy";
		gridColumnEx20.Width = 120;
		gridColumnEx20.Visible = false;
		gridColumnEx21.Name = "PurPackMsr";
		gridColumnEx21.Caption = "PurPackMsr";
		gridColumnEx21.FieldName = "PurPackMsr";
		gridColumnEx21.Width = 120;
		gridColumnEx21.Visible = false;
		gridColumnEx22.Name = "PurPackUn";
		gridColumnEx22.Caption = "PurPackUn";
		gridColumnEx22.FieldName = "PurPackUn";
		gridColumnEx22.Width = 120;
		gridColumnEx22.Visible = false;
		gridColumnEx23.Name = "PurFactor1";
		gridColumnEx23.Caption = "PurFactor1";
		gridColumnEx23.FieldName = "PurFactor1";
		gridColumnEx23.Width = 120;
		gridColumnEx23.Visible = false;
		gridColumnEx24.Name = "PurFactor2";
		gridColumnEx24.Caption = "PurFactor2";
		gridColumnEx24.FieldName = "PurFactor2";
		gridColumnEx24.Width = 120;
		gridColumnEx24.Visible = false;
		gridColumnEx25.Name = "PurFactor3";
		gridColumnEx25.Caption = "PurFactor3";
		gridColumnEx25.FieldName = "PurFactor3";
		gridColumnEx25.Width = 120;
		gridColumnEx25.Visible = false;
		gridColumnEx26.Name = "PurFactor4";
		gridColumnEx26.Caption = "PurFactor4";
		gridColumnEx26.FieldName = "PurFactor4";
		gridColumnEx26.Width = 120;
		gridColumnEx26.Visible = false;
		gridColumnEx27.Name = "SalUnitMsr";
		gridColumnEx27.Caption = "SalUnitMsr";
		gridColumnEx27.FieldName = "SalUnitMsr";
		gridColumnEx27.Width = 120;
		gridColumnEx27.Visible = false;
		gridColumnEx28.Name = "NumInSale";
		gridColumnEx28.Caption = "NumInSale";
		gridColumnEx28.FieldName = "NumInSale";
		gridColumnEx28.Width = 120;
		gridColumnEx28.Visible = false;
		gridColumnEx29.Name = "SalPackMsr";
		gridColumnEx29.Caption = "SalPackMsr";
		gridColumnEx29.FieldName = "SalPackMsr";
		gridColumnEx29.Width = 120;
		gridColumnEx29.Visible = false;
		gridColumnEx30.Name = "SalPackUn";
		gridColumnEx30.Caption = "SalPackUn";
		gridColumnEx30.FieldName = "SalPackUn";
		gridColumnEx30.Width = 120;
		gridColumnEx30.Visible = false;
		gridColumnEx31.Name = "SalFactor1";
		gridColumnEx31.Caption = "SalFactor1";
		gridColumnEx31.FieldName = "SalFactor1";
		gridColumnEx31.Width = 120;
		gridColumnEx31.Visible = false;
		gridColumnEx32.Name = "SalFactor2";
		gridColumnEx32.Caption = "SalFactor2";
		gridColumnEx32.FieldName = "SalFactor2";
		gridColumnEx32.Width = 120;
		gridColumnEx32.Visible = false;
		gridColumnEx33.Name = "SalFactor3";
		gridColumnEx33.Caption = "SalFactor3";
		gridColumnEx33.FieldName = "SalFactor3";
		gridColumnEx33.Width = 120;
		gridColumnEx33.Visible = false;
		gridColumnEx34.Name = "SalFactor4";
		gridColumnEx34.Caption = "SalFactor4";
		gridColumnEx34.FieldName = "SalFactor4";
		gridColumnEx34.Width = 120;
		gridColumnEx34.Visible = false;
		gridColumnEx35.Name = "GLMethods";
		gridColumnEx35.Caption = "GLMethods";
		gridColumnEx35.FieldName = "GLMethods";
		gridColumnEx35.Width = 120;
		gridColumnEx35.Visible = false;
		gridColumnEx36.Name = "EvalSystem";
		gridColumnEx36.Caption = "EvalSystem";
		gridColumnEx36.FieldName = "EvalSystem";
		gridColumnEx36.Width = 120;
		gridColumnEx36.Visible = false;
		gridColumnEx37.Name = "AvgPrice";
		gridColumnEx37.Caption = "AvgPrice";
		gridColumnEx37.FieldName = "AvgPrice";
		gridColumnEx37.Width = 120;
		gridColumnEx37.Visible = false;
		gridColumnEx38.Name = "ByWh";
		gridColumnEx38.Caption = "ByWh";
		gridColumnEx38.FieldName = "ByWh";
		gridColumnEx38.Width = 120;
		gridColumnEx38.Visible = false;
		gridColumnEx39.Name = "ReorderQty";
		gridColumnEx39.Caption = "ReorderQty";
		gridColumnEx39.FieldName = "ReorderQty";
		gridColumnEx39.Width = 120;
		gridColumnEx39.Visible = false;
		gridColumnEx40.Name = "MinLevel";
		gridColumnEx40.Caption = "MinLevel";
		gridColumnEx40.FieldName = "MinLevel";
		gridColumnEx40.Width = 120;
		gridColumnEx40.Visible = false;
		gridColumnEx40.Name = "MaxLevel";
		gridColumnEx40.Caption = "MaxLevel";
		gridColumnEx40.FieldName = "MaxLevel";
		gridColumnEx40.Width = 120;
		gridColumnEx40.Visible = false;
		gridColumnEx41.Name = "VatGroupPu";
		gridColumnEx41.Caption = "VatGroupPu";
		gridColumnEx41.FieldName = "PurchaseVATGroup";
		gridColumnEx41.Width = 120;
		gridColumnEx41.Visible = false;
		gridColumnEx42.Name = "VatGourpSa";
		gridColumnEx42.Caption = "VatGourpSa";
		gridColumnEx42.FieldName = "SalesVATGroup";
		gridColumnEx42.Width = 120;
		gridColumnEx42.Visible = false;
		gridColumnEx43.Name = "BOMTreeType";
		gridColumnEx43.Caption = "BOMTreeType";
		gridColumnEx43.FieldName = "BOMTreeType";
		gridColumnEx43.Width = 120;
		gridColumnEx43.Visible = false;
		gridColumnEx44.Name = "OITMVatGroupPu";
		gridColumnEx44.Caption = "VatGroupPu";
		gridColumnEx44.FieldName = "VatGroupPu";
		gridColumnEx44.Width = 120;
		gridColumnEx44.Visible = false;
		gridColumnEx45.Name = "OITMVatGourpSa";
		gridColumnEx45.Caption = "VatGourpSa";
		gridColumnEx45.FieldName = "VatGourpSa";
		gridColumnEx45.Width = 120;
		gridColumnEx45.Visible = false;
		gridColumnEx46.Name = "ColVatPuInActive";
		gridColumnEx46.Caption = "VatPuInActive";
		gridColumnEx46.FieldName = "VatPuInActive";
		gridColumnEx46.Width = 120;
		gridColumnEx46.Visible = false;
		gridColumnEx47.Name = "ColVatSaInActive";
		gridColumnEx47.Caption = "VatSaInActive";
		gridColumnEx47.FieldName = "VatSaInActive";
		gridColumnEx47.Width = 120;
		gridColumnEx47.Visible = false;
		gridColumnEx48.Name = "ColValidTo";
		gridColumnEx48.Caption = "ValidTo";
		gridColumnEx48.FieldName = "ValidTo";
		gridColumnEx48.Width = 120;
		gridColumnEx48.Visible = false;
		gridColumnEx49.Name = "ColValidFrom";
		gridColumnEx49.Caption = "ValidFrom";
		gridColumnEx49.FieldName = "ValidFrom";
		gridColumnEx49.Width = 120;
		gridColumnEx49.Visible = false;
		gridColumnEx50.Name = "ColValidRemarks";
		gridColumnEx50.Caption = "ValidRemarks";
		gridColumnEx50.FieldName = "ValidRemarks";
		gridColumnEx50.Width = 120;
		gridColumnEx50.Visible = false;
		gridColumnEx51.Name = "ColFrozenTo";
		gridColumnEx51.Caption = "FrozenTo";
		gridColumnEx51.FieldName = "FrozenTo";
		gridColumnEx51.Width = 120;
		gridColumnEx51.Visible = false;
		gridColumnEx52.Name = "ColFrozenFrom";
		gridColumnEx52.Caption = "FrozenFrom";
		gridColumnEx52.FieldName = "FrozenFrom";
		gridColumnEx52.Width = 120;
		gridColumnEx52.Visible = false;
		gridColumnEx53.Name = "ColFrozenRemarks";
		gridColumnEx53.Caption = "FrozenRemarks";
		gridColumnEx53.FieldName = "FrozenRemarks";
		gridColumnEx53.Width = 120;
		gridColumnEx53.Visible = false;
		gridColumnEx54.Name = "ColMngMethod";
		gridColumnEx54.Caption = "MngMethod";
		gridColumnEx54.FieldName = "MngMethod";
		gridColumnEx54.Width = 120;
		gridColumnEx54.Visible = false;
		gridColumnEx55.Name = "ColManBtchNum";
		gridColumnEx55.Caption = "ManBtchNum";
		gridColumnEx55.FieldName = "ManBtchNum";
		gridColumnEx55.Width = 120;
		gridColumnEx55.Visible = false;
		gridColumnEx56.Name = "ColfrozenFor";
		gridColumnEx56.Caption = "frozenFor";
		gridColumnEx56.FieldName = "frozenFor";
		gridColumnEx56.Width = 120;
		gridColumnEx56.Visible = false;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[56]
		{
			gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5, gridColumnEx6, gridColumnEx7, gridColumnEx8, gridColumnEx9, gridColumnEx10,
			gridColumnEx11, gridColumnEx12, gridColumnEx13, gridColumnEx14, gridColumnEx15, gridColumnEx16, gridColumnEx17, gridColumnEx18, gridColumnEx19, gridColumnEx20,
			gridColumnEx21, gridColumnEx22, gridColumnEx23, gridColumnEx24, gridColumnEx25, gridColumnEx26, gridColumnEx27, gridColumnEx28, gridColumnEx29, gridColumnEx30,
			gridColumnEx31, gridColumnEx32, gridColumnEx33, gridColumnEx34, gridColumnEx35, gridColumnEx36, gridColumnEx37, gridColumnEx38, gridColumnEx39, gridColumnEx40,
			gridColumnEx41, gridColumnEx42, gridColumnEx43, gridColumnEx44, gridColumnEx45, gridColumnEx46, gridColumnEx47, gridColumnEx48, gridColumnEx49, gridColumnEx50,
			gridColumnEx51, gridColumnEx52, gridColumnEx53, gridColumnEx54, gridColumnEx55, gridColumnEx56
		};
		columns.AddRange(columns2);
	}

	private void LoadColumnsDistRule()
	{
		Text = "List of Distribution Rules";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "cl_OcrCode";
		gridColumnEx.Caption = "Distribution Rules";
		gridColumnEx.FieldName = "OcrCode";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "cl_OcrName";
		gridColumnEx2.Caption = "Distribution Rules Name";
		gridColumnEx2.FieldName = "OcrName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsProject()
	{
		Text = "List of Project";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "clPrjCode";
		gridColumnEx.Caption = "Project Code";
		gridColumnEx.FieldName = "PrjCode";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clPrjName";
		gridColumnEx2.Caption = "Project Name";
		gridColumnEx2.FieldName = "PrjName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsPrOrder()
	{
		Text = "List of Production Order";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		gridColumnEx.Name = "clDocEntry";
		gridColumnEx.Caption = "Doc Entry";
		gridColumnEx.FieldName = "DocEntry";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clDocNum";
		gridColumnEx2.Caption = "Doc Num";
		gridColumnEx2.FieldName = "DocNum";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "clItemCode";
		gridColumnEx3.Caption = "Item Code";
		gridColumnEx3.FieldName = "ItemCode";
		gridColumnEx3.Width = 120;
		gridColumnEx3.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[3] { gridColumnEx, gridColumnEx2, gridColumnEx3 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsNGReport()
	{
		Text = "List of NG Report";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		GridColumnEx gridColumnEx5 = new GridColumnEx();
		GridColumnEx gridColumnEx6 = new GridColumnEx();
		gridColumnEx.Name = "clDocEntry";
		gridColumnEx.Caption = "Doc Entry";
		gridColumnEx.FieldName = "DocEntry";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = false;
		gridColumnEx2.Name = "clItemCode";
		gridColumnEx2.Caption = "Item Code";
		gridColumnEx2.FieldName = "ItemCode";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "clItemName";
		gridColumnEx3.Caption = "Item Name";
		gridColumnEx3.FieldName = "ItemName";
		gridColumnEx3.Width = 120;
		gridColumnEx3.Visible = true;
		gridColumnEx4.Name = "clWhse";
		gridColumnEx4.Caption = "WareHouse";
		gridColumnEx4.FieldName = "Warehouse";
		gridColumnEx4.Width = 120;
		gridColumnEx4.Visible = true;
		gridColumnEx5.Name = "clPlannedQty";
		gridColumnEx5.Caption = "Planned Qty";
		gridColumnEx5.FieldName = "PlannedQty";
		gridColumnEx5.Width = 120;
		gridColumnEx5.Visible = true;
		gridColumnEx6.Name = "clCmpltQty";
		gridColumnEx6.Caption = "Completed Qty";
		gridColumnEx6.FieldName = "CmpltQty";
		gridColumnEx6.Width = 120;
		gridColumnEx6.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[6] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5, gridColumnEx6 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsAccount()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfAccount;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		gridColumnEx.Name = "cl_AcctCode";
		gridColumnEx.Caption = "Account Number";
		gridColumnEx.FieldName = "AcctCode";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "cl_AcctName";
		gridColumnEx2.Caption = "Account Name";
		gridColumnEx2.FieldName = "AcctName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "cl_CurrTotal";
		gridColumnEx3.Caption = "Account Balance";
		gridColumnEx3.FieldName = "Balance";
		gridColumnEx3.Width = 120;
		gridColumnEx3.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[3] { gridColumnEx, gridColumnEx2, gridColumnEx3 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsBatch()
	{
		Text = "List of Batch Number";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		GridColumnEx gridColumnEx5 = new GridColumnEx();
		gridColumnEx.Name = "clItemCode";
		gridColumnEx.Caption = "ItemCode";
		gridColumnEx.FieldName = "ItemCode";
		gridColumnEx.Width = 70;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clDistNumber";
		gridColumnEx2.Caption = "Dist Number";
		gridColumnEx2.FieldName = "DistNumber";
		gridColumnEx2.Width = 150;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "clQuantity";
		gridColumnEx3.Caption = "Quantity";
		gridColumnEx3.FieldName = "Quantity";
		gridColumnEx3.Width = 150;
		gridColumnEx3.Visible = false;
		gridColumnEx4.Name = "clLeng";
		gridColumnEx4.Caption = "Length";
		gridColumnEx4.FieldName = "U_Length";
		gridColumnEx4.Width = 150;
		gridColumnEx4.Visible = true;
		gridColumnEx5.Name = "clWidth";
		gridColumnEx5.Caption = "Width";
		gridColumnEx5.FieldName = "U_Width";
		gridColumnEx5.Width = 150;
		gridColumnEx5.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[5] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsWareHouseByItemCode()
	{
		Text = "List of WareHouse By ItemCode";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		gridColumnEx.Name = "WhsCode";
		gridColumnEx.Caption = "WhsCode";
		gridColumnEx.FieldName = "WhsCode";
		gridColumnEx.Width = 150;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "ItemCode";
		gridColumnEx2.Caption = "ItemCode";
		gridColumnEx2.FieldName = "ItemCode";
		gridColumnEx2.Width = 150;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "clQuantity";
		gridColumnEx3.Caption = "Quantity";
		gridColumnEx3.FieldName = "Quantity";
		gridColumnEx3.Width = 150;
		gridColumnEx3.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[3] { gridColumnEx, gridColumnEx2, gridColumnEx3 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsForeCast()
	{
		Text = "List of ForeCast";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "Code";
		gridColumnEx.Caption = "ForeCast Code";
		gridColumnEx.FieldName = "AbsID";
		gridColumnEx.Width = 100;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "Name";
		gridColumnEx2.Caption = "Name";
		gridColumnEx2.FieldName = "Name";
		gridColumnEx2.Width = 250;
		gridColumnEx2.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsMRP()
	{
		Text = "List of MRP";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "Code";
		gridColumnEx.Caption = "Code";
		gridColumnEx.FieldName = "Id";
		gridColumnEx.Width = 100;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "Name";
		gridColumnEx2.Caption = "Name";
		gridColumnEx2.FieldName = "MrpName";
		gridColumnEx2.Width = 250;
		gridColumnEx2.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsCashFlow()
	{
		Text = "List of MRp";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "Code";
		gridColumnEx.Caption = "Doc Entry";
		gridColumnEx.FieldName = "DocEntry";
		gridColumnEx.Width = 100;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "Name";
		gridColumnEx2.Caption = "Item Code";
		gridColumnEx2.FieldName = "ItemCode";
		gridColumnEx2.Width = 250;
		gridColumnEx2.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsSchedule()
	{
		Text = "Schedule Detail";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "cl_Code";
		gridColumnEx.Caption = "Schedule Code";
		gridColumnEx.FieldName = "Code";
		gridColumnEx.Width = 200;
		gridColumnEx.Visible = true;
		RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit
		{
			AllowMouseWheel = false
		};
		repositoryItemButtonEdit.Buttons[0].IsLeft = true;
		repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Glyph;
		repositoryItemButtonEdit.Buttons[0].Image = Apzon.Forms.Properties.Resources.right;
		repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs ev) =>
		{
			ChooseItem = gv_CFL.GetFocusedDataRow();
			Close();
		};
		gridColumnEx.ColumnEdit = repositoryItemButtonEdit;
		gridColumnEx.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		gridColumnEx2.Name = "cl_Name";
		gridColumnEx2.Caption = "Schedule Name";
		gridColumnEx2.FieldName = "Name";
		gridColumnEx2.Width = 150;
		gridColumnEx2.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
		columns.AddRange(columns2);
		btnChoose.Text = "Ok";
	}

	private void LoadColumnsProductionOrder()
	{
		Text = "List of Production Order";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		GridColumnEx gridColumnEx5 = new GridColumnEx();
		GridColumnEx gridColumnEx6 = new GridColumnEx();
		gridColumnEx.Name = "DocEntry";
		gridColumnEx.Caption = "DocEntry";
		gridColumnEx.FieldName = "DocEntry";
		gridColumnEx.Width = 55;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "DocNum";
		gridColumnEx2.Caption = "DocNum";
		gridColumnEx2.FieldName = "DocNum";
		gridColumnEx2.Width = 55;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "ItemCode";
		gridColumnEx3.Caption = "Item Code";
		gridColumnEx3.FieldName = "ItemCode";
		gridColumnEx3.Width = 85;
		gridColumnEx3.Visible = true;
		gridColumnEx4.Name = "ItemName";
		gridColumnEx4.Caption = "Item Name";
		gridColumnEx4.FieldName = "ItemName";
		gridColumnEx4.Width = 100;
		gridColumnEx4.Visible = true;
		gridColumnEx5.Name = "Type";
		gridColumnEx5.Caption = "Type";
		gridColumnEx5.FieldName = "Type";
		gridColumnEx5.Width = 75;
		gridColumnEx5.Visible = true;
		gridColumnEx6.Name = "Series";
		gridColumnEx6.Caption = "Series";
		gridColumnEx6.FieldName = "Series";
		gridColumnEx6.Width = 50;
		gridColumnEx6.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[6] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5, gridColumnEx6 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsRequester()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_ListOfRequester;
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		new GridColumnEx();
		new GridColumnEx();
		gridColumnEx.Name = "clRequester";
		gridColumnEx.Caption = Apzon.Commons.Properties.Resources.OPRQ_Requester;
		gridColumnEx.FieldName = "Requester";
		gridColumnEx.Width = 100;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clReqName";
		gridColumnEx2.Caption = Apzon.Commons.Properties.Resources.OPRQ_ReqName;
		gridColumnEx2.FieldName = "ReqName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataRequester()
	{
		try
		{
			using HttpClientHelper<RequesterImpl> helper = new HttpClientHelper<RequesterImpl>(Configuration.ApiServer, "Requester");
			List<RequesterImpl> dataSource = await helper.GetListAsync("Cfl");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsAcctCode()
	{
		Text = "List of AcctCode";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		new GridColumnEx();
		gridColumnEx.Name = "clAcctCode";
		gridColumnEx.Caption = "Account Number";
		gridColumnEx.FieldName = "AcctCode";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clAcctName";
		gridColumnEx2.Caption = "Account Name";
		gridColumnEx2.FieldName = "AcctName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "clBalnce";
		gridColumnEx3.Caption = "Account Balance";
		gridColumnEx3.FieldName = "CurrTotal";
		gridColumnEx3.Width = 120;
		gridColumnEx3.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[3] { gridColumnEx, gridColumnEx2, gridColumnEx3 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataAcctCode()
	{
		try
		{
			string accountType = _AccountType;
			using HttpClientHelper<AcctCodeImpl> helper = new HttpClientHelper<AcctCodeImpl>(Configuration.ApiServer, "AcctCode");
			List<AcctCodeImpl> dataSource = await helper.GetListAsync("Cfl", new { accountType });
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnsRevenueAccount()
	{
		Text = "List Of Revenue Account";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		gridColumnEx.Name = "clAcctCode";
		gridColumnEx.Caption = "Account Code";
		gridColumnEx.FieldName = "AcctCode";
		gridColumnEx.Width = 120;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clAcctName";
		gridColumnEx2.Caption = "Account Name";
		gridColumnEx2.FieldName = "AcctName";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gridColumnEx3.Name = "clBalnce";
		gridColumnEx3.Caption = "Account Balance";
		gridColumnEx3.FieldName = "CurrTotal";
		gridColumnEx3.Width = 120;
		gridColumnEx3.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[3] { gridColumnEx, gridColumnEx2, gridColumnEx3 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsCountry()
	{
		Text = "List Of Country";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "Code";
		gridColumnEx.Caption = "Code";
		gridColumnEx.FieldName = "Code";
		gridColumnEx.Width = 50;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "Name";
		gridColumnEx2.Caption = "Name";
		gridColumnEx2.FieldName = "Name";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
		columns.AddRange(columns2);
	}

	private void LoadColumnsState()
	{
		Text = "List Of State";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "Code";
		gridColumnEx.Caption = "Code";
		gridColumnEx.FieldName = "Code";
		gridColumnEx.Width = 50;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "Name";
		gridColumnEx2.Caption = "Name";
		gridColumnEx2.FieldName = "Name";
		gridColumnEx2.Width = 120;
		gridColumnEx2.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
		columns.AddRange(columns2);
	}

	private async Task LoadDataRevenueAccount()
	{
		try
		{
			using HttpClientHelper<AcctCodeImpl> helper = new HttpClientHelper<AcctCodeImpl>(Configuration.ApiServer, "Commons");
			List<AcctCodeImpl> dataSource = await helper.GetListAsync("GetRevenueAccount");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnSerial()
	{
		try
		{
			Text = "List of Serial - Item";
			GridColumnEx gridColumnEx = new GridColumnEx();
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			GridColumnEx gridColumnEx3 = new GridColumnEx();
			GridColumnEx gridColumnEx4 = new GridColumnEx();
			gridColumnEx.Name = "ItemCode";
			gridColumnEx.Caption = "Item Code";
			gridColumnEx.FieldName = "ItemCode";
			gridColumnEx.Width = 120;
			gridColumnEx.Visible = true;
			gridColumnEx2.Name = "DistNumber";
			gridColumnEx2.Caption = "Serial Number";
			gridColumnEx2.FieldName = "DistNumber";
			gridColumnEx2.Width = 120;
			gridColumnEx2.Visible = true;
			gridColumnEx3.Name = "MnfSerial";
			gridColumnEx3.Caption = "Mfr Serial No.";
			gridColumnEx3.FieldName = "MnfSerial";
			gridColumnEx3.Width = 120;
			gridColumnEx3.Visible = true;
			gridColumnEx4.Name = "LotNumber";
			gridColumnEx4.Caption = "Lot Number";
			gridColumnEx4.FieldName = "LotNumber";
			gridColumnEx4.Width = 120;
			gridColumnEx4.Visible = true;
			gv_CFL.Columns.Clear();
			GridColumnCollection columns = gv_CFL.Columns;
			GridColumn[] columns2 = new GridColumnEx[4] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4 };
			columns.AddRange(columns2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnRevaluation()
	{
		try
		{
			Text = "Inventory Revaluation";
			GridColumnEx gridColumnEx = new GridColumnEx();
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			GridColumnEx gridColumnEx3 = new GridColumnEx();
			GridColumnEx gridColumnEx4 = new GridColumnEx();
			gridColumnEx.Name = "DocNum";
			gridColumnEx.Caption = "Doc Number";
			gridColumnEx.FieldName = "DocNum";
			gridColumnEx.Width = 120;
			gridColumnEx.Visible = true;
			gridColumnEx2.Name = "DocDate";
			gridColumnEx2.Caption = "Posting Date";
			gridColumnEx2.FieldName = "DocDate";
			gridColumnEx2.Width = 120;
			gridColumnEx2.Visible = true;
			gridColumnEx3.Name = "TaxDate";
			gridColumnEx3.Caption = "Document Date";
			gridColumnEx3.FieldName = "TaxDate";
			gridColumnEx3.Width = 120;
			gridColumnEx3.Visible = true;
			gridColumnEx4.Name = "Comments";
			gridColumnEx4.Caption = "Remarks";
			gridColumnEx4.FieldName = "Comments";
			gridColumnEx4.Width = 120;
			gridColumnEx4.Visible = true;
			gv_CFL.Columns.Clear();
			GridColumnCollection columns = gv_CFL.Columns;
			GridColumn[] columns2 = new GridColumnEx[4] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4 };
			columns.AddRange(columns2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataRevaluation()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "MaterialRevaluation");
			DataTable dataSource = await helper.GetAsync("GetAll");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnPayment(int type)
	{
		try
		{
			Text = "List of " + ((type == 1) ? "Outgoing" : "Incoming") + " Payment";
			GridColumnEx gridColumnEx = new GridColumnEx();
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			GridColumnEx gridColumnEx3 = new GridColumnEx();
			GridColumnEx gridColumnEx4 = new GridColumnEx();
			GridColumnEx gridColumnEx5 = new GridColumnEx();
			gridColumnEx.Name = "DocNum";
			gridColumnEx.Caption = "Doc Number";
			gridColumnEx.FieldName = "DocNum";
			gridColumnEx.Width = 120;
			gridColumnEx.Visible = true;
			gridColumnEx2.Name = "DocDate";
			gridColumnEx2.Caption = "Posting Date";
			gridColumnEx2.FieldName = "DocDate";
			gridColumnEx2.Width = 120;
			gridColumnEx2.Visible = true;
			gridColumnEx3.Name = "Total";
			gridColumnEx3.Caption = "Document Total";
			gridColumnEx3.FieldName = "Total";
			gridColumnEx3.Width = 120;
			gridColumnEx3.Visible = true;
			gridColumnEx4.Name = "DocCurr";
			gridColumnEx4.Caption = "Currency";
			gridColumnEx4.FieldName = "DocCurr";
			gridColumnEx4.Width = 120;
			gridColumnEx4.Visible = true;
			gridColumnEx5.Name = "Comments";
			gridColumnEx5.Caption = "Remarks";
			gridColumnEx5.FieldName = "Comments";
			gridColumnEx5.Width = 120;
			gridColumnEx5.Visible = true;
			gv_CFL.Columns.Clear();
			GridColumnCollection columns = gv_CFL.Columns;
			GridColumn[] columns2 = new GridColumnEx[5] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5 };
			columns.AddRange(columns2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataPayment(int type)
	{
		try
		{
			string controler = ((type == 1) ? "OutgoingPayment" : "IncomingPayment");
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, controler);
			DataTable dataSource = await helper.GetAsync("GetAll");
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnTracking()
	{
		try
		{
			Text = "List of Tracking Issue";
			GridColumnEx gridColumnEx = new GridColumnEx();
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			GridColumnEx gridColumnEx3 = new GridColumnEx();
			GridColumnEx gridColumnEx4 = new GridColumnEx();
			GridColumnEx gridColumnEx5 = new GridColumnEx();
			gridColumnEx.Name = "Id";
			gridColumnEx.Caption = "Id";
			gridColumnEx.FieldName = "Id";
			gridColumnEx.Width = 120;
			gridColumnEx.Visible = true;
			gridColumnEx2.Name = "IssueDate";
			gridColumnEx2.Caption = "Issue Date";
			gridColumnEx2.FieldName = "IssueDate";
			gridColumnEx2.Width = 120;
			gridColumnEx2.Visible = true;
			gridColumnEx3.Name = "BpCode";
			gridColumnEx3.Caption = "Bp Code";
			gridColumnEx3.FieldName = "BpCode";
			gridColumnEx3.Width = 120;
			gridColumnEx3.Visible = true;
			gridColumnEx4.Name = "Note";
			gridColumnEx4.Caption = "Note";
			gridColumnEx4.FieldName = "Note";
			gridColumnEx4.Width = 120;
			gridColumnEx4.Visible = true;
			gridColumnEx5.Name = "Summary";
			gridColumnEx5.Caption = "Summary";
			gridColumnEx5.FieldName = "Summary";
			gridColumnEx5.Width = 120;
			gridColumnEx5.Visible = true;
			gv_CFL.Columns.Clear();
			GridColumnCollection columns = gv_CFL.Columns;
			GridColumn[] columns2 = new GridColumnEx[5] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5 };
			columns.AddRange(columns2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataTracking()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TrackingIssue");
			DataTable dataSource = await helper.GetAsync("Search", new
			{
				bpCode = _vatType,
				projectCode = "",
				projectOwner = 0,
				reportBy = "",
				handleBy = "",
				id = 0,
				status = "",
				fromDate = "",
				toDate = "",
				userName = ""
			});
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadColumnBlanket()
	{
		try
		{
			Text = "List of Blanket Agreement";
			GridColumnEx gridColumnEx = new GridColumnEx();
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			GridColumnEx gridColumnEx3 = new GridColumnEx();
			GridColumnEx gridColumnEx4 = new GridColumnEx();
			GridColumnEx gridColumnEx5 = new GridColumnEx();
			GridColumnEx gridColumnEx6 = new GridColumnEx();
			gridColumnEx.Name = "Number";
			gridColumnEx.Caption = "Agreement No.";
			gridColumnEx.FieldName = "Number";
			gridColumnEx.Width = 120;
			gridColumnEx.Visible = true;
			gridColumnEx2.Name = "Descript";
			gridColumnEx2.Caption = "Agreement Description";
			gridColumnEx2.FieldName = "Descript";
			gridColumnEx2.Width = 120;
			gridColumnEx2.Visible = true;
			gridColumnEx3.Name = "BpCode";
			gridColumnEx3.Caption = "Bp Code";
			gridColumnEx3.FieldName = "BpCode";
			gridColumnEx3.Width = 120;
			gridColumnEx3.Visible = true;
			gridColumnEx4.Name = "Method";
			gridColumnEx4.Caption = "Agreement Method";
			gridColumnEx4.FieldName = "Method";
			gridColumnEx4.Width = 120;
			gridColumnEx4.Visible = true;
			gridColumnEx5.Name = "StartDate";
			gridColumnEx5.Caption = "Start Date";
			gridColumnEx5.FieldName = "StartDate";
			gridColumnEx5.Width = 120;
			gridColumnEx5.Visible = true;
			gridColumnEx6.Name = "EndDate";
			gridColumnEx6.Caption = "End Date";
			gridColumnEx6.FieldName = "EndDate";
			gridColumnEx6.Width = 120;
			gridColumnEx6.Visible = true;
			gv_CFL.Columns.Clear();
			GridColumnCollection columns = gv_CFL.Columns;
			GridColumn[] columns2 = new GridColumnEx[6] { gridColumnEx, gridColumnEx2, gridColumnEx3, gridColumnEx4, gridColumnEx5, gridColumnEx6 };
			columns.AddRange(columns2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataBlanket()
	{
		try
		{
			DocumentSBAImpl documentSBAImpl = new DocumentSBAImpl();
			documentSBAImpl.BpCode = _vatType;
			DataTable dataSource = await _httpClient.BlanketAgreement.GetDoc(documentSBAImpl);
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadEmpJobVacancy()
	{
		try
		{
			Text = "Interview team";
			GridColumnEx gridColumnEx = new GridColumnEx();
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			gridColumnEx.Name = "cl_EmpId";
			gridColumnEx.Caption = "Emp Id";
			gridColumnEx.FieldName = "EmpID";
			gridColumnEx.Width = 150;
			gridColumnEx.Visible = true;
			gridColumnEx.OptionsColumn.ReadOnly = true;
			gridColumnEx2.Name = "cl_EmpName";
			gridColumnEx2.Caption = "Emp Name";
			gridColumnEx2.FieldName = "EmpName";
			gridColumnEx2.Width = 150;
			gridColumnEx2.Visible = true;
			gridColumnEx2.OptionsColumn.ReadOnly = true;
			gv_CFL.Columns.Clear();
			GridColumnCollection columns = gv_CFL.Columns;
			GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
			columns.AddRange(columns2);
			gv_CFL.OptionsBehavior.ReadOnly = false;
			grd_CFL.DataSource = DataEmpJobVacancy;
			grd_CFL.DataSourceTable.AcceptChanges();
			grd_CFL.RefreshDataSource();
			btnChoose.Text = "Ok";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadApplicantNo()
	{
		try
		{
			Text = "Applicants";
			GridColumnEx gridColumnEx = new GridColumnEx();
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			GridColumnEx gridColumnEx3 = new GridColumnEx();
			GridColumnEx gridColumnEx4 = new GridColumnEx();
			GridColumnEx gridColumnEx5 = new GridColumnEx();
			GridColumnEx gridColumnEx6 = new GridColumnEx();
			GridColumnEx gridColumnEx7 = new GridColumnEx();
			GridColumnEx gridColumnEx8 = new GridColumnEx();
			gridColumnEx.Name = "cl_ApplicantNo";
			gridColumnEx.Caption = "Applicant No";
			gridColumnEx.FieldName = "ApplicantNo";
			gridColumnEx.Width = 150;
			gridColumnEx.Visible = true;
			gridColumnEx.OptionsColumn.ReadOnly = true;
			gridColumnEx2.Name = "cl_ApplicantName";
			gridColumnEx2.Caption = "Applicant Name";
			gridColumnEx2.FieldName = "ApplicantName";
			gridColumnEx2.Width = 150;
			gridColumnEx2.Visible = true;
			gridColumnEx2.OptionsColumn.ReadOnly = true;
			gridColumnEx3.Name = "cl_DoB";
			gridColumnEx3.Caption = "DoB";
			gridColumnEx3.FieldName = "DoB";
			gridColumnEx3.Width = 150;
			gridColumnEx3.Visible = true;
			gridColumnEx3.OptionsColumn.ReadOnly = true;
			gridColumnEx4.Name = "cl_Gender";
			gridColumnEx4.Caption = "Gender";
			gridColumnEx4.FieldName = "Gender";
			gridColumnEx4.Width = 50;
			gridColumnEx4.Visible = true;
			gridColumnEx4.OptionsColumn.ReadOnly = true;
			gridColumnEx5.Name = "cl_Qualifications";
			gridColumnEx5.Caption = "Qualifications";
			gridColumnEx5.FieldName = "Qualifications";
			gridColumnEx5.Width = 100;
			gridColumnEx5.Visible = true;
			gridColumnEx5.OptionsColumn.ReadOnly = true;
			gridColumnEx6.Name = "cl_Address";
			gridColumnEx6.Caption = "Address";
			gridColumnEx6.FieldName = "Address";
			gridColumnEx6.Width = 150;
			gridColumnEx6.Visible = true;
			gridColumnEx6.OptionsColumn.ReadOnly = true;
			gridColumnEx7.Name = "cl_Phone";
			gridColumnEx7.Caption = "Phone";
			gridColumnEx7.FieldName = "Phone";
			gridColumnEx7.Width = 150;
			gridColumnEx7.Visible = true;
			gridColumnEx7.OptionsColumn.ReadOnly = true;
			gridColumnEx8.Name = "cl_Email";
			gridColumnEx8.Caption = "Email";
			gridColumnEx8.FieldName = "Email";
			gridColumnEx8.Width = 150;
			gridColumnEx8.Visible = true;
			gridColumnEx8.OptionsColumn.ReadOnly = true;
			gv_CFL.Columns.Clear();
			GridColumnCollection columns = gv_CFL.Columns;
			GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
			columns.AddRange(columns2);
			gv_CFL.OptionsBehavior.ReadOnly = false;
			grd_CFL.DataSource = DataApplicant;
			grd_CFL.DataSourceTable.AcceptChanges();
			grd_CFL.RefreshDataSource();
			btnChoose.Text = "Ok";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_CFL_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			if (CFL_Type != Enums.TypeCFL.JetstarFltSort)
			{
				return;
			}
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

	private void gv_CFL_MouseMove(object sender, MouseEventArgs e)
	{
		try
		{
			if (CFL_Type != Enums.TypeCFL.JetstarFltSort)
			{
				return;
			}
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

	private void grd_CFL_DragOver(object sender, DragEventArgs e)
	{
		try
		{
			if (CFL_Type == Enums.TypeCFL.JetstarFltSort && e.Data.GetDataPresent(typeof(GridHitInfo)) && e.Data.GetData(typeof(GridHitInfo)) is GridHitInfo gridHitInfo && sender is GridControlEx { MainView: GridViewEx mainView } gridControlEx)
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

	private void grd_CFL_DragDrop(object sender, DragEventArgs e)
	{
		try
		{
			if (CFL_Type != Enums.TypeCFL.JetstarFltSort || !(sender is GridControlEx gridControlEx))
			{
				return;
			}
			GridViewEx gridViewEx = gridControlEx.MainView as GridViewEx;
			GridHitInfo gridHitInfo = e.Data.GetData(typeof(GridHitInfo)) as GridHitInfo;
			if (gridViewEx != null)
			{
				GridHitInfo gridHitInfo2 = gridViewEx.CalcHitInfo(gridControlEx.PointToClient(new Point(e.X, e.Y)));
				if (gridHitInfo != null)
				{
					int rowHandle = gridHitInfo.RowHandle;
					int rowHandle2 = gridHitInfo2.RowHandle;
					MoveRow(rowHandle, rowHandle2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void MoveRow(int sourceRow, int targetRow)
	{
		try
		{
			if (sourceRow != targetRow)
			{
				DataRow dataRow = JetstarDataFltSort.Rows[sourceRow];
				DataRow dataRow2 = JetstarDataFltSort.NewRow();
				dataRow2.ItemArray = dataRow.ItemArray;
				JetstarDataFltSort.Rows.Remove(dataRow);
				JetstarDataFltSort.Rows.InsertAt(dataRow2, targetRow);
				JetstarDataFltSort.AcceptChanges();
				gv_CFL.FocusedRowHandle = targetRow;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadApplicantInOffer()
	{
		try
		{
			Text = "Applicants";
			GridColumnEx gridColumnEx = new GridColumnEx();
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			gridColumnEx.Name = "cl_ApplicantNo";
			gridColumnEx.Caption = "Applicant No";
			gridColumnEx.FieldName = "ApplicantNo";
			gridColumnEx.Width = 150;
			gridColumnEx.Visible = true;
			gridColumnEx.OptionsColumn.ReadOnly = true;
			gridColumnEx2.Name = "cl_ApplicantName";
			gridColumnEx2.Caption = "Applicant Name";
			gridColumnEx2.FieldName = "ApplicantName";
			gridColumnEx2.Width = 150;
			gridColumnEx2.Visible = true;
			gridColumnEx2.OptionsColumn.ReadOnly = true;
			gv_CFL.Columns.Clear();
			GridColumnCollection columns = gv_CFL.Columns;
			GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
			columns.AddRange(columns2);
			gv_CFL.OptionsBehavior.ReadOnly = false;
			grd_CFL.DataSource = DataApplicantInOffer;
			grd_CFL.DataSourceTable.AcceptChanges();
			grd_CFL.RefreshDataSource();
			btnChoose.Text = "Ok";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetTaskInItem()
	{
		try
		{
			Text = "Tasks";
			GridColumnEx gridColumnEx = new GridColumnEx();
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			gridColumnEx.Name = "cl_TaskCode";
			gridColumnEx.Caption = "Task Code";
			gridColumnEx.FieldName = "TaskCode";
			gridColumnEx.Width = 150;
			gridColumnEx.Visible = true;
			gridColumnEx.OptionsColumn.ReadOnly = true;
			gridColumnEx2.Name = "cl_TaskName";
			gridColumnEx2.Caption = "Task Name";
			gridColumnEx2.FieldName = "TaskName";
			gridColumnEx2.Width = 150;
			gridColumnEx2.Visible = true;
			gridColumnEx2.OptionsColumn.ReadOnly = true;
			gv_CFL.Columns.Clear();
			GridColumnCollection columns = gv_CFL.Columns;
			GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
			columns.AddRange(columns2);
			gv_CFL.OptionsBehavior.ReadOnly = false;
			grd_CFL.DataSource = DataTaskInTeam;
			grd_CFL.DataSourceTable.AcceptChanges();
			grd_CFL.RefreshDataSource();
			btnChoose.Text = "Ok";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_CFL_ShowingEditor(object sender, CancelEventArgs e)
	{
		e.Cancel = true;
	}

	private async Task ProjectManagerItemDocument(int objectType, int absEntry, int lineNum)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CFL");
			gv_CFL.OptionsBehavior.Editable = true;
			gv_CFL.OptionsView.ColumnAutoWidth = false;
			GridColumnEx gridColumnEx = new GridColumnEx();
			gridColumnEx.Visible = true;
			gridColumnEx.Width = 80;
			gridColumnEx.Caption = "#";
			gridColumnEx.FieldName = "DocEntry";
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frm_ChooseFromList));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = new RepositoryItemButtonEditCFL();
			new EditorButtonImageOptions();
			repositoryItemButtonEditCFL.Buttons.RemoveAt(0);
			EditorButton editorButton = new EditorButton();
			editorButton.Kind = ButtonPredefines.Glyph;
			editorButton.IsLeft = true;
			editorButton.ImageOptions.Image = (Image)componentResourceManager.GetObject("editorButtonImageOptions1.Image");
			repositoryItemButtonEditCFL.Buttons.Add(editorButton);
			gridColumnEx.ColumnEdit = repositoryItemButtonEditCFL;
			repositoryItemButtonEditCFL.ButtonPressed += (object s, ButtonPressedEventArgs e) =>
			{
				string text = Function.ToString(gv_CFL.GetFocusedRowCellValue("TypeDocument"));
				string text2 = Function.ToString(gv_CFL.GetFocusedRowCellValue("DocNum"));
				if (!string.IsNullOrEmpty(text2))
				{
					XtraFormEx baseForm = Owner as XtraFormEx;
					switch (text)
					{
					case "1470000113":
						baseForm.DrillDown(ApzObjectTypes.oPurchaseRequest, ApzObjectTypes.oPurchaseRequest, text2);
						break;
					case "22":
						baseForm.DrillDown(ApzObjectTypes.oPurchaseOrders, ApzObjectTypes.oPurchaseOrders, text2);
						break;
					case "20":
						baseForm.DrillDown(ApzObjectTypes.oPurchaseDeliveryNotes, ApzObjectTypes.oPurchaseDeliveryNotes, text2);
						break;
					case "15":
						baseForm.DrillDown(ApzObjectTypes.oDeliveryNotes, ApzObjectTypes.oDeliveryNotes, text2);
						break;
					case "18":
						baseForm.DrillDown(ApzObjectTypes.oPurchaseInvoices, ApzObjectTypes.oPurchaseInvoices, text2);
						break;
					case "13":
						baseForm.DrillDown(ApzObjectTypes.oInvoices, ApzObjectTypes.oInvoices, text2);
						break;
					case "540000006":
						baseForm.DrillDown(ApzObjectTypes.oPurchaseQuotations, ApzObjectTypes.oPurchaseQuotations, text2);
						break;
					case "60":
						baseForm.DrillDown(ApzObjectTypes.oInventoryGenExit, ApzObjectTypes.oInventoryGenExit, text2);
						break;
					}
				}
			};
			gv_CFL.Columns.Add(gridColumnEx);
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			gridColumnEx2.Caption = "Line Num";
			gridColumnEx2.FieldName = "LineNum";
			gridColumnEx2.Visible = false;
			gridColumnEx2.OptionsColumn.AllowEdit = false;
			gridColumnEx2.OptionsColumn.ReadOnly = true;
			gv_CFL.Columns.Add(gridColumnEx2);
			GridColumnEx gridColumnEx3 = new GridColumnEx();
			gridColumnEx3.Width = 200;
			gridColumnEx3.Caption = "Item Code";
			gridColumnEx3.FieldName = "ItemCode";
			gridColumnEx3.Visible = true;
			gridColumnEx3.OptionsColumn.AllowEdit = false;
			gridColumnEx3.OptionsColumn.ReadOnly = true;
			gv_CFL.Columns.Add(gridColumnEx3);
			GridColumnEx gridColumnEx4 = new GridColumnEx();
			gridColumnEx4.Width = 200;
			gridColumnEx4.Caption = "Item Name";
			gridColumnEx4.FieldName = "Dscription";
			gridColumnEx4.Visible = true;
			gridColumnEx4.OptionsColumn.AllowEdit = false;
			gridColumnEx4.OptionsColumn.ReadOnly = true;
			gv_CFL.Columns.Add(gridColumnEx4);
			GridColumnEx gridColumnEx5 = new GridColumnEx();
			gridColumnEx5.Width = 200;
			gridColumnEx5.Caption = "Open Quantity";
			gridColumnEx5.FieldName = "OpenQty";
			gridColumnEx5.Visible = true;
			gridColumnEx5.OptionsColumn.AllowEdit = false;
			gridColumnEx5.OptionsColumn.ReadOnly = true;
			gv_CFL.Columns.Add(gridColumnEx5);
			GridColumnEx gridColumnEx6 = new GridColumnEx();
			gridColumnEx6.Width = 200;
			gridColumnEx6.Caption = "Line Total";
			gridColumnEx6.FieldName = "LineTotal";
			gridColumnEx6.Visible = true;
			gridColumnEx6.OptionsColumn.AllowEdit = false;
			gridColumnEx6.OptionsColumn.ReadOnly = true;
			gv_CFL.Columns.Add(gridColumnEx6);
			GridColumnEx gridColumnEx7 = new GridColumnEx();
			gridColumnEx7.Width = 200;
			gridColumnEx7.Caption = "BasePrice";
			gridColumnEx7.FieldName = "BasePrice";
			gridColumnEx7.Visible = true;
			gridColumnEx7.OptionsColumn.AllowEdit = false;
			gridColumnEx7.OptionsColumn.ReadOnly = true;
			gv_CFL.Columns.Add(gridColumnEx7);
			GridColumnEx gridColumnEx8 = new GridColumnEx();
			gridColumnEx8.Width = 200;
			gridColumnEx8.Caption = "LineStatus";
			gridColumnEx8.FieldName = "LineStatus";
			gridColumnEx8.Visible = true;
			gridColumnEx8.OptionsColumn.AllowEdit = false;
			gridColumnEx8.OptionsColumn.ReadOnly = true;
			gv_CFL.Columns.Add(gridColumnEx8);
			DataTable dataSource = await helper.GetAsync("GetAllDocumentByWbs", new { objectType, absEntry, lineNum });
			grd_CFL.DataSource = dataSource;
			grd_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void LoadFilterSourceValue()
	{
		Text = "Choose Filter Value";
		GridColumnEx gridColumnEx = new GridColumnEx();
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx.Name = "clCode";
		gridColumnEx.Caption = "Code";
		gridColumnEx.FieldName = "Code";
		gridColumnEx.Width = 100;
		gridColumnEx.Visible = true;
		gridColumnEx2.Name = "clName";
		gridColumnEx2.Caption = "Name";
		gridColumnEx2.FieldName = "Name";
		gridColumnEx2.Width = 200;
		gridColumnEx2.Visible = true;
		gv_CFL.Columns.Clear();
		GridColumnCollection columns = gv_CFL.Columns;
		GridColumn[] columns2 = new GridColumnEx[2] { gridColumnEx, gridColumnEx2 };
		columns.AddRange(columns2);
		if (FilterSourceValue.IsNotNull())
		{
			grd_CFL.DataSource = FilterSourceValue;
		}
		gv_CFL.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
		gv_CFL.ClearSelection();
		if (string.IsNullOrEmpty(ValueFilterSource) || !FilterSourceValue.IsNotNull())
		{
			return;
		}
		string[] lstValue = ValueFilterSource.Split(';');
		int i;
		for (i = 0; i < lstValue.Length; i++)
		{
			Type dataType = FilterSourceValue.Columns["Code"].DataType;
			if (dataType == typeof(int) || dataType == typeof(short) || dataType == typeof(int) || dataType == typeof(long))
			{
				DataRow dataRow = FilterSourceValue.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["Code"]) == Function.ParseInt(lstValue[i]));
				if (dataRow != null)
				{
					int rowHandle = gv_CFL.GetRowHandle(FilterSourceValue.Rows.IndexOf(dataRow));
					gv_CFL.SelectRow(rowHandle);
				}
			}
			else if (dataType == typeof(double) || dataType == typeof(decimal) || dataType == typeof(float))
			{
				DataRow dataRow2 = FilterSourceValue.AsEnumerable().FirstOrDefault((DataRow t) => Math.Abs(Function.ParseDouble(t["Code"]) - Function.ParseDouble(lstValue[i])) < 1E-06);
				if (dataRow2 != null)
				{
					int rowHandle2 = gv_CFL.GetRowHandle(FilterSourceValue.Rows.IndexOf(dataRow2));
					gv_CFL.SelectRow(rowHandle2);
				}
			}
			else if (dataType == typeof(DateTime))
			{
				DataRow dataRow3 = FilterSourceValue.AsEnumerable().FirstOrDefault((DataRow t) =>
				{
					DateTime? dateTime = Function.ParseDateTime(t["Code"]);
					DateTime? dateTime2 = Function.ParseDateTime(lstValue[i]);
					if (dateTime.HasValue != dateTime2.HasValue)
					{
						return false;
					}
					return !dateTime.HasValue || dateTime.GetValueOrDefault() == dateTime2.GetValueOrDefault();
				});
				if (dataRow3 != null)
				{
					int rowHandle3 = gv_CFL.GetRowHandle(FilterSourceValue.Rows.IndexOf(dataRow3));
					gv_CFL.SelectRow(rowHandle3);
				}
			}
			else
			{
				DataRow dataRow4 = FilterSourceValue.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Code"]).Equals(Function.ToString(lstValue[i])));
				if (dataRow4 != null)
				{
					int rowHandle4 = gv_CFL.GetRowHandle(FilterSourceValue.Rows.IndexOf(dataRow4));
					gv_CFL.SelectRow(rowHandle4);
				}
			}
		}
	}

	private void LoadDataBarcodeInventoryCounting()
	{
		try
		{
			Text = "List Barcode Inventory Counting record";
			GridColumnEx gridColumnEx = new GridColumnEx();
			GridColumnEx gridColumnEx2 = new GridColumnEx();
			GridColumnEx gridColumnEx3 = new GridColumnEx();
			GridColumnEx gridColumnEx4 = new GridColumnEx();
			GridColumnEx gridColumnEx5 = new GridColumnEx();
			gridColumnEx.Name = "cl_DocEntry";
			gridColumnEx.Caption = "DocEntry";
			gridColumnEx.FieldName = "DocEntry";
			gridColumnEx.Width = 120;
			gridColumnEx.Visible = true;
			gridColumnEx2.Name = "cl_CountDate";
			gridColumnEx2.Caption = "Count Date";
			gridColumnEx2.FieldName = "CountDate";
			gridColumnEx2.Width = 120;
			gridColumnEx2.Visible = true;
			gridColumnEx3.Name = "cl_WhsCode";
			gridColumnEx3.Caption = "WhsCode";
			gridColumnEx3.FieldName = "WhsCode";
			gridColumnEx3.Width = 120;
			gridColumnEx3.Visible = true;
			gridColumnEx4.Name = "cl_BinCode";
			gridColumnEx4.Caption = "Bin Location";
			gridColumnEx4.FieldName = "BinCode";
			gridColumnEx4.Width = 120;
			gridColumnEx4.Visible = true;
			gridColumnEx5.Name = "cl_Remark";
			gridColumnEx5.Caption = "Remark";
			gridColumnEx5.FieldName = "Remark";
			gridColumnEx5.Width = 120;
			gridColumnEx5.Visible = true;
			gv_CFL.Columns.Clear();
			GridColumnCollection columns = gv_CFL.Columns;
			GridColumn[] columns2 = new GridColumnEx[3] { gridColumnEx, gridColumnEx2, gridColumnEx3 };
			columns.AddRange(columns2);
			grd_CFL.DataSource = DataBarcodeCounting;
			grd_CFL.RefreshDataSource();
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.CFL.frm_ChooseFromList));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.grd_CFL = new Apzon.Controls.Grid.GridControlEx();
		this.gv_CFL = new Apzon.Controls.Grid.GridViewEx();
		this.repositoryItemLookUpEdit_Type = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rtxt = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.txt_Search = new Apzon.Controls.Editor.ButtonEditEx();
		this.lb_Search = new Apzon.Controls.Controls.LabelLine();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnChoose = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_CFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_CFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rtxt).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Search.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_CFL.AllowPasteExcel = false;
		this.grd_CFL.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_CFL.ClipboardData = null;
		this.grd_CFL.Location = new System.Drawing.Point(0, 32);
		this.grd_CFL.MainView = this.gv_CFL;
		this.grd_CFL.Name = "grd_CFL";
		this.grd_CFL.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemLookUpEdit_Type, this.rtxt });
		this.grd_CFL.Size = new System.Drawing.Size(458, 305);
		this.grd_CFL.TabIndex = 1;
		this.grd_CFL.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_CFL });
		this.grd_CFL.DragDrop += new System.Windows.Forms.DragEventHandler(this.grd_CFL_DragDrop);
		this.grd_CFL.DragOver += new System.Windows.Forms.DragEventHandler(this.grd_CFL_DragOver);
		this.gv_CFL.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.gv_CFL.Appearance.SelectedRow.Options.UseBackColor = true;
		this.gv_CFL.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_CFL.ColumnKey = null;
		this.gv_CFL.DisableFormSetting = false;
		this.gv_CFL.DisplayException = false;
		this.gv_CFL.FieldCurrency = null;
		this.gv_CFL.FieldRate = null;
		this.gv_CFL.FormId = "frm_ChooseFromList";
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
		this.gv_CFL.OptionsBehavior.ReadOnly = true;
		this.gv_CFL.OptionsDetail.EnableMasterViewMode = false;
		this.gv_CFL.OptionsView.ColumnAutoWidth = false;
		this.gv_CFL.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.gv_CFL.UseBatchMod = false;
		this.gv_CFL.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_CFL_ShowingEditor);
		this.gv_CFL.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gv_CFL_KeyDown);
		this.gv_CFL.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gv_CFL_MouseDown);
		this.gv_CFL.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gv_CFL_MouseMove);
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
		this.txt_Search.Location = new System.Drawing.Point(94, 4);
		this.txt_Search.Name = "txt_Search";
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.txt_Search.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_Search.SettingInVisible = false;
		this.txt_Search.SettingReadonly = false;
		this.txt_Search.Size = new System.Drawing.Size(311, 24);
		this.txt_Search.TabIndex = 0;
		this.txt_Search.ToolTip = "Search by Item code";
		this.txt_Search.EditValueChanged += new System.EventHandler(this.txt_Search_EditValueChanged);
		this.lb_Search.EditControl = null;
		this.lb_Search.EditValue = "Search";
		this.lb_Search.FieldName = null;
		this.lb_Search.HideWhenEmpty = false;
		this.lb_Search.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Search.Location = new System.Drawing.Point(19, 9);
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
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(94, 341);
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
		this.btnChoose.Location = new System.Drawing.Point(13, 341);
		this.btnChoose.Name = "btnChoose";
		this.btnChoose.ReadOnly = false;
		this.btnChoose.SettingInVisible = false;
		this.btnChoose.SettingReadonly = false;
		this.btnChoose.Size = new System.Drawing.Size(75, 23);
		this.btnChoose.TabIndex = 2;
		this.btnChoose.Text = "Choose";
		this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(458, 368);
		base.Controls.Add(this.grd_CFL);
		base.Controls.Add(this.txt_Search);
		base.Controls.Add(this.lb_Search);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnChoose);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "frm_ChooseFromList";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ChooseFromList";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Choose From List";
		base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frm_ChooseFromList_FormClosed);
		((System.ComponentModel.ISupportInitialize)this.grd_CFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_CFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rtxt).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Search.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
