using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.PackingManagement;

[ValidateLoginSapExceptionHandling]
public class CreateTypeCaseController : BaseApiController
{
	[HttpGet]
	public DataTable GetTypeCase()
	{
		return UnitOfWork.CreateTypeCase.GetTypeCase();
	}

	[HttpGet]
	public DataTable TypeCaseGetDataDoc(Enums.NavigateType type, string TypeCode)
	{
		return UnitOfWork.CreateTypeCase.TypeCaseGetDataDoc(type, TypeCode);
	}

	[HttpPost]
	public HttpResult SaveTCDataSet([FromBody] DataSet ds)
	{
		return UnitOfWork.CreateTypeCase.SaveTCDataSet(ds, "A");
	}

	[HttpPost]
	public HttpResult SaveTCDataSetUpdate([FromBody] DataSet ds)
	{
		return UnitOfWork.CreateTypeCase.SaveTCDataSet(ds, "U");
	}

	[HttpGet]
	public string DelTC(string TypeCode)
	{
		return UnitOfWork.CreateTypeCase.DelTC(TypeCode);
	}

	[HttpGet]
	public DataTable Search(string TypeCode, string TypeName, DateTime CreateDate)
	{
		return UnitOfWork.CreateTypeCase.Search(TypeCode, TypeName, CreateDate);
	}
}
