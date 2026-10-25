using System.Data;
using Apzon.Entities.Models.Documents;

namespace Apzon.DataServices.Interfaces;

public interface ISalesBlanketAgreement : IRepository<DocumentSBAImpl>
{
	string Save(string sessionId, DataSet dtset);

	string Cancel(int docEntry, string sessionId);
}
