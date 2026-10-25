using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IDocumentNumberingClient
{
	Task<DataTable> GetDataSeries(int userId);

	Task<HttpResult> Save(DataSet dataSeries);
}
