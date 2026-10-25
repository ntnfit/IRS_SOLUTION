using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Production_Addon;

namespace Apzon.Api.Controllers.ProductionAddon;

[ValidateLoginSapExceptionHandling]
public class ApzBillOfMaterialsController : BaseApiController
{
	[HttpPost]
	public DataTable SearchBom([FromBody] ApzBillOfMaterials bom)
	{
		return UnitOfWork.ApzBillOfMaterials.Search(bom);
	}

	[HttpPost]
	public DataTable SearchLines(ApzBillOfMaterials bom)
	{
		return UnitOfWork.ApzBillOfMaterials.SearchLines(bom);
	}

	[HttpPost]
	public string SaveBom(DataSet ds)
	{
		return UnitOfWork.ApzBillOfMaterials.SaveBoms(ds);
	}

	[HttpPost]
	public string DeleteBom([FromBody] ApzBillOfMaterials bom)
	{
		return UnitOfWork.ApzBillOfMaterials.DeleteBom(bom);
	}

	[HttpGet]
	public DataTable GetList_BOM()
	{
		return UnitOfWork.ApzBillOfMaterials.GetList_BOM();
	}
}
