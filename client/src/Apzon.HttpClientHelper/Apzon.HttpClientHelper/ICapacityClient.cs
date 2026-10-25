using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;

namespace Apzon.HttpClientHelper;

public interface ICapacityClient
{
	Task<DataTable> GetResources();

	Task<DataTable> GetDataAppointments(int scheduleId, Enums.TransactionType transactionType = Enums.TransactionType.A);

	Task<DataTable> GetColorSetting();

	Task<string> Save(DataTable dt);

	DataTable GetDataResourceEffic(string resCode, string routCode, int routLine);
}
