using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Apzon.DataServices.DiServerServiceWebRef;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[MessageContract(IsWrapped = false)]
public class ProcessRequest
{
	[MessageBodyMember(Name = "Process", Namespace = "http://www.sap.com/SBO/DIS", Order = 0)]
	public ProcessRequestBody Body;

	public ProcessRequest()
	{
	}

	public ProcessRequest(ProcessRequestBody Body)
	{
		this.Body = Body;
	}
}
