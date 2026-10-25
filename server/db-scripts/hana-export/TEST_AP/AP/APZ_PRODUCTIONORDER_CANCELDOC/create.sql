
create procedure APZ_ProductionOrder_CancelDoc(in entry int default 0)

As
Begin
DECLARE error INT =0;
	DECLARE error_message NVARCHAR(254)='';
	DECLARE error_prefix INT=5300000;
	Declare isqty int;
SELECT count(1) into isqty FROM owor WHERE "DocEntry"=:entry AND (Ifnull("CmpltQty",0)<>0 OR Ifnull("RjctQty",0)<>0);
	IF (ifnull(isqty,0)=1)
	then
		select :error_prefix+1 into error from dummy;
		 select  N'Action is valid for planned or released production orders with no transactions' into error_message from dummy;
		SELECT :error,:error_message from dummy;
		RETURN;
	END If;
	UPDATE owor SET "Status"='C' WHERE "DocEntry"=:entry;
	-- ++++++++++++ COMMIT +++++++++++++++

		UPDATE a 
			SET a."OnOrder"=a."OnOrder"- CASE WHEN b."Type" IN ('S','P') THEN b."PlannedQty" ELSE 0 END,
				a."IsCommited"=a."IsCommited" - CASE WHEN b."Type" IN ('D') THEN b."PlannedQty" ELSE 0 END 
		FROM oitw a INNER JOIN owor b ON a."ItemCode"=b."ItemCode" AND a."WhsCode"=b."Warehouse"
		WHERE b."DocEntry"=:entry;

		UPDATE a 
			SET a."OnOrder"=a."OnOrder"- CASE WHEN b."Type" IN ('S','P') THEN b."PlannedQty" ELSE 0 END,
				a."IsCommited"=a."IsCommited" - CASE WHEN b."Type" IN ('D') THEN b."PlannedQty" ELSE 0 END 
		FROM oitm a INNER JOIN owor b ON a."ItemCode"=b."ItemCode"
		WHERE b."DocEntry"=:entry;

		-- ++++++++++++ ORDER +++++++++++++++

		UPDATE a
		SET a."IsCommited"=
			a."IsCommited" - CASE WHEN  c."Type" IN ('S','P') THEN Ifnull((SELECT  SUM(Ifnull(T1."PlannedQty",0)) 
						FROM WOR1 T1  
						WHERE T1."DocEntry"=:entry),0) ELSE 0 END,
			a."OnOrder"=
			a."OnOrder" - CASE WHEN  c."Type" IN ('D') THEN Ifnull((SELECT  SUM(Ifnull(T1."PlannedQty",0)) 
						FROM WOR1 T1  
						WHERE T1."DocEntry"=:entry),0) ELSE 0 END
                       
		FROM OITW a 
		INNER JOIN WOR1 b ON a."ItemCode"=b."ItemCode" AND a."WhsCode"=b."wareHouse"
		INNER JOIN OWOR c ON b."DocEntry"=b."DocEntry"
		WHERE b."DocEntry"=:entry;

		

		UPDATE a
		SET a."IsCommited"=
			a."IsCommited" - CASE WHEN  c."Type" IN ('S','P') THEN Ifnull((SELECT  SUM(Ifnull(T1."PlannedQty",0)) 
						FROM WOR1 T1  
						WHERE T1."DocEntry"=:entry),0) ELSE 0 END,
			a."OnOrder"=
			a."OnOrder" - CASE WHEN  c."Type" IN ('D') THEN Ifnull((SELECT  SUM(Ifnull(T1."PlannedQty",0)) 
						FROM WOR1 T1  
						WHERE T1."DocEntry"=:entry),0) ELSE 0 END
		FROM OITM a 
		INNER JOIN WOR1 b ON a."ItemCode"=b."ItemCode"
		INNER JOIN OWOR c ON b."DocEntry"=b."DocEntry"
		WHERE b."DocEntry"=:entry;
End;




