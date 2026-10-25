using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup.Purchasing;

public interface ILandedCostsSetup
{
	DataTable GetLandedCostsSetup();

	HttpResult SaveLandedCostsSetup(DataTable dt);

	string RemoveLandedCostsSetup(string code);
}
