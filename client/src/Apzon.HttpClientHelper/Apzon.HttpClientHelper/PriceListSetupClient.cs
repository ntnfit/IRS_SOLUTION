using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.MasterData;

namespace Apzon.HttpClientHelper;

internal class PriceListSetupClient : IPriceListSetup
{
	private const string FormController = "STDPriceLists";

	public DataTable LoadData()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "STDPriceLists");
			return httpClientHelper.Get("LoadData");
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
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "STDPriceLists");
			return httpClientHelper.Get("GetGroupNumPriceList");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public string SaveDoc(DataTable dt)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "STDPriceLists");
			return httpClientHelper.Post("SaveDoc", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public string SaveDocSelection(DataSet ds)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "STDPriceLists");
			return httpClientHelper.Post("SaveDocSelection", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable DetailsLoadData(int listNum)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "STDPriceLists");
			return httpClientHelper.Get("DetailsLoadData", new { listNum });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public string DetailsSaveDoc(DataSet ds)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "STDPriceLists");
			return httpClientHelper.Post("DetailsSaveDoc", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public int GetUgpEntry(string itemCode)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "STDPriceLists");
			return httpClientHelper.GetInt("GetUgpEntry", new { itemCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetBasePrice(int priceList, string itemCode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "STDPriceLists");
			return httpClientHelper.Get("GetBasePrice", new { priceList, itemCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable DetailsLoadIsGross(int listNum)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "STDPriceLists");
			return httpClientHelper.Get("DetailsLoadIsGross", new { listNum });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public string DeletePriceList(int listNum)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "STDPriceLists");
			return httpClientHelper.Post("DeletePriceList", listNum);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public string CorrectPriceCurrency()
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "STDPriceLists");
			return httpClientHelper.Get("CorrectPriceCurrency");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetPriceForListItem(DataTable lstItemCode, int priceList)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "STDPriceLists");
		return httpClientHelper.Post("GetPriceForListItem", new Tuple<DataTable, int>(lstItemCode, priceList));
	}

	public PriceListCurrencyImpl GetDataCurrencyByPriceList(int priceList)
	{
		using HttpClientHelper<PriceListCurrencyImpl> httpClientHelper = new HttpClientHelper<PriceListCurrencyImpl>(Configuration.ApiServer, "STDPriceLists");
		return httpClientHelper.Get("GetDataCurrencyByPriceList", new { priceList });
	}

	public DataTable GetDataSystemPriceListDetail(int priceList)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "STDPriceLists");
		return httpClientHelper.Get("GetDataSystemPriceListDetail", new { priceList });
	}
}
