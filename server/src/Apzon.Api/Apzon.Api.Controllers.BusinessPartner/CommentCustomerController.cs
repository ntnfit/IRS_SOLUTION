using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Api.Controllers.BusinessPartner;

[ValidateLoginSapExceptionHandling]
public class CommentCustomerController : BaseApiController
{
	[HttpGet]
	public ResponseResult GetDataCommentCustomer(string relatedEnt)
	{
		try
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = "INVALIDSESSION"
				};
			}
			string userName = SessionUserInfo.UserName;
			if (string.IsNullOrEmpty(userName))
			{
				return new ResponseResult
				{
					Message = "User name invalid"
				};
			}
			DataTable userSign = UnitOfWork.Users.GetUserSign(userName);
			DataTable dataCommentCustomer = UnitOfWork.CommentCustomer.GetDataCommentCustomer(Function.ParseInt(userSign.Rows[0]["USERID"]), relatedEnt, "oBusinessPartners");
			dataCommentCustomer.TableName = "Commet";
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Table0 = dataCommentCustomer
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

	[HttpPost]
	public ResponseResult SaveData([FromBody] DataTable dt)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SessionId",
					MessageCode = "500"
				};
			}
			HttpResult httpResult = UnitOfWork.CommentCustomer.Save(dt);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				return new ResponseResult
				{
					Message = "",
					MessageCode = "200"
				};
			}
			return new ResponseResult
			{
				Message = httpResult.Message,
				MessageCode = "400"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "Lỗi 107",
				Content = new
				{
					Table0 = ex.Message
				}
			};
		}
	}
}
