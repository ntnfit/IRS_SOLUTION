using System.Collections.Generic;
using System.Data;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Commons;

public interface ICFL
{
	DataSet LoadBusinessPartner(string cardType, string userName, string branch, string bpType = "");

	DataSet GetListItem(string itemCode, string itemType, string userName, string branch, string pr, string listField = "");

	DataSet GetListItemFilter(string itemCode, string itemType, string userName, string branch, string pr);

	DataSet GetDataByStore(string storeName, string value, string userName, string branch);

	DataTable GetListEmployees();

	DataSet GetResource(string rescode, string userName, string branch);

	DataSet GetListWareHouse(string wareHouseCode, string wareHouseName, string branch);

	DataSet GetListOfInstallBase(string customer, string itemCode, string userName, string branch);

	DataSet GetProject(string prjCode, string userName, string branch);

	DataSet GetListMasterResource(string resType);

	DataSet GetListCostCenter();

	DataTable GetAllOcrByDistRule();

	DataSet GetListDefinedShift(int branch);

	DataSet GetListAdvanceForecast();

	DataSet GetListMrp(string mrpCode, string mrpStatus, string forecast, string mrpType);

	DataSet GetListSchedule(string scheduleCode, string mrpCode, string status, string mrpType, string scheduleType, string isGenCapacity);

	DataTable GetListItemsMRPItemCode(int bplId);

	DataSet GetListMRPUsers();

	DataSet GetListDataLines(int branch);

	DataTable LoadDataAdvaceForecast(string lstForecast);

	DataSet GetListBomItem(string code);

	DataSet GetBatchSerialNumbers(string itemCode, string whsCode, string objtype, string bpCode);

	DataSet GetBatchSerialForReturnComponents(int baseEntry, int baseLine, string whsCode);

	DataSet GetBatchSerialBinNumbers(string itemCode, string whsCode, string objtype);

	DataTable GetInfoColum(DataTable dt, string userCode);

	DataSet GetDashBoardController();

	DataSet GetDataTransferRequestCopyFrom(string fromWhsCode, string toWhsCode);

	DataSet GetDataByProcedure(string storeName);

	DataSet GetDataByQuery(string query);

	DataSet GetDataBanks();

	DataSet GetDataSalesOder();

	DataSet GetDataFactoringIndicator();

	DataSet GetDataEmployeesTechnician();

	DataSet GetDataTerritory();

	DataTable LoadDataMrpSalesOrder(string lstMrpSalesOrder);

	DataSet GetDataListUoMByUoMGroup(int entry);

	DataSet GetDataListUoMNameItemMaster();

	DataSet GetDataListUoMs();

	DataSet GetDataListAlterUoMs(string lstUomEntry);

	DataSet GetDataListUoMForUoMPrices(string itemCode, int priceList, int ugpEntry);

	DataSet GetDataChartOfAccount();

	DataSet GetDataUomCode(int uomGroup);

	DataSet ListOfBinLocationByWareHouse(string whsCode);

	DataSet GetProductionOrder(string type);

	DataSet GetListSalesPerson();

	DataSet GetListOfCostCenterByDimCode(int dimCode);

	DataSet GetEmployeeListForSalesEmpSetup();

	DataSet GetListInventoryCounting(string countDate, int countTime, string status, int series, string ref2, int docNum, string wareHouse);

	DataSet GetListInventoryCyles(DataTable search);

	DataSet GetDataBpCatalogItem(string bpCode, string subCatNum, string userName, string docdate, string branch, string itemCode);

	DataSet GetDataBOMCFLOfProduct();

	DataSet GetQualificationList();

	DataSet GetApplicantList();

	DataSet GetListQuestion();

	DataSet GetListAtp(string type, string status, string outputType);

	DataSet FindWorkPeriod(string status, string code, string name);

	DataSet GetListScheduleAtp(string atpCode, string atpScheduleType, string atpScheduleStatus, string atpScheduleCode);

	DataTable LoadDataProductionMaterialShortageReport(string listXcludeOrder);

	DataSet GetLanedCostsList();

	DataSet LandedCostCopyFrom(string vendor);

	DataSet LandedCostGodInvoice(string vendor, int objectType);

	DataSet GetTableNameORW(string objtype);

	DataSet GetSalesOppPartner();

	DataSet GetEmployees(int branch, string value);

