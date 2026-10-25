using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IOpportunityStage
{
	HttpResult Save(DataTable dt, string sessionId);

	string Delete(int seqNo, string sessionId);
}
