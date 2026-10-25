using System;
using System.Runtime.Serialization;
using System.Security;

namespace Apzon.Commons.ExceptionEx;

public class ExceptionEx : Exception
{
	public string LogMessage { get; set; }

	public int ExceptionCode { get; set; }

	public override string Message { get; }

	public ExceptionEx()
	{
	}

	public ExceptionEx(string message)
	{
	}

	public ExceptionEx(string message, Exception innerException)
	{
	}

	[SecuritySafeCritical]
	protected ExceptionEx(SerializationInfo info, StreamingContext context)
	{
	}
}
