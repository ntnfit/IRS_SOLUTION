using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface IResourceGroup
{
	string Save(DataTable dt, string sessionId);
}
