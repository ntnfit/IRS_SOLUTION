using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Inventory.ItemManagement;

public interface IBusinessPartnerCatalogNumbers
{
	DataTable GetItemByCardCode(string cardcode);

	DataTable GetBPByItemCode(string itemcode);

	HttpResult Save(DataTable dt, ApzObjectTypes type);

	string Remove(string code, string subtitle, string item, string type);

	DataTable GetData(Enums.NavigateType type, Enums.BusinessPartnerCatalogNumbersTableType objectType, string par);

	string GetNameKey(string code, Enums.BusinessPartnerCatalogNumbersTableType type);

	DataTable GetBPByCardCode(string cardCode);

	DataTable GetBPByCode(string catalogCode, string cardCode);

	string GetCardNameByCode(string cardCode);

	string GetItemNameByCode(string itemName);

	DataTable GetDataItemByBpCatalog(string subCatNum, string cardCode, string itemCode);

	DataTable GetExisted(DataTable model, ApzObjectTypes fatherObjectTypes);

	string CheckExist(string code, string subtitle, string item, ApzObjectTypes objtype);

	string GetSubCatNumByItemAndCardCode(string itemCode, string cardCode);
}
