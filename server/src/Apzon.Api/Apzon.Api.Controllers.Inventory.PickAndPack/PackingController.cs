using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Inventory.PickAndPack;

public class PackingController : BaseApiController
{
	[HttpPost]
	public DataTable GetDoc([FromBody] DataTable pr)
	{
		return UnitOfWork.Packing.GetDoc(pr);
	}

	[HttpGet]
	public DataTable GetLines(int id)
	{
		return UnitOfWork.Packing.GetLines(id);
	}

	[HttpGet]
	public int GetNextId()
	{
		return UnitOfWork.Packing.GetNextId();
	}

	[HttpGet]
	public DataTable GetLineByPickNo(int id)
	{
		return UnitOfWork.Packing.GetLineByPickNo(id);
	}

	[HttpGet]
	public DataTable GetLineByPackNo(int id)
	{
		return UnitOfWork.Packing.GetLineByPackNo(id);
	}

	[HttpGet]
	public DataSet ChoosePickList(string cardcode)
	{
		return UnitOfWork.Packing.ChoosePickList(cardcode);
	}

	[HttpGet]
	public DataSet ChoosePacking()
	{
		return UnitOfWork.Packing.ChoosePacking();
	}

	[HttpGet]
	public DataSet ChooseBatch()
	{
		return UnitOfWork.Packing.ChooseBatch();
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataSet ds)
	{
		return UnitOfWork.Packing.Save(ds);
	}

	[HttpPost]
	public HttpResult Close([FromBody] int docEntry)
	{
		return UnitOfWork.Packing.Close(docEntry);
	}

	[HttpPost]
	public HttpResult Cancel([FromBody] int docEntry)
	{
		return UnitOfWork.Packing.Cancel(docEntry);
	}
}
