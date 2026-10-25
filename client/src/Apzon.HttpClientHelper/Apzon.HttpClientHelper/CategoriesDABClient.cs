using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

internal class CategoriesDABClient : ICategoriesDABClient
{
	private const string FormController = "CategoriesDAB";

	public async Task<DataTable> GetTreeListCategory(string type, int userId)
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CategoriesDAB");
			return await http.GetAsync("GetTreeListCategory", new { type, userId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSet> GetDetailByDasId(int dasId, int userId)
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CategoriesDAB");
			return await http.GetAsync("GetDetailByDasId", new { dasId, userId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetCategoriesByParentId(int parentId)
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CategoriesDAB");
			return await http.GetAsync("GetCategoriesByParentId", new { parentId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetLayoutByCatId(int catId, int userId)
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CategoriesDAB");
			return await http.GetAsync("GetLayoutByCatId", new { catId, userId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetLookupCategories()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CategoriesDAB");
			return await http.GetAsync("GetLookupCategories");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> Remove(int dashId, int userId)
	{
		try
		{
			using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "CategoriesDAB");
			return await http.PostParamsAsJsonAsync("Remove", new Tuple<int, int>(dashId, userId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetXMLByDasId(int DashId, int userId)
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CategoriesDAB");
			return await http.GetAsync("GetXMLByDasId", new { DashId, userId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> LoadData(int DashId, int userId)
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CategoriesDAB");
			return await http.GetAsync("LoadData", new { DashId, userId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> UpdateDashboard(string document, int userId, int dashId)
	{
		try
		{
			using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "CategoriesDAB");
			return await http.PostAsync("UpdateDashboard", new Tuple<string, int, int>(document, userId, dashId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetDataCategories()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CategoriesDAB");
			return await http.GetAsync("GetDataCategories");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> SaveCategories(DataTable dt)
	{
		try
		{
			using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "CategoriesDAB");
			return await http.PostAsync("SaveCategories", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> DeleteCategory(string ids)
	{
		try
		{
			using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "CategoriesDAB");
			return await http.PostAsJsonAsync("DeleteCategory", ids);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public HttpResult UpdateCaption(DataTable dt)
	{
		try
		{
			using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "CategoriesDAB");
			return httpClientHelper.Post("UpdateCaption", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public HttpResult RemoveCategory(int id)
	{
		try
		{
			using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "CategoriesDAB");
			return httpClientHelper.Post("RemoveCategory", id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
