using System;
using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public class UdtClient : IUdtClient
{
	private const string Controller = "UDT";

	public async Task<DataTable> DatDataUdt(string tableName)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDT");
		return await helper.GetAsync("LoadDoc", new { tableName });
	}

	public async Task<string> SaveData(Tuple<DataTable, string> dataSave)
	{
		using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "UDT");
		return await helper.PostAsync("Save", dataSave);
	}
}
