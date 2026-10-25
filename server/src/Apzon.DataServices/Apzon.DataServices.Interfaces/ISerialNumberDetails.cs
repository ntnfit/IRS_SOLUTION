using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface ISerialNumberDetails
{
	string Save(DataTable dt, string sessionId);
}
