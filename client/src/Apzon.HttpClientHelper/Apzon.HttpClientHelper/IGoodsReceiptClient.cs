using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

public interface IGoodsReceiptClient
{
	Task<HttpResult> SaveDoc(DocumentsImpl doc);

	Task<HttpResult> AddNew(DocumentsImpl doc);

	Task<HttpResult> SaveDocWithUdo(DocumentsImpl doc, DataSet ds);
}
