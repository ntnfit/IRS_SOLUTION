
CREATE PROCEDURE APZ_EmployeeMaster_SaveBranches
AS
BEGIN
declare code int;
declare maxCode int;
	create local temporary table #APZ_Branches as (select top 0* from OUBR);
	execute immediate 'insert into #APZ_Branches select * from "#APZ_Branches"';
	Delete from OUBR;
	Insert into OUBR
	Select * from #APZ_Branches where IFNULL("Code",0)<> 0;

	 select IFNULL((select "AutoKey" from onnm where "ObjectCode" = '175'),0) into code from DUMMY;
	 select IFNULL((select Max("Code") from OUBR),0) into maxCode from DUMMY;
	  if code < maxCode
	  then
      select IFNULL((select Max("Code") from OUBR),0) into code from DUMMY;
      end if;

	  Insert into OUBR
  --lấy nhiều dòng tăng dần theo ID
  Select :code + ROW_NUMBER() OVER(ORDER BY "Name" ASC)-1 , "Name","Remarks","UserSign" from #APZ_Branches where IFNULL("Code",0) = 0;

 select IFNULL((select Max("Code") from OUBR),0) into code from DUMMY;
  update onnm set "AutoKey"=:code + 1 where "ObjectCode"='175';

END




