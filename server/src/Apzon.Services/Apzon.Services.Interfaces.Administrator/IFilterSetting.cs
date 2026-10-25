using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator;

public interface IFilterSetting
{
	DataSet GetCategory();

	DataSet GetFilter(string category);

	DataTable GetFilterByCode(string code);

	DataTable GetDataFilter(string datafilter, string userCode);

	HttpResult Save(DataTable dt);

	string SaveUserSign(DataTable dt);

	DataTable GetUserByFilter(string code);

	string RemoveData(string code);

	DataTable GetFilterSelect(string userName, string objType);

	string GetFilterText(string filter);

	void InsertQueryFilter(string username);
}
