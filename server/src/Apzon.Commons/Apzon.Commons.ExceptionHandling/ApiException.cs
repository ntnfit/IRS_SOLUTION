namespace Apzon.Commons.ExceptionHandling;

public class ApiException : BaseException
{
	public ApiException()
	{
		ErrorPrefix = "2";
	}
}
