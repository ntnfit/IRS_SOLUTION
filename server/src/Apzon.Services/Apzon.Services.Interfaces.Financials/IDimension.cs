using System.Data;

namespace Apzon.Services.Interfaces.Financials;

public interface IDimension
{
	string Save(DataTable dtDoc);

	DataTable GetData();

	DataTable GetDisableDim();
}
