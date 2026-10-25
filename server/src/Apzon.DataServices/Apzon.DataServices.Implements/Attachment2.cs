using System;
using System.Collections.Generic;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class Attachment2 : Repository<List<AttachmentFile_Impl>>, IAttachment2, IRepository<List<AttachmentFile_Impl>>, IDisposable
{
	public Attachment2(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public virtual HttpResult Add(List<AttachmentFile_Impl> lstFile, string sesstionId)
	{
		throw new NotImplementedException();
	}

	public void Dispose()
	{
		Dispose();
	}

	public virtual HttpResult Update(int docEntry, List<AttachmentFile_Impl> lstFile, string sesstionId)
	{
		throw new NotImplementedException();
	}

	protected virtual string Mapping(List<AttachmentFile_Impl> lstFiles)
	{
		return "";
	}
}
