using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Apzon.DataServices.DiServerServiceWebRef;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[MessageContract(IsWrapped = false)]
public class LoginRequest
{
	[MessageBodyMember(Name = "Login", Namespace = "http://www.sap.com/SBO/DIS", Order = 0)]
	public LoginRequestBody Body;

	public LoginRequest()
	{
	}

	public LoginRequest(LoginRequestBody Body)
	{
		this.Body = Body;
	}
}
