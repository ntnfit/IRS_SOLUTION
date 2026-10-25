using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.MasterData;

namespace Apzon.Api.Controllers.Administrator.Setup;

public class RequesterController : BaseApiController
{
	[HttpGet]
	public List<RequesterImpl> Cfl()
	{
		return UnitOfWork.Requester.Cfl();
	}

	[HttpGet]
	public DataTable GetRequesterByUser()
	{
		return UnitOfWork.Requester.GetRequesterByUser();
	}

	[HttpGet]
	public DataTable GetRequesterByEmployee()
	{
		return UnitOfWork.Requester.GetRequesterByEmployee();
	}

	[HttpGet]
	public DataTable GetUser_tableOUSR()
	{
		return UnitOfWork.Requester.GetUser_tableOUSR();
	}

	[HttpGet]
	public DataTable GetAllUser_tableOUSR()
	{
		return UnitOfWork.Requester.GetAllUser_tableOUSR();
	}
}
