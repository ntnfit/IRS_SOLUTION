using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.MasterData;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IRequester : IService<RequesterImpl>
{
	List<RequesterImpl> Cfl();

	DataTable GetRequesterByUser();

	DataTable GetRequesterByEmployee();

	DataTable GetUser_tableOUSR();

	DataTable GetAllUser_tableOUSR();
}
