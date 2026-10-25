using System;
using System.Data;
using Apzon.Commons;
using Apzon.Services.Interfaces.DMS;

namespace Apzon.Services.Repository.DMS;

internal class OutletRepository : BaseService<DataSet>, IOutlet
{
	private IUnitOfWork _unitOfWork;

	public OutletRepository(IDatabaseService databaseService, UnitOfWork unitOfWork)
		: base(databaseService)
	{
		_unitOfWork = unitOfWork;
	}

	public OutletRepository(IDatabaseService databaseService, HanaUnitOfWork unitOfWork)
		: base(databaseService)
	{
		_unitOfWork = unitOfWork;
	}

	public DataTable GetList(string userCode)
	{
		try
		{
			return _unitOfWork.BusinessMasterData.GetBpList("C", "", "S", userCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}
}
