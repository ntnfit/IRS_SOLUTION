using System.Threading.Tasks;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

public interface IGoodsIssueClient
{
	Task<HttpResult> SaveDoc(DocumentsImpl doc);
}
