
create procedure  APZ_SHIPPING_BEFOREACTION (in obj_id nvarchar(50), in transtype char(1))
as

begin

	DECLARE error INT =0;
	DECLARE error_message NVARCHAR(254)='';
	DECLARE error_prefix INT=6100000;
	DECLARE DocEntry INT=cast(:obj_id as int);

	create local temporary table #APZ_SHIP1_temp as (select top 0 * from APZ_SHIP1);
	execute immediate 'insert into #APZ_SHIP1_temp select * from #APZ_SHIP1;';
	
	-- update base document

	IF (:transtype='A') then
	
		IF (SELECT count(1) FROM #APZ_SHIP1_temp) > 0 then
		
			select :error_prefix + 1 into error from dummy; 
			select N'Content is missing' into error_message from dummy;
			SELECT :error, :error_message from dummy;
			return;
		END	IF;
		
		UPDATE #APZ_SHIP1_temp SET "OpenQty"="Quantity";
		UPDATE A
			SET A."OpenQty"= CASE WHEN a."Quantity" - T0."Quantity">0 THEN A."Quantity"-T0."Quantity" ELSE 0 END,
			"LineStatus"=CASE WHEN a."Quantity"- T0."Quantity"<=0 THEN 'C' ELSE A."LineStatus" end
		FROM APZ_PAK1 A INNER JOIN #APZ_SHIP1_temp T0 ON A."DocEntry"=T0."BaseEntry" AND T0."BaseType"=A."ObjType" AND A."LineNum"=T0."BaseRow";

		UPDATE APZ_OPAK  SET "DocStatus"='L' WHERE "DocEntry" IN (SELECT T0."BaseEntry" FROM #APZ_SHIP1_temp T0 WHERE T0."BaseType"=APZ_OPAK."ObjType" AND T0."BaseEntry"=APZ_OPAK."DocEntry")
		AND NOT EXISTS (SELECT 1 FROM apz_pak1 T0 WHERE t0."LineStatus"='O' AND T0."DocEntry"=APZ_OPAK."DocEntry"); 
		
		UPDATE T0 
		SET T0."OpenQty"=(CASE WHEN T0."Quantity"-T1."Quantity">0 THEN T0."Quantity"-T1."Quantity" ELSE 0 END),
		"PickStatus"=(CASE WHEN T0."Quantity" -T1."Quantity"<=0 THEN 'C' ELSE T0."PickStatus" END )
		FROM apz_pkl2 T0
		INNER JOIN #APZ_SHIP1_temp T1 ON T0."AbsEntry"=T1."BaseEntry" AND T1."BaseRow"=T0."PickEntry" AND T0."MdAbsEntry"=T1."BatchSerialEntry";
		
		UPDATE T0 
		SET T0."Status"='L'
		FROM APZ_OPKL T0
		WHERE "AbsEntry" IN (SELECT DISTINCT "BaseEntry" FROM #APZ_SHIP1_temp) and  NOT EXISTS (SELECT 1 FROM APZ_PKL2 A WHERE a."AbsEntry"=T0."AbsEntry" AND A."PickStatus"<>'C');
	END IF;
	
end;




