using System.Data;

namespace Apzon.Entities.Models;

public static class OpportunityTableHelper
{
	public static DataTable Opportunity { get; set; }

	public static DataTable OpportunityStages { get; set; }

	public static DataTable OpportunityItems { get; set; }

	public static DataTable Competitors { get; set; }

	public static DataTable Partner { get; set; }

	public static DataTable Reason { get; set; }

	public static DataTable LinkActivity { get; set; }

	public static DataTable Attachment { get; set; }

	public static DataTable Interest { get; set; }
}
