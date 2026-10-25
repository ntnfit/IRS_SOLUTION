using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Commons;

public class UDTController : BaseApiController
{
	[HttpGet]
	public DataTable GetAllUDT()
	{
		return UnitOfWork.UDTs.GetAllUDT();
	}

	[HttpGet]
	public List<UDTsImpl> LoadBarUdt()
	{
		return UnitOfWork.UDTs.LoadBarUdt();
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDoc([FromBody] UDTsImpl doc)
	{
		return UnitOfWork.UDTs.SaveDoc(doc);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteDoc([FromBody] string table)
	{
		return UnitOfWork.UDTs.DeleteDoc(table);
	}

	[HttpPost]
	public string UpdateUdt(DataTable dtDoc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA)
			{
				return unitOfWork.UserTablesDefined.SaveData(dtDoc, SessionUserInfo.SessionDiServer);
			}
		}
		return UnitOfWork.UDTs.UpdateUdt(dtDoc);
	}

	[HttpGet]
	public DataTable LoadDoc(string tableName)
	{
		return UnitOfWork.UDTs.LoadDoc(tableName);
	}

	[HttpGet]
	public DataTable AddNEW(string tableName)
	{
		return UnitOfWork.UDTs.AddNEW(tableName);
	}

	[HttpPost]
	public string Save(Tuple<DataTable, string> lstTuple)
	{
		return UnitOfWork.UDTs.Save(lstTuple);
	}

	[HttpGet]
	public DataTable GetListUDT(string userName, string branch)
	{
		return UnitOfWork.UDTs.GetListUDT(userName, branch);
	}
}
