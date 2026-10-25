namespace Apzon.Entities.Models.SalesOpportunity;

public class OpportunityCompetitorImpl
{
	public int OpportId { get; set; }

	public short Line { get; set; }

	public int? CompetId { get; set; }

	public string Memo { get; set; }

	public string Won { get; set; }

	public short? ThreatLevl { get; set; }
}
