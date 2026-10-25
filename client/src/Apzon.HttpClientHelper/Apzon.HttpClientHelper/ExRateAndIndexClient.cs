using System;
using System.Data;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Global;

namespace Apzon.HttpClientHelper;

internal class ExRateAndIndexClient : IExRateAndIndexClient
{
	private const string FormController = "ExRateAndIndex";

	public double GetExchangeRate(string curr, DateTime date)
	{
		try
		{
			double result = 1.0;
			if (curr.ToUpper() != GlobalData.MainCurrency.ToUpper())
			{
				DataTable dataTable2;
				using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ExRateAndIndex"))
				{
					DataTable dataTable = new DataTable();
					dataTable.Columns.AddRange(new DataColumn[2]
					{
						new DataColumn("Curr", typeof(string)),
						new DataColumn("PostDate", typeof(DateTime))
					});
					dataTable.Rows.Add(dataTable.NewRow());
					dataTable.Rows[0]["Curr"] = curr;
					dataTable.Rows[0]["PostDate"] = date;
					dataTable.AcceptChanges();
					dataTable2 = httpClientHelper.Post("GetExchangeRate", dataTable);
				}
				result = (dataTable2.IsNotNull() ? Function.ParseDouble(dataTable2.Rows[0][0]) : 0.0);
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return 0.0;
		}
	}
}
