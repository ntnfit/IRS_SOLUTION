using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.PlanningProduction;

public interface IShift
{
	DataTable GetData(int branch);

	DataTable GetColumn();

	HttpResult Save(DataTable dt, Enums.TransactionType type, string objId);
}
