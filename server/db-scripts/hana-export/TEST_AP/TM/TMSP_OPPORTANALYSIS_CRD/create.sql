-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables2 AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam

CREATE PROCEDURE TmSp_OpportAnalysis_CRD (in CondText nvarchar(2048),in GetCrdGroup int,in GetAllOpr nchar) 
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
tmpStr nvarchar(4000);
temp_table_count int;
v_sql nvarchar(1000);
begin
--For HANA security issue call procedure "_TmSp_ValidateSpParam"
    call _TmSp_ValidateSpParam(:CondText);    
	call _TmSp_ValidateSpParam(:GetAllOpr);    
	
	delete from "TMPSOpportAnal_CRD_Output" where 1 = 1;
	
tmpStr := ' FROM OOPR T1 INNER JOIN OPR1 T0 ON T1."OpprId" = T0."OpprId" INNER JOIN OSLP T4 ON T1."SlpCode" = T4."SlpCode" INNER JOIN OCRD T3 ON T1."CardCode" = T3."CardCode" INNER JOIN OOST T2 ON T0."Step_Id" = T2."Num" INNER JOIN OCRG T5 ON T3."GroupCode" = T5."GroupCode"';
if :GetAllOpr = 'N' then
	if :GetCrdGroup = 0 then 
		tmpStr := 'SELECT T3."CardCode", T3."CardName", T1."Status", T1."OpprId"'||:tmpStr||'  '||:CondText || ' GROUP BY T3."CardCode", T3."CardName", T1."Status", T1."OpprId" ORDER BY T3."CardCode"';
	else 
		tmpStr := 'SELECT T5."GroupCode", T5."GroupName", T1."Status", T1."OpprId"'||:tmpStr||'  '||:CondText || ' GROUP BY T5."GroupCode", T5."GroupName", T1."Status", T1."OpprId" ORDER BY T5."GroupCode"';
	end if;
	
else
	tmpStr := 'SELECT T0."OpprId", T0."Line", T0."SlpCode", T0."CntctCode", T0."OpenDate", T0."CloseDate", T0."Step_Id", T0."ClosePrcnt", T0."MaxSumLoc", T0."MaxSumSys", CAST(T0."Memo" AS nvarchar(50)) AS Expr1, T0."DocId", T0."ObjType", T0."Status", T0."Linked", T0."WtSumLoc", T0."WtSumSys"'||:tmpStr||
	'  '||:CondText||' GROUP BY T0."OpprId", T0."Line", T0."SlpCode", T0."CntctCode", T0."OpenDate", T0."CloseDate", T0."Step_Id", T0."ClosePrcnt", T0."MaxSumLoc", T0."MaxSumSys", CAST(T0."Memo" AS nvarchar(50)), T0."DocId", T0."ObjType", T0."Status",  T0."Linked", T0."WtSumLoc", T0."WtSumSys" ORDER BY T0."OpprId"';
	create local temporary row table #OPP_AN_CRD_OUTPUT ("OpprId" int, "Line" int, "SlpCode" int, "CntctCode" int,"OpenDate" date,"CloseDate" date, "Step_Id" int, "ClosePrcnt" DECIMAL(21,6),"MaxSumLoc" DECIMAL(21,6),"MaxSumSys" DECIMAL(21,6), "Expr1" NVARCHAR(50), "DocId" INT, "ObjType" NVARCHAR (2), "Status" NVARCHAR (1), "Linked" NVARCHAR (1), "WtSumLoc" DECIMAL(21,6), "WtSumSys" DECIMAL(21,6));
end if;

	tmpStr:='INSERT INTO "TMPSOpportAnal_CRD_Output" '||tmpStr;
	exec (:tmpStr);
	select * from "TMPSOpportAnal_CRD_Output";
	delete from "TMPSOpportAnal_CRD_Output" where 1 = 1;
end;


