using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Api.Controllers.HumanResources;

[ValidateLoginSapExceptionHandling]
public class EmpMasterDataController : BaseApiController
{
	[HttpPost]
	public DataTable Search([FromBody] Tuple<DataTable, string> model)
	{
		return UnitOfWork.EmpMasterData.Search(model.Item1, model.Item2);
	}

	[HttpGet]
	public string GetRecord(int _empID, int type, string userCode)
	{
		return UnitOfWork.EmpMasterData.GetRecord(_empID, type, userCode);
	}

	[HttpGet]
	public DataSet GetDetail(int _empID)
	{
		return UnitOfWork.EmpMasterData.GetDetail(_empID);
	}

	[HttpGet]
	public DataTable GetTeamMembers()
	{
		return UnitOfWork.EmpMasterData.GetTeamMembers();
	}

	[HttpGet]
	public DataTable ShowBankAccByCountryCod(string countryCod)
	{
		return UnitOfWork.EmpMasterData.ShowBankAccByCountryCod(countryCod);
	}

	[HttpGet]
	public DataTable ExistFileInDoc(int empId, string fileName, string fileExt)
	{
		return UnitOfWork.EmpMasterData.ExistFileInDoc(empId, fileName, fileExt);
	}

	[HttpPost]
	public string Save([FromBody] Tuple<string, DataSet> model)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return "";
				}
				return BrokerService.GetUnitOfWork(SessionId).EmployeeMasterData.Save(model.Item2, SessionUserInfo.SessionDiServer).Message;
			}
			return UnitOfWork.EmpMasterData.SaveDocDataSet(model.Item2, model.Item1).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpPost]
	[MappingSap(IsMapping = true)]
	public string DeleteDoc([FromBody] int empID)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return "Invalid session";
				}
				return BrokerService.GetUnitOfWork(SessionId).EmployeeMasterData.Remove(empID, SessionUserInfo.SessionDiServer);
			}
			return UnitOfWork.EmpMasterData.DeleteDoc(empID);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public bool CheckDoc(int empId)
	{
		return UnitOfWork.EmpMasterData.CheckDoc(empId);
	}

	[HttpPost]
	public HttpResult SaveDocDataSet([FromBody] DataSet ds)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error);
				}
				return BrokerService.GetUnitOfWork(SessionId).EmployeeMasterData.Save(ds, SessionUserInfo.SessionDiServer);
			}
			return UnitOfWork.EmpMasterData.SaveDocDataSet(ds, "A");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpPost]
	public string SaveWithRollBack([FromBody] DataTable dt)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return "";
				}
				return BrokerService.GetUnitOfWork(SessionId).EmployeeMasterData.SaveListHeader(dt, SessionUserInfo.SessionDiServer);
			}
			return UnitOfWork.EmpMasterData.SaveWithRollBack(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpPost]
	public HttpResult SaveDocDataSetUpdate([FromBody] DataSet ds)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error);
				}
				return BrokerService.GetUnitOfWork(SessionId).EmployeeMasterData.Save(ds, SessionUserInfo.SessionDiServer);
			}
			return UnitOfWork.EmpMasterData.SaveDocDataSet(ds, "U");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public DataTable GetOrgChartData(int empID)
	{
		return UnitOfWork.EmpMasterData.GetOrgChartData(empID);
	}

	[HttpGet]
	public ResponseResult GetDataEmployeeDMS(int pageIndex, string searchKey, int empID, int userId)
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
			DataTable dataEmployeeDMS = UnitOfWork.EmpMasterData.GetDataEmployeeDMS(pageIndex, searchKey, empID, userId);
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Table0 = dataEmployeeDMS
				},
				MessageCode = "406"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "107"
			};
		}
	}

	private static ImageCodecInfo GetEncoderInfo(string mimeType)
	{
		try
		{
			ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
			for (int i = 0; i < imageEncoders.Length; i++)
			{
				if (imageEncoders[i].MimeType == mimeType)
				{
					return imageEncoders[i];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	public static void SaveJpeg(string path, Image img, int quality)
	{
		try
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
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	[HttpPost]
	public ResponseResult UploadAvatarEmployeeDMS(DataTable param)
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
			DataTable dataTable = UnitOfWork.EmpMasterData.UploadAvatarEmployeeDMS(param);
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Result"])))
			{
				return new ResponseResult
				{
					Message = Function.ToString(dataTable.Rows[0]["Result"]),
					MessageCode = "200"
				};
			}
			if (!string.IsNullOrEmpty(Function.ToString(param.Rows[0]["Avatar"])))
			{
				string path = Function.ToString(dataTable.Rows[0]["FilePath"]);
				string path2 = Function.ToString(Path.GetDirectoryName(path));
				if (!Directory.Exists(path2))
				{
					Directory.CreateDirectory(path2);
				}
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				Convert.FromBase64String(Function.ToString(param.Rows[0]["Avatar"]));
				Image img = Image.FromStream(new MemoryStream(Convert.FromBase64String(Function.ToString(param.Rows[0]["Avatar"]))));
				SaveJpeg(path, img, 35);
			}
			return new ResponseResult
			{
				Message = "",
				MessageCode = "200"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "404"
			};
		}
	}

	[HttpGet]
	public DataTableResult GetEmployeeInfo(int empId)
	{
		return UnitOfWork.EmpMasterData.GetEmployeeInfo(empId);
	}
}
