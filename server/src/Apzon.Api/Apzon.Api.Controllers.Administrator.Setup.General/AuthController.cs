using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup.General;

public class AuthController : BaseApiController
{
	[HttpGet]
	public DataTable DMSGetAllAuth(string user)
	{
		return UnitOfWork.Auths.DMSGetAllAuth(user);
	}

	[HttpGet]
	public DataTable GetAllAuth(string user, string lngCode = "")
	{
		return UnitOfWork.Auths.GetAllAuth(user, lngCode);
	}

	[HttpGet]
	public DataTable GetUserForCopy(string user)
	{
		return UnitOfWork.Auths.GetUserForCopy(user);
	}

	[HttpGet]
	public DataTable GetAllAuthForm(string user, string lngCode)
	{
		return UnitOfWork.Auths.GetAllAuthForm(user, lngCode, Global.SuperUser);
	}

	[HttpGet]
	public DataTable GetFormAuth(string user, string form)
	{
		return UnitOfWork.Auths.GetFormAuth(user, form);
	}

	[HttpPost]
	public string SaveDocs([FromBody] DataTable lstDoc)
	{
		return UnitOfWork.Auths.SaveDocs(lstDoc);
	}

	[HttpPost]
	public string SaveDMSAuthorization([FromBody] DataTable dt)
	{
		return UnitOfWork.Auths.SaveDMSAuthorization(dt);
	}

	[HttpPost]
	public string CopyAuth([FromBody] List<string> lstUser)
	{
		return UnitOfWork.Auths.CopyAuth(lstUser);
	}

	[HttpPost]
	public string CopyAuthDMS([FromBody] List<string> lstUser)
	{
		return UnitOfWork.Auths.CopyAuthDMS(lstUser);
	}

	[HttpPost]
	public string InsertFormsToAuthTbl([FromBody] DataTable dt)
	{
		return UnitOfWork.Auths.InsertFormsToAuthTbl(dt);
	}

	[HttpGet]
	public DataTable GetOwnerShipAuthorization(int empId)
	{
		return UnitOfWork.Auths.GetOwnerShipAuthorization(empId);
	}

	[HttpPost]
	public string SaveOwnerShip([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.Auths.SaveOwnerShip(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpGet]
	public string CheckExistsMemu()
	{
		return UnitOfWork.Auths.CheckExistsMemu();
	}

	[HttpPost]
	public HttpResult CopyOwner(List<int> lstEmp)
	{
		return UnitOfWork.Auths.CopyOwner(lstEmp);
	}
}
