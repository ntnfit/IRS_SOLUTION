using System;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.HumanResources;

public interface IJVQuestion
{
	DataTable LoadQuestionGroup();

	DataTable LoadQuestion();

	DataTable LoadAnswer(int id);

	DataTable CheckExist(Tuple<int, string, string> ID);

	DataTable CheckQuestion(int ID);

	string DeleteQuestion(int ID);

	DataTable CheckQuestionGroup(int ID);

	HttpResult SaveData(Tuple<DataTable, string> dt);

	DataTable LoadBenefit();

	DataTable CheckBenefit(int ID);

	HttpResult SaveBenefit(Tuple<DataTable, string> dt);
}
