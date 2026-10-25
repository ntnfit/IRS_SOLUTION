using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Promotion;

public class PromotionBenefitController : BaseApiController
{
	[HttpPost]
	public DataSet Search([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.PromotionBenefit.Search(dt);
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
			return UnitOfWork.PromotionBenefit.GetLines(docentry);
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
		return UnitOfWork.PromotionBenefit.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.PromotionBenefit.Save(ds, "U");
	}

	[HttpPost]
	public HttpResult Delete([FromBody] DataSet ds)
	{
		return UnitOfWork.PromotionBenefit.Save(ds, "D");
	}

	[HttpGet]
	public string GetNameByCode(string code)
	{
		return UnitOfWork.PromotionBenefit.GetNameByCode(code);
	}

	[HttpGet]
	public DataTable GetAttribute(string code)
	{
		return UnitOfWork.PromotionBenefit.GetAttribute(code);
	}
}
