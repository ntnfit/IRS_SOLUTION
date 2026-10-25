using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface IUserTablesDefined
{
	string SaveData(DataTable dt, string sessionId);
}
