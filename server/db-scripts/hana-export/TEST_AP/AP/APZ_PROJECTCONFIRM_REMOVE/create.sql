
create procedure APZ_ProjectConfirm_Remove
(
in Id int
)
as
begin
	Declare countLineStatus int=0;
	UPDATE T0  set "OpenQty" =(T0."OpenQty" + T1."Quantity")
	from APZ_RTC1  T0 Inner join APZ_PJC1 T1 ON T0."DocEntry"=T1."BaseEntry" and T0."LineNum"=T1."BaseLine" where T1."DocEntry"=:Id;
	
	UPDATE T0 set "LineStatus" =case when T0."OpenQty" = 0 then 'L' else 'O' end
	 from APZ_RTC1  T0 Inner join APZ_PJC1 T1 ON T0."DocEntry"=T1."BaseEntry" and T0."LineNum"=T1."BaseLine" where T0."DocEntry"=:Id;
		
		select count(*) into countLineStatus from APZ_RTC1  T0 Inner join APZ_PJC1 T1 ON T0."DocEntry"=T1."BaseEntry" and T0."LineNum"=T1."BaseLine" and (T0."LineStatus" ='O' or T0."LineStatus" ='C');
		if :countLineStatus=0
			then
			UPDATE APZ_ORTC set "Status" =case when  :countLineStatus = 0 then 'L' else 'O' end 
			WHERE "DocEntry"= (select top 1 T0."DocEntry" from APZ_RTC1  T0 left join APZ_PJC1 T1 ON T0."DocEntry"=T1."BaseEntry" and T0."LineNum"=T1."BaseLine") ;
			end if;
			
	Delete from APZ_OPJC where "DocEntry"=:Id ;
	Delete from APZ_PJC1 where "DocEntry"=:Id;
	
	select 0, '' from dummy;
end;




