-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables2 AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_FifoGetINM_Records(in itemCode nvarchar(50),
	in requestQty DECIMAL(21,6),in fromWH nvarchar(8))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
 	 qtySum DECIMAL(21,6); 
 	 sqlStr1 nvarchar(5000);
 	 rowCount int:=0; 
	 rowN int:=1; 
	 curOpenQty decimal:=0;  
 	  		
BEGIN
	delete from "TMSPFifoGetINMRec_Data";
	delete from "TMSPFifoGetINMRec_INM";
		
	INSERT INTO "TMSPFifoGetINMRec_Data"(transNum, openQty, CalcPrice, instance, OpenValue, inQty, 
		outQty, transValue,expenses, openExp, allocation, openAlloc, priceDiff, 
		openPDiff, expAlloc, oExpAlloc,exchDiff, openEDiff, negInvAdjs, openNegInv, 
		btransVal, bexpVal, bnegAVal, baseQty, transSeq, prevTrans,Warehouse)
		SELECT "TransNum", "OpenQty", "CalcPrice", "Instance", "OpenValue", "InQty", 
			"OutQty", "TransValue","Expenses", "OpenExp", "Allocation", "OpenAlloc",
			"PriceDiff", "OpenPDiff", "ExpAlloc", "OExpAlloc","ExchDiff", "OpenEDiff",
			"NegInvAdjs", "OpenNegInv", "BTransVal", "BExpVal", "BNegAVal", "BaseQty",
			"TransSeq", "PrevTrans","Warehouse"
			FROM OINM WHERE "OpenQty" > 0 AND "ItemCode" = :itemCode 
			ORDER BY "TransNum", "Instance";
	sqlStr1 := 'CREATE SEQUENCE squ INCREMENT BY 1 START WITH 1';
	EXEC sqlStr1;		
	IF LENGTH(:fromWH) > 0 THEN
		EXEC 'INSERT INTO "TMSPFifoGetINMRec_INM" SELECT squ.NEXTVAL, transNum, openQty, CalcPrice, instance AS oIns,'||
			'OpenValue, inQty, outQty, transValue,expenses, openExp,allocation,'|| 
			'openAlloc, priceDiff, openPDiff, expAlloc, oExpAlloc,'||
			'exchDiff, openEDiff, negInvAdjs, openNegInv, btransVal, bexpVal,'||
			'bnegAVal, baseQty, transSeq, prevTrans,instance'||
			' FROM "TMSPFifoGetINMRec_Data" WHERE Warehouse ='''||:fromWH||''' ORDER BY transNum, oIns'; 		
	ELSE
		EXEC 'INSERT INTO "TMSPFifoGetINMRec_INM" SELECT squ.NEXTVAL, transNum, openQty, CalcPrice, instance AS OIns,'||
			'OpenValue, inQty, outQty, transValue,expenses, openExp,allocation,'|| 
			'openAlloc, priceDiff, openPDiff, expAlloc, oExpAlloc,'||
			'exchDiff, openEDiff, negInvAdjs, openNegInv, btransVal, bexpVal,'||
			'bnegAVal, baseQty, transSeq, prevTrans,instance'||
			' FROM "TMSPFifoGetINMRec_Data" ORDER BY transNum, oIns'; 		
	END IF;	
	
	qtySum := 0;
	SELECT MAX(rowNum) INTO rowCount FROM "TMSPFifoGetINMRec_INM";
	IF :rowCount>0 THEN
		WHILE :rowN<=:rowCount DO
			SELECT openQty INTO curOpenQty FROM "TMSPFifoGetINMRec_INM" WHERE rowNum=:rowN;
			qtySum := :qtySum+:curOpenQty;
			rowN := :rowN+1;
			IF :qtySum>=:requestQty THEN
				BREAK;
			END IF;
		END WHILE;
		--DELETE FROM "TMSPFifoGetINMRec_INM" WHERE rowNum>=:rowN;
	END IF;

	
	SELECT * FROM "TMSPFifoGetINMRec_INM" WHERE rowNum<:rowN ORDER BY transNum,instance;
	EXEC 'DROP SEQUENCE squ';
	delete from "TMSPFifoGetINMRec_INM";
	delete from "TMSPFifoGetINMRec_Data";	
END;


