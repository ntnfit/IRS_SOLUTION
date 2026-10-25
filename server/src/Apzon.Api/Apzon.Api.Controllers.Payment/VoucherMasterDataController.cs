using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Payment;

public class VoucherMasterDataController : BaseApiController
{
	[HttpGet]
	public DataTable GetAllVoucher()
	{
		return UnitOfWork.VoucherMasterData.GetAllVoucher();
	}

	[HttpGet]
	public DataTable GetDetailByCode(string pinCode)
	{
		return UnitOfWork.VoucherMasterData.GetDetailByCode(pinCode);
	}

	[HttpGet]
	public DataTable FindDataNextLast(string id, int type)
	{
		return UnitOfWork.VoucherMasterData.FindDataNextLast(id, type);
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		return UnitOfWork.VoucherMasterData.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.VoucherMasterData.Save(ds, "U");
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string ids)
	{
		return UnitOfWork.VoucherMasterData.DeleteDocs(ids);
	}
}
