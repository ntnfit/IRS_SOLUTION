using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IShippingTypes : IService<ShippingTypesImpl>
{
	List<ShippingTypesImpl> Cfl();

	DataTable GetShippingTypes();

	DataTable ShowShipping();

	DataTable Search(string name);

	int GetMaxCode();

	bool Check(int trnspCode);

	HttpResult SaveDoc(DataTable dt);

	HttpResult DeleteDoc(int trnspCode);

	DataTable CheckShippingOITM(int trnspCode);
}
