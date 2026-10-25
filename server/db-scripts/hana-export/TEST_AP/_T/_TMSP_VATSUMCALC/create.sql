-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables BEFORE:PT:PROCESS_START

CREATE PROCEDURE _TmSp_VatSumCalc (
in obj nvarchar(4),
in field nvarchar(20))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
obj1 nvarchar(4);
vdec tinyint;
rowNum int:=1;
rowCount int:=0;
vatSum1 float;
vatSumFrgn1 float;
vatSumSy1 float;	
vsum float;
sumFrgn float;
sumsy float;
sqlStr varchar(5000);
BEGIN
	delete from VatSumCalc_TmpTab;
	obj1 := right(:obj,3)||'1';
	SELECT MAX("SumDec") INTO vdec FROM OADM;
	vsum :=0;
	sumFrgn :=0;
	sumSy :=0;
	
	exec 'CREATE SEQUENCE my_s INCREMENT BY 1 START WITH 1';
	sqlStr := 'INSERT INTO VatSumCalc_TmpTab SELECT my_s.NEXTVAL, A."DocEntry",A."LineNum",A."VatPrcnt",A."LineTotal",'||
		'A."TotalFrgn",A."TotalSumSy",B."VatSum",B."VatSumFC",B."'||:field||'",B."DiscPrcnt"'||
		' FROM '||:obj1||' A INNER JOIN '||:obj||' B ON A."DocEntry"=B."DocEntry" '||
		' WHERE (A."VatSum" IS NULL OR A."VatSum"=0) AND (NOT A."VatPrcnt" IS NULL AND A."VatPrcnt"<>0)'||
		' AND NOT A."VatGroup" IS NULL ORDER BY A."DocEntry",A."LineNum"';
	--exec 'insert into TEST111 select my_s.NEXTVAL from dual';
	select :sqlStr from DUMMY;
	exec sqlStr;
	SELECT MAX(RowNum) INTO rowCount FROM VatSumCalc_TmpTab;
	IF :rowCount>0 THEN
		WHILE :rowNum < :rowCount DO
			SELECT ROUND(LineTotal*(VatPrcnt/100)*(1-DiscPrcnt/100),:vdec) INTO vatSum1
				FROM VatSumCalc_TmpTab WHERE RowNum=:rowNum;
			SELECT ROUND(TotalFrgn*(VatPrcnt/100)*(1-DiscPrcnt/100),:vdec) INTO vatSumFrgn1
				FROM VatSumCalc_TmpTab WHERE RowNum=:rowNum;
			SELECT ROUND(TotalSumSy*(VatPrcnt/100)*(1-DiscPrcnt/100),:vdec) INTO vatSumSy1
				FROM VatSumCalc_TmpTab WHERE RowNum=:rowNum;
			
			vsum := :vsum+:vatSum1;
			sumFrgn := :sumFrgn+:vatSumFrgn1;
			sumsy := :sumsy+:vatSumSy1;
			sqlStr := 'UPDATE '||:obj1||' SET "VatSum"='||:vatSum1||',"VatSumFrgn"='||:vatSumFrgn1||
				',"VatSumSy"='||:vatSumSy1||
				' WHERE "DocEntry" IN (SELECT DocEntry FROM VatSumCalc_TmpTab WHERE RowNum='||:rowNum||')'||
				' AND "LineNum" IN (SELECT LineNum FROM VatSumCalc_TmpTab WHERE RowNum='||:rowNum||')';
			EXEC sqlStr;
			rowNum := :rowNum+1;
		END WHILE;
			SELECT ROUND(VatSum-:vsum,:vdec) INTO vatSum1
				FROM VatSumCalc_TmpTab WHERE RowNum=:rowCount;
			SELECT ROUND(VatSumFc-:sumFrgn,:vdec) INTO vatSumFrgn1
				FROM VatSumCalc_TmpTab WHERE RowNum=:rowCount;
			SELECT ROUND(Dfield-:sumsy ,:vdec) INTO vatSumSy1
				FROM VatSumCalc_TmpTab WHERE RowNum=:rowCount;
			vsum :=0;
			sumFrgn:=0;
			sumSy:=0;
			
			sqlStr := 'UPDATE '||:obj1||' SET "VatSum"='||:vatSum1||',"VatSumFrgn"='||:vatSumFrgn1||
				',"VatSumSy"='||:vatSumSy1||
				' WHERE "DocEntry" IN (SELECT DocEntry FROM VatSumCalc_TmpTab WHERE RowNum='||:rowCount||')'||
				' AND "LineNum" IN (SELECT LineNum FROM VatSumCalc_TmpTab WHERE RowNum='||:rowCount||')';
			EXEC sqlStr;
	END IF;		
	exec 'drop sequence my_s';
	delete from VatSumCalc_TmpTab;
END;


