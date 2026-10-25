using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.PM;

public interface IComments
{
	DataTable GetByObject(string objectType, string objectCode, string objectDetail);

	HttpResult AddNew(DataTable dt, string userCode);
}
