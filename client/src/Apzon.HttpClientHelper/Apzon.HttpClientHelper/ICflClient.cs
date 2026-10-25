using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface ICflClient
{
	Task<DataSet> GetResource(string rescode, string userName, string branch);

	Task<DataSet> GetDataByQuery(string query);

	Task<DataSet> GetDataByStore(string storeName, string value, string userName, string branch);

	Task<DataSet> GetDataByProcedure(string storeName, List<SqlParameter> lstpr = null);

	Task<DataSet> LoadBusinessPartner(string cardType);

	Task<DataSet> GetListAtp(string type, string status, string outputType);

	Task<DataSet> GetListScheduleAtp(string atpCode, string atpScheduleType, string atpScheduleStatus, string atpScheduleCode);

	Task<DataSet> GetListMaterialShortageReport(int bplId, string type, string status);

	Task<DataSet> GetListInventoryCounting(string countDate, int countTime, string status, int series, string ref2, int docNum, string wareHouse);

	Task<DataSet> GetEmployees(int branch, string value);

	Task<DataTable> GetDataBaseOnInputForecast();

	Task<DataSet> GetEmployeeContract(int empID);

	Task<DataSet> GetUserList();

	DataSet GetDataFactoringIndicator();

	Task<DataSet> GetSolution();

	Task<DataSet> UserDefinedTexts();

	Task<DataSet> LoadBusinessPartner(string cardType, string userName, string branch, string bpType = "");

	Task<DataSet> GetDataProjectTask(int projectId);

	Task<DataSet> GetDataProjectManager(int projectId = 0);

	Task<DataSet> GetItemBySaleOrder(int orderId);

	Task<DataSet> GetBOMTemplateBySaleOrder();

	Task<DataSet> GetDataProjectReportCompletion(int projectId);

	Task<DataSet> GetDataWBSInTabItem(int projectId);

	Task<DataSet> GetDataItemInTabItem(int projectId, string wbs);

	Task<DataSet> GetDataTrackingIssue();

	Task<DataTable> GetInfoColum(List<DatabaseParamater> lst);

	Task<DataTable> GetQueryParams(string sql);

	Task<DataSet> UserDefinedReport();

	Task<DataSet> GetDataDeliveryTypes();

	Task<DataSet> GetDataLanguages();

	DataSet GetEmployeeListForSalesEmpSetup();

	Task<DataSet> GetBinOfItemGroupsWarehouse(string whsCode);

	Task<DataSet> GetDataDefaultForUser();

	Task<DataSet> GetBatchSerial(string itemCode, string whsCode);

	Task<DataSet> GetDataInventoryCountingRequest();

	Task<DataSet> GetDataBin(string whsCode, string itemCode, int tranType);

	Task<DataSet> GetListOperation();

	Task<DataSet> GetListSemiOfProduct(string producCode);

	Task<DataTable> LoadDataWorkSchedule();

	Task<DataSet> GetListWorkCenter();

	Task<DataSet> GetListAdvanceRouting(string proCode);

	Task<DataSet> GetListTask();

	Task<DataSet> ListItemByCardCode(string cardCode, string type);

	Task<DataSet> ListServiceContractTemplate(string type);

	Task<DataTable> GetDataServiceLevelAgreement();

	Task<DataSet> GetListItemForServiceCall(string cardCode);

	DataSet GetListItemForServiceCallNoAsync(string cardCode);

	Task<DataSet> GetListSerialNum(string itemCode);

	DataSet GetListSerialNumNoAsync(string itemCode);

	Task<DataSet> GetMarketingCampaign(int campaignId = 0);

	Task<DataSet> GetWbsTemplate();

	Task<DataSet> GetAllProjectManager();

	Task<DataSet> GetAllOpportunities();

	Task<DataSet> GetListCapacity(string capacityStatus);

	Task<DataSet> GetListResourceWorkSchedule(string status);

	Task<DataSet> GetBioDataList(int branch, string period);

	Task<DataSet> GetAllTerritory();

	Task<DataSet> GetBioMapping(int machineNo);

	Task<DataSet> GetDataListUoMForUoMPrices(string itemCode, int priceList, int ugpEntry);

	Task<DataSet> GetAllGeography();

	Task<DataSet> GetAllEvents();

	Task<DataSet> GetGeoGraphyDetails(string countryCode, string fatherCode);

	Task<DataSet> GetStore(string ggpCode);

	Task<DataSet> LoadDataGetPO();

	Task<DataSet> LoadDataGetPOforCopy();

	Task<DataSet> LoadDataGetPOforWrap();

	Task<DataSet> LoadDataGetCase();

	Task<DataSet> LoadDataGetTypeCase();

	Task<DataSet> LoadDataGetTC();

	Task<DataSet> LoadDataGetItem();

	Task<DataSet> LoadDataGetProductionOrder();

	Task<DataSet> GetStoreGeography(string parentCode);

	Task<DataSet> GetUsers();

	Task<DataSet> GetDraftNo();

	Task<DataSet> GetDraftType();

	Task<DataSet> GetRequest();

	Task<DataSet> GetWorkingProcessRequest(int userId);

	Task<DataSet> GetBinLocAttribute(int fldAbs);

	Task<DataSet> GetWarehouseSublevel(int fldAbs);

	Task<DataSet> GetWarehouseBinActivated();

	Task<DataSet> GetAllReportDMS();

	Task<DataSet> GetChannel();

	Task<DataSet> GetSalesRoute();

	Task<DataSet> GetSalesRouteEmployees();

	Task<DataSet> GetSeriesOutlet();

	DataSet GetBusinessPartner();

	Task<DataSet> GetDistributors();

	Task<DataSet> GetWorkingProcessTooltips();

	Task<DataSet> GetListContractTemplate();

	Task<DataSet> GetApprovalUsers(int departmentCode);

	Task<DataSet> GetDepartments();

	Task<DataSet> GetApprovalStages();

	Task<DataSet> GetApprovalTemplateStages();

	Task<DataSet> GetListKPIs();

	Task<DataSet> GetPromotionCondition(string conditionType);

	Task<DataSet> GetKPIsMaster();

	Task<DataSet> GetKPIsSettings();

	Task<DataSet> GetPromotionBenefit(string benefitType);

	Task<DataSet> PreDecisionTask(int clgCode);

	Task<DataSetResult> GetCflStructure(ApzObjectTypes objType);

	Task<DataSet> GetPriceList();

	Task<DataSet> GetGoodsIssueOpen();

	Task<List<string>> GetListCFLItemField();
}
