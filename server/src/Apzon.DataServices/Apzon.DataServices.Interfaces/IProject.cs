using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IProject : IRepository<Common.Table_Code>
{
	string Save(DataTable dt, string sessionId);

	string Delete(string prjCode, string sessionId);

	bool Exists(string prjCode, string sessionId);

	string DeleteProject(string code, string sessionId);
}
