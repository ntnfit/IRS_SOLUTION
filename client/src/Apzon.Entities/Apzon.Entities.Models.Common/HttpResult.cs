using Apzon.Commons.Enums;

namespace Apzon.Entities.Models.Common;

public class HttpResult
{
	public object Content { get; set; }

	public string Message { get; set; }

	public Enums.MessageCode MessageCode { get; set; }

	public HttpResult()
	{
	}

	public HttpResult(Enums.MessageCode messageCode, string message = "", object obj = null)
	{
		MessageCode = messageCode;
		Content = obj;
		Message = message;
	}
}
