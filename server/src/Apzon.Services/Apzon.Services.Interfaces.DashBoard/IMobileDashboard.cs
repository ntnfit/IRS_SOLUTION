using System.Data;

namespace Apzon.Services.Interfaces.DashBoard;

public interface IMobileDashboard
{
	DataTable GetCustomerDashboard(string username);

	DataSet GetCustomerDashboardDetail(string username, string dashboardItem);

	DataSet GetCustomerAvailableProducts(string cardCode);

	DataSet GetFormStructure(string cardCode);

	string AddProductToFavorite(string cardCode, string itemCode);

	string RemoveProductFromFavorite(string cardCode, string itemCode);

	DataTable GetAPISetting(string key);

	int GetBPBranch(string username);

	string GetLastProductUpdatedDate();

	DataSet GetOrderRequest(int docentry, string dashboardItem = "");

	string ConfirmOrder(string OrderId);

	DataTable GetCustomerOrders(string cardCode);

	DataTable GetServiceUserDeliveryOrder(string username, string status);

	DataSet GetServiceUserDeliOrdersDetail(string docentry);

	string UpdateDeliveryOrderStatus(string docentry, string status);
}
