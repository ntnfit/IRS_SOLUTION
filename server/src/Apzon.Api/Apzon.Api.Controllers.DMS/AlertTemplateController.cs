using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.DMS;

public class AlertTemplateController : BaseApiController
{
	[HttpGet]
	public DataTable GetAllTemplate()
	{
		return UnitOfWork.AlertTemplate.GetAllTemplate();
	}

	[HttpPost]
	public HttpResult SaveData(Tuple<DataTable, string> tupData)
	{
		return UnitOfWork.AlertTemplate.SaveData(tupData.Item1, tupData.Item2);
	}

	[HttpPost]
	public HttpResult Remove(Tuple<string> tupData)
	{
		return UnitOfWork.AlertTemplate.Remove(tupData.Item1);
	}
}
