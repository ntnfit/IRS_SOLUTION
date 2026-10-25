using System.Collections.Generic;

namespace Apzon.Entities.Models.MobileApp;

public class CustomerOrder
{
	public string OrderId { get; set; }

	public string Title { get; set; }

	public string Status { get; set; }

	public string Editable { get; set; }

	public string Cancelable { get; set; }

	public string Color { get; set; }

	public List<LineModel> Lines { get; set; }
}
