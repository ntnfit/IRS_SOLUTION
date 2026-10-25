using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Api.Controllers.Promotion;

[ValidateLoginSapExceptionHandling]
public class PromotionController : BaseApiController
{
	[HttpGet]
	public ResponseResult GetSyncData()
	{
		try
		{
			ResponseResult responseResult = new ResponseResult();
			DataSet syncData = UnitOfWork.Promotion.GetSyncData();
			syncData.Tables[0].TableName = "ListPromotion";
			syncData.Tables[1].TableName = "RegisterPromotion";
			syncData.Tables[2].TableName = "ConditionPromotion";
			syncData.Tables[3].TableName = "BenefitPromotion";
			responseResult.Message = "";
			responseResult.Content = syncData;
			return responseResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message
			};
		}
	}

	[HttpGet]
	public DataTable GetListPromotions(int promotionType, string cardCode, string promotionCode, string groupCode)
	{
		try
		{
			return UnitOfWork.Promotion.GetListPromotions(promotionType, cardCode, promotionCode, groupCode, SessionUserInfo.UserName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable GetHeader(string code)
	{
		try
		{
			return UnitOfWork.Promotion.GetHeader(SessionUserInfo.UserName, code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable LoadCondition(string code)
	{
		try
		{
			return UnitOfWork.Promotion.LoadCondition(code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable LoadBenefit(string code)
	{
		try
		{
			return UnitOfWork.Promotion.LoadBenefit(code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataSet GetDetailByCode(string code, string groupCode = "")
	{
		try
		{
			return UnitOfWork.Promotion.GetDetailByCode(code, groupCode, SessionUserInfo.UserName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataSet();
		}
	}

	[HttpPost]
	public HttpResult Register([FromBody] DataSet model)
	{
		try
		{
			string userName = SessionUserInfo.UserName;
			return UnitOfWork.PromotionRegister.Save(model, userName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public ResponseResult GetAttachment([FromBody] DataTable dt)
	{
		try
		{
			string userName = SessionUserInfo.UserName;
			DataTable attachment = UnitOfWork.PromotionRegister.GetAttachment(dt, userName);
			return new ResponseResult
			{
				Message = "",
				Content = attachment,
				MessageCode = "200"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "401"
			};
		}
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataSet model)
	{
		try
		{
			return UnitOfWork.Promotion.Save(model, SessionUserInfo.UserName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public DataTable CheckPromotion([FromBody] DataTable input)
	{
		try
		{
			return UnitOfWork.Promotion.CheckPromotion(input);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	[HttpPost]
	public HttpResult SaveAppliedBenefit(DataTable model)
	{
		try
		{
			return UnitOfWork.CheckPromotion.Save(model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpGet]
	public DataTable GetExistDocPromotion(string guid)
	{
		try
		{
			return UnitOfWork.CheckPromotion.GetExistDocPromotion(guid);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}
}
