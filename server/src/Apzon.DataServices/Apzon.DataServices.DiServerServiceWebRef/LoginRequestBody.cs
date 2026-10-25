using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Apzon.DataServices.DiServerServiceWebRef;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
[DataContract(Namespace = "http://www.sap.com/SBO/DIS")]
public class LoginRequestBody
{
	[DataMember(EmitDefaultValue = false, Order = 0)]
	public string DataBaseServer;

	[DataMember(EmitDefaultValue = false, Order = 1)]
	public string DataBaseName;

	[DataMember(EmitDefaultValue = false, Order = 2)]
	public string DataBaseType;

	[DataMember(EmitDefaultValue = false, Order = 3)]
	public string DataBaseUserName;

	[DataMember(EmitDefaultValue = false, Order = 4)]
	public string DataBasePassword;

	[DataMember(EmitDefaultValue = false, Order = 5)]
	public string CompanyUserName;

	[DataMember(EmitDefaultValue = false, Order = 6)]
	public string CompanyPassword;

	[DataMember(EmitDefaultValue = false, Order = 7)]
	public string Language;

	[DataMember(EmitDefaultValue = false, Order = 8)]
	public string LicenseServer;

	public LoginRequestBody()
	{
	}

	public LoginRequestBody(string DataBaseServer, string DataBaseName, string DataBaseType, string DataBaseUserName, string DataBasePassword, string CompanyUserName, string CompanyPassword, string Language, string LicenseServer)
	{
		this.DataBaseServer = DataBaseServer;
		this.DataBaseName = DataBaseName;
		this.DataBaseType = DataBaseType;
		this.DataBaseUserName = DataBaseUserName;
		this.DataBasePassword = DataBasePassword;
		this.CompanyUserName = CompanyUserName;
		this.CompanyPassword = CompanyPassword;
		this.Language = Language;
		this.LicenseServer = LicenseServer;
	}
}
