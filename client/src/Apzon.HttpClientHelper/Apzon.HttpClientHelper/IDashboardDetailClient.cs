using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IDashboardDetailClient
{
	Task<HttpResult> AddDetailInForm(DataSet ds);

	Task<HttpResult> UpdateDetailInForm(DataSet ds);

	Task<DataTable> LoadAll();

	Task<HttpResult> Save(DataTable dt);

	HttpResult UpdateLayout(int id, string layout);

	Task<DataTable> GetById(int id);

	Task<DataTable> GetParams(int dashboardId);

	Task<HttpResult> SaveDataSet(DataSet ds);

	Task<HttpResult> Remove(int itemClickedId);
}
