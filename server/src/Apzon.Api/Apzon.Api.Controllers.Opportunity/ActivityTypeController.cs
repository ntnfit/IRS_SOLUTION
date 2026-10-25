using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Opportunity;

public class ActivityTypeController : BaseApiController
{
	[HttpGet]
	public DataTable GetAll()
	{
		return UnitOfWork.ActivityType.GetAll();
	}

	[HttpGet]
	public DataTable GetData()
	{
		return UnitOfWork.ActivityType.ShowData();
	}

	[HttpGet]
	public DataTable GetUserSignByUserCodeOUSR(string madk)
	{
		return UnitOfWork.ActivityType.GetUserSignByUserCode_tableOUSR(madk);
	}

	[HttpPost]
	public string SaveDoc([FromBody] DataTable dt)
	{
		return UnitOfWork.ActivityType.SaveDoc(dt);
	}

	[HttpPost]
	public string DeleteDoc([FromBody] int code)
	{
		return UnitOfWork.ActivityType.DeleteDoc(code);
	}

	[HttpGet]
	public int GetMaxActivityType()
	{
		return UnitOfWork.ActivityType.GetMaxActivityType();
	}
}
