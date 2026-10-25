using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.LandedCosts;

namespace Apzon.HttpClientHelper;

internal class LandedCostsClient : ILandedCostsClient
{
	private const string FormController = "LandedCosts";

	public async Task<DataTable> SearchData(string fromDate, string toDate, string vendor, string broker, int docNum)
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LandedCosts");
			return await _help.GetAsync("SearchData", new { fromDate, toDate, vendor, broker, docNum });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<DataSet> GetDetailDocId(int docId)
	{
		try
		{
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, "LandedCosts");
			return await hp.GetAsync("GetDetailDocId", new { docId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<DataTable> FindDataNextLast(int id, int type)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LandedCosts");
			return await hp.GetAsync("FindDataNextLast", new { id, type });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<DataTable> GetNo()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LandedCosts");
			return await hp.GetAsync("GetNo");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<DataTable> GetPriceList()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LandedCosts");
			return await hp.GetAsync("GetPriceList");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<DataTable> GetLandedCostName()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LandedCosts");
			return await hp.GetAsync("GetLandedCostName");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<HttpResult> Add(DataSet ds)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "LandedCosts");
			return await helper.PostAsync("Add", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> Update(DataSet ds)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "LandedCosts");
			return await helper.PostAsync("Update", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<string> DeleteDocs(string ids)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "LandedCosts");
			return await helper.PostParamsAsJsonAsync("DeleteDocs", ids);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<DataTable> GetListDetailGLI(int objectType, string ids)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LandedCosts");
			return await hp.GetAsync("GetListDetailGLI", new { objectType, ids });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<DataTable> GetBusinessPartner(string cardCode, string cardType, string userName, string branch)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LandedCosts");
			return await hp.GetAsync("GetBusinessPartner", new { cardCode, cardType, userName, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<HttpResult> Save(LandedCostImpl doc)
	{
		try
		{
			using HttpClientHelper<HttpResult> hp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "LandedCosts");
			return await hp.PostAsync("Save", doc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<HttpResult> Close(int docEntry)
	{
		try
		{
			using HttpClientHelper<HttpResult> hp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "LandedCosts");
			return await hp.PostAsync("Close", docEntry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<int> GetMaxDocEntry()
	{
		try
		{
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "LandedCosts");
			return await hp.GetIntAsync("GetMaxDocEntry");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}
}
