
CREATE PROCEDURE APZ_SalaryCalculation_SearchEmployees(
	in branch nvarchar(4000) default null,
	in dept nvarchar(4000) default null,
	in empNo int default null,
	in empName nvarchar(200) default null,
	in status nvarchar(4000) default null)
as
BEGIN
-- Select MAIN Information
	Select b."empID",b."dept", Ifnull(b."lastName",'') || 
							Case When Ifnull(b."middleName",'') != '' Then ' ' ||  Ifnull(b."middleName",'') Else '' End ||
							Case When Ifnull(b."firstName",'') != '' Then ', ' ||  Ifnull(b."firstName",'') Else '' End as "empName", b."branch",b."status" 
	From OHEM b  
	Where 
		(ifnull(:branch,0)=0  or (:branch<>0  and b."branch" = :branch)) 
		AND (ifnull(:dept,'')='' or :dept = '0' or (:dept<>'' and cast(b."dept" as nvarchar) in (select data from split(:dept,','))))
		AND (ifnull(:empName,'')='' or (:empName<>'' and 
			(b."firstName" like N'%' || :empName || '%'
			or b."middleName" like N'%' || :empName || '%'
			or b."lastName" like N'%' || :empName || '%')))
		AND (ifnull(:empNo,0)=0 or (:empNo<>0 and b."empID"=:empNo))
		AND (ifnull(:status,'')='' or :status = '0' or (:status<>'' and cast(b."status" as nvarchar) in (select data from split(:status,','))))
		and b."Active" = 'Y';
END




