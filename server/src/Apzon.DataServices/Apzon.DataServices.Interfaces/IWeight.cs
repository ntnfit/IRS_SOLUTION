using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IWeight
{
	int CheckExists(int unitCode, string sessionId);

	HttpResult Save(DataTable dt, string sessionId);

	HttpResult Remove(DataTable dt, string sessionId);
}
