using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

internal class BlanketAgreementClient : IBlanketAgreement
{
	private const string Controller = "SalesBlanketAgreement";

	public async Task<DataTable> GetLine(int absId)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesBlanketAgreement");
			return await helper.PostAsync("GetLine", absId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetDoc(DocumentSBAImpl condiDoc)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesBlanketAgreement");
			return await helper.PostAsync("GetDoc", condiDoc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetBlanketAgreementDetails(Tuple<int, int> tupe)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesBlanketAgreement");
			return httpClientHelper.Post("GetBlanketAgreementDetails", tupe);
		}
		catch (Exception)
		{
			return new DataTable();
		}
	}

	public async Task<string> Cancel(int DocEntry)
	{
		try
		{
			using HttpClientHelper<string> _help = new HttpClientHelper<string>(Configuration.ApiServer, "SalesBlanketAgreement");
			return await _help.PostAsync("Cancel", DocEntry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}
}
