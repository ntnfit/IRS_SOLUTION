using System.Collections.Generic;

namespace Apzon.Entities.Models.MobileApp;

public class DashboardDetail
{
	public string Title { get; set; }

	public string ImageUrl { get; set; }

	public string Color { get; set; }

	public string OrderId { get; set; }

	public string Status { get; set; }

	public string Editable { get; set; }

	public string Cancelable { get; set; }

	public string OrderType { get; set; }

	public List<DashboardAttributes> Attributes { get; set; }
}
