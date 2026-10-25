using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IDMSReportClient
{
	Task<HttpResult> SalesCoverage();

	Task<DataTable> GetMarkers();
}
