using System.Data;

namespace Apzon.Entities.Models;

public static class ProjectManagementTableHelper
{
	public static DataTable Project { get; set; }

	public static DataTable ProjectWorkBreakdownStructure { get; set; }

	public static DataTable ProjectBudget { get; set; }

	public static DataSet ProjectDataLines { get; set; }
}
