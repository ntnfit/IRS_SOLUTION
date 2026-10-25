using System.Data;
using Apzon.Commons.Enums;

namespace Apzon.Services.Interfaces.RoutingAndResource;

public interface ICapacity
{
	DataTable GetResources();

	DataTable GetDataAppointments(int scheduleId, Enums.TransactionType transactionType);

	DataTable GetColorSetting();

	string Save(DataTable dt);

	DataTable GetDataResourceEffic(string resCode, string routCode, int routLine);
}
