using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Apzon.Commons;
using Apzon.Controls;
using Apzon.Entities.Models.SMS;
using Apzon.Global;
using Apzon.HttpClientHelper;
using Newtonsoft.Json;

namespace Apzon.Forms.Commons;

public class SendSMSClient
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private string GetAcccessToken(string grandType, string clientId, string clientSecret, string[] scope)
	{
		try
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Function.ToString("https://service.sms.fpt.net/oauth2/token"));
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";
			httpWebRequest.UseDefaultCredentials = true;
			httpWebRequest.PreAuthenticate = true;
			httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
			using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				string value = JsonConvert.SerializeObject(new AccessTokenModel
				{
					grant_type = grandType,
					client_id = clientId,
					client_secret = clientSecret,
					scope = string.Join(" ", scope),
					session_id = GlobalData.SessionId
				});
				streamWriter.Write(value);
				streamWriter.Flush();
				streamWriter.Close();
			}
			using StreamReader streamReader = new StreamReader(((HttpWebResponse)httpWebRequest.GetResponse()).GetResponseStream());
			RequestResponseModel requestResponseModel = JsonConvert.DeserializeObject<RequestResponseModel>(streamReader.ReadToEnd());
			try
			{
				string text = Function.ToString(requestResponseModel.access_token);
				if (!string.IsNullOrWhiteSpace(text))
				{
					Logging.Write("TRACE", "Lấy SMS Token thành công. access_token: ", text);
					return text;
				}
			}
			catch (Exception ex)
			{
				Logging.Write("TRACE", "Lấy SMS Token thất bại: ", ex.Message);
				return "";
			}
			try
			{
				string text2 = "";
				string text3 = "";
				text3 = requestResponseModel.error_description;
				text2 = requestResponseModel.error;
				if (!string.IsNullOrWhiteSpace(text2))
				{
					Logging.Write("TRACE", "Lấy SMS Token thất bại: ", text2 + ":" + text3);
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
			WinCommons.ErrorLineNumber(ex3);
		}
		return "";
	}

	public string CreateCampagin(string campaignName, string brandName, string message, string scheduleTime, int quota)
	{
		try
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Function.ToString("https://service.sms.fpt.net/api/create-campaign"));
			string acccessToken = GetAcccessToken("client_credentials", "cF1D5079a224e124d50f668db62df413a595b02f", "8205335aa1b37c59dFe4c611db9940e9D54672419b161756f35429634cF52db09Bda3a27", new string[1] { "send_brandname_otp" });
			if (string.IsNullOrWhiteSpace(campaignName) || string.IsNullOrWhiteSpace(brandName) || string.IsNullOrWhiteSpace(message) || string.IsNullOrWhiteSpace(scheduleTime) || quota <= 0)
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
					session_id = GlobalData.SessionId,
					BrandName = brandName,
					CampaignName = campaignName,
					Message = message,
					ScheduleTime = scheduleTime,
					Quota = quota
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
			WinCommons.ErrorLineNumber(ex3);
		}
		return string.Empty;
	}

	public string SendSMS(string brandName, string phoneNumber, string message)
	{
		try
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Function.ToString("https://service.sms.fpt.net/api/push-brandname-otp"));
			string acccessToken = GetAcccessToken("client_credentials", "cF1D5079a224e124d50f668db62df413a595b02f", "8205335aa1b37c59dFe4c611db9940e9D54672419b161756f35429634cF52db09Bda3a27", new string[1] { "send_brandname_otp" });
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
					BrandName = brandName,
					Phone = phoneNumber
				};
				byte[] bytes = Encoding.UTF8.GetBytes(message);
				sMSModel.Message = Convert.ToBase64String(bytes);
				sMSModel.session_id = GlobalData.SessionId;
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
			WinCommons.ErrorLineNumber(ex3);
		}
		return "Gửi tin nhắn lỗi!";
	}

	public string SendCampaignAd(string campaignCode, string[] phoneList)
	{
		try
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Function.ToString("http://service.sms.fpt.net/api/push-brandname-ads"));
			string acccessToken = GetAcccessToken("client_credentials", "cF1D5079a224e124d50f668db62df413a595b02f", "8205335aa1b37c59dFe4c611db9940e9D54672419b161756f35429634cF52db09Bda3a27", new string[1] { "send_brandname" });
			if (string.IsNullOrWhiteSpace(campaignCode))
			{
				return "Campaign Code không được để trống!";
			}
			if (phoneList.Length == 0)
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
					CampaignCode = campaignCode,
					PhoneList = string.Join(",", phoneList),
					session_id = GlobalData.SessionId
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
			WinCommons.ErrorLineNumber(ex3);
		}
		return "Gửi tin nhắn lỗi!";
	}

	public string CancelCampaignAd(string campaignCode)
	{
		try
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Function.ToString("http://sandbox.sms.fpt.net/api/cancel-ads"));
			string acccessToken = GetAcccessToken("client_credentials", "cF1D5079a224e124d50f668db62df413a595b02f", "8205335aa1b37c59dFe4c611db9940e9D54672419b161756f35429634cF52db09Bda3a27", new string[1] { "send_brandname" });
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
					session_id = GlobalData.SessionId
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
			WinCommons.ErrorLineNumber(ex3);
		}
		return "Gửi tin nhắn lỗi!";
	}
}
