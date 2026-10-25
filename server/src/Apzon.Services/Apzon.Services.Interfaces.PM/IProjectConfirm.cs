using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.PM;

public interface IProjectConfirm
{
	DataTable LoadData(int id, string projectCode, string startFrom, string startTo, string status);

	DataTable GetDataById(int Id);

	HttpResult Save(DataSet ds, string action);

	HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum);

	HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum);

	string DeleteDocs(string Id);

	HttpResult DeleteData(ref HttpResult ret, ref string objId, ref string objNum);

	int GetNo();

	HttpResult UpdateStatus(int docEntry, string status);

	DataTable FindNextLastFrist(int id, int type);
}
