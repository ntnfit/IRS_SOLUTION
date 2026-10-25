namespace Apzon.Entities.Models.BusinessPartners;

public class PaymentTermsImpl
{
	public int GroupNum { get; set; }

	public string PymntGroup { get; set; }

	public string BslineDate { get; set; }

	public int ExtraDays { get; set; }

	public string OpenRcpt { get; set; }

	public string DiscCode { get; set; }

	public decimal VolumDscnt { get; set; }

	public decimal LatePyChrg { get; set; }

	public int ListNum { get; set; }

	public decimal CredLimit { get; set; }

	public decimal ObligLimit { get; set; }
}
