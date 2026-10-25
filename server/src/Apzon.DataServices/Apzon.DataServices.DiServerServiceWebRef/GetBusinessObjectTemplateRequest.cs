using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Apzon.DataServices.DiServerServiceWebRef;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[MessageContract(IsWrapped = false)]
public class GetBusinessObjectTemplateRequest
{
	[MessageBodyMember(Name = "GetBusinessObjectTemplate", Namespace = "http://www.sap.com/SBO/DIS", Order = 0)]
	public GetBusinessObjectTemplateRequestBody Body;

	public GetBusinessObjectTemplateRequest()
	{
	}

	public GetBusinessObjectTemplateRequest(GetBusinessObjectTemplateRequestBody Body)
	{
		this.Body = Body;
	}
}
