using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Widget;

public interface IWidget
{
	DataTable GetDocument(int widgetEntry);

	DataTable GetItems(int widgetEntry);

	DataTable GetHeader(int widgetEntry);

	HttpResult Save(DataSet ds);

	DataTable GetPages(int widgetEntry);

	DataTable GetAll();

	HttpResult Remove(int absEntry);
}
