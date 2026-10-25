using System.Collections.Generic;

namespace Apzon.Entities.Models.MobileApp;

public class DeliItem
{
	public string ProductId { get; set; }

	public string Title { get; set; }

	public string Quantity { get; set; }

	public string Date { get; set; }

	public List<LineModel> Lines { get; set; }
}
