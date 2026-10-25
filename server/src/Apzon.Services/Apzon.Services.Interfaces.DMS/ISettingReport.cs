using System.Data;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Services.Interfaces.DMS;

public interface ISettingReport
{
	DataTable GetUserReportParams(int docEntry, string userCode);

	ResponseResult SaveUserReportParams(DataTable dt);

	DataTable GetDataDashboardById(int docEntry, int id);

	DataTable LoadData(int id, string fromDate, string toDate, string report);

	DataTable GetAllLineById(int Id);

	HttpResult Save(DataSet ds, string action);

	string DeleteDocs(string Id);

	int GetNo();

	DataTable FindNextLastFrist(int id, int type);

	DataTable GetColumnStoreReport(string code);

	ResponseResult GetDataById(int Id);

	DataTable GetAllData(string userCode);

	DataTable DataSourceReportByStore(DataTable par);

	DataSet GetParameterByCode(string code);

	DataTable DataSourceCFL(string parName, string valuepar, string storeCFL);

	DataTable GetReportByCode(string code);

	DataTable GetDataStoreNotParameter(string store);

	DataTable GetDataChartById(int Id);

	DataTable GetDefaultDataSourceReport(string code);
}
