using System.Collections.Generic;

namespace Apzon.Entities.Models.MobileApp;

public class OrderOutputModel
{
	public string OrderId { get; set; }

	public string Title { get; set; }

	public string Status { get; set; }

	public string Editable { get; set; }

	public string Cancelable { get; set; }

	public ProductModel OrderHeader { get; set; }

	public List<ProductModel> OrderItems { get; set; }

	public OrderModel CreateOrderRequest { get; set; }
}
