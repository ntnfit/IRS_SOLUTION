using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Promotion;

public class PromotionProgramController : BaseApiController
{
	[HttpPost]
	public DataSet Search([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.PromotionProgram.Search(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetLinesBenefit(int docentry)
	{
		try
		{
			return UnitOfWork.PromotionProgram.GetLinesBenefit(docentry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetLinesCondition(int docentry)
	{
		try
		{
			return UnitOfWork.PromotionProgram.GetLinesCondition(docentry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		return UnitOfWork.PromotionProgram.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.PromotionProgram.Save(ds, "U");
	}

	[HttpPost]
	public HttpResult Delete([FromBody] DataSet ds)
	{
		return UnitOfWork.PromotionProgram.Save(ds, "D");
	}

	[HttpPost]
	public HttpResult Cancel([FromBody] DataSet ds)
	{
		return UnitOfWork.PromotionProgram.Save(ds, "C");
	}
}
