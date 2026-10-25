using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Commons;

public class DefineNewController : BaseApiController
{
	[HttpGet]
	public DataTable Load(string tableName)
	{
		return UnitOfWork.DefineNew.Load(tableName);
	}

	[HttpGet]
	public DataTable Check(int Id, string tableName, string fieldName)
	{
		return UnitOfWork.DefineNew.Check(Id, tableName, fieldName);
	}

	[HttpGet]
	public DataTable CheckStatusTrackingIssue(int Id, string tableName)
	{
		return UnitOfWork.DefineNew.CheckStatusTrackingIssue(Id, tableName);
	}

	[HttpPost]
	public HttpResult Save(Tuple<DataTable, string, string> dt)
	{
		return UnitOfWork.DefineNew.Save(dt);
	}
}
