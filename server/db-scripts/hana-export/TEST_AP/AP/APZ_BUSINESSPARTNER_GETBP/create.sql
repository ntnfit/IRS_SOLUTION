
CREATE  PROCEDURE APZ_BusinessPartner_GetBp(
	in CardType NVARCHAR(10),
	in UserName NVARCHAR(50))
AS
BEGIN

	------------------------------------- GET OWNER VIEW ------------------------------------------
	call apz_func_OwnerOverShip(:UserName,2, OwnerView);
	IF :CardType<>''
	THEN
		SELECT A."CardCode", A."CardName", A."Balance", A."CntctPrsn", A."ShipType", A."BillToDef", A."ShipToDef", B."Name", D."Street" AS "MailAddress", E."Street" AS "Address", IFNULL(A."Currency", '##') AS "Currency"
				, IFNULL(A."Phone1", '') AS "Phone1", IFNULL(A."CardType", '') AS "CardType", IFNULL(A."SlpCode", 0) AS "SlpCode", A."OwnerCode" FROM OCRD A 
									LEFT OUTER JOIN OCPR B ON A."CardCode" = B."CardCode" AND A."CntctPrsn" = B."Name" 
									LEFT OUTER JOIN CRD1 D ON A."CardCode" = D."CardCode" AND A."ShipToDef" = D."Address" 
									LEFT OUTER JOIN CRD1 E ON A."CardCode" = E."CardCode" AND A."BillToDef" = E."Address"
		                                     WHERE (A."validFor"='Y' OR(a."validFor"='N' AND a."frozenFor"='N'))
											 AND "CardType"=:CardType AND	(IFNULL(A."OwnerCode",-1)=-1 OR (A."OwnerCode" IN (select distinct x."OwnerId" from :OwnerView x)));
	ELSE
		
		SELECT A."CardCode", A."CardName", A."Balance", A."CntctPrsn", A."ShipType", A."BillToDef", A."ShipToDef", B."Name", D."Street" AS "MailAddress", E."Street" AS "Address", IFNULL(A."Currency", '##') AS "Currency",
				 IFNULL(A."Phone1", '') AS "Phone1", IFNULL(A."CardType", '') AS "CardType", IFNULL(A."SlpCode", 0) AS "SlpCode" FROM OCRD A 
				 					LEFT OUTER JOIN OCPR B ON A."CardCode" = B."CardCode" AND A."CntctPrsn" = B."Name" 
				 					LEFT OUTER JOIN CRD1 D ON A."CardCode" = D."CardCode" AND A."ShipToDef" = D."Address" 
				 					LEFT OUTER JOIN CRD1 E ON A."CardCode" = E."CardCode" AND A."BillToDef" = E."Address"
		                                     WHERE (A."validFor"='Y' OR(a."validFor"='N' AND a."frozenFor"='N'))
											  AND	(IFNULL(A."OwnerCode",-1)=-1 OR (A."OwnerCode" IN (select distinct x."OwnerId" from :OwnerView x)));
	END IF;

END;



