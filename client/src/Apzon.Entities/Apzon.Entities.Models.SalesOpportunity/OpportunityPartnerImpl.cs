namespace Apzon.Entities.Models.SalesOpportunity;

public class OpportunityPartnerImpl
{
	public int OpportId { get; set; }

	public short Line { get; set; }

	public int? ParterId { get; set; }

	public string Memo { get; set; }

	public int? OrlCode { get; set; }

	public string RelatCard { get; set; }
}
