using System;

namespace Apzon.Commons.ExceptionHandling;

public class BaseException : Exception
{
	public int ErrorId { get; set; }

	public string ErrorString { get; set; }

	public string ErrorMessage { get; set; }

	public string ErrorPrefix { get; set; }

	public BaseException()
	{
	}

	public BaseException(string message)
	{
		ErrorString = message;
	}

	public virtual void GetError()
	{
		GetBaseException().GetType();
		ErrorMessage = $"{ErrorPrefix}{ErrorString} - {Message}";
	}
}
