using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IVehicleType
{
	DataTable GetList();

	HttpResult Save(DataTable dt);

	HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dtSave, ref string objId, ref string objNum);

	HttpResult SaveData(DataTable dt, Enums.TransactionType tranType, ref string objId, ref string objNum);

	string DeleteDocs(string ids);

	HttpResult DeleteData(ref HttpResult ret, ref string objId, ref string objNum);
}
