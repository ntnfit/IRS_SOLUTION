using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Apzon.DataServices.DiServerServiceWebRef;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[DataContract(Namespace = "http://www.sap.com/SBO/DIS")]
public class AddObjectRequestBody
{
	[DataMember(EmitDefaultValue = false, Order = 0)]
	public string sessionId;

	[DataMember(EmitDefaultValue = false, Order = 1)]
	public string xmlObject;

	public AddObjectRequestBody()
	{
	}

	public AddObjectRequestBody(string sessionId, string xmlObject)
	{
		this.sessionId = sessionId;
		this.xmlObject = xmlObject;
	}
}
