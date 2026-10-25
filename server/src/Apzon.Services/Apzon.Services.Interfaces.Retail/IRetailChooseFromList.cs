using System.Data;

namespace Apzon.Services.Interfaces.Retail;

public interface IRetailChooseFromList
{
	DataSet ListItems(string username, string storecode, string itemcode, string cardcode, string barcode);

	DataSet Customer(string username, string storecode, string cardCode);
}
