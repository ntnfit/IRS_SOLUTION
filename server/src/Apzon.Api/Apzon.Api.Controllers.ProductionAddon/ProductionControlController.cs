using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.Production_Addon;

namespace Apzon.Api.Controllers.ProductionAddon;

public class ProductionControlController : BaseApiController
{
	[HttpGet]
	public DataSet GetDocs(string cardcode, string cardname, string fromdate, string todate)
	{
		return UnitOfWork.ProductionControl.GetDocs(cardcode, cardname, fromdate, todate);
	}

	[HttpPost]
	public DataTable GetDocsTabPlaned(ProductionOder_Impl_OWOR po)
	{
		return UnitOfWork.ProductionControl.GetDocsTabPlaned(po);
	}

	[HttpPost]
	public DataTable GetDocsWhenProPlan([FromBody] string ids)
	{
		return UnitOfWork.ProductionControl.GetDocsWhenProPlan(ids);
	}

	[HttpPost]
	public DataTable Search(ProductionControlImpl obj)
	{
		return UnitOfWork.ProductionControl.Search(obj);
	}

	[HttpPost]
	public DataTable ProPlanning(DataTable doc)
	{
		if (Global.LoginSap)
		{
			string.IsNullOrEmpty(SessionId);
			return new DataTable();
		}
		return new DataTable();
	}
}
