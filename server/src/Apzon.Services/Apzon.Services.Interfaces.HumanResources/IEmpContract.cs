using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.HumanResources;

public interface IEmpContract
{
	DataTable LoadDoc(int empID);

	DataTable LoadContractTypes();

	DataTable CheckContractType(int typeID);

	string SaveContract(DataTable dt);

	string DeleteContract(DataTable dt);

	HttpResult SaveData(DataTable dt, Enums.TransactionType transtype = Enums.TransactionType.A);
}
