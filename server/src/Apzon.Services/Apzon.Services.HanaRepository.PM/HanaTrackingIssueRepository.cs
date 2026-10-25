using Apzon.Services.Repository.PM;

namespace Apzon.Services.HanaRepository.PM;

public class HanaTrackingIssueRepository : TrackingIssueRepository
{
	public HanaTrackingIssueRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetDataTypeByIdQuery()
	{
		return "select * from APZ_IETE where \"Code\"=@Id;";
	}

	protected override string GetDataSearchByUserIdQuery()
	{
		return "select * from APZ_OSPH where \"UserCode\"=userId and \"FormId\"=@formId;";
	}

	protected override string LoadIssueByCallIdQuery()
	{
		return "select * from APZ_TGIE where \"ServiceCall\"=@callId;";
	}

	protected override string GetUseHandleByIdQuery()
	{
		return "select top 1 USER_CODE from OUSR a INNER JOIN OHEM b ON b.\"userId\" = a.\"USERID\" \r\n                                                                  INNER JOIN (select \"HandleBy\" from APZ_TGIE where \"Id\" = id) c ON b.\"empID\" = c.\"HandleBy\";";
	}

	protected override string GetUseReportByIdQuery()
	{
		return "select USER_CODE from OUSR a INNER JOIN OHEM b ON b.\"userId\" = a.USERID \r\n                                                                  INNER JOIN (select \"Reporter\" from APZ_TGIE where \"Id\" = id) c ON b.\"empID\" = c.\"Reporter\";";
	}

	protected override string GetLstUserNameByIssueIdQuery()
	{
		return "Select  string_agg(cast(a.USER_CODE as nvarchar),';') || ';' FROM OUSR a INNER JOIN OHEM b ON a.\"USERID\" = b.\"userId\" \r\n                        where exists(select 1 from APZ_TGIE1 x where \"Id\" = @id and \"FieldName\" = 'Handle by' and b.\"empID\" = cast(x.\"OldVal\" as int));";
	}

	protected override string GetDataStatusByIdQuery()
	{
		return "select * from APZ_OTIS where (ifnull(@Id,'') ='' or \"Code\"=@Id);";
	}
}
