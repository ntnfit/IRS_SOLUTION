using System;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.ImportData;

public interface IImportItemMasterData
{
	DataTable GetTableColumns(string tableName);

	DataTable CheckStatus(Common.Table_Code tb);

	string SaveAll(Tuple<DataTable, string> tup);

	string Save(DataTable docs);

	DataTable GetColumnNam(string tableName);
}
