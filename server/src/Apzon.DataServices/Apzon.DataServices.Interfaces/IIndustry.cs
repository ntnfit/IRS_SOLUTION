using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IIndustry
{
	HttpResult Save(DataTable dt, string sessionId);

	string Delete(int indCode, string sessionId);
}
