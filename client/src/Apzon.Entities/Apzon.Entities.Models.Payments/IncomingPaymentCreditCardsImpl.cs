using System;

namespace Apzon.Entities.Models.Payments;

public class IncomingPaymentCreditCardsImpl
{
	public int DocNum { get; set; }

	public DateTime DocDate { get; set; }

	public int CreditCard { get; set; }

	public string CrCardNum { get; set; }

	public string CardName { get; set; }

	public DateTime CardValid { get; set; }

	public string OwnerIdNum { get; set; }

	public string OwnerPhone { get; set; }

	public string OwnerAdd { get; set; }

	public decimal CreditSum { get; set; }

	public string CreditCur { get; set; }

	public string ConfNum { get; set; }
}
