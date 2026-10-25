using System.Data;

namespace Apzon.Services.Interfaces.Payment;

public interface IPaymentWizard
{
	string Save(DataSet ds);

	DataSet GetData(DataTable dtSearch);

	DataSet GetPaymentInfo(DataTable dtSearch);

	DataSet GetDetails(DataTable dtSearch);
}
