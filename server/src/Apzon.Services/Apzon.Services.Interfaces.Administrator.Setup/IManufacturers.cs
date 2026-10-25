using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IManufacturers : IService<ManufacturersImpl>
{
	List<ManufacturersImpl> Cfl();

	DataTable GetManufacturers();

	DataTable ShowManufacturers();

	DataTable ShowCurrency();

	DataTable Search(string firmName);

	int GetMaxCode();

	HttpResult SaveDoc(DataTable doc);

	string DeleteDoc(int firmCode);

	DataTable CheckFirmCodeOITM(int firmCode);

	HttpResult Remove(DataTable dt);
}
