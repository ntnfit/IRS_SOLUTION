using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Business_Partners;

public interface IBusinessPartnerProperties
{
	DataTable LoadDoc();

	HttpResult Save(DataTable dtDoc);
}
