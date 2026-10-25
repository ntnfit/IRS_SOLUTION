using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MobileApp;
using Apzon.Services.Interfaces.DMS;

namespace Apzon.Services.Repository.DMS;

public class UploadImageRepository : BaseService<DataTable>, IUploadImage
{
	private const string tempDoc = "#APZ_D_UPIM";

	private const string tempDocUploadFile = "#APZ_D_UUPIM";

	public UploadImageRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetListAlbum()
	{
		try
		{
			string query = "select * from APZ_D_ImageAlbum;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetServerUpload(string ServerAddress, string DomainName, string DBName)
	{
		try
		{
			string query = "select * from APZ_D_UPSV where \"ServerAddress\" = @serverAddress and \"Domain\" = @domain and \"DBName\" = @dbName;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@serverAddress", ServerAddress));
			list.Add(new SqlParameter("@domain", DomainName));
			list.Add(new SqlParameter("@dbName", DBName));
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetAvatarUser(string userName)
	{
		try
		{
			string query = "select top 1 * from APZ_D_UUPIM where \"UserSign\" = (select top 1 USERID from OUSR where USER_CODE = @userName) AND \"ObjectType\" = @objectType;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@userName", userName));
			list.Add(new SqlParameter("@objectType", Function.ToString(1470000171)));
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetAvatar(string cardCode)
	{
		try
		{
			string query = "select * from APZ_D_UPIM where \"CardCode\" = @cardCode and \"IsAvatar\" = 'Y';";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@cardCode", cardCode));
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetAllAvatarStore(string userName)
	{
		try
		{
			string query = "APZ_D_GetAllAvatarStore";
			new DataTable();
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter
				{
					ParameterName = "@saleUser",
					Value = userName
				}
			};
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataSet GetImageByDate(string cardCode, string date)
	{
		try
		{
			string query = "select * from APZ_D_UPIM where \"CardCode\" = @cardCode and \"IsAvatar\" = 'N' and (@date is null or CAST(\"UploadDate\" as DATE) = @date);\r\n                                 select distinct CAST(\"UploadDate\" as Date) as \"UploadDate\" from APZ_D_UPIM where \"IsAvatar\" = 'N' and \"CardCode\" = @cardCode;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@cardCode", cardCode));
			list.Add(new SqlParameter("@date", string.IsNullOrEmpty(date) ? ((IConvertible)DBNull.Value) : ((IConvertible)date)));
			return ExecuteData(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataSet();
		}
	}

	public HttpResult UploadImage(DataTable dt)
	{
		try
		{
			if (dt.ExistsColumn("ObjectType"))
			{
				return Process(ApzObjectTypes.oUploadAvatarUserDMS, -1, Enums.TransactionType.U, dt);
			}
			return Process(ApzObjectTypes.oUploadImageDMS, -1, Enums.TransactionType.U, dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult
			{
				Message = ex.ToString()
			};
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		HttpResult httpResult = new HttpResult();
		string message = "";
		switch (transtype)
		{
		case Enums.TransactionType.U:
			message = Upadate(objtype, bplId, ref transtype, dt, ref objId, ref objNum).Message;
			break;
		case Enums.TransactionType.D:
			message = SaveFiles(objtype, bplId, ref transtype, dt, ref objId, ref objNum).Message;
			break;
		}
		httpResult.Message = message;
		return httpResult;
	}

	public HttpResult Upadate(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		try
		{
			HttpResult httpResult = new HttpResult();
			string query = "APZ_D_UploadImage";
			if (!dt.IsExistsColumn("FileNameDrive"))
			{
				dt.Columns.Add("FileNameDrive", typeof(string));
				dt.Rows[0]["FileNameDrive"] = null;
			}
			if (!dt.IsExistsColumn("Extension"))
			{
				dt.Columns.Add("Extension", typeof(string));
				dt.Rows[0]["Extension"] = null;
			}
			if (!dt.IsExistsColumn("PathThumbnail"))
			{
				dt.Columns.Add("PathThumbnail", typeof(string));
				dt.Rows[0]["PathThumbnail"] = null;
			}
			if (!dt.IsExistsColumn("Domain"))
			{
				dt.Columns.Add("Domain", typeof(string));
				dt.Rows[0]["Domain"] = null;
			}
			if (!dt.IsExistsColumn("DateCreated"))
			{
				dt.Columns.Add("DateCreated", typeof(DateTime));
				dt.Rows[0]["DateCreated"] = DBNull.Value;
			}
			if (!dt.IsExistsColumn("TimestampCreated"))
			{
				dt.Columns.Add("TimestampCreated", typeof(long));
				dt.Rows[0]["TimestampCreated"] = DBNull.Value;
			}
			dt.AcceptChanges();
			if (objtype == ApzObjectTypes.oUploadAvatarUserDMS)
			{
				query = "APZ_D_UploadFile";
				CreateTempTable(dt.Copy(), "#APZ_D_UUPIM");
				BulkCopy(dt.Copy(), "#APZ_D_UUPIM");
			}
			else
			{
				CreateTempTable(dt.Copy(), "#APZ_D_UPIM");
				BulkCopy(dt.Copy(), "#APZ_D_UPIM");
			}
			DataSet dataSet = ExecuteData(query, CommandType.StoredProcedure);
			for (int i = 0; i < dataSet.Tables.Count; i++)
			{
				if (Function.ToString(dataSet.Tables[i].Rows[0]["Message"]) != "")
				{
					httpResult.Message = Function.ToString(dataSet.Tables[i].Rows[0]["Message"]);
				}
			}
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetFileByOjectType(string objectType, string objectCode)
	{
		try
		{
			string query = "APZ_D_UploadFile_GetFileByOjectType";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@objectType", objectType));
			list.Add(new SqlParameter("@objectCode", objectCode));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public HttpResult UploadFiles(DataTable dt)
	{
		try
		{
			return Process(ApzObjectTypes.oUploadAvatarUserDMS, -1, Enums.TransactionType.D, dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult
			{
				Message = ex.ToString()
			};
		}
	}

	public HttpResult SaveFiles(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		try
		{
			new HttpResult();
			string query = "APZ_D_UploadFile_Save";
			DataTable tableStruct = GetTableStruct("APZ_D_UUPIM");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dt = dt.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(dt, "#APZ_D_UUPIM");
			BulkCopy(dt, "#APZ_D_UUPIM");
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
				{
					objId = Function.ToString(dataTable.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, "Insert table APZ_D_UUPIM fail");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetImageList(DataTable dt)
	{
		try
		{
			string query = "APZ_D_Image_GetList";
			if (!dt.IsNotNull())
			{
				return new DataTable();
			}
			DataRow dataRow = dt.Rows[0];
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@fromDate", (dataRow["FromDate"] == null) ? dataRow["FromDate"] : ((object)Function.ParseDateTimes(dataRow["FromDate"]))));
			list.Add(new SqlParameter("@toDate", (dataRow["ToDate"] == null) ? dataRow["ToDate"] : ((object)Function.ParseDateTimes(dataRow["ToDate"]))));
			list.Add(new SqlParameter("@empID", Function.ParseInt(dataRow["empID"])));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetItemImageList(DataTable dt)
	{
		try
		{
			string query = "APZ_D_Image_GetListItem";
			if (!dt.IsNotNull())
			{
				return new DataTable();
			}
			DataRow dataRow = dt.Rows[0];
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@fromCode", Function.ToString(dataRow["FromCode"])));
			list.Add(new SqlParameter("@toCode", Function.ToString(dataRow["ToCode"])));
			list.Add(new SqlParameter("@itemGroup", Function.ToString(dataRow["ItemGroups"])));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public ResponseResult GetStoreAlbumImage(string fromDate, string toDate, int userSign, string listCardCode, int pageIndex, string listAlbumId, string filterTypeAttach, string lstObjectType, int empId)
	{
		try
		{
			string[] array = Function.ToString(listCardCode).Split(',');
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("CardCode", typeof(string));
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(array[i])))
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["CardCode"] = Function.ToString(array[i]);
					dataTable.Rows.Add(dataRow);
				}
			}
			dataTable.AcceptChanges();
			string[] array2 = Function.ToString(listAlbumId).Split(',');
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add("Id", typeof(string));
			for (int j = 0; j < array2.Length; j++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(array2[j])))
				{
					DataRow dataRow2 = dataTable2.NewRow();
					dataRow2["Id"] = Function.ToString(array2[j]);
					dataTable2.Rows.Add(dataRow2);
				}
			}
			dataTable2.AcceptChanges();
			string[] array3 = Function.ToString(lstObjectType).Split(',');
			DataTable dataTable3 = new DataTable();
			dataTable3.Columns.Add("ObjectType", typeof(string));
			for (int k = 0; k < array3.Length; k++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(array3[k])))
				{
					DataRow dataRow3 = dataTable3.NewRow();
					dataRow3["ObjectType"] = Function.ToString(array3[k]);
					dataTable3.Rows.Add(dataRow3);
				}
			}
			dataTable3.AcceptChanges();
			DataTable content = new DataTable();
			using (DatabaseService.OpenConnection())
			{
				CreateTempTable(dataTable, "#tbl");
				BulkCopy(dataTable, "#tbl");
				CreateTempTable(dataTable2, "#tblAlbum");
				BulkCopy(dataTable2, "#tblAlbum");
				CreateTempTable(dataTable3, "#tblObjectType");
				BulkCopy(dataTable3, "#tblObjectType");
				string query = "APZ_D_StoreImageAlbum_GetList";
				List<SqlParameter> list = new List<SqlParameter>();
				SqlParameter item = new SqlParameter("@fromDate", fromDate);
				SqlParameter item2 = new SqlParameter("@toDate", toDate);
				SqlParameter item3 = new SqlParameter("@userSign", userSign);
				SqlParameter item4 = new SqlParameter("@empId", empId);
				SqlParameter item5 = new SqlParameter("@pageIndex", pageIndex);
				SqlParameter item6 = new SqlParameter("@typeAttach", filterTypeAttach);
				list.Add(item);
				list.Add(item2);
				list.Add(item3);
				list.Add(item5);
				list.Add(item6);
				list.Add(item4);
				content = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			}
			return new ResponseResult
			{
				Message = "",
				MessageCode = "200",
				Content = content
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "406"
			};
		}
	}
}
