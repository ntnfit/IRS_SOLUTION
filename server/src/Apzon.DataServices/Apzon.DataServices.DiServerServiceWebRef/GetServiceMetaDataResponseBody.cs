using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;

namespace Apzon.DataServices.DiServerServiceWebRef;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[DataContract(Namespace = "http://www.sap.com/SBO/DIS")]
public class GetServiceMetaDataResponseBody
{
	[DataMember(EmitDefaultValue = false, Order = 0)]
	public XmlElement GetServiceMetaDataResult;

	public GetServiceMetaDataResponseBody()
	{
	}

	public GetServiceMetaDataResponseBody(XmlElement GetServiceMetaDataResult)
	{
		this.GetServiceMetaDataResult = GetServiceMetaDataResult;
	}
}
