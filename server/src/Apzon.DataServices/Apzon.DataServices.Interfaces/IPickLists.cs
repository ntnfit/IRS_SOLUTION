using System.Threading.Tasks;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Inventory;

namespace Apzon.DataServices.Interfaces;

public interface IPickLists : IRepository<PickListsImpl>
{
	Task<HttpResult> Save(PickListsImpl doc, string sessionId);
}
