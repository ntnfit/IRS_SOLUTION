using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IWidgetClient
{
	Task<DataTable> GetDocument(int widgetEntry);

	Task<DataTable> GetItems(int widgetEntry);

	Task<DataTable> GetHeader(int widgetEntry);

	Task<DataTable> GetPages(int widgetEntry);

	Task<HttpResult> Save(DataSet ds);

	Task<DataTable> GetAll();

	Task<HttpResult> Remove(int absEntry);
}
