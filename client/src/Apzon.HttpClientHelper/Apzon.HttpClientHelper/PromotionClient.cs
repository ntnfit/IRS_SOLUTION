using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

internal class PromotionClient : IPromotionClient
{
	private const string FormController = "Promotion";

	public async Task<DataTable> CheckPromotion(DataTable input)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Promotion");
			return await helper.PostAsync("CheckPromotion", input);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<DataTable> GetExistDocPromotion(string guid)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Promotion");
			return await helper.GetAsync("GetExistDocPromotion", new { guid });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<DataTable> GetHeader(string code)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Promotion");
			return await helper.GetAsync("GetHeader", new { code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<DataTable> GetListPromotions(int promotionType, string cardCode, string promotionCode, string groupCode)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Promotion");
			return await helper.GetAsync("GetListPromotions", new { promotionType, cardCode, promotionCode, groupCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<DataTable> LoadBenefit(string code)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Promotion");
			return await helper.GetAsync("LoadBenefit", new { code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<DataTable> LoadCondition(string code)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Promotion");
			return await helper.GetAsync("LoadCondition", new { code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<HttpResult> Save(DataSet ds)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Promotion");
			return await helper.PostAsync("Save", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<HttpResult> SaveAppliedBenefit(DataTable model)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Promotion");
			return await helper.PostAsync("SaveAppliedBenefit", model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}
}
