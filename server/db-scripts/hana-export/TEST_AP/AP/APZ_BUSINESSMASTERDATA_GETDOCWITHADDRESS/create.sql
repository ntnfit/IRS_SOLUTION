
CREATE PROCEDURE APZ_BusinessMasterData_GetDocWithAddress (
IN CardCode nvarchar(50) default '', 
IN UserCode nvarchar(50) default '', 
IN BPLId integer default 0) AS 
BEGIN 
	SELECT TOP 1 IFNULL(a."CardCode", '') AS "CardCode", 
			IFNULL("CardName", '') AS "CardName", IFNULL("CardFName", '') AS "CardFName", 
			IFNULL("CardType", '') AS "CardType", IFNULL("GroupCode", 0) AS "GroupCode", 
			IFNULL("Currency", '') AS "Currency", IFNULL(a."LicTradNum", '') AS "LicTradNum", 
			IFNULL("Balance", 0) AS "Balance", IFNULL("DNotesBal", 0) AS "DNotesBal", 
			IFNULL("OrdersBal", 0) AS "OrdersBal", IFNULL("Phone1", '') AS "Phone1", 
			IFNULL("Phone2", '') AS "Phone2", IFNULL("Cellular", '') AS "Cellular", 
			IFNULL("Fax", '') AS "Fax", IFNULL("E_Mail", '') AS "E_Mail", 
			IFNULL("IntrntSite", '') AS "IntrntSite", IFNULL("ShipType", 0) AS "ShipType", 
			IFNULL("Password", '') AS "Password", IFNULL("validFor", '') AS "validFor", 
			IFNULL("CntctPrsn", '') AS "CntctPrsn", IFNULL("AddID", '') AS "AddID", 
			IFNULL("VatIdUnCmp", '') AS "VatIdUnCmp", IFNULL("Notes", '') AS "Notes", 
			IFNULL("SlpCode", 0) AS "SlpCode", IFNULL('', '') AS "BillTo", 
			IFNULL('', '') AS "ShipTo", IFNULL("GroupNum", 0) AS "GroupNum", 
			IFNULL("IntrstRate", 0) AS "IntrstRate", IFNULL("ListNum", 0) AS "ListNum", 
			IFNULL("Discount", 0) AS "Discount", IFNULL("BankCode", '') AS "BankCode", 
			IFNULL('', '') AS "BankName", IFNULL(a."DflBranch", '') AS "DflBranch", 
			IFNULL(a."DflAccount", '') AS "DflAccount", IFNULL('', '') AS "AcctName", 
			IFNULL("CreditCard", 0) AS "CreditCard", IFNULL("CrCardNum", '') AS "CrCardNum", 
			IFNULL("CardValid", '') AS "CardValid", IFNULL("OwnerIdNum", '') AS "OwnerIdNum", 
			IFNULL("AvrageLate", 0) AS "AvrageLate", IFNULL("Free_Text", '') AS "Free_Text", 
			IFNULL(a."CreateDate", '') AS "CreateDate", IFNULL(a."UpdateDate", '') AS "UpdateDate", 
			IFNULL(a."UserSign", 0) AS "UserSign", IFNULL("UserSign2", 0) AS "UserSign2", 
			IFNULL(a."ObjType", '') AS "ObjType", IFNULL("Deleted", '') AS "Deleted", 
			IFNULL("DocEntry", 0) AS "DocEntry", IFNULL('', '') AS "PicName", 
			IFNULL("ShipToDef", '') AS "ShipToDef", IFNULL("BillToDef", '') AS "BillToDef", 
			IFNULL('', '') AS "UType", IFNULL("Series", 0) AS "Series", 
			IFNULL(b."Street", '') || ' ' || IFNULL(b."City", '') || ' ' || IFNULL(b."Country", '') AS "Address" 
	FROM ocrd a LEFT JOIN crd1 b ON a."CardCode" = b."CardCode" WHERE a."CardCode" = :CardCode;

END;




