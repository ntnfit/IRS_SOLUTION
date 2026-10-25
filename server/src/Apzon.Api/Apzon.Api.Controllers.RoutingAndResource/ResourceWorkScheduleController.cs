using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.RoutingAndResource;

public class ResourceWorkScheduleController : BaseApiController
{
	[HttpGet]
	public DataTable SearchData(string code, string status, Enums.NavigateType type)
	{
		return UnitOfWork.ResourceWorkSchedule.SearchData(code, status, type);
	}

	[HttpGet]
	public DataTable GetDataLine(string code, int year)
	{
		return UnitOfWork.ResourceWorkSchedule.GetDataLine(code, year);
	}

	[HttpPost]
	public HttpResult Delete([FromBody] string code)
	{
		return UnitOfWork.ResourceWorkSchedule.Save(code, new DataSet(), Enums.TransactionType.D);
	}

	[HttpPost]
	public HttpResult AddNew([FromBody] DataSet dtSave)
	{
		return UnitOfWork.ResourceWorkSchedule.Save("", dtSave, Enums.TransactionType.A);
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet dtSave)
	{
		return UnitOfWork.ResourceWorkSchedule.Save("", dtSave, Enums.TransactionType.U);
	}
}
