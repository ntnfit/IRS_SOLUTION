using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IActivityStatus
{
	HttpResult Save(DataTable dtSave, string sessionId);
}
