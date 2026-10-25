namespace Apzon.Entities.Models.Common;

public class ResponseMessage
{
	public object Content { get; set; }

	public string Message { get; set; }

	public int Code { get; set; }

	public ResponseMessage()
	{
	}

	public ResponseMessage(string msg)
	{
		Code = 505;
		Message = msg;
	}

	public ResponseMessage(string msg, object obj)
	{
		Message = msg;
		Content = obj;
		Code = 505;
	}

	public ResponseMessage(object obj)
	{
		Code = 0;
		Message = "";
		Content = obj;
	}
}
