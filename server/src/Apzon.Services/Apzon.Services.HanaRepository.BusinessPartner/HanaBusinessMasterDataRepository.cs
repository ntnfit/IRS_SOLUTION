using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.HanaRepository.BusinessPartner;

public class HanaBusinessMasterDataRepository : BusinessMasterDataRepository
{
	public HanaBusinessMasterDataRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetExistsTableQuery()
	{
		return $"DECLARE TableName VARCHAR(100) = Table;\r\n                     DECLARE Exists nvarchar(10) = 'false';\r\n                    select IFNULL((SELECT top 1 'true' FROM tables WHERE SCHEMA_NAME = CURRENT_SCHEMA and TABLE_NAME = TableName),'false') into Exists from DUMMY;\r\n                     select Exists from DUMMY;";
	}

	protected override string LoadCBPaymentRunBCACodeQueryString()
	{
		return "select '' \"Code\" ,'' \"Name\" FROM DUMMY\r\n                    union\r\n                    select* from OBCA;";
	}

	protected override string LoadComboTechnicianQueryString()
	{
		return "select a.\"empID\",\"lastName\" || ', ' || \"firstName\" AS \"Name\" \r\n                        from OHEM a \r\n                    inner join HEM6 b on a.\"empID\" = b.\"empID\" \r\n                    inner join OHTY c on b.\"roleID\"= c.\"typeID\" \r\n                    where c.\"typeID\"=-2;";
	}

	protected override string GetVatByCardCodeQuery()
	{
		return "Select ifnull(a.\"ECVatGroup\",'') as \"VatGroup\",  ifnull(b.\"Rate\",0) as \"Rate\" from OCRD a\r\n                            left join ovtg b on a.\"ECVatGroup\"=b.\"Code\" where \"CardCode\"=cardCode;";
	}

	protected override string GetBPPropertiesQuery()
	{
		return "SELECT 'N' AS \"QryGroup1\", 'N' AS \"QryGroup2\", 'N' AS \"QryGroup3\", 'N' AS \"QryGroup4\", 'N' AS \"QryGroup5\", 'N' AS \"QryGroup6\", 'N' AS \"QryGroup7\", 'N' AS \"QryGroup8\", 'N' AS \"QryGroup9\", 'N' AS \"QryGroup10\", 'N' AS \"QryGroup11\", 'N' AS \"QryGroup12\", 'N' AS \"QryGroup13\", 'N' AS \"QryGroup14\", 'N' AS \"QryGroup15\", 'N' AS \"QryGroup16\", 'N' AS \"QryGroup17\", 'N' AS \"QryGroup18\", 'N' AS \"QryGroup19\", 'N' AS \"QryGroup20\", 'N' AS \"QryGroup21\", 'N' AS \"QryGroup22\", 'N' AS \"QryGroup23\", 'N' AS \"QryGroup24\", 'N' AS \"QryGroup25\", 'N' AS \"QryGroup26\", 'N' AS \"QryGroup27\", 'N' AS \"QryGroup28\", 'N' AS \"QryGroup29\", 'N' AS \"QryGroup30\", 'N' AS \"QryGroup31\", 'N' AS \"QryGroup32\", 'N' AS \"QryGroup33\", 'N' AS \"QryGroup34\", 'N' AS \"QryGroup35\", 'N' AS \"QryGroup36\", 'N' AS \"QryGroup37\", 'N' AS \"QryGroup38\", 'N' AS \"QryGroup39\", 'N' AS \"QryGroup40\", 'N' AS \"QryGroup41\", 'N' AS \"QryGroup42\", 'N' AS \"QryGroup43\", 'N' AS \"QryGroup44\", 'N' AS \"QryGroup45\", 'N' AS \"QryGroup46\", 'N' AS \"QryGroup47\", 'N' AS \"QryGroup48\", 'N' AS \"QryGroup49\", 'N' AS \"QryGroup50\", 'N' AS \"QryGroup51\", 'N' AS \"QryGroup52\", 'N' AS \"QryGroup53\", 'N' AS \"QryGroup54\", 'N' AS \"QryGroup55\", 'N' AS \"QryGroup56\", 'N' AS \"QryGroup57\", 'N' AS \"QryGroup58\", 'N' AS \"QryGroup59\", 'N' AS \"QryGroup60\", 'N' AS \"QryGroup61\", 'N' AS \"QryGroup62\", 'N' AS \"QryGroup63\", 'N' AS \"QryGroup64\" FROM DUMMY;\r\n\r\n                    SELECT \"GroupName\" FROM OCQG ORDER BY \"GroupCode\" ASC;";
	}

