using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;

namespace Apzon.HttpClientHelper;

public interface IUdfClient
{
	Task<List<UserFieldsImpl>> GetListField(string tblName);

	Task<List<Common.Table_Code>> GetLinkedTable(string linkedTable, string username, int branch);

	DataTable GetListKeyAlias(string tblName);

	Task<HttpResult> UpdateDataUdf(UserFieldsImpl udfField, QueryParamater keyCondition, string tableName);

	Task<HttpResult> RefreshUDF();
}
