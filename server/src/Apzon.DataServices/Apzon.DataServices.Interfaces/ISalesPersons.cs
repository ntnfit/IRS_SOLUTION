using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface ISalesPersons : IRepository<DataTable>
{
	string Save(string sessionId, DataTable dt);

	HttpResult AddDoc(string sessionId, DataTable dt);

	string Remove(string sessionId, int slpCode);
}
