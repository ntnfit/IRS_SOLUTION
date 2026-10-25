using System;
using System.Data;

namespace Apzon.Services.Interfaces.ImportData;

public interface IImportBOM
{
	DataTable GetTableColumns(string tableName);

	DataTable CheckStatus(Tuple<string, string, int> tup);

	DataTable CheckStatus2(Tuple<string, string> tup);

	string SaveAll(Tuple<DataTable, string> tup);

	DataTable GetColumnNam(string tableName);
}
