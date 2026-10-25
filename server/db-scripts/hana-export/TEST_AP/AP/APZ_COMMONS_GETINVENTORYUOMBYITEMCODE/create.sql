
CREATE PROCEDURE APZ_Commons_GetInventoryUoMByItemCode(in itemCode nvarchar(50), in typeUom int)
AS
BEGIN
	
	if :typeUom = 3
		then
			SELECT * FROM OITM a LEFT OUTER JOIN UGP1 b ON b."UgpEntry" = IFNULL(a."UgpEntry", -1) 
							INNER JOIN OUOM c ON b."UomEntry" = c."UomEntry" AND (b."UomEntry" = -1 OR a."InvntryUom" = c."UomName") WHERE "ItemCode" = :itemCode;		
	else 

		select 1 from dummy;

	end if;

END;




