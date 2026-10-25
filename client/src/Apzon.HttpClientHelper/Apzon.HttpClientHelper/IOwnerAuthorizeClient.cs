using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IOwnerAuthorizeClient
{
	Task<DataTable> GetOwnerShipAuthorization(int empId);

	Task<string> Save(DataTable dt);

	bool CheckExistsMemu();

	Task<HttpResult> CopyOwner(List<int> lstEmp);
}
