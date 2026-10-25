using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IProject
{
	HttpResult Save_OPRJ(DataTable dt);

	DataTable ShowData();

	DataTable GetUserSignByUserCode_tableOUSR(string madk);

	string CheckExistCode(string code);

	DataTable LadDataMrpProject(string LstProjectCode);

	string DeleteProject(string code);

	bool ExistProject(string code);

	DataTable GetStruct();
}
