using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IPromotionProgramClient
{
	Task<DataSet> Search(DataTable dt);

	Task<DataSet> GetLinesBenefit(int docentry);

	Task<DataSet> GetLinesCondition(int docentry);

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<HttpResult> Delete(DataSet ds);

	Task<HttpResult> Cancel(DataSet ds);
}
