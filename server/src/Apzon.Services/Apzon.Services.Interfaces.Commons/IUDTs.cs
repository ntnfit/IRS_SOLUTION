using System;
using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Commons;

public interface IUDTs
{
	DataTable GetAllUDT();

	DataTable GetUDT(string table);

	string DeleteDoc(string table);

	string SaveDoc(UDTsImpl doc);

	string UpdateUdt(DataTable dtDoc);

	List<UDTsImpl> LoadBarUdt();

	DataTable GetListUDT(string userName, string branch);

	DataTable LoadDoc(string tableName);

	DataTable AddNEW(string tableName);

	string Save(Tuple<DataTable, string> lstTuple);
}
