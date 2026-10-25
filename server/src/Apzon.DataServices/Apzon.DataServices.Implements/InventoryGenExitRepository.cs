using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Documents;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class InventoryGenExitRepository : Repository<DocumentsImpl>, IInventoryGenExit, IRepository<DocumentsImpl>, IDisposable
{
	public InventoryGenExitRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public async Task<string> Save(DocumentsImpl doc, string sessionId)
	{
		try
		{
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public void Dispose()
	{
	}

	public string Close(int docEntry, string sessionId)
	{
		throw new NotImplementedException();
	}

	public string Reopen(int docEntry, string sessionId)
	{
		throw new NotImplementedException();
	}

	public string Cancel(int docEntry, string sessionId)
	{
		throw new NotImplementedException();
	}

	public string Remove(int docEntry, string sessionId)
	{
		throw new NotImplementedException();
	}

	public bool Exists(int docEntry, string sessionId)
	{
		throw new NotImplementedException();
	}

	public string SaveAsDraft(DocumentsImpl doc, string sessionId)
	{
		throw new NotImplementedException();
	}
}
