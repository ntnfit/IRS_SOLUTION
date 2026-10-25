
create procedure APZ_ProjectConfirm_Save(
	in type char(1)
)
as
begin
	Declare Id int =0;
	
	create local temporary table #APZ_OPJCTmp as (select top 0 * from APZ_OPJC);
	exec 'DROP TABLE #APZ_OPJCTmp;';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_OPJCTmp AS (SELECT * FROM #APZ_OPJC);';
	
	create local temporary table #APZ_PJC1Tmp as (select top 0 * from APZ_PJC1);
	exec 'DROP TABLE #APZ_PJC1Tmp;';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_PJC1Tmp AS (SELECT * FROM #APZ_PJC1);';

	if :type='U'
	then
		select MIN("DocEntry") into Id from #APZ_OPJCTmp;
		delete from APZ_PJC1 where "DocEntry" =:Id;
		delete from APZ_OPJC where "DocEntry"=:Id;
	else
		select ifnull(max("DocEntry"), 0) + 1 into Id from #APZ_OPJCTmp;
		update #APZ_OPJCTmp set "DocEntry" = :Id;
	end if;
			
	UPDATE #APZ_PJC1Tmp set "DocEntry" = :Id,"LineNum"= (SELECT ROW_NUMBER() OVER( ORDER BY "DocEntry") FROM #APZ_PJC1Tmp );

	insert into APZ_PJC1 select * from #APZ_PJC1Tmp;
	insert into APZ_OPJC select * from #APZ_OPJCTmp;
	-- update OpenQty report

	UPDATE T0  set "OpenQty" = CASE when (IFNULL(T0."ActualQty",0)- IFNULL(T1."QtyConfirm",0))<0 then 0 else IFNULL(T0."Quantity",0)- IFNULL(T1."QtyConfirm",0) end from APZ_RTC1  T0 
	Inner join (select Sum("ActualQty") as "QtyConfirm","BaseEntry","BaseLine" from APZ_PJC1 y 
	left join  APZ_OPJC y1 ON y1."DocEntry" = y."DocEntry" where  y1."Status"<>'C' group by "BaseEntry","BaseLine")T1
 	ON T0."DocEntry"=T1."BaseEntry" and T0."LineNum"=T1."BaseLine" ;

	-- Update LineStatus report
	UPDATE T0 set "LineStatus" =case when IFNULL( T0."OpenQty",0) = 0 then 'L' else 'O' end  from APZ_RTC1  T0 Inner join #APZ_PJC1Tmp T1 ON T0."DocEntry"=T1."BaseEntry" and T0."LineNum"=T1."BaseLine"; 
	--update status header report		
	UPDATE a set "Status" = case when b."OpenQty" = 0 then 'L' else 'O' end from APZ_ORTC a
				INNER JOIN (select "DocEntry", sum(ifnull(t0."OpenQty",0)) as "OpenQty" from APZ_RTC1 T0 
				INNER JOIN (select distinct "BaseEntry" from #APZ_PJC1Tmp) T1 ON t0."DocEntry" = T1."BaseEntry" Group by "DocEntry") b ON a."DocEntry" = b."DocEntry";
				
	select :Id,'' from DUMMY;
end;




