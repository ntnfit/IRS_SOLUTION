
CREATE  PROCEDURE apz_picking_beforeaction(
	in obj_id VARCHAR(50),
	in transtype CHAR(1))
AS
BEGIN
	-------------------------------------------------- VARIABLE ---------------------------------------------
	DECLARE error INT =0;
	DECLARE error_message NVARCHAR(254)='';
	DECLARE error_prefix INT=6100000;
	DECLARE DocEntry INT= cast(:obj_id as int);
	declare fromDate timestamp;
	declare toDate timestamp;
	DECLARE itm VARCHAR(50);
	declare ItemCode nvarchar(50)='';
	declare OrderNum int=0;
	DECLARE maxLineNum INT = 0;
	-------------------------------------------------- END VARIABLE ----------------------------------------
		
	select ADD_DAYS(current_date, -1) into fromDate FROM DUMMY;
	select ADD_DAYS(current_date, 1) into toDate from DUMMY;
	
	--------------------------------------------------- TEMP TABLE -----------------------------------------
	create local temporary table #APZ_PKL1Data as (select top 0 * from APZ_PKL1);
	exec 'drop table #APZ_PKL1Data;';
	exec 'create local temporary table #APZ_PKL1Data as (select * from "#APZ_PKL1");';
	--------------------------------------------------- END TEMP TABLE -------------------------------------
	
	select ifNULL((SELECT TOP 1 T0."ItemCode" FROM #APZ_PKL1Data T0 LEFT JOIN OITM T1 ON T0."ItemCode"=T1."ItemCode" WHERE T1."ItemCode" IS NULL ORDER BY T0."PickEntry" ASC),'') into itm from DUMMY;
	IF :itm<>''
	THEN
		error=:error_prefix+7;
		error_message='Item ''' || :itm || ''' does not exists';
		SELECT :error, :error_message err_msg from dummy;
		RETURN;
	ELSE
 
		SELECT IFNULL((SELECT TOP 1 T0."ItemCode" FROM #APZ_PKL1Data T0 WHERE T0."ItemCode" NOT IN 
		(SELECT A."ItemCode" FROM #APZ_PKL1Data T1 INNER JOIN OITM  A ON T1."ItemCode"=A."ItemCode"  AND ((A."validFor" = 'Y' and ifnull(A."frozenFor",'N') = 'N' and (ifnull(a."validFrom", :fromDate) <= CURRENT_DATE
			 and ifnull(a."validTo", :toDate) >= current_date)) 
		OR (ifnull(A."validFor",'N') = 'N' and A."frozenFor" = 'Y' and (ifnull(a."frozenFrom", :fromDate) > current_date or ifnull(a."frozenTo", :toDate) < current_date))
		or (A."validFor" = 'Y' and A."frozenFor" = 'Y' and (ifnull(a."validFrom", :fromDate) <= current_date and ifnull(a."validTo", :toDate) >= current_date) and
					(ifnull(a."frozenFrom", :fromDate) > current_date or ifnull(a."frozenTo", :toDate) < current_date))))),'') into itm from DUMMY;
	    IF :itm<>''
		then
			error=:error_prefix+8;
			error_message='Item ''' || :itm || ''' is inactive';
			SELECT :error,:error_message err_msg from dummy;
			RETURN;
		END IF;
	end IF;

	select (select T0."ItemCode" FROM #APZ_PKL1Data T0 INNER JOIN ORDR T1 ON T0."OrderEntry"=T1."DocNum" 
			left JOIN RDR1 T2 ON T0."ItemCode"=T2."ItemCode" and T1."DocEntry"=T2."DocEntry" where T2."LineNum" is null) into ItemCode from DUMMY;
			
	select (select T0."OrderEntry" FROM #APZ_PKL1Data T0 INNER JOIN ORDR T1 ON T0."OrderEntry"=T1."DocNum" 
			left JOIN RDR1 T2 ON T0."ItemCode"=T2."ItemCode" and T1."DocEntry"=T2."DocEntry" where T2."LineNum" is null) into OrderNum from DUMMY;

	if :ItemCode<>''
	THEN
		error= :error_prefix+8;
		error_message='No matching record found. Order:' || cast(:OrderNum as nvarchar(11)) || '. Item:' || :ItemCode;
		SELECT :error,:error_message err_msg from dummy;
		RETURN;
	END IF;
    
	UPDATE T1
	SET T1."OrderLine"=T0."LineNum" from RDR1 T0 INNER JOIN #APZ_PKL1Data T1 ON T0."ItemCode"=T1."ItemCode"
									INNER JOIN ORDR T2 ON T1."OrderEntry"=T2."DocNum" AND T0."DocEntry"=T2."DocEntry"  WHERE T1."OrderLine" IS null;
	UPDATE T1
	SET  T1."WhsCode"=T0."WhsCode" from RDR1 T0 INNER JOIN #APZ_PKL1Data T1 ON T0."ItemCode"=T1."ItemCode" and T1."OrderLine"=T0."LineNum"
									INNER JOIN ORDR T2 ON T1."OrderEntry"=T2."DocNum" AND T0."DocEntry"=T2."DocEntry";
	--SELECT T0.ItemCode,t0.MdAbsEntry,SUM(T0.Quantity) Qty INTO #picked FROM APZ_PKL2 T0 INNER JOIN APZ_PKL1 T1 ON T0.AbsEntry=T1.AbsEntry WHERE T1.OrderEntry=@DocEntry AND T1.OrderLine=@LineNum AND T1.ItemCode=@ItemCode AND T1.WhsCode=@WhsCode GROUP BY t0.ItemCode,T0.MdAbsEntry 
 
 	create local temporary table #salebom as(SELECT T0."AbsEntry",T0."PickEntry",T2."VisOrder",T0."OrderEntry",T2."InvQty",T0."PickQtty" FROM  #APZ_PKL1Data T0
		INNER JOIN ORDR T1 ON T0."OrderEntry"=T1."DocNum"
		INNER JOIN RDR1 T2 ON T1."DocEntry"=T2."DocEntry" AND T0."OrderLine"=T2."LineNum"
		 WHERE T0."ItemCode" IN (SELECT DISTINCT "Code" FROM oitt WHERE "TreeType"='S'));
	
	
	select ifnull((SELECT MAX("PickEntry") FROM #apz_pkl1Data),0) into maxLineNum from DUMMY;

	INSERT INTO #APZ_PKL1Data ("AbsEntry", "PickEntry", "OrderEntry", "OrderLine", "CardCode", "CardName", "ItemCode", "ItemName", "WhsCode", "PickQtty", "PickStatus")
		SELECT T0."AbsEntry", ROW_NUMBER() OVER ( ORDER BY A."VisOrder") + :maxLineNum, B."DocNum", A."LineNum", B."CardCode", B."CardName", A."ItemCode", A."Dscription", A."WhsCode", "PickQtty" * (A."InvQty" / T0."InvQty"),
			 'O' FROM RDR1 A INNER JOIN ORDR B ON A."DocEntry" = b."DocEntry" INNER JOIN #salebom T0 ON B."DocNum" = T0."OrderEntry" 
			 WHERE A."TreeType" = 'I' AND A."VisOrder" BETWEEN (SELECT MIN("VisOrder") FROM rdr1 WHERE "DocEntry" = B."DocEntry" AND "TreeType" = 'I' AND "VisOrder" > T0."VisOrder") 
			 AND ifnull((SELECT MAX("VisOrder") FROM rdr1 WHERE "DocEntry" = B."DocEntry" AND "TreeType" <> 'I' AND "VisOrder" > T0."VisOrder"), (SELECT MAX("VisOrder") 
			 FROM RDR1 WHERE "DocEntry" = A."DocEntry"));
	
	DELETE from #APZ_PKL1Data WHERE "PickEntry" IN (SELECT "PickEntry" FROM #salebom);
	
	exec 'delete from "#APZ_PKL1";';
	exec 'insert into "#APZ_PKL1" select * from #APZ_PKL1Data;';

	SELECT :error,:error_message from DUMMY;

END;



