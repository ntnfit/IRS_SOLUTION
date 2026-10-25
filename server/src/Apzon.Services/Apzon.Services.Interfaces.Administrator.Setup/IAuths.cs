using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IAuths
{
	DataTable GetAllAuth(string user, string lngCode = "");

	DataTable DMSGetAllAuth(string user);

	string SaveDocs(DataTable lstDoc);

	DataTable GetFormAuth(string user, string form);

	DataTable GetAllAuthForm(string user, string lngCode, string superUser);

	string CopyAuth(List<string> lstUser);

	string CopyAuthDMS(List<string> lstUser);

	DataTable GetUserForCopy(string user);

	string InsertFormsToAuthTbl(DataTable dt);

	DataTable GetOwnerShipAuthorization(int empId);

	string SaveData(DataTable dt);

	string SaveDMSAuthorization(DataTable dt);

	string SaveOwnerShip(DataTable dt);

	string CheckExistsMemu();

	HttpResult CopyOwner(List<int> lstEmp);
}
