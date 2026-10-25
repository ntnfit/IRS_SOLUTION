using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.LandedCosts;

namespace Apzon.DataServices.Interfaces;

public interface ILandedCosts
{
	HttpResult Save(LandedCostImpl doc, string sessionId);

	HttpResult Close(int docEntry, string sessionId);
}
