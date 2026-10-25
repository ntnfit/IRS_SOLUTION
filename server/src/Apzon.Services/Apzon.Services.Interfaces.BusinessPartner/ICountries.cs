using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.BusinessPartner;

public interface ICountries
{
	DataTable LoadOADF();

	HttpResult Save(DataTable dtDoc);

	DataTable GetOCRY();

	DataTable CheckExistCode(string code);
}
