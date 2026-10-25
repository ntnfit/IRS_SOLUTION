using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Udos;
using Apzon.Services.Interfaces.Udos;
using SAPbobsCOM;

namespace Apzon.Services.Repository.Udos;

public class UserObjectRepository : BaseService<UserObjectsImpl>, IUserObject
{
	public UserObjectRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public List<UserObjectsImpl> GetAll()
	{
		try
		{
			string query = "apz_udo_getall";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				List<UserObjectsImpl> list = new List<UserObjectsImpl>();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					UserObjectsImpl userObjectsImpl = new UserObjectsImpl();
					userObjectsImpl.Code = Function.ToString(dataTable.Rows[i]["Code"]);
					userObjectsImpl.Name = Function.ToString(dataTable.Rows[i]["Name"]);
					userObjectsImpl.TableName = Function.ToString(dataTable.Rows[i]["TableName"]);
					userObjectsImpl.LogTableName = Function.ToString(dataTable.Rows[i]["LogTable"]);
					switch (Function.ParseInt(dataTable.Rows[i]["TYPE"]))
					{
					case 1:
						userObjectsImpl.ObjectType = BoUDOObjType.boud_MasterData;
						break;
					case 3:
						userObjectsImpl.ObjectType = BoUDOObjType.boud_Document;
						break;
					}
					userObjectsImpl.ManageSeries = Function.ToString(dataTable.Rows[i]["MngSeries"]).Equals("Y");
					userObjectsImpl.CanDelete = Function.ToString(dataTable.Rows[i]["CanDelete"]).Equals("Y");
					userObjectsImpl.CanClose = Function.ToString(dataTable.Rows[i]["CanClose"]).Equals("Y");
					userObjectsImpl.CanCancel = Function.ToString(dataTable.Rows[i]["CanCancel"]).Equals("Y");
					userObjectsImpl.ExtensionName = Function.ToString(dataTable.Rows[i]["ExtName"]);
					userObjectsImpl.CanFind = Function.ToString(dataTable.Rows[i]["CanFind"]).Equals("Y");
					userObjectsImpl.CanYearTransfer = Function.ToString(dataTable.Rows[i]["CanYrTrnsf"]).Equals("Y");
					userObjectsImpl.CanCreateDefaultForm = Function.ToString(dataTable.Rows[i]["CanDefForm"]).Equals("Y");
					userObjectsImpl.CanLog = Function.ToString(dataTable.Rows[i]["CanLog"]).Equals("Y");
					userObjectsImpl.OverwriteDllfile = Function.ToString(dataTable.Rows[i]["OvrWrtDll"]).Equals("Y");
					userObjectsImpl.CanArchive = Function.ToString(dataTable.Rows[i]["CanArchive"]).Equals("Y");
					userObjectsImpl.MenuItem = Function.ToString(dataTable.Rows[i]["MenuItem"]).Equals("Y");
					userObjectsImpl.MenuCaption = Function.ToString(dataTable.Rows[i]["MenuCapt"]);
					userObjectsImpl.FatherMenuId = Function.ParseInt(dataTable.Rows[i]["FatherMenu"]);
					userObjectsImpl.Position = Function.ParseInt(dataTable.Rows[i]["Position"]);
					userObjectsImpl.MenuUid = Function.ToString(dataTable.Rows[i]["MenuUid"]);
					list.Add(userObjectsImpl);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new List<UserObjectsImpl>();
	}
}
