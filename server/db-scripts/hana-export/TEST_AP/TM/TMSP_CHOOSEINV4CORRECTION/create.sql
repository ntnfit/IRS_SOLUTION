-- B1 DEPENDS: AFTER:PT:PROCESS_END


CREATE PROCEDURE  TmSp_ChooseINV4Correction(
IN card nvarchar(100),
IN Type1 nvarchar(2),
IN curr nvarchar(4))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
BEGIN
if (:Type1=N'I')
THEN
	if ( LENGTH(ltrim(rtrim(:curr)))>0 )
	THEN
                select "DocEntry", "DocNum",  "DocDate", "CardCode", "Comments", "DocDueDate"  from OINV
		
		WHERE (NOT EXISTS
				(select distinct("DocEntry") from INV1 
					where "DocEntry"=OINV."DocEntry" AND
						("Quantity"<=0 OR 
						"LineStatus"=N'C' OR
						"OpenCreQty"<>"Quantity") ))
			
	
			 AND "CANCELED"<>N'Y' 
			AND "CardCode" = :card 
			AND "DocType"=N'I'
			AND "DocCur"=:curr
		ORDER BY "DocNum";
	
	else
	
		select "DocEntry", "DocNum",  "DocDate", "CardCode", "Comments", "DocDueDate"  from OINV
		WHERE (NOT EXISTS
				(select distinct("DocEntry") from INV1 
					where "DocEntry"=OINV."DocEntry" AND
						("Quantity"<=0 OR 
						"LineStatus"=N'C' OR
						"OpenCreQty"<>"Quantity") ))
			
	
			 AND "CANCELED"<>N'Y' 
			AND "CardCode" = :card 
			AND "DocType"=N'I'
		ORDER BY "DocNum";
	end IF;

else

	if ( LENGTH(ltrim(rtrim(:curr)))>0 )
	THEN	
		select "DocEntry", "DocNum",  "DocDate", "CardCode", "Comments", "DocDueDate"  from OINV
		WHERE (NOT EXISTS
				(select distinct("DocEntry") from INV1 
					where "DocEntry"=OINV."DocEntry" AND
						"LineStatus"<>N'O'))
	
			 AND "CANCELED"<>N'Y' 
			AND "CardCode" = :card 
			AND "DocType"=N'S'
			AND "DocCur"=:curr
		ORDER BY "DocNum";
	
	else
	
		select "DocEntry", "DocNum",  "DocDate", "CardCode", "Comments", "DocDueDate"  from OINV
		WHERE (NOT EXISTS
				(select distinct("DocEntry") from INV1 
					where "DocEntry"=OINV."DocEntry" AND
						"LineStatus"<>N'O'))
	
			 AND "CANCELED"<>N'Y' 
			AND "CardCode" = :card 
			AND "DocType"=N'S'
		ORDER BY "DocNum";
	end IF;
end IF;
END;


