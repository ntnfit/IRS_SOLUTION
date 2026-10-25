using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.DMS;

public interface IOutletVisitingFrequency
{
	DataTable GetData(string userName, int branch);

	HttpResult Save(DataTable dt);
}
