using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Apzon.DataServices.DiServerServiceWebRef;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[DataContract(Namespace = "http://www.sap.com/SBO/DIS")]
public class GetBusinessObjectTemplateRequestBody
{
	[DataMember(EmitDefaultValue = false, Order = 0)]
	public string sessionId;

	[DataMember(EmitDefaultValue = false, Order = 1)]
	public string objType;

	public GetBusinessObjectTemplateRequestBody()
	{
	}

	public GetBusinessObjectTemplateRequestBody(string sessionId, string objType)
	{
		this.sessionId = sessionId;
		this.objType = objType;
	}
}
