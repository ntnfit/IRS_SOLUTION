using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Administrator;

namespace Apzon.HttpClientHelper;

public class UserClientRepository : IUserClient
{
	private const string FormController = "Users";

	public async Task<UserDefaultGroupImpl> GetUserDefaultGroup(string username)
	{
		try
		{
			using HttpClientHelper<UserDefaultGroupImpl> helper = new HttpClientHelper<UserDefaultGroupImpl>(Configuration.ApiServer, "Users");
			return await helper.GetAsync("GetUserDefaultGroup", new { username });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public string GetUserIdByEmpId(int empID)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "Users");
			return httpClientHelper.Get("GetUserIdByEmpId", new { empID });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> ShowUsers()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Users");
		return await http.GetAsync("ShowUsers");
	}

	public async Task<DataTable> ShowBranch()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Users");
		return await http.GetAsync("ShowBranch");
	}

	public async Task<DataTable> ShowDepartment()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Department");
		return await http.GetAsync("ShowDepartment");
	}

	public async Task<string> DeleteDoc(int code)
	{
		using HttpClientHelper<string> client = new HttpClientHelper<string>(Configuration.ApiServer, "Users");
		return await client.PostAsync("DeleteDoc", code);
	}

	public async Task<string> SaveDoc(UsersImpl doc)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "Users");
		return await http.PostAsync("SaveDoc", doc);
	}

	public async Task<DataTable> GetUserBranchAssignment(string userCode)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Users");
		return await helper.GetAsync("GetUserBranchAssignment", new { userCode });
	}

	public async Task<DataTable> GetEmployee()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Users");
		return await http.GetAsync("GetEmployee");
	}

	public async Task<DataTable> LoadData()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Users");
		return await http.GetAsync("LoadData");
	}

	public async Task<DataTable> GetAnUser(int userId)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Users");
		return await http.GetAsync("GetAnUser", new { userId });
	}

	public async Task<string> UpdateUsersAcount(UsersImpl doc)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "Users");
		return await http.PostAsync("UpdateUsersAcount", doc);
	}
}
