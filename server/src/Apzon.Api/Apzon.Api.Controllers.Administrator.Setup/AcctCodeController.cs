using System.Collections.Generic;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.MasterData;

namespace Apzon.Api.Controllers.Administrator.Setup;

public class AcctCodeController : BaseApiController
{
	[HttpGet]
	public List<AcctCodeImpl> Cfl(string accountType)
	{
		return UnitOfWork.AcctCode.Cfl(accountType);
	}
}
