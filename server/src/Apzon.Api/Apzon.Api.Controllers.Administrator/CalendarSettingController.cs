using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Administrator;

public class CalendarSettingController : BaseApiController
{
	[HttpPost]
	public string SaveDoc([FromBody] DataSet ds)
	{
		return UnitOfWork.CalendarSetting.SaveDoc(ds);
	}

	[HttpGet]
	public DataSet GetDoc(string user)
	{
		return UnitOfWork.CalendarSetting.GetDoc(user);
	}

	[HttpGet]
	public string DeleteDoc(string code)
	{
		return UnitOfWork.CalendarSetting.DeleteDoc(code);
	}
}
