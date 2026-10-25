using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.DMS;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.MobileApp;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Api.Controllers.Inventory;

[ValidateLoginSapExceptionHandling]
public class BusinessMasterDataController : BaseApiController
{
	private string _messageCodePrefix = "213001";

	[HttpGet]
	public DataTable LoadBPBankAccount(string cardCode)
	{
		return UnitOfWork.BusinessMasterData.LoadBPBankAccount(cardCode);
	}

	public DataTable Get()
	{
		return UnitOfWork.Items.Table();
	}

	[HttpGet]
	public List<ItemsImpl> Cfl()
	{
		return UnitOfWork.BusinessMasterData.Cfl();
	}

	[HttpGet]
	public DataTable ShowCardGroup(string groupType)
	{
		return UnitOfWork.BusinessMasterData.ShowCardGroup(groupType);
	}

	[HttpGet]
	public bool CheckDoc(string cardCode)
	{
		return UnitOfWork.BusinessMasterData.CheckDoc(cardCode);
	}

	[HttpGet]
	public DataTable LoadUDFData(string cardCode)
	{
		return UnitOfWork.BusinessMasterData.LoadUDFData(cardCode);
	}

	[HttpGet]
	public DataTable LoadUDFDataSAP(string cardCode)
	{
		return UnitOfWork.BusinessMasterData.LoadUDFDataSAP(cardCode);
	}

	[HttpGet]
	public DataTable ShowCreditCardType()
	{
		return UnitOfWork.BusinessMasterData.ShowCreditCardType();
	}

