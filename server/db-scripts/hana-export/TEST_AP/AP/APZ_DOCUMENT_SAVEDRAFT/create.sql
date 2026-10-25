

Create PROCEDURE apz_document_savedraft(in DocEntry INT, in DraftKey INT, in ObjType VARCHAR(20))
AS
BEGIN

	DECLARE tblname NVARCHAR(10)='';
	DECLARE tblline NVARCHAR(10)='';
	DECLARE sqlString VARCHAR(400000)='';
	DECLARE wddstatus CHAR(1);
	DECLARE wddcode INT;
	DECLARE rowCount INT;
	
	create local temporary table #tblWddCode("WddCode" int);
	insert into #tblWddCode("WddCode") SELECT "WddCode" FROM owdd WHERE "DraftEntry"=:DraftKey and "Status" = 'Y';
	rowCount := cast((select count(1) from #tblWddCode) as int);
	
	UPDATE odrf SET "DocStatus"='C' WHERE "DocEntry"=:DraftKey;
	
	select IFNULL((SELECT top 1 "TblName" FROM apz_tblobject a WHERE a."ObjType"=:ObjType), '') into tblname from DUMMY;
	select IFNULL((SELECT top 1 "TblLine" FROM apz_tblobject a WHERE a."ObjType"=:ObjType), '') into tblline FROM DUMMY;

	SELECT IFNULL((SELECT "WddStatus" FROM odrf WHERE "DocEntry"=:DraftKey),'-') into wddstatus from DUMMY;
	if :wddstatus='Y' then
		wddstatus='P';
	END IF;
	
	IF :wddstatus='N'
	THEN
		UPDATE ODRF SET "WddStatus"='-' WHERE "DocEntry"=:DraftKey;
		wddstatus='P';
	END IF;
    
    
	sqlString = 'UPDATE ' || :tblname || ' SET "draftKey"=' || cast(:DraftKey as nvarchar) || ' ,"WddStatus"=''' || :wddstatus || ''' where "DocEntry" = ' || cast(:DocEntry as nvarchar);
	EXEC :sqlString;
	
	while :rowCount > 0
	do
		wddCode := cast((SELECT top 1 "WddCode" FROM #tblWddCode) as int);
		if :wddcode > 0
		then
			update owdd set "DocEntry" = :DocEntry, "ObjType" = :ObjType, "IsDraft" = 'N',"ProcesStat"='P' where "WddCode" = :wddcode;
		end if;
		
		delete from #tblWddCode where "WddCode" = :wddCode;
		rowCount := :rowCount - 1;	
	end while;
		
END



