using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Service;

public class WarrantyCardController : BaseApiController
{
	[HttpGet]
	public DataTable LoadData(int id, string warrantyNo, string itemCode, string bpCode, string status)
	{
		return UnitOfWork.WarrantyCard.LoadData(id, warrantyNo, itemCode, bpCode, status);
	}

	[HttpGet]
	public DataTable GetDataById(int Id)
	{
		return UnitOfWork.WarrantyCard.GetDataById(Id);
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		return UnitOfWork.WarrantyCard.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.WarrantyCard.Save(ds, "U");
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string Id)
	{
		return UnitOfWork.WarrantyCard.DeleteDocs(Id);
	}

	[HttpGet]
	public int GetMaxId()
	{
		return UnitOfWork.WarrantyCard.GetMaxId();
	}

	[HttpGet]
	public DataTable FindNextLastFrist(int id, int type)
	{
		return UnitOfWork.WarrantyCard.FindNextLastFrist(id, type);
	}
}
