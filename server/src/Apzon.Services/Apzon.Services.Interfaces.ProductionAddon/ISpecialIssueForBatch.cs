using System.Data;

namespace Apzon.Services.Interfaces.ProductionAddon;

public interface ISpecialIssueForBatch
{
	DataTable InsertRowByBatch(string batIssue);

	DataTable InsertRowByBatchToReceipt(string batIssue);

	string UpdateQtyByItemCode(double qty, int docEntry, double Complete);
}
