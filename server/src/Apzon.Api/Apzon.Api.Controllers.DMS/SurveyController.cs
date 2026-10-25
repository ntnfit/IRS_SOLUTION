using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.DMS;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Api.Controllers.DMS;

[ValidateLoginSapExceptionHandling]
public class SurveyController : BaseApiController
{
	[HttpGet]
	public ResponseResult GetAllSurvey(int clgCode)
	{
		try
		{
			DataTable allSurvey = UnitOfWork.Survey.GetAllSurvey(clgCode);
			allSurvey.TableName = "survey";
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Table0 = allSurvey
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
				MessageCode = ex.Message
			};
		}
	}

	[HttpGet]
	public ResponseResult GetQuestionById()
	{
		try
		{
			int docentry = 1;
			DataTable questionById = UnitOfWork.Survey.GetQuestionById(docentry);
			questionById.TableName = "question";
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Table0 = questionById
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
				MessageCode = ex.Message
			};
		}
	}

	[HttpPost]
	public ResponseResult Add([FromBody] SurveyImpl entities)
	{
		try
		{
			if (entities == null)
			{
				return new ResponseResult
				{
					Content = new
					{
						Table0 = "Data null"
					}
				};
			}
			if (string.IsNullOrEmpty(SessionId))
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = "INVALIDSESSION"
				};
			}
			string userName = SessionUserInfo.UserName;
			if (string.IsNullOrEmpty(userName))
			{
				return new ResponseResult
				{
					Message = "User name invalid"
				};
			}
			entities.CreateTime = DateTime.Now;
			entities.UpdateTime = DateTime.Now;
			UnitOfWork.Users.GetUserSign(userName);
			HttpResult httpResult = UnitOfWork.Survey.Save(entities, "A");
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Code = httpResult.Content
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
				MessageCode = ex.Message
			};
		}
	}

	[HttpPost]
	public ResponseResult Update([FromBody] SurveyImpl entities)
	{
		try
		{
			if (entities == null)
			{
				return new ResponseResult
				{
					Content = new
					{
						Table0 = "Data null"
					}
				};
			}
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Content = new
					{
						Table0 = "401"
					}
				};
			}
			if (string.IsNullOrEmpty(SessionId))
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = "Lỗi401"
				};
			}
			entities.UpdateTime = DateTime.Now;
			HttpResult httpResult = UnitOfWork.Survey.Save(entities, "U");
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Code = httpResult.Content
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
				MessageCode = "Lỗi107",
				Content = new
				{
					Table0 = ex.Message
				}
			};
		}
	}

	[HttpGet]
	public DataSet GetSurveyById(int docEntry)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new DataSet();
			}
			return UnitOfWork.Survey.GetSurveyById(docEntry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	[HttpPost]
	public ResponseResult Delete([FromBody] SurveyImpl model)
	{
		try
		{
			if (model == null)
			{
				return new ResponseResult
				{
					Message = "Data null",
					Content = new
					{
						Table0 = "Data null"
					}
				};
			}
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					Content = new
					{
						Table0 = "401"
					}
				};
			}
			if (string.IsNullOrEmpty(SessionId))
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					MessageCode = "Lỗi401"
				};
			}
			if (string.IsNullOrEmpty(SessionUserInfo.UserName))
			{
				return new ResponseResult
				{
					Message = "User name invalid",
					Content = new
					{
						Table0 = "User name invalid"
					}
				};
			}
			HttpResult httpResult = UnitOfWork.Survey.Save(model, "D");
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Code = httpResult.Content
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Content = new
				{
					Table0 = ex.Message
				}
			};
		}
	}
}
