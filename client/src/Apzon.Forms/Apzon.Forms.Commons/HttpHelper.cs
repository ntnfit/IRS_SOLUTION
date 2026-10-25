using System.Net;

namespace Apzon.Forms.Commons;

public class HttpHelper
{
	public static string GetStatus(HttpStatusCode statusCode)
	{
		return statusCode switch
		{
			HttpStatusCode.Accepted => "Accepted", 
			HttpStatusCode.MultipleChoices => "Ambiguous", 
			HttpStatusCode.BadGateway => "BadGateway", 
			HttpStatusCode.BadRequest => "BadRequest", 
			HttpStatusCode.Conflict => "Conflict", 
			HttpStatusCode.Continue => "Continue", 
			HttpStatusCode.Created => "Created", 
			HttpStatusCode.ExpectationFailed => "ExpectationFailed", 
			HttpStatusCode.Forbidden => "Forbidden", 
			HttpStatusCode.Found => "Found", 
			HttpStatusCode.GatewayTimeout => "GatewayTimeout", 
			HttpStatusCode.Gone => "Gone", 
			HttpStatusCode.HttpVersionNotSupported => "HttpVersionNotSupported", 
			HttpStatusCode.InternalServerError => "InternalServerError", 
			HttpStatusCode.LengthRequired => "LengthRequired", 
			HttpStatusCode.MethodNotAllowed => "MethodNotAllowed", 
			HttpStatusCode.MovedPermanently => "Moved", 
			HttpStatusCode.NoContent => "NoContent", 
			HttpStatusCode.NonAuthoritativeInformation => "NonAuthoritativeInformation", 
			HttpStatusCode.NotAcceptable => "NotAcceptable", 
			HttpStatusCode.NotFound => "NotFound", 
			HttpStatusCode.NotImplemented => "NotImplemented", 
			HttpStatusCode.NotModified => "NotModified", 
			HttpStatusCode.OK => "OK", 
			HttpStatusCode.PartialContent => "PartialContent", 
			HttpStatusCode.PaymentRequired => "PaymentRequired", 
			HttpStatusCode.PreconditionFailed => "PreconditionFailed", 
			HttpStatusCode.ProxyAuthenticationRequired => "ProxyAuthenticationRequired", 
			HttpStatusCode.TemporaryRedirect => "RedirectKeepVerb", 
			HttpStatusCode.SeeOther => "RedirectMethod", 
			HttpStatusCode.RequestEntityTooLarge => "RequestEntityTooLarge", 
			HttpStatusCode.RequestTimeout => "RequestTimeout", 
			HttpStatusCode.RequestUriTooLong => "RequestUriTooLong", 
			HttpStatusCode.RequestedRangeNotSatisfiable => "RequestedRangeNotSatisfiable", 
			HttpStatusCode.ResetContent => "ResetContent", 
			HttpStatusCode.ServiceUnavailable => "ServiceUnavailable", 
			HttpStatusCode.SwitchingProtocols => "SwitchingProtocols", 
			HttpStatusCode.Unauthorized => "Unauthorized", 
			HttpStatusCode.UnsupportedMediaType => "UnsupportedMediaType", 
			HttpStatusCode.Unused => "Unused", 
			HttpStatusCode.UpgradeRequired => "UpgradeRequired", 
			HttpStatusCode.UseProxy => "UseProxy", 
			_ => "Nothing to do!!!", 
		};
	}
}
