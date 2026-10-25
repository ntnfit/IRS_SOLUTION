using System.Data;

namespace Apzon.Services.Interfaces.Mrps;

public interface IItemQuotaArrangement
{
	DataTable GetLines(string ItemCode);

	DataTable GetDocs(string str);

	string SaveDoc(DataSet ds);

	string DeleteDoc(string ItemCode);

	DataTable GetADoc(string itemCode);

	DataTable CheckExistPeriod(int periodID);

	DataTable CheckExistItem(string ItemCode);

	DataTable GetItemName(string ItemCode);
}
