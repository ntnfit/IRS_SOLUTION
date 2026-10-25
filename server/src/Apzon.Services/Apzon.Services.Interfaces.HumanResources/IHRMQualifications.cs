using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.HumanResources;

public interface IHRMQualifications
{
	DataTable GetAdvSource();

	HttpResult SaveAdvSource(DataTable dt);

	string RemoveAdvSource(int ID);

	DataTable GetJobType();

	HttpResult SaveJobType(DataTable dt);

	string RemoveJobType(int ID);

	DataTable GetQualificationsGroup();

	HttpResult SaveQualificationsGroup(DataTable dt);

	string RemoveQualificationsGroup(int ID);

	int GetMaxDocEntry();

	DataTable GetQualifications();

	HttpResult SaveQualifications(DataTable dt);

	string RemoveQualifications(int QuaCodes);

	DataTable CheckQuaGroup(int QuaCodes);

	DataTable GetQuaCode(int quaId);
}
