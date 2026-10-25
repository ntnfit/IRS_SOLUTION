using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator;

public class HanaUsersRepository : UsersRepository
{
	public HanaUsersRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetUserDefaultGroupQuery()
	{
		return "SELECT \"Code\",\"Name\",\"Warehouse\",\"SalePerson\"\r\n                                  ,\"ICTCard\",\"CashAcct\",\"CheckAcct\",\"CreditCard\"\r\n                                  ,\"PrintRcpt\",\t\"ShortRcpt\",\"Color\"\r\n                                  ,\"Address\",\t\t\"Country\",\t\"PrintHeadr\"\r\n                                  ,\"Phone1\",\t\t\"Phone2\",\t\"Fax\"\r\n                                  ,\"E_Mail\",\t\t\"FrgnAddr\",\t\"FrnPrntHdr\"\r\n                                  ,\"FrgnPhone1\",\t\"FrgnPhone2\",\t\t\t\t\"FrgnFax\"\r\n                                  ,\"DflTaxCode\",\t\"FreeZoneNo\",\t\t\t\t\"UserSign\"\r\n                                  ,\"free1\",\t\t\t\"UseTax\",\t\t\t\t\t\"AdrsFromWh\"\r\n                                  ,\"Language\",\t\t\"Font\",\t\t                \"FontSize\"\r\n                                  ,\"BPLId\",\t\t\t\"AssetInDoc\",\"AttachPath\",\"DflPTICode\" \r\n                             FROM OUDG\r\n                              WHERE \"Code\"=(SELECT TOP 1 \"DfltsGroup\" FROM ousr WHERE USER_CODE=UserName);";
	}

	protected override string GetUserLoginQuery()
	{
		return "select top 1 a.\"USERID\",ifnull( a.USER_CODE,'') as USER_CODE , ifnull(a.U_NAME,'') as U_NAME, ifnull(a.\"Department\",0)as \"Department\", ifnull(b.\"Name\",'') as \"Name\"\r\n                    ,ifnull(a.\"Branch\",-1) as \"Branch\", ifnull(c.\"Name\",'') as \"Name\" ,ifnull (a.\"userSign\",0) as \"UserSign\", ifnull( a.\"WallPaper\",'') as \"WallPaper\"\r\n                    , ifnull( a.\"WllPprDsp\",0) as \"WllPprDsp\"\r\n                             from OUSR a left join  OUDP b ON a.\"Department\"=b.\"Code\"\r\n                             left join OUBR c ON c.\"Code\"=a.\"Branch\" where a.USER_CODE = Usercode;";
	}

	protected override string GetUserSignQuery()
	{
		return "select IFNULL(\"userSign\",-1) as \"UserSign\",  IFNULL(USERID,-1) as USERID  from OUSR Where USER_CODE = Usercode;";
	}

	protected override string GetShowUsersQuery()
	{
		return "select  a.USERID,a.PASSWORD, a.USER_CODE,a.U_NAME, IFNULL(a.INTERNAL_K,0) as INTERNAL_K ,\r\n\t                                IFNULL(a.GROUPS,0) as GROUPS, \r\n\t\t\t\t\t\t\t\t\ta.SUPERUSER, \r\n\t\t\t\t\t\t\t\t\tIFNULL(a.\"E_Mail\",'') as \"E_Mail\"\r\n\t\t\t\t\t\t\t\t\t,IFNULL(a.\"PortNum\",'') as \"PortNum\",\r\n\t                                IFNULL(a.\"Fax\",'') as \"Fax\",\r\n\t\t\t\t\t\t\t\t\t IFNULL(a.\"Locked\",'N') as \"Locked\",\r\n\t\t\t\t\t\t\t\t\t IFNULL(a.\"Department\",0) as \"Department\",\r\n\t\t\t\t\t\t\t\t\t IFNULL(a.\"Branch\",0) as \"Branch\",\r\n\t\t\t\t\t\t\t\t\t  IFNULL (a.\"userSign\",3) as \"userSign\",  IFNULL(a.\"createDate\",current_date) \"createDate\",\r\n\t                                 IFNULL (a.\"userSign2\",0) as \"userSign2\",\r\n\t\t\t\t\t\t\t\t\t a.\"updateDate\",a.\"lastLogin\"\r\n\t\t\t\t\t\t\t\t\t ,IFNULL(b.\"Name\",'') as \"Name\",\r\n\t\t\t\t\t\t\t\t\t IFNULL( c.\"Name\",'') as \"NameBranch\"\r\n                                from OUSR a                          \r\n\t\t\t\t\t\t\t\tleft join OUDP b on b.\"Code\" = a.\"Department\"\r\n                                left join  OUBR c on c.\"Code\" = a.\"Branch\" Where a.GROUPS != 99;";
	}

