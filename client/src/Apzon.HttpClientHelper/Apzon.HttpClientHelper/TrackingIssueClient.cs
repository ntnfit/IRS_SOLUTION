using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

internal class TrackingIssueClient : ITrackingIssueClient
{
	private const string FormController = "TrackingIssue";

	public async Task<DataTable> Search(string bpCode, string projectCode, int projectOwner, string reportBy, string handleBy, int id, string status, string fromDate, string toDate, string userName = "")
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TrackingIssue");
			return await hp.GetAsync("Search", new { bpCode, projectCode, projectOwner, reportBy, handleBy, id, status, fromDate, toDate, userName });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSet> GetDataById(int Id)
	{
		try
		{
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, "TrackingIssue");
			return await hp.GetAsync("GetDataById", new { Id });
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
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "TrackingIssue");
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
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "TrackingIssue");
			return await helper.PostAsync("Update", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> DeleteDocs(string Id)
	{
		try
		{
			using HttpClientHelper<string> heper = new HttpClientHelper<string>(Configuration.ApiServer, "TrackingIssue");
			return await heper.PostParamsAsJsonAsync("DeleteDocs", Id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> SaveSearch(DataSet ds, string userId)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "TrackingIssue");
			return await helper.PostAsync("SaveSearch", new Tuple<DataSet, string>(ds, userId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetDataTypeById(string Id)
	{
		try
		{
			using HttpClientHelper<DataTable> heper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TrackingIssue");
			return await heper.GetAsync("GetDataTypeById", new { Id });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetDataSearchByUserId(int userId, string formId)
	{
		try
		{
			using HttpClientHelper<DataTable> heper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TrackingIssue");
			return await heper.GetAsync("GetDataSearchByUserId", new { userId, formId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> LoadIssueByProjectId(int projectId)
	{
		try
		{
			using HttpClientHelper<DataTable> heper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TrackingIssue");
			return await heper.GetAsync("LoadIssueByProjectId", new { projectId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> LoadIssueByCallId(int callId)
	{
		try
		{
			using HttpClientHelper<DataTable> heper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TrackingIssue");
			return await heper.GetAsync("LoadIssueByCallId", new { callId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetEmployeeByUserId(int userId)
	{
		try
		{
			using HttpClientHelper<DataTable> heper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TrackingIssue");
			return await heper.GetAsync("GetEmployeeByUserId", new { userId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> UpdateStatus(DataSet ds)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "TrackingIssue");
			return await helper.PostAsync("UpdateStatus", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSet> GetStructure()
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "TrackingIssue");
			return await helper.GetAsync("GetStructure");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetDataStatusById(string Id)
	{
		try
		{
			using HttpClientHelper<DataTable> heper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TrackingIssue");
			return await heper.GetAsync("GetDataStatusById", new { Id });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetAllStatus()
	{
		try
		{
			using HttpClientHelper<DataTable> heper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TrackingIssue");
			return await heper.GetAsync("GetAllStatus");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetFileByOjectType(string objectType, string objectCode)
	{
		try
		{
			using HttpClientHelper<DataTable> heper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TrackingIssue");
			return await heper.GetAsync("GetFileByOjectType", new { objectType, objectCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
