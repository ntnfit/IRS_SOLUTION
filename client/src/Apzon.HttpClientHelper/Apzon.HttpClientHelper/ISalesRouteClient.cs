using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface ISalesRouteClient
{
	Task<DataTable> GetDataDoc(string routeCode, string routeName, string status);

	DataTable GetDataDetails(string routeCode);

	Task<DataTable> GetDataOutlets(int id, string routeCode, int empId);

	DataTable GetRecord(string position, string routeCode);

	Task<HttpResult> SaveData(Tuple<DataSet, string> tupData);

	Task<HttpResult> Remove(Tuple<string> tupData);

	Task<DataTable> GetListDistributors(int id, string routeCode, int empId);

	DataTable GetBpMasterData(string type, string cardCode);

	DataTable GetEmployees(string routeCode);

	DataTable GetOutlets(string routeCode);

	DataTable GetDistributors(string routeCode);
}
