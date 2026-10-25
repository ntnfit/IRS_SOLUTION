
CREATE PROCEDURE APZ_SalInformation_GetAdvancedDetail
	(in empId int)
as
begin
select IFNULL(a."LineNum",0) as "LineNum",a."Period",a."Amount",a."Remain",IFNULL(a."MonthlyPay",'N') as "MonthlyPay",IFNULL(a."Received",'N') as "Received",a."Remarks",b."dept"
	,IFNULL(b."firstName",'') || ', ' || IFNULL(b."lastName",'') as "empName",b."branch", b."empID",a."UserSign2",a."UpdateDate" from OHEM b LEFT JOIN APZ_HSI2 a ON a."EmpId" = b."empID" 
	 where b."empID" = :empId;
end
	




