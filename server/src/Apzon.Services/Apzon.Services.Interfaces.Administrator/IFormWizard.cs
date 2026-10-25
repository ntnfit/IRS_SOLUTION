using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator;

public interface IFormWizard
{
	DataTable GetValid();

	DataSet GetById(int templateId);

	DataTable GetAll();

	HttpResult SaveData(DataSet ds);

	HttpResult Remove(int absEntry);
}
