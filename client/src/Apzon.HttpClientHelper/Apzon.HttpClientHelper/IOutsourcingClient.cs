using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

public interface IOutsourcingClient
{
	DataTable AddNewRow(string grid);

	Task<DataTable> Search(DataTable dt);

	Task<DataTable> GetADoc(int docNum, string grid);

	Task<string> SaveDoc(List<DocumentsImpl> lst);

	DataTable GetItemDetails(string itemCode);

	Task<DataTable> GetBatchSerials(string docEntry, string objType);

	Task<DataTable> GetBatchSerialsColumns();

	DataTable GetAllBinData(string itemCode, string whsCode);

	DataTable GetBOMComponents(DataTable itemCodes);

	Task<double> GetTotalAvgPrice(DataTable dtItemWhs);

	Task<int> GetRecord(string position, int code);

	Task<bool> ExistsCode(int code);

	DataTable GetItemsAvgPrice(DataTable dtItemWhs);

	Task<DataTable> GetBinData(string code, int type, string itemCode);
}
