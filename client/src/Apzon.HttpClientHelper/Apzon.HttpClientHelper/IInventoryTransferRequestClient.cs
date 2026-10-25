using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

public interface IInventoryTransferRequestClient
{
	Task<HttpResult> SaveDoc(DocumentsImpl doc);

	Task<DataSet> GetDataCopyFrom(string docEntry, ApzObjectTypes fromType);

	Task<HttpResult> SaveAsDraft(DocumentsImpl doc);
}
