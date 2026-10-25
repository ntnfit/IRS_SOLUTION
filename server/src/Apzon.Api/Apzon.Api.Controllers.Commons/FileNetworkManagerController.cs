using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Commons.Helper;

namespace Apzon.Api.Controllers.Commons;

public class FileNetworkManagerController : BaseApiController
{
	public bool isFullPath;

	private readonly FtpHelper _ftpHelper = new FtpHelper();

	public FileNetworkManagerController()
	{
	}

	public FileNetworkManagerController(bool fullPath)
	{
		isFullPath = fullPath;
	}

	[HttpPost]
	public HttpResult SaveAttachment([FromBody] DataTable dt)
	{
		return UnitOfWork.Files.DMSSaveAttachment(dt);
	}

	[HttpPost]
	public string UploadFile()
	{
		try
		{
			HttpRequest request = HttpContext.Current.Request;
			bool flag = (Function.ToString(NameValueHeaderValue.Parse(request.Headers["Content-Disposition"].Split(';')[1].Trim()).Value).Equals("1") ? true : false);
			foreach (string file in request.Files)
			{
				HttpPostedFile httpPostedFile = request.Files[file];
				if (httpPostedFile != null)
				{
					string str = $"{(string.IsNullOrWhiteSpace(Global.AppDataFolder) ? HttpContext.Current.Server.MapPath($"~/App_Data") : Global.AppDataFolder)}\\{httpPostedFile.FileName}";
					str = HttpUtility.UrlDecode(str);
					string path = Function.ToString(Path.GetDirectoryName(str));
					if (!Directory.Exists(path))
					{
						Directory.CreateDirectory(path);
					}
					if (flag && File.Exists(str))
					{
						File.Delete(str);
					}
					Stream inputStream = httpPostedFile.InputStream;
					using FileStream destination = File.Create(str);
					inputStream.CopyTo(destination);
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpPost]
	public HttpResponseMessage DownLoadFile([FromBody] string sourceFile)
	{
		try
		{
			if (string.IsNullOrEmpty(sourceFile))
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}
			sourceFile = string.Format("{0}\\{1}", string.IsNullOrWhiteSpace(Global.AppDataFolder) ? HttpContext.Current.Server.MapPath("~/App_Data/") : Global.AppDataFolder, sourceFile);
			HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
			httpResponseMessage.Content = new StreamContent(new FileStream(sourceFile, FileMode.Open, FileAccess.Read));
			httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("DownLoadFile");
			httpResponseMessage.Content.Headers.ContentDisposition.FileName = Path.GetFileName(sourceFile);
			return httpResponseMessage;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return Request.CreateResponse(HttpStatusCode.BadRequest);
		}
	}

	[HttpPost]
	public HttpResponseMessage LoadImage([FromBody] string sourceFile)
	{
		try
		{
			if (string.IsNullOrEmpty(sourceFile))
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}
			HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
			httpResponseMessage.Content = new StreamContent(new FileStream(sourceFile, FileMode.Open, FileAccess.Read));
			httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("DownLoadFile");
			httpResponseMessage.Content.Headers.ContentDisposition.FileName = Path.GetFileName(sourceFile);
			return httpResponseMessage;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return Request.CreateResponse(HttpStatusCode.BadRequest);
		}
	}

	[HttpPost]
	public HttpResponseMessage DownLoadFolder([FromBody] string sourceFolder)
	{
		try
		{
			if (string.IsNullOrEmpty(sourceFolder))
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}
			if (!isFullPath)
			{
				sourceFolder = string.Format("{0}\\{1}", string.IsNullOrWhiteSpace(Global.AppDataFolder) ? HttpContext.Current.Server.MapPath("~/App_Data/") : Global.AppDataFolder, sourceFolder);
			}
			if (!Directory.Exists(sourceFolder))
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest);
			}
			string text = (string.IsNullOrWhiteSpace(Global.AppDataFolder) ? HttpContext.Current.Server.MapPath("~/App_Data/") : Global.AppDataFolder) + $"{Path.GetFileName(sourceFolder)}.zip";
			if (File.Exists(text))
			{
				if (Function.GetFolderLastModifiedDate(sourceFolder) > File.GetLastWriteTime(text))
				{
					File.Delete(text);
					Function.ZipFolder(sourceFolder, text);
				}
			}
			else
			{
				Function.ZipFolder(sourceFolder, text);
			}
			HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
			httpResponseMessage.Content = new StreamContent(new FileStream(text, FileMode.Open, FileAccess.Read));
			httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("DownLoadFile");
			httpResponseMessage.Content.Headers.ContentDisposition.FileName = Path.GetFileName(text);
			return httpResponseMessage;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return Request.CreateResponse(HttpStatusCode.BadRequest);
		}
	}

	[HttpPost]
	public void DeleteFile([FromBody] string fileDelete)
	{
		try
		{
			if (!File.Exists(fileDelete))
			{
				fileDelete = string.Format("{0}\\{1}", string.IsNullOrWhiteSpace(Global.AppDataFolder) ? HttpContext.Current.Server.MapPath("~/App_Data/") : Global.AppDataFolder, fileDelete);
			}
			if (File.Exists(fileDelete))
			{
				File.Delete(fileDelete);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	[HttpPost]
	public void DeleteFolder([FromBody] string folderDelete)
	{
		try
		{
			if (!Directory.Exists(folderDelete))
			{
				folderDelete = string.Format("{0}\\{1}", string.IsNullOrWhiteSpace(Global.AppDataFolder) ? HttpContext.Current.Server.MapPath("~/App_Data/") : Global.AppDataFolder, folderDelete);
			}
			if (Directory.Exists(folderDelete))
			{
				Function.DeleteFolder(folderDelete);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	[HttpGet]
	public string CheckExistFile(string file)
	{
		try
		{
			file = string.Format("{0}\\{1}", string.IsNullOrWhiteSpace(Global.AppDataFolder) ? HttpContext.Current.Server.MapPath("~/App_Data/") : Global.AppDataFolder, file);
			return File.Exists(file) ? "1" : "0";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "0";
		}
	}

	[HttpGet]
	public string CheckExistFolder(string folder, bool isFullPath)
	{
		try
		{
			if (!isFullPath)
			{
				folder = string.Format("{0}\\{1}", string.IsNullOrWhiteSpace(Global.AppDataFolder) ? HttpContext.Current.Server.MapPath("~/App_Data/") : Global.AppDataFolder, folder);
			}
			return Directory.Exists(folder) ? "1" : "0";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "0";
		}
	}

	[HttpGet]
	public List<Common.ListItems> LoadReportModified(string folder)
	{
		try
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				throw new Exception("Invalid Session");
			}
			string databaseName = SessionUserInfo.DatabaseName;
			string path = string.Format("{0}\\{1}", string.IsNullOrWhiteSpace(Global.AppDataFolder) ? HttpContext.Current.Server.MapPath("~/App_Data/") : Global.AppDataFolder, databaseName + "\\" + folder + "\\");
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			string path2 = Function.ToString(Path.GetDirectoryName(path));
			if (!Directory.Exists(path2))
			{
				Directory.CreateDirectory(path2);
				return new List<Common.ListItems>();
			}
			FileInfo[] files = directoryInfo.GetFiles();
			List<Common.ListItems> list = new List<Common.ListItems>();
			FileInfo[] array = files;
			foreach (FileInfo fileInfo in array)
			{
				Common.ListItems listItems = new Common.ListItems();
				listItems.Value = fileInfo.Name;
				listItems.Text = fileInfo.LastWriteTime.ToString("yyyyMMddHHmmss");
				list.Add(listItems);
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new List<Common.ListItems>();
		}
	}
}
