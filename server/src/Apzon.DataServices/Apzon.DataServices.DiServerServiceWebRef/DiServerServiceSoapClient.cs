using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Xml;

namespace Apzon.DataServices.DiServerServiceWebRef;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
public class DiServerServiceSoapClient : ClientBase<DiServerServiceSoap>, DiServerServiceSoap
{
	public DiServerServiceSoapClient()
	{
	}

	public DiServerServiceSoapClient(string endpointConfigurationName)
		: base(endpointConfigurationName)
	{
	}

	public DiServerServiceSoapClient(string endpointConfigurationName, string remoteAddress)
		: base(endpointConfigurationName, remoteAddress)
	{
	}

	public DiServerServiceSoapClient(string endpointConfigurationName, EndpointAddress remoteAddress)
		: base(endpointConfigurationName, remoteAddress)
	{
	}

	public DiServerServiceSoapClient(Binding binding, EndpointAddress remoteAddress)
		: base(binding, remoteAddress)
	{
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	GetBusinessObjectTemplateResponse DiServerServiceSoap.GetBusinessObjectTemplate(GetBusinessObjectTemplateRequest request)
	{
		return Channel.GetBusinessObjectTemplate(request);
	}

	public XmlElement GetBusinessObjectTemplate(string sessionId, string objType)
	{
		GetBusinessObjectTemplateRequest getBusinessObjectTemplateRequest = new GetBusinessObjectTemplateRequest();
		getBusinessObjectTemplateRequest.Body = new GetBusinessObjectTemplateRequestBody();
		getBusinessObjectTemplateRequest.Body.sessionId = sessionId;
		getBusinessObjectTemplateRequest.Body.objType = objType;
		return ((DiServerServiceSoap)this).GetBusinessObjectTemplate(getBusinessObjectTemplateRequest).Body.GetBusinessObjectTemplateResult;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<GetBusinessObjectTemplateResponse> DiServerServiceSoap.GetBusinessObjectTemplateAsync(GetBusinessObjectTemplateRequest request)
	{
		return Channel.GetBusinessObjectTemplateAsync(request);
	}

	public Task<GetBusinessObjectTemplateResponse> GetBusinessObjectTemplateAsync(string sessionId, string objType)
	{
		GetBusinessObjectTemplateRequest getBusinessObjectTemplateRequest = new GetBusinessObjectTemplateRequest();
		getBusinessObjectTemplateRequest.Body = new GetBusinessObjectTemplateRequestBody();
		getBusinessObjectTemplateRequest.Body.sessionId = sessionId;
		getBusinessObjectTemplateRequest.Body.objType = objType;
		return ((DiServerServiceSoap)this).GetBusinessObjectTemplateAsync(getBusinessObjectTemplateRequest);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	GetServiceMetaDataResponse DiServerServiceSoap.GetServiceMetaData(GetServiceMetaDataRequest request)
	{
		return Channel.GetServiceMetaData(request);
	}

	public XmlElement GetServiceMetaData(string sessionId, string servicename)
	{
		GetServiceMetaDataRequest getServiceMetaDataRequest = new GetServiceMetaDataRequest();
		getServiceMetaDataRequest.Body = new GetServiceMetaDataRequestBody();
		getServiceMetaDataRequest.Body.sessionId = sessionId;
		getServiceMetaDataRequest.Body.servicename = servicename;
		return ((DiServerServiceSoap)this).GetServiceMetaData(getServiceMetaDataRequest).Body.GetServiceMetaDataResult;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<GetServiceMetaDataResponse> DiServerServiceSoap.GetServiceMetaDataAsync(GetServiceMetaDataRequest request)
	{
		return Channel.GetServiceMetaDataAsync(request);
	}

	public Task<GetServiceMetaDataResponse> GetServiceMetaDataAsync(string sessionId, string servicename)
	{
		GetServiceMetaDataRequest getServiceMetaDataRequest = new GetServiceMetaDataRequest();
		getServiceMetaDataRequest.Body = new GetServiceMetaDataRequestBody();
		getServiceMetaDataRequest.Body.sessionId = sessionId;
		getServiceMetaDataRequest.Body.servicename = servicename;
		return ((DiServerServiceSoap)this).GetServiceMetaDataAsync(getServiceMetaDataRequest);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	BatchProcessResponse DiServerServiceSoap.BatchProcess(BatchProcessRequest request)
	{
		return Channel.BatchProcess(request);
	}

	public XmlElement BatchProcess(string xmlObject)
	{
		BatchProcessRequest batchProcessRequest = new BatchProcessRequest();
		batchProcessRequest.Body = new BatchProcessRequestBody();
		batchProcessRequest.Body.xmlObject = xmlObject;
		return ((DiServerServiceSoap)this).BatchProcess(batchProcessRequest).Body.BatchProcessResult;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<BatchProcessResponse> DiServerServiceSoap.BatchProcessAsync(BatchProcessRequest request)
	{
		return Channel.BatchProcessAsync(request);
	}

	public Task<BatchProcessResponse> BatchProcessAsync(string xmlObject)
	{
		BatchProcessRequest batchProcessRequest = new BatchProcessRequest();
		batchProcessRequest.Body = new BatchProcessRequestBody();
		batchProcessRequest.Body.xmlObject = xmlObject;
		return ((DiServerServiceSoap)this).BatchProcessAsync(batchProcessRequest);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	ProcessResponse DiServerServiceSoap.Process(ProcessRequest request)
	{
		return Channel.Process(request);
	}

	public XmlElement Process(string xmlObject)
	{
		ProcessRequest processRequest = new ProcessRequest();
		processRequest.Body = new ProcessRequestBody();
		processRequest.Body.xmlObject = xmlObject;
		return ((DiServerServiceSoap)this).Process(processRequest).Body.ProcessResult;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<ProcessResponse> DiServerServiceSoap.ProcessAsync(ProcessRequest request)
	{
		return Channel.ProcessAsync(request);
	}

	public Task<ProcessResponse> ProcessAsync(string xmlObject)
	{
		ProcessRequest processRequest = new ProcessRequest();
		processRequest.Body = new ProcessRequestBody();
		processRequest.Body.xmlObject = xmlObject;
		return ((DiServerServiceSoap)this).ProcessAsync(processRequest);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	AddObjectResponse DiServerServiceSoap.AddObject(AddObjectRequest request)
	{
		return Channel.AddObject(request);
	}

	public XmlElement AddObject(string sessionId, string xmlObject)
	{
		AddObjectRequest addObjectRequest = new AddObjectRequest();
		addObjectRequest.Body = new AddObjectRequestBody();
		addObjectRequest.Body.sessionId = sessionId;
		addObjectRequest.Body.xmlObject = xmlObject;
		return ((DiServerServiceSoap)this).AddObject(addObjectRequest).Body.AddObjectResult;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<AddObjectResponse> DiServerServiceSoap.AddObjectAsync(AddObjectRequest request)
	{
		return Channel.AddObjectAsync(request);
	}

	public Task<AddObjectResponse> AddObjectAsync(string sessionId, string xmlObject)
	{
		AddObjectRequest addObjectRequest = new AddObjectRequest();
		addObjectRequest.Body = new AddObjectRequestBody();
		addObjectRequest.Body.sessionId = sessionId;
		addObjectRequest.Body.xmlObject = xmlObject;
		return ((DiServerServiceSoap)this).AddObjectAsync(addObjectRequest);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	UpdateObjectResponse DiServerServiceSoap.UpdateObject(UpdateObjectRequest request)
	{
		return Channel.UpdateObject(request);
	}

	public XmlElement UpdateObject(string sessionId, string xmlObject)
	{
		UpdateObjectRequest updateObjectRequest = new UpdateObjectRequest();
		updateObjectRequest.Body = new UpdateObjectRequestBody();
		updateObjectRequest.Body.sessionId = sessionId;
		updateObjectRequest.Body.xmlObject = xmlObject;
		return ((DiServerServiceSoap)this).UpdateObject(updateObjectRequest).Body.UpdateObjectResult;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<UpdateObjectResponse> DiServerServiceSoap.UpdateObjectAsync(UpdateObjectRequest request)
	{
		return Channel.UpdateObjectAsync(request);
	}

	public Task<UpdateObjectResponse> UpdateObjectAsync(string sessionId, string xmlObject)
	{
		UpdateObjectRequest updateObjectRequest = new UpdateObjectRequest();
		updateObjectRequest.Body = new UpdateObjectRequestBody();
		updateObjectRequest.Body.sessionId = sessionId;
		updateObjectRequest.Body.xmlObject = xmlObject;
		return ((DiServerServiceSoap)this).UpdateObjectAsync(updateObjectRequest);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	GetByKeyResponse DiServerServiceSoap.GetByKey(GetByKeyRequest request)
	{
		return Channel.GetByKey(request);
	}

	public XmlElement GetByKey(string sessionId, string xmlObject)
	{
		GetByKeyRequest getByKeyRequest = new GetByKeyRequest();
		getByKeyRequest.Body = new GetByKeyRequestBody();
		getByKeyRequest.Body.sessionId = sessionId;
		getByKeyRequest.Body.xmlObject = xmlObject;
		return ((DiServerServiceSoap)this).GetByKey(getByKeyRequest).Body.GetByKeyResult;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<GetByKeyResponse> DiServerServiceSoap.GetByKeyAsync(GetByKeyRequest request)
	{
		return Channel.GetByKeyAsync(request);
	}

	public Task<GetByKeyResponse> GetByKeyAsync(string sessionId, string xmlObject)
	{
		GetByKeyRequest getByKeyRequest = new GetByKeyRequest();
		getByKeyRequest.Body = new GetByKeyRequestBody();
		getByKeyRequest.Body.sessionId = sessionId;
		getByKeyRequest.Body.xmlObject = xmlObject;
		return ((DiServerServiceSoap)this).GetByKeyAsync(getByKeyRequest);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	LoginResponse DiServerServiceSoap.Login(LoginRequest request)
	{
		return Channel.Login(request);
	}

	public string Login(string DataBaseServer, string DataBaseName, string DataBaseType, string DataBaseUserName, string DataBasePassword, string CompanyUserName, string CompanyPassword, string Language, string LicenseServer)
	{
		LoginRequest loginRequest = new LoginRequest();
		loginRequest.Body = new LoginRequestBody();
		loginRequest.Body.DataBaseServer = DataBaseServer;
		loginRequest.Body.DataBaseName = DataBaseName;
		loginRequest.Body.DataBaseType = DataBaseType;
		loginRequest.Body.DataBaseUserName = DataBaseUserName;
		loginRequest.Body.DataBasePassword = DataBasePassword;
		loginRequest.Body.CompanyUserName = CompanyUserName;
		loginRequest.Body.CompanyPassword = CompanyPassword;
		loginRequest.Body.Language = Language;
		loginRequest.Body.LicenseServer = LicenseServer;
		return ((DiServerServiceSoap)this).Login(loginRequest).Body.LoginResult;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<LoginResponse> DiServerServiceSoap.LoginAsync(LoginRequest request)
	{
		return Channel.LoginAsync(request);
	}

	public Task<LoginResponse> LoginAsync(string DataBaseServer, string DataBaseName, string DataBaseType, string DataBaseUserName, string DataBasePassword, string CompanyUserName, string CompanyPassword, string Language, string LicenseServer)
	{
		LoginRequest loginRequest = new LoginRequest();
		loginRequest.Body = new LoginRequestBody();
		loginRequest.Body.DataBaseServer = DataBaseServer;
		loginRequest.Body.DataBaseName = DataBaseName;
		loginRequest.Body.DataBaseType = DataBaseType;
		loginRequest.Body.DataBaseUserName = DataBaseUserName;
		loginRequest.Body.DataBasePassword = DataBasePassword;
		loginRequest.Body.CompanyUserName = CompanyUserName;
		loginRequest.Body.CompanyPassword = CompanyPassword;
		loginRequest.Body.Language = Language;
		loginRequest.Body.LicenseServer = LicenseServer;
		return ((DiServerServiceSoap)this).LoginAsync(loginRequest);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	LogOutResponse DiServerServiceSoap.LogOut(LogOutRequest request)
	{
		return Channel.LogOut(request);
	}

	public object LogOut(string sSessionID)
	{
		LogOutRequest logOutRequest = new LogOutRequest();
		logOutRequest.Body = new LogOutRequestBody();
		logOutRequest.Body.sSessionID = sSessionID;
		return ((DiServerServiceSoap)this).LogOut(logOutRequest).Body.LogOutResult;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<LogOutResponse> DiServerServiceSoap.LogOutAsync(LogOutRequest request)
	{
		return Channel.LogOutAsync(request);
	}

	public Task<LogOutResponse> LogOutAsync(string sSessionID)
	{
		LogOutRequest logOutRequest = new LogOutRequest();
		logOutRequest.Body = new LogOutRequestBody();
		logOutRequest.Body.sSessionID = sSessionID;
		return ((DiServerServiceSoap)this).LogOutAsync(logOutRequest);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	GetServiceStatusResponse DiServerServiceSoap.GetServiceStatus(GetServiceStatusRequest request)
	{
		return Channel.GetServiceStatus(request);
	}

	public string GetServiceStatus()
	{
		GetServiceStatusRequest getServiceStatusRequest = new GetServiceStatusRequest();
		getServiceStatusRequest.Body = new GetServiceStatusRequestBody();
		return ((DiServerServiceSoap)this).GetServiceStatus(getServiceStatusRequest).Body.GetServiceStatusResult;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	Task<GetServiceStatusResponse> DiServerServiceSoap.GetServiceStatusAsync(GetServiceStatusRequest request)
	{
		return Channel.GetServiceStatusAsync(request);
	}

	public Task<GetServiceStatusResponse> GetServiceStatusAsync()
	{
		GetServiceStatusRequest getServiceStatusRequest = new GetServiceStatusRequest();
		getServiceStatusRequest.Body = new GetServiceStatusRequestBody();
		return ((DiServerServiceSoap)this).GetServiceStatusAsync(getServiceStatusRequest);
	}
}
