using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Apzon.Api.Controllers.Commons;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Kit;
using Apzon.Entities.Models.Common;
using Apzon.Services;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace Apzon.Api.Controllers.Administrator;

public class ApprovalDecisionController : Controller
{
	[HttpGet]
	public ActionResult Index(string token, string key)
	{
		ViewBag.Key = key;
		ViewBag.Token = token;
		string value = "";
		try
		{
			string[] array = Function.Decrypt(RSA.PrivateKey, key).Split(';');
			DataTable infoCompanyDbServer = UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.DatabaseSystemType).Commons.GetInfoCompanyDbServer(array[0]);
			string text = "";
			string text2 = "";
			string text3 = "";
			try
			{
				text = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][0]));
				text2 = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][1]));
				text3 = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][2]));
			}
			catch
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Could not get infomation database to login");
			}
			IUnitOfWork unitOfWork = UnitOfWorkFactory.GetUnitOfWork((Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA) ? $"Server = {text};CS=\"{array[0]}\"; UserID = {text2}; Password = {text3}" : string.Format("Data Source={0}; Initial Catalog={3}; Persist Security Info=True; User ID={1}; Password={2}; ", text, text2, text3, array[0]), Global.DatabaseSystemType);
			int wddCode = Function.ParseInt(array[3]);
			DataTable sourceMailContent = unitOfWork.ApprovalDecision.GetSourceMailContent(wddCode);
			if (sourceMailContent.IsNotNull())
			{
				DataSet data = unitOfWork.ContextAction.GetData("H$ApprovalDecision", Function.ToString(sourceMailContent.Rows[0]["WtmCode"]));
				if (data.IsNotNull() && data.Tables[0].IsNotNull())
				{
					ActionHandle actionHandle = new ActionHandle();
					if (actionHandle.GetMailContent(data, sourceMailContent) != null)
					{
						string text4 = Function.ToString(sourceMailContent.Rows[0]["DbName"]);
						string text5 = (string.IsNullOrWhiteSpace(Global.AppDataFolder) ? System.Web.HttpContext.Current.Server.MapPath("~/App_Data/") : Global.AppDataFolder) + "\\" + text4 + "\\Action";
						string text6 = Guid.NewGuid().ToString();
						string commandText = "";
						string fileName = "";
						ReportDocument crystalReport = actionHandle.GetCrystalReport(data, sourceMailContent, text5, out commandText, out fileName);
						if (!string.IsNullOrEmpty(commandText))
						{
							DataTable dataSource = unitOfWork.Commons.ExecuteDataTable(commandText, CommandType.Text);
							crystalReport.SetDataSource(dataSource);
							crystalReport.Refresh();
							string text7 = text5 + "\\" + text6 + ".html";
							crystalReport.ExportToDisk(ExportFormatType.HTML40, text7);
							value = System.IO.File.ReadAllText(text7);
							System.IO.File.Delete(text7);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		Session["Preview"] = value;
		return View();
	}

	[HttpPost]
	public ActionResult Index(string token, string key, string remarks, string submitType, string pwd)
	{
		try
		{
			ViewBag.Key = key;
			ViewBag.Token = token;
			ViewBag.Error = false;
			ViewBag.Password = pwd;
			string[] array = Function.Decrypt(RSA.PrivateKey, key).Split(';');
			string text = array[0] + ";" + array[1] + ";" + pwd + ";" + array[3] + ";" + submitType;
			Logging.Write("WATCH", "Approval Decision:" + text);
			ExternalController externalController = new ExternalController();
			HttpRequestMessage request = new HttpRequestMessage
			{
				Headers = { { "Token", token } }
			};
			externalController.Request = request;
			HttpResult dataRequest = new HttpResult
			{
				Content = Function.Encrypt(RSA.PublicKey, text),
				Message = remarks
			};
			externalController.Request = request;
			HttpResult httpResult = externalController.ApprovalRequest(dataRequest);
			Logging.Write("WATCH", httpResult.Message);
			if (!string.IsNullOrEmpty(Function.ToString(httpResult.Message)))
			{
				ViewBag.Message = Function.ToString(httpResult.Message);
				ViewBag.Error = true;
			}
			else if (!(submitType == "N"))
			{
				ViewBag.Approved = true;
			}
			else
			{
				ViewBag.Rejected = true;
			}
		}
		catch (Exception ex)
		{
			ViewBag.Key = key;
			ViewBag.Token = token;
			ViewBag.Message = ex.Message;
			ViewBag.Error = true;
			Logging.Write("ERROR", ex);
		}
		return View();
	}
}
