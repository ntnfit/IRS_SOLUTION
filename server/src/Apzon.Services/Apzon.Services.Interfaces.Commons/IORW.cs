using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Commons;

public interface IORW
{
	DataTable GetTableNameOUDO();

	DataTable GetTableChild(int objtype, string usedInObj);

	DataTable GetTableColumChild(string colchild);

	string SaveORWDataSet(DataSet ds, Enums.TransactionType transtype);

	HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum, string objCode = "");

	string RemoveObjectRegistration(string code);

	DataTable GetcolEditTableUDO1(int objtype, string colUDO1);

	string DeleteDT(string tableName);

	DataSet GetFieldsForFind(string tableName, string type);

	DataSet GetFieldsForForm(string tableName, string type);

	DataSet GetFieldsChild(string tableName);

	HttpResult Unregister(string code);

	HttpResult Delete(string code);
}
