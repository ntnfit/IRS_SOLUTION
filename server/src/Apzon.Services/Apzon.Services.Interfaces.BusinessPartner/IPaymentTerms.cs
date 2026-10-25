using System.Data;

namespace Apzon.Services.Interfaces.BusinessPartner;

public interface IPaymentTerms
{
	DataTable LoadDoc();

	string SaveDoc(DataTable dt);

	string DeleteDoc(int groupNum);

	DataTable GetPaymentTermInfo(int groupNum);
}
