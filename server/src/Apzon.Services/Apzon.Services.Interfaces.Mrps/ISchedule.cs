using System;
using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;

namespace Apzon.Services.Interfaces.Mrps;

public interface ISchedule
{
	DataTable CheckPOInShift(string itemCode, DateTime date, string shift, string psCode);

	DataTable SP_APZ_Search(string scheduleNo, DateTime fDate, DateTime tDate, string status);

	DataTable SP_APZ_GetScheduling(string scheduleNo);

	int GetMaxDocEntry(string tableName);

	DataTable SP_APZ_RunSchedule(string mrpNo);

	DataTable SP_APZ_GetHoliDay(DateTime fDate, DateTime tDate);

	HttpResult Save(DataSet ds, Enums.TransactionType tranType, string objId);

	DataTable GetScheduleGenPO(string scheduleNo);

	DataTable GetDataGenProduction(string id, string type, string inputType);

	string UpdateStatus(ProductionOder_Impl_OWOR doc);

	DataTable SP_APZ_ProductionGetCheck(string barcode);

	DataTable GetBarCodeManual(string barcode);

	DataSet Run(string mrpCode, string scheduleType, string fdate, string tdate, string isDetailSchedule);

	DataSet GetDataLine(int id);

	DataTable GetDataDoc(int id, Enums.NavigateType type);

	DataTable GetDataProductionOrder(string itemCode, string version, string sourceType, int sourceId, int sourceLine);

	DataSet GetTablePO();

	DataTable GetDataMrp(int mrpCode);

	DataTable GetWhs(string inputType, int inputId);

	DataTable GetDataGenPurchase(string input, int code, string user);

	DataTable GetSeriesOrderAndRequest(string type);

	DataTable GetDataInventoryTransferFormSchedule(string input, int code, string user);

	DataSet GetDataStructureGenProduction();

	DataTable GetStructureGenPurchase();

	DataTable GetStructureGenTransfer();

	DataTable GetDataStructureAutoreport();

	HttpResult GetNewIdSchedule();

	DataTable GetdataBpItem(string bpCode, string itemCode, string shipDate);

	DataTable GetDocStructure();

	string CheckInputData(string type, int code, string genType);
}
