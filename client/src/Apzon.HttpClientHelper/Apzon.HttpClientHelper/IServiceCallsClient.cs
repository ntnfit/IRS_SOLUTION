using System;
using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IServiceCallsClient
{
	Task<DataTable> LoadServiceCalls(string cardCode, int handleBy, int callType, int callId, string tag, DateTime fromDate, DateTime toDate);

	Task<DataTable> GetContactPersonAsync(string cardCode);

	DataTable GetContactPerson(string cardCode);

	Task<DataTable> GetSolutionByCallId(int callId);

	Task<DataTable> GetDocumentByCallId(int callId);

	Task<string> SaveDoc(DataSet dt);

	Task<string> Remove(int callId);
}
