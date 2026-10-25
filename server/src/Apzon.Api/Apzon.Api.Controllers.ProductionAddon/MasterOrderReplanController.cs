using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.ProductionAddon;

[ValidateLoginSapExceptionHandling]
public class MasterOrderReplanController : BaseApiController
{
	[HttpPost]
	public DataTable Search([FromBody] DataTable dt)
	{
		string moNo = string.Empty;
		DateTime fromDate = DateTime.Today;
		DateTime toDate = DateTime.Today;
		if (dt != null && dt.Rows.Count > 0)
		{
			if (dt.Rows[0].Table.Columns.Contains("MONo"))
			{
				moNo = ((dt.Rows[0]["MONo"] != null) ? dt.Rows[0]["MONo"].ToString() : "");
			}
			if (dt.Rows[0].Table.Columns.Contains("FromDate"))
			{
				fromDate = Convert.ToDateTime(dt.Rows[0]["FromDate"]);
			}
			if (dt.Rows[0].Table.Columns.Contains("ToDate"))
			{
				toDate = Convert.ToDateTime(dt.Rows[0]["ToDate"]);
			}
		}
		return UnitOfWork.MasterOrderReplan.Search(moNo, fromDate, toDate);
	}

	[HttpPost]
	public string Save(DataTable dt)
	{
		return UnitOfWork.MasterOrderReplan.Save(dt);
	}
}
