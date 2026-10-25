using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.DMS;
using Apzon.Entities.Models.MobileApp;
using Apzon.Global;
using Newtonsoft.Json;

namespace Apzon.HttpClientHelper;

internal class FileNetworkManagerClient : IFileNetworkManagerClient
{
	private const string Controller = "FileNetworkManager";

	public async Task<HttpResult> SaveAttachment(DataTable dt)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "FileNetworkManager");
			return await helper.PostAsync("SaveAttachment", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<UploadOneDriveResultImpl> UploadFileOneDriveResult(string filePath, string fileName = "")
	{
		_ = 3;
		try
		{
			fileName = fileName.ToLower();
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			using HttpClient client = new HttpClient();
			using MultipartFormDataContent formData = new MultipartFormDataContent();
			string appName = "i-DMS";
			DataTable dataTable = await http.GetAsync("GetDataConfigDMS", new { appName });
			string uriUploadOneDriver = "";
			if (dataTable.Rows.Count > 0)
			{
				uriUploadOneDriver = Function.ToString(dataTable.Rows[0]["UploadServer"]);
			}
			HttpContent httpContent;
			if (filePath.ToLower().EndsWith(".jpeg") || filePath.ToLower().EndsWith(".jpg") || filePath.ToLower().EndsWith(".png") || filePath.ToLower().EndsWith(".gif") || filePath.ToLower().EndsWith(".bmp"))
			{
				Image image = Image.FromFile(filePath);
				Image image2 = Function.ScaleImage(image, 1280, 720);
				MemoryStream memoryStream = new MemoryStream();
				image2.Save(memoryStream, image.RawFormat);
				memoryStream.Position = 0L;
				httpContent = new StreamContent(memoryStream);
			}
			else
			{
				httpContent = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read));
			}
			httpContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
			{
				Name = Path.GetFileName(filePath),
				FileName = (string.IsNullOrEmpty(fileName) ? Path.GetFileName(filePath) : fileName)
			};
			HttpContent content = new StringContent(GlobalData.Domain + "-" + GlobalData.DatabaseName);
			HttpContent content2 = new StringContent(GlobalData.Domain);
			HttpContent content3 = new StringContent(GlobalData.DatabaseName);
			formData.Add(httpContent);
			formData.Add(content, "pathName");
			formData.Add(content2, "appDomain");
			formData.Add(content3, "appDB");
			long currentTimeUpload = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
			string uploadKeyApp = "d0e0-4b29";
			string uploadSecrecyApp = "";
			using (HttpClientHelper<DataSet> httpGetDMSConfig = new HttpClientHelper<DataSet>(Configuration.ApiServer, "AdditionalGeneralSettings"))
			{
				DataSet dataSet = await httpGetDMSConfig.GetAsync("GetData");
				if (dataSet.Tables.Count > 2)
				{
					DataTable dataTable2 = dataSet.Tables[2].Copy();
					uploadSecrecyApp = Function.ToString(dataTable2.Rows[0]["UploadSecrecyApp"]);
				}
			}
			string arg = Function.GetStringSha256Hash($"{uploadKeyApp}{currentTimeUpload}{uploadSecrecyApp}{fileName}").ToLower();
			string value = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{uploadKeyApp}.{currentTimeUpload}.{arg}"));
			client.DefaultRequestHeaders.Add("TOKEN", value);
			HttpResponseMessage httpResponseMessage = await client.PostAsync(uriUploadOneDriver, formData);
			if (!httpResponseMessage.IsSuccessStatusCode)
			{
				return new UploadOneDriveResultImpl
				{
					Success = false,
					File = null,
					Message = "IsSuccessStatusCode false"
				};
			}
			return (UploadOneDriveResultImpl)JsonConvert.DeserializeObject(await httpResponseMessage.Content.ReadAsStringAsync(), typeof(UploadOneDriveResultImpl), new JsonSerializerSettings());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new UploadOneDriveResultImpl
			{
				Success = false,
				File = null,
				Message = ex.Message
			};
		}
	}

	public async Task<Tuple<string, UploadOneDriveFileResultImpl>> UploadFileOneDrive(UploadOneDriveInputImpl input)
	{
		_ = 2;
		try
		{
			UploadOneDriveResultImpl resultUpload = await UploadFileOneDriveResult(input.FilePath, input.FileName);
			if (resultUpload.Success)
			{
				using (new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons"))
				{
					using HttpClientHelper<ResponseResult> httpSaveUploadInfo = new HttpClientHelper<ResponseResult>(Configuration.ApiServer, "UploadImage");
					using HttpClientHelper<DataTable> httpGetOldData = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UploadImage");
					DataTable dataTable = await httpGetOldData.GetAsync("GetFileByOjectType", new
					{
						objectType = Function.ToString((int)input.ObjectTypes),
						objectCode = input.ObjectCode
					});
					string value = ((!dataTable.IsNotNull()) ? Function.ToString(Guid.NewGuid()) : Function.ToString(dataTable.Rows[0]["Code"]));
					if (dataTable.Columns.Count > 0)
					{
						DataTable dataTable2 = dataTable.Clone();
						DataRow dataRow = dataTable2.NewRow();
						UploadOneDriveFileResultImpl file = resultUpload.File;
						dataRow["Code"] = value;
						dataRow["UserSign"] = GlobalData.UserID;
						dataRow["FileId"] = file.FileId;
						dataRow["FileName"] = file.FileName;
						dataRow["FileNameDrive"] = file.FileNameDrive;
						dataRow["LinkDownload"] = file.LinkDownload;
						dataRow["LinkThumbnails"] = file.LinkThumbnails;
						dataRow["MimeType"] = file.MimeType;
						dataRow["Path"] = file.Path;
						dataRow["Size"] = file.Size;
						dataRow["WebUrl"] = file.WebUrl;
						dataRow["UploadDate"] = DateTime.UtcNow;
						dataRow["AlbumId"] = -1;
						dataRow["ObjectType"] = Function.ToString((int)input.ObjectTypes);
						dataRow["ObjectCode"] = input.ObjectCode;
						dataRow["Extension"] = file.Extension;
						dataRow["PathThumbnail"] = file.PathThumbnail;
						dataRow["Domain"] = file.Domain;
						dataRow["DateCreated"] = file.DateCreated;
						dataRow["TimestampCreated"] = file.TimestampCreated;
						dataTable2.Rows.Add(dataRow);
						dataTable2.AcceptChanges();
						return Tuple.Create((await httpSaveUploadInfo.PostAsync("UploadImage", dataTable2)).Message, file);
					}
					return Tuple.Create("Table structure APZ_D_UUPIM not found", new UploadOneDriveFileResultImpl());
				}
			}
			return Tuple.Create(resultUpload.Message, resultUpload.File);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return Tuple.Create(ex.Message, new UploadOneDriveFileResultImpl());
		}
	}

	public async Task<string> UpLoadFile(string destFolder, bool isOverride, params string[] sourceFile)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "FileNetworkManager");
		_ = 1;
		try
		{
			if (sourceFile.Length == 0)
			{
				return "SourceFile is empty";
			}
			http.UpdateSessionId();
			bool isParseData = false;
			string uri = http.GetUri("UploadFile", typeof(string), ref isParseData);
			MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
			multipartFormDataContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("Dest-Folder")
			{
				Parameters = 
				{
					new NameValueHeaderValue("isOverride", isOverride ? "1" : "0")
				}
			};
			for (int i = 0; i < sourceFile.Length; i++)
			{
				string path = sourceFile[i];
				StreamContent streamContent = new StreamContent(new FileStream(path, FileMode.Open, FileAccess.Read));
				streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
				{
					Name = $"UploadFile_{i}",
					FileName = HttpUtility.UrlEncode(string.Format("{2}\\{0}\\{1}", destFolder, Path.GetFileName(path), GlobalData.DatabaseName))
				};
				multipartFormDataContent.Add(streamContent);
			}
			using HttpResponseMessage response = await http.HttpClient.PostAsync(uri, multipartFormDataContent);
			if (response.IsSuccessStatusCode)
			{
				return JsonConvert.DeserializeObject<string>(await http.GetResponseString(response.Content));
			}
			throw new Exception("Could not upload file");
		}
		catch (Exception)
		{
			throw;
		}
	}

	public async Task<string> DownLoadFile(string sourceFile, string destFolder, bool isOverride)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "FileNetworkManager");
		_ = 1;
		try
		{
			if (string.IsNullOrEmpty(sourceFile))
			{
				return "SourceFile is empty";
			}
			if (string.IsNullOrEmpty(destFolder))
			{
				return "DestFolder is empty";
			}
			if (!Directory.Exists(destFolder))
			{
				return "DestFolder does not exist";
			}
			http.UpdateSessionId();
			bool isParseData = false;
			string uri = http.GetUri("DownLoadFile", typeof(string), ref isParseData);
			new MultipartFormDataContent();
			using HttpResponseMessage response = await http.HttpClient.PostAsJsonAsync(uri, $"{GlobalData.DatabaseName}\\{sourceFile}");
			if (response.IsSuccessStatusCode)
			{
				HttpContent content = response.Content;
				Stream stream = await content.ReadAsStreamAsync();
				string path = string.Format("{1}\\{0}", content.Headers.ContentDisposition.FileName.Replace("\"", ""), destFolder);
				if (isOverride && File.Exists(path))
				{
					File.Delete(path);
				}
				using (FileStream destination = File.Create(path))
				{
					stream.CopyTo(destination);
				}
				return "";
			}
			return "Could not download file";
		}
		catch (Exception)
		{
			throw;
		}
	}

	public async Task<string> UpLoadFolder(string sourceFolder, string destFolder, bool isOverride)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "FileNetworkManager");
		_ = 1;
		try
		{
			if (string.IsNullOrEmpty(sourceFolder))
			{
				return "SourceFile is empty";
			}
			if (!Directory.Exists(sourceFolder))
			{
				return "SourceFolder does not exist";
			}
			http.UpdateSessionId();
			bool isParseData = false;
			string uri = http.GetUri("UploadFile", typeof(string), ref isParseData);
			MultipartFormDataContent form = new MultipartFormDataContent();
			form.Headers.ContentDisposition = new ContentDispositionHeaderValue("Dest-Folder")
			{
				Parameters = 
				{
					new NameValueHeaderValue("isOverride", isOverride ? "1" : "0")
				}
			};
			LoadFileInFolderToUpLoadFile(sourceFolder, $"{GlobalData.DatabaseName}\\{destFolder}", ref form);
			using HttpResponseMessage response = await http.HttpClient.PostAsync(uri, form);
			if (response.IsSuccessStatusCode)
			{
				return JsonConvert.DeserializeObject<string>(await http.GetResponseString(response.Content));
			}
			throw new Exception("Could not upload file");
		}
		catch (Exception)
		{
			throw;
		}
	}

	public async Task<string> DownLoadFolder(string sourceFolder, string destFolder, bool isOverride)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "FileNetworkManager");
		string path = "";
		try
		{
			if (string.IsNullOrEmpty(sourceFolder))
			{
				return "SourceFolder is empty";
			}
			if (string.IsNullOrEmpty(destFolder))
			{
				return "DestFolder is empty";
			}
			http.UpdateSessionId();
			bool isParseData = false;
			string uri = http.GetUri("DownLoadFolder", typeof(string), ref isParseData);
			using HttpResponseMessage response = await http.HttpClient.PostAsJsonAsync(uri, $"{GlobalData.DatabaseName}\\{sourceFolder}");
			if (response.IsSuccessStatusCode)
			{
				HttpContent content = response.Content;
				Stream stream = await content.ReadAsStreamAsync();
				path = string.Format("{1}\\{0}", content.Headers.ContentDisposition.FileName.Replace("\"", ""), destFolder);
				string text = $"{destFolder}\\{Path.GetFileName(sourceFolder)}";
				if (!Directory.Exists(Path.GetDirectoryName(text)))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(text));
				}
				if (isOverride && Directory.Exists(text))
				{
					Function.DeleteFolder(text);
				}
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				if (!Directory.Exists(Path.GetDirectoryName(path)))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(path));
				}
				using (FileStream destination = File.Create(path))
				{
					stream.CopyTo(destination);
				}
				Function.UnZipFile(path, text);
				File.Delete(path);
				return "";
			}
			return "Could not download file";
		}
		catch (Exception)
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			throw;
		}
	}

	public async Task DeleteFile(string fileDelete)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "FileNetworkManager");
		await http.PostAsync("DeleteFile", $"{GlobalData.DatabaseName}\\{fileDelete}");
	}

	public async Task DeleteFolder(string folderDelete)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "FileNetworkManager");
		await http.PutAsync("DeleteFolder", $"{GlobalData.DatabaseName}\\{folderDelete}");
	}

	public async Task<bool> CheckExistsFile(string file)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "FileNetworkManager");
		return (await http.GetAsync("CheckExistFile", new
		{
			file = $"{GlobalData.DatabaseName}\\{file}"
		})).Equals("1");
	}

	public async Task<bool> CheckExistsFolder(string folder, bool isFullPath = false)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "FileNetworkManager");
		return (await http.GetAsync("CheckExistFolder", new { folder, isFullPath })).Equals("1");
	}

	private static void LoadFileInFolderToUpLoadFile(string sourceFolder, string folder, ref MultipartFormDataContent form)
	{
		string[] files = Directory.GetFiles(sourceFolder);
		for (int i = 0; i < files.Length; i++)
		{
			string path = files[i];
			StreamContent streamContent = new StreamContent(new FileStream(path, FileMode.Open, FileAccess.Read));
			streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
			{
				Name = $"UpLoadFolder_{sourceFolder}_{i}",
				FileName = (string.IsNullOrEmpty(folder) ? $"{Path.GetFileName(sourceFolder)}\\{Path.GetFileName(path)}" : $"{folder}\\{Path.GetFileName(sourceFolder)}\\{Path.GetFileName(path)}")
			};
			form.Add(streamContent);
		}
		string[] directories = Directory.GetDirectories(sourceFolder);
		for (int j = 0; j < directories.Length; j++)
		{
			LoadFileInFolderToUpLoadFile(directories[j], string.IsNullOrEmpty(folder) ? Path.GetFileName(sourceFolder) : $"{folder}\\{Path.GetFileName(sourceFolder)}", ref form);
		}
	}

	public List<Common.ListItems> LoadReportModified(string folder = "Report")
	{
		using HttpClientHelper<Common.ListItems> httpClientHelper = new HttpClientHelper<Common.ListItems>(Configuration.ApiServer, "FileNetworkManager");
		return httpClientHelper.GetList("LoadReportModified", new { folder });
	}
}
