using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface IBatchNumberDetails
{
	string Save(DataSet ds, string sessionId);
}
