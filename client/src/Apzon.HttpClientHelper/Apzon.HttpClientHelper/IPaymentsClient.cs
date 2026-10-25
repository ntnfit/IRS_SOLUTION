using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IPaymentsClient
{
	Task<DataTable> GetById(int docEntry);
}
