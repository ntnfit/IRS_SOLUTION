using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Commons;

public interface IPublishService
{
	HttpResult Save(DataSet ds);

	DataTable GetByName(string name, string method);

	DataTable GetJsonStructureById(int id);

	DataTable Search(string controllerName, string actionName, string description);

	DataSet GetById(int id);

	string Navigate(int absEntry, Enums.NavigateType type);

	HttpResult Remove(int absEntry);
}