	protected override string GetEmployeeQuery()
	{
		return "select \"lastName\" || ',' || \"firstName\" \"Name\",\"empID\" \"Code\" from ohem;";
	}

	protected override string GetAnUserQuery()
	{
		return "Select a.*,(select \"empID\" from OHEM t where t.\"userId\" = a.userid) \"empID\" from OUSR a where a.userid=@userId_;";
	}

	protected override string GetLstUserNameByCodeQuery()
	{
		return "Select  string_agg(cast(USER_CODE as nvarchar),';') || ';' FROM OUSR WHERE cast(USERID as nvarchar) in (select data from split(@lstCode, ';'));";
	}

	protected override string GetUserByCodeQuery()
	{
		return "declare currentDate timestamp;\r\n\t                 select (select current_date from dummy) into currentDate from dummy;\r\n\t                 select a.USERID, a.PASSWORD, a.USER_CODE,a.U_NAME, IFNULL( a.INTERNAL_K, 0) as INTERNAL_K,\r\n\t                 IFNULL\t(a.GROUPS, 0) as GROUPS, a.SUPERUSER, \r\n\t                 IFNULL( a.\"E_Mail\", '') as \"E_Mail\",\r\n\t                 IFNULL( a.\"PortNum\", '') as \"PortNum\",\r\n\t                 IFNULL(a.\"Fax\", '') as \"Fax\",\r\n\t                 IFNULL(a.\"Locked\", 'N') as \"Locked\",\r\n\t                 IFNULL(a.\"Department\", 0) as \"Department\",\r\n\t                 IFNULL(a.\"Branch\", 0) as \"Branch\",\r\n\t                 IFNULL(a.\"userSign\", 3) as \"userSign\",  \r\n\t                 IFNULL(a.\"createDate\", currentDate) as \"createDate\",\r\n\t                 IFNULL(a.\"userSign2\", 0) as \"userSign2\",\r\n\t                 IFNULL(b.\"Name\", '') as \"Name\",\r\n\t                 IFNULL(c.\"Name\", '') as \"NameBranch\", a.\"updateDate\", a.\"lastLogin\"\r\n                     from OUSR a                          \r\n\t                 left join OUDP b on b.\"Code\" = a.\"Department\"\r\n                     left join OUBR c on c.\"Code\" = a.\"Branch\"\r\n                     where a.USER_CODE = @Usercode;";
	}

	protected override string GetSearchQuery()
	{
		return "select a.USERID, a.PASSWORD, a.USER_CODE, a.U_NAME, IFNULL(a.INTERNAL_K, 0) AS \"INTERNAL_K\", IFNULL(a.GROUPS, 0) AS \"GROUPS\", \r\n                    a.SUPERUSER, IFNULL(a.\"E_Mail\", '') AS \"E_Mail\", IFNULL(a.\"PortNum\", '') AS \"PortNum\", IFNULL(a.\"Fax\", '') AS \"Fax\", a.\"Locked\",\r\n                    a.\"Department\", a.\"Branch\", IFNULL(a.\"userSign\", 3) AS \"userSign\", IFNULL(a.\"createDate\", CURRENT_DATE) AS \"createDate\", \r\n                    IFNULL(a.\"userSign2\", 0) AS \"userSign2\", a.\"updateDate\", a.\"lastLogin\", IFNULL(b.\"Name\", '') AS \"Name\", IFNULL(c.\"Name\", '') AS \"NameBranch\"\r\n                    from OUSR a\r\n                    left join OUDP b on b.\"Code\" = a.\"Department\"\r\n                    left join OUBR c on c.\"Code\" = a.\"Branch\"\r\n                    where 1 = 1 and a.GROUPS <> 99;";
	}
}
