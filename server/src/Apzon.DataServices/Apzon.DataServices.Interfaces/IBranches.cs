using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IBranches
{
	HttpResult SaveData(string sessionId, DataTable doc);
}
