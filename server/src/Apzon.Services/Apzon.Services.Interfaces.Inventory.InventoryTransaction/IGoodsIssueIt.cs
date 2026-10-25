using System.Collections.Generic;
using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Documents;

namespace Apzon.Services.Interfaces.Inventory.InventoryTransaction;

public interface IGoodsIssueIt : IService<DocumentsImpl>
{
	DataSet SelectOne(int idDocEntry);

	int GetMaxDocEntry();

	DataTable GetItemProductionOrder(int docentry);

	List<string> GetTableName(ApzObjectTypes type);

	DataTable GetDepartment(string requester);

	DataTable GetBranch(string requester);

	DataTable GetInStock(string itemCode);

	DataTable GetGroupNum();

	DataTable GetListNum(string key);

	int GetNewDocNum(int series);

	DataTable GetDataDoc(int baseType, int docNum, int docEntry, int priList, string fromDate, string toDate, Enums.NavigateType type, string userCode);
}
