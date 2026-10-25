using System.Data;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.DMS;

namespace Apzon.Services.Interfaces.DMS;

public interface ISurvey
{
	HttpResult Save(SurveyImpl entities, string action);

	DataTable GetQuestionById(int docentry);

	DataTable GetAllSurvey(int clgCode);

	DataSet GetSurveyById(int docEntry);
}
