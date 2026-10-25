using System.Data;

namespace Apzon.Services.Interfaces.Payment;

public interface IPayments
{
	DataTable GetById(int docEntry);
}
