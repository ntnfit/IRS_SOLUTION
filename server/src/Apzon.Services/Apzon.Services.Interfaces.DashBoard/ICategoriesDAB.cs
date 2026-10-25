using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Dashboard;

public interface ICategoriesDAB
{
	DataTable GetDataCategories();

	DataTable GetTreeListCategory(string type, int userId);

	DataSet GetDetailByDasId(int dasId, int userId);

	DataTable GetLayoutByCatId(int userId);

	DataTable GetXMLByDasId(int DashId, int userId);

	string SaveDashboard(string document, string dashboardName, int userId, int catId);

	HttpResult UpdateDashboard(string document, int userId, int dashId);

	DataTable LoadData(int DasId, int userId);

	DataTable GetParameterCatId(int catId, int userId);

	DataTable GetLookupCategories();

	string Remove(int dashId, int userId);

	HttpResult SaveCategories(DataTable dt);

	HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dsSave, ref string objId, ref string objNum);

	HttpResult SaveData(DataTable dt, Enums.TransactionType tranType, ref string objId, ref string objNum);

	string DeleteCategory(string ids);

	HttpResult UpdateCaption(DataTable dt);

	HttpResult RemoveCategory(int id);
}
