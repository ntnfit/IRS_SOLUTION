
create procedure APZ_PackingManagement_InsertCase(in CaseCode nvarchar(50), in POCode int, in USERID int)
as
begin

	INSERT into APZ_DMC ("CaseCode", "POCode", USERID, "PostingDate", "CreateDate", "TypeCode", "WhsCode", "LineNumber", "Status", "Unit")
	values
	(
		:CaseCode,
		:POCode,
		:USERID,
		current_date,
		current_date,
		(select "TypeCode" from APZ_DMPO where "POCode" =:POCode),
		(Select "WhsCode" from APZ_DMPO where "POCode" = :POCode),
		ifnull((select max("LineNumber") from APZ_DMC), 0) + 1,
		(select "Status" from APZ_DMPO where "POCode" = :POCode),
		(select "Unit" from APZ_DMPO where "POCode" = :POCode)
	);
	
end;




