using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IActivitiesMeetingLocation
{
	HttpResult Save(DataTable dt, string sessionId);

	string Delete(int code, string sessionId);
}
