using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IRoutingClient
{
	Task<DataTable> GetDateDoc(string routCode, string status, string fDate, string tDate, Enums.NavigateType transType);

	Task<DataTable> GetDataLine(string routCode);

	Task<HttpResult> Save(DataSet ds, Enums.TransactionType transaction);
}
