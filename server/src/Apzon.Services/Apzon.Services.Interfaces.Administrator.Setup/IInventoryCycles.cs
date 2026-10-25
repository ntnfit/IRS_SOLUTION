using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IInventoryCycles
{
	DataTable FindNextLastFrist(int docId, int type);

	HttpResult Save(DataTable ds, string action);

	HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dsSave, ref string objId, ref string objNum);

	HttpResult SaveData(DataTable doc, Enums.TransactionType tranType, ref string objId, ref string objNum);

	int GetMaxId();
}
