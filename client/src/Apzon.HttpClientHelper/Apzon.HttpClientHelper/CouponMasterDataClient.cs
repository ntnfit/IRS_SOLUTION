using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

internal class CouponMasterDataClient : ICouponMasterDataClient
{
	private const string FormController = "CouponMasterData";

	public async Task<DataTable> GetAllCoupon()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CouponMasterData");
			return await hp.GetAsync("GetAllCoupon");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> FindDataNextLast(string id, int type)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CouponMasterData");
			return await hp.GetAsync("FindDataNextLast", new { id, type });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> Add(DataSet ds)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "CouponMasterData");
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
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "CouponMasterData");
			return await helper.PostAsync("Update", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetDetailByCode(string couponCode)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CouponMasterData");
			return await hp.GetAsync("GetDetailByCode", new { couponCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> DeleteDocs(string ids)
	{
		try
		{
			using HttpClientHelper<string> heper = new HttpClientHelper<string>(Configuration.ApiServer, "CouponMasterData");
			return await heper.PostParamsAsJsonAsync("DeleteDocs", ids);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
