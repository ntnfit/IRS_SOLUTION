using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.HumanResources;

public class HRMQualificationsController : BaseApiController
{
	[HttpGet]
	public DataTable GetAdvSource()
	{
		return UnitOfWork.HRMQualifications.GetAdvSource();
	}

	[HttpPost]
	public HttpResult SaveAdvSource([FromBody] DataTable dt)
	{
		return UnitOfWork.HRMQualifications.SaveAdvSource(dt);
	}

	[HttpPost]
	public string RemoveAdvSource([FromBody] int ID)
	{
		return UnitOfWork.HRMQualifications.RemoveAdvSource(ID);
	}

	[HttpGet]
	public DataTable GetJobType()
	{
		return UnitOfWork.HRMQualifications.GetJobType();
	}

	[HttpPost]
	public HttpResult SaveJobType([FromBody] DataTable dt)
	{
		return UnitOfWork.HRMQualifications.SaveJobType(dt);
	}

	[HttpPost]
	public string RemoveJobType([FromBody] int ID)
	{
		return UnitOfWork.HRMQualifications.RemoveJobType(ID);
	}

	[HttpGet]
	public DataTable GetQualificationsGroup()
	{
		return UnitOfWork.HRMQualifications.GetQualificationsGroup();
	}

	[HttpPost]
	public HttpResult SaveQualificationsGroup([FromBody] DataTable dt)
	{
		return UnitOfWork.HRMQualifications.SaveQualificationsGroup(dt);
	}

	[HttpPost]
	public string RemoveQualificationsGroup([FromBody] int ID)
	{
		return UnitOfWork.HRMQualifications.RemoveQualificationsGroup(ID);
	}

	[HttpGet]
	public int GetMaxDocEntry()
	{
		return UnitOfWork.GoodsReceipt.GetMaxDocEntry();
	}

	[HttpGet]
	public DataTable GetQualifications()
	{
		return UnitOfWork.HRMQualifications.GetQualifications();
	}

	[HttpPost]
	public HttpResult SaveQualifications([FromBody] DataTable dt)
	{
		return UnitOfWork.HRMQualifications.SaveQualifications(dt);
	}

	[HttpPost]
	public string RemoveQualifications([FromBody] int QuaCodes)
	{
		return UnitOfWork.HRMQualifications.RemoveQualifications(QuaCodes);
	}

	[HttpGet]
	public DataTable CheckQuaGroup(int QuaCodes)
	{
		return UnitOfWork.HRMQualifications.CheckQuaGroup(QuaCodes);
	}

	[HttpGet]
	public DataTable GetQuaCode(int quaId)
	{
		return UnitOfWork.HRMQualifications.GetQuaCode(quaId);
	}
}
