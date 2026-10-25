
CREATE PROCEDURE APZ_WorkScheduleRegister_Search(in 
	branch nvarchar(255) default null, 
	dept nvarchar(255) default null, 
	empNo int, 
	empName nvarchar(200) default null,
	status nvarchar(255) default null,
	validFrom timestamp,
	validTo timestamp)
AS 
BEGIN
	declare maxDate timestamp;
	select ifnull((select max("ValidTo") from APZ_OWSR),'9999-12-31') into maxDate from DUMMY;
	
	select b."empID", b."ExtEmpNo", a."LineNum", ifnull(a."ValidFrom", :validFrom) as "ValidFrom"
			, case when(a."LineNum" = 0 or a."LineNum" is null) then ifnull(a."ValidTo", :ValidTo) else a."ValidTo" end as "ValidTo",
			a."ValidTo", a."SchID", b."dept", ifnull(b."lastName", '')||', '|| ifnull(b."firstName", '') as "empName", b."branch",b."status"
		from OHEM b left join APZ_OWSR a on a."empID" = b."empID" and ((a."ValidFrom" between :validFrom and ifnull(:validTo, :maxdate) 
											or ifnull(a."ValidTo", a."ValidFrom") between :validFrom and ifnull(:validTo, :maxdate) or :validFrom between a."ValidFrom" 
											and ifnull(a."ValidTo", :maxdate) and :validFrom is not null)
										or (:validFrom is null and a."ValidFrom" < ifnull(:validTo, :maxDate)))
			where IFNULL(b."Active",'') = 'Y'
				and (ifnull(:branch, '') = '' or :branch = '0' or b."branch" = :branch)
					and (ifnull(:dept, '') = '' or :dept = '0' or b."dept" in (select data from split(:dept,',')))
					and (ifnull(:empName,'') = '' or (b."firstName" like '%' || :empName || '%' or b."middleName" like '%' || :empName || '%' or b."lastName" like '%' || :empName || '%'))
					and (ifnull(:empNo, 0) = 0 or b."empID" = :empNo)
					and (ifnull(:status, '') = '' or :status = '0' or exists(select 1 from split(:status, ',') x where cast(x.data as int) = b."status"));
END;

