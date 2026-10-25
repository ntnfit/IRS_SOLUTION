using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Service.MarketingCampaign;

public class MarketingCampaignController : BaseApiController
{
	[HttpPost]
	public DataTable LoadData([FromBody] Tuple<int, string, string, string, string> model)
	{
		return UnitOfWork.MarketingCampaign.LoadData(model.Item1, model.Item2, model.Item3, model.Item4, model.Item5);
	}

	[HttpGet]
	public DataSet GetDataById(int Id)
	{
		return UnitOfWork.MarketingCampaign.GetDataById(Id);
	}

	[HttpGet]
	public DataTable GetDataTargetBPById(int Id)
	{
		return UnitOfWork.MarketingCampaign.GetDataTargetBPById(Id);
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		return UnitOfWork.MarketingCampaign.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.MarketingCampaign.Save(ds, "U");
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string Id)
	{
		return UnitOfWork.MarketingCampaign.DeleteDocs(Id);
	}

	[HttpGet]
	public int GetMaxId()
	{
		return UnitOfWork.MarketingCampaign.GetMaxId();
	}

	[HttpGet]
	public DataTable FindNextLastFrist(int id, int type)
	{
		return UnitOfWork.MarketingCampaign.FindNextLastFrist(id, type);
	}

	[HttpGet]
	public DataTable LoadTargetGroup()
	{
		return UnitOfWork.MarketingCampaign.LoadTargetGroup();
	}

	[HttpGet]
	public DataTable LoadTargetGroupDetails(string code)
	{
		return UnitOfWork.MarketingCampaign.LoadTargetGroupDetails(code);
	}
}
