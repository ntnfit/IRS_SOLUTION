using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator;

public interface IUnitOfMeasure
{
	DataTable LoadDoc();

	HttpResult Save(DataTable dt);

	DataTable CheckLengthUnit(string unitDisply);

	DataTable CheckWidthUnit(string unitDisply);

	DataTable GetAllLengthUnits();
}
