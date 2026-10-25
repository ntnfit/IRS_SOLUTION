using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IJVQuestionClient
{
	DataTable LoadQuestionGroup();

	DataTable LoadQuestion();

	Task<DataTable> LoadAnswer(int id);

	DataTable CheckExist(Tuple<int, string, string> ID);

	DataTable CheckQuestion(int ID);

	Task<string> DeleteQuestion(int ID);

	DataTable CheckQuestionGroup(int ID);

	HttpResult SaveQuestion(Tuple<DataTable, string> dt);

	DataTable LoadBenefit();

	DataTable CheckBenefit(int ID);

	HttpResult SaveBenefit(Tuple<DataTable, string> dt);
}
