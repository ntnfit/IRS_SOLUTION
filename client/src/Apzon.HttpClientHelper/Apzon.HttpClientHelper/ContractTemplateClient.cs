using System.Data;

namespace Apzon.HttpClientHelper;

internal class ContractTemplateClient : IContractTemplateClient
{
	private const string FormController = "ContractTemplate";

	public DataTable CheckDoc(string tmpltName)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ContractTemplate");
		return httpClientHelper.Get("CheckDoc", new { tmpltName });
	}

	public DataTable GetData(string tmpltName)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ContractTemplate");
		return httpClientHelper.Get("GetData", new { tmpltName });
	}

	public DataTable GetRecord(string position, string tmpltName)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ContractTemplate");
		return httpClientHelper.Get("GetRecord", new { position, tmpltName });
	}

	public string SaveDoc(DataSet ds)
	{
		using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "ContractTemplate");
		return httpClientHelper.Post("SaveDoc", ds);
	}

	public DataSet Search(DataTable dt)
	{
		using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ContractTemplate");
		return httpClientHelper.Post("Search", dt);
	}
}
