using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Service;

public class SolutionController : BaseApiController
{
	[HttpGet]
	public DataTable Search(int sltCode, int status, string itemCode = "")
	{
		return UnitOfWork.Solution.Search(sltCode, itemCode, status);
	}

	[HttpGet]
	public DataTable FindDataNextLast(int id, int type)
	{
		return UnitOfWork.Solution.FindDataNextLast(id, type);
	}

	[HttpGet]
	public DataTable GetLine(int atcEntry)
	{
		return UnitOfWork.Solution.GetLine(atcEntry);
	}

	[HttpGet]
	public DataTable GetSolutionStatuses()
	{
		return UnitOfWork.Solution.GetSolutionStatuses();
	}

	[HttpGet]
	public DataTable GetOwner()
	{
		return UnitOfWork.Solution.GetOwner();
	}

	[HttpPost]
	public string Add([FromBody] DataSet ds)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "";
			}
			return BrokerService.GetUnitOfWork(SessionId).Solution.Save(ds, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.Solution.Save(ds, "A");
	}

	[HttpPost]
	public string Update([FromBody] DataSet ds)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "";
			}
			return BrokerService.GetUnitOfWork(SessionId).Solution.Save(ds, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.Solution.Save(ds, "U");
	}

	[HttpPost]
	public string DeleteDocs([FromBody] int id)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Solution.DeleteDoc(id, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.Solution.DeleteDocs(id);
	}

	[HttpGet]
	public int GetMaxSltCode()
	{
		return UnitOfWork.Solution.GetMaxSltCode();
	}
}
