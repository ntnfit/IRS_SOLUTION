using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IFormWizardClient
{
	Task<DataTable> GetValid();

	Task<DataSet> GetById(int templateId);

	Task<DataTable> GetAll();

	Task<HttpResult> SaveData(DataSet ds);

	Task<HttpResult> Remove(int absEntry);
}
