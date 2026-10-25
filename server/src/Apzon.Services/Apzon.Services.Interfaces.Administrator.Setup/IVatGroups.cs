using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IVatGroups : IService<VatGroupsImpl>
{
	List<VatGroupsImpl> Cfl();

	List<VatGroupsImpl> Cfl(string category);

	DataTable GetVatGroupI();

	DataTable GetVatGroupO();

	DataTable GetRate(VatGroupsImpl doc);

	DataTable ShowVatGroupI();

	DataTable ShowVatGroupO();

	DataTable SearchVatGroupI(VatGroupsImpl doc);

	DataTable SearchVatGroupO(VatGroupsImpl doc);

	DataTable ShowRateByCodeVatGroup(string codeVatGroup);

	DataTable GetData();

	string CheckCode(string key);

	DataTable GetAllLine_VTG1();

	HttpResult SaveVat(DataSet ds);

	DataTable GetDataOVTG(string vatType);

	bool Exists(string code);

	string Delete(string code);

	DataTable GetStructHeader();

	DataTable GetStructLines();
}
