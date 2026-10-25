using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator;

public interface ICustomsGroup
{
	DataTable LoadDoc();

	DataTable GetMaxCstGrpCode();

	DataTable CheckExistCstGrpCode(int cstGrpCode);

	HttpResult SaveOARG(DataTable dt);
}
