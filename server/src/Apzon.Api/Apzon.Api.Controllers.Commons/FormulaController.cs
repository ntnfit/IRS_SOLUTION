using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Commons;

public class FormulaController : BaseApiController
{
	[HttpGet]
	public DataSet GetData(string sql)
	{
		return UnitOfWork.Formula.GetData(sql);
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataTable model)
	{
		return UnitOfWork.Formula.Save(model);
	}

	[HttpGet]
	public string GetFormulaText(string formCode, Enums.EventId enumEvent, string itemEvent)
	{
		return UnitOfWork.Formula.GetFormulaText(formCode, enumEvent, itemEvent);
	}

	[HttpGet]
	public string GetFormularText(string formCode, Enums.EventId enumEvent, string itemEvent)
	{
		return UnitOfWork.Formula.GetFormulaText(formCode, enumEvent, itemEvent);
	}

	[HttpGet]
	public DataTable GetFormula(string formCode, Enums.EventId enumEvent, string itemId)
	{
		return UnitOfWork.Formula.GetFormula(formCode, enumEvent, itemId);
	}

	[HttpGet]
	public DataTable GetCategory()
	{
		return UnitOfWork.Formula.GetCategory();
	}

	[HttpPost]
	public string DeleteFormula([FromBody] Tuple<string, string, Enums.EventId> model)
	{
		return UnitOfWork.Formula.DeleteFormula(model.Item1, model.Item2, model.Item3);
	}
}
