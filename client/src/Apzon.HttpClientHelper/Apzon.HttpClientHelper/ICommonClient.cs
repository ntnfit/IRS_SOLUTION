using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface ICommonClient
{
	Task<DataTable> GetAllEmployees();

	Task<DateTime> GetServerDate();

	Task<DataTable> GetTableStructure(string tableName);

	Task<DataTable> GetLicenseCar(string username, int branch);

	Task<DataTable> ExistFileInDoc(int docEntry, ApzObjectTypes FormObjectType, string fileName, string fileExt);

	Task<DataTable> ExistFileInDocOpportunity(int oprId, string fileName);

	Task<string> GetGoogleMap();

	Task<DataTable> GetBranchList(string userCode);

	Task<DataTable> GetDTGeneralSettings();

	Task<DataTable> GetADM1();

	Task<DataTable> GetAccessLogLastLogin();

	Task<DataTable> GetAccessLogUser();

	Task<DataTable> ExistsTransaction();

	DataSet GetDataCombobox(string cardCode);

	Task<DataTable> GetAccessLog(string user, string fromDate, string toDate);

	Task<DataTable> GetPriceList();

	Task<DataTable> GetDataTypeObject(ApzObjectTypes objType);

	Task<DataTable> GetSaleEmployee();

	DataTable GetDataBrach(string userCode);

	DataTable GetDataBatchAndSerial(int absEntry, string disNumber, string itemCode, string whsCode);

	DataTable DocumentPrice(string itemCode, string cardCode, DateTime? docDate, double qty, string uomCode, int infoPriceList);

	DataTable ItemMaster(string itemCode, int BPLId, Enums.ItemObjectType itObjType = Enums.ItemObjectType.SellItem, string cardcode = "", DateTime? docdate = null, double quantity = 1.0, string subCatNum = "");

	Task<DataSetResult> ExecuteStore(string sql, List<Common.ParameterValue> lstParamater);

	Task<DataTableResult> ExecuteQuery(string sql);

	Task<DataTable> LoadSeriesByPostingDate(string usercode, ApzObjectTypes apzObjectTypes, string docSubType, string postingDate);

	DataTable GetCodeBySeries(string s, string o, int b);

	DataTable GetListSeries(string o, int b, string docSubType);

	DataTable GetDefaultSeries(string obj, string docSubType, int userId);

	DataSet ExecuteSql(string sql, List<QueryParamater> prs = null, string filter = "");

	DataTable GetExecuteSqlResult(string sql, List<QueryParamater> prs = null, string filter = "");

	DataTable GetExecuteStoreResult(string sql, List<QueryParamater> prs = null);

	bool CheckExistsByCodeObject(Enums.TypeCFL type, string code);

	DataTable GetOnstockItem(string itemCode, string whsCode);

	DataTable GetDataItemCode(string itemCode, int priceList, string userName, int branch);

	DataTable GetBusinessPartner(string cardCode);

	Task<DataTable> GetCurrency();

	Task<DataTable> GetDataUomGroup();

	string GetFileNameReport(string code);

	DataTable GetDataTableNewRow(string tableName);

	DataTable GetFormulaSearchByTable(string tblName);

	DataTable GetCflFormulars(string objType);

	Task<DataTable> GetDataOperationByCode(string opeCode);

	Task<DataTable> GetPrintPreferences(ApzObjectTypes objType, string userName, int branch);

	DataSet ExecuteTemporyTable(string querytext, DataTable temporaryTable, string TemporaryTableName = "tbltemp");

	Task<HttpResult> DoQuery(List<ContextActionDoQuery> doquery);

	Task<HttpResult> DoQueryWithTempTable(string queryText, string tempTableName, DataTable tempTableData);

	DataTable GetFreight(string tableName, string docEntry);

	DataTable CreateFreightGridDatasource(string vatType);

	Task<List<Common.Table_ID>> GetDatabaseListByByDomain(string domainName);

	Task<CompanyInfoImpl> GetCompanyInfo();

	Task<Tuple<string, DataTable>> GetDataByObject(ApzObjectTypes objType, string userName, string lngCode);

	Task<DataTable> GetContactPerson(string cardCode);

	Task<string> CheckIsIns(ApzObjectTypes oInvoices, int docNum);

	Task<string> CheckPosted(ApzObjectTypes oDownPayment, int docEntry);

	Task<HttpResult> CheckDocumentDuplicateBpRefNo(ApzObjectTypes objType, string bpCode, string refNo);

	Task<string> GetSystemSuperUser();

	DataTable GetDataState(string countryCode);

	DataTable GetTableStructureWithDefaultValue(string tableName);

	Task<DataTable> GetEInvoiceToken(string usercode);

	Task<string> CheckExistColumn(string table, string column);

	Task<HttpResult> ProcessSAPJson(ProcessJsonImpl data);
}
