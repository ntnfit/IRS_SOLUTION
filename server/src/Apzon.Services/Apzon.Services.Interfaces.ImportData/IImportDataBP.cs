using System;
using System.Collections.Generic;
using System.Data;

namespace Apzon.Services.Interfaces.ImportData;

public interface IImportDataBP
{
	DataTable GetTableColumns(string tableName);

	DataTable CheckData(Tuple<string, DataTable, List<Tuple<string, Type>>> tupleCheck);

	string SaveData(Tuple<string, DataTable, List<Tuple<string, Type>>> tupleSave);
}
