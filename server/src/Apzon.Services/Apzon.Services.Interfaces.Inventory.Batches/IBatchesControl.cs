using System.Data;

namespace Apzon.Services.Interfaces.Inventory.Batches;

public interface IBatchesControl
{
	DataSet LoadBatch(string option, string sb, string so, string bp, string userName, int branch);

	string UpdateBatchSalesBlanket(string batches, int argNo);

	string UpdateBatchForSales(DataTable dtBatch);

	string UpdateBatchDetails(DataTable dtBatch);
}
