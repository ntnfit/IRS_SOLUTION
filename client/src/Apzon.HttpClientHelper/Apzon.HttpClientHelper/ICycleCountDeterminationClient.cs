using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface ICycleCountDeterminationClient
{
	Task<DataTable> GetList();

	Task<HttpResult> Saves(DataTable line);
}
