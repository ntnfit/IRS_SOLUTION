using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.DashBoard;

public interface IDashboardAuthorization
{
	DataTable GetAllAuth(string user);

	HttpResult SaveDoc(DataTable dt);

	DataTable GetFormAuth(string user, string form);

	string CopyAuth(List<string> lstUser);

	DataTable GetUserForCopy(string user);

	DataTable GetAllAuthForm(string user);
}
