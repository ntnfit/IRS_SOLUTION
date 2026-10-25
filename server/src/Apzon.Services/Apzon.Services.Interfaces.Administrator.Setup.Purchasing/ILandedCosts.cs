using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup.Purchasing;

public interface ILandedCosts
{
	DataTable SearchData(string fromDate, string toDate, string vendor, string broker, int docNum);

	DataSet GetDetaiByDocId(int docId);

	DataTable FindDataNextLast(int id, int type);

	DataTable GetNo();

	DataTable GetPriceList();

	DataTable GetLandedCostName();

	HttpResult Save(DataSet ds, string action);

	HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum);

	HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum);

	HttpResult DeleteData(ref HttpResult ret, ref string objId, ref string objNum);

	string DeleteDocs(string ids);

	DataTable GetListDetailGLI(int objectType, string ids);

	DataTable GetBusinessPartner(string cardCode, string cardType, string userName, string branch);

	int GetMaxDocEntry();
}
