using System.Data;
using Apzon.Entities.Models.MasterData;

namespace Apzon.Services.Interfaces.Inventory;

public interface ISTDPriceLists
{
	DataTable LoadData();

	string SaveDoc(DataTable dt);

	DataTable GetGroupNumPriceList();

	string SaveDocSelection(DataSet dt);

	string DetailsSaveDoc(DataSet ds);

	DataTable DetailsLoadData(int ListNum);

	int GetUgpEntry(string itemCode);

	DataTable GetPriceCurrency(int priceList, string itemCode);

	DataTable DetailsLoadIsGross(int listNum);

	string DeletePriceList(int listNum);

	string CorrectPriceCurrency();

	DataTable GetPriceForListItem(DataTable lstItemCode, int priceList);

	PriceListCurrencyImpl GetDataCurrencyByPriceList(int priceList);

	string UpdateRelatedPrices(string itemCode, int basePriceList, int OriginalPriceList);

	DataTable GetDataSystemPriceListDetail(int priceList);
}
