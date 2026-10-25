using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.Services.Interfaces.Commons;

public interface ICommons
{
	DataTable SaleEmployee();

	DataTable GetRole();

	DataTable GetQualifications();

	DataTable ContactPerson(string cardCode);

	DataTable VatGroup(string category);

	DataTable ListChooseItemMaster(string ItemCode, Enums.ItemObjectType itObjType, string userCode, int BPLId, string userName = "", string cardcode = "", DateTime? docdate = null, double quantity = 1.0);

	DataTable ItemMaster(string ItemCode, Enums.ItemObjectType itObjType, string userCode, int BPLId, string cardcode = "", DateTime? docdate = null, double quantity = 1.0, string subCatNum = "");

	DataTable ItemMasterTypes(string itemTypes, string userCode, int BPLId);

	DataTable ItemMasterByGroup(string itemGroup, string userCode, int BPLId);

	DataTable ItemMasters(List<Common.ListItems> ItemCode);

	DateTime GetDate();

	DataTable WareHouse();

	DataTable GetMaxID(string table);

	DataTable ForeCast();

	DataTable GetMRP();

	DataTable GetSchedule();

	DataTable GetProductionOrder();

	string GetWhsDefaultByUser(string user);

	DataTable ListDocByCardCode(ApzObjectTypes type, string cardCode);

	DataTable GetListDocumentCopy(ApzObjectTypes objType, ApzObjectTypes toType, string cardCode, string postringDate = "");

	List<DocumentsImpl> ListDocDraft(ApzObjectTypes type);

	List<BusinessPartnersImpl> GetBusinessPartner(string cardCode, ApzObjectTypes type);

	DataTable GetShipType(int id);

	DataTable GetShipTo(string cardCode);

	DataTable GetBillTo(string cardCode);

	DataSet GetDataCombobox(string cardCode);

	DataTable GetRequester(string requester);

	DataTable GetAcctCode(string acctCode);

	List<DocumentsImpl> ListTranferRequest(ApzObjectTypes type);

	DataTable GetCustomer();

	DataTable ShowSeries();

	DataTable GetNextNumberSeries(int series);

	DataTable ItemMaterInventory(string userCode, int BPLId);

	int GetMaxDocEntrySaveAsDraf();

	DataTable ListDocByCardCode_SalesOpportunity(ApzObjectTypes type, string cardCode);

	DataTable GetFormulars(string objType);

	DataTable GetCflFormulars(string objType);

	DataSet Query(string query);

	DataTable GetFormularSearch(string fieldName, string tblName);

	string CreateStoreUdf(string tblName, string line);

	DataTable GetOpportunity(int id);

	DataTable GetOpportunityDetail(int id);

	DataTable GetListCardCodeByTableName(string table, string bpType);

	DataTable GetListContacrPersonByAllCardCode();

	DataTable GetWareHouseByCode(string whsCode);

	DataTable GetBinByWhsCode(string whsCode);

	DataTable GetColumnNames(string tableName);

	DataTable GetBranchList(string userCode);

	DataTable LoadSeriesByPostingDate(string usercode, ApzObjectTypes apzObjectTypes, string docSubType, string postingDate);

	DataTable GetListSeries(string o, int b, string docSubType);

	DataTable GetCodeBySeries(string s, string o, int b);

	DataTable GetIRSConfig();

	DataTable GetIsMultiBranch();

	DataTable GetDefaultData(string userCode, int branch);

	DataTable CheckExistsItem(string itemCode);

	DataTable GetAllContactPerson();

	DataTable GetAllFieldTable(string tablename);

	DataTable LoadDataCashFlow();

	string GetFieldNameFromTable(bool header, string field);

	DataTable GetRecord(int ObjType, string tbl, int DocNum, string TypeRecord, string userCode);

	string GetServerDate();

	DataTable GetAllEmployees();

	DataTable GetPosition();

	DataTable GetExecute(string sql, string filter = "");

	DataTable GetCheckQuantityInventory(string item);

	DataTable GetListSQL(string filter);

	DataTable GetQuantityInWarehouse(string item, string warehouse);

	DataTable GetAllProject();

