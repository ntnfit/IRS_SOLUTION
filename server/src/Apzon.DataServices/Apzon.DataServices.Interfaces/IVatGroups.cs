using System.Collections.Generic;
using Apzon.Entities.Models.Administrator;

namespace Apzon.DataServices.Interfaces;

public interface IVatGroups : IRepository<VatGroupsImpl>
{
	bool CheckExists(string code);

	string Save(List<VatGroupsImpl> docs, string sessionId);

	string Delete(string code, string sessionId);
}
