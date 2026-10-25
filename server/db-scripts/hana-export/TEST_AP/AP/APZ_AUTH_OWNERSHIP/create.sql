
CREATE PROCEDURE APZ_Auth_OwnerShip(in empId int default 0)
AS
BEGIN
	
	declare superUser nvarchar(1) = '';
	declare rule nvarchar(1) = '';
	declare rCount int = 0;
	
	--Create temp table save data
	create local temporary table #tempTable ("empId" int, "Object" nvarchar(20), "Peer" char(1), "Manager" char(1), "Subord" char(1), "Dept" char(1), "Branch" char(1), "Team" char(1), "Cmpny" char(1));
	
	--Get Super User of Employee Id
	select IFNULL((select SUPERUSER from OUSR where USERID = (select top 1 "userId" from OHEM where "empID" = :empId)), 'N') into superUser from dummy;
	
	--Set Rule by Super User
	if :superUser = 'Y' then
		rule := 'F';
	else
		rule := 'N';
	end if;
	
	--Insert data
	insert into #tempTable("empId", "Object", "Peer", "Manager", "Subord", "Dept", "Branch", "Team", "Cmpny") 
	select :empId, IFNULL("Object", T0.DATA) as "Object", IFNULL("Peer", :rule) as "Peer", 
	IFNULL("Manager", :rule) as "Manager", IFNULL("Subord", 'N') as "Subord", IFNULL("Dept", :rule) as "Dept", IFNULL("Branch", :rule) as "Branch",
	IFNULL("Team", :rule) as "Team", IFNULL("Cmpny", :rule) as "Cmpny"  
	FROM Split('13,14,1470000113,15,234000031,16,17,18,19,20,203,204,21,22,23,540000006,97,2,4,1470000128,1470000129,19000102,1470000132,1250000025,1470000130,171', ',') T0
	left join HEM5 T1 on T0.DATA = T1."Object" and T1."empID" = :empId;
	
	select (select count(1) from #tempTable where "Object" = '4') into rCount from dummy;
	if :rCount = 0
	then
		insert into #tempTable("empId", "Object", "Peer", "Manager", "Subord", "Dept", "Branch", "Team", "Cmpny") 
		select :empId, '4', :rule, :rule, :rule, :rule, :rule, :rule, :rule from dummy;
	end if;
	
	select (select count(1) from #tempTable where "Object" = '2') into rCount from dummy;
	if :rCount = 0
	then
		insert into #tempTable("empId", "Object", "Peer", "Manager", "Subord", "Dept", "Branch", "Team", "Cmpny") 
		select :empId, '4', :rule, :rule, :rule, :rule, :rule, :rule, :rule from dummy;
	end if;
	
	select (select count(1) from #tempTable where "Object" = '1250000025') into rCount from dummy;
	if :rCount = 0
	then
		insert into #tempTable("empId", "Object", "Peer", "Manager", "Subord", "Dept", "Branch", "Team", "Cmpny") 
		select :empId, '1250000025', :rule, :rule, :rule, :rule, :rule, :rule, :rule from dummy;
	end if;
	
	if :superUser = 'Y'
	then
		update #tempTable set "Peer" = 'F', "Manager" = 'F', "Subord" = 'F', "Dept" = 'F', "Branch" = 'F', "Team" = 'F', "Cmpny" = 'F';
	end if;
	
	--Get data
	select * from #tempTable;
	
END;




