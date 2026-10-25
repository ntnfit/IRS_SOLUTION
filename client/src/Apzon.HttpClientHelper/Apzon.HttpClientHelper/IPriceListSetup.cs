using System.Data;
using Apzon.Entities.Models.MasterData;

namespace Apzon.HttpClientHelper;

public interface IPriceListSetup
{
	DataTable LoadData();

	string SaveDoc(DataTable dt);

	DataTable GetGroupNumPriceList();

	string SaveDocSelection(DataSet ds);

	DataTable DetailsLoadData(int listNum);

	string DetailsSaveDoc(DataSet ds);

	int GetUgpEntry(string itemCode);

	DataTable GetBasePrice(int priceList, string itemCode);

	DataTable DetailsLoadIsGross(int _ListNum);

	string DeletePriceList(int listNum);

	string CorrectPriceCurrency();

	DataTable GetPriceForListItem(DataTable lstItemCode, int priceList);

	PriceListCurrencyImpl GetDataCurrencyByPriceList(int priceList);

	DataTable GetDataSystemPriceListDetail(int priceList);
}
