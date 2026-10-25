using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface ISolution : IRepository<DataSet>
{
	string Save(DataSet doc, string sessionId);

	string DeleteDoc(int sltCode, string sessionId);
}
