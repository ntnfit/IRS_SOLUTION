using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

internal class ProductionScheduleClient : IProductionScheduleClient
{
	private const string FormController = "Schedule";

	public async Task<DataSet> Run(string mrpCode, string scheduleType, string fdate, string tdate, string isDetailSchedule)
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "Schedule");
			return await http.GetAsync("Run", new { mrpCode, scheduleType, fdate, tdate, isDetailSchedule });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
