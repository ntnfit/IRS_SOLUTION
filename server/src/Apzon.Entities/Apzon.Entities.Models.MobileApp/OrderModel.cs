using System.Collections.Generic;

namespace Apzon.Entities.Models.MobileApp;

public class OrderModel
{
	public string OrderId { get; set; }

	public string CusCode { get; set; }

	public string CusName { get; set; }

	public List<ProductModel> Products { get; set; }
}
