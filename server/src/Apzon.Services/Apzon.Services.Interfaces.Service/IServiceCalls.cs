using System;
using System.Data;

namespace Apzon.Services.Interfaces.Service;

public interface IServiceCalls
{
	DataTable GetServiceCalls(string cardCode, int handleBy, int callType, int callId, string tag, DateTime? fromDate, DateTime? toDate);

	DataTable GetContactPerson(string cardCode);

	DataTable GetSolutionByCallId(int callId);

	DataTable GetDocumentByCallId(int callId);

	string RemoveDoc(int callId);

	string SaveDoc(DataSet dt);
}
