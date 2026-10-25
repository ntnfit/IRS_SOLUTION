using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class UserObjectDefinedRepository : Repository<DataTable>, IUserObjectDefined, IDisposable
{
	public UserObjectDefinedRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	protected virtual string Mapping(DataSet ds, EnumCommandType commandType, string sessionId)
	{
		try
		{
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public virtual string SaveData(DataSet ds, EnumCommandType commandType, string sessionId)
	{
		throw new NotImplementedException();
	}

	public virtual HttpResult Delete(string code, string sessionId)
	{
		throw new NotImplementedException();
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
