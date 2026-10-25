using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IDMSDeliveryClient
{
	Task<DataSet> SearchDeliveryOrder(DataTable dt);

	Task<DataSet> GetLines(int docentry);

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);
}
