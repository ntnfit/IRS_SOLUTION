namespace Apzon.Api.Controllers.DashBoard;

public class OrderInput
{
	public string OrderId { get; set; }

	public string DeliveryOrderId { get; set; }

	public string Status { get; set; }

	public string DashboardItem { get; set; }
}
