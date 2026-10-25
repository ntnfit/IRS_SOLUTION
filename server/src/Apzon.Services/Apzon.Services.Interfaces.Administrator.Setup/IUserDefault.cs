using System.Data;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IUserDefault
{
	DataTable GetOUDG(string code);

	DataTable GetUDG1(string code, string type);

	DataTable GetUDG2(string code);

	string Save(DataSet ds);

	string Remove(string code);

	DataTable Search(DataTable dtFind);

	DataTable GetTableColumns(string tableName);

	DataTable GetRecord(string tbl, string key, string TypeRecord);
}
