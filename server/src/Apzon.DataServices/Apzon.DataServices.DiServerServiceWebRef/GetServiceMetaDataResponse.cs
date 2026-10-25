using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Apzon.DataServices.DiServerServiceWebRef;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[MessageContract(IsWrapped = false)]
public class GetServiceMetaDataResponse
{
	[MessageBodyMember(Name = "GetServiceMetaDataResponse", Namespace = "http://www.sap.com/SBO/DIS", Order = 0)]
	public GetServiceMetaDataResponseBody Body;

	public GetServiceMetaDataResponse()
	{
	}

	public GetServiceMetaDataResponse(GetServiceMetaDataResponseBody Body)
	{
		this.Body = Body;
	}
}
