CREATE procedure ILD_IRS_Default_OutSourcing_Pro09
(
	in usernameLogin varchar(20)
)
as
begin
	declare userlist nvarchar(100);	
	declare df_OutSourcing nvarchar(10);
	
 IF exists (select "U_Value2" from "@OTHERSETINGS" where   "Code"  = 'Pro09_OutSourcing_Param') then
 	select "U_Value2"
 	into userlist
 	from  "@OTHERSETINGS"
 	where   "Code"  = 'Pro09_OutSourcing_Param';
 	
 	---
	userlist = replace(replace (userlist,'[',''''),']','''');
    userlist  = replace(:userlist,:usernameLogin,'6868');
   
   
    if (select count(*) from dummy where  :userlist like '%6868%') > 0 then
    	df_OutSourcing = 'Y';
    else
    	df_OutSourcing = 'N';
    end if;
   ---
 	select  :df_OutSourcing   as "OutSourcing" from dummy;
 	/*
	Create  local temporary  table #tbl
	(
		OutSourcing nvarchar(10)
	);
	insert into #tbl
    select  :df_OutSourcing   as "OutSourcing" from dummy;
	--
	select * from #tbl;
	drop table #tbl;
*/
End If;
end;

