using System.Data;
using System.Threading.Tasks;

namespace Apzon.DataServices.Interfaces;

public interface IDistributionRule : IRepository<DataSet>
{
	Task<string> Save(string sessionId, DataSet ds);

	Task<string> DeleteDistributeRule(string sessionId, string ocrCode);
}
