using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IOperationsClient
{
	Task<DataTable> SearchData(DataTable dt);

	Task<DataTable> GetDataLine(string opeCode);

	Task<DataTable> OperationsGetDataDoc(string OpeCode, Enums.NavigateType type);

	Task<HttpResult> SaveData(DataTable dt);

	Task<HttpResult> DeleteData(DataTable dt);
}
