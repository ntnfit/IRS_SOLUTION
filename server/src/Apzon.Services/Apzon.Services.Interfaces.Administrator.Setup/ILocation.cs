using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface ILocation
{
	List<LocationImpl> Cfl();

	DataTable GetLocation();

	DataTable ShowLocation();

	DataTable Search(LocationImpl doc);

	int GetMaxCode();

	bool Check(int code);

	string SaveDoc(LocationImpl doc);

	string DeleteDoc(int code);

	DataTable LoadLocation();

	DataTable CheckExistLocationCode(int locCode);

	HttpResult SaveOLCT(DataTable dtOLCT);

	string GetLocationNameByCode(int code);
}
