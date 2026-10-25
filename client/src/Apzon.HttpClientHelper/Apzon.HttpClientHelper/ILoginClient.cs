using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apzon.Entities.Models.Administrator;

namespace Apzon.HttpClientHelper;

public interface ILoginClient
{
	void LogOut(string sessionId);

	Task<List<string>> GetSessionPool(Tuple<LoginImpl, int> model);
}
