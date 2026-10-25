using System;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.MasterData;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class OCYCRepository : Repository<OCYC_Impl>, IOCYC, IRepository<OCYC_Impl>, IDisposable
{
	public OCYCRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public string Save(OCYC_Impl item, string sessionId)
	{
		try
		{
			if (!Exists(item.Code, sessionId))
			{
				return Add(item, sessionId);
			}
			return Update(item, sessionId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string Update(OCYC_Impl item, string sessionId)
	{
		string empty = string.Empty;
		try
		{
			return "Erro";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return empty;
		}
	}

	private string Add(OCYC_Impl item, string sessionId)
	{
		string empty = string.Empty;
		try
		{
			return "Erro";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return empty;
		}
	}

	private bool Exists(int p, string sessionId)
	{
		return false;
	}

	public void Dispose()
	{
	}
}
