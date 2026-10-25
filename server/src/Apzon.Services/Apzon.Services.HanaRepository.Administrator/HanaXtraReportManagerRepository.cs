using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator;

public class HanaXtraReportManagerRepository : XtraReportManagerRepository
{
	public HanaXtraReportManagerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetQuerySaveReport(bool insert)
	{
		if (insert)
		{
			return "INSERT INTO APZ_RDOC (\"TypeCode\", \"TypeName\", \"DocCode\", \"DocName\", \"Menu\", \"CreateDate\", \"UserSign\", \"Status\", \"Description\", \"Text\")\r\nVALUES (@TypeCode,@TypeName,@DocCode,@DocName,@Menu,@CreateDate,@UserSign,@Status,@Description,@Text);\r\nSELECT CURRENT_IDENTITY_VALUE() FROM DUMMY;";
		}
		return "UPDATE APZ_RDOC SET \"TypeCode\" = @TypeCode,\"TypeName\" = @TypeName,\"DocCode\" = @DocCode,\"Menu\" = @Menu,\"UpdateDate\" = @UpdateDate,\"UserSign2\" =  @UserSign2,\"Status\" = @Status,\"Description\" = @Description,\"Text\" = @Text\r\nWHERE ID =@ID;\r\nSELECT @ID FROM DUMMY;";
	}
}
