using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IUserObjectDefined
{
	string SaveData(DataSet ds, EnumCommandType commandType, string sessionId);

	HttpResult Delete(string code, string sessionId);
}
