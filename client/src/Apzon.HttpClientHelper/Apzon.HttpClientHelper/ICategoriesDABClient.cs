using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface ICategoriesDABClient
{
	Task<DataTable> GetTreeListCategory(string type, int userId);

	Task<DataSet> GetDetailByDasId(int dasId, int userId);

	Task<DataTable> GetCategoriesByParentId(int parentId);

	Task<DataTable> GetLookupCategories();

	Task<string> Remove(int dashId, int userId);

	Task<DataTable> GetXMLByDasId(int DashId, int userId);

	Task<DataTable> LoadData(int DashId, int userId);

	Task<HttpResult> UpdateDashboard(string document, int userId, int dashId);

	Task<DataTable> GetDataCategories();

	Task<HttpResult> SaveCategories(DataTable dt);

	Task<string> DeleteCategory(string ids);

	HttpResult UpdateCaption(DataTable dt);

	HttpResult RemoveCategory(int id);
}
