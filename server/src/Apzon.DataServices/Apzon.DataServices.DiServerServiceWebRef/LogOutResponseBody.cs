using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Apzon.DataServices.DiServerServiceWebRef;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[DataContract(Namespace = "http://www.sap.com/SBO/DIS")]
public class LogOutResponseBody
{
	[DataMember(EmitDefaultValue = false, Order = 0)]
	public object LogOutResult;

	public LogOutResponseBody()
	{
	}

	public LogOutResponseBody(object LogOutResult)
	{
		this.LogOutResult = LogOutResult;
	}
}
