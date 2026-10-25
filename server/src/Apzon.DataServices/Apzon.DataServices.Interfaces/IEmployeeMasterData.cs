using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IEmployeeMasterData : IRepository<DataTable>
{
	HttpResult Save(DataSet data, string sessionId);

	string Remove(int empID, string sessionId);

	string SaveListHeader(DataTable dt, string sessionId);
}
