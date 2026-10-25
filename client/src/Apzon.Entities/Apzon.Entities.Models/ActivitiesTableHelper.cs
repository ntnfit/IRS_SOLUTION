using System.Data;

namespace Apzon.Entities.Models;

public class ActivitiesTableHelper
{
	public static DataTable Acivity { get; set; }

	public static DataTable AcitityFilter { get; set; }

	public static DataTable ActivityComments { get; set; }

	public static DataTable ActivityCommentUnread { get; set; }
}
