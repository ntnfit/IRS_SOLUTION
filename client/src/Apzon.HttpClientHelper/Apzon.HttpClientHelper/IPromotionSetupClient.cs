using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IPromotionSetupClient
{
	Task<DataSet> GetData();

	Task<HttpResult> SaveData(DataSet ds);

	string CheckUsed(string type, int id);
}
