using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class InterestLevelRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService) : Repository<DataTable>(unitOfWork, dataService), IInterestLevel, IDisposable
{
	private string _objectType = "oInterestLevel";

	public string Delete(int indCode, string sessionId)
	{
		try
		{
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public HttpResult Save(DataTable dt, string sessionId)
	{
		try
		{
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
