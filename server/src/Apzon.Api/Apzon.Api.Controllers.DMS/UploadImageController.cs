using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Api.Controllers.DMS;

[ValidateLoginSapExceptionHandling]
public class UploadImageController : BaseApiController
{
	public async Task<string> ConvertImageURLToBase64(string url)
	{
		try
		{
			StringBuilder _sb = new StringBuilder();
			byte[] array = await GetImageFromUrl(url);
			new StringBuilder().Append(Convert.ToBase64String(array, 0, array.Length));
			using Stream stream = new MemoryStream(array);
			Image img = Image.FromStream(stream);
			using MemoryStream stream2 = new MemoryStream();
			SaveJpeg(stream2, img, 35);
			byte[] streamBytes = GetStreamBytes(stream2);
			_sb.Append(Convert.ToBase64String(streamBytes, 0, streamBytes.Length));
			return _sb.ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public static byte[] GetStreamBytes(Stream stream)
	{
		try
		{
			stream.Position = 0L;
		}
		catch
		{
		}
		byte[] array = new byte[1024];
		List<byte> list = new List<byte>();
		int num = 0;
		while (true)
		{
			int num2 = stream.Read(array, 0, array.Length);
			if (num2 == 0)
			{
				break;
			}
			if (num2 == array.Length)
			{
				list.AddRange(array);
				num += num2;
				continue;
			}
			byte[] array2 = new byte[num2];
			Array.Copy(array, array2, num2);
			list.AddRange(array2);
			break;
		}
		return list.ToArray();
	}

	private async Task<byte[]> GetImageFromUrl(string url)
	{
		byte[] result;
		try
		{
			HttpWebResponse httpWebResponse = (HttpWebResponse)(await ((HttpWebRequest)WebRequest.Create(url)).GetResponseAsync());
			Stream responseStream = httpWebResponse.GetResponseStream();
			using (BinaryReader binaryReader = new BinaryReader(responseStream))
			{
				int count = (int)httpWebResponse.ContentLength;
				result = binaryReader.ReadBytes(count);
				binaryReader.Close();
			}
			responseStream.Close();
			httpWebResponse.Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			result = null;
		}
		return result;
	}

	public static void SaveJpeg(Stream stream, Image img, int quality)
	{
		try
		{
			if (quality < 0 || quality > 100)
			{
				throw new ArgumentOutOfRangeException("quality must be between 0 and 100.");
			}
			EncoderParameter encoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
			ImageCodecInfo encoderInfo = GetEncoderInfo("image/jpeg");
			EncoderParameters encoderParameters = new EncoderParameters(1);
			encoderParameters.Param[0] = encoderParameter;
			img.Save(stream, encoderInfo, encoderParameters);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private static ImageCodecInfo GetEncoderInfo(string mimeType)
	{
		ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
		for (int i = 0; i < imageEncoders.Length; i++)
		{
			if (imageEncoders[i].MimeType == mimeType)
			{
				return imageEncoders[i];
			}
		}
		return null;
	}

	[HttpPost]
	public HttpResponseMessage DMSDownloadFile([FromBody] string sourceFile)
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

	[HttpGet]
	public DataTable GetAvatarUserSyncData(string userName)
	{
		try
		{
			DataTable avatarUser = UnitOfWork.UploadImage.GetAvatarUser(userName);
			avatarUser.Columns.Add("Base64", typeof(string));
			string text = "";
			if (avatarUser.IsNotNull())
			{
				text = GetImage100Base64(Function.ToString(avatarUser.Rows[0]["LinkDownload"]));
				avatarUser.Rows[0]["Base64"] = "data:image/jpg;base64," + Function.ToString(text);
				avatarUser.AcceptChanges();
			}
			return avatarUser;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public async Task<DataRow> GetAvatarSyncData(DataTable data)
	{
		try
		{
			DataTable dt = data.Copy();
			dt.Columns.Add("Base64", typeof(string));
			if (dt.IsNotNull())
			{
				string text = await GetImage100Base64Async(Function.ToString(dt.Rows[0]["LinkThumbnails"]));
				dt.Rows[0]["Base64"] = "data:image/jpg;base64," + Function.ToString(text);
				dt.AcceptChanges();
			}
			return dt.Rows[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public async Task<DataTable> GetAvatarAllStoreSyncData(string userName)
	{
		try
		{
			DataTable allAvatarStore = UnitOfWork.UploadImage.GetAllAvatarStore(userName);
			new DataSet();
			List<Task<DataRow>> list = new List<Task<DataRow>>();
			for (int i = 0; i < allAvatarStore.Rows.Count; i++)
			{
				DataTable dataTable = allAvatarStore.Clone();
				dataTable.ImportRow(allAvatarStore.Rows[i]);
				dataTable.AcceptChanges();
				list.Add(GetAvatarSyncData(dataTable));
			}
			List<DataRow> list2 = (await Task.WhenAll(list)).ToList();
			DataTable dataTable2 = new DataTable();
			for (int j = 0; j < list2.Count; j++)
			{
				if (j == 0)
				{
					dataTable2 = list2[0].Table;
				}
				else
				{
					dataTable2.Rows.Add(list2[j].ItemArray);
				}
			}
			dataTable2.AcceptChanges();
			return dataTable2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public DataTable GetAvatarUser(string userName)
	{
		try
		{
			return UnitOfWork.UploadImage.GetAvatarUser(userName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public DataTable GetAvatar(string cardCode)
	{
		try
		{
			return UnitOfWork.UploadImage.GetAvatar(cardCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpPost]
	public DataSet GetImageByDate(DataTable data)
	{
		try
		{
			string cardCode = Function.ToString(data.Rows[0]["cardCode"]);
			string date = Function.ToString(data.Rows[0]["date"]);
			return UnitOfWork.UploadImage.GetImageByDate(cardCode, date);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpPost]
	public DataTable GetServerUpload(DataTable data)
	{
		try
		{
			string serverAddress = Function.ToString(data.Rows[0]["serverAddress"]);
			string domainName = Function.ToString(data.Rows[0]["domain"]);
			string dBName = Function.ToString(data.Rows[0]["dbName"]);
			return UnitOfWork.UploadImage.GetServerUpload(serverAddress, domainName, dBName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpPost]
	public ResponseResult UploadImage(DataTable dt)
	{
		try
		{
			ResponseResult responseResult = new ResponseResult();
			HttpResult httpResult = UnitOfWork.UploadImage.UploadImage(dt);
			if (httpResult.Message == "")
			{
				responseResult.MessageCode = "200";
				responseResult.Message = "";
			}
			else
			{
				responseResult.MessageCode = "400";
				responseResult.Message = httpResult.Message;
			}
			return responseResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				MessageCode = "401",
				Message = ex.Message
			};
		}
	}

	[HttpGet]
	public DataTable GetListAlbum()
	{
		try
		{
			new DataTable();
			return UnitOfWork.UploadImage.GetListAlbum();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public DataTable GetFileByOjectType(string objectType, string objectCode = "")
	{
		try
		{
			return UnitOfWork.UploadImage.GetFileByOjectType(objectType, objectCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	public ResponseResult UploadFiles(DataTable dt)
	{
		try
		{
			ResponseResult responseResult = new ResponseResult();
			HttpResult httpResult = UnitOfWork.UploadImage.UploadFiles(dt);
			if (httpResult.Message == "")
			{
				responseResult.MessageCode = "200";
				responseResult.Message = "";
			}
			else
			{
				responseResult.MessageCode = "400";
				responseResult.Message = httpResult.Message;
			}
			return responseResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpPost]
	public DataTable GetImageList(DataTable dt)
	{
		try
		{
			return UnitOfWork.UploadImage.GetImageList(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	public DataTable GetItemImageList(DataTable dt)
	{
		try
		{
			return UnitOfWork.UploadImage.GetItemImageList(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public ResponseResult GetStoreAlbumImage(string fromDate, string toDate, int userSign, string listCardCode, int pageIndex, string listAlbumId, string filterTypeAttach, string lstObjectType, int empId)
	{
		try
		{
			return UnitOfWork.UploadImage.GetStoreAlbumImage(fromDate, toDate, userSign, listCardCode, pageIndex, listAlbumId, filterTypeAttach, lstObjectType, empId);
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
