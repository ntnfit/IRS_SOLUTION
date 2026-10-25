using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.HumanResources;

public class JVApplicantsController : BaseApiController
{
	[HttpGet]
	public DataSet LoadDoc(int id, string appNo)
	{
		return UnitOfWork.JVApplicants.LoadDoc(id, appNo);
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataSet dt)
	{
		return UnitOfWork.JVApplicants.Save(dt);
	}

	[HttpGet]
	public DataTable CheckApplicant(string id)
	{
		return UnitOfWork.JVApplicants.CheckApplicant(id);
	}

	public int GetMaxIdApplicant()
	{
		return UnitOfWork.JVApplicants.GetMaxIdApplicant();
	}
}
