CREATE procedure ILD_YesORNo_QAResult
(
	in FromDate			timestamp,
	in ToDate			timestamp
)
as
begin
select distinct cast("DocEntry" as varchar(50)) as "Code",   "U_empNameCreated" as "Created By"
from   "@QC_RESULT_H"
Where "CreateDate" between  FromDate and ToDate

union all
	select '0','Not Yet QA Result' from dummy

order by "Code";
/*
	select '000' as "Code",'QA Result' as "Name" from dummy
	union all
	select '999','Not Yet QA Result' from dummy;*/
end;

