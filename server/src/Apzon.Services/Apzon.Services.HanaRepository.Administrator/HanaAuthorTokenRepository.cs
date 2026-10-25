using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator;

public class HanaAuthorTokenRepository : AuthorTokenRepository
{
	public HanaAuthorTokenRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetAuthorizedEmp()
	{
		return "declare checkExist int;\r\nDELETE FROM APZ_HET1 WHERE \"EmpId\" = @EmpId AND \"UserId\" = @UserId;\r\n            INSERT INTO APZ_HET1(\"EmpId\", \"UserId\") VALUES(@EmpId, @UserId);\r\n            select IFNULL((select Count(*) from APZ_OHET where \"EmpId\" = @EmpId),0) into checkExist from DUMMY;\r\n            IF(checkExist > 0)\r\n            THEN INSERT INTO APZ_OHET (\"EmpId\", \"TokenStr\") VALUES(@EmpId, '');\r\n            END IF; ";
	}
}
