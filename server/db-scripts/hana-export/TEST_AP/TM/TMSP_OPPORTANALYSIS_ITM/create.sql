-- B1 DEPENDS: AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam

CREATE        PROCEDURE TmSp_OpportAnalysis_ITM(
in CondText 	nvarchar(2048),
in cutByObject	int,
in GetItmGroup	int,
in GetAllOpr	nchar)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS

tmpStr		nvarchar(4000);
begin
--For HANA security issue call procedure "_TmSp_ValidateSpParam"
    call _TmSp_ValidateSpParam(:CondText);    
	call _TmSp_ValidateSpParam(:GetAllOpr);   

if (:GetAllOpr = N'N')
then 
    if (:GetItmGroup = 0)
    then 
	if (:cutByObject = 2)
	then 
		tmpStr := 'SELECT '||
				'T9."ItemCode" AS "INV_Itm", T9."ItemName" AS "INV_ItmN", '||
				'T1."Status", T1."OpprId", '||
				'T1."IntCat1" as "Tmp1", T1."IntCat1" as "Tmp2", T1."RealSumLoc",   '||
				'T10."ItemCode" AS "RDR_Itm", T10."ItemName" AS "RDR_ItmN",  '||
				'T11."ItemCode" AS "DLN_Itm", T11."ItemName" AS "DLN_ItmN",  '||
		         '       T12."ItemCode" AS "QUT_Itm", T12."ItemName" AS "QUT_ItmN",  '||
		         '       T3."CardCode", T3."CardName"';
		         end if;
	 if (:cutByObject = 53)
	then 
		 tmpStr := 'SELECT  '||
				'T9."ItemCode" AS "INV_Itm", T9."ItemName" AS "INV_ItmN", '|| 
				'T1."Status", T1."OpprId",  '||
				'T1."IntCat1" as "Tmp1", T1."IntCat1" as "Tmp2", T1."RealSumLoc",  '|| 
				'T10."ItemCode" AS "RDR_Itm", T10."ItemName" AS "RDR_ItmN",  '||
				'T11."ItemCode" AS "DLN_Itm", T11."ItemName" AS "DLN_ItmN",  '||
		         '       T12."ItemCode" AS "QUT_Itm", T12."ItemName" AS "QUT_ItmN",  '||
		           '     T4."SlpCode", T4."SlpName"';
		           end if;
	if(:cutByObject <> 2 and :cutByObject <> 53)
	then
		 tmpStr := 'SELECT  '||
				'T9."ItemCode" AS "INV_Itm", T9."ItemName" AS "INV_ItmN", '||
				'T1."Status", T1."OpprId",   '||
				'T1."IntCat1" as "Tmp1", T1."IntCat1" as "Tmp2", T1."RealSumLoc", '||  
				'T10."ItemCode" AS "RDR_Itm", T10."ItemName" AS "RDR_ItmN",  '||
				'T11."ItemCode" AS "DLN_Itm", T11."ItemName" AS "DLN_ItmN",  '||
		           '     T12."ItemCode" AS "QUT_Itm", T12."ItemName" AS "QUT_ItmN" ';
		           end if;
		            
    
    
    else
    
	if (:cutByObject = 2)
	then
		 tmpStr := 'SELECT  '||
				'T13."ItmsGrpCod" AS "INV_Grp", T13."ItmsGrpNam" AS "INV_GrpN", '|| 
				'T1."Status", T1."OpprId",   '||
				'T1."IntCat1" as "Tmp1", T1."IntCat1" as "Tmp2", T1."RealSumLoc", '||  
				'T14."ItmsGrpCod" AS "RDR_Grp", T14."ItmsGrpNam" AS "RDR_GrpN", '|| 
	           '    		T15."ItmsGrpCod" AS "DLN_Grp", T15."ItmsGrpNam" AS "DLN_GrpN", '|| 
				'T16."ItmsGrpCod" AS "QUT_Grp", T16."ItmsGrpNam" AS "QUT_GrpN",  '||
				'T3."CardCode", T3."CardName"';
				end if;
	if ( :cutByObject = 53)
	then
		 tmpStr := 'SELECT  '||
				'T13."ItmsGrpCod" AS "INV_Grp", T13."ItmsGrpNam" AS "INV_GrpN",  '||
				'T1."Status", T1."OpprId",   '||
				'T1."IntCat1" as "Tmp1", T1."IntCat1" as "Tmp2", T1."RealSumLoc",  '|| 
				'T14."ItmsGrpCod" AS "RDR_Grp", T14."ItmsGrpNam" AS "RDR_GrpN",  '||
	           '    		T15."ItmsGrpCod" AS "DLN_Grp", T15."ItmsGrpNam" AS "DLN_GrpN", '|| 
				'T16."ItmsGrpCod" AS "QUT_Grp", T16."ItmsGrpNam" AS "QUT_GrpN", '|| 
				'T4."SlpCode", T4."SlpName"';
				end if ;
	if(:cutByObject <> 2 and :cutByObject <> 53)
	then 
		 tmpStr := 'SELECT  '||
				'T13."ItmsGrpCod" AS "INV_Grp", T13."ItmsGrpNam" AS "INV_GrpN",  '||
				'T1."Status", T1."OpprId",   '||
				'T1."IntCat1" as "Tmp1", T1."IntCat1" as "Tmp2", T1."RealSumLoc",  '|| 
				'T14."ItmsGrpCod" AS "RDR_Grp", T14."ItmsGrpNam" AS "RDR_GrpN", '|| 
	            '   		T15."ItmsGrpCod" AS "DLN_Grp", T15."ItmsGrpNam" AS "DLN_GrpN", '|| 
				'T16."ItmsGrpCod" AS "QUT_Grp", T16."ItmsGrpNam" AS "QUT_GrpN"';
	 end if; 
	end if;
  	

