using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

internal class LandedCostsSetupCLient : ILandedCostsSetupCLient
{
	private const string FormController = "LandedCostsSetup";

	public async Task<DataTable> GetLandedCostsSetup()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LandedCostsSetup");
			return await _help.GetDataTableAsync("GetLandedCostsSetup");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> RemoveLandedCostsSetup(string code)
	{
		try
		{
			using HttpClientHelper<string> heper = new HttpClientHelper<string>(Configuration.ApiServer, "LandedCostsSetup");
			return await heper.PostParamsAsJsonAsync("RemoveLandedCostsSetup", code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> SaveLandedCostsSetup(DataTable dt)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "LandedCostsSetup");
			return await helper.PostAsync("SaveLandedCostsSetup", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
