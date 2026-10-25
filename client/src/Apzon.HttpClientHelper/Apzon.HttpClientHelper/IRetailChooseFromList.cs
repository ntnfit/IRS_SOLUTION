using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IRetailChooseFromList
{
	Task<DataSet> ListItems(string username, string storecode, string itemcode, string cardcode, string barcode);

	Task<DataSet> Customer(string userName, string storeCode, string strSearch);
}
