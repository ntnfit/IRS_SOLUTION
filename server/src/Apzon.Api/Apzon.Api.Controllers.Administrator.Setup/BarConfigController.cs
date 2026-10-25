using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;

namespace Apzon.Api.Controllers.Administrator.Setup;

public class BarConfigController : BaseApiController
{
	[HttpGet]
	public DataTable GetData()
	{
		try
		{
			DataTable data = UnitOfWork.BarConfig.GetData();
			if (data == null || data.Rows.Count == 0)
			{
				return UnitOfWork.BarConfig.GetColumns();
			}
			return data;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpPost]
	public string Save([FromBody] DataTable dt)
	{
		return UnitOfWork.BarConfig.Save(dt);
	}
}
