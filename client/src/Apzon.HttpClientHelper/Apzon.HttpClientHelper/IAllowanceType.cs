using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IAllowanceType
{
	Task<DataTable> GetAllowanceType();

	Task<string> RemoveAllowanceType(int code);

	Task<HttpResult> SaveAllowanceType(DataTable dt);

	Task<DataTable> GetPerIncomeTax();

	Task<DataTable> GetDetail(int code);

	Task<string> RemovePerIncomeTax(string code);

	Task<HttpResult> SavePersonalIncomeTax(DataSet ds);

	Task<HttpResult> UpdatePersonalIncomeTax(DataSet ds);

	Task<int> GetMaxCode();

	Task<DataTable> FindDataNextLast(int id, int type);
}
