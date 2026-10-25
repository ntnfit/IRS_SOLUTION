using System.Data;

namespace Apzon.Entities.Models.MobileApp;

public class CustomerAvailableProductsModel
{
	public string ProductId { get; set; }

	public string Title { get; set; }

	public string ImageUrl { get; set; }

	public string Color { get; set; }

	public string IsFavorite { get; set; }

	public DataTable Attributes { get; set; }

	public FormOpenModel OrderForm { get; set; }

	public FormOpenModel AddedServiceForm { get; set; }
}
