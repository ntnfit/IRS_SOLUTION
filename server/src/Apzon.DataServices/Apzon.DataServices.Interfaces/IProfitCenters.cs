using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface IProfitCenters : IRepository<DataTable>
{
	string Save(string sessionId, DataTable dt);

	string Remove(string sessionId, string prcCode);
}
