using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IManufacturer
{
	HttpResult Save(DataTable dt, string sessionId);

	string Delete(int code, string sessionId);

	HttpResult Delete(DataTable dt, string sessionId);
}
