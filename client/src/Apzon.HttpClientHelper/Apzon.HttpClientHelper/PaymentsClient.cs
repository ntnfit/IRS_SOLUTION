using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

internal class PaymentsClient : IPaymentsClient
{
	private const string FormController = "Payments";

	public async Task<DataTable> GetById(int docEntry)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Payments");
			return await helper.GetAsync("GetById", new { docEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}
}
