using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Commons;

[ValidateLoginSapExceptionHandling]
public class CFLController : BaseApiController
{
	[HttpGet]
	public DataSet GetProductionOrder(string type)
	{
		try
		{
			return UnitOfWork.CFL.GetProductionOrder(type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet LoadBusinessPartner(string cardType, string userName, string branch, string bpType = "")
	{
		try
		{
			return UnitOfWork.CFL.LoadBusinessPartner(cardType, userName, branch, bpType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetListItem(string itemCode, string itemType, string userName, string branch, string pr, string listField = "")
	{
		try
		{
			return UnitOfWork.CFL.GetListItem(itemCode, itemType, userName, branch, pr, listField);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetListItemFilter(string itemCode, string itemType, string userName, string branch, string pr)
	{
		try
		{
			return UnitOfWork.CFL.GetListItemFilter(itemCode, itemType, userName, branch, pr);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpPost]
	public DataSet GetDataByStore([FromBody] dynamic storeDate)
	{
		try
		{
			string storeName = storeDate.storeName;
			string value = storeDate.value;
			string userName = storeDate.userName;
			string branch = storeDate.branch;
			return UnitOfWork.CFL.GetDataByStore(storeName, value, userName, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataTable GetListEmployees()
	{
		try
		{
			return UnitOfWork.CFL.GetListEmployees();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataSet GetResource(string rescode, string userName, string branch)
	{
		try
		{
			return UnitOfWork.CFL.GetResource(rescode, userName, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetProject(string prjCode, string userName, string branch)
	{
		try
		{
			return UnitOfWork.CFL.GetProject(prjCode, userName, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetListWareHouse(string wareHouseCode, string wareHouseName, string branch)
	{
		try
		{
			return UnitOfWork.CFL.GetListWareHouse(wareHouseCode, wareHouseName, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetListOfInstallBase(string customer, string itemCode, string userName, string branch)
	{
		try
		{
			return UnitOfWork.CFL.GetListOfInstallBase(customer, itemCode, userName, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetListMasterResource(string resType)
	{
		try
		{
			return UnitOfWork.CFL.GetListMasterResource(resType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetListCostCenter()
	{
		try
		{
			return UnitOfWork.CFL.GetListCostCenter();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataTable GetAllOcrByDistRule()
	{
		try
		{
			return UnitOfWork.CFL.GetAllOcrByDistRule();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataSet GetListDefinedShift(int branch)
	{
		return UnitOfWork.CFL.GetListDefinedShift(branch);
	}

	[HttpGet]
	public DataSet GetListAdvanceForecast()
	{
		return UnitOfWork.CFL.GetListAdvanceForecast();
	}

	[HttpGet]
	public DataSet GetListMrp(string mrpCode, string mrpStatus, string forecast, string mrpType)
	{
		return UnitOfWork.CFL.GetListMrp(mrpCode, mrpStatus, forecast, mrpType);
	}

	[HttpGet]
	public DataSet GetListSchedule(string scheduleCode, string mrpCode, string status, string mrpType, string scheduleType, string isGenCapacity)
	{
		return UnitOfWork.CFL.GetListSchedule(scheduleCode, mrpCode, status, mrpType, scheduleType, isGenCapacity);
	}

	[HttpGet]
	public DataTable GetListItemsMRPItemCode(int BPLId)
	{
		return UnitOfWork.CFL.GetListItemsMRPItemCode(BPLId);
	}

	[HttpGet]
	public DataSet GetListMRPUsers()
	{
		return UnitOfWork.CFL.GetListMRPUsers();
	}

	[HttpGet]
	public DataSet GetListDataLines(int branch)
	{
		return UnitOfWork.CFL.GetListDataLines(branch);
	}

	[HttpGet]
	public DataTable LoadDataAdvaceForecast(string LstForecast)
	{
		return UnitOfWork.CFL.LoadDataAdvaceForecast(LstForecast);
	}

	[HttpGet]
	public DataTable LoadDataMrpSalesOrder(string LstMrpSalesOrder)
	{
		return UnitOfWork.CFL.LoadDataMrpSalesOrder(LstMrpSalesOrder);
	}

	[HttpGet]
	public DataSet GetListBomItem(string code)
	{
		return UnitOfWork.CFL.GetListBomItem(code);
	}

	[HttpGet]
	public DataSet GetBatchSerialBinNumbers(string itemCode, string whsCode, string objtype)
	{
		return UnitOfWork.CFL.GetBatchSerialBinNumbers(itemCode, whsCode, objtype);
	}

	[HttpGet]
	public DataSet GetBatchSerialNumbers(string itemCode, string whsCode, string objtype, string bpCode)
	{
		return UnitOfWork.CFL.GetBatchSerialNumbers(itemCode, whsCode, objtype, bpCode);
	}

	[HttpGet]
	public DataSet GetBatchSerialForReturnComponents(int baseEntry, int baseLine, string whsCode)
	{
		return UnitOfWork.CFL.GetBatchSerialForReturnComponents(baseEntry, baseLine, whsCode);
	}

	[HttpPost]
	public DataTable GetInfoColum([FromBody] Tuple<DataTable, string> model)
	{
		return UnitOfWork.CFL.GetInfoColum(model.Item1, model.Item2);
	}

	[HttpGet]
	public DataSet GetListDBController()
	{
		return UnitOfWork.CFL.GetDashBoardController();
	}

	[HttpPost]
	public DataSet GetDataByQuery([FromBody] string query)
	{
		try
		{
			return UnitOfWork.CFL.GetDataByQuery(query);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetDataByProcedure(string storeName)
	{
		try
		{
			return UnitOfWork.CFL.GetDataByProcedure(storeName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetDataBanks()
	{
		return UnitOfWork.CFL.GetDataBanks();
	}

	[HttpGet]
	public DataSet GetDataSalesOder()
	{
		return UnitOfWork.CFL.GetDataSalesOder();
	}

	[HttpGet]
	public DataSet GetDataFactoringIndicator()
	{
		return UnitOfWork.CFL.GetDataFactoringIndicator();
	}

	[HttpGet]
	public DataSet GetDataEmployeesTechnician()
	{
		return UnitOfWork.CFL.GetDataEmployeesTechnician();
	}

	[HttpGet]
	public DataSet GetDataTerritory()
	{
		return UnitOfWork.CFL.GetDataTerritory();
	}

	[HttpGet]
	public DataSet GetDataListUoMByUoMGroup(int entry)
	{
		return UnitOfWork.CFL.GetDataListUoMByUoMGroup(entry);
	}

	[HttpGet]
	public DataSet GetDataListUoMNameItemMaster()
	{
		return UnitOfWork.CFL.GetDataListUoMNameItemMaster();
	}

	[HttpGet]
	public DataSet GetDataListUoMs()
	{
		return UnitOfWork.CFL.GetDataListUoMs();
	}

	[HttpGet]
	public DataSet GetDataListAlterUoMs(string lstUomEntry)
	{
		return UnitOfWork.CFL.GetDataListAlterUoMs(lstUomEntry);
	}

	[HttpGet]
	public DataSet GetDataTransferRequestCopyFrom(string fromWhsCode, string toWhsCode)
	{
		return UnitOfWork.CFL.GetDataTransferRequestCopyFrom(fromWhsCode, toWhsCode);
	}

	[HttpGet]
	public DataSet GetDataListUoMForUoMPrices(string itemCode, int priceList, int ugpEntry)
	{
		return UnitOfWork.CFL.GetDataListUoMForUoMPrices(itemCode, priceList, ugpEntry);
	}

	[HttpGet]
	public DataSet GetDataChartOfAccount()
	{
		return UnitOfWork.CFL.GetDataChartOfAccount();
	}

	[HttpGet]
	public DataSet GetDataUomCode(int uomGroup)
	{
		return UnitOfWork.CFL.GetDataUomCode(uomGroup);
	}

	[HttpGet]
	public DataSet ListOfBinLocationByWareHouse(string whsCode)
	{
		return UnitOfWork.CFL.ListOfBinLocationByWareHouse(whsCode);
	}

	[HttpGet]
	public DataSet GetListSalesPerson()
	{
		return UnitOfWork.CFL.GetListSalesPerson();
	}

	[HttpGet]
	public DataSet GetListOfCostCenterByDimCode(int dimCode)
	{
		return UnitOfWork.CFL.GetListOfCostCenterByDimCode(dimCode);
	}

	[HttpGet]
	public DataSet GetEmployeeListForSalesEmpSetup()
	{
		return UnitOfWork.CFL.GetEmployeeListForSalesEmpSetup();
	}

	[HttpPost]
	public DataSet GetListInventoryCyles(DataTable search)
	{
		return UnitOfWork.CFL.GetListInventoryCyles(search);
	}

	[HttpGet]
	public DataSet GetListInventoryCounting(string countDate, int countTime, string status, int series, string ref2, int docNum, string wareHouse)
	{
		return UnitOfWork.CFL.GetListInventoryCounting(countDate, countTime, status, series, ref2, docNum, wareHouse);
	}

	[HttpGet]
	public DataSet GetDataBpCatalogItem(string bpCode, string subCatNum, string userName, string docdate, string branch, string itemCode)
	{
		return UnitOfWork.CFL.GetDataBpCatalogItem(bpCode, subCatNum, userName, docdate, branch, itemCode);
	}

	[HttpGet]
	public DataSet GetDataBOMCFLOfProduct()
	{
		return UnitOfWork.CFL.GetDataBOMCFLOfProduct();
	}

	[HttpGet]
	public DataSet GetDataListBOMActive()
	{
		return UnitOfWork.CFL.GetDataListBOMActive();
	}

	[HttpGet]
	public DataSet GetQualificationList()
	{
		return UnitOfWork.CFL.GetQualificationList();
	}

	[HttpGet]
	public DataSet GetApplicantList()
	{
		return UnitOfWork.CFL.GetApplicantList();
	}

	[HttpGet]
	public DataSet GetListQuestion()
	{
		return UnitOfWork.CFL.GetListQuestion();
	}

	[HttpGet]
	public DataSet GetListAtp(string type, string status, string outputType)
	{
		return UnitOfWork.CFL.GetListAtp(type, status, outputType);
	}

	[HttpGet]
	public DataSet FindWorkPeriod(string status, string code, string name)
	{
		return UnitOfWork.CFL.FindWorkPeriod(status, code, name);
	}

	[HttpGet]
	public DataSet GetListScheduleAtp(string atpCode, string atpScheduleType, string atpScheduleStatus, string atpScheduleCode)
	{
		return UnitOfWork.CFL.GetListScheduleAtp(atpCode, atpScheduleType, atpScheduleStatus, atpScheduleCode);
	}

	[HttpGet]
	public DataTable LoadDataMaterialShortageReport(string listXcludeOrder)
	{
		return UnitOfWork.CFL.LoadDataProductionMaterialShortageReport(listXcludeOrder);
	}

	[HttpGet]
	public DataSet GetLanedCostsList()
	{
		return UnitOfWork.CFL.GetLanedCostsList();
	}

	[HttpGet]
	public DataSet LandedCostCopyFrom(string vendor)
	{
		return UnitOfWork.CFL.LandedCostCopyFrom(vendor);
	}

	[HttpGet]
	public DataSet LandedCostGodInvoice(string vendor, int objectType)
	{
		return UnitOfWork.CFL.LandedCostGodInvoice(vendor, objectType);
	}

	[HttpGet]
	public DataSet GetTableNameORW(string objtype)
	{
		return UnitOfWork.CFL.GetTableNameORW(objtype);
	}

	[HttpGet]
	public DataSet GetSalesOppPartner()
	{
		return UnitOfWork.CFL.GetSalesOppPartner();
	}

	[HttpGet]
	public DataSet GetEmployees(int branch, string value)
	{
		return UnitOfWork.CFL.GetEmployees(branch, value);
	}

	[HttpGet]
	public DataSet GetEmployeeContract(int empID)
	{
		return UnitOfWork.CFL.GetEmployeeContract(empID);
	}

	[HttpGet]
	public DataSet GetUserList()
	{
		return UnitOfWork.CFL.GetUserList();
	}

	[HttpGet]
	public DataSet GetSolution()
	{
		return UnitOfWork.CFL.GetSolution();
	}

	[HttpGet]
	public DataSet GetDataBPControlAccounts(string cardType, string acctType)
	{
		return UnitOfWork.CFL.GetDataBPControlAccounts(cardType, acctType);
	}

	[HttpGet]
	public DataSet UserDefinedTexts()
	{
		return UnitOfWork.CFL.UserDefinedTexts();
	}

	[HttpGet]
	public DataSet GetDataBOMForProduction()
	{
		return UnitOfWork.CFL.GetDataBOMForProduction();
	}

	[HttpGet]
	public DataSet GetItemWarrantyTemplates(string type)
	{
		return UnitOfWork.CFL.GetItemWarrantyTemplates(type);
	}

	[HttpGet]
	public string ValidateDistrRule(string value)
	{
		return UnitOfWork.CFL.ValidateDistrRule(value);
	}

	[HttpGet]
	public DataSet GetDataProjectTask(int projectId)
	{
		return UnitOfWork.CFL.GetDataProjectTask(projectId);
	}

	[HttpGet]
	public DataSet GetDataProjectManager(int projectid)
	{
		return UnitOfWork.CFL.GetDataProjectManager(projectid);
	}

	[HttpGet]
	public DataSet GetItemBySaleOrder(int orderId)
	{
		return UnitOfWork.CFL.GetItemBySaleOrder(orderId);
	}

	[HttpGet]
	public DataSet GetBOMTemplateBySaleOrder()
	{
		return UnitOfWork.CFL.GetBOMTemplateBySaleOrder();
	}

	[HttpGet]
	public DataSet GetDataTrackingIssue()
	{
		return UnitOfWork.CFL.GetDataTrackingIssue();
	}

	[HttpGet]
	public DataSet GetDataProjectReportCompletion(int projectId)
	{
		return UnitOfWork.CFL.GetDataProjectReportCompletion(projectId);
	}

	[HttpGet]
	public DataSet GetDataWBSInTabItem(int projectId)
	{
		return UnitOfWork.CFL.GetDataWBSInTabItem(projectId);
	}

	[HttpGet]
	public DataSet GetDataItemInTabItem(int projectId, string wbs)
	{
		return UnitOfWork.CFL.GetDataItemInTabItem(projectId, wbs);
	}

	[HttpGet]
	public DataTable GetAllDocumentByWbs(int objectType, int absEntry, int lineNum)
	{
		return UnitOfWork.CFL.GetAllDocumentByWbs(objectType, absEntry, lineNum);
	}

	[HttpGet]
	public DataSet UserDefinedReport()
	{
		return UnitOfWork.CFL.UserDefinedReport();
	}

	[HttpGet]
	public DataSet GetDataDeliveryTypes()
	{
		return UnitOfWork.CFL.GetDataDeliveryTypes();
	}

	[HttpGet]
	public DataSet GetDataLanguages()
	{
		return UnitOfWork.CFL.GetDataLanguages();
	}

	[HttpGet]
	public DataSet GetBinOfItemGroupsWarehouse(string whsCode)
	{
		return UnitOfWork.CFL.GetBinOfItemGroupsWarehouse(whsCode);
	}

	[HttpGet]
	public DataSet GetDataDefaultForUser()
	{
		return UnitOfWork.CFL.GetDataDefaultForUser();
	}

	[HttpGet]
	public DataSet GetBatchSerial(string itemCode, string whsCode)
	{
		return UnitOfWork.CFL.GetBatchSerial(itemCode, whsCode);
	}

	[HttpGet]
	public DataSet GetDataInventoryCountingRequest()
	{
		return UnitOfWork.CFL.GetDataInventoryCountingRequest();
	}

	[HttpGet]
	public DataSet GetDataBin(string whsCode, string itemCode, int tranType = 0)
	{
		return UnitOfWork.CFL.GetDataBin(whsCode, itemCode, tranType);
	}

	[HttpGet]
	public DataTable LoadDataLstSchedule(string LstSchedule)
	{
		return UnitOfWork.CFL.LoadDataLstSchedule(LstSchedule);
	}

	[HttpGet]
	public DataSet GetListOperation()
	{
		return UnitOfWork.CFL.GetListOperation();
	}

	[HttpGet]
	public DataSet GetListSemiOfProduct(string producCode)
	{
		return UnitOfWork.CFL.GetListSemiOfProduct(producCode);
	}

	[HttpGet]
	public DataTable LoadDataWorkSchedule()
	{
		return UnitOfWork.CFL.LoadDataWorkSchedule();
	}

	[HttpGet]
	public DataSet GetListWorkCenter()
	{
		return UnitOfWork.CFL.GetListWorkCenter();
	}

	[HttpGet]
	public DataSet GetListAdvanceRouting(string proCode)
	{
		return UnitOfWork.CFL.GetListAdvanceRouting(proCode);
	}

	[HttpGet]
	public DataSet GetListTask()
	{
		return UnitOfWork.CFL.GetListTask();
	}

	[HttpGet]
	public DataSet ListItemByCardCode(string cardCode, string type)
	{
		return UnitOfWork.CFL.ListItemByCardCode(cardCode, type);
	}

	[HttpGet]
	public DataSet ListServiceContractTemplate(string type)
	{
		return UnitOfWork.CFL.ListServiceContractTemplate(type);
	}

	[HttpGet]
	public DataTable GetDataServiceLevelAgreement()
	{
		return UnitOfWork.CFL.GetDataServiceLevelAgreement();
	}

	[HttpGet]
	public DataSet GetListItemForServiceCall(string cardCode)
	{
		return UnitOfWork.CFL.GetListItemForServiceCall(cardCode);
	}

	[HttpGet]
	public DataSet GetListSerialNum(string itemCode)
	{
		return UnitOfWork.CFL.GetListSerialNum(itemCode);
	}

	[HttpGet]
	public DataSet GetMarketingCampaign(int campaignId)
	{
		return UnitOfWork.CFL.GetMarketingCampaign(campaignId);
	}

	[HttpGet]
	public DataSet GetWbsTemplate()
	{
		return UnitOfWork.CFL.GetWbsTemplate();
	}

	[HttpGet]
	public DataSet GetAllProjectManager()
	{
		return UnitOfWork.CFL.GetAllProjectManager();
	}

	[HttpGet]
	public DataSet GetAllOpportunities()
	{
		return UnitOfWork.CFL.GetAllOpportunities();
	}

	[HttpGet]
	public DataSet GetListCapacity(string capacityStatus)
	{
		return UnitOfWork.CFL.GetListCapacity(capacityStatus);
	}

	[HttpGet]
	public DataSet GetListResourceWorkSchedule(string status)
	{
		return UnitOfWork.CFL.GetListResourceWorkSchedule(status);
	}

	[HttpGet]
	public DataSet GetBioDataList(int branch, string period)
	{
		return UnitOfWork.CFL.GetBioDataList(branch, period);
	}

	[HttpGet]
	public DataSet GetAllTerritory()
	{
		return UnitOfWork.CFL.GetAllTerritory();
	}

	[HttpGet]
	public DataSet GetBioMapping(int machineNo)
	{
		return UnitOfWork.CFL.GetBioMapping(machineNo);
	}

	[HttpGet]
	public DataSet GetAllGeography()
	{
		return UnitOfWork.CFL.GetAllGeography();
	}

	[HttpGet]
	public DataSet GetAllEvents()
	{
		return UnitOfWork.CFL.GetAllEvents();
	}

	[HttpGet]
	public DataSet GetGeographyDetails(string countryCode, string fatherCode)
	{
		return UnitOfWork.CFL.GetGeographyDetails(countryCode, fatherCode);
	}

	[HttpGet]
	public DataSet GetStore(string ggpCode)
	{
		return UnitOfWork.CFL.GetStore(ggpCode);
	}

	[HttpGet]
	public DataSet GetStoreGeography(string parentCode)
	{
		return UnitOfWork.CFL.GetStoreGeography(parentCode);
	}

	[HttpGet]
	public DataSet GetPO()
	{
		return UnitOfWork.CFL.GetPO();
	}

	[HttpGet]
	public DataSet GetPOforCopy()
	{
		return UnitOfWork.CFL.GetPOforCopy();
	}

	[HttpGet]
	public DataSet GetPOforWrap()
	{
		return UnitOfWork.CFL.GetPOforWrap();
	}

	[HttpGet]
	public DataSet GetCase()
	{
		return UnitOfWork.CFL.GetCase();
	}

	[HttpGet]
	public DataSet GetTypeCase()
	{
		return UnitOfWork.CFL.GetTypeCase();
	}

	[HttpGet]
	public DataSet GetItem()
	{
		return UnitOfWork.CFL.GetItem();
	}

	[HttpGet]
	public DataSet GetTC()
	{
		return UnitOfWork.CFL.GetTC();
	}

	[HttpGet]
	public DataSet GetProductionOrder()
	{
		return UnitOfWork.CFL.GetProductionOrder();
	}

	[HttpGet]
	public DataSet GetUsers()
	{
		return UnitOfWork.CFL.GetUsers();
	}

	[HttpGet]
	public DataSet GetDraftNo()
	{
		return UnitOfWork.CFL.GetDraftNo();
	}

	[HttpGet]
	public DataSet GetDraftType()
	{
		return UnitOfWork.CFL.GetDraftType();
	}

	[HttpGet]
	public DataSet GetWorkingProcessRequest(int userId)
	{
		return UnitOfWork.CFL.GetWorkingProcessRequest(userId);
	}

	[HttpGet]
	public DataSet GetBinLocAttribute(int fldAbs)
	{
		return UnitOfWork.CFL.GetBinLocAttribute(fldAbs);
	}

	[HttpGet]
	public DataSet GetWarehouseSublevel(int fldAbs)
	{
		return UnitOfWork.CFL.GetWarehouseSublevel(fldAbs);
	}

	[HttpGet]
	public DataSet GetWarehouseBinActivated()
	{
		return UnitOfWork.CFL.GetWarehouseBinActivated();
	}

	[HttpGet]
	public DataSet GetAllReportDMS()
	{
		try
		{
			return UnitOfWork.CFL.GetAllReportDMS();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetChannel()
	{
		return UnitOfWork.CFL.GetChannel();
	}

	[HttpGet]
	public DataSet GetSalesRoute()
	{
		return UnitOfWork.CFL.GetSalesRoute();
	}

	[HttpGet]
	public DataSet GetSalesRouteEmployees()
	{
		return UnitOfWork.CFL.GetSalesRouteEmployees();
	}

	[HttpGet]
	public DataSet GetSeriesOutlet()
	{
		return UnitOfWork.CFL.GetSeriesOutlet();
	}

	[HttpGet]
	public DataSet GetBusinessPartner()
	{
		return UnitOfWork.CFL.GetBusinessPartner();
	}

	[HttpGet]
	public DataSet GetDistributors()
	{
		return UnitOfWork.CFL.GetDistributors();
	}

	[HttpGet]
	public DataSet GetWorkingProcessTooltips()
	{
		return UnitOfWork.CFL.GetWorkingProcessTooltips();
	}

	[HttpGet]
	public DataSet GetListContractTemplate()
	{
		return UnitOfWork.CFL.GetListContractTemplate();
	}

	[HttpGet]
	public DataSet GetKPIsMaster()
	{
		return UnitOfWork.CFL.GetKPIsMaster();
	}

	[HttpGet]
	public DataSet GetListKPIs()
	{
		return UnitOfWork.CFL.GetListKPIs();
	}

	[HttpGet]
	public DataSet GetPromotionCondition(string conditionType)
	{
		return UnitOfWork.CFL.GetPromotionCondition(conditionType);
	}

	public DataSet GetApprovalUsers(int departmentCode)
	{
		return UnitOfWork.CFL.GetApprovalUsers(departmentCode);
	}

	[HttpGet]
	public DataSet GetDepartments()
	{
		return UnitOfWork.CFL.GetDepartments();
	}

	[HttpGet]
	public DataSet GetApprovalStages()
	{
		return UnitOfWork.CFL.GetApprovalStages();
	}

	[HttpGet]
	public DataSet GetApprovalTemplateStages()
	{
		return UnitOfWork.CFL.GetApprovalTemplateStages();
	}

	[HttpGet]
	public DataSet GetKPIsSettings()
	{
		return UnitOfWork.CFL.GetKPIsSettings();
	}

	[HttpGet]
	public DataSet GetPromotionBenefit(string benefitType)
	{
		return UnitOfWork.CFL.GetPromotionBenefit(benefitType);
	}

	[HttpGet]
	public DataSet PreDecisionTask(int clgCode)
	{
		return UnitOfWork.CFL.PreDecisionTask(clgCode);
	}

	[HttpGet]
	public DataSetResult GetCflStructure(ApzObjectTypes objType)
	{
		return UnitOfWork.CFL.GetCflStructure(objType);
	}

	[HttpGet]
	public DataSet GetPriceList()
	{
		return UnitOfWork.CFL.GetPriceList();
	}

	[HttpGet]
	public DataSet GetGoodsIssueOpen()
	{
		return UnitOfWork.CFL.GetGoodsIssueOpen();
	}

	[HttpGet]
	public List<string> GetListCFLItemField()
	{
		return UnitOfWork.CFL.GetListCFLItemField();
	}
}
