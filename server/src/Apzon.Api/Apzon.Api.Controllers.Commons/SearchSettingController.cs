using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;

namespace Apzon.Api.Controllers.Commons;

public class SearchSettingController : BaseApiController
{
	[HttpGet]
	public DataTable GetSearchSetting(string formId, string itemId, string user)
	{
		return UnitOfWork.SearchSetting.GetSearchSetting(formId, itemId, user);
	}

	[HttpPost]
	public string SaveData([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.SearchSetting.SaveData(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
