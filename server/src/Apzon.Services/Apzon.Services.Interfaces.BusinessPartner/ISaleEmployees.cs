using System.Data;
using Apzon.Entities.Models.BusinessPartners;

namespace Apzon.Services.Interfaces.BusinessPartner;

public interface ISaleEmployees : IService<SaleEmployeesImpl>
{
	DataTable ShowSaleEmployee();

	int GetMaxCode();

	string SaveDoc(SaleEmployeesImpl doc);

	string DeleteDoc(int code);
}
