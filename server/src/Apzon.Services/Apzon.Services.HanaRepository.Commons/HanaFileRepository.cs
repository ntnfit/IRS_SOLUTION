using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaFileRepository : FileRepository
{
	public HanaFileRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public override string AddFile()
	{
		try
		{
			return Function.ToString(ExecuteScalar("insert into OATC (\"AbsEntry\") values ((select ifnull(max(\"AbsEntry\"),0)+1 From OATC));\r\n                       Update ONNM set \"AutoKey\"= ifnull((select max(\"AbsEntry\") + 1 from OATC),0) where \"ObjectCode\"='221';\r\n                       select ifnull((select max(\"AbsEntry\") from OATC),0) from DUMMY;", CommandType.Text));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC file oatc table FAILED ", ex.Message);
			return ex.Message;
		}
	}

	protected override string GetQueryUpdateTrgPath()
	{
		return "update ATC1 set \"trgtPath\"=@trgtPath where \"AbsEntry\"=@absEntry and cast(\"srcPath\" as nvarchar)=@srcPath and \"FileName\"=@fileName and \"FileExt\"=@fileExt;";
	}
}
