using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator;

public class ProjectController : BaseApiController
{
	[HttpGet]
	public DataTable GetData()
	{
		return UnitOfWork.Project.ShowData();
	}

	[HttpGet]
	public DataTable GetUserSignByUserCodeOUSR(string madk)
	{
		return UnitOfWork.Project.GetUserSignByUserCode_tableOUSR(madk);
	}

	[HttpGet]
	public string CheckExistCode(string code)
	{
		return UnitOfWork.Project.CheckExistCode(code);
	}

	[HttpPost]
	public string DeleteProject([FromBody] string code)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Project.DeleteProject(code, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.Project.DeleteProject(code);
	}

	[HttpPost]
	public HttpResult SaveDoc([FromBody] DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			return new HttpResult(Enums.MessageCode.Success, unitOfWork.Project.Save(dt, SessionUserInfo.SessionDiServer));
		}
		return UnitOfWork.Project.Save_OPRJ(dt);
	}

	[HttpGet]
	public DataTable LadDataMrpProject(string LstProjectCode)
	{
		return UnitOfWork.Project.LadDataMrpProject(LstProjectCode);
	}

	[HttpGet]
	public bool ExistProject(string code)
	{
		return UnitOfWork.Project.ExistProject(code);
	}
}
