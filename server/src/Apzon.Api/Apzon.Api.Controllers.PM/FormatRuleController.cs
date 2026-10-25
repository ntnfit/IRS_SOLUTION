using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.PM;

public class FormatRuleController : BaseApiController
{
	[HttpGet]
	public DataTable GetFormatRule(string formId, string itemId, string user)
	{
		return UnitOfWork.FormatRule.GetFormatRule(formId, itemId, user);
	}

	[HttpPost]
	public string Saves([FromBody] DataTable lst)
	{
		return UnitOfWork.FormatRule.Saves(lst);
	}

	[HttpPost]
	public string CopyFormatRule([FromBody] DataTable dt)
	{
		return UnitOfWork.FormatRule.CopyFormatRule(dt);
	}

	[HttpPost]
	public string Delete([FromBody] Tuple<string, string, string> model)
	{
		return UnitOfWork.FormatRule.Delete(model.Item1, model.Item2, model.Item3);
	}
}
