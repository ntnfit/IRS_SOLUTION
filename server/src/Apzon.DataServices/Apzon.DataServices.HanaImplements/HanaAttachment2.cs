using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaAttachment2 : Attachment2
{
	public HanaAttachment2(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public override HttpResult Add(List<AttachmentFile_Impl> lstFile, string sesstionId)
	{
		try
		{
			string jsonObject = Mapping(lstFile);
			HttpResult ret = DataService.ProcessJson(ApzObjectTypes.oAttachments2, sesstionId, jsonObject);
			if (ret.MessageCode == Enums.MessageCode.Success)
			{
				Task.Run(() =>
				{
					ServiceUnitOfWork.Files.UpdateTargetPath(lstFile, Function.ParseInt(ret.Content));
				});
			}
			return ret;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult Update(int absEntry, List<AttachmentFile_Impl> lstFile, string sesstionId)
	{
		string jsonObject = Mapping(lstFile);
		HttpResult httpResult = DataService.ProcessJson(ApzObjectTypes.oAttachments2, sesstionId, jsonObject, Enums.HttpAction.Update, absEntry);
		if (httpResult.MessageCode == Enums.MessageCode.Success)
		{
			Task.Run(() =>
			{
				ServiceUnitOfWork.Files.UpdateTargetPath(lstFile, absEntry);
			});
		}
		return httpResult;
	}

	protected override string Mapping(List<AttachmentFile_Impl> lstFiles)
	{
		try
		{
			string text = "{ Attachments2_Lines: [";
			foreach (AttachmentFile_Impl lstFile in lstFiles)
			{
				text += "{";
				text += Function.GetJsonString("FileExtension", lstFile.FileExt);
				text += Function.GetJsonString("FileName", lstFile.FileName);
				text += Function.GetJsonString("SourcePath", lstFile.srcPath);
				text += Function.GetJsonString("Override", lstFile.Override);
				text += Function.GetJsonString("FreeText", lstFile.FreeText);
				text += DataService.MappingUdf(lstFile.UserFields);
				text += "},";
			}
			return text + "] }";
		}
		catch (Exception)
		{
			throw;
		}
	}
}
