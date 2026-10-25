using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

public interface IStockTransferClient
{
	Task<HttpResult> SaveDoc(DocumentsImpl doc);

	Task<HttpResult> SaveTransferBin(DocumentsImpl doc, DataSet ds);

	Task<HttpResult> SaveAsDraft(DocumentsImpl doc);
}
