using System;

namespace Apzon.Api.Models;

public class TokenAuthorizationImpl
{
	public string SessionId { get; set; }

	public DateTime ExpiredDate { get; set; }
}