	[HttpGet]
	public DataTable GetDataQuickBPMasterData(string cardCode)
	{
		try
		{
			if (Global.LoginSap && string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
				return new DataTable();
			}
			return UnitOfWork.BusinessMasterData.GetDataQuickBPMasterData(cardCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public DataTable GetBPByCardCode(string CardCode, int BPLId)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
					return new DataTable();
				}
				string userName = SessionUserInfo.UserName;
				string[] array = CardCode.Split(';');
				return UnitOfWork.BusinessMasterData.GetBPByCardCode(array[0], userName, BPLId);
			}
			string[] array2 = CardCode.Split(';');
			return UnitOfWork.BusinessMasterData.GetBPByCardCode(array2[0], array2[1], BPLId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public DataTable GetBPByCardCodeWithAddress(string CardCode, int BPLId)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
					return new DataTable();
				}
				string userName = SessionUserInfo.UserName;
				string[] array = CardCode.Split(';');
				return UnitOfWork.BusinessMasterData.GetBPByCardCodeWithAddress(array[0], userName, BPLId);
			}
			string[] array2 = CardCode.Split(';');
			return UnitOfWork.BusinessMasterData.GetBPByCardCodeWithAddress(array2[0], array2[1], BPLId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public DataTable CheckExistCardCode(string CardCode)
	{
		return UnitOfWork.BusinessMasterData.CheckExistCardCode(CardCode);
	}

	[HttpGet]
	public DataTable GetEmployeeCodeBySlpCode(int slpCode)
	{
		return UnitOfWork.BusinessMasterData.GetEmployeeCodeBySlpCode(slpCode);
	}

	[HttpGet]
	public DataTable GetVatByCardCode(string cardCode)
	{
		return UnitOfWork.BusinessMasterData.GetVatByCardCode(cardCode);
	}

	[HttpPost]
	public DataTable Search([FromBody] DataTable doc)
	{
		return UnitOfWork.BusinessMasterData.Search(doc, SessionUserInfo.UserName);
	}

	[HttpGet]
	public DataTable GetADoc(string CardCode)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
					return new DataTable();
				}
				string userName = SessionUserInfo.UserName;
				return UnitOfWork.BusinessMasterData.GetADoc(CardCode, userName);
			}
			return UnitOfWork.BusinessMasterData.GetADoc(CardCode, "");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public DataTable GetAddressByCardCode(string cardCode)
	{
		return UnitOfWork.BusinessMasterData.GetAddressByCardCode(cardCode);
	}

	[HttpGet]
	public DataTable GetListAddressByCardCode(string cardCode)
	{
		return UnitOfWork.BusinessMasterData.GetListAddressByCardCode(cardCode);
	}

	[HttpGet]
	public DataTable GetSearchTableAddress()
	{
		return UnitOfWork.BusinessMasterData.GetSearchTableAddress();
	}

	[HttpGet]
	public DataTable GetContactPersons(string cardType)
	{
		return UnitOfWork.BusinessMasterData.GetContactPersons(cardType);
	}

	[HttpGet]
	public DataSet GetBPProperties(string cardCode)
	{
		return UnitOfWork.BusinessMasterData.GetBPProperties(cardCode);
	}

	[HttpGet]
	public DataTable GetBPTableStructure(string table)
	{
		return UnitOfWork.BusinessMasterData.GetBPTableStructure(table);
	}

	[HttpGet]
	public DataTable ExistFileInDoc(string cardCode, string fileName, string fileExt)
	{
		return UnitOfWork.BusinessMasterData.ExistFileInDoc(cardCode, fileName, fileExt);
	}

	[HttpGet]
	public DataTable LoadComboCountry()
	{
		return UnitOfWork.BusinessMasterData.LoadComboCountry();
	}

	[HttpGet]
	public DataTable ShowBankAccount()
	{
		return UnitOfWork.BusinessMasterData.ShowBankAccount();
	}

	[HttpGet]
	public DataTable IsValidBankCode(string bankCode)
	{
		return UnitOfWork.BusinessMasterData.IsValidBankCode(bankCode);
	}

	[HttpGet]
	public DataTable LoadComboIndustry()
	{
		return UnitOfWork.BusinessMasterData.LoadComboIndustry();
	}

	[HttpGet]
	public DataTable LoadComboTerritory()
	{
		return UnitOfWork.BusinessMasterData.LoadComboTerritory();
	}

	[HttpGet]
	public DataTable LoadComboLanguage()
	{
		return UnitOfWork.BusinessMasterData.LoadComboLanguage();
	}

	[HttpGet]
	public DataTable LoadComboTechnician()
	{
		return UnitOfWork.BusinessMasterData.LoadComboTechnician();
	}

	[HttpGet]
	public DataTable LoadComboPaymentTermPriority()
	{
		return UnitOfWork.BusinessMasterData.LoadComboPaymentTermPriority();
	}

	[HttpGet]
	public DataTable LoadComboPaymentTermHolidays()
	{
		return UnitOfWork.BusinessMasterData.LoadComboPaymentTermHolidays();
	}

	[HttpGet]
	public DataTable GetFactoryIndicatorName(string code)
	{
		return UnitOfWork.BusinessMasterData.GetFactoryIndicatorName(code);
	}

	[HttpGet]
	public DataTable LoadPaymentDates(string cardCode)
	{
		return UnitOfWork.BusinessMasterData.LoadPaymentDates(cardCode);
	}

	[HttpGet]
	public DataTable GetBPGroup()
	{
		return UnitOfWork.BusinessMasterData.GetBPGroup();
	}

	[HttpPost]
	public string SaveDoc([FromBody] BPImpls doc)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return "Invalid session";
				}
				return BrokerService.GetUnitOfWork(SessionId).BusinessPartners.Save(doc, SessionUserInfo.SessionDiServer).Message;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "This function not support.";
	}

	public string SaveIgnoreEmpty([FromBody] BPImpls doc)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return "Invalid session";
				}
				return BrokerService.GetUnitOfWork(SessionId).BusinessPartners.Save(doc, SessionUserInfo.SessionDiServer, ignoreEmptyValue: true).Message;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "This function not support.";
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDocDataSet([FromBody] DataSet ds)
	{
		return UnitOfWork.BusinessMasterData.SaveDocDataSet(ds, "A");
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDocDataSetUpdate([FromBody] DataSet ds)
	{
		return UnitOfWork.BusinessMasterData.SaveDocDataSet(ds, "U");
	}

	[HttpGet]
	public DataTable GetRecord(string p, string c, string u, int b)
	{
		return UnitOfWork.BusinessMasterData.GetRecord(p, c, u, b);
	}

	[HttpPost]
	[MappingSap(IsMapping = true)]
	public string DeleteDoc([FromBody] string code)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return "Invalid session";
				}
				return BrokerService.GetUnitOfWork(SessionId).BusinessPartners.Remove(code, SessionUserInfo.SessionDiServer);
			}
			return UnitOfWork.BusinessMasterData.DeleteDoc(code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpPost]
	[MappingSap(IsMapping = true)]
	public string DeleteMulti([FromBody] List<string> lstCardCode)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return "Invalid session";
				}
				return BrokerService.GetUnitOfWork(SessionId).BusinessPartners.RemoveMulti(lstCardCode, SessionUserInfo.SessionDiServer);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		throw new Exception("Function not support! Delete one per time please");
	}

	[HttpGet]
	public bool CheckCustomer(string CodeCustormer)
	{
		return UnitOfWork.BusinessMasterData.CheckDoc(CodeCustormer);
	}

	[HttpGet]
	public DataTable ExistsStockTransaction(string cardCode)
	{
		return UnitOfWork.BusinessMasterData.ExistsStockTransaction(cardCode);
	}

	[HttpGet]
	public DataTable ExistsLinked(string cardCode)
	{
		return UnitOfWork.BusinessMasterData.ExistsLinked(cardCode);
	}

	[HttpGet]
	public DataTable ExistsLinkedAll(string cardCode)
	{
		return UnitOfWork.BusinessMasterData.ExistsLinkedAll(cardCode);
	}

	[HttpPost]
	public string ValidateChangedValue([FromBody] DataSet ds)
	{
		return UnitOfWork.BusinessMasterData.ValidateChangedValue(ds);
	}

	[HttpGet]
	public DataTable GetPaymentTerm(int groupNum)
	{
		return UnitOfWork.BusinessMasterData.GetPaymentTerm(groupNum);
	}

	[HttpGet]
	public DataTable ShowPaymentTerm()
	{
		return UnitOfWork.BusinessMasterData.ShowPaymentTerm();
	}

	[HttpGet]
	public string ExistLinkedContact(int cntctCode)
	{
		return UnitOfWork.BusinessMasterData.ExistLinkedContact(cntctCode);
	}

	[HttpGet]
	public DataTable GetControlAccounts(string cardCode)
	{
		return UnitOfWork.BusinessMasterData.GetControlAccounts(cardCode);
	}

	[HttpGet]
	public DataTable GetAccountTabName(string fatherCard, string debPayAcct, string dpmClear, string dpmIntAct, string connBP)
	{
		return UnitOfWork.BusinessMasterData.GetAccountTabName(fatherCard, debPayAcct, dpmClear, dpmIntAct, connBP);
	}

	[HttpGet]
	public DataTable GetDefaultLanguage()
	{
		return UnitOfWork.BusinessMasterData.GetDefaultLanguage();
	}

	[HttpGet]
	public DataTable LoadCBPaymentRunBank(string cryCode)
	{
		return UnitOfWork.BusinessMasterData.LoadCBPaymentRunBank(cryCode);
	}

	[HttpGet]
	public DataTable LoadCBPaymentRunAccount(string bankCode, string cryCode)
	{
		return UnitOfWork.BusinessMasterData.LoadCBPaymentRunAccount(bankCode, cryCode);
	}

	[HttpGet]
	public DataTable LoadCBPaymentRunBCACode()
	{
		return UnitOfWork.BusinessMasterData.LoadCBPaymentRunBCACode();
	}

	[HttpGet]
	public DataTable GetBpBranchAssignment(string cardCode)
	{
		return UnitOfWork.BusinessMasterData.GetBpBranchAssignment(cardCode);
	}

	[HttpGet]
	public ResponseResult GetBpList(string cardType, string cardName, string subType, string saleUser, string type = "")
	{
		try
		{
			DataTable bpList = UnitOfWork.BusinessMasterData.GetBpList(cardType, cardName, subType, saleUser, type);
			bpList.TableName = "BpList";
			_ = SessionUserInfo.DatabaseName;
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Table0 = bpList
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpPost]
	public ResponseResult GetListStore([FromBody] GetListStoreParamsImpl data)
	{
		if (data.checkApi == "Y")
		{
			return new ResponseResult
			{
				Message = "",
				MessageCode = "200",
				Content = "Check api success"
			};
		}
		return UnitOfWork.BusinessMasterData.GetListStore(data);
	}

	[HttpGet]
	public DataTable GetBpListDMS(string userName, string type = "M")
	{
		return UnitOfWork.BusinessMasterData.GetBpList("", "", "", userName, type);
	}

	[HttpGet]
	public ResponseResult GetDetailBPRevenue(string cardCode)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = "401"
				};
			}
			string userName = SessionUserInfo.UserName;
			DataTable detailBPRevenue = UnitOfWork.BusinessMasterData.GetDetailBPRevenue(cardCode, userName);
			return new ResponseResult
			{
				Message = "",
				Content = detailBPRevenue,
				MessageCode = "200"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "400"
			};
		}
	}

	[HttpGet]
	public ResponseResult GetDetailBP(string cardCode)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			string userName = SessionUserInfo.UserName;
			DataTable detailBP = UnitOfWork.BusinessMasterData.GetDetailBP(cardCode, userName);
			detailBP.TableName = "BPDetail";
			DataTable top10VisitHistory = UnitOfWork.VisitStore.GetTop10VisitHistory(cardCode);
			top10VisitHistory.TableName = "BPDetailHistoryVisit";
			DataTable dataTable = UnitOfWork.Manufacturers.ShowCurrency();
			dataTable.Columns.Add("CardCode", typeof(string));
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				dataTable.Rows[i]["CardCode"] = cardCode;
			}
			dataTable.AcceptChanges();
			dataTable.TableName = "BPDetailCurrency";
			DataTable dataTable2 = UnitOfWork.ContactPerson.ShowContactPerson(cardCode);
			dataTable2.TableName = "BPDetailContact";
			DataTable activityByCardCode = UnitOfWork.BusinessMasterData.GetActivityByCardCode(cardCode);
			activityByCardCode.TableName = "BPDetailActivity";
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(detailBP);
			dataSet.Tables.Add(dataTable);
			dataSet.Tables.Add(dataTable2);
			dataSet.Tables.Add(activityByCardCode);
			dataSet.Tables.Add(top10VisitHistory);
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Table0 = dataSet
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "107"
			};
		}
	}

	[HttpGet]
	public ResponseResult GetChildDataActivity(int clgCode)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			DataTable dataById = UnitOfWork.SubActivities.GetDataById(clgCode);
			dataById.TableName = "CurrentActivity";
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataById.Copy());
			if (!string.IsNullOrEmpty(Function.ToString(dataById.Rows[0]["ProjectEntry"])))
			{
				int num = Function.ParseInt(Function.ToString(dataById.Rows[0]["ProjectEntry"]));
				DataTable dataTable = UnitOfWork.ProjectManager.LoadData(num, null, null, null, null, null, null, null);
				dataTable.TableName = "ActivityProject";
				DataTable dataTable2 = UnitOfWork.CFL.GetDataProjectTask(num).Tables[1];
				dataTable2.TableName = "ActivityProjectWBS";
				dataSet.Tables.Add(dataTable.Copy());
				dataSet.Tables.Add(dataTable2.Copy());
			}
			DataTable dataTable3 = UnitOfWork.CFL.GetAllOpportunities().Tables[1];
			dataTable3.TableName = "ActivityOpporentunity";
			dataSet.Tables.Add(dataTable3.Copy());
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Table0 = dataSet
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "107"
			};
		}
	}

	[HttpGet]
	public ResponseResult GetBPGroup(string groupType)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			DataTable dataTable = UnitOfWork.BusinessMasterData.ShowCardGroup(groupType);
			dataTable.TableName = "BPGroup";
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Table0 = dataTable
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "107"
			};
		}
	}

	public static void SaveJpeg(string path, Image img, int quality)
	{
		if (quality < 0 || quality > 100)
		{
			throw new ArgumentOutOfRangeException("quality must be between 0 and 100.");
		}
		EncoderParameter encoderParameter = new EncoderParameter(Encoder.Quality, quality);
		ImageCodecInfo encoderInfo = GetEncoderInfo("image/jpeg");
		EncoderParameters encoderParameters = new EncoderParameters(1);
		encoderParameters.Param[0] = encoderParameter;
		img.Save(path, encoderInfo, encoderParameters);
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
	public string UploadFile()
	{
		try
		{
			HttpRequest request = HttpContext.Current.Request;
			foreach (string file in request.Files)
			{
				HttpPostedFile httpPostedFile = request.Files[file];
				if (httpPostedFile != null)
				{
					string path = $"{(string.IsNullOrWhiteSpace(Global.AppDataFolder) ? HttpContext.Current.Server.MapPath($"~/App_Data") : Global.AppDataFolder)}\\{httpPostedFile.FileName}";
					string path2 = Function.ToString(Path.GetDirectoryName(path));
					if (!Directory.Exists(path2))
					{
						Directory.CreateDirectory(path2);
					}
					if (File.Exists(path))
					{
						File.Delete(path);
					}
					Image img = Image.FromStream(httpPostedFile.InputStream);
					SaveJpeg(path, img, 35);
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

	private List<string> GetAllLastSubFolder(string rootFolder)
	{
		try
		{
			List<string> list = new List<string>();
			string[] directories = Directory.GetDirectories(rootFolder);
			foreach (string text in directories)
			{
				string[] directories2 = Directory.GetDirectories(text);
				if (directories2.Length == 0)
				{
					list.Add(text);
				}
				else
				{
					string[] array = directories2;
					foreach (string text2 in array)
					{
						string[] directories3 = Directory.GetDirectories(text2);
						if (directories3.Length == 0)
						{
							list.Add(text2);
							continue;
						}
						string[] array2 = directories3;
						foreach (string item in array2)
						{
							list.Add(item);
						}
					}
				}
				Console.WriteLine(text);
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public void DeleteFolder(string folderDelete)
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

	[HttpPost]
	public ResponseResult AllImagesVisitStore([FromBody] string sourceFolder)
	{
		try
		{
			if (string.IsNullOrEmpty(sourceFolder))
			{
				return new ResponseResult
				{
					Message = "bad request",
					MessageCode = _messageCodePrefix + "109"
				};
			}
			sourceFolder = string.Format("{0}\\{1}", string.IsNullOrWhiteSpace(Global.AppDataFolder) ? HttpContext.Current.Server.MapPath("~/App_Data/") : Global.AppDataFolder, sourceFolder);
			List<string> allLastSubFolder = GetAllLastSubFolder(sourceFolder);
			DataTable dataTable = new DataTable
			{
				Columns = 
				{
					{
						"FileName",
						typeof(string)
					},
					{
						"FilePath",
						typeof(string)
					}
				}
			};
			new List<string>();
			foreach (string item in allLastSubFolder)
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(item);
				FileInfo[] files = directoryInfo.GetFiles("*.jpeg");
				FileInfo[] files2 = directoryInfo.GetFiles();
				if (files.Length == 0 && files2.Length == 0)
				{
					DeleteFolder(item);
				}
				for (int i = 0; i < files.Length; i++)
				{
					string value = files[i].FullName.Split(new string[1] { "App_Data" }, StringSplitOptions.None)[1];
					string name = files[i].Name;
					DataRow dataRow = dataTable.NewRow();
					dataRow["FileName"] = name;
					dataRow["FilePath"] = value;
					dataTable.Rows.Add(dataRow.ItemArray);
					dataTable.AcceptChanges();
				}
			}
			return new ResponseResult
			{
				MessageCode = "0",
				Content = new
				{
					Table0 = dataTable
				},
				Message = ""
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "109"
			};
		}
	}

	[HttpPost]
	public ResponseResult AllImageVisitStoreByDateTime([FromBody] string sourceFolder)
	{
		try
		{
			if (string.IsNullOrEmpty(sourceFolder))
			{
				return new ResponseResult
				{
					Message = "bad request",
					MessageCode = _messageCodePrefix + "109"
				};
			}
			sourceFolder = string.Format("{0}\\{1}", string.IsNullOrWhiteSpace(Global.AppDataFolder) ? HttpContext.Current.Server.MapPath("~/App_Data/") : Global.AppDataFolder, sourceFolder);
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("FileName", typeof(string));
			dataTable.Columns.Add("FilePath", typeof(string));
			new List<string>();
			FileInfo[] files = new DirectoryInfo(sourceFolder).GetFiles("*.jpeg");
			for (int i = 0; i < files.Length; i++)
			{
				string value = files[i].FullName.Split(new string[1] { "App_Data" }, StringSplitOptions.None)[1];
				string name = files[i].Name;
				DataRow dataRow = dataTable.NewRow();
				dataRow["FileName"] = name;
				dataRow["FilePath"] = value;
				dataTable.Rows.Add(dataRow.ItemArray);
				dataTable.AcceptChanges();
			}
			return new ResponseResult
			{
				MessageCode = "0",
				Content = new
				{
					Table0 = dataTable
				},
				Message = ""
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "109"
			};
		}
	}

	[HttpPost]
	public ResponseResult AddStoreDMS(DataTable data)
	{
		try
		{
			BPImpls bPImpls = new BPImpls();
			bPImpls.Series = Function.ParseInt(data.Rows[0]["series"]);
			bPImpls.CardName = Function.ToString(data.Rows[0]["cardName"]);
			bPImpls.Cellular = Function.ToString(data.Rows[0]["cellular"]);
			if (!string.IsNullOrEmpty(Function.ToString(data.Rows[0]["address"])))
			{
				string text = Function.ToString(data.Rows[0]["address"]);
				if (text.Length > 99)
				{
					text = text.Substring(0, 99);
				}
				List<AddressesImpl> addresses = new List<AddressesImpl>
				{
					new AddressesImpl
					{
						Address = "DMS add store",
						Street = text,
						LineNum = -1,
						AdresType = "B"
					}
				};
				bPImpls.Addresses = addresses;
			}
			bPImpls.E_Mail = Function.ToString(data.Rows[0]["email"]);
			bPImpls.Notes = Function.ToString(data.Rows[0]["notes"]);
			bPImpls.ListUserFields = new List<UserFieldsImpl>
			{
				new UserFieldsImpl
				{
					Name = "U_SubType",
					Value = "S"
				},
				new UserFieldsImpl
				{
					Name = "U_Latitude",
					Value = Function.ToString(data.Rows[0]["latitude"])
				},
				new UserFieldsImpl
				{
					Name = "U_Longitude",
					Value = Function.ToString(data.Rows[0]["longitude"])
				}
			};
			if (data.ExistsColumn("cardType"))
			{
				bPImpls.ListUserFields.Add(new UserFieldsImpl
				{
					Name = "U_CardType",
					Value = Function.ToString(data.Rows[0]["cardType"])
				});
			}
			string text2 = "";
			if (data.ExistsColumn("businessType"))
			{
				text2 = Function.ToString(data.Rows[0]["businessType"]);
			}
			if (!string.IsNullOrEmpty(text2))
			{
				string[] array = text2.Split(',');
				List<string> list = new List<string>();
				for (int i = 0; i < 64; i++)
				{
					list.Add("N");
				}
				for (int j = 0; j < 64; j++)
				{
					for (int k = 0; k < array.Length; k++)
					{
						if (Function.ParseInt(array[k]) == j + 1)
						{
							list[j] = "Y";
						}
					}
				}
				bPImpls.ItemProperties = list;
			}
			if (!string.IsNullOrEmpty(Function.ToString(data.Rows[0]["ownerName"])))
			{
				bPImpls.ListContactPerson = new List<ContactPersonImpl>
				{
					new ContactPersonImpl
					{
						Active = "Y",
						BirthDate = Function.ParseDateTime(data.Rows[0]["ownerBirthDate"]),
						Cellolar = Function.ToString(data.Rows[0]["ownerMobile"]),
						E_MailL = Function.ToString(data.Rows[0]["ownerEmail"]),
						Name = Function.ToString(data.Rows[0]["ownerName"])
					}
				};
				bPImpls.IntrntSite = Function.ToString(data.Rows[0]["ownerFacebook"]);
			}
			bPImpls.SlpCode = Function.ParseInt(data.Rows[0]["salesPrson"]);
			bPImpls.CardType = "C";
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new ResponseResult
					{
						MessageCode = "500",
						Message = "Invalid SessionId"
					};
				}
				HttpResult httpResult = BrokerService.GetUnitOfWork(SessionId).BusinessPartners.AddOrUpdate(bPImpls, SessionUserInfo.SessionDiServer);
				if (string.IsNullOrEmpty(Function.ToString(httpResult.Message)))
				{
					return new ResponseResult
					{
						MessageCode = "200",
						Content = httpResult.Content,
						Message = ""
					};
				}
				return new ResponseResult
				{
					MessageCode = "400",
					Message = Function.ToString(httpResult.Message)
				};
			}
			return new ResponseResult
			{
				MessageCode = "404",
				Message = "Not Found"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "400"
			};
		}
	}

	[HttpPost]
	public ResponseResult UpdateStoreDMS(BPImpls data)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new ResponseResult
					{
						MessageCode = "500",
						Message = "Invalid SessionId"
					};
				}
				data.validFor = "Y";
				HttpResult httpResult = BrokerService.GetUnitOfWork(SessionId).BusinessPartners.Save(data, SessionUserInfo.SessionDiServer);
				if (string.IsNullOrEmpty(Function.ToString(httpResult.Message)))
				{
					return new ResponseResult
					{
						MessageCode = "200",
						Content = httpResult.Content,
						Message = ""
					};
				}
				return new ResponseResult
				{
					MessageCode = "400",
					Message = Function.ToString(httpResult.Message)
				};
			}
			return new ResponseResult
			{
				MessageCode = "404",
				Message = "Not Found"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "400"
			};
		}
	}

	[HttpPost]
	public ResponseResult AddNewStoreDMS(BPImpls data)
	{
		try
		{
			if (Global.LoginSap)
			{
				DataTable dataConfigDMS = UnitOfWork.Commons.GetDataConfigDMS("i-DMS");
				if (dataConfigDMS.IsNotNull())
				{
					data.CardCode = null;
					data.validFor = "Y";
					data.Series = Function.ParseInt(dataConfigDMS.Rows[0]["DefaultSeriesStore"]);
				}
				if (string.IsNullOrEmpty(SessionId))
				{
					return new ResponseResult
					{
						MessageCode = "500",
						Message = "Invalid SessionId"
					};
				}
				HttpResult httpResult = BrokerService.GetUnitOfWork(SessionId).BusinessPartners.AddOrUpdate(data, SessionUserInfo.SessionDiServer);
				if (string.IsNullOrEmpty(Function.ToString(httpResult.Message)))
				{
					return new ResponseResult
					{
						MessageCode = "200",
						Content = httpResult.Content,
						Message = ""
					};
				}
				return new ResponseResult
				{
					MessageCode = "400",
					Message = Function.ToString(httpResult.Message)
				};
			}
			return new ResponseResult
			{
				MessageCode = "404",
				Message = "Not Found"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "400"
			};
		}
	}

	[HttpPost]
	public ResponseResult UpdateCustomer(DataTable customerData)
	{
		try
		{
			DataRow dataRow = customerData.Rows[0];
			string text = Function.ToString(dataRow["cardCode"]);
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = _messageCodePrefix + "401"
				};
			}
			DataRow row = UnitOfWork.BusinessMasterData.GetDataQuickBPMasterData(text).Rows[0];
			DataTable dataTable = UnitOfWork.ContactPerson.ShowContactPerson(text);
			List<ContactPersonImpl> list = new List<ContactPersonImpl>();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				ContactPersonImpl item = Function.DataRowToEntity<ContactPersonImpl>(dataTable.Rows[i]);
				list.Add(item);
			}
			BPImpls bPImpls = Function.DataRowToEntity<BPImpls>(row);
			bPImpls.CardName = Function.ToString(dataRow["cardName"]);
			bPImpls.Cellular = Function.ToString(dataRow["cellular"]);
			bPImpls.E_Mail = Function.ToString(dataRow["email"]);
			bPImpls.GroupCode = Function.ParseInt(Function.ToString(dataRow["groupCode"]));
			bPImpls.OldCardCode = Function.ToString(dataRow["cardCode"]);
			bPImpls.ListContactPerson = list;
			bPImpls.ListUserFields = new List<UserFieldsImpl>
			{
				new UserFieldsImpl
				{
					Name = "U_Latitude",
					Value = Function.ToString(dataRow["latitude"])
				},
				new UserFieldsImpl
				{
					Name = "U_Longitude",
					Value = Function.ToString(dataRow["longitude"])
				}
			};
			bPImpls.IntrntSite = Function.ToString(dataRow["facebook"]);
			string text2 = Function.ToString(dataRow["address"]);
			if (text2.Length > 99)
			{
				text2 = text2.Substring(0, 99);
			}
			bPImpls.Addresses = new List<AddressesImpl>
			{
				new AddressesImpl
				{
					Address = "DMS update store",
					CardCode = Function.ToString(dataRow["cardCode"]),
					Street = text2,
					LineNum = -1,
					AdresType = "B"
				}
			};
			string text3 = "";
			if (customerData.ExistsColumn("businessType"))
			{
				text3 = Function.ToString(customerData.Rows[0]["businessType"]);
			}
			if (!string.IsNullOrEmpty(text3))
			{
				string[] array = text3.Split(',');
				List<string> list2 = new List<string>();
				for (int j = 0; j < 64; j++)
				{
					list2.Add("N");
				}
				for (int k = 0; k < 64; k++)
				{
					for (int l = 0; l < array.Length; l++)
					{
						if (Function.ParseInt(array[l]) == k + 1)
						{
							list2[k] = "Y";
						}
					}
				}
				bPImpls.ItemProperties = list2;
			}
			if (customerData.ExistsColumn("customerType"))
			{
				bPImpls.ListUserFields.Add(new UserFieldsImpl
				{
					Name = "U_CardType",
					Value = Function.ToString(customerData.Rows[0]["customerType"])
				});
			}
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new ResponseResult
					{
						MessageCode = _messageCodePrefix + "401",
						Message = "Invalid SessionId"
					};
				}
				HttpResult httpResult = BrokerService.GetUnitOfWork(SessionId).BusinessPartners.Save(bPImpls, SessionUserInfo.SessionDiServer);
				if (string.IsNullOrEmpty(Function.ToString(httpResult)))
				{
					return new ResponseResult
					{
						MessageCode = "0",
						Message = ""
					};
				}
				return new ResponseResult
				{
					MessageCode = "101",
					Message = Function.ToString(httpResult)
				};
			}
			return new ResponseResult
			{
				MessageCode = _messageCodePrefix + "405",
				Message = "Function not support"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = _messageCodePrefix + "107"
			};
		}
	}

	[HttpPost]
	public DataSet GetDataQuickBPPager(Tuple<int, int, DataTable> data)
	{
		if (Global.LoginSap && string.IsNullOrEmpty(SessionId))
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
			return new DataSet();
		}
		return UnitOfWork.BusinessMasterData.GetDataQuickBPPager(data.Item1, data.Item2, data.Item3);
	}

	[HttpGet]
	public object GetNumberOfRecordQuickBP()
	{
		if (Global.LoginSap && string.IsNullOrEmpty(SessionId))
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
			return null;
		}
		return UnitOfWork.BusinessMasterData.GetNumberOfRecordQuickBP();
	}

	[HttpGet]
	public DataTable DMSGetDataCardType()
	{
		try
		{
			if (Global.LoginSap && string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
				return null;
			}
			return UnitOfWork.BusinessMasterData.DMSGetDataCardType();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public DataTable DMSGetDataPropertiesBusinessType()
	{
		try
		{
			if (Global.LoginSap && string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
				return null;
			}
			return UnitOfWork.BusinessMasterData.DMSGetDataPropertiesBusinessType();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public ResponseResult DMSGetDetailAllStore(string userName)
	{
		try
		{
			DataSet dataSet = UnitOfWork.BusinessMasterData.DMSGetDetailAllStore(userName);
			if (dataSet.Tables.Count == 3)
			{
				dataSet.Tables[0].TableName = "dtDetail";
				dataSet.Tables[1].TableName = "dtDetailActivity";
				dataSet.Tables[2].TableName = "dtDetailContact";
				dataSet.Tables[0].AcceptChanges();
				dataSet.Tables[1].AcceptChanges();
				dataSet.Tables[2].AcceptChanges();
			}
			return new ResponseResult
			{
				Message = "",
				Content = dataSet,
				MessageCode = "200"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public ResponseResult CheckPotential(string cardCode)
	{
		try
		{
			DataTable content = UnitOfWork.BusinessMasterData.CheckPotential(cardCode);
			return new ResponseResult
			{
				Message = "",
				Content = content,
				MessageCode = "200"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "401"
			};
		}
	}
}
