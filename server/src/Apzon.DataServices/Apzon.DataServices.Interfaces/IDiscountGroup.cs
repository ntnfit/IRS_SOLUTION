using System.Data;
using System.Threading.Tasks;

namespace Apzon.DataServices.Interfaces;

public interface IDiscountGroup
{
	Task<string> Save(string sessionId, DataSet ds);
}
