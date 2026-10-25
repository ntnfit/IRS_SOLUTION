using System;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Api.Controllers.Administrator.Setup.General;

[ValidateLoginSapExceptionHandling]
public class ContactUsController : BaseApiController
{
	[HttpPost]
	public ResponseResult Add([FromBody] ContactUsImpl entities)
	{
		try
		{
			if (entities == null)
			{
				return new ResponseResult
				{
					Message = "Data null"
				};
			}
			if (string.IsNullOrEmpty(SessionId))
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = "INVALIDSESSION"
				};
			}
			HttpResult httpResult = UnitOfWork.ContactUs.Save(entities);
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Code = httpResult.Content
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = ex.Message
			};
		}
	}
}
