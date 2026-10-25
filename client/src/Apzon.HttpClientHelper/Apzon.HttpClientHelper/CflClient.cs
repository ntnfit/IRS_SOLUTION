using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Global;

namespace Apzon.HttpClientHelper;

internal class CflClient : ICflClient
{
	private const string FormController = "CFL";

	public async Task<DataSet> GetResource(string rescode, string userName, string branch)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await helper.GetAsync("GetResource", new { rescode, userName, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSet> GetDataByQuery(string query)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await helper.PostAsync("GetDataByQuery", query);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSet> GetDataByStore(string storeName, string value, string userName, string branch)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await helper.PostAsync("GetDataByStore", new { storeName, value, userName, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSet> GetDataByProcedure(string storeName, List<SqlParameter> lstpr = null)
	{
		try
		{
			if (lstpr != null && lstpr.Count > 0)
			{
				foreach (SqlParameter item in lstpr)
				{
					storeName += ((!string.IsNullOrEmpty(item.ParameterName)) ? $" {item.ParameterName}='{item.Value}'," : $" '{item.Value}',");
				}
				storeName = storeName.Substring(0, storeName.Length - 1);
			}
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await helper.GetAsync("GetDataByProcedure", new { storeName });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSet> LoadBusinessPartner(string cardType)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await helper.GetAsync("LoadBusinessPartner", new
			{
				cardType = cardType,
				userName = GlobalData.UserName,
				branch = GlobalData.DefaultBranch
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSet> GetListAtp(string type, string status, string outputType)
	{
		using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await helper.GetAsync("GetListAtp", new { type, status, outputType });
	}

	public async Task<DataSet> GetListScheduleAtp(string atpCode, string atpScheduleType, string atpScheduleStatus, string atpScheduleCode)
	{
		using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await helper.GetAsync("GetListScheduleAtp", new { atpCode, atpScheduleType, atpScheduleStatus, atpScheduleCode });
	}

	public async Task<DataSet> GetListMaterialShortageReport(int bplId, string type, string status)
	{
		using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "MaterialShortageReport");
		return await helper.GetAsync("GetListMaterialShortageReport", new { bplId, type, status });
	}

	public async Task<DataSet> GetListInventoryCounting(string countDate, int countTime, string status, int series, string ref2, int docNum, string wareHouse)
	{
		using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await helper.GetAsync("GetListInventoryCounting", new { countDate, countTime, status, series, ref2, docNum, wareHouse });
	}

	public async Task<DataSet> GetEmployees(int branch, string value)
	{
		using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await helper.GetAsync("GetEmployees", new { branch, value });
	}

	public async Task<DataTable> GetDataBaseOnInputForecast()
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "AdvanceForecast");
		return await helper.GetAsync("GetDataBaseOnInputForecast");
	}

	public async Task<DataSet> GetEmployeeContract(int empID)
	{
		using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await helper.GetAsync("GetEmployeeContract", new { empID });
	}

	public async Task<DataSet> GetUserList()
	{
		using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await helper.GetAsync("GetUserList");
	}

	public DataSet GetDataFactoringIndicator()
	{
		using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return httpClientHelper.Get("GetDataFactoringIndicator");
	}

	public async Task<DataSet> GetSolution()
	{
		using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await helper.GetAsync("GetSolution");
	}

	public async Task<DataSet> UserDefinedTexts()
	{
		using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await helper.GetAsync("UserDefinedTexts");
	}

	public async Task<DataSet> LoadBusinessPartner(string cardType, string userName, string branch, string bpType = "")
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await helper.GetAsync("LoadBusinessPartner", new { cardType, userName, branch, bpType });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public async Task<DataSet> GetDataProjectTask(int projectId)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetDataProjectTask", new { projectId });
	}

	public async Task<DataSet> GetDataProjectManager(int projectid = 0)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetDataProjectManager", new { projectid });
	}

	public async Task<DataSet> GetItemBySaleOrder(int orderId)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetItemBySaleOrder", new { orderId });
	}

	public async Task<DataSet> GetBOMTemplateBySaleOrder()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetBOMTemplateBySaleOrder");
	}

	public async Task<DataSet> GetDataProjectReportCompletion(int projectId)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetDataProjectReportCompletion", new { projectId });
	}

