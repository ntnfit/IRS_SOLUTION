using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.DMS;

public interface IDMSSettingUDF
{
	HttpResult DMSGetDataUDF(int menuId, string tableName, string codeField, string code);

	HttpResult GetData(string tableName, int menuId);

	HttpResult Save(DataTable dt, string action);
}
