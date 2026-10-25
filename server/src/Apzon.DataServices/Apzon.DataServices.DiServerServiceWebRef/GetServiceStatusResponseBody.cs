using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Apzon.DataServices.DiServerServiceWebRef;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[DataContract(Namespace = "http://www.sap.com/SBO/DIS")]
public class GetServiceStatusResponseBody
{
	[DataMember(EmitDefaultValue = false, Order = 0)]
	public string GetServiceStatusResult;

	public GetServiceStatusResponseBody()
	{
	}

	public GetServiceStatusResponseBody(string GetServiceStatusResult)
	{
		this.GetServiceStatusResult = GetServiceStatusResult;
	}
}
