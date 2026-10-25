using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IShipping
{
	Task<HttpResult> Save(DataSet ds, string sessionId);
}
