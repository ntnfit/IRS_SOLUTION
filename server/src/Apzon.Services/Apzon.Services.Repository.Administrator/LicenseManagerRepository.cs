using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Administrator;

namespace Apzon.Services.Repository.Administrator;

public class LicenseManagerRepository : BaseService<ILicenseManager>, ILicenseManager
{
	public LicenseManagerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public string GetServerDate()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable(GetServerDateQuery(), CommandType.Text);
			if (dataTable.Rows.Count > 0)
			{
				return Function.ParseDateTimes(dataTable.Rows[0]["d"]).ToString("yyyyMMdd");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	protected virtual string GetServerDateQuery()
	{
		return "SELECT GETDATE() as d";
	}

	public string Save(string bpcode, string licenseNo, string hardwareKey, string file, string fileId, string licenseType, string dateCreate, DataTable dtLicType)
	{
		try
		{
			string serverDate = GetServerDate();
			if (Function.ParseInt(serverDate) < Function.ParseInt(dateCreate))
			{
				return "Date server is not real!";
			}
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@bpcode", bpcode),
				new SqlParameter("@InstallNumber", licenseNo),
				new SqlParameter("@HardwareKey", hardwareKey),
				new SqlParameter("@license", file),
				new SqlParameter("@fileId", fileId),
				new SqlParameter("@licenseType", licenseType),
				new SqlParameter("@dateRequest", Function.Encrypt(RSA.PublicKey, serverDate))
			};
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				CreateTempTable(dtLicType, "#tempLicType");
				BulkCopy(dtLicType, "#tempLicType");
				ExecuteNonQuery("APZ_LicenseManager_SaveLic", CommandType.StoredProcedure, list.ToArray());
				dbTransaction.Commit();
				return "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetUser()
	{
		try
		{
			return ExecuteDataTable("APZ_LicenseManager_GetListUser", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetLicenseUser(string licenseNo, string licenseType)
	{
		try
		{
			return ExecuteDataTable(parameters: new SqlParameter[2]
			{
				new SqlParameter("@licenseNo", licenseNo),
				new SqlParameter("@licenseType", licenseType)
			}, query: GetLicenseUserQuery());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetLicenseUserQuery()
	{
		return "select LOWER([User]) [User],[DataBase] from APZ_LOU where InstallNumber = @licenseNo and LicenseType = @licenseType";
	}

	public DataSet GetLicense(string hardwareKey)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@hardwarekey", hardwareKey);
			return ExecuteData(GetLicenseQuery(), CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	protected virtual string GetLicenseQuery()
	{
		return "select License, \"InstallNumber\" From APZ_OALC a where a.HardwareKey = @hardwarekey\r\n                     select x.LicenseType, count(1) Used from (select a.LicenseType from APZ_LOU a inner join APZ_OALC b ON a.InstallNumber = b.InstallNumber where b.HardwareKey = @hardwarekey group by a.LicenseType, a.[User]) x group by x.LicenseType";
	}

	public string SaveUser(DataTable dt, string database, string licenseId)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query;
				if (dt != null && dt.Rows.Count > 0)
				{
					query = "APZ_LicenseManager_SaveUser";
					CreateTempTable(dt, "#tabletemp");
					BulkCopy(dt, "#tabletemp");
					if (ExecuteNonQuery(query, CommandType.StoredProcedure) > 0)
					{
						dbTransaction.Commit();
						return "";
					}
					dbTransaction.Rollback();
					return "Save error!";
				}
				query = "APZ_LicenseManager_DeleteUser";
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, new SqlParameter("@database", database), new SqlParameter("@licenseID", licenseId)) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "Delete error!";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "error connect sql";
		}
	}

	public DataTable GetOffline()
	{
		try
		{
			return ExecuteDataTable(GetOfflineQuery(), CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetOfflineQuery()
	{
		return "select Session, [User], UpdateTime, [DataBase] from APZ_LOGLOGIN where status = 'Y'";
	}

	public string UpdateSession(string session)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@session", session),
				new SqlParameter("@logoffDate", DateTime.Now),
				new SqlParameter("@status", "N")
			};
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (ExecuteNonQuery(GetUpdateSessionQuery(), CommandType.Text, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected virtual string GetUpdateSessionQuery()
	{
		return "UPDATE APZ_LOGLOGIN SET LogoffTime = @logoffDate , status = @status where Session = @session";
	}

	public string DeleteAllLicenseUser(string hardwareKey)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (ExecuteNonQuery(GetDeleteAllLicenseUserQuery(), CommandType.Text, new SqlParameter("@hardwareKey", hardwareKey)) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	protected virtual string GetDeleteAllLicenseUserQuery()
	{
		return "Delete a from APZ_LOU a INNER JOIN APZ_OALC b on a.InstallNumber = b.InstallNumber where b.HardwareKey = @hardwareKey";
	}

	public void UpdateDataTrial(string userName)
	{
		try
		{
			if (!string.IsNullOrEmpty(userName) && !userName.ToUpper().Equals("manager".ToUpper()))
			{
				return;
			}
			int num = Function.ParseInt(ExecuteScalar("Select max(\"MessageID\") from OILM;", CommandType.Text));
			DataTable dataTable;
			switch (num)
			{
			case 0:
				return;
			default:
				dataTable = ExecuteDataTable(GetUpdateDataTrialMaxIdQuery(), CommandType.Text, new SqlParameter("@id", new Random().Next(1, num)));
				break;
			case 1:
				dataTable = ExecuteDataTable("Select top 1 \"Quantity\", \"LocCode\", \"ItemCode\", \"DocDate\", \"TransType\",\"DocEntry\",\"MessageID\" from OILM;", CommandType.Text);
				break;
			}
			DataTable dataTable2 = dataTable;
			while (!dataTable2.IsNotNull())
			{
				dataTable2 = ExecuteDataTable(GetUpdateDataTrialGetdataQuery(), CommandType.Text, new SqlParameter("@id", new Random().Next(1, num)));
			}
			string text = Function.ToString(ExecuteScalar("Select top 1 \"TextCode\" from APZ_CINF;", CommandType.Text));
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			text = Function.Decrypt(RSA.PrivateKey, text);
			string[] array = text.Split('#');
			string text2 = (string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[0]["DocDate"])) ? "" : Function.ParseDateTimes(dataTable2.Rows[0]["DocDate"]).ToString("yyyyMMdd"));
			string plainText = string.Join("#", array[0], array[1], Function.ToString(dataTable2.Rows[0]["MessageID"]), Function.ToString(dataTable2.Rows[0]["TransType"]), Function.ToString(dataTable2.Rows[0]["DocEntry"]), text2, Function.ToString(dataTable2.Rows[0]["ItemCode"]), Function.ToString(dataTable2.Rows[0]["LocCode"]), Function.ToString(dataTable2.Rows[0]["Quantity"]));
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (ExecuteNonQuery(GetUpdateDataTrialUpdateQuery(), CommandType.Text, new SqlParameter("@code", Function.Encrypt(RSA.PublicKey, plainText))) > 0)
				{
					dbTransaction.Commit();
				}
				else
				{
					dbTransaction.Rollback();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	protected virtual string GetUpdateDataTrialMaxIdQuery()
	{
		return "Select \"Quantity\", \"LocCode\", \"ItemCode\", \"DocDate\", \"TransType\", \"DocEntry\", \"MessageID\" from OILM where \"MessageID\" = @id";
	}

	protected virtual string GetUpdateDataTrialGetdataQuery()
	{
		return "Select Quantity, LocCode, ItemCode, DocDate, TransType,DocEntry,MessageID from OILM where MessageID = @id";
	}

	protected virtual string GetUpdateDataTrialUpdateQuery()
	{
		return "UPDATE APZ_CINF SET TextCode = @code";
	}

	public void UpdateSessionUser(string session, string userName, string database, string clientIp, string clientMachine, string clientLogon)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@session", session),
				new SqlParameter("@user", userName),
				new SqlParameter("@database", database),
				new SqlParameter("@client_ip", clientIp ?? ""),
				new SqlParameter("@client_machine", clientMachine ?? ""),
				new SqlParameter("@client_logon", clientLogon ?? "")
			};
			ExecuteNonQuery(GetUpdateSessionUserQuery(), CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	protected virtual string GetUpdateSessionUserQuery()
	{
		return "UPDATE APZ_LOGLOGIN SET LogoffTime = GetDate() , status = 'N' where [user] = @user and Session=@session\r\n                                        INSERT INTO APZ_LOGLOGIN ([Session], [User],  [DataBase], LoginTime, UpdateTime , [status],ClientLocalIpAddress,ClientMachineName,ClientWindowsLogonName) values \r\n\t\t\t\t\t\t\t\t(@session, @user, @database, GetDate(), Getdate() , 'Y',@client_ip,@client_machine,@client_logon)";
	}
}
