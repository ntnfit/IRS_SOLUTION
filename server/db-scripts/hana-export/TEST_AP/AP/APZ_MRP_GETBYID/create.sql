
CREATE procedure apz_mrp_getbyId(in Id bigint)
AS
BEGIN
	declare minDate date; 
	declare maxDate date;
	select min("DueDate") into minDate from APZ_MRP1 where "OMrpId"=:Id;
	select max("DueDate") into maxDate from APZ_MRP1 where "OMrpId"=:Id;
	SELECT *, case when "DueDate" = :minDate then '-1' else cast(YEAR ("DueDate") as nvarchar) || '.' || right(cast(100 + WEEK("DueDate") as nvarchar), 2) end "WeekOfDay", 
			  case when "DueDate" = :minDate then '-1' else TO_NVARCHAR("DueDate", 'yyyy.MM') end "MonthOfDay" from apz_MRP1 where "OMrpId"=:Id;
END




