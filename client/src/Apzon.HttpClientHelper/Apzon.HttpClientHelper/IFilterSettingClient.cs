using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IFilterSettingClient
{
	Task<DataSet> GetCategory();

	Task<DataSet> GetFilter(string category);

	Task<DataTable> GetFilterByCode(string code);

	Task<DataTable> GetDataFilter(string datafilter);

	Task<HttpResult> Save(DataTable dt);

	Task<DataSet> GetUsers();

	Task<DataTable> GetUserByFilter(string code);

	Task<string> SaveUserSign(DataTable dt);

	Task<string> Remove(string code);

	Task<DataTable> GetFilterSelect(string userName, string objType);

	Task InsertQueryFilter(string username);
}
