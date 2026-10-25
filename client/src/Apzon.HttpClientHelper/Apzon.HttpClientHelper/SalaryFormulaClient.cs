using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

internal class SalaryFormulaClient : ISalaryFormula
{
	public string FormController = "SalaryFormula";

	public async Task<DataTable> GetFormula(string formCode, Enums.EventId enumEvent, int branch, int dept)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("GetFormula", new { formCode, enumEvent, branch, dept });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> Save(DataTable model)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
			return await helper.PostAsync("Save", model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public string GetFormulaText(string formCode, Enums.EventId eventId, int branch, int dept)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
			return httpClientHelper.Get("GetFormularText", new
			{
				formCode = formCode,
				enumEvent = eventId,
				branch = branch,
				dept = dept
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public async Task<string> DeleteFormula(string formId, Enums.EventId eventId, int branch, int dept)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
		return await http.PostAsync("DeleteFormula", new Tuple<string, Enums.EventId, int, int>(formId, eventId, branch, dept));
	}
}
