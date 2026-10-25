using System.CodeDom.Compiler;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Apzon.DataServices.DiServerServiceWebRef;

[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[ServiceContract(Namespace = "http://www.sap.com/SBO/DIS", ConfigurationName = "DiServerServiceWebRef.DiServerServiceSoap")]
public interface DiServerServiceSoap
{
	[OperationContract(Action = "http://www.sap.com/SBO/DIS/GetBusinessObjectTemplate", ReplyAction = "*")]
	GetBusinessObjectTemplateResponse GetBusinessObjectTemplate(GetBusinessObjectTemplateRequest request);

	[OperationContract(Action = "http://www.sap.com/SBO/DIS/GetBusinessObjectTemplate", ReplyAction = "*")]
	Task<GetBusinessObjectTemplateResponse> GetBusinessObjectTemplateAsync(GetBusinessObjectTemplateRequest request);

	[OperationContract(Action = "http://www.sap.com/SBO/DIS/GetServiceMetaData", ReplyAction = "*")]
	GetServiceMetaDataResponse GetServiceMetaData(GetServiceMetaDataRequest request);

	[OperationContract(Action = "http://www.sap.com/SBO/DIS/GetServiceMetaData", ReplyAction = "*")]
	Task<GetServiceMetaDataResponse> GetServiceMetaDataAsync(GetServiceMetaDataRequest request);

	[OperationContract(Action = "http://www.sap.com/SBO/DIS/BatchProcess", ReplyAction = "*")]
	BatchProcessResponse BatchProcess(BatchProcessRequest request);

	[OperationContract(Action = "http://www.sap.com/SBO/DIS/BatchProcess", ReplyAction = "*")]
	Task<BatchProcessResponse> BatchProcessAsync(BatchProcessRequest request);

	[OperationContract(Action = "http://www.sap.com/SBO/DIS/Process", ReplyAction = "*")]
	ProcessResponse Process(ProcessRequest request);

	[OperationContract(Action = "http://www.sap.com/SBO/DIS/Process", ReplyAction = "*")]
	Task<ProcessResponse> ProcessAsync(ProcessRequest request);

	[OperationContract(Action = "http://www.sap.com/SBO/DIS/AddObject", ReplyAction = "*")]
	AddObjectResponse AddObject(AddObjectRequest request);

	[OperationContract(Action = "http://www.sap.com/SBO/DIS/AddObject", ReplyAction = "*")]
	Task<AddObjectResponse> AddObjectAsync(AddObjectRequest request);

	[OperationContract(Action = "http://www.sap.com/SBO/DIS/UpdateObject", ReplyAction = "*")]
	UpdateObjectResponse UpdateObject(UpdateObjectRequest request);

	[OperationContract(Action = "http://www.sap.com/SBO/DIS/UpdateObject", ReplyAction = "*")]
	Task<UpdateObjectResponse> UpdateObjectAsync(UpdateObjectRequest request);

	[OperationContract(Action = "http://www.sap.com/SBO/DIS/GetByKey", ReplyAction = "*")]
	GetByKeyResponse GetByKey(GetByKeyRequest request);

	[OperationContract(Action = "http://www.sap.com/SBO/DIS/GetByKey", ReplyAction = "*")]
	Task<GetByKeyResponse> GetByKeyAsync(GetByKeyRequest request);

	[OperationContract(Action = "http://www.sap.com/SBO/DIS/Login", ReplyAction = "*")]
	LoginResponse Login(LoginRequest request);

	[OperationContract(Action = "http://www.sap.com/SBO/DIS/Login", ReplyAction = "*")]
	Task<LoginResponse> LoginAsync(LoginRequest request);

	[OperationContract(Action = "http://www.sap.com/SBO/DIS/LogOut", ReplyAction = "*")]
	LogOutResponse LogOut(LogOutRequest request);

	[OperationContract(Action = "http://www.sap.com/SBO/DIS/LogOut", ReplyAction = "*")]
	Task<LogOutResponse> LogOutAsync(LogOutRequest request);

	[OperationContract(Action = "http://www.sap.com/SBO/DIS/GetServiceStatus", ReplyAction = "*")]
	GetServiceStatusResponse GetServiceStatus(GetServiceStatusRequest request);

	[OperationContract(Action = "http://www.sap.com/SBO/DIS/GetServiceStatus", ReplyAction = "*")]
	Task<GetServiceStatusResponse> GetServiceStatusAsync(GetServiceStatusRequest request);
}
