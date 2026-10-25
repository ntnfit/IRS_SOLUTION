using System;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.HumanResources;

public interface IJVInterview
{
	DataTable LoadDoc(int empID);

	DataTable LoadInterviewStatus();

	DataTable LoadInterviewTypes();

	DataTable CheckInterviewStatus(int ID);

	DataTable CheckInterviewTypes(int ID);

	HttpResult SaveData(Tuple<DataTable, string> dt);
}
