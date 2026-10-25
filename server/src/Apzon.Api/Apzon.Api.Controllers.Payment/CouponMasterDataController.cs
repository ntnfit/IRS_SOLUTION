using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Payment;

public class CouponMasterDataController : BaseApiController
{
	[HttpGet]
	public DataTable GetAllCoupon()
	{
		return UnitOfWork.CouponMasterData.GetAllCoupon();
	}

	[HttpGet]
	public DataTable GetDetailByCode(string couponCode)
	{
		return UnitOfWork.CouponMasterData.GetDetailByCode(couponCode);
	}

	[HttpGet]
	public DataTable FindDataNextLast(string id, int type)
	{
		return UnitOfWork.CouponMasterData.FindDataNextLast(id, type);
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		return UnitOfWork.CouponMasterData.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.CouponMasterData.Save(ds, "U");
	}

	[HttpGet]
	public DataTable GetDetail(string cashCode)
	{
		return UnitOfWork.CouponMasterData.GetDetailByCode(cashCode);
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string ids)
	{
		return UnitOfWork.CouponMasterData.DeleteDocs(ids);
	}
}
