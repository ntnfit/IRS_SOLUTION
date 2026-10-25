-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE  TmSp_ChooseCIN4Correction(in card nvarchar(100),
in Type1 nvarchar(2),
in curr nvarchar(4))

LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
begin

if (:Type1 = N'I')
then
	if ( LENGTH(ltrim(rtrim(:curr)))>0 )
	then
		select "DocEntry", "DocNum",  "DocDate", "CardCode", "Comments", "DocDueDate"  from OCIN
		WHERE (NOT EXISTS
				(select distinct("DocEntry") from CIN1 
					where "DocEntry" = OCIN."DocEntry" AND
						("Quantity" <= 0 OR 
						"LineStatus"= N'C' OR
						"OpenCreQty"<>"Quantity") ))
			
	
			 AND "CANCELED"<>N'Y' 
			AND "CardCode" = :card 
			AND "DocType"=N'I'
			AND "DocCur"=:curr
		ORDER BY "DocNum";
	
	else
	
		select "DocEntry", "DocNum",  "DocDate", "CardCode", "Comments", "DocDueDate"  from OCIN
		WHERE (NOT EXISTS
				(select distinct("DocEntry") from CIN1 
					where "DocEntry"=OCIN."DocEntry" AND
						("Quantity"<=0 OR 
						"LineStatus"=N'C' OR
						"OpenCreQty"<>"Quantity") ))
			
	
			 AND "CANCELED"<>N'Y' 
			AND "CardCode" = :card 
			AND "DocType"=N'I'
		ORDER BY "DocNum";
	end if;


else

	if ( LENGTH(ltrim(rtrim(:curr)))>0 )
	then	
		select "DocEntry", "DocNum",  "DocDate", "CardCode", "Comments", "DocDueDate"  from OCIN
		WHERE (NOT EXISTS
				(select distinct("DocEntry") from CIN1 
					where "DocEntry"=OCIN."DocEntry" AND
						"LineStatus"<>N'O'))
	
			 AND "CANCELED"<>N'Y' 
			AND "CardCode" = :card 
			AND "DocType"=N'S'
			AND "DocCur"=:curr
		ORDER BY "DocNum";
	
	else
	
		select "DocEntry", "DocNum",  "DocDate", "CardCode", "Comments", "DocDueDate"  from OCIN
		WHERE (NOT EXISTS
				(select distinct("DocEntry") from CIN1 
					where "DocEntry"=OCIN."DocEntry" AND
						"LineStatus"<>N'O'))
	
			 AND "CANCELED"<>N'Y' 
			AND "CardCode" = :card 
			AND "DocType"=N'S'
		ORDER BY "DocNum";
	end if;
end if;
end;


