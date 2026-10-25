using System.Collections.Generic;
using System.Threading.Tasks;
using Apzon.Entities.Models.UserFields;

namespace Apzon.HttpClientHelper;

public interface IPurchaseRequestControlClient
{
	Task<List<UserFieldsImpl>> GetListField(string tblName);
}
