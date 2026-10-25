using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.KPIs;

public interface IKPIScope
{
	DataSet Search(DataTable dt);

	DataSet GetLines(int docentry);

	HttpResult Save(DataSet ds, string action);
}
