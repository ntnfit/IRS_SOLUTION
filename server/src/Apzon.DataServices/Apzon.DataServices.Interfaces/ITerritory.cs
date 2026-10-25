using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface ITerritory : IRepository<DataTable>
{
	string SaveData(DataTable dt, string sessionId);
}
