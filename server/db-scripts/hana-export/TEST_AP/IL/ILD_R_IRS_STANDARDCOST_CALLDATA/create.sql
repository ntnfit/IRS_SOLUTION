CREATE procedure ILD_R_IRS_StandardCost_CallData (
	in fdate			timestamp,
	in tdate			timestamp
)
as
begin

call ILD_R_IRS_StandardCost (fdate,tdate,tab_result);
select * from $tab_result;
end;

