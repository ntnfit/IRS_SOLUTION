-- B1 DEPENDS: AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam

CREATE PROCEDURE TmSp_OpportAnalysis_SLP(in condText nvarchar(2048),
	in getAllOpr nchar)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
	tmpStr nvarchar(4000);
BEGIN
--For HANA security issue call procedure "_TmSp_ValidateSpParam"
    call _TmSp_ValidateSpParam(:condText);    
	call _TmSp_ValidateSpParam(:getAllOpr);   
	
	IF :getAllOpr=N'N' THEN
		tmpStr := 'SELECT T4."SlpCode",T4."SlpName",T1."Status",T1."OpprId"';
	ELSE
		tmpStr := 'SELECT T0."OpprId",T0."Line",T0."SlpCode",T0."CntctCode",'||
			'T0."OpenDate",T0."CloseDate",T0."Step_Id",T0."ClosePrcnt",T0."MaxSumLoc",'||
			'T0."MaxSumSys",CAST(T0."Memo" AS nvarchar(50)) AS Expr1,T0."DocId",'||
			'T0."ObjType",T0."Status",T0."Linked",T0."WtSumLoc",T0."WtSumSys"';
	END IF;
	
	tmpStr := :tmpStr||' FROM OOPR T1 INNER JOIN OPR1 T0 ON T1."OpprId"=T0."OpprId"'||
		' INNER JOIN OSLP T4 ON T1."SlpCode"=T4."SlpCode" INNER JOIN'||
		' OCRD T3 On T1."CardCode"=T3."CardCode" INNER JOIN'||
		' OOST T2 ON T0."Step_Id"=T2."Num"';
		
	IF :getAllOpr=N'N' THEN
		tmpStr := :tmpStr||' '||:condText||' GROUP BY T4."SlpCode",T4."SlpName",'||
			'T1."Status",T1."OpprId" ORDER BY T4."SlpCode"';
	ELSE 
		tmpStr := :tmpStr || ' ' || :condText || ' GROUP BY T0."OpprId", T0."Line",'|| 
						'T0."SlpCode", T0."CntctCode",T0."OpenDate", T0."CloseDate",'|| 
						'T0."Step_Id", T0."ClosePrcnt",T0."MaxSumLoc", T0."MaxSumSys",'|| 
						'CAST(T0."Memo" AS nvarchar(50)),T0."DocId", T0."ObjType",'|| 
						'T0."Status", T0."Linked",T0."WtSumLoc", T0."WtSumSys"'||
						'ORDER BY T0."OpprId"';
	END IF;
	--select :tmpstr from dual;
	EXEC(:tmpStr);	
END;


