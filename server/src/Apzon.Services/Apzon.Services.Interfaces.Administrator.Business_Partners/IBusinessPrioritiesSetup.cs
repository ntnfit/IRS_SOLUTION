using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Business_Partners;

public interface IBusinessPrioritiesSetup
{
	DataTable LoadDoc();

	HttpResult Save(DataTable dtDoc);

	DataTable CheckExistBPP(int prioCode);

	HttpResult Remove(DataTable dt);
}