	public async Task<DataSet> GetDataWBSInTabItem(int projectId)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetDataWBSInTabItem", new { projectId });
	}

	public async Task<DataSet> GetDataItemInTabItem(int projectId, string wbs)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetDataItemInTabItem", new { projectId, wbs });
	}

	public async Task<DataSet> GetDataTrackingIssue()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetDataTrackingIssue");
	}

	public async Task<DataTable> GetInfoColum(List<DatabaseParamater> lst)
	{
		if (lst == null || lst.Count == 0)
		{
			return new DataTable();
		}
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Para", typeof(string));
		dataTable.Columns.Add("Column", typeof(string));
		dataTable.Columns.Add("Table", typeof(string));
		dataTable.BeginLoadData();
		for (int i = 0; i < lst.Count; i++)
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow["Para"] = lst[i].ParaName;
			string text = (string)(dataRow["Column"] = lst[i].ColumnName);
			if (text.LastIndexOf(".", StringComparison.Ordinal) >= 0)
			{
				dataRow["Column"] = text.Substring(text.LastIndexOf(".", StringComparison.Ordinal) + 1);
			}
			dataRow["Table"] = lst[i].TableName;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
		}
		dataTable.EndLoadData();
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CFL");
		return await http.PostAsync("GetInfoColum", new Tuple<DataTable, string>(dataTable, GlobalData.UserCodeLogin));
	}

	public async Task<DataTable> GetQueryParams(string sql)
	{
		return await GetInfoColum(Function.ParseSql(sql));
	}

	public async Task<DataSet> UserDefinedReport()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("UserDefinedReport");
	}

	public async Task<DataSet> GetDataDeliveryTypes()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetDataDeliveryTypes");
	}

	public async Task<DataSet> GetDataLanguages()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetDataLanguages");
	}

	public DataSet GetEmployeeListForSalesEmpSetup()
	{
		using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return httpClientHelper.Get("GetEmployeeListForSalesEmpSetup");
	}

	public async Task<DataSet> GetBinOfItemGroupsWarehouse(string whsCode)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetBinOfItemGroupsWarehouse", new { whsCode });
	}

	public async Task<DataSet> GetDataDefaultForUser()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetDataDefaultForUser");
	}

	public async Task<DataSet> GetBatchSerial(string itemCode, string whsCode)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetBatchSerial", new { itemCode, whsCode });
	}

	public async Task<DataSet> GetDataInventoryCountingRequest()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetDataInventoryCountingRequest");
	}

	public async Task<DataSet> GetDataBin(string whsCode, string itemCode, int tranType)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetDataBin", new { whsCode, itemCode, tranType });
	}

	public async Task<DataSet> GetListOperation()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetListOperation");
	}

	public async Task<DataSet> GetListSemiOfProduct(string producCode)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetListSemiOfProduct", new { producCode });
	}

	public async Task<DataTable> LoadDataWorkSchedule()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("LoadDataWorkSchedule");
	}

	public async Task<DataSet> GetListWorkCenter()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetListWorkCenter");
	}

	public async Task<DataSet> GetListAdvanceRouting(string proCode)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetListAdvanceRouting", new { proCode });
	}

	public async Task<DataSet> GetListTask()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetListTask");
	}

	public async Task<DataSet> ListItemByCardCode(string cardCode, string type)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("ListItemByCardCode", new { cardCode, type });
	}

	public async Task<DataSet> ListServiceContractTemplate(string type)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("ListServiceContractTemplate", new { type });
	}

	public async Task<DataTable> GetDataServiceLevelAgreement()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetDataServiceLevelAgreement");
	}

	public async Task<DataSet> GetListItemForServiceCall(string cardCode)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetListItemForServiceCall", new { cardCode });
	}

	public DataSet GetListItemForServiceCallNoAsync(string cardCode)
	{
		using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return httpClientHelper.Get("GetListItemForServiceCall", new { cardCode });
	}

	public async Task<DataSet> GetListSerialNum(string itemCode)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetListSerialNum", new { itemCode });
	}

	public DataSet GetListSerialNumNoAsync(string itemCode)
	{
		using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return httpClientHelper.Get("GetListSerialNum", new { itemCode });
	}

	public async Task<DataSet> GetMarketingCampaign(int campaignId = 0)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetMarketingCampaign", new { campaignId });
	}

	public async Task<DataSet> GetWbsTemplate()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetWbsTemplate");
	}

	public async Task<DataSet> GetAllProjectManager()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetAllProjectManager");
	}

	public async Task<DataSet> GetAllOpportunities()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetAllOpportunities");
	}

	public async Task<DataSet> GetListCapacity(string capacityStatus)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetListCapacity", new { capacityStatus });
	}

	public async Task<DataSet> GetListResourceWorkSchedule(string status)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetListResourceWorkSchedule", new { status });
	}

	public async Task<DataSet> GetBioDataList(int branch, string period)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetBioDataList", new { branch, period });
	}

	public async Task<DataSet> GetAllTerritory()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetAllTerritory");
	}

	public async Task<DataSet> GetBioMapping(int machineNo)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetBioMapping", new { machineNo });
	}

	public async Task<DataSet> GetDataListUoMForUoMPrices(string itemCode, int priceList, int ugpEntry)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetDataListUoMForUoMPrices", new { itemCode, priceList, ugpEntry });
	}

	public async Task<DataSet> GetAllGeography()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetAllGeography");
	}

	public async Task<DataSet> GetAllEvents()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetAllEvents");
	}

	public async Task<DataSet> GetGeoGraphyDetails(string countryCode, string fatherCode)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetGeoGraphyDetails", new { countryCode, fatherCode });
	}

	public async Task<DataSet> GetStore(string ggpCode)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetStore", new { ggpCode });
	}

	public async Task<DataSet> GetStoreGeography(string parentCode)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetStoreGeography", new { parentCode });
	}

	public async Task<DataSet> LoadDataGetPO()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetPO");
	}

	public async Task<DataSet> LoadDataGetPOforCopy()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetPOforCopy");
	}

	public async Task<DataSet> LoadDataGetPOforWrap()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetPOforWrap");
	}

	public async Task<DataSet> LoadDataGetCase()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetCase");
	}

	public async Task<DataSet> LoadDataGetTypeCase()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetTypeCase");
	}

	public async Task<DataSet> LoadDataGetTC()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetTC");
	}

	public async Task<DataSet> LoadDataGetItem()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetItem");
	}

	public async Task<DataSet> LoadDataGetProductionOrder()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetProductionOrder");
	}

	public async Task<DataSet> GetUsers()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetUsers");
	}

	public async Task<DataSet> GetDraftNo()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetDraftNo");
	}

	public async Task<DataSet> GetDraftType()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetDraftType");
	}

	public async Task<DataSet> GetRequest()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetRequest");
	}

	public async Task<DataSet> GetWorkingProcessRequest(int userId)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetWorkingProcessRequest", new { userId });
	}

	public async Task<DataSet> GetBinLocAttribute(int fldAbs)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetBinLocAttribute", new { fldAbs });
	}

	public async Task<DataSet> GetWarehouseSublevel(int fldAbs)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetWarehouseSublevel", new { fldAbs });
	}

	public async Task<DataSet> GetWarehouseBinActivated()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetWarehouseBinActivated");
	}

	public async Task<DataSet> GetAllReportDMS()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetAllReportDMS");
	}

	public async Task<DataSet> GetChannel()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetChannel");
	}

	public async Task<DataSet> GetSalesRoute()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetSalesRoute");
	}

	public async Task<DataSet> GetSalesRouteEmployees()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetSalesRouteEmployees");
	}

	public async Task<DataSet> GetSeriesOutlet()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetSeriesOutlet");
	}

	public DataSet GetBusinessPartner()
	{
		using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return httpClientHelper.Get("GetBusinessPartner");
	}

	public async Task<DataSet> GetDistributors()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetDistributors");
	}

	public async Task<DataSet> GetWorkingProcessTooltips()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetWorkingProcessTooltips");
	}

	public async Task<DataSet> GetListContractTemplate()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetListContractTemplate");
	}

	public async Task<DataSet> GetApprovalUsers(int departmentCode)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetApprovalUsers", new { departmentCode });
	}

	public async Task<DataSet> GetDepartments()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetDepartments");
	}

	public async Task<DataSet> GetApprovalStages()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetApprovalStages");
	}

	public async Task<DataSet> GetApprovalTemplateStages()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetApprovalTemplateStages");
	}

	public async Task<DataSet> GetListKPIs()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetListKPIs");
	}

	public async Task<DataSet> GetPromotionCondition(string conditionType)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetPromotionCondition", new { conditionType });
	}

	public async Task<DataSet> GetKPIsMaster()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetKPIsMaster");
	}

	public async Task<DataSet> GetKPIsSettings()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetKPIsSettings");
	}

	public async Task<DataSet> GetPromotionBenefit(string benefitType)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetPromotionBenefit", new { benefitType });
	}

	public async Task<DataSet> PreDecisionTask(int clgCode)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("PreDecisionTask", new { clgCode });
	}

	public async Task<DataSetResult> GetCflStructure(ApzObjectTypes objType)
	{
		using HttpClientHelper<DataSetResult> http = new HttpClientHelper<DataSetResult>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetCflStructure", new { objType });
	}

	public async Task<DataSet> GetPriceList()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetPriceList");
	}

	public async Task<DataSet> GetGoodsIssueOpen()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetGoodsIssueOpen");
	}

	public async Task<List<string>> GetListCFLItemField()
	{
		using HttpClientHelper<List<string>> http = new HttpClientHelper<List<string>>(Configuration.ApiServer, "CFL");
		return await http.GetAsync("GetListCFLItemField");
	}
}
