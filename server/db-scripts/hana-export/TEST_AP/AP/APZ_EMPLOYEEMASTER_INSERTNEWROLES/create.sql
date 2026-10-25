
CREATE PROCEDURE APZ_EmployeeMaster_InsertNewRoles
AS
BEGIN
 declare typeID int;
 declare maxID int;
 
 select  IFNULL((select "AutoKey" from onnm where "ObjectCode" = '172'),0) into typeID from Dummy;
	
	create local temporary table #APZ_Roles as (select top 0* from OHTY);
	execute immediate 'insert into #APZ_Roles select * from "#APZ_Roles"';
 delete from OHTY;
 
  Insert into OHTY
	Select * from #APZ_Roles where IFNULL("typeID",0) <> 0;

  --kiểm tra typeID luôn Max
 select IFNULL((select Max("typeID") from OHTY),0) into MaxID from DUMMY;
  if (typeID < maxID)
  then
  begin
 SELECT IFNULL((select Max("typeID") from OHTY),0) into typeID from DUMMY;
 end;
   end if;

  Insert into OHTY
  --lấy nhiều dòng tăng dần theo ID
  Select :typeID + ROW_NUMBER() OVER(ORDER BY "name" ASC)-1 , "name" ,"descriptio",'N' from #APZ_Roles where IFNULL(:typeID,0) = 0;

  select IFNULL((select Max(typeID) from OHTY),0) into typeID from DUMMY;
  update onnm set "AutoKey"=:typeID + 1 where "ObjectCode"='172';

END

--Select * ,(CASE locked WHEN typeID = 0 Then  'Y' Else  'N' END) from OHTY 
--SELECT *,
--(CASE 
--  WHEN typeID < 0 THEN 'Y' else 'N'
--END) as locked
--FROM OHTY




