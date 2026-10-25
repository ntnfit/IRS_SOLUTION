using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

public interface IGoodsReceiptPOByBarcodeClient
{
	Task<DataSetResult> GetDataByBarcode(string barcode, string userName, int branch);

	Task<HttpResult> AddNew(DocumentsImpl doc);

	Task<HttpResult> GetCurrentRate(DataTable dtSearch);
}
