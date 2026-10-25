using System;
using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;

namespace Apzon.Services.Interfaces.Inventory;

public interface IItemMasterData : IService<ItemsImpl>
{
	DataTable Table();

	List<ItemsImpl> Cfl();

	DataTable ShowItem();

	DataTable ShowWareHouse();

	DataTable Search(Tuple<ItemsImpl, DataTable> doc);

	DataTable GetOITM(ItemsImpl doc);

	DataTable FindDataByShip(ItemsImpl doc);

	DataTable SearchLine(string itemCode);

	DataTable SearchOITW(string itemCode);

	DataTable Check_FirstRecord();

	DataTable Check_LastRecord();

	bool CheckDoc(string itemCode);

	string SaveDoc(DataSet ds, string type = "");

	string DeleteDoc(string itemCode);

	string DeleteOITW(string itemCode, string whsCode);

	DataTable GetNext(ItemsImpl doc);

	DataTable GetPrevious(ItemsImpl doc);

	DataTable GetFirst(ItemsImpl doc);

	DataTable GetLast(ItemsImpl doc);

	DataTable GetRecord(string itmCode, string typeRecord, string userCode, int BPLId, bool isAssetMaster = false);

	DataTable GetFirstRecord();

	DataTable GetLastRecord();

	DataTable GetNextRecord(string item);

	DataTable GetPreviousRecord(string item);

	DataTable GetItemByItemCode(string ItemCode);

	DataTable GetAllDivision();

	DataTable GetCategory(string divCode);

	DataTable GetModel(string group);

	DataTable GetItem(string model, string color, string size);

	DataTable GetModelByDivision(string divCode, int? pageNum, int? pageSize, string catId, string model);

	DataTable LoadUDFDataSAP(string itemCode);

	DataTable CheckExistItem(string ItemCode);

	DataTable GetItemDetails(string itemCode, string userCode, int BPLId);

	DataTable GetOCYC();

	DataTable GetAnItemByItemCode(string itemCode, string username);

	HttpResult Navigate(int type, string currentCode, string user, int branch);

	DataSet GetByCode(string itemCode, string user, int branch);

	DataTable LoadUomGroup();

	DataTable LoadPriceList();

	DataTable LoadManufactures();

	DataTable LoadShippingType();

	DataTable LoadCustomGroup();

	DataTable LoadVatGroup(string category);

	DataTable LoadProperties();

	HttpResult GetItemsPerUnit(string itmCode, string type);

	DataTable LoadSeries(string user);

	DataTable GetItemStruct();

	DataTable Find(DataTable source, string user);

	DataTable ReportItemSummary(DateTime fDate, DateTime tDate, string item, string whs, string display, string num);

	DataTable GetADoc(string itmCode);

	DataSet GetItemProperties(string itemCode);

	DataTable ExistFileInDoc(string itemCode, string fileName, string fileExt);

	DataTable GetItemLineTableStructure();

	DataTable GetUoMGroup();

	DataTable GetVolumeUnit();

	DataTable GetLghtAndWghtDisplay();

	DataTable GetUoMByUomGroup(int ugpEntry);

	DataTable LoadBaseUoM(int ugrpEntry);

	DataTable SearchITM12(string itemCode);

	DataTable GetUoMPricesByItmCodePrList(int? priceList, string itemCode, int ugpEntry);

	DataTable UoMPricesLoadPriceList(int listNum);

	DataTable UoMPricesLoadComBo(string itemCode);

	DataTable GetUoMCombo(int ugpEntry, string itemCode, string type);

	DataTable GetUomInItemByUomGroup(int ugpEntry, string itemCode);

	DataTable UoMInItemGetRowsDetails(int uomEntry, int ugpEntry);

	DataTable ItemsMasterGetListCustomsGroup();

	DataTable GetItemType(string itemCode);

	DataTable GetTop1(string itemCode);

	string IsExistsTransItemWarehouse(string itemCode, string whsCode);

	string IsStockInItemWarehouse(string itemCode, string whsCode);

	string IsLinkedTrans(string itemCode);

	string RemoveItem(string itemCode);

	DataTable GetPriceList();

	DataTable GetItemGroupData(int itmsGrpCod);

	DataTable GetProductionData(string itemCode);

	DataTable GetPreferredVendor(string itemCode);

	DataTable GetAPreferredVendorData(string itemCode, string vendorCode);

	bool ItemHaveQuantity(string itemCode);

	int GetDefaultLengthCode();

	DataTable GetPrice(string itemCode);

	DataTable GetItemWarehouse(string itemCode);

	string SaveDocFromImport(DataSet ds, string action);

	DataTable GetAllDataItem();

	DataTable GetDataItemByItemCode(string itemCode);

	DataTable GetDataItemGroup();

	DataTable SearchWishItemMaster(string userCode);

	DataTable SearchItemMaster(string itemCode, int ItmsGrpCod);

	DataSet GetSaleItems(string userName, string cardCode);
}
