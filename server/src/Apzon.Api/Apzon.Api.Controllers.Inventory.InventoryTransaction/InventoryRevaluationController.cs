using System.Data;
using System.Web.Http;
using System.Web.Mvc;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Inventory;

namespace Apzon.Api.Controllers.Inventory.InventoryTransaction;

public class InventoryRevaluationController : BaseApiController
{
	[System.Web.Mvc.HttpGet]
	public int GetMaxDocEntry()
	{
		return UnitOfWork.InventoryRevaluation.GetMaxDocEntry();
	}

	[System.Web.Mvc.HttpPost]
	public DataTable Search([FromBody] InventoryRevaluationImpl doc)
	{
		return UnitOfWork.InventoryRevaluation.Search(doc);
	}

	[System.Web.Mvc.HttpGet]
	public DataTable GetLineMRV1(int idDocEntry)
	{
		return UnitOfWork.InventoryRevaluation.SearchLineMRV1(idDocEntry);
	}

	[System.Web.Mvc.HttpGet]
	public DataTable GetLineMRV2(int idDocEntry)
	{
		return UnitOfWork.InventoryRevaluation.SearchLineMRV2(idDocEntry);
	}

	[System.Web.Mvc.HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDoc([FromBody] InventoryRevaluationImpl doc)
	{
		return UnitOfWork.InventoryRevaluation.SaveDoc(doc);
	}

	[System.Web.Mvc.HttpGet]
	[MappingSap(IsMapping = false)]
	public string GetDelete(int docEntry, int lineNum)
	{
		return UnitOfWork.InventoryRevaluation.DeleteLineMRV1(docEntry, lineNum);
	}
}
