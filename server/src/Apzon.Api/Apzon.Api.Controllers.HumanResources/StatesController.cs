using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.HumanResources;

public class StatesController : BaseApiController
{
	[HttpGet]
	public DataTable GetNewRoles()
	{
		return UnitOfWork.States.GetNewRoles();
	}

	[HttpPost]
	public HttpResult SaveNewRoles([FromBody] DataTable ds)
	{
		if (Global.LoginSap && Global.ServiceType == Enums.ServiceType.ServiceLayer)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.Roles.Save(ds, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
		}
		return UnitOfWork.States.SaveNewRoles(ds);
	}

	[HttpPost]
	public string RemoveRoles([FromBody] int objID)
	{
		if (Global.LoginSap && Global.ServiceType == Enums.ServiceType.ServiceLayer)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.Roles.Delete(objID, SessionUserInfo.SessionDiServer).Message;
			}
			return "INVALIDSESSION";
		}
		return UnitOfWork.States.RemoveRoles(objID);
	}

	[HttpGet]
	public DataTable GetEmployeeStatus()
	{
		return UnitOfWork.States.GetEmployeeStatus();
	}

	[HttpGet]
	public DataTable GetTermReason()
	{
		return UnitOfWork.States.GetTermReason();
	}

	[HttpGet]
	public DataTable GetBranchs()
	{
		return UnitOfWork.States.GetBranchs();
	}

	[HttpGet]
	public DataTable GetCountryCode()
	{
		return UnitOfWork.States.GetCountryCode();
	}

	[HttpGet]
	public DataTable GetTeamSetup()
	{
		return UnitOfWork.States.GetTeamSetup();
	}

	[HttpPost]
	public HttpResult SaveTeamSetup([FromBody] DataTable dt)
	{
		if (Global.LoginSap && Global.ServiceType == Enums.ServiceType.ServiceLayer)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.Teams.Save(dt, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
		}
		return UnitOfWork.States.SaveTeamSetup(dt);
	}

	[HttpPost]
	public string RemoveTeamSetup([FromBody] int objID)
	{
		if (Global.LoginSap && Global.ServiceType == Enums.ServiceType.ServiceLayer)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.Teams.Delete(objID, SessionUserInfo.SessionDiServer).Message;
			}
			return "INVALIDSESSION";
		}
		return UnitOfWork.States.RemoveTeamSetup(objID);
	}

	[HttpGet]
	public DataTable GetEmpPos()
	{
		return UnitOfWork.States.GetEmpPos();
	}

	[HttpPost]
	public HttpResult SaveEmpPos([FromBody] DataTable dt)
	{
		return UnitOfWork.States.SaveEmpPos(dt);
	}

	[HttpPost]
	public string RemoveEmpPos([FromBody] int ID)
	{
		return UnitOfWork.States.RemoveEmpPos(ID);
	}

	[HttpGet]
	public DataTable GetEmpStatus()
	{
		return UnitOfWork.States.GetEmpStatus();
	}

	[HttpPost]
	public HttpResult SaveEmpStatus([FromBody] DataTable dt)
	{
		return UnitOfWork.States.SaveEmpStatus(dt);
	}

	[HttpPost]
	public string RemoveEmpStatus([FromBody] int ID)
	{
		return UnitOfWork.States.RemoveEmpStatus(ID);
	}

	[HttpGet]
	public DataTable GetTerminationReasons()
	{
		return UnitOfWork.States.GetTerminationReasons();
	}

	[HttpPost]
	public HttpResult SaveTerminationReasons([FromBody] DataTable dt)
	{
		return UnitOfWork.States.SaveTerminationReasons(dt);
	}

	[HttpPost]
	public string RemoveTerminationReasons([FromBody] int ID)
	{
		return UnitOfWork.States.RemoveTerminationReasons(ID);
	}

	[HttpGet]
	public DataTable GetDepartment()
	{
		return UnitOfWork.States.GetDepartment();
	}

	[HttpPost]
	public string RemoveDepartment([FromBody] int ID)
	{
		return UnitOfWork.States.RemoveDepartment(ID);
	}

	[HttpPost]
	public HttpResult SaveDepartment([FromBody] DataTable dt)
	{
		return UnitOfWork.States.SaveDepartment(dt);
	}

	[HttpGet]
	public DataTable GetBranches()
	{
		return UnitOfWork.States.GetBranches();
	}

	[HttpPost]
	public HttpResult SaveBranches([FromBody] DataTable dt)
	{
		return UnitOfWork.States.SaveBranches(dt);
	}

	[HttpPost]
	public string RemoveBranches([FromBody] int ID)
	{
		return UnitOfWork.States.RemoveBranches(ID);
	}

	[HttpGet]
	public DataTable GetStates()
	{
		return UnitOfWork.States.GetStates();
	}

	[HttpPost]
	public HttpResult SaveStates([FromBody] DataTable dt)
	{
		return UnitOfWork.States.SaveStates(dt);
	}

	[HttpPost]
	public string RemoveStates([FromBody] string code)
	{
		return UnitOfWork.States.RemoveStates(code);
	}

	[HttpGet]
	public DataTable LoadStates(string CountryCode)
	{
		return UnitOfWork.States.LoadStates(CountryCode);
	}
}
