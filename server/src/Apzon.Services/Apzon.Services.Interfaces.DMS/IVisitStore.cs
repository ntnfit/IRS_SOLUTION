using System.Data;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.DMS;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Services.Interfaces.DMS;

public interface IVisitStore
{
	DataTable DMSHomeGetData(string userCode);

	DataTable GetVisitStoreStep();

	DataSet GetVisitHistoryDetail(string visitId);

	DataTable GetTop10VisitHistory(string storeCode);

	DataTable GetListVisitHistory(string fromDate, string toDate, int userSign, string listCardCode, int pageIndex);

	HttpResult FinishVisitStore(FinishVisitStoreImpl ds);

	DataTable SearchDataUpdateInventory(string cardCode, string searchKey);

	DataTable GetListCompetitor(string cardCode);

	DataTable GetListCompetitorProductAvailable(string competitorCode);

	DataTable GetOrderByVisitId(string orderType, string visitId);

	DataTable GetVisitSchedule(string userCode, string date);

	ResponseResult GetListPotentialCustomers(string userCode);

	DataTable GetBusinessTypeDataByCardCode(string cardCode);

	ResponseResult SyncDataDetailStore(DataTable listBP, string userName);
}
