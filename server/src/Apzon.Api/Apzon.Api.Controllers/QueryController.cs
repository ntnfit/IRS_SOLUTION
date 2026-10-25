using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Apzon.Api.Controllers.Administrator.Setup.General;
using Apzon.Api.Controllers.Sales;
using Apzon.Api.Models;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apzon.Api.Controllers;

public class QueryController : BaseApiController
{
	[HttpGet]
	[ActionName("0")]
	public HttpResponseMessage Table(string category, string name)
	{
		try
		{
			NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.RequestUri.Query);
			DataRow dataRow = UnitOfWork.ExternalRouting.GetAllRoutings().AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Controller"]) == category && Function.ToString(t["ActionName"]) == name && Function.ToString(t["Active"]) == "Y" && Function.ToString(t["ActionType"]) == "1");
			if (dataRow != null)
			{
				int absEntry = Function.ParseInt(dataRow["AbsEntry"]);
				short? num = Function.ParseShort(dataRow["ResultType"]);
				string text = Function.ToString(dataRow["ResultName"]);
				DataSet lines = UnitOfWork.ExternalRouting.GetLines(absEntry);
				if (lines.Tables.Count == 2)
				{
					DataTable dataTable = lines.Tables[0];
					DataTable treeConfig = lines.Tables[1];
					string text2 = "";
					string text3 = Function.ToString(dataRow["SourceData"]);
					if (text3 == "T")
					{
						text2 = Function.ToString(dataRow["DataString"]);
					}
					else
					{
						if (!(text3 == "Q"))
						{
							Logging.Write("ERROR", "Cấu hình source data không hợp lệ!");
							return new HttpResponseMessage(HttpStatusCode.InternalServerError);
						}
						DataTable queryById = UnitOfWork.Query.GetQueryById(Function.ParseInt(dataRow["DataString"]));
						if (queryById.EmptyData())
						{
							Logging.Write("ERROR", "Không tìm thấy hoặc Defined Query không hợp lệ!");
							return new HttpResponseMessage(HttpStatusCode.InternalServerError);
						}
						text2 = Function.ToString(queryById.Rows[0]["QString"]);
					}
					List<QueryParamater> list = new List<QueryParamater>();
					foreach (DataRow row in dataTable.Rows)
					{
						int num2 = 0;
						string text4 = "";
						string[] allKeys = nameValueCollection.AllKeys;
						foreach (string text5 in allKeys)
						{
							if (text5 == Function.ToString(row["Alias"]))
							{
								text4 = nameValueCollection[text5];
								num2++;
							}
						}
						if (num2 == 0)
						{
							Logging.Write("ERROR", "Request không gửi lên đúng param: " + Function.ToString(row["Alias"]));
							return new HttpResponseMessage(HttpStatusCode.NotFound);
						}
						string text6 = Function.ToString(row["DfltValue"]);
						QueryParamater item = new QueryParamater
						{
							DataType = Function.ToString(row["DataType"]),
							ParaName = Function.ToString(row["ParamName"]),
							Value = (string.IsNullOrWhiteSpace(text4) ? text6 : text4)
						};
						list.Add(item);
					}
					Tuple<string, List<QueryParamater>, string> tuple = new Tuple<string, List<QueryParamater>, string>(text2, list, "");
					DataSet dataSet = UnitOfWork.Commons.ExecuteSql(tuple.Item1, tuple.Item2, tuple.Item3);
					string text7 = "";
					string text8 = "";
					if (dataSet.IsNotNull() && dataSet.Tables.Count >= 2)
					{
						switch (num)
						{
						case 1:
							text7 += (string.IsNullOrWhiteSpace(text) ? ("{" + Function.ConvertTableToJson(treeConfig, dataSet.Tables[1], -1, isArrayObject: false) + "}") : ("{\"" + text + "\":{" + Function.ConvertTableToJson(treeConfig, dataSet.Tables[1], -1, isArrayObject: false) + "}}"));
							break;
						case 2:
						{
							DataSet dataSet2 = UnitOfWork.Commons.ExecuteSqlDataSet(tuple.Item1, tuple.Item2, tuple.Item3).Copy();
							if (!string.IsNullOrWhiteSpace(text))
							{
								dataSet2.DataSetName = text;
							}
							dataSet2.Tables.RemoveAt(0);
							text7 = JsonConvert.SerializeObject(dataSet2);
							break;
						}
						case 0:
							if (!string.IsNullOrWhiteSpace(text))
							{
								dataSet.Tables[1].TableName = text;
							}
							text7 = JsonConvert.SerializeObject(dataSet.Tables[1]);
							break;
						case 3:
							if (dataSet.Tables[1].IsNotNull())
							{
								text7 = Function.ToString(dataSet.Tables[1].Rows[0][0]);
							}
							break;
						}
						DataTable dataTable2 = dataSet.Tables["Message"];
						if (dataTable2.IsNotNull())
						{
							string text9 = Function.ToString(dataTable2.Rows[0]["MsgType"]);
							text8 = Function.ToString(dataTable2.Rows[0]["Message"]);
							if (!(text9 == "200") && text9 == "5xx")
							{
								Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text8, tuple.Item1);
							}
						}
						HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
						httpResponseMessage.Content = new StringContent(text7, Encoding.UTF8, "application/json");
						httpResponseMessage.ReasonPhrase += text8;
						return httpResponseMessage;
					}
					Logging.Write("ERROR", "Dataset null or Dataset's Tables count <2!");
					return new HttpResponseMessage(HttpStatusCode.InternalServerError);
				}
			}
			else
			{
				Logging.Write("TRACE", "Request không hợp lệ!");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			return Request.CreateResponse(HttpStatusCode.InternalServerError);
		}
		return Request.CreateResponse(HttpStatusCode.BadRequest);
	}

	[HttpPost]
	public HttpResponseMessage GetToken([FromBody] TokenParam model)
	{
		TokenResult tokenResult = new TokenResult();
		tokenResult.AccessToken = "";
		tokenResult.Error = "";
		HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
		try
		{
			string content;
			if (string.IsNullOrWhiteSpace(model.key) || string.IsNullOrWhiteSpace(model.user) || string.IsNullOrWhiteSpace(model.pass))
			{
				Logging.Write("ERROR", "GetToken FAILED: Params is missing!");
				tokenResult.Error = "Params is missing!";
				content = JsonConvert.SerializeObject(tokenResult);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.BadRequest);
				httpResponseMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");
				return httpResponseMessage;
			}
			string plainText;
			try
			{
				plainText = Function.Decrypt(RSA.PrivateKey, model.key);
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", "GetToken FAILED: Invalid key! " + ex.Message);
				tokenResult.Error = "Invalid key!";
				content = JsonConvert.SerializeObject(tokenResult);
				httpResponseMessage = Request.CreateResponse(HttpStatusCode.BadRequest);
				httpResponseMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");
				return httpResponseMessage;
			}
			KeyValuePair<string, SessionUserInfoImpl> keyValuePair = Global.SessionUserInfo.FirstOrDefault((KeyValuePair<string, SessionUserInfoImpl> t) => t.Value.UserName.Equals(model.user));
			string text = "";
			if (!string.IsNullOrEmpty(keyValuePair.Key))
			{
				text = keyValuePair.Key;
				tokenResult.AccessToken = text;
			}
			else
			{
				LoginImpl model2 = new LoginImpl
				{
					UserName = Function.Encrypt(RSA.PublicKey, model.user),
					Password = Function.Encrypt(RSA.PublicKey, model.pass),
					DbName = Function.Encrypt(RSA.PublicKey, plainText),
					ClientVersion = Function.Encrypt(RSA.PublicKey, Function.ConvertDoubleToStringEnUs(Versions.ApplicationVersion, 2, 1)),
					BuildVersion = Function.Encrypt(RSA.PublicKey, Function.ConvertDoubleToStringEnUs(Versions.BuildVersion, 0, 3))
				};
				LoginResult loginResult = new LoginIRSController
				{
					Request = new HttpRequestMessage
					{
						Headers = { { "SessionId", "" } }
					}
				}.Login(model2);
				string text2 = Function.Decrypt(RSA.PrivateKey, loginResult.Message);
				if (loginResult.Content != null)
				{
					string[] array = Function.ToString(Function.Decrypt(RSA.PrivateKey, Function.ToString(loginResult.Content))).Split(',');
					if (array.Length == 3)
					{
						text = (tokenResult.AccessToken = Function.ToString(array[0]));
						Logging.Write("TRACE", "GetToken SUCCESS: Token " + text);
					}
					else
					{
						httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
						Logging.Write("ERROR", "GetToken FAILED: Login Result problem" + text2);
					}
				}
				else
				{
					tokenResult.Error = text2;
					httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError);
					Logging.Write("ERROR", "GetToken FAILED" + text2);
				}
			}
			content = JsonConvert.SerializeObject(tokenResult);
			httpResponseMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");
			return httpResponseMessage;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", "GetToken FAILED" + ex2.Message);
			tokenResult.Error = ex2.Message;
			string content = JsonConvert.SerializeObject(tokenResult);
			httpResponseMessage = Request.CreateResponse(HttpStatusCode.BadRequest);
			httpResponseMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");
			return httpResponseMessage;
		}
		finally
		{
			Logging.Write("TRACE", "GetToken Param:user/" + model.user);
		}
	}

	[HttpPost]
	[ActionName("0")]
	public HttpResponseMessage Table1(string category, string name)
	{
		try
		{
			JObject jObject = (JObject)JsonConvert.DeserializeObject(Request.Content.ReadAsStringAsync().Result);
			DataRow dataRow = UnitOfWork.ExternalRouting.GetAllRoutings().AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Controller"]) == category && Function.ToString(t["ActionName"]) == name && Function.ToString(t["Active"]) == "Y" && Function.ToString(t["ActionType"]) == "2");
			if (dataRow != null)
			{
				int absEntry = Function.ParseInt(dataRow["AbsEntry"]);
				short? num = Function.ParseShort(dataRow["ResultType"]);
				string text = Function.ToString(dataRow["ResultName"]);
				DataSet lines = UnitOfWork.ExternalRouting.GetLines(absEntry);
				if (lines.Tables.Count == 2)
				{
					DataTable dataTable = lines.Tables[0];
					DataTable treeConfig = lines.Tables[1];
					string text2 = "";
					string text3 = Function.ToString(dataRow["SourceData"]);
					if (text3 == "T")
					{
						text2 = Function.ToString(dataRow["DataString"]);
					}
					else
					{
						if (!(text3 == "Q"))
						{
							Logging.Write("ERROR", "Cấu hình source data không hợp lệ!");
							return new HttpResponseMessage(HttpStatusCode.InternalServerError);
						}
						DataTable queryById = UnitOfWork.Query.GetQueryById(Function.ParseInt(dataRow["DataString"]));
						if (queryById.EmptyData())
						{
							Logging.Write("ERROR", "Không tìm thấy hoặc Defined Query không hợp lệ!");
							return new HttpResponseMessage(HttpStatusCode.InternalServerError);
						}
						text2 = Function.ToString(queryById.Rows[0]["QString"]);
					}
					List<QueryParamater> list = new List<QueryParamater>();
					foreach (DataRow row in dataTable.Rows)
					{
						int num2 = 0;
						string text4 = "";
						foreach (KeyValuePair<string, JToken> item in jObject)
						{
							if (item.Key == Function.ToString(row["Alias"]))
							{
								text4 = item.Value.Value<string>();
								num2++;
							}
						}
						if (num2 == 0)
						{
							Logging.Write("ERROR", "Request không gửi lên đúng param: " + Function.ToString(row["Alias"]));
							return new HttpResponseMessage(HttpStatusCode.NotFound);
						}
						string value = Function.ToString(row["DfltValue"]);
						QueryParamater queryParamater = new QueryParamater
						{
							DataType = Function.ToString(row["DataType"]),
							ParaName = Function.ToString(row["ParamName"])
						};
						Enums.DisplayFormatType displayFormatType = (Enums.DisplayFormatType)Function.ParseInt(row["DataType"]);
						if (string.IsNullOrWhiteSpace(text4))
						{
							queryParamater.Value = value;
						}
						else
						{
							switch (displayFormatType)
							{
							case Enums.DisplayFormatType.Encrypted:
							{
								string text6 = "";
								try
								{
									text6 = Function.Decrypt(RSA.PrivateKey, text4);
								}
								catch (Exception ex2)
								{
									Logging.Write("TRACE", "Can not decrypt string " + text4 + " : " + ex2.Message);
									text6 = "";
								}
								queryParamater.Value = text6;
								break;
							}
							case Enums.DisplayFormatType.StringBase64:
							{
								string text5 = "";
								try
								{
									text5 = Convert.ToBase64String(Encoding.UTF8.GetBytes(text4));
								}
								catch (Exception ex)
								{
									Logging.Write("TRACE", "Can not decrypt string " + text4 + " : " + ex.Message);
									text5 = "";
								}
								queryParamater.Value = text5;
								break;
							}
							default:
								queryParamater.Value = text4;
								break;
							}
						}
						list.Add(queryParamater);
					}
					Tuple<string, List<QueryParamater>, string> tuple = new Tuple<string, List<QueryParamater>, string>(text2, list, "");
					DataSet dataSet = UnitOfWork.Commons.ExecuteSql(tuple.Item1, tuple.Item2, tuple.Item3);
					string text7 = "";
					string text8 = "";
					if (dataSet.IsNotNull() && dataSet.Tables.Count >= 2)
					{
						switch (num)
						{
						case 1:
							text7 += (string.IsNullOrWhiteSpace(text) ? ("{" + Function.ConvertTableToJson(treeConfig, dataSet.Tables[1], -1, isArrayObject: false) + "}") : ("{\"" + text + "\":{" + Function.ConvertTableToJson(treeConfig, dataSet.Tables[1], -1, isArrayObject: false) + "}}"));
							break;
						case 2:
						{
							DataSet dataSet2 = UnitOfWork.Commons.ExecuteSqlDataSet(tuple.Item1, tuple.Item2, tuple.Item3).Copy();
							if (!string.IsNullOrWhiteSpace(text))
							{
								dataSet2.DataSetName = text;
							}
							dataSet2.Tables.RemoveAt(0);
							text7 = JsonConvert.SerializeObject(dataSet2);
							break;
						}
						case 0:
							if (!string.IsNullOrWhiteSpace(text))
							{
								dataSet.Tables[1].TableName = text;
							}
							text7 = JsonConvert.SerializeObject(dataSet.Tables[1]);
							break;
						case 3:
							if (dataSet.Tables[1].IsNotNull())
							{
								text7 = Function.ToString(dataSet.Tables[1].Rows[0][0]);
							}
							break;
						}
						DataTable dataTable2 = dataSet.Tables["Message"];
						if (dataTable2.IsNotNull())
						{
							string text9 = Function.ToString(dataTable2.Rows[0]["MsgType"]);
							text8 = Function.ToString(dataTable2.Rows[0]["Message"]);
							if (!(text9 == "200") && text9 == "5xx")
							{
								Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text8, tuple.Item1);
							}
						}
						HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
						httpResponseMessage.Content = new StringContent(text7, Encoding.UTF8, "application/json");
						httpResponseMessage.ReasonPhrase += text8;
						return httpResponseMessage;
					}
					Logging.Write("ERROR", "Dataset null or Dataset's Tables count <2!");
					return new HttpResponseMessage(HttpStatusCode.InternalServerError);
				}
			}
			else
			{
				Logging.Write("TRACE", "Request không hợp lệ!");
			}
		}
		catch (Exception ex3)
		{
			Logging.Write("ERROR", ex3.Message);
			return Request.CreateResponse(HttpStatusCode.InternalServerError);
		}
		return Request.CreateResponse(HttpStatusCode.BadRequest);
	}
}
