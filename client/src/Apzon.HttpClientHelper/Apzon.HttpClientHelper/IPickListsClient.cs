using System.Threading.Tasks;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Inventory;

namespace Apzon.HttpClientHelper;

public interface IPickListsClient
{
	Task<HttpResult> Save(PickListsImpl doc);
}
