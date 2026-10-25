using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator;

public class FilterSettingController : BaseApiController
{
	[HttpGet]
	public DataSet GetCategory()
	{
		try
		{
			return UnitOfWork.FilterSetting.GetCategory();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetFilter(string category)
	{
		try
		{
			return UnitOfWork.FilterSetting.GetFilter(category);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataTable GetFilterByCode(string code)
	{
		try
		{
			return UnitOfWork.FilterSetting.GetFilterByCode(code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpPost]
	public DataTable GetDataFilter([FromBody] Tuple<string, string> model)
	{
		try
		{
			return UnitOfWork.FilterSetting.GetDataFilter(model.Item1, model.Item2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpPost]
	public HttpResult SaveData([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.FilterSetting.Save(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpGet]
	public DataTable GetUserByFilter(string code)
	{
		try
		{
			return UnitOfWork.FilterSetting.GetUserByFilter(code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	public string SaveUserSign([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.FilterSetting.SaveUserSign(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpPost]
	public string RemoveData([FromBody] string code)
	{
		try
		{
			return UnitOfWork.FilterSetting.RemoveData(code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpGet]
	public DataTable GetFilterSelect(string userName, string objType)
	{
		try
		{
			return UnitOfWork.FilterSetting.GetFilterSelect(userName, objType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	public void InsertQueryFilter([FromBody] string username)
	{
		try
		{
			UnitOfWork.FilterSetting.InsertQueryFilter(username);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}
}
