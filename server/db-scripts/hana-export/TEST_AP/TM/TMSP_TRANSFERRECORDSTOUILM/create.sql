-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables2 AFTER:PT:PROCESS_END

create procedure TmSp_TransferRecordsToUILM(in MsgFromCopy INT,in MsgToCopy INT)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
	MaxMIDfromUILM INT; 
	MessageIDDelta INT;
	cnum int;
	col nvarchar(256);
	sqll varchar(4096);
BEGIN
	SELECT IFNULL(MAX("MessageID"),0) into MaxMIDfromUILM FROM UILM; 
	MessageIDDelta := :MaxMIDfromUILM - :MsgFromCopy;
	
	delete from "TmSpTransferRecordsToUILM_TMP";
	
	insert into "TmSpTransferRecordsToUILM_TMP" 
	SELECT SC.COLUMN_NAME, SC.POSITION
	FROM "PUBLIC".TABles SO INNER JOIN "PUBLIC"."TABLE_COLUMNS" SC ON SO.TABLE_OID = SC.TABLE_OID  
	WHERE SO.Table_name = 'OILM' and SC.POSITION <> 1 and SC.SCHEMA_NAME = CURRENT_SCHEMA 	ORDER BY SC.POSITION;
	
	sqll := 'INSERT INTO UILM SELECT "MessageID" + ' || cast(:MessageIDDelta as char(128));
	
	select count(*) into cnum from "TmSpTransferRecordsToUILM_TMP";
	--select cnum from dummy;
	while (:cnum) > 0 do
		select top 1 "NAME" into col from "TmSpTransferRecordsToUILM_TMP" order by "POS";
		delete from "TmSpTransferRecordsToUILM_TMP" where name = :col;
		sqll := :sqll || ', "' || :col || '"';
		cnum := :cnum - 1;
	END while;
	
	sqll := :sqll || ' FROM OILM WHERE "MessageID" > '
		|| cast (:MsgFromCopy as char(128)) || ' AND "MessageID" <= '
		|| cast (:MsgToCopy as char(128));
	exec (:sqll);

	INSERT INTO UILM1
	 	SELECT "MessageID" + :MessageIDDelta, "ItemCode", "SysNumber", "Quantity", "MdAbsEntry"
	FROM "ILM1"
		WHERE "MessageID" > :MsgFromCopy AND "MessageID" <= :MsgToCopy; 
	
	INSERT INTO UILM2
		SELECT "MessageID" + :MessageIDDelta,"AccountId","AcctCode","DebitCredi"
	FROM "ILM2"
		WHERE "MessageID" > :MsgFromCopy AND "MessageID" <= :MsgToCopy; 
	
	delete from "TmSpTransferRecordsToUILM_TMP";
end;


