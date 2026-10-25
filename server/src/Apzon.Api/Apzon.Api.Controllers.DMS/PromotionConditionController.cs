using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.DMS;

public class PromotionConditionController : BaseApiController
{
	[HttpPost]
	public DataSet SearchPromotionCondition([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.PromotionCondition.SearchPromotionCondition(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetLines(int docentry)
	{
		try
		{
			return UnitOfWork.PromotionCondition.GetLines(docentry);
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
		return UnitOfWork.PromotionCondition.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.PromotionCondition.Save(ds, "U");
	}

	[HttpPost]
	public HttpResult Delete([FromBody] DataSet ds)
	{
		return UnitOfWork.PromotionCondition.Save(ds, "D");
	}

	[HttpGet]
	public string GetNameByCode(string conditionCode)
	{
		return UnitOfWork.PromotionCondition.GetNameByCode(conditionCode);
	}

	[HttpGet]
	public DataTable GetAttribute(string code, string objectType)
	{
		return UnitOfWork.PromotionCondition.GetAttribute(code, objectType);
	}
}
