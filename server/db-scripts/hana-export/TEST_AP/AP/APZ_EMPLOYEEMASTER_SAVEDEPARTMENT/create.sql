
CREATE PROCEDURE APZ_EmployeeMaster_SaveDepartment
AS
BEGIN
declare code int;
declare maxCode int;
create local temporary table #APZ_Department as (select top 0* from OUDP);
	execute immediate 'insert into #APZ_Department select * from "#APZ_Department"';
 delete from OUDP;
 
  Insert into OUDP 
  Select * from #APZ_Department where IFNULL("Code",0) <> 0;
  
  --kiểm tra ID luôn Max
   select IFNULL((select "AutoKey" from onnm where "ObjectCode" = '119'),0) into code from DUMMY;
   select IFNULL((select Max("Code") from OUDP),0) into maxCode from DUMMY;
  if code< maxCode
  then 
   Select IFNULL((select Max("Code") from OUDP),0) into code from DUMMY;
  end if;
  --Kiểm tra UserSign 
  
   

  Insert into OUDP 
  --lấy nhiều dòng tăng dần theo ID
  Select :code + ROW_NUMBER() OVER(ORDER BY "Name" ASC)-1 ,"Name" ,"Remarks","UserSign","Father" from #APZ_Department where IFNULL("Code",0) = 0;

  select  IFNULL((select Max("Code") from OUDP),0) into code from DUMMY;
  update onnm set "AutoKey"=:code + 1 where "ObjectCode"='119';

END




