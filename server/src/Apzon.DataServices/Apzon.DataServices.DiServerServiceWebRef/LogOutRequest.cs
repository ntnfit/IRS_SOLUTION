using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Apzon.DataServices.DiServerServiceWebRef;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[MessageContract(IsWrapped = false)]
public class LogOutRequest
{
	[MessageBodyMember(Name = "LogOut", Namespace = "http://www.sap.com/SBO/DIS", Order = 0)]
	public LogOutRequestBody Body;

	public LogOutRequest()
	{
	}

	public LogOutRequest(LogOutRequestBody Body)
	{
		this.Body = Body;
	}
}
