using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Commons;

[ValidateLoginSapExceptionHandling]
public class QueriesController : BaseApiController
{
	[HttpGet]
	public DataSet GetListQueries(string userName, string branch, string filter)
	{
		try
		{
			return UnitOfWork.Query.GetListQueries(userName, branch, filter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public string DeleteLine(int cateId)
	{
		return UnitOfWork.Query.DeleteLine(cateId);
	}

	[HttpPost]
	public string DeleteQuery([FromBody] Tuple<string, string> model)
	{
		return UnitOfWork.Query.DeleteQuery(model.Item1, model.Item2);
	}

	[HttpGet]
	public DataTable GetParameters(string qname)
	{
		return UnitOfWork.Query.GetParameters(qname);
	}

	[HttpPost]
	public DataTable GetColumns([FromBody] Tuple<string, CommandType, DataTable> model)
	{
		return UnitOfWork.Query.GetColumns(model.Item1, model.Item2, model.Item3);
	}

	[HttpPost]
	public HttpResult SaveExtractQuery([FromBody] DataSet ds)
	{
		return UnitOfWork.Query.SaveExtractQuery(ds);
	}

	[HttpGet]
	public DataSet LoadParameters(int reportId)
	{
		return UnitOfWork.Query.LoadParameters(reportId);
	}

	[HttpGet]
	public DataTable ListOfQueryReport(string username)
	{
		return UnitOfWork.Query.ListOfQueryReport(username);
	}

	[HttpPost]
	public DataSet SubmitQueryReport([FromBody] DataTable model)
	{
		return UnitOfWork.Query.SubmitQueryReport(model);
	}

	[HttpPost]
	public string RemoveExtractQR([FromBody] string Id)
	{
		return UnitOfWork.Query.RemoveExtractQR(Id);
	}

	[HttpGet]
	public DataTable GetQueryById(int queryId)
	{
		return UnitOfWork.Query.GetQueryById(queryId);
	}

	[HttpGet]
	public DataTable GetAuthGroup()
	{
		return UnitOfWork.Query.GetAuthGroup();
	}
}
