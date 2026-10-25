using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Dashboard;

public interface IDashboardDetail
{
	HttpResult SaveDetailInForm(DataSet ds, string action);

	HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum);

	HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum);

	DataTable LoadAll();

	HttpResult Save(DataTable dt);

	HttpResult UpdateLayout(int id, string layout);

	DataTable GetById(int id);

	DataTable GetParams(int id);

	HttpResult SaveDataSet(DataSet ds);

	HttpResult Remove(int dashBoardId);
}
