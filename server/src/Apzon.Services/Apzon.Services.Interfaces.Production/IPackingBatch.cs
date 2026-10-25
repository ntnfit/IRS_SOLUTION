using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Production;

public interface IPackingBatch
{
	DataSet GetBatch(string batchNo);

	DataSet GetIssue(int id);

	DataSet GetReceipt(int id);

	int GetPage(int current, int type, string username, int branch);

	DataSetResult GetDataByBarcode(string barcode, string boxCode);
}
