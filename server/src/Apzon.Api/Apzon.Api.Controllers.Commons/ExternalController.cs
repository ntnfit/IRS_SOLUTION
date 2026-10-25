using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.Api.Controllers.Commons;

public class ExternalController : ApiController
{
	[HttpPost]
	public HttpResult ApprovalRequest([FromBody] HttpResult dataRequest)
	{
		try
		{
			string text = Function.ToString(Request.Headers.GetValues("Token").FirstOrDefault());
			if (string.IsNullOrEmpty(text))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Token request is null");
				return new HttpResult(Enums.MessageCode.Error, "Invalid Request");
			}
			string text2 = Function.ToString(dataRequest.Content);
			if (string.IsNullOrEmpty(text2))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Data request is null");
				return new HttpResult(Enums.MessageCode.Error, "Request is not real");
			}
			string text3;
			try
			{
				text3 = Function.Decrypt(RSA.PrivateKey, text2);
			}
			catch (Exception)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Data request is not real. Data: " + dataRequest);
				return new HttpResult(Enums.MessageCode.Error, "Request is not real");
			}
			string[] array = text3.Split(';');
			if (array.Length != 5)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "The amount of Data is not enough");
				return new HttpResult(Enums.MessageCode.Error, "Request is not real");
			}
			DataTable infoCompanyDbServer = Apzon.Services.UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.DatabaseSystemType).Commons.GetInfoCompanyDbServer(array[0]);
			if (infoCompanyDbServer == null)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Can not get infomation database form common");
				return new HttpResult(Enums.MessageCode.Error, "Request is not real");
			}
			if (infoCompanyDbServer.Rows.Count == 0)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Can not get infomation database form common");
				return new HttpResult(Enums.MessageCode.Error, "Request is not real");
			}
			string text4;
			string text5;
			string text6;
			try
			{
				if (Function.ToString(infoCompanyDbServer.Rows[0][3]).Equals("P"))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Database is in the update period. You can not login");
					return new HttpResult(Enums.MessageCode.Error, "Database is in the update period. You can not login");
				}
				if (Function.ToString(infoCompanyDbServer.Rows[0][3]).Equals("N"))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Database is inactive");
					return new HttpResult(Enums.MessageCode.Error, "Database is inactive");
				}
				text4 = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][0]));
				text5 = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][1]));
				text6 = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][2]));
			}
			catch
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Could not get infomation database to login");
				return new HttpResult(Enums.MessageCode.Error, "Request is not real");
			}
			string text7 = ((Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA) ? $"Server = {text4};CS=\"{array[0]}\"; UserID = {text5}; Password = {text6}" : string.Format("Data Source={0}; Initial Catalog={3}; Persist Security Info=True; User ID={1}; Password={2}; ", text4, text5, text6, array[0]));
			Apzon.Services.IUnitOfWork unitOfWork = Apzon.Services.UnitOfWorkFactory.GetUnitOfWork(text7, Global.DatabaseSystemType);
			if (!unitOfWork.ExternalRequest.CheckExternalRequestToken(text))
			{
				return new HttpResult(Enums.MessageCode.Error, "Request is out of date");
			}
			int num = Function.ParseInt(unitOfWork.Commons.ExecuteScalar("select top 1 \"UserID\" from OUSR where \"USER_CODE\" = @UserCode;", CommandType.Text, new SqlParameter("@UserCode", array[1])));
			if (num <= 0)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "User Name does not exist");
				return new HttpResult(Enums.MessageCode.Error, "Request is not real");
			}
			Apzon.DataServices.IUnitOfWork unitOfWork2 = Apzon.DataServices.UnitOfWorkFactory.GetUnitOfWork(text7, Global.ServiceAddress, Global.DatabaseSystemType, Global.ServiceType);
			HttpResult httpResult = unitOfWork2.User.Login(Global.CommonDbServer, array[0], Global.CommonDbServerType, array[1], array[2], Global.CommonLanguage, Global.LicenseServer, Global.CommonDbUserName, Global.CommonDbPassword, "");
			if (httpResult == null || !string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Login Failed with data request");
				return new HttpResult(Enums.MessageCode.Error, "Request is not real");
			}
			string sessionId = Function.ToString(httpResult.Content).Split(',')[0];
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("WddCode", typeof(int));
			dataTable.Columns.Add("Password", typeof(string));
			dataTable.Columns.Add("status", typeof(string));
			dataTable.Columns.Add("Remarks", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow[0] = Function.ParseInt(array[3]);
			dataRow[1] = EncodePassword.EncryptString(array[2], RSA.PasswordPhrase);
			dataRow[2] = array[4];
			dataRow[3] = dataRequest.Message;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			string message = unitOfWork2.ApprovalRequests.ApproveDraft(sessionId, array[1], num, dataTable, string.IsNullOrWhiteSpace(Global.AppDataFolder) ? HttpContext.Current.Server.MapPath("~/App_Data/") : Global.AppDataFolder);
			unitOfWork.ExternalRequest.SaveExternalRequestToken(text, "C", "");
			return new HttpResult(Enums.MessageCode.Success, message);
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
	}
}
