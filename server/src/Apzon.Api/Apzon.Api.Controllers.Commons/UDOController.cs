using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Commons;

public class UDOController : BaseApiController
{
	[HttpGet]
	public DataTable GetListUdo()
	{
		return UnitOfWork.UDOs.GetListUdo();
	}

	[HttpPost]
	public DataTable GetUdoByCode([FromBody] Common.Table_Code item)
	{
		return UnitOfWork.UDOs.GetUdoByCode(item);
	}

	[HttpGet]
	public string GetTypeByCode(string code)
	{
		return UnitOfWork.UDOs.GetTypeByCode(code);
	}

	[HttpPost]
	public DataTable GetUdoByCodeDMS([FromBody] string code)
	{
		Common.Table_Code item = new Common.Table_Code
		{
			Name = "OUDO",
			Code = code
		};
		return GetUdoByCode(item);
	}

	[HttpGet]
	public DataTable GetUdo1ByCode(string code)
	{
		return UnitOfWork.UDOs.GetUdo1ByCode(code);
	}

	[HttpPost]
	public DataSet GetData([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.UDOs.GetData(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpPost]
	public HttpResult GetPage([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.UDOs.GetPage(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public DataTable GetDataByDocEntry([FromBody] Common.Table_Code item)
	{
		return UnitOfWork.UDOs.GetDataByDocEntry(item);
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataSet ds)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult
					{
						Message = "INVALIDSESSION",
						Content = null
					};
				}
				return BrokerService.GetUnitOfWork(SessionId).UDOs.Save(ds, SessionUserInfo.SessionDiServer);
			}
			return UnitOfWork.UDOs.Save(ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpPost]
	public string DeleteDoc([FromBody] DataSet ds)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return "Invalid session";
				}
				return BrokerService.GetUnitOfWork(SessionId).UDOs.DeleteDoc(ds, SessionUserInfo.SessionDiServer);
			}
			return UnitOfWork.UDOs.Remove(ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpPost]
	public string CancelDoc([FromBody] DataSet ds)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return "Invalid session";
				}
				return BrokerService.GetUnitOfWork(SessionId).UDOs.CancelDoc(ds, SessionUserInfo.SessionDiServer);
			}
			return UnitOfWork.UDOs.Cancel(ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpPost]
	public string CloseDoc([FromBody] DataSet ds)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return "Invalid session";
				}
				return BrokerService.GetUnitOfWork(SessionId).UDOs.CloseDoc(ds, SessionUserInfo.SessionDiServer);
			}
			return UnitOfWork.UDOs.Close(ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpGet]
	public int GetMaxId(string code)
	{
		try
		{
			return UnitOfWork.UDOs.GetMaxId(code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	[HttpGet]
	public DataTable GetSeriesByUser(string udoCode)
	{
		return UnitOfWork.UDOs.GetSeriesByUser(udoCode, Global.SessionUserInfo[SessionId].UserName);
	}

	[HttpGet]
	public string GetNextNumber(string udoCode, int series)
	{
		return UnitOfWork.UDOs.GetNextNumber(udoCode, series);
	}
}
