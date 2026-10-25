using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Commons;

public class SalaryFormulaController : BaseApiController
{
	[HttpGet]
	public DataSet GetData(string sql)
	{
		return UnitOfWork.Formula.GetData(sql);
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataTable model)
	{
		return UnitOfWork.SalaryFormula.Save(model);
	}

	[HttpGet]
	public string GetFormulaText(string formCode, Enums.EventId enumEvent, int branch, int dept)
	{
		return UnitOfWork.SalaryFormula.GetFormulaText(formCode, enumEvent, branch, dept);
	}

	[HttpGet]
	public string GetFormularText(string formCode, Enums.EventId enumEvent, int branch, int dept)
	{
		return UnitOfWork.SalaryFormula.GetFormulaText(formCode, enumEvent, branch, dept);
	}

	[HttpGet]
	public DataTable GetFormula(string formCode, Enums.EventId enumEvent, int branch, int dept)
	{
		return UnitOfWork.SalaryFormula.GetFormula(formCode, enumEvent, branch, dept);
	}

	[HttpGet]
	public DataTable GetCategory()
	{
		return UnitOfWork.Formula.GetCategory();
	}

	[HttpPost]
	public string DeleteFormula([FromBody] Tuple<string, Enums.EventId, int, int> model)
	{
		return UnitOfWork.SalaryFormula.DeleteFormula(model.Item1, model.Item2, model.Item3, model.Item4);
	}
}
