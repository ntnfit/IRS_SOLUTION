using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface ICommentsClient
{
	Task<DataTable> GetByObject(string objectType, string objectCode, string objectDetail);

	Task<HttpResult> AddNew(DataTable dt);
}
