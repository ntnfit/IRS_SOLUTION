using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Production;

namespace Apzon.HttpClientHelper;

public interface IBillOfMasterialsClient
{
	Task<DataTable> GetLineByFather(string father, string username, int branch);

	bool IsABOM(string code);

	DataTable Warehouse_ShowWarehouse();

	DataTable PriceList_ShowPriceList();

	DataTable GetItemName(string itemCode);

	DataTable ShowITT1(string code);

	DataTable Search(DataSet ds);

	Task<DataTable> getBoMByItemCode(string code);

	DataTable GetActiveDimensionCount();

	string SaveDoc(BillOfMasterials_Impl_OITT doc);

	string SaveDocNoSAP(DataSet ds);

	string SaveDocNoSAPUpdate(DataSet ds);

	DataTable SearchADoc(string code);

	DataTable GetSearchTableDoc();

	DataTable GetSearchTableLine();

	DataTable CheckValidItemType(string code);

	DataTable GetItemPriceByPriceList(string itemCode, int priceList);

	string DeleteDoc(string code);

	DataTable GetRecord(string p, string c);

	DataTable GetLastRecord();

	DataTable GetFirstRecord();

	DataTable GetItemDetails(string type, string code);

	DataTable IsValidCode(int type, string code);

	DataTable GetDataLineBomVersion(string fatherCode, string version);
}
