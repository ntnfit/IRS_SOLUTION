
CREATE PROCEDURE APZ_EmployeeMaster_SaveStates
AS
BEGIN

create local temporary table #APZ_States as (select top 0* from OCST);
	execute immediate 'insert into #APZ_States select * from "#APZ_States"';
 delete from OCST a Where a."Country" in (select "Country" from #APZ_States);

 
  Insert into OCST("Code","Country", "Name","UserSign","eCode","GNRECode")
  Select "Code","Country", "Name","UserSign","eCode","GNRECode" from #APZ_States;
  
END





