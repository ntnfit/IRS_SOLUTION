CREATE procedure USP_PRODUCTION_COSTING_PEROID (
)
as
begin
	select "Code","Name" from "OFPR" where "PeriodStat"='N' order by "Code" desc;
end

