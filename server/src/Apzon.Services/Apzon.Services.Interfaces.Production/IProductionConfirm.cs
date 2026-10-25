using System.Data;

namespace Apzon.Services.Interfaces.Production;

public interface IProductionConfirm
{
	DataSet GetProductionControl(DataTable dt);

	DataSet GetProductionDetail(string docEntry, string username, string branch);

	DataTable GetReceiptBatch(string receiptBatch);

	string isExistBatch(string batchNo);

	string GetWorkCenter(string batchNo);
}
