using System.Data;
using Apzon.Entities.Models.Inventory;

namespace Apzon.Services.Interfaces.Inventory.InventoryTransaction;

public interface IInventoryRevaluation
{
	int GetMaxDocEntry();

	int GetMaxLineNum();

	bool CheckDoc(int docEntry);

	bool CheckDocLineMRV1(int docEntry, int lineNum);

	bool CheckDocLineMRV2(int docEntry, int lineNum);

	DataTable Search(InventoryRevaluationImpl doc);

	DataTable SearchLineMRV1(int idDocEntry);

	DataTable SearchLineMRV2(int idDocEntry);

	string SaveDoc(InventoryRevaluationImpl doc);

	string SaveLineMRV1(InventoryRevaluation_LineMRV1Impl line);

	string SaveLineMRV2(InventoryRevaluation_LineMRV2Impl line);

	string DeleteLineMRV1(int docEntry, int lineNum);

	string DeleteLineMRV2(int docEntry, int lineNum);
}
