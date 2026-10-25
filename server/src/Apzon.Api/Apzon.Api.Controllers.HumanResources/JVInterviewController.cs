using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.HumanResources;

public class JVInterviewController : BaseApiController
{
	[HttpGet]
	public DataTable LoadInterviewStatus()
	{
		return UnitOfWork.JVInterview.LoadInterviewStatus();
	}

	[HttpGet]
	public DataTable LoadInterviewTypes()
	{
		return UnitOfWork.JVInterview.LoadInterviewTypes();
	}

	[HttpGet]
	public DataTable CheckInterviewStatus(int ID)
	{
		return UnitOfWork.JVInterview.CheckInterviewStatus(ID);
	}

	[HttpGet]
	public DataTable CheckInterviewTypes(int ID)
	{
		return UnitOfWork.JVInterview.CheckInterviewTypes(ID);
	}

	[HttpPost]
	public HttpResult Save(Tuple<DataTable, string> dt)
	{
		return UnitOfWork.JVInterview.SaveData(dt);
	}
}
