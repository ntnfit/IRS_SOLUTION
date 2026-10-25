using System.Collections.Generic;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Api.Controllers.Purchase;

public class PurchaseRequestControlController : BaseApiController
{
	[HttpGet]
	public List<UserFieldsImpl> GetListField(string tableName)
	{
		return UnitOfWork.PurchaseRequestControl.GetListField(tableName);
	}
}
