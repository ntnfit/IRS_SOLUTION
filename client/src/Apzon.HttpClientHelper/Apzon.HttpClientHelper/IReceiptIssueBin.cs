using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

public interface IReceiptIssueBin
{
	Task<DataSet> GetData(string batch, string bin, ApzObjectTypes objtype);

	Task<HttpResult> SaveDoc(DocumentsImpl doc, ApzObjectTypes formObjectType);
}
