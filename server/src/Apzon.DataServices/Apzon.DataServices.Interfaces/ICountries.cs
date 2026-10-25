using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface ICountries
{
	bool CheckExists(string code);

	HttpResult Save(DataTable dt, string sessionId);

	HttpResult Remove(string code, string sessionId);
}
