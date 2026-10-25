using System.Data;

namespace Apzon.Services.Interfaces.Administrator;

public interface IFreight
{
	DataTable GetFreight();

	DataTable GetMaxExpnsCode();

	DataTable GetOutputTaxGroup();

	DataTable GetVatGroupI();

	string DeleteFreight(int code);

	string UpdateFreight(DataTable dt);
}
