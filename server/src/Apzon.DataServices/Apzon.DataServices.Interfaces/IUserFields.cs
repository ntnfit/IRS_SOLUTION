using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;

namespace Apzon.DataServices.Interfaces;

public interface IUserFields : IRepository<UserFieldsImpl>
{
	List<UserFieldsImpl> GetListField(string tableName, string sessionId);

	HttpResult Save(DataSet ds, string sessionId);

	string RemoveUdf(string tableID, int fieldID, string sessionId);
}
