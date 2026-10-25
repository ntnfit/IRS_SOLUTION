using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Opportunity;

public class ActivitySubjectController : BaseApiController
{
	[HttpGet]
	public DataTable GetAll(string madk)
	{
		return UnitOfWork.ActivitySubjects.GetAll(madk);
	}

	[HttpGet]
	public DataTable GetData(string madk)
	{
		return UnitOfWork.ActivitySubjects.ShowData(madk);
	}

	[HttpGet]
	public DataTable GetUserSignByUserCodeOUSR(string madk)
	{
		return UnitOfWork.ActivitySubjects.GetUserSignByUserCode_tableOUSR(madk);
	}

	[HttpPost]
	public string SaveDoc([FromBody] DataTable dt)
	{
		return UnitOfWork.ActivitySubjects.SaveDoc(dt);
	}

	[HttpPost]
	public string DeleteDoc([FromBody] int code)
	{
		return UnitOfWork.ActivitySubjects.DeleteDoc(code);
	}

	[HttpGet]
	public int GetMaxActivitySubject()
	{
		return UnitOfWork.ActivitySubjects.GetMaxActivitySubject();
	}
}
