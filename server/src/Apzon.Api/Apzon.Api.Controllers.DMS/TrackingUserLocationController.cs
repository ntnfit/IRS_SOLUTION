using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Api.Controllers.DMS;

[ValidateLoginSapExceptionHandling]
public class TrackingUserLocationController : BaseApiController
{
	[HttpPost]
	public ResponseResult SaveAppSetting(DataTable dt)
	{
		try
		{
			return UnitOfWork.TrackingUserLocation.SaveAppSetting(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "406"
			};
		}
	}

	[HttpGet]
	public DataTable GetLastestAllEmployeeTrackingLocation(int userId = 0)
	{
		try
		{
			string userName = SessionUserInfo.UserName;
			return UnitOfWork.TrackingUserLocation.GetLastestAllEmployeeTrackingLocation(userName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public DataTable GetLastestEmployeeTrackingLocation(int empId)
	{
		try
		{
			return UnitOfWork.TrackingUserLocation.GetLastestEmployeeTrackingLocation(empId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpPost]
	public DataTable GetEmployeeRoute(DataTable data)
	{
		try
		{
			return UnitOfWork.TrackingUserLocation.GetEmployeeRoute(data);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public DataTable GetLastestUserTrackingLocation(int userId)
	{
		try
		{
			return UnitOfWork.TrackingUserLocation.GetLastestUserTrackingLocation(userId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpPost]
	public ResponseResult TrackingUserLocation(DataTable data)
	{
		try
		{
			HttpResult httpResult = UnitOfWork.TrackingUserLocation.TrackingUserLocation(data);
			if (httpResult.Message == "")
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
				MessageCode = "406"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "107"
			};
		}
	}
}
