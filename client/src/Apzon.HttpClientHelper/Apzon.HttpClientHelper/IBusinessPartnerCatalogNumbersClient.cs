using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IBusinessPartnerCatalogNumbersClient
{
	DataTable GetDataItemByBpCatalog(string subCatNum, string cardCode, string itemCode);

	DataTable GetBpByItemCode(string itemcode);

	DataTable GetItemByCardCode(string cardcode);

	string Remove(Tuple<string, string, string, string> model);

	DataTable GetData(Enums.NavigateType type, Enums.BusinessPartnerCatalogNumbersTableType objectType, string par);

	string GetNameKey(string code, Enums.BusinessPartnerCatalogNumbersTableType type);

	Task<string> GetCardNameByCode(string cardCode);

	Task<string> GetItemNameByCode(string itemCode);

	Task<HttpResult> SaveItem(DataTable dt);

	Task<HttpResult> SaveBp(DataTable dt);

	string CheckExist(string code, string subtitle, string item, ApzObjectTypes objtype);

	string GetSubCatNumByItemAndCardCode(string itemCode, string cardCode);
}
