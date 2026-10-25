using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;

namespace Apzon.HttpClientHelper;

public interface IItems
{
	Task<HttpResult> Navigate(int p, string currentCode, string userName, int branch);

	Task<DataSet> GetByCode(string itemCode, string username, int branch);

	Task<DataTable> LoadUomGroup();

	Task<DataTable> LoadPriceList();

	Task<DataTable> LoadManufactures();

	Task<DataTable> LoadShippingType();

	Task<DataTable> LoadCustomGroup();

	Task<DataTable> LoadVatGroup(string category);

	Task<DataTable> LoadProperties();

	double GetItemsPerUnit(string itmCode, string type);

	Task<DataTable> LoadSeries(string username);

	Task<DataTable> GetItemStruct();

	Task<DataTable> Find(DataTable source, string user);

	Task<DataSet> Cfl(string itemCode, string itemType, string userName, string branch);

	Task<DataTable> ReportItemSummary(DateTime fDate, DateTime tDate, string item, string whs, string display, string num);

	Task<DataTable> GetTop1(string itemCode);

	string IsExistsTransItemWarehouse(string itemCode, string whsCode);

	string IsStockInItemWarehouse(string itemCode, string whsCode);

	DataTable GetAPreferredVendorData(string itemCode, string vendorCode);

	string IsLinkedTrans(string itemCode);

	string RemoveItem(string itemCode);

	DataTable GetPriceList();

	Task<string> SaveDoc(ItemsImpl doc);

	Task<string> SaveIgnoreEmpty(ItemsImpl doc);

	string SaveDocDataSet(DataSet ds);

	DataTable ShowWareHouse();

	DataTable SearchLine(string itemCode);

	DataTable SearchOITW(string itemCode);

	DataTable SearchITM12(string itemCode);

	Task<DataTable> Search(Tuple<ItemsImpl, DataTable> doc);

	DataTable GetADoc(string itmCode);

	DataTable GetRecord(string tmCode, string typeRecord, int BPLId, bool isAssetMaster = false);

	DataSet GetItemProperties(string itemCode);

	DataTable CheckExistItem(string itemCode);

	DataTable GetUoMByUomGroup(int ugpEntry);

	DataTable LoadBaseUoM(int ugrpEntry);

	DataTable GetItemLineTableStructure();

	DataTable GetItemGroup();

	DataTable GetUoMGroup();

	DataTable ShowCurrency();

	DataTable GetGroupNumPriceList();

	DataTable GetShippingTypes();

	DataTable GetManufacturers();

	DataTable ItemsMasterGetListCustomsGroup();

	DataTable GetVatGroupO();

	DataTable GetVatGroupI();

	DataTable GetShowOCYC();

	DataTable GetVolumeUnit();

	DataTable GetLghtAndWghtDisplay();

	DataTable GetUoMCombo(int ugpEntry, string itemCode, string type);

	DataTable GetItemGroupData(int itmsGrpCod);

	DataTable GetProductionData(string itemCode);

	DataTable GetPreferredVendor(string itemCode);

	bool ItemHaveQuantity(string itemCode);

	Task<int> GetDefaultLengthCode();

	DataTable GetPrice(string itemCode);

	DataTable GetItemWarehouse(string itemCode);

	DataTable GetUomInItemByUomGroup(int ugpEntry, string itemCode);

	DataTable GetUoMPricesByItmCodePrList(int? priceList, string itemCode, int ugpEntry);
}
