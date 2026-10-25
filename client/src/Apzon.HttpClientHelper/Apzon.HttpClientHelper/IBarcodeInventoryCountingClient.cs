using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IBarcodeInventoryCountingClient
{
	Task<DataSet> GetDataStructure();

	Task<DataTable> SearchDoc(DataTable dtSearch);

	Task<DataTable> GetDataLineByDocEntry(int docEntry);

	Task<int> GetNewDocEntry();

	Task<DataTable> GetNextLastData(Enums.NavigateType type, int docEntry);

	DataTable GetDataByBarcode(string barcodeString, string whsCode, string binCode);

	Task<HttpResult> AddNew(DataSet dsSave);

	Task<HttpResult> UpdateData(DataSet dsSave);

	Task<string> Delete(int docEntry);
}
