using System.Collections.Generic;
using Apzon.Entities.Models.Udos;

namespace Apzon.Services.Interfaces.Udos;

public interface IUserObject
{
	List<UserObjectsImpl> GetAll();
}
