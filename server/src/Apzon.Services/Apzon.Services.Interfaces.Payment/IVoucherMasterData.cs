using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Payment;

public interface IVoucherMasterData
{
	DataTable GetAllVoucher();

	DataTable GetDetailByCode(string cashCode);

	DataTable FindDataNextLast(string id, int type);

	HttpResult Save(DataSet ds, string action);

	HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum);

	HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum);

	string DeleteDocs(string ids);

	HttpResult DeleteData(ref HttpResult ret, ref string objId, ref string objNum);
}
