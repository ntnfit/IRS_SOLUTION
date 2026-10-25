
CREATE PROCEDURE USP_InsertDocumentApprovalRequest
(
	in WtmCode int,
	in OwnerId int,
	in DocEntry int,
	in DocDate timestamp,
	in Remarks nclob,
	in UpdateEntry int
)
as
BEGIN

	declare WddCode int;
	declare CreateTime int;
	declare DocType  nvarchar(20);
	declare wstcode int;
	declare tblname nvarchar(10);
	declare updateSql nvarchar(100);
	
	-- ******************************************* OWDD **********************************************************
	-- Tính wddCode
	SELECT TOP 1 "WddCode" into WddCode default 0 FROM owdd WHERE "DraftType" = '112' AND "WtmCode" = :WtmCode AND "DraftEntry" = :DocEntry AND "OwnerID" = :OwnerId;

	-- createtime
	select cast(to_varchar(now(), 'HH24mi') as int) into CreateTime from DUMMY;
	-- CurrStep

	select ifnull((select "ObjType" from ODRF where "DocEntry"=:DocEntry),'') into DocType from DUMMY;
	
	select ifnull((SELECT TOP 1  "TblName" FROM apz_tblobject a WHERE a."ObjType"= :DocType), '') into tblname from DUMMY;
	
	if :tblname <> '' and :UpdateEntry<>0
	THEN
		updateSql = N'UPDATE ' || :tblname || ' SET "WddStatus"=''W'' WHERE "DocEntry"=' || cast(:UpdateEntry as nvarchar(11)) || ';';
		EXEC :updateSql;
	end IF;
	
	UPDATE odrf SET "WddStatus" ='W' WHERE "DocEntry" =:DocEntry;
	
	DELETE from oWdd WHERE "WddCode"=:WddCode;
	DELETE from wdd1 WHERE "WddCode"=:WddCode;
	
	SELECT OWDD_S.NEXTVAL into WddCode FROM DUMMY;
	UPDATE ONNM SET "AutoKey" = :WddCode + 1 WHERE "ObjectCode" = '122';
	
	INSERT INTO OWDD ("WddCode", "ProcessID", "BFType", "WtmCode", "OwnerID", "DraftEntry", "ObjType", "DraftType", "DocDate", "CurrStep", "Status", "Remarks", "UserSign", "CreateDate", "CreateTime", "IsDraft", "MaxReqr", "MaxRejReqr", "DocEntry") 
		SELECT TOP 1 :WddCode, :WddCode, case when IFNULL(:UpdateEntry,0) = 0 then 'A' else 'U' end, :WtmCode, :OwnerID, :DocEntry, :DocType, 112, :DocDate, a."WstCode", 'W', :Remarks, :OwnerID, current_date, :CreateTime, 'Y', b."MaxReqr", b."MaxRejReqr", nullif(:UpdateEntry,0)
		FROM WTM2 a INNER JOIN OWST b ON a."WstCode" = b."WstCode" WHERE "WtmCode" = :WtmCode
	ORDER BY a."SortId";
	
	-- ******************************************* WDD1 **********************************************************
	select Ifnull((select "CurrStep" from OWDD where "WddCode"=:WddCode),0) into wstcode from DUMMY;
		
	INSERT INTO WDD1 ("WddCode", "StepCode", "UserID", "Status", "UserSign", "CreateDate", "CreateTime", "SortId") 
	SELECT :WddCode, A."WstCode", b."UserID", 'W', :OwnerId, current_date, :CreateTime, A."SortId"
	FROM WTM2 A INNER JOIN wst1 b ON b."WstCode" = a."WstCode"
	WHERE a."WtmCode" = :WtmCode AND a."WstCode" = :wstcode ORDER BY a."SortId";
	
	select :WddCode from dummy;
end

