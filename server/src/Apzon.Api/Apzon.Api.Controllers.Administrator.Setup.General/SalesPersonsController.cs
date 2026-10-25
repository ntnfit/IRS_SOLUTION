using System.Collections.Generic;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Administrator;

namespace Apzon.Api.Controllers.Administrator.Setup.General;

public class SalesPersonsController : BaseApiController
{
	[HttpGet]
	public List<SalesPersonsImpl> Cfl()
	{
		return UnitOfWork.SalesPersons.Cfl();
	}
}
