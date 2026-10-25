using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

public interface IPaymentWizardClient
{
	Task<DataSet> GetData(DataTable dtSearch);

	Task<DataSet> GetPaymentInfo(DataTable dtSearch);

	Task<DataSet> GetDetails(DataTable dtSearch);

	Task<HttpResult> SaveDoc(DocumentsImpl doc, ApzObjectTypes formObjectType);
}
