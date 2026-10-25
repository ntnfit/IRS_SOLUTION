using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface ISTDPriceLists
{
	string Save(DataTable dt, string sessionId);

	string SaveItemPrice(DataTable dt, string sessionId);

	string UpdatePriceFromBOM(string itemCode, int priceList, double price, string currency, string sessionId);

	string Delete(int listNum, string sessionId);
}
