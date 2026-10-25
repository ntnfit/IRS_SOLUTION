using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator;

public interface IUoMGroupDefinition
{
	DataTable LoadDoc(int ugpEntry);

	HttpResult Save(DataTable dt);
}
