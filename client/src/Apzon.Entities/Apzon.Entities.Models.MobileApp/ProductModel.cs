using System.Collections.Generic;

namespace Apzon.Entities.Models.MobileApp;

public class ProductModel
{
	public string ProductId { get; set; }

	public string Title { get; set; }

	public string Quantity { get; set; }

	public string Date { get; set; }

	public List<FieldFormModel> Lines { get; set; }

	public FormOpenModel OrderRequest { get; set; }

	public List<FormOpenModel> AddedServiceRequests { get; set; }
}
