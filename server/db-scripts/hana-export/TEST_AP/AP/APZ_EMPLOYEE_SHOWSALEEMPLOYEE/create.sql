
CREATE PROCEDURE apz_Employee_ShowSaleEmployee
as
begin
	select t0."SlpCode", t0."SlpName", t0."Memo", t0."Commission",'' as "UAddress",t1."empID",t2."USERID", t2."USER_CODE"
	FROM  OSLP t0
	left join Ohem t1 on t0."SlpCode" = t1."salesPrson"  
	left join OUSR t2 on t1."userId" = t2."USERID" 
	Where t0."Active"='Y';
end;




