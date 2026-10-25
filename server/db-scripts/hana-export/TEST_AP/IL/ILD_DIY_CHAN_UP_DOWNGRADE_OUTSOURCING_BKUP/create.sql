create procedure ILD_DIY_Chan_Up_DownGrade_Outsourcing_bkup
(
	in sscc_List varchar(5000),
	out tab_result table
	(
		Result int 
	)
)
as
Begin
	--declare sscc_List varchar (5000) ='30230000800011;30230000800013;30230000800014;30250000800014;';	
create local temporary table #tbl
	(
		Row_Number int,
		SSCC varchar(20),
		WONO varchar(9),
		WO_Status  varchar(20)
	);
	insert into #tbl (Row_Number,SSCC,WONO )
	select  ROW_NUMBER() OVER() ,
			"part",
			 left("part",9)as "ProductionNum"
			 
	from  "SPLITSTRING" (:sscc_List,';');
---
delete from  #tbl where ifnull(SSCC,'')  = '';
--
update a
set  WO_Status  =  ifnull(upper(b."U_POStatus"),'')
from #tbl a
inner join OWOR b on a.WONO  = b."DocNum";
---
create local temporary table #rs
(
	Result int  -- 1: Error, 0: Khong bao loi
);
--
IF exists (select 1 from #tbl where ifnull(WO_Status,'')  = 'CLOSED') then
		insert into #rs values (1);
else   
	 	insert into #rs values (0);
		  
End IF;


---
tab_result = select * from #rs;
---
drop table #tbl;
drop table #rs;
end

