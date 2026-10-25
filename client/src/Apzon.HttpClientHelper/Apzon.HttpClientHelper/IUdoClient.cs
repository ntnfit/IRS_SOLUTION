using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IUdoClient
{
	Task<int> GetMaxId(string code);

	Task<HttpResult> Save(DataSet ds);

	Task<DataTable> GetUdoByCode(Common.Table_Code item);

	Task<string> GetTypeByCode(string code);

	Task<DataTable> GetStatus();

	Task<DataTable> GetByDocEntry(Common.Table_Code item);

	Task<DataTable> GetTableByName(string tblName);

	Task<DataTable> GetUdo1ByCode(string code);

	Task<DataTable> GetListUdo();

	Task<DataTable> GetSeriesByUser(string udoCode);

	Task<string> GetNextNumber(string udoCode, int series);
}
