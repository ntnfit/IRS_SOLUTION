create procedure ILD_R_PXKKVCNB_DOCNO (
	in fromdate date ,
	in todate date 
)
as
begin
	SELECT "DocNum" FROM OWTR where "DocDate" between :fromdate and :todate 
	UNION ALL
	SELECT "DocNum" FROM ODLN where "DocDate" between :fromdate and :todate ;
end;
