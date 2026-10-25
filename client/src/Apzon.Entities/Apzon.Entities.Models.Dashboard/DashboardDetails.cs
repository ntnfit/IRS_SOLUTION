namespace Apzon.Entities.Models.Dashboard;

public class DashboardDetails
{
	public int Id { get; set; }

	public string Name { get; set; }

	public int CatId { get; set; }

	public int UserSign { get; set; }

	public string Layout { get; set; }

	public bool ValidFor { get; set; }
}
