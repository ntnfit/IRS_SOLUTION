using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IPickAndPacks : IRepository<DataTable>
{
	HttpResult Save(DataSet ds, string sessionId);
}
