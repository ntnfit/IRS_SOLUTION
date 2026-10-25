using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;

namespace Apzon.Api.Controllers.PackingManagement;

[ValidateLoginSapExceptionHandling]
public class TraceabilityController : BaseApiController
{
	[HttpGet]
	public DataTable GetCase()
	{
		return UnitOfWork.Traceability.GetCase();
	}

	[HttpGet]
	public DataTable GetCaseTracibility()
	{
		return UnitOfWork.Traceability.GetCaseTracibility();
	}

	[HttpGet]
	public DataTable GetUser()
	{
		return UnitOfWork.Traceability.GetUser();
	}

	[HttpGet]
	public DataTable GetItemByLineNumber(int LineNumber)
	{
		return UnitOfWork.Traceability.GetItemByLineNumber(LineNumber);
	}

	[HttpGet]
	public DataTable Search(string CaseCode, string TypeCode, DateTime PostingDate)
	{
		return UnitOfWork.Traceability.Search(CaseCode, TypeCode, PostingDate);
	}

	[HttpPost]
	public string Update([FromBody] DataSet ds)
	{
		return UnitOfWork.Traceability.Update(ds);
	}

	[HttpGet]
	public DataTable CaseGetDataDoc(string CaseCode, Enums.NavigateType type)
	{
		return UnitOfWork.Traceability.CaseGetDataDoc(CaseCode, type);
	}

	[HttpGet]
	public DataTable GetLastLineNum()
	{
		return UnitOfWork.Traceability.GetLastLineNum();
	}
}
