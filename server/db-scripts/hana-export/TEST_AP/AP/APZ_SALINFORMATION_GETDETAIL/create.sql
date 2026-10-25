
CREATE PROCEDURE APZ_SalInformation_GetDetail(in empId int)
as
begin
	declare sqlQuery nvarchar(400000);
	declare columnQuery nvarchar(400000);
	
	select string_agg('a."' || COLUMN_NAME || '"', ',') into columnQuery from TABLE_COLUMNS 
					where TABLE_NAME = 'APZ_OHSI' and SCHEMA_NAME = current_schema and COLUMN_NAME not in ('LineNum','LineStatus','EmpId');
	
	sqlQuery = N'Select IFNULL(a."LineNum",1) as "LineNum",case when ifnull(a."LineNum",0) = 0 then b."status" else  a."LineStatus" end as "LineStatus",'|| :columnQuery  
	|| ',b."dept", IFNULL(b."lastName",'''') || '', '' || IFNULL(b."firstName",'''') as "empName",b."branch" ,b."status", b."empID", b."ExtEmpNo" '
	|| 'From OHEM b  LEFT OUTER JOIN APZ_OHSI a ON a."EmpId" = b."empID" '
	|| 'Where b."empID" = '|| :empId ||';';
	execute immediate :sqlQuery;
end




