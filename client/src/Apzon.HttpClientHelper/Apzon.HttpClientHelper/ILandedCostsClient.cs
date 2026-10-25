using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.LandedCosts;

namespace Apzon.HttpClientHelper;

public interface ILandedCostsClient
{
	Task<DataTable> SearchData(string fromDate, string toDate, string vendor, string broker, int docNum);

	Task<DataSet> GetDetailDocId(int docId);

	Task<DataTable> FindDataNextLast(int id, int type);

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<DataTable> GetNo();

	Task<DataTable> GetPriceList();

	Task<DataTable> GetLandedCostName();

	Task<string> DeleteDocs(string ids);

	Task<DataTable> GetListDetailGLI(int objectType, string ids);

	Task<DataTable> GetBusinessPartner(string cardCode, string cardType, string userName, string branch);

	Task<HttpResult> Save(LandedCostImpl doc);

	Task<HttpResult> Close(int docEntry);

	Task<int> GetMaxDocEntry();
}
