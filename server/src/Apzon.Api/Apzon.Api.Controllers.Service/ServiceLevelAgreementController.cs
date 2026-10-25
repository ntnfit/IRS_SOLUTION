using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Service;

public class ServiceLevelAgreementController : BaseApiController
{
	[HttpGet]
	public DataTable LoadData(int id, string SLANo, string proService)
	{
		return UnitOfWork.ServiceLevelAgreement.LoadData(id, SLANo, proService);
	}

	[HttpGet]
	public DataSet GetDataById(int Id)
	{
		return UnitOfWork.ServiceLevelAgreement.GetDataById(Id);
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		return UnitOfWork.ServiceLevelAgreement.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.ServiceLevelAgreement.Save(ds, "U");
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string Id)
	{
		return UnitOfWork.ServiceLevelAgreement.DeleteDocs(Id);
	}

	[HttpGet]
	public int GetMaxId()
	{
		return UnitOfWork.ServiceLevelAgreement.GetMaxId();
	}

	[HttpGet]
	public DataTable FindNextLastFrist(int id, int type)
	{
		return UnitOfWork.ServiceLevelAgreement.FindNextLastFrist(id, type);
	}
}
