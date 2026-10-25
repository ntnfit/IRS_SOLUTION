using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class LoginIRSRepository : BaseService<ILoginIRS>, ILoginIRS
{
	public LoginIRSRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public HttpResult LoginIrs(string user, string pass, string dbName)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@user", user),
				new SqlParameter("@pass", Function.GetMd5Hash(pass))
			};
			DataTable dataTable = ExecuteDataTable("select Locked from OUSR where USER_CODE = @user and PASSWORD = @pass", CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count == 1)
			{
				if (Function.ToString(dataTable.Rows[0]["Locked"]) == "N")
				{
					return new HttpResult
					{
						Message = "",
						Content = Function.RandomSession(user, dbName)
					};
				}
				return new HttpResult
				{
					Message = "This account is locked"
				};
			}
			return new HttpResult
			{
				Message = "The username or password is incorrect"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult
			{
				Message = "Could not connect sql server"
			};
		}
	}

	public string GetServerDate()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable(GetServerDateQuery(), CommandType.Text);
			if (dataTable.Rows.Count > 0)
			{
				return Function.ParseDateTimes(dataTable.Rows[0][0]).ToString("yyyyMMdd");
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
		return "SELECT GETDATE()";
	}

	public HttpResult CheckLicenseIrs(string hardwareKeyServer, string userName, string dbName, string clientVersion)
	{
		try
		{
			string serverDate = GetServerDate();
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@user", userName.ToLower()),
				new SqlParameter("@database", dbName),
				new SqlParameter("@hardKey", hardwareKeyServer)
			};
			DataSet dataSet = ExecuteData(GetCheckLicenseIrsGetLicenseQuery(), CommandType.Text, list.ToArray());
			if (dataSet != null && dataSet.Tables.Count == 3 && dataSet.Tables[0].IsNotNull())
			{
				if (Function.ParseInt(Function.Decrypt(RSA.PrivateKey, Function.ToString(dataSet.Tables[0].Rows[0]["DateRequest"]))) > Function.ParseInt(serverDate))
				{
					return new HttpResult
					{
						Content = -1,
						Message = "Date server is invalid"
					};
				}
				Function.ReadLicense(Function.ToString(dataSet.Tables[0].Rows[0]["License"]), out var _, out var installNumber, out var hardwareKey, out var _, out var _, out var _, out var version, out var lstLicense);
				if (!Function.CompareVersionLicenseAndVersionIrs(version, clientVersion))
				{
					return new HttpResult
					{
						Message = "License File is invalid. License File use for other diffirent Irs version"
					};
				}
				if (!hardwareKey.Equals(hardwareKeyServer))
				{
					return new HttpResult
					{
						Message = "License is invalid. Login with administrator account to import license"
					};
				}
				if (!Function.ToString(dataSet.Tables[0].Rows[0]["InstallNumber"]).Equals(installNumber))
				{
					return new HttpResult
					{
						Message = "License is invalid. Login with administrator account to import license"
					};
				}
				UserLicenseImpl userLicense = new UserLicenseImpl();
				userLicense.UserCode = userName;
				userLicense.IsUserLicense = false;
				userLicense.LstLicenseMapping = new List<LicenseInfoImpl>();
				DataTable dataTable = dataSet.Tables[2];
				DataTable source = dataSet.Tables[1];
				LicenseRule licenseRule = LicenseRule.Service;
				if (dataTable.IsNotNull())
				{
					LicenseRule licenseRule2 = LicenseRule.Service;
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						string licenseType = Function.ToString(dataTable.Rows[i]["LicenseType"]);
						DataRow dataRow = source.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["LicenseType"]).Equals(licenseType));
						int num = ((dataRow != null) ? Function.ParseInt(dataRow["LicenseUse"]) : 0);
						LicenseInfoImpl licenseInfoImpl = lstLicense.FirstOrDefault((LicenseInfoImpl t) => t.TypeCode.Equals(licenseType));
						if (licenseInfoImpl == null || licenseInfoImpl.Amount < num)
						{
							return new HttpResult
							{
								Content = -1,
								Message = "Data base casting arbitrarily be changed"
							};
						}
						bool licenseStatus = (licenseInfoImpl.IsMappingUser = Function.ParseDateTimes(serverDate, "yyyyMMdd") <= licenseInfoImpl.ExpiredDate);
						licenseInfoImpl.LicenseStatus = licenseStatus;
						licenseInfoImpl.Used = num;
						if (licenseInfoImpl.LicenseStatus)
						{
							userLicense.IsUserLicense = true;
							if (licenseInfoImpl.LicenseRule == LicenseRule.Normal)
							{
								if (licenseRule2 == LicenseRule.Service)
								{
									licenseRule2 = LicenseRule.Normal;
								}
								if (licenseRule == LicenseRule.Service)
								{
									licenseRule = LicenseRule.Normal;
								}
							}
							if (licenseInfoImpl.LicenseRule == LicenseRule.Super)
							{
								licenseRule2 = (licenseRule = LicenseRule.Super);
							}
						}
						userLicense.LstLicenseMapping.Add(licenseInfoImpl);
					}
					userLicense.UserRule = licenseRule2;
				}
				List<LicenseInfoImpl> list2 = lstLicense.Where((LicenseInfoImpl t) => userLicense.LstLicenseMapping.All((LicenseInfoImpl x) => !x.TypeCode.Equals(t.TypeCode))).ToList();
				for (int num2 = 0; num2 < list2.Count; num2++)
				{
					LicenseInfoImpl licenseInfo = list2[num2];
					DataRow dataRow2 = source.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["LicenseType"]).Equals(licenseInfo.TypeCode));
					int num3 = ((dataRow2 != null) ? Function.ParseInt(dataRow2["LicenseUse"]) : 0);
					if (licenseInfo.Amount < num3)
					{
						return new HttpResult
						{
							Content = -1,
							Message = "Data base casting arbitrarily be changed"
						};
					}
					licenseInfo.LicenseStatus = Function.ParseDateTimes(serverDate, "yyyyMMdd") <= licenseInfo.ExpiredDate;
					licenseInfo.IsMappingUser = false;
					licenseInfo.Used = num3;
					if (licenseInfo.LicenseStatus)
					{
						if (licenseInfo.LicenseRule == LicenseRule.Normal && licenseRule == LicenseRule.Service)
						{
							licenseRule = LicenseRule.Normal;
						}
						if (licenseInfo.LicenseRule == LicenseRule.Super)
						{
							licenseRule = LicenseRule.Super;
						}
					}
					userLicense.LstLicenseMapping.Add(licenseInfo);
				}
				userLicense.Rule = licenseRule;
				userLicense.LstAccessFormInLicense = new List<string>();
				userLicense.LstAccessFunctionOfUser = new List<string>();
				for (int num4 = 0; num4 < userLicense.LstLicenseMapping.Count; num4++)
				{
					if (userLicense.LstLicenseMapping[num4].LstAccessForm.Any() && userLicense.LstLicenseMapping[num4].LicenseStatus)
					{
						userLicense.LstAccessFormInLicense = userLicense.LstAccessFormInLicense.Union(userLicense.LstLicenseMapping[num4].LstAccessForm).ToList();
						if (userLicense.LstLicenseMapping[num4].IsMappingUser)
						{
							userLicense.LstAccessFunctionOfUser = userLicense.LstAccessFunctionOfUser.Union(userLicense.LstLicenseMapping[num4].LstAccessForm).ToList();
						}
					}
				}
				if (!userLicense.IsUserLicense)
				{
					return new HttpResult
					{
						Content = userLicense,
						Message = (dataTable.IsNotNull() ? "License has expired" : "Your account has not been granted a license")
					};
				}
				ExecuteNonQuery(parameters: new SqlParameter[2]
				{
					new SqlParameter("@dateRespuest", Function.Encrypt(RSA.PublicKey, serverDate)),
					new SqlParameter("@insNum", installNumber)
				}, query: GetGetupdateDateRequestQuery());
				return new HttpResult
				{
					Content = userLicense,
					Message = ""
				};
			}
			return new HttpResult
			{
				Message = "License is invalid. Login with administrator account to import license"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult
			{
				Content = -1,
				Message = $"License File is invalid. {((ex.Message.Length > 60) ? ex.Message.Substring(0, 60) : ex.Message)}"
			};
		}
	}

	protected virtual string GetCheckLicenseIrsGetLicenseQuery()
	{
		return "declare @installNumber nvarchar(50) = iSnull((select top 1 InstallNumber from APZ_OALC where HardwareKey = @hardKey), '');\r\n                    Select top 1 InstallNumber, License, DateRequest from APZ_OALC where HardwareKey = @hardKey;\r\n\r\n                    Select LicenseType, COUNT(1) LicenseUse from (select LicenseType from APZ_LOU where InstallNumber = @installNumber group by LicenseType, [User]) x group by LicenseType;\r\n\r\n                    Select LicenseType from APZ_LOU where InstallNumber = @installNumber and [User] = @user and [DataBase] = @database;";
	}

	protected virtual string GetGetupdateDateRequestQuery()
	{
		return "update APZ_OALC set DateRequest = @dateRespuest where InstallNumber = @insNum";
	}

	public string CheckAdministrator(string userName, string superUser)
	{
		try
		{
			DataSet dataSet = ExecuteData(GetCheckAdministratorQuery(), CommandType.Text, new SqlParameter("@user", userName), new SqlParameter("@userAdministrator", superUser));
			if (dataSet == null || dataSet.Tables.Count < 2)
			{
				return "Could not check user from database";
			}
			if (dataSet.Tables[0].IsNotNull())
			{
				return "Data base casting arbitrarily be changed";
			}
			if (!dataSet.Tables[1].IsNotNull())
			{
				return "The username or password is incorrect";
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return (ex.Message.Length > 100) ? ex.Message.Substring(0, 100) : ex.Message;
		}
	}

	protected virtual string GetCheckAdministratorQuery()
	{
		return "";
	}

	public HttpResult CheckTrialDatabase(string username, string dbName)
	{
		try
		{
			if (!username.ToLower().Equals("manager".ToLower()))
			{
				return new HttpResult(Enums.MessageCode.Error, "You must login with user manager to badatase trial");
			}
			string text = Function.ToString(ExecuteScalar("Select top 1 TextCode from APZ_CINF", CommandType.Text));
			if (!string.IsNullOrEmpty(text))
			{
				text = Function.Decrypt(RSA.PrivateKey, text);
				string[] array = text.Split('#');
				if (dbName.Equals(array[0]) && !string.IsNullOrEmpty(Function.ToString(array[1])))
				{
					DateTime? dateTime = Function.ParseDateTime(array[1], "yyyyMMdd");
					DateTime dateTime2 = DateTime.Now.Date.AddDays(-30.0);
					if (dateTime.HasValue && dateTime.GetValueOrDefault() >= dateTime2 && Function.ParseDateTime(array[1], "yyyyMMdd") <= DateTime.Now.Date)
					{
						if (Function.ParseInt(ExecuteScalar("Select top 1 1 from OILM", CommandType.Text)) != 1)
						{
							return new HttpResult
							{
								Content = "P"
							};
						}
						if (array.Length == 9)
						{
							DataTable dataTable = ExecuteDataTable("Select Quantity, LocCode, ItemCode, DocDate,TransType,DocEntry from OILM where MessageID = @code", CommandType.Text, new SqlParameter("@code", Function.ParseInt(array[2])));
							if (dataTable.IsNotNull() && !string.IsNullOrEmpty(array[5]))
							{
								dateTime = Function.ParseDateTime(array[5], "yyyyMMdd");
								dateTime2 = Function.ParseDateTimes(dataTable.Rows[0][3]);
								if (dateTime.HasValue && (!dateTime.HasValue || dateTime.GetValueOrDefault() == dateTime2) && Function.ToString(array[6]).Equals(Function.ToString(dataTable.Rows[0][2])) && Math.Abs(Function.ParseDouble(array[8]) - Function.ParseDouble(dataTable.Rows[0][0])) < 0.001 && Function.ToString(array[7]).Equals(Function.ToString(dataTable.Rows[0][1])) && Function.ToString(array[4]).Equals(Function.ToString(dataTable.Rows[0][5])) && Function.ToString(array[3]).Equals(Function.ToString(dataTable.Rows[0][4])))
								{
									return new HttpResult
									{
										Content = "P"
									};
								}
							}
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Error, "Could not check trial");
	}
}
