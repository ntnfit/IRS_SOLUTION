using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IORWClient
{
	Task<DataTable> GetTableNameOUDO();

	string SaveORW(DataSet data);

	string SaveORWUpdate(DataSet data);

	Task<string> RemoveObjectRegistration(string code);

	Task<DataTable> GetcolEditTableUDO1(int objtype, string colUDO1);

	Task<string> DeleteDT(string tableName);

	Task<DataSet> GetFieldsForFind(string tableName, string type);

	Task<DataSet> GetFieldsForForm(string tableName, string type);

	Task<DataSet> GetFieldsChild(string tableName);

	Task<HttpResult> Unregister(string code);

	Task<HttpResult> Delete(string code);
}
