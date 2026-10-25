using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.DMS;

public interface ISalesRoute
{
	DataTable GetDataDoc(string routeCode, string routeName, string status);

	DataTable GetDataDetails(string routeCode);

	DataTable GetDataOutlets(int id, string routeCode, int empId);

	DataTable GetListDistributors(int id, string routeCode, int empId);

	DataTable GetBpMasterData(string type, string cardCode);

	HttpResult SaveData(DataSet ds, string type);

	DataTable GetRecord(string position, string routeCode);

	HttpResult Remove(string routeCode);

	DataTable GetEmployees(string routeCode);

	DataTable GetOutlets(string routeCode);

	DataTable GetDistributors(string routeCode);
}
