using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.HumanResources;

public interface IJVAnswer
{
	DataTable LoadQuestion();

	DataTable LoadAnswer(int id);

	HttpResult SaveData(DataSet dt);
}
