using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Apzon.DataServices.DiServerServiceWebRef;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[MessageContract(IsWrapped = false)]
public class BatchProcessResponse
{
	[MessageBodyMember(Name = "BatchProcessResponse", Namespace = "http://www.sap.com/SBO/DIS", Order = 0)]
	public BatchProcessResponseBody Body;

	public BatchProcessResponse()
	{
	}

	public BatchProcessResponse(BatchProcessResponseBody Body)
	{
		this.Body = Body;
	}
}
