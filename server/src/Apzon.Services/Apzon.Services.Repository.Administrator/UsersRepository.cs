using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Administrator;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator;

public class UsersRepository : BaseService<UsersImpl>, IUsers, IService<UsersImpl>
{
	public UsersRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable ShowUsers()
	{
		DataTable result = new DataTable();
		try
		{
			result = ExecuteDataTable(GetShowUsersQuery(), CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string GetShowUsersQuery()
	{
		return "select  a.USERID,a.PASSWORD, a.USER_CODE,a.U_NAME, ISNULL( a.INTERNAL_K,0) as INTERNAL_K ,\r\n\t                                ISNULL\t(a.GROUPS,0) as GROUPS, \r\n\t\t\t\t\t\t\t\t\ta.SUPERUSER, \r\n\t\t\t\t\t\t\t\t\tisnull( a.E_Mail,'') as E_Mail\r\n\t\t\t\t\t\t\t\t\t, isnull( a.PortNum,'') as PortNum,\r\n\t                                isnull(a.Fax,'') as Fax,\r\n\t\t\t\t\t\t\t\t\t isnull(a.Locked,'N') as Locked,\r\n\t\t\t\t\t\t\t\t\t isnull(a.Department,0) as Department,\r\n\t\t\t\t\t\t\t\t\t isnull(a.Branch,0) as Branch,\r\n\t\t\t\t\t\t\t\t\t  isnull (a.userSign,3) as userSign,  ISNULL(a.createDate,GETDATE()) 'createDate',\r\n\t                                 isnull (a.userSign2,0) as userSign2,\r\n\t\t\t\t\t\t\t\t\t a.updateDate,a.lastLogin\r\n\t\t\t\t\t\t\t\t\t ,ISNULL( b.Name,'') as Name,\r\n\t\t\t\t\t\t\t\t\t ISNULL( c.Name,'') as NameBranch\r\n                                from OUSR a                          \r\n\t\t\t\t\t\t\t\tleft join OUDP b on b.Code = a.Department\r\n                                left join  OUBR c on c.Code = a.Branch Where a.GROUPS != 99";
	}

	public DataTable Search(UsersImpl doc)
	{
		DataTable result = new DataTable();
		try
		{
			string text = GetSearchQuery();
			if (!string.IsNullOrEmpty(doc.USER_CODE))
			{
				text = text + " AND USER_CODE like N'%" + doc.USER_CODE + "%' ";
			}
			result = ExecuteData(text, CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Location Department - Doc ", ex.Message);
		}
		return result;
	}

	public int GetMaxCode()
	{
		try
		{
			string query = "SELECT ISNULL( MAX(USERID ),0) as USERID\r\n                            FROM OUSR ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["USERID"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	public bool Check(int code)
	{
		try
		{
			string query = "SELECT   A.USERID \r\n                            FROM OUSR A \r\n                            WHERE A.USERID = '" + code + "'";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public bool CheckUser_Code(string user)
	{
		try
		{
			string query = "SELECT   A.USER_CODE \r\n                            FROM OUSR A \r\n                            WHERE A.USER_CODE = '" + user + "'";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public string SaveDoc(UsersImpl doc)
	{
		string empty = string.Empty;
		List<SqlParameter> list = new List<SqlParameter>();
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				bool flag = CheckUser_Code(doc.USER_CODE);
				string superUser = GetSuperUser(doc.USER_CODE);
				int uSERID = doc.USERID;
				string text;
				if (!flag)
				{
					uSERID = Function.ParseInt(GetMaxCode() + 1);
					text = "INSERT INTO OUSR  (USERID, PASSWORD, USER_CODE, U_NAME, INTERNAL_K ,\r\n\t                                            GROUPS, SUPERUSER,  E_Mail, PortNum, Fax, Locked,Department,Branch,\r\n\t                                            userSign, createDate,userSign2,updateDate,lastLogin)\r\n                            VALUES \r\n                                        ( ";
					text += " @uSERID ";
					list.Add(new SqlParameter("@uSERID", uSERID));
					text += ", @pASSWORD ";
					list.Add(new SqlParameter("@pASSWORD", Function.ToString(doc.PASSWORD)));
					text += ", @uSER_CODE ";
					list.Add(new SqlParameter("@uSER_CODE", Function.ToString(doc.USER_CODE)));
					text += ", @u_NAME ";
					list.Add(new SqlParameter("@u_NAME", Function.ToString(doc.U_NAME)));
					text += ", @iNTERNAL_K ";
					list.Add(new SqlParameter("@iNTERNAL_K", uSERID));
					text += ", @gROUPS ";
					list.Add(new SqlParameter("@gROUPS", Function.ParseInt(doc.GROUPS)));
					text += ", @sUPERUSER ";
					list.Add(new SqlParameter("@sUPERUSER", Function.ToString(doc.SUPERUSER)));
					text += ", @e_Mail ";
					list.Add(new SqlParameter("@e_Mail", Function.ToString(doc.E_Mail)));
					text += ", @portNum ";
					list.Add(new SqlParameter("@portNum", Function.ToString(doc.PortNum)));
					text += ", @fax ";
					list.Add(new SqlParameter("@fax", Function.ToString(doc.Fax)));
					text += ", @locked ";
					list.Add(new SqlParameter("@locked", Function.ToString(doc.Locked)));
					text += ", @Department ";
					list.Add(new SqlParameter("@Department", Function.ParseInt(doc.Department)));
					text += ", @branch ";
					list.Add(new SqlParameter("@branch", Function.ParseInt(doc.Branch)));
					text += ", @_userSign ";
					list.Add(new SqlParameter("@_userSign", Function.ParseInt(doc.userSign)));
					text += ", @_createDate ";
					list.Add(new SqlParameter("@_createDate", Function.ParseDateTime(doc.createDate)));
					text += ", @_userSign2 ";
					list.Add(new SqlParameter("@_userSign2", Function.ParseInt(doc.userSign2)));
					text += ", @_updateDate ";
					list.Add(new SqlParameter("@_updateDate", Function.ParseDateTime(doc.updateDate)));
					text += ", @_lastLogin ";
					list.Add(new SqlParameter("@_lastLogin", Function.ParseDateTime(doc.lastLogin)));
					text += " )";
				}
				else
				{
					list = new List<SqlParameter>();
					text = "UPDATE OUSR  \r\n                            SET ";
					text += "USER_CODE= @uSER_CODE ";
					list.Add(new SqlParameter("@uSER_CODE", Function.ToString(doc.USER_CODE)));
					if (doc.IsChangePassword)
					{
						text += ",PASSWORD= @pASSWORD ";
						list.Add(new SqlParameter("@pASSWORD", Function.ToString(doc.PASSWORD)));
					}
					text += ", U_NAME=@u_NAME ";
					list.Add(new SqlParameter("@u_NAME", Function.ToString(doc.U_NAME)));
					text += ",GROUPS= @gROUPS ";
					list.Add(new SqlParameter("@gROUPS", Function.ParseInt(doc.GROUPS)));
					text += ",SUPERUSER= @sUPERUSER ";
					list.Add(new SqlParameter("@sUPERUSER", Function.ToString(doc.SUPERUSER)));
					text += ",E_Mail= @e_Mail ";
					list.Add(new SqlParameter("@e_Mail", Function.ToString(doc.E_Mail)));
					text += ",PortNum= @portNum ";
					list.Add(new SqlParameter("@portNum", Function.ToString(doc.PortNum)));
					text += ",Fax= @fax ";
					list.Add(new SqlParameter("@fax", Function.ToString(doc.Fax)));
					text += ",Locked= @locked ";
					list.Add(new SqlParameter("@locked", Function.ToString(doc.Locked)));
					text += ",Department= @Department ";
					list.Add(new SqlParameter("@Department", Function.ParseInt(doc.Department)));
					text += ",Branch= @branch ";
					list.Add(new SqlParameter("@branch", Function.ParseInt(doc.Branch)));
					text += ",userSign= @_userSign ";
					list.Add(new SqlParameter("@_userSign", Function.ParseInt(doc.userSign)));
					text += ",createDate= @_createDate ";
					list.Add(new SqlParameter("@_createDate", Function.ParseDateTime(doc.createDate)));
					text += ",userSign2= @_userSign2 ";
					list.Add(new SqlParameter("@_userSign2", Function.ParseInt(doc.userSign2)));
					text += ",updateDate= @_updateDate ";
					list.Add(new SqlParameter("@_updateDate", Function.ParseDateTime(doc.updateDate)));
					text += ",lastLogin= @_lastLogin ";
					list.Add(new SqlParameter("@_lastLogin", Function.ParseDateTime(doc.lastLogin)));
					text += " WHERE USERID =@uSERID";
					list.Add(new SqlParameter("@uSERID", Function.ParseInt(doc.USERID)));
				}
				if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) > 0)
				{
					string text2 = "";
					if (string.Compare(doc.SUPERUSER, "y", ignoreCase: true) == 0)
					{
						text2 = SaveAuthorization(doc.DTAuth, "F");
					}
					else if (string.Compare(superUser, "y", ignoreCase: true) == 0)
					{
						text2 = SaveAuthorization(doc.DTAuth, "N");
					}
					if (!string.IsNullOrEmpty(text2))
					{
						return text2;
					}
					if (doc.UserBranchAssignment != null && doc.UserBranchAssignment.Count > 0)
					{
						text = "Delete USR6 where usercode=@userCode\r\ninsert into usr6 (UserCode,BPLId,DigCrtPath) values ";
						list.Clear();
						list.Add(new SqlParameter("@userCode", Function.ToString(doc.USER_CODE)));
						for (int i = 0; i < doc.UserBranchAssignment.Count; i++)
						{
							text = text + Environment.NewLine + "(@userCode, @BPLId" + i + ", null),";
							list.Add(new SqlParameter("@BPLId" + i, doc.UserBranchAssignment[i].BPLId));
						}
						if (text.EndsWith(","))
						{
							text = text.Remove(text.Length - 1);
						}
						ExecuteNonQuery(text, CommandType.Text, list.ToArray());
					}
					dbTransaction.Commit();
					return string.Empty;
				}
				return "ERROR";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	private string SaveAuthorization(DataTable dt, string auth)
	{
		try
		{
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				dt.Rows[i]["authoriza"] = auth;
			}
			dt.AcceptChanges();
			return GetUnitOfWork().Auths.SaveData(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string GetSuperUser(string uSER_CODE)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select SUPERUSER from OUSR where USER_CODE=@usercode", CommandType.Text, new SqlParameter("@usercode", uSER_CODE));
			if (dataTable.IsNotNull())
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
			return "N";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "N";
		}
	}

	public string DeleteDoc(int code)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				SqlParameter sqlParameter = new SqlParameter("@userid", code);
				if (ExecuteNonQuery("delete OUSR where USERID = @userid", CommandType.Text, sqlParameter) > 0)
				{
					dbTransaction.Commit();
					return string.Empty;
				}
				return "ERROR";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string Insert(UsersImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(UsersImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(UsersImpl entity)
	{
		throw new NotImplementedException();
	}

	public UsersImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public DataTable ShowBranch()
	{
		try
		{
			return ExecuteDataTable("select  \"Code\" , \"Name\"\r\n                                from OUBR;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string UpdateUsersAcount(UsersImpl doc)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			string empty = string.Empty;
			List<SqlParameter> list = new List<SqlParameter>();
			try
			{
				bool flag = CheckUsersAcount(doc.USER_CODE);
				string query = string.Empty;
				if (flag)
				{
					list = new List<SqlParameter>();
					query = "UPDATE OUSR  \r\n                            SET ";
					query += "PASSWORD= @pASSWORD ";
					list.Add(new SqlParameter("@pASSWORD", Function.ToString(doc.PASSWORD)));
					query += " WHERE USER_CODE =@uSER_CODE";
					list.Add(new SqlParameter("@uSER_CODE", Function.ToString(doc.USER_CODE)));
				}
				if (ExecuteNonQuery(query, CommandType.Text, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return null;
				}
				dbTransaction.Rollback();
				return "ERROR";
			}
			catch (Exception ex)
			{
				dbTransaction.Dispose();
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
				empty = ex.Message;
			}
			return empty;
		}
	}

	public bool CheckUsersAcount(string userCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT   A.USER_CODE \r\n                            FROM OUSR A \r\n                            WHERE A.USER_CODE = @UserCode", CommandType.Text, new SqlParameter("@UserCode", userCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public DataTable GetUserLogin(string madk)
	{
		try
		{
			return ExecuteDataTable(GetUserLoginQuery(), CommandType.Text, new SqlParameter("@Usercode", madk));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetUserLoginQuery()
	{
		return "select top 1 a.UserID,isnull( a.USER_CODE,'') as USER_CODE , isnull(a.U_NAME,'') as U_NAME, isnull(a.Department,0)as Department, isnull(b.Name,'') as Name,isnull(a.Branch,-1) as Branch, isnull( c.Name,'')as Name ,isnull (a.UserSign,0) as UserSign, isnull( a.WallPaper,'') as WallPaper, isnull( a.WllPprDsp,0) as WllPprDsp\r\n                             from OUSR a left join  OUDP b ON a.Department=b.Code\r\n                             left join OUBR c ON c.Code=a.Branch where a.USER_CODE = @Usercode";
	}

	public DataTable GetUserSign(string nameLogin)
	{
		try
		{
			return ExecuteDataTable(GetUserSignQuery(), CommandType.Text, new SqlParameter("@Usercode", nameLogin));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetUserSignQuery()
	{
		return "select ISNULL( UserSign,-1) as UserSign,  isnull(USERID,-1) as USERID  from OUSR Where USER_CODE = @Usercode ";
	}

	public DataTable GetUsersByCode(string userCode)
	{
		DataTable result = new DataTable();
		try
		{
			result = ExecuteDataTable(GetUserByCodeQuery(), CommandType.Text, new SqlParameter("@Usercode", userCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataSet GetAll()
	{
		try
		{
			return ExecuteData("apz_users_getall", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public UserDefaultGroupImpl GetUserDefaultGroup(string username)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@UserName", username);
			DataTable dataTable = new DataTable();
			dataTable = ExecuteDataTable(GetUserDefaultGroupQuery(), CommandType.Text, sqlParameter);
			if (dataTable.IsNotNull())
			{
				UserDefaultGroupImpl userDefaultGroupImpl = new UserDefaultGroupImpl();
				DataRow dataRow = dataTable.Rows[0];
				userDefaultGroupImpl.Address = Function.ToString(dataRow["Address"]);
				userDefaultGroupImpl.BPLID = Function.ParseInt(dataRow["BPLId"]);
				userDefaultGroupImpl.CashAccount = Function.ToString(dataRow["CashAcct"]);
				userDefaultGroupImpl.CheckingAcct = Function.ToString(dataRow["CheckAcct"]);
				userDefaultGroupImpl.Code = Function.ToString(dataRow["Code"]);
				userDefaultGroupImpl.Country = Function.ToString(dataRow["Country"]);
				userDefaultGroupImpl.DefaultTaxCode = Function.ToString(dataRow["DflTaxCode"]);
				userDefaultGroupImpl.eMail = Function.ToString(dataRow["E_Mail"]);
				userDefaultGroupImpl.FaxNumber = Function.ToString(dataRow["Fax"]);
				userDefaultGroupImpl.FaxNumberForeignLang = Function.ToString(dataRow["FrgnFax"]);
				userDefaultGroupImpl.Name = Function.ToString(dataRow["Name"]);
				userDefaultGroupImpl.PhoneNumber1 = Function.ToString(dataRow["Phone1"]);
				userDefaultGroupImpl.PhoneNumber1ForeignLang = Function.ToString(dataRow["FrgnPhone1"]);
				userDefaultGroupImpl.PhoneNumber2 = Function.ToString(dataRow["Phone2"]);
				userDefaultGroupImpl.PhoneNumber2ForeignLang = Function.ToString(dataRow["FrgnPhone2"]);
				userDefaultGroupImpl.PrintingHeader = Function.ToString(dataRow["PrintHeadr"]);
				userDefaultGroupImpl.PrintingHeaderInForeignLangu = Function.ToString(dataRow["FrnPrntHdr"]);
				userDefaultGroupImpl.SalesEmployee = Function.ParseInt(dataRow["SalePerson"]);
				userDefaultGroupImpl.Warehouse = Function.ToString(dataRow["Warehouse"]);
				return userDefaultGroupImpl;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new UserDefaultGroupImpl();
	}

	protected virtual string GetUserDefaultGroupQuery()
	{
		return "SELECT [Code],\t\t[Name],\t\t[Warehouse],\t[SalePerson]\r\n                                  ,[ICTCard],\t\t[CashAcct],\t[CheckAcct],\t[CreditCard]\r\n                                  ,[PrintRcpt],\t\t[ShortRcpt],[Color]\r\n                                  ,[Address],\t\t[Country],\t[PrintHeadr]\r\n                                  ,[Phone1],\t\t[Phone2],\t[Fax]\r\n                                  ,[E_Mail],\t\t[FrgnAddr],\t[FrnPrntHdr]\r\n                                  ,[FrgnPhone1],\t[FrgnPhone2],\t\t\t\t[FrgnFax]\r\n                                  ,[DflTaxCode],\t[FreeZoneNo],\t\t\t\t[UserSign]\r\n                                  ,[free1],\t\t\t[UseTax],\t\t\t\t\t[AdrsFromWh]\r\n                                  ,[Language],\t\t[Font],\t\t                [FontSize]\r\n                                  ,[BPLId],\t\t\t[AssetInDoc],[AttachPath],[DflPTICode] \r\n                             FROM [OUDG]\r\n                              WHERE Code=(SELECT TOP 1 DfltsGroup FROM ousr WHERE USER_CODE=@UserName)";
	}

	public string GetUserNameById(int id)
	{
		try
		{
			return Function.ToString(ExecuteScalar("select U_NAME from OUSR where USERID = @id;", CommandType.Text, new SqlParameter("@id", id)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public string GetLstUserNameByCode(string lstCode)
	{
		try
		{
			return Function.ToString(ExecuteScalar(GetLstUserNameByCodeQuery(), CommandType.Text, new SqlParameter("@lstCode", lstCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	protected virtual string GetLstUserNameByCodeQuery()
	{
		return "Select  convert(nvarchar,USER_CODE) + ';' FROM OUSR WHERE USERID in (select data from split(@lstCode, ';')) FOR XML PATH('')";
	}

	public string GetUserCodeById(int id)
	{
		try
		{
			return Function.ToString(ExecuteScalar("select USER_CODE from OUSR where USERID = @id;", CommandType.Text, new SqlParameter("@id", id)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public string GetUserIdByEmpId(int empID)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@empID", empID);
			return Function.ToString(ExecuteScalar("SELECT \"userId\" FROM OHEM WHERE \"empID\"=@empID;", CommandType.Text, sqlParameter));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public DataTable GetUserBranchAssignment(string userCode)
	{
		try
		{
			string text = "";
			DataTable dataTable = new DataTable();
			if (string.IsNullOrWhiteSpace(userCode))
			{
				text = "select \t\"BPLId\",\"BPLName\",\"AliasName\",\"BPLFrName\",\"County\",\"State\",\"Country\",\"Disabled\" ,\t'N' \"Checked\" from obpl order by \"BPLId\";";
				dataTable = ExecuteDataTable(text, CommandType.Text);
			}
			else
			{
				text = "SELECT a.\"BPLId\", \"BPLName\", \"AliasName\", \"BPLFrName\", \"County\", \"State\", \"Country\", \"Disabled\"\r\n        , CASE WHEN b.\"BPLId\" IS NULL THEN 'N' ELSE 'Y' END AS \"Checked\" FROM obpl a LEFT JOIN USR6 b ON a.\"BPLId\" = b.\"BPLId\" AND \"UserCode\" = @userCode order by a.\"BPLId\";";
				dataTable = ExecuteDataTable(text, CommandType.Text, new SqlParameter("@userCode", userCode));
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	public DataTable GetEmployee()
	{
		try
		{
			return ExecuteDataTable(GetEmployeeQuery(), CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetEmployeeQuery()
	{
		return "select lastName+','+firstName Name,empID Code from ohem";
	}

	public DataTable LoadData()
	{
		try
		{
			return ExecuteDataTable("select USERID,USER_CODE from ousr where groups <> 99;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetAnUser(int userId)
	{
		try
		{
			return ExecuteDataTable(GetAnUserQuery(), CommandType.Text, new SqlParameter("@userId_", userId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	protected virtual string GetAnUserQuery()
	{
		return "Select a.*,(select top 1 empID from OHEM t where t.userid = a.userid) empID from OUSR a where userid=@userId_";
	}

	protected virtual string GetUserByCodeQuery()
	{
		return "select  a.USERID,a.PASSWORD, a.USER_CODE,a.U_NAME, ISNULL( a.INTERNAL_K,0) as INTERNAL_K ,\r\n\t                                ISNULL\t(a.GROUPS,0) as GROUPS, \r\n\t\t\t\t\t\t\t\t\ta.SUPERUSER, \r\n\t\t\t\t\t\t\t\t\tisnull( a.E_Mail,'') as E_Mail\r\n\t\t\t\t\t\t\t\t\t, isnull( a.PortNum,'') as PortNum,\r\n\t                                isnull(a.Fax,'') as Fax,\r\n\t\t\t\t\t\t\t\t\t isnull(a.Locked,'N') as Locked,\r\n\t\t\t\t\t\t\t\t\t isnull(a.Department,0) as Department,\r\n\t\t\t\t\t\t\t\t\t isnull(a.Branch,0) as Branch,\r\n\t\t\t\t\t\t\t\t\t  isnull (a.userSign,3) as userSign,  ISNULL(a.createDate,GETDATE()) 'createDate',\r\n\t                                 isnull (a.userSign2,0) as userSign2,\r\n\t\t\t\t\t\t\t\t\t a.updateDate,a.lastLogin\r\n\t\t\t\t\t\t\t\t\t ,ISNULL( b.Name,'') as Name,\r\n\t\t\t\t\t\t\t\t\t ISNULL( c.Name,'') as NameBranch\r\n                                from OUSR a                          \r\n\t\t\t\t\t\t\t\tleft join OUDP b on b.Code = a.Department\r\n                                left join  OUBR c on c.Code = a.Branch Where USER_CODE = @Usercode";
	}

	protected virtual string GetSearchQuery()
	{
		return "select  a.USERID,a.PASSWORD, a.USER_CODE,a.U_NAME, ISNULL( a.INTERNAL_K,0) as INTERNAL_K ,\r\n\t                                ISNULL\t(a.GROUPS,0) as GROUPS, a.SUPERUSER, isnull( a.E_Mail,'') as E_Mail, isnull( a.PortNum,'') as PortNum,\r\n\t                                isnull(a.Fax,'') as Fax, a.Locked,a.Department,a.Branch, isnull (a.userSign,3) as userSign,  ISNULL(a.createDate,GETDATE()) 'createDate',\r\n\t                                 isnull (a.userSign2,0) as userSign2,a.updateDate,a.lastLogin,ISNULL( b.Name,'') as Name,ISNULL( c.Name,'') as NameBranch\r\n                                from OUSR a                          \r\n\t\t\t\t\t\t\t\tleft join OUDP b on b.Code = a.Department\r\n                                left join  OUBR c on c.Code = a.Branch\r\n                                Where 1=1  and a.GROUPS != 99";
	}

	public DataTable DataPassAdmin()
	{
		try
		{
			string query = "Select * from OPPA";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}
}