	DataTable GetRevenueAccount();

	DataTable GetUserCode();

	DataTable GetUDFFieldValueBOMItem(string Father, int ChildNum);

	DataTable GetFreight(string tableName, string DocEntry);

	DataTable GetCountry(int user);

	DataTable GetState(string Code);

	List<Common.Table_ID> GetDatabaseList(bool loginSap, string domain);

	List<Common.Table_ID> GetNewDbList(bool cloudSystem, bool loginSap, string domain, string databaseServer, string databaseUser, string databasePass);

	DataTable LoadDoc(string tableName);

	DataTable GetAddNewRow(string tableName, int codeMax);

	string Save(DataTable dtDoc, string tableName);

	DataTable GetCurrencyByListNum(int listNum);

	DataTable GetTableColumns(string table);

	DataTable GetLicenseCar(string userName, int branch);

	DataSet DocumentGetComBoTabAccounting(string condition);

	DataTable DocumentLoadTabAccounting(int entry, ApzObjectTypes apzObjectTypes);

	DataTable DocumentLoadTabAccountingColumns();

	DataTable CheckExistUgpEntry(int ugpEntry);

	DataTable GetDataParamaterSql(string sql);

	DataSet ExecuteSql(string sql, List<QueryParamater> paramater, string filter = "");

	HttpResult ExecuteSql(string query);

	DataSet ExecuteSqlStoredProcedure(string sql, List<QueryParamater> paramaters);

	DataSet ExecuteTemporyTable(string querytext, DataTable temporaryTable);

	DataSet GetDataCategories(string userCode, string filter = "");

	string SaveQuery(string sql, string name, string category);

	string SaveQueryCategory(DataTable dt);

	DataTable GetDataStructure(string tblName);

	DataTable GetItemOnwh(string item, string wh);

	DataTable GetPriceItem(int priceList, string itemcode);

	DataTable GetSeries(int ObjectCode);

	DataTable GetCurrency();

	DataTable ExistFileInDoc(int docEntry, ApzObjectTypes FormObjectType, string fileName, string fileExt);

	DataTable ExistFileInDocOpportunity(int oprId, string fileName);

	DataTable GetOldBOMSalesQty(int docEntry, int lineNum, ApzObjectTypes FormObjectType);

	DataTable GetFirstRecord(string tableName, string fieldKey);

	DataTable GetLastRecord(string tableName, string columnId);

	DataTable GetNextRecord(string tableName, string columnId, string value);

	DataTable GetPreviousRecord(string tableName, string columnId, string value);

	DataTable GetListOrcCode(string p, DateTime? date);

	DataTable GetDimensionByCode(string code);

	DataTable GetDimensions();

	DataTable DocumentPrice(DataTable dt);

	DataTable GetDocLineUDFNameAndType();

	DataTable GetDocUDFNameAndType();

	DataTable GetSettingSap();

	DataTable GetDataBrach(string userCode);

	DataTable CheckExistValidValue(string queryID, string value);

	DataTable GetDataPricelist();

	string GetItemNameByCode(string code);

	string GetBPCurrency(string cardCode);

	string GetNewDocNum(int objType, int series, int bplId, int usersign);

	DataTable GetDataTaxCode();

	DataTable GetDataItemByCode(string itemCode, int priceList, string userName, int branch);

	DataTable GetListUserReports(string userName, string branch);

	DataTable GetParamStore(string store, string userName, string branch);

	DataSet ExecuteStore(string sql, List<Common.ParameterValue> lstParamater);

	DataSet ReceiptIssueBinData(string batchNo, string bin, ApzObjectTypes objtype);

	DataTable GetColumnSchema(string tableName);

	DataTable GetDataUomGroup();

	string GetUoMCodeByName(int uomGroup, string uomName);

	DataTable GetDataUomByCode(int uomGroup, string uomCode);

	DataTable GetInventoryUoMByItemCode(string itemCode, Enums.ItemUomType type);

	DataTable GetBaseUomByUomGroup(int uomGroup);

	DataTable GetDataItemOnhand(string itemCode, string whsCode);

	DataTable GetDataBPByCode(string cardCode);

