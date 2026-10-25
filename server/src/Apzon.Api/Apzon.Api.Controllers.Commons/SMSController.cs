using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.SMS;
using Newtonsoft.Json;

namespace Apzon.Api.Controllers.Commons;

public class SMSController : BaseApiController
{
	private string GetAcccessToken(string[] scope, out string sUri)
	{
		sUri = "";
		try
		{
			string grant_type = "client_credentials";
			DataTable config = UnitOfWork.SmsConfig.GetConfig();
			if (config.EmptyData())
			{
				Logging.Write("TRACE", "Lấy SMS Token thất bại: Chưa thiết lập cấu hình gửi SMS!");
				return "";
			}
			string text = Function.ToString(config.Rows[0]["EndpointType"]);
			string client_id = Function.ToString(config.Rows[0]["ClientID"]);
			string client_secret = Function.ToString(config.Rows[0]["ClientSecret"]);
			if (text == "L")
			{
				sUri = "https://service.sms.fpt.net";
			}
			else if (text == "S")
			{
				sUri = "https://sandbox.sms.fpt.net";
			}
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Function.ToString(sUri + "/oauth2/token"));
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";
			httpWebRequest.UseDefaultCredentials = true;
			httpWebRequest.PreAuthenticate = true;
			httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
			using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				string value = JsonConvert.SerializeObject(new AccessTokenModel
				{
					grant_type = grant_type,
					client_id = client_id,
					client_secret = client_secret,
					scope = string.Join(" ", scope),
					session_id = SessionId
				});
				streamWriter.Write(value);
				streamWriter.Flush();
				streamWriter.Close();
			}
			using StreamReader streamReader = new StreamReader(((HttpWebResponse)httpWebRequest.GetResponse()).GetResponseStream());
			RequestResponseModel requestResponseModel = JsonConvert.DeserializeObject<RequestResponseModel>(streamReader.ReadToEnd());
			try
			{
				string text2 = Function.ToString(requestResponseModel.access_token);
				if (!string.IsNullOrWhiteSpace(text2))
				{
					Logging.Write("TRACE", "Lấy SMS Token thành công. access_token: ", text2);
					return text2;
				}
			}
			catch (Exception ex)
			{
				Logging.Write("TRACE", "Lấy SMS Token thất bại: ", ex.Message);
				return "";
			}
			try
			{
				string text3 = "";
				string text4 = "";
				text4 = requestResponseModel.error_description;
				text3 = requestResponseModel.error;
				if (!string.IsNullOrWhiteSpace(text3))
				{
					Logging.Write("TRACE", "Lấy SMS Token thất bại: ", text3 + ":" + text4);
					return "";
				}
			}
			catch (Exception ex2)
			{
				Logging.Write("TRACE", "Gửi tin nhắn lỗi: ", ex2.Message);
			}
		}
		catch (Exception ex3)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex3.Message);
		}
		return "";
	}

	[HttpPost]
	public string CreateCampagin(Tuple<string, string, string, string, int> tpl)
	{
		try
		{
			string sUri = "";
			string acccessToken = GetAcccessToken(new string[1] { "send_brandname_otp" }, out sUri);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Function.ToString(sUri + "/api/create-campaign"));
			if (string.IsNullOrWhiteSpace(tpl.Item1) || string.IsNullOrWhiteSpace(tpl.Item2) || string.IsNullOrWhiteSpace(tpl.Item3) || string.IsNullOrWhiteSpace(tpl.Item4) || tpl.Item5 <= 0)
			{
				Logging.Write("TRACE", "Tạo Campaign thất bại: Thông tin truyền lên chưa đầy đủ!");
				return "";
			}
			if (string.IsNullOrWhiteSpace(acccessToken))
			{
				Logging.Write("TRACE", "Tạo Campaign thất bại: Không lấy được Access Token");
				return "";
			}
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";
			httpWebRequest.UseDefaultCredentials = true;
			httpWebRequest.PreAuthenticate = true;
			httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
			using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				string value = JsonConvert.SerializeObject(new SMSCampaignModel
				{
					access_token = acccessToken,
					session_id = SessionId,
					CampaignName = tpl.Item1,
					BrandName = tpl.Item2,
					Message = tpl.Item3,
					ScheduleTime = tpl.Item4,
					Quota = tpl.Item5
				});
				streamWriter.Write(value);
				streamWriter.Flush();
				streamWriter.Close();
			}
			string text = "";
			string text2 = "";
			using StreamReader streamReader = new StreamReader(((HttpWebResponse)httpWebRequest.GetResponse()).GetResponseStream());
			RequestResponseModel requestResponseModel = JsonConvert.DeserializeObject<RequestResponseModel>(streamReader.ReadToEnd());
			try
			{
				string campaignCode = requestResponseModel.CampaignCode;
				if (!string.IsNullOrWhiteSpace(campaignCode))
				{
					Logging.Write("TRACE", "Tạo Campaign thành công. Campaign Code: ", campaignCode);
					return campaignCode;
				}
			}
			catch (Exception ex)
			{
				Logging.Write("TRACE", "Tạo Campaign thất bại: ", ex.Message);
				return "";
			}
			try
			{
				text2 = requestResponseModel.error_description;
				text = requestResponseModel.error;
				if (!string.IsNullOrWhiteSpace(text))
				{
					Logging.Write("TRACE", "Tạo Campaign thất bại: ", text + ":" + text2);
					return "";
				}
			}
			catch (Exception ex2)
			{
				Logging.Write("TRACE", "Gửi tin nhắn lỗi: ", ex2.Message);
			}
		}
		catch (Exception ex3)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex3.Message);
		}
		return string.Empty;
	}

	[HttpPost]
	public string SendOTPMessage(Tuple<string, string, string> tpl)
	{
		try
		{
			string sUri = "";
			string acccessToken = GetAcccessToken(new string[1] { "send_brandname_otp" }, out sUri);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Function.ToString(sUri + "/api/push-brandname-otp"));
			if (string.IsNullOrWhiteSpace(acccessToken))
			{
				return "Không lấy được Access Token";
			}
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";
			httpWebRequest.UseDefaultCredentials = true;
			httpWebRequest.PreAuthenticate = true;
			httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
			using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				SMSModel sMSModel = new SMSModel
				{
					access_token = acccessToken,
					BrandName = tpl.Item1,
					Phone = tpl.Item2
				};
				byte[] bytes = Encoding.UTF8.GetBytes(tpl.Item3);
				sMSModel.Message = Convert.ToBase64String(bytes);
				sMSModel.session_id = SessionId;
				string value = JsonConvert.SerializeObject(sMSModel);
				streamWriter.Write(value);
				streamWriter.Flush();
				streamWriter.Close();
			}
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			int num = 0;
			string text = "";
			string text2 = "";
			using StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
			RequestResponseModel requestResponseModel = JsonConvert.DeserializeObject<RequestResponseModel>(streamReader.ReadToEnd());
			try
			{
				num = requestResponseModel.MessageId;
				if (num > 0)
				{
					Logging.Write("TRACE", "Gửi tin nhắn thành công. MessageID: ", Function.ToString(num));
					return "";
				}
			}
			catch (Exception ex)
			{
				Logging.Write("TRACE", "Gửi tin nhắn lỗi: ", ex.Message);
				return "SMS API không trả về MessageId!";
			}
			try
			{
				text2 = requestResponseModel.error_description;
				text = requestResponseModel.error;
				if (!string.IsNullOrWhiteSpace(text))
				{
					return text + ":" + text2;
				}
			}
			catch (Exception ex2)
			{
				Logging.Write("TRACE", "Gửi tin nhắn lỗi: ", ex2.Message);
			}
		}
		catch (Exception ex3)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex3.Message);
		}
		return "Gửi tin nhắn lỗi!";
	}

	[HttpPost]
	public string SendCampaignAd(Tuple<string, string[]> tpl)
	{
		try
		{
			string sUri = "";
			string acccessToken = GetAcccessToken(new string[1] { "send_brandname" }, out sUri);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Function.ToString(sUri + "/api/push-brandname-ads"));
			if (string.IsNullOrWhiteSpace(tpl.Item1))
			{
				return "Campaign Code không được để trống!";
			}
			if (tpl.Item2.Length == 0)
			{
				return "Không có số điện thoại nào";
			}
			if (string.IsNullOrWhiteSpace(acccessToken))
			{
				return "Không lấy được Access Token";
			}
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";
			httpWebRequest.UseDefaultCredentials = true;
			httpWebRequest.PreAuthenticate = true;
			httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
			using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				string value = JsonConvert.SerializeObject(new SendCampaignAdModel
				{
					access_token = acccessToken,
					CampaignCode = tpl.Item1,
					PhoneList = string.Join(",", tpl.Item2),
					session_id = SessionId
				});
				streamWriter.Write(value);
				streamWriter.Flush();
				streamWriter.Close();
			}
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			string text = "";
			string text2 = "";
			string text3 = "";
			using StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
			RequestResponseModel requestResponseModel = JsonConvert.DeserializeObject<RequestResponseModel>(streamReader.ReadToEnd());
			try
			{
				text = requestResponseModel.BatchId;
				if (!string.IsNullOrWhiteSpace(text))
				{
					Logging.Write("TRACE", "Gửi tin nhắn quảng cáo thành công. BatchId: ", text);
					return "";
				}
			}
			catch (Exception ex)
			{
				Logging.Write("TRACE", "Gửi tin nhắn quảng cáo lỗi: ", ex.Message);
				return "SMS API không trả về BatchId!";
			}
			try
			{
				text3 = requestResponseModel.error_description;
				text2 = requestResponseModel.error;
				if (!string.IsNullOrWhiteSpace(text2))
				{
					return text2 + ":" + text3;
				}
			}
			catch (Exception ex2)
			{
				Logging.Write("TRACE", "Gửi tin nhắn lỗi: ", ex2.Message);
			}
		}
		catch (Exception ex3)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex3.Message);
		}
		return "Gửi tin nhắn lỗi!";
	}

	[HttpPost]
	public string CancelCampaignAd([FromBody] string campaignCode)
	{
		try
		{
			string sUri = "";
			string acccessToken = GetAcccessToken(new string[1] { "send_brandname" }, out sUri);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Function.ToString(sUri + "/api/cancel-ads"));
			if (string.IsNullOrWhiteSpace(acccessToken))
			{
				return "Không lấy được Access Token";
			}
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";
			httpWebRequest.UseDefaultCredentials = true;
			httpWebRequest.PreAuthenticate = true;
			httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
			using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				string value = JsonConvert.SerializeObject(new SendCampaignAdModel
				{
					access_token = acccessToken,
					CampaignCode = campaignCode,
					session_id = SessionId
				});
				streamWriter.Write(value);
				streamWriter.Flush();
				streamWriter.Close();
			}
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			string text = "";
			string text2 = "";
			using StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
			RequestResponseModel requestResponseModel = JsonConvert.DeserializeObject<RequestResponseModel>(streamReader.ReadToEnd());
			try
			{
				string[] success = requestResponseModel.success;
				string[] fail = requestResponseModel.fail;
				if (success.Length != 0 || fail.Length != 0)
				{
					Logging.Write("TRACE", "Hủy tin nhắn quảng cáo thành công. Các BatchId hủy thành công: ", string.Join(",", success) + " .Các BatchId hủy thất bại:" + string.Join(",", fail));
					return "";
				}
			}
			catch (Exception ex)
			{
				Logging.Write("TRACE", "Hủy tin nhắn quảng cáo lỗi: ", ex.Message);
				return "SMS API không trả về kết quả đúng!";
			}
			try
			{
				text2 = requestResponseModel.error_description;
				text = requestResponseModel.error;
				if (!string.IsNullOrWhiteSpace(text))
				{
					return text + ":" + text2;
				}
			}
			catch (Exception ex2)
			{
				Logging.Write("TRACE", "Hủy tin nhắn quảng cáo lỗi: ", ex2.Message);
			}
		}
		catch (Exception ex3)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex3.Message);
		}
		return "Gửi tin nhắn lỗi!";
	}

	[HttpGet]
	public DataTable GetConfig()
	{
		return UnitOfWork.SmsConfig.GetConfig();
	}

	[HttpPost]
	public string CreateAndSendCampaign(Tuple<string, string, string, string, int, string[]> tpl)
	{
		string text = CreateCampagin(new Tuple<string, string, string, string, int>(tpl.Item1, tpl.Item2, tpl.Item3, tpl.Item4, tpl.Item5));
		if (string.IsNullOrEmpty(text))
		{
			Logging.Write("ERROR", "Không lấy được campaignCode!");
			return "Không lấy được campaignCode!";
		}
		return SendCampaignAd(new Tuple<string, string[]>(text, tpl.Item6));
	}

	[HttpPost]
	public string SaveConfig([FromBody] DataTable dt)
	{
		return UnitOfWork.SmsConfig.SaveConfig(dt);
	}
}
