using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Inventory;

namespace Apzon.Api.Controllers.Financials;

public class MaterialRevaluationController : BaseApiController
{
	[HttpGet]
	public DataTable CostingTable(string period)
	{
		return UnitOfWork.MaterialRevaluation.CostingTable(period);
	}

	[HttpPost]
	public string SaveDoc([FromBody] MaterialRevaluation doc)
	{
		return UnitOfWork.MaterialRevaluation.Add(doc);
	}

	[HttpPost]
	public string AddDt([FromBody] MaterialRevaluation doc, [FromBody] DataTable dt)
	{
		return UnitOfWork.MaterialRevaluation.AddDt(doc, dt);
	}

	[HttpPost]
	public string AddPart([FromBody] MaterialRevaluation doc, [FromBody] List<MaterialRevaluation_linesImpl> list)
	{
		return UnitOfWork.MaterialRevaluation.AddPart(doc, list);
	}

	[HttpGet]
	public DataTable LoadCBPeriod()
	{
		return UnitOfWork.MaterialRevaluation.LoadCBPeriod();
	}

	[HttpGet]
	public DataTable LastDate(string period)
	{
		return UnitOfWork.MaterialRevaluation.LastDate(period);
	}

	[HttpGet]
	public DataTable Costing(string period)
	{
		return UnitOfWork.MaterialRevaluation.Costing(period);
	}

	[HttpGet]
	public DataTable Cogs(string period)
	{
		return UnitOfWork.MaterialRevaluation.Cogs(period);
	}

	[HttpGet]
	public DataTable KCSX(string period)
	{
		return UnitOfWork.MaterialRevaluation.KCSX(period);
	}

	[HttpGet]
	public DataTable CostingAllocate(string period)
	{
		return UnitOfWork.MaterialRevaluation.CostingAllocate(period);
	}

	[HttpGet]
	public bool CheckRevaluation(string period)
	{
		return UnitOfWork.MaterialRevaluation.CheckRevaluation(period);
	}

	[HttpGet]
	public bool CheckCogs(string period)
	{
		return UnitOfWork.MaterialRevaluation.CheckCogs(period);
	}

	[HttpGet]
	public bool CheckProd(string period)
	{
		return UnitOfWork.MaterialRevaluation.CheckProd(period);
	}

	[HttpGet]
	public DataTable GetAll()
	{
		return UnitOfWork.MaterialRevaluation.GetAll();
	}
}
