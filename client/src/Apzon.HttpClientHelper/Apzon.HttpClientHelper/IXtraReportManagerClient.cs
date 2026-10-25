using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IXtraReportManagerClient
{
	Task<string> GetDataReportLayoutById(int id);
}
