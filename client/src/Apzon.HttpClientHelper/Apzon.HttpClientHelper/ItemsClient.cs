using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;

namespace Apzon.HttpClientHelper;

internal class ItemsClient : IItems
{
	private const string FormController = "ItemsMaster";

	public async Task<HttpResult> Navigate(int type, string currentCode, string user, int branch)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ItemsMaster");
		return await helper.GetAsync("Navigate", new { type, currentCode, user, branch });
	}

	public async Task<DataSet> GetByCode(string itemCode, string user, int branch)
	{
		using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ItemsMaster");
		return await helper.GetAsync("GetByCode", new { itemCode, user, branch });
	}

	public async Task<DataTable> LoadUomGroup()
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return await helper.GetAsync("LoadUomGroup");
	}

	public async Task<DataTable> LoadPriceList()
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return await helper.GetAsync("LoadPriceList");
	}

	public async Task<DataTable> LoadManufactures()
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return await helper.GetAsync("LoadManufactures");
	}

	public async Task<DataTable> LoadShippingType()
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return await helper.GetAsync("LoadShippingType");
	}

	public async Task<DataTable> LoadCustomGroup()
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return await helper.GetAsync("LoadCustomGroup");
	}

	public async Task<DataTable> LoadVatGroup(string category)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return await helper.GetAsync("LoadVatGroup", new { category });
	}

	public async Task<DataTable> LoadProperties()
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return await helper.GetAsync("LoadProperties");
	}

	public double GetItemsPerUnit(string itmCode, string type)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return Function.ParseDouble(httpClientHelper.GetResult("GetItemsPerUnit", new { itmCode, type }).Content);
	}

	public async Task<DataTable> LoadSeries(string user)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return await helper.GetAsync("LoadSeries", new { user });
	}

	public async Task<DataTable> GetItemStruct()
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return await helper.GetAsync("GetItemStruct");
	}

	public async Task<DataTable> Find(DataTable source, string username)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return await helper.PostDataTableAsync("Find", new Tuple<DataTable, string>(source, username));
	}

	public async Task<DataSet> Cfl(string itemCode, string itemType, string userName, string branch)
	{
		using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
		return await helper.GetAsync("GetListItem", new
		{
			itemCode = itemCode,
			itemType = itemType,
			userName = userName,
			branch = branch,
			pr = ""
		});
	}

	public async Task<DataTable> ReportItemSummary(DateTime fDate, DateTime tDate, string item, string whs, string display, string num)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return await helper.PostDataTableAsync("ReportItemSummary", new Tuple<DateTime, DateTime, string, string, string, string>(fDate, tDate, item, whs, display, num));
	}

	public async Task<DataTable> GetTop1(string itemCode)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return await helper.GetAsync("GetTop1", new { itemCode });
	}

	public string IsExistsTransItemWarehouse(string itemCode, string whsCode)
	{
		using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "ItemsMaster");
		return httpClientHelper.Get("IsExistsTransItemWarehouse", new { itemCode, whsCode });
	}

	public string IsStockInItemWarehouse(string itemCode, string whsCode)
	{
		using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "ItemsMaster");
		return httpClientHelper.Get("IsStockInItemWarehouse", new { itemCode, whsCode });
	}

	public string IsLinkedTrans(string itemCode)
	{
		using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "ItemsMaster");
		return httpClientHelper.Get("IsLinkedTrans", new { itemCode });
	}

	public string RemoveItem(string itemCode)
	{
		using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "ItemsMaster");
		return httpClientHelper.Get("RemoveItem", new { itemCode });
	}

	public DataTable GetPriceList()
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return httpClientHelper.Get("GetPriceList");
	}

	public async Task<string> SaveDoc(ItemsImpl doc)
	{
		try
		{
			using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "ItemsMaster");
			return await http.PostAsync("SaveDoc", doc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> SaveIgnoreEmpty(ItemsImpl doc)
	{
		try
		{
			using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "ItemsMaster");
			return await http.PostAsync("SaveIgnoreEmpty", doc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public string SaveDocDataSet(DataSet ds)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Post("SaveDocDataSet", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable ShowWareHouse()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Get("ShowWareHouse");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable SearchLine(string itemCode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Get("SearchLine", new { itemCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable SearchOITW(string itemCode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Get("SearchOITW", new { itemCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable SearchITM12(string itemCode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Get("SearchITM12", new { itemCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> Search(Tuple<ItemsImpl, DataTable> doc)
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return await http.PostAsync("Search", doc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetADoc(string itmCode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Post("GetADoc", itmCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetRecord(string itmCode, string typeRecord, int BPLId, bool isAssetMaster = false)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Get("GetRecord", new { itmCode, typeRecord, BPLId, isAssetMaster });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataSet GetItemProperties(string itemCode)
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Get("GetItemProperties", new { itemCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable CheckExistItem(string itemCode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Post("CheckExistItem", itemCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetUoMByUomGroup(int ugpEntry)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Get("GetUoMByUomGroup", new { ugpEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable LoadBaseUoM(int ugrpEntry)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Get("LoadBaseUoM", new { ugrpEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetItemLineTableStructure()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Get("GetItemLineTableStructure");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetItemGroup()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemGroups");
			return httpClientHelper.Get("GetItemGroup");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetUoMGroup()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Get("GetUoMGroup");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable ShowCurrency()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Manufacturers");
			return httpClientHelper.Get("ShowCurrency");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetGroupNumPriceList()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "PriceList");
			return httpClientHelper.Get("GetGroupNumPriceList");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetShippingTypes()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ShippingTypes");
			return httpClientHelper.Get("GetShippingTypes");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetManufacturers()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Manufacturers");
			return httpClientHelper.Get("GetManufacturers");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable ItemsMasterGetListCustomsGroup()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Get("ItemsMasterGetListCustomsGroup");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetVatGroupO()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "VatGroups");
			return httpClientHelper.Get("GetVatGroupO");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetVatGroupI()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "VatGroups");
			return httpClientHelper.Get("GetVatGroupI");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetShowOCYC()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Get("GetShowOCYC");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetVolumeUnit()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Get("GetVolumeUnit");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetLghtAndWghtDisplay()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Get("GetLghtAndWghtDisplay");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetUoMCombo(int ugpEntry, string itemCode, string type)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Get("GetUoMCombo", new { ugpEntry, itemCode, type });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetItemGroupData(int itmsGrpCod)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Get("GetItemGroupData", new { itmsGrpCod });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetProductionData(string itemCode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return httpClientHelper.Get("GetProductionData", new { itemCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetPrice(string itemCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return httpClientHelper.Get("GetPrice", new { itemCode });
	}

	public DataTable GetItemWarehouse(string itemCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return httpClientHelper.Get("GetItemWarehouse", new { itemCode });
	}

	public DataTable GetPreferredVendor(string itemCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return httpClientHelper.Get("GetPreferredVendor", new { itemCode });
	}

	public DataTable GetAPreferredVendorData(string itemCode, string vendorCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return httpClientHelper.Get("GetAPreferredVendorData", new { itemCode, vendorCode });
	}

	public bool ItemHaveQuantity(string itemCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return httpClientHelper.GetBool("ItemHaveQuantity", new { itemCode });
	}

	public async Task<int> GetDefaultLengthCode()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return await http.GetIntAsync("GetDefaultLengthCode");
	}

	public DataTable GetUomInItemByUomGroup(int ugpEntry, string itemCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return httpClientHelper.Get("GetUomInItemByUomGroup", new { ugpEntry, itemCode });
	}

	public DataTable GetUoMPricesByItmCodePrList(int? priceList, string itemCode, int ugpEntry)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
		return httpClientHelper.Get("GetUoMPricesByItmCodePrList", new { priceList, itemCode, ugpEntry });
	}
}
