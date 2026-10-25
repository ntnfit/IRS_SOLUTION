using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IPublishServiceClient
{
	Task<HttpResult> Save(DataSet ds);

	Task<DataTable> Search(string controllerName, string actionName, string description);

	Task<DataSet> GetById(int choosedEntry);

	Task<int> Navigate(int absEntry, Enums.NavigateType last);

	Task<HttpResult> Remove(int absEntry);
}
