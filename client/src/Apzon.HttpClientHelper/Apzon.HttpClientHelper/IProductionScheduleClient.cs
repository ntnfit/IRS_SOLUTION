using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IProductionScheduleClient
{
	Task<DataSet> Run(string mrpCode, string scheduleType, string fdate, string tdate, string isDetailSchedule);
}
