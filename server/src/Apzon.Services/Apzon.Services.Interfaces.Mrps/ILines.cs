using System.Data;

namespace Apzon.Services.Interfaces.Mrps;

public interface ILines
{
	DataTable GetLines(DataRow dataRow);

	DataSet GetALine(int id);

	string Save(DataSet ds);

	DataTable GetUsers();

	string DeleteData(int LineID);

	DataTable ExistsItemCodeInBranch(string ItemCode, int Branch, int LineID);

	DataTable ExistsLineCode(string LineCode, int LineID);

	DataTable ExistsLine(int LineID);
}