	protected override string GetDefaultLanguageQuery()
	{
		return "SELECT a.\"DefLang\" FROM SBOCOMMON.SLSD a WHERE \"Name\" = (SELECT b.LOC FROM SBOCOMMON.SRGC b WHERE b.\"dbName\" = CURRENT_SCHEMA); ";
	}

	protected override string GetAccountTabNameQuery()
	{
		return "SELECT(SELECT \"CardName\" FROM OCRD WHERE \"CardCode\" = @fatherCard) AS \"FatherCard\", (SELECT \"AcctName\" FROM OACT WHERE \"AcctCode\" = @debPayAcct) AS \"DebPayAcct\", (SELECT \"AcctName\" FROM OACT WHERE \"AcctCode\" = @dpmClear) AS \"DpmClear\", (SELECT \"AcctName\" FROM OACT WHERE \"AcctCode\" = @dpmIntAct) AS \"DpmIntAct\", (SELECT \"CardName\" FROM OCRD WHERE \"CardCode\" = @connBP) AS \"ConnBP\" FROM DUMMY; ";
	}

	protected override string GetEmployeeCodeBySlpCodeQuery()
	{
		return "Select \"empID\" as \"OwnerCode\",(\"firstName\"||', '||\"lastName\") as \"OwnerName\" \r\n            from OHEM where \"salesPrson\"=";
	}

	protected override string ExistFileInDocQuery()
	{
		return "SELECT \"FileName\" FROM atc1 WHERE \"AbsEntry\" = (SELECT TOP 1 \"AtcEntry\" FROM OCRD WHERE \"CardCode\" = @cardCode) AND \"FileExt\" = @fileExt AND \"FileName\" = @fileName AND \"AbsEntry\" IS NOT NULL; ";
	}

	protected override string LoadCBPaymentRunAccountQuery()
	{
		return "SELECT '' AS \"Account\", '' AS \"IBAN\", '' AS \"Branch\", '' AS \"SwiftNum\", '' AS \"ControlKey\" FROM DUMMY UNION ALL SELECT \"Account\", IBAN, \"Branch\", \"SwiftNum\", \"ControlKey\" FROM dsc1 WHERE \"BankCode\" = @bankCode AND \"Country\" = @cryCode;";
	}

	protected override string LoadCBPaymentRunBankQuery()
	{
		return "SELECT '' AS \"BankCode\", '' AS \"BankName\" FROM DUMMY UNION ALL SELECT \"BankCode\", \"BankName\" FROM ODSC WHERE \"CountryCod\" = @countryCode;";
	}

	protected override string GetSearchString(string condition)
	{
		return $"declare dofilter char(1);\r\n            DECLARE DoMngMth CHAR(1);\r\n            select IFNULL((select \"DoFilter\" from cinf),'N') into dofilter from DUMMY;\r\n\r\n            SELECT IFNULL((SELECT  \"DoMngMth\" FROM oadm ),'') into DoMngMth from DUMMY;\r\n\r\n            call apz_func_OwnerOverShip(@UserCode,2, OwnerView);\r\n        select \r\n            * \r\n            ,(select \"TrnspName\" from OSHP where \"TrnspCode\" = a.\"ShipType\") as \"Shipping Type Name\"\r\n            , (select \"PymntGroup\" from OCTG where \"GroupNum\" = a.\"GroupNum\") as \"Payment Terms\"\r\n            from ocrd a where 1 = 1 \r\n\t\t\t{condition}\r\n\t\t\tAND (dofilter='N' OR (dofilter='Y' AND DoMngMth NOT IN ('A','B')) OR (dofilter='Y' AND DoMngMth IN ('A','B') \r\n                AND\t(IFNULL(A.\"OwnerCode\",-1)=-1 OR exists( select 1 from $OwnerView y where \"OwnerId\" = A.\"OwnerCode\"))));";
	}

	protected override string GetSearchStringCompare(string colName)
	{
		return string.Format(" and a.\"{0}\" LIKE N'%'|| @{0} ||'%' ", colName);
	}
}