	DataTable VirDocumentGetBPPaymentTerm(string cardCode);

	DataTable VirDocumentGetItemPriceByListNum(int listNum, string itemCode);

	DataTable GetPaymentTermByGroupNum(int groupNum);

	string GetDatabaseVersion();

	DataTable ValidAttachmentEntry(int atcEntry);

	DataTable CreateFreightGridDatasource(string vatType);

	string GetHardwarekey(bool isCloudSystem, string database);

	string CheckExitsDatabase(string databaseName);

	DataTable GetInfoCompanyDbServer(string databaseName);

	DataTable GetDTGeneralSettings();

	DataTable GetDataBatch(string batNo, string itemCode, string whsCode);

	DataTable GetDataBatchAndSerial(int absEntry, string serialNo, string itemCode, string whsCode);

	DataTable ExistsTransaction();

	DataTable GetDataTypeObject(ApzObjectTypes objType);

	HttpResult CheckExistsByCodeObject(Enums.TypeCFL type, string code);

	string GetFileNameReport(string code);

	DataTable GetStoreName(string code);

	string GetDocTypeFromDocEntryForDraft(int docEntry);

	DataTable ValidateDocNumDraft(string docNum, int docType);

	DataTable ValidateDocNumByDocType(string docNum, ApzObjectTypes docType);

	DataTable GetTableColumnSchema(string tableName);

	DataTable GetDefaultSeries(string obj, string docSubType, int userId);

	string GetNewApiAddress();

	DataTable GetDataTableNewRow(string tableName);

	DataTable GetFormulaSearchByTable(string tblName);

	DataTable GetDocumentHeader(int docNum, int type);

	bool CheckExistProject(string code);

	DataTable GetDataOperationByCode(string opeCode);

	DataTable GetPrintPreferences(ApzObjectTypes objType, string userName, int branch);

	DataTable GetSearchTable(string table);

	List<string> getUDFFieldSAP(string table);

	string FileAttachmentProcess(DataTable dtFile, int absEntry);

	string GetOverSizeColumns(DataTable dtSource, string tableName);

	DataTable ProcessDefaultColumnValue(DataTable dtSource, string tableName);

	DataTable ExecuteDataTable(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters);

	int ExecuteNonQuery(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters);

	DataSet ExecuteData(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters);

	object ExecuteScalar(string query, CommandType commandType = CommandType.Text, params SqlParameter[] parameters);

	string AddFile(int absEntry);

	DataTable GetCompanyInfo();

	Tuple<string, DataTable> GetDataByObject(ApzObjectTypes objType, string userName, string lngCode);

	HttpResult DMSGetDataByObject(ApzObjectTypes objType, string userName, string lngCode);

	DataSet ExecuteSqlDataSet(string sql, List<QueryParamater> paramater, string filter = "");

	string CheckIsIns(int type, int docNum);

	string CheckPosted(int type, int docEntry);

	HttpResult CheckDocumentDuplicateBpRefNo(ApzObjectTypes objType, string bpCode, string refNo);

	string GetWhsDefault(string userName, int branch);

	DataTable GetListDomain();

	DataTable GetDataState(string countryCode);

	DataTable GetInfomationQuantityMenuDms(string userName);

	DataTable GetDataKPIDms(int userCode, string KPIType);

	DataTable GetDataConfigDMS(string appName);

	DataTable GetDataAuthorizationToken(string tokenId);

	HttpResult DoQuery(string queryText, string userName);

	HttpResult DoQueryWithTempTable(string queryText, string tempTableName, DataTable tempTableData, string userName);

	DataTable GetTableStructureWithDefaultValue(string tableName);

	DataTable GetEInvoiceToken(string usercode);

	HttpResult UpdateDatabase(double appVersion, int buldVersion, string foldeUpdate);

	DataTable GetAccessLog(string databaseName, string user, string fromDate, string toDate);

	DataTable GetAccessLogUser();

	DataTable GetAccessLogLastLogin(string databaseName);

	DataTable GetLogFormById(string formId, string userCode);

	string CheckExistColumn(string table, string column);
}
