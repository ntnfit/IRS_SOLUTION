using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface IServiceContract : IRepository<DataTable>
{
	string Save(DataSet data, string sessionId);
}
