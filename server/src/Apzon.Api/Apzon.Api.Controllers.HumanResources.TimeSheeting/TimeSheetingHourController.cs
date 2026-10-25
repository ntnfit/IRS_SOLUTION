using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.HumanResources.TimeSheeting;

public class TimeSheetingHourController : BaseApiController
{
	[HttpGet]
	public DataTable LoadTimeSheetingHour(int branch, string dept, int empID, string empName, string status, string period, string workcode, int baseEntry)
	{
		return UnitOfWork.TimeSheetingHour.LoadTimeSheetingHour(branch, dept, empID, empName, status, period, workcode, baseEntry);
	}

	[HttpGet]
	public DataTable LoadTimeSheetingHourSetting(string userSign)
	{
		return UnitOfWork.TimeSheetingHour.LoadTimeSheetingHourSetting(userSign);
	}

	[HttpGet]
	public DataSet LoadTimeSheetingHourCopyFrom(int baseEntry, string workcode)
	{
		return UnitOfWork.TimeSheetingHour.LoadTimeSheetingHourCopyFrom(baseEntry, workcode);
	}

	[HttpPost]
	public HttpResult SaveTimeSheetingHour([FromBody] DataTable dt)
	{
		return UnitOfWork.TimeSheetingHour.SaveTimeSheetingHour(dt);
	}

	[HttpPost]
	public string SaveTimeSheetingHourSetting([FromBody] DataTable dt)
	{
		return UnitOfWork.TimeSheetingHour.SaveTimeSheetingHourSetting(dt);
	}
}
