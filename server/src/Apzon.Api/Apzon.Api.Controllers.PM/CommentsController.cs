using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.PM;

public class CommentsController : BaseApiController
{
	[HttpGet]
	public DataTable GetByObject(string objectType, string objectCode, string objectDetail)
	{
		return UnitOfWork.Comments.GetByObject(objectType, objectCode, objectDetail);
	}

	[HttpPost]
	public HttpResult AddNew([FromBody] DataTable dt)
	{
		if (string.IsNullOrEmpty(SessionId))
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
			return new HttpResult(Enums.MessageCode.Error, "INVALID SESSION ID");
		}
		string userName = SessionUserInfo.UserName;
		if (string.IsNullOrEmpty(userName))
		{
			return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
		}
		return UnitOfWork.Comments.AddNew(dt, userName);
	}
}
