using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Documents;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaIntergratedRepository : IntergratedRepository
{
	private HanaDocumentsRepository _DocRepository;

	public HanaIntergratedRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
		_DocRepository = new HanaDocumentsRepository(serviceUnitOfWork, unitOfWork, dataService);
	}

	public override async Task<string> Save(DocumentsImpl doc, string sessionId)
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
