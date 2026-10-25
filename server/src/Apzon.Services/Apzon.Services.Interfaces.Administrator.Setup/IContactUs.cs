using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IContactUs
{
	HttpResult Save(ContactUsImpl entities);
}
