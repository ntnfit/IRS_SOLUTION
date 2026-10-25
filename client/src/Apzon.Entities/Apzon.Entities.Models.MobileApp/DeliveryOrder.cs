using System.Collections.Generic;

namespace Apzon.Entities.Models.MobileApp;

public class DeliveryOrder
{
	public string DeliveryOrderId { get; set; }

	public string Title { get; set; }

	public string Status { get; set; }

	public string Color { get; set; }

	public List<LineModel> Lines { get; set; }

	public DeliHeader OrderHeader { get; set; }

	public List<DeliItem> OrderItems { get; set; }
}