	DataSet GetDataListBOMActive();

	DataSet GetEmployeeContract(int empID);

	DataSet GetUserList();

	DataSet GetSolution();

	DataSet UserDefinedTexts();

	DataSet GetDataBPControlAccounts(string cardType, string acctType);

	DataSet GetDataBOMForProduction();

	DataSet GetItemWarrantyTemplates(string type);

	string ValidateDistrRule(string value);

	DataSet GetDataProjectTask(int projectId);

	DataSet GetDataProjectManager(int projectId);

	DataSet GetItemBySaleOrder(int orderId);

	DataSet GetBOMTemplateBySaleOrder();

	DataSet GetDataTrackingIssue();

	DataSet UserDefinedReport();

	DataSet GetDataDeliveryTypes();

	DataSet GetDataLanguages();

	DataSet GetBinOfItemGroupsWarehouse(string whsCode);

	DataSet GetDataProjectReportCompletion(int projectId);

	DataSet GetDataWBSInTabItem(int projectId);

	DataSet GetDataItemInTabItem(int projectId, string wbs);

	DataTable GetAllDocumentByWbs(int objectType, int absEntry, int lineNum);

	DataSet GetDataDefaultForUser();

	DataSet GetBatchSerial(string itemCode, string whsCode);

	DataSet GetDataInventoryCountingRequest();

	DataSet GetDataBin(string whsCode, string itemCode, int tranType);

	DataTable LoadDataLstSchedule(string LstSchedule);

	DataSet GetListOperation();

	DataSet GetListSemiOfProduct(string producCode);

	DataTable LoadDataWorkSchedule();

	DataSet GetListWorkCenter();

	DataSet GetListAdvanceRouting(string proCode);

	DataSet GetListTask();

	DataSet ListItemByCardCode(string cardCode, string type);

	DataSet ListServiceContractTemplate(string type);

	DataTable GetDataServiceLevelAgreement();

	DataSet GetListItemForServiceCall(string cardCode);

	DataSet GetListSerialNum(string itemCode);

	DataSet GetMarketingCampaign(int campaignId = 0);

	DataSet GetWbsTemplate();

	DataSet GetAllProjectManager();

	DataSet GetAllOpportunities();

	DataSet GetListCapacity(string capacityStatus);

	DataSet GetListResourceWorkSchedule(string status);

	DataSet GetBioDataList(int branch, string period);

	DataSet GetAllTerritory();

	DataSet GetBioMapping(int machineNo);

	DataSet GetAllGeography();

	DataSet GetAllEvents();

	DataSet GetGeographyDetails(string countryCode, string fatherCode);

	DataSet GetStore(string ggpCode);

	DataSet GetStoreGeography(string parentCode);

	DataSet GetPO();

	DataSet GetPOforCopy();

	DataSet GetPOforWrap();

	DataSet GetCase();

	DataSet GetTypeCase();

	DataSet GetTC();

	DataSet GetItem();

	DataSet GetProductionOrder();

	DataSet GetUsers();

	DataSet GetDraftNo();

	DataSet GetDraftType();

	DataSet GetWorkingProcessRequest(int userId);

	DataSet GetBinLocAttribute(int fldAbs);

	DataSet GetWarehouseSublevel(int fldAbs);

	DataSet GetWarehouseBinActivated();

	DataSet GetAllReportDMS();

	DataSet GetChannel();

	DataSet GetSalesRoute();

	DataSet GetSalesRouteEmployees();

	DataSet GetSeriesOutlet();

	DataSet GetBusinessPartner();

	DataSet GetDistributors();

	DataSet GetWorkingProcessTooltips();

	DataSet GetListContractTemplate();

	DataSet GetKPIsMaster();

	DataSet GetApprovalUsers(int departmentCode);

	DataSet GetDepartments();

	DataSet GetApprovalStages();

	DataSet GetApprovalTemplateStages();

	DataSet GetListKPIs();

	DataSet GetPromotionCondition(string conditionType);

	DataSet GetKPIsSettings();

	DataSet GetPromotionBenefit(string benefitType);

	DataSet PreDecisionTask(int clgCode);

	DataSetResult GetCflStructure(ApzObjectTypes objType);

	DataSet GetPriceList();

	DataSet GetGoodsIssueOpen();

	List<string> GetListCFLItemField();
}
