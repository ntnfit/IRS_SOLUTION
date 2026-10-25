using System.Data;

namespace Apzon.Services.Interfaces.Inventory;

public interface IPeriodAndVolumeDiscounts
{
	DataSet LoadData(string cardCode);

	string SaveDoc(DataSet ds);

	DataTable GetItemByItemCode(string itemCode, int listNum);

	DataTable GetItems(string fCode, string tCode, string fVendor, string tVendor);
}
