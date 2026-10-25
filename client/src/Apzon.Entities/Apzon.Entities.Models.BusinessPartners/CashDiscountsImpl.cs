namespace Apzon.Entities.Models.BusinessPartners;

public class CashDiscountsImpl
{
	public string CdcCode { get; set; }

	public int LineId { get; set; }

	public int NumOfDays { get; set; }

	public decimal Discount { get; set; }
}
