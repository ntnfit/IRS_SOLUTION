using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Apzon.DataServices.DiServerServiceWebRef;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[MessageContract(IsWrapped = false)]
public class AddObjectRequest
{
	[MessageBodyMember(Name = "AddObject", Namespace = "http://www.sap.com/SBO/DIS", Order = 0)]
	public AddObjectRequestBody Body;

	public AddObjectRequest()
	{
	}

	public AddObjectRequest(AddObjectRequestBody Body)
	{
		this.Body = Body;
	}
}
