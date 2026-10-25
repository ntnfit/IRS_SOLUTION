using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaUserDefinedTextsRepository : UserDefinedTextsRepository
{
	public HanaUserDefinedTextsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetSaveQuery()
	{
		return "declare maxId integer;\r\n                    select \"AutoKey\" into maxId From ONNM Where \"ObjectCode\"=@ObjectCode;\r\n                                INSERT INTO OPDT(\"AbsEntry\",\"TextCode\",\"Text\") VALUES (maxId,@TextCodeValue,@TextValue);\r\n                                UPDATE ONNM SET \"AutoKey\"=maxId+1 WHERE \"ObjectCode\"=@ObjectCode;";
	}
}
