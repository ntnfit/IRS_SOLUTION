using System.Data;

namespace Apzon.Services.Interfaces.Administrator.Banking;

public interface ICreditCardPaymentMethods
{
	DataTable LoadDoc();

	string Save(DataTable dtDoc);

	DataTable GetPaymentCodes();

	DataTable GetCreditCards();

	string DeleteLine(int crTypeCode);
}
