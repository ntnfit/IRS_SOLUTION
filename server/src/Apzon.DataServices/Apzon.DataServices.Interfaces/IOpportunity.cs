using System.Data;
using Apzon.Entities.Models.SalesOpportunity;

namespace Apzon.DataServices.Interfaces;

public interface IOpportunity : IRepository<OpportunitiesImpl>
{
	string Save(DataSet ds, string sessionId);

	string DeleteDocs(string ids, string sessionId);
}
