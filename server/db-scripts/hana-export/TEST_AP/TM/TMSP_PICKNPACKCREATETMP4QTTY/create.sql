-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_PicknPackCreateTmp4Qtty
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
tableName varchar(40);
stmt nvarchar(2000);
begin
	select '#TMP_' || CURRENT_CONNECTION || to_char(hour(current_TIMESTAMP)) || to_char(minute(current_TIMESTAMP)) || to_char(second(current_TIMESTAMP)*1000) into tableName from dummy;
	
	stmt := 'create local temporary table "' || :tableName || '" AS (select U0."ItemCode", U0."WhsCode", SUM(U0."ReleasQtty") as "ReleasQtty", 
	SUM(u0."PickOty") as "PickOty", min (U0."ChgAsmBoMW") as "ChgAsmBoMW"	from	(	
	select T1."ItemCode", T1."WhsCode", T1."ReleasQtty" * T1."NumPerMsr" as "ReleasQtty", T1."PickOty" * T1."NumPerMsr" as "PickOty", T1."ChgAsmBoMW"
		from  "RDR1" T1 inner  join "ORDR" T0 on T0."DocEntry" = T1."DocEntry"	
		where T0."DocType" = ''I'' and T1."LineStatus" <> ''C''	
		union all	
		select T1."ItemCode",T1."WhsCode",T1."ReleasQtty" * T1."NumPerMsr" as "ReleasQtty", T1."PickOty" * T1."NumPerMsr" as "PickOty", T1."ChgAsmBoMW"	
		from  "INV1" T1 inner  join "OINV" T0 on  T0."DocEntry" = T1."DocEntry"	
		where T0."isIns" = ''Y'' and T0."DocType" = ''I'' and T1."LineStatus"<>''C''	
union all
select T1."ItemCode",T1."FromWhsCod" as "WhsCode",T1."ReleasQtty" * T1."NumPerMsr" as "ReleasQtty" , T1."PickOty" * T1."NumPerMsr" as "PickOty" , T1."ChgAsmBoMW" 
	from  "WTQ1" T1 inner  join "OWTQ" T0 on  T0."DocEntry" = T1."DocEntry" 
where T1."LineStatus"<>''C''
union all
select T1."ItemCode",T1."wareHouse" as "WhsCode",T1."ReleaseQty" as "ReleasQtty", T1."PickQty" as "PickOty", ''N'' as "ChgAsmBoMW"
	from  "WOR1" T1 inner  join "OWOR" T0 on  T0."DocEntry" = T1."DocEntry" 
where (T0."Status" = N''P'' OR T0."Status" = N''R'') AND (T0."Type" = N''S'' OR T0."Type" = N''P'') AND T1."ItemType" = 4
		) U0	
		group by u0."ItemCode",u0."WhsCode")';
	--ins_msg_proc (:stmt);
	exec(:stmt);

	select :tableName from dummy;
end;


