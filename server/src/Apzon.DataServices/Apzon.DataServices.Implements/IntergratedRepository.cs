using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Documents;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class IntergratedRepository : Repository<DataTable>, IIntergrated
{
	private DocumentsRepository _DocRepository;

	public IntergratedRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
		_DocRepository = new DocumentsRepository(serviceUnitOfWork, unitOfWork, dataService);
	}

	public virtual async Task<string> Save(DocumentsImpl doc, string sessionId)
	{
		string message;
		try
		{
			message = (await _DocRepository.Save(doc, sessionId, (ApzObjectTypes)doc.ObjType)).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			message = ex.Message;
		}
		return message;
	}
}
