using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IWarehouseSublevel
{
	HttpResult Save(string sessionId, DataTable doc);

	HttpResult SaveGen(string sessionId, DataTable doc);
}
