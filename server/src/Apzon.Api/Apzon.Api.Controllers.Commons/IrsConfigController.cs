using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Web.Configuration;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Services;

namespace Apzon.Api.Controllers.Commons;

public class IrsConfigController : BaseApiController
{
	[HttpGet]
	public DataTable GetTableStructure(string tblName)
	{
		try
		{
			return UnitOfWork.IrsConfig.GetTableStructure(tblName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpPost]
	public string UpdateConfigConnection(Tuple<string, string, string, string, string, string> updateData)
	{
		try
		{
			string item = updateData.Item1;
			string item2 = updateData.Item2;
			string item3 = updateData.Item3;
			string item4 = updateData.Item4;
			string item5 = updateData.Item5;
			string item6 = updateData.Item6;
			if (!item.ToLower().Equals(Global.SuperUser) || !(item2 == Global.PassSuperUser))
			{
				return "Cannot authorization";
			}
			UnitOfWorkFactory.GetUnitOfWork((Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA) ? $"Server = {item3};CS={Global.CommonDb}; UserID = {item5}; Password = {item6}" : string.Format("Data Source={0}; Initial Catalog={3}; Persist Security Info=True; User ID={1}; Password={2}; ", item3, item5, item6, string.IsNullOrEmpty(Global.CommonDb) ? "SBO-COMMON" : Global.CommonDb), Global.DatabaseSystemType).Commons.ExecuteScalar("select 1", CommandType.Text);
			Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
			AppSettingsSection appSettingsSection = (AppSettingsSection)configuration.GetSection("appSettings");
			appSettingsSection.Settings["CommonDbServerType"].Value = item4;
			appSettingsSection.Settings["CommonDbUserName"].Value = Function.Encrypt(RSA.PublicKey, item5);
			appSettingsSection.Settings["CommonDbPassword"].Value = Function.Encrypt(RSA.PublicKey, item6);
			appSettingsSection.Settings["CommonDbServer"].Value = Function.Encrypt(RSA.PublicKey, item3);
			configuration.Save();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "Cannot update config connection";
		}
		return "";
	}
}
