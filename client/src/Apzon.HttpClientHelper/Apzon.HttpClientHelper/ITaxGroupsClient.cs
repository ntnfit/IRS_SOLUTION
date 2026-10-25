using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface ITaxGroupsClient
{
	string CheckCode(string code);

	bool Exists(string code);

	string Delete(string code);

	Task<DataTable> GetDataOVTG(string vatType);

	HttpResult SaveVat(DataSet ds);

	Task<DataTable> GetAllLine_VTG1();

	Task<DataTable> GetStructHeader();

	Task<DataTable> GetStructLines();
}
