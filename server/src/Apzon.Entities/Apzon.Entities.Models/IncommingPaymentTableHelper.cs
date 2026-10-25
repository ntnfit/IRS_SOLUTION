using System.Data;

namespace Apzon.Entities.Models;

public class IncommingPaymentTableHelper
{
	public static DataTable Payment { get; set; }

	public static DataTable PaymentInvoices { get; set; }

	public static DataTable PaymentAccounts { get; set; }

	public static DataTable PaymentChecks { get; set; }
}
