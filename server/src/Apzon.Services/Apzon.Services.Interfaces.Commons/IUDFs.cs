using System;
using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Services.Interfaces.Commons;

public interface IUDFs
{
	DataTable GetTableLinked();

	List<Common.Table_Code> GetDefaultValueUdf(string tableName);

	List<Common.Table_Code> GetValidValue(string tableId, string fieldId);

	List<Common.Table_Code> GetLinkedTable(string linkedTable, string userName, int branch);

	DataTable GetKeyFieldOUDO(string tblName);

	DataTable LoadDataOUTB();

	string CheckExistUFD1(Tuple<string, int, int> tup);

	DataTable GetListUdf(string table);

	DataTable GetListCUFD(string tableName);

	DataTable GetAddNewRow(string tableName);

	DataTable GetListUDFFieldSQL(string tableName);

	string RemoveUdf(Tuple<string, int, string> tupe);

	string RemoveUFD1(Tuple<string, int, int> tupe);

	DataTable GetObject();

	DataTable GetUfd1(string tableCode, int fieldId);

	DataTable GetUkd1(string tableName, int keyId);

	DataTable GetUKD1ByTable(string tableName);

	DataTable GetOUKD(string tableName);

	List<UserFieldsImpl> GetListField(string tableName);

	DataTable GetListKeyAlias(string tableName);

	DataTable GetStructureUKD1();

	DataTable GetStructureUFD1();

	HttpResult Save(DataSet ds);

	DataTable CheckEnumConnect(string dbName);

	HttpResult UpdateDataUdf(UserFieldsImpl udfField, QueryParamater keyCondition, string tableName);

	List<string> GetListFieldNotValidated(string tableID);
}
