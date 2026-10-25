using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Apzon.DataServices.DiServerServiceWebRef;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[MessageContract(IsWrapped = false)]
public class LoginResponse
{
	[MessageBodyMember(Name = "LoginResponse", Namespace = "http://www.sap.com/SBO/DIS", Order = 0)]
	public LoginResponseBody Body;

	public LoginResponse()
	{
	}

	public LoginResponse(LoginResponseBody Body)
	{
		this.Body = Body;
	}
}