else
	 tmpStr := 'SELECT T0."OpprId", T0."Line", T0."SlpCode", T0."CntctCode",  '||
			'T0."OpenDate", T0."CloseDate", T0."Step_Id", T0."ClosePrcnt",  '||
			'T0."MaxSumLoc", T0."MaxSumSys",  '||
			'T0."Memo" AS "Expr1", T0."DocId",  '||
			'T0."ObjType", T0."Status", T0."Linked", T0."WtSumLoc", T0."WtSumSys"';
end if;
 tmpStr := :tmpStr || '  		FROM  OITM T9 INNER JOIN '||
              ' INV1 T5 ON T9."ItemCode" = T5."ItemCode" INNER JOIN '||
              ' OITB T13 ON T9."ItmsGrpCod" = T13."ItmsGrpCod" FULL OUTER JOIN '||
              ' OITM T10 INNER JOIN '||
               'RDR1 T6 ON T10."ItemCode" = T6."ItemCode" INNER JOIN '||
              ' OITB T14 ON T10."ItmsGrpCod" = T14."ItmsGrpCod" FULL OUTER JOIN '||
              ' OITM T11 INNER JOIN '||
              ' DLN1 T7 ON T11."ItemCode" = T7."ItemCode" INNER JOIN '||
              ' OITB T15 ON T11."ItmsGrpCod" = T15."ItmsGrpCod" FULL OUTER JOIN '||
               'OITM T12 INNER JOIN '||
              ' QUT1 T8 ON T12."ItemCode" = T8."ItemCode" INNER JOIN '||
               'OITB T16 ON T16."ItmsGrpCod" = T12."ItmsGrpCod" FULL OUTER JOIN '||
               'OSLP T4 INNER JOIN '||
              ' OPR1 T0 INNER JOIN '||
              ' OOST T2 ON T2."Num" = T0."Step_Id" ON T4."SlpCode" = T0."SlpCode" FULL OUTER JOIN '||
               'OCRD T3 INNER JOIN '||
               'OOPR T1 ON T3."CardCode" = T1."CardCode" ON T0."OpprId" = T1."OpprId" ON T8."DocEntry" = T0."DocId" AND T0."ObjType" = 23 ON T7."DocEntry" = T0."DocId" AND '|| 
               'T0."ObjType" = 15 ON T6."DocEntry" = T0."DocId" AND T0."ObjType" = 17 ON T5."DocEntry" = T0."DocId" AND T0."ObjType" = 13';

 tmpStr  :=  :tmpStr || ' ' ||:CondText || '  ORDER BY T0."ObjType", T1."OpprId"' ;

exec (:tmpStr);
end ;


