using System;

namespace Apzon.Entities.Models.Payments;

public class IncomingPaymentCouponsImpl
{
	public int DocNum { get; set; }

	public DateTime DocDate { get; set; }

	public string CouponNum { get; set; }

	public string CouponName { get; set; }

	public DateTime CardValid { get; set; }

	public string OwnerIdNum { get; set; }

	public string OwnerPhone { get; set; }

	public string OwnerAdd { get; set; }

	public decimal CouponSum { get; set; }

	public string CouponCur { get; set; }
}
