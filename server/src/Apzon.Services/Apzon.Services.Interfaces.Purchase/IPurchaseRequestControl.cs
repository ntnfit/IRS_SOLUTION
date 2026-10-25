using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Services.Interfaces.Purchase;

public interface IPurchaseRequestControl
{
	List<UserFieldsImpl> GetListField(string tableName);
}
