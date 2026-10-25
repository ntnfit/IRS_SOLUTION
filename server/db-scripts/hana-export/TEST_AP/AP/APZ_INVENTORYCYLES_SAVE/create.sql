

Create PROCEDURE APZ_InventoryCyles_Save(in tranType char(1) default '')
AS 
BEGIN
	Declare code bigint = 0;
	create local temporary table #tempdocLocal as (select top 0 * from OCYC);
	exec 'drop table #tempdocLocal';
	exec 'create local temporary table #tempdocLocal as (select * from #OCYC)';
	
	if :tranType='D'
	then
		select ifnull((select top 1 "Code" from #tempdocLocal),0) into code from dummy;
		delete from OCYC where "Code" = :code or "Name" = :code;
	else
		if :tranType = 'U'
		then
			select (select top 1 "Code" from #tempdocLocal) into code from dummy;
			delete from OCYC where "Code" = :code;
		else
			select ifnull((select MAX("Code") from OCYC), 0) + 1 into code from dummy;
		end if;
 	end if;
	
	--Update #tempdocLocal set "Code"= :code;

	

	insert into OCYC("Code", "Name", "Frequency", "Day", "Hour", "UserSign" ,"NextDate", "Type", "SubOption", "Interval", "EndType", "MaxOccur", "SeEndDat", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "DayInMonth", "Week", "DayOfWeek", "Month") 
	select ROW_NUMBER() OVER(ORDER BY "Code")  as "Code", "Name", "Frequency", "Day", "Hour", "UserSign" ,"NextDate", "Type", "SubOption", "Interval", "EndType", "MaxOccur", "SeEndDat", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "DayInMonth", "Week", "DayOfWeek", "Month" 
	from #tempdocLocal;
	
	--insert into OCYC("Code","Name","Frequency","Hour","NextDate","SubOption","Interval","EndType","MaxOccur","SeEndDat","Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday","DayInMonth","Week","DayOfWeek","Month")
	--			select :code,"Name","Frequency", "Hour" ,"NextDate", "SubOption", "Interval","EndType","MaxOccur","SeEndDat","Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday","DayInMonth","Week","DayOfWeek","Month" from #tempdocLocal ;
	select :code  from dummy; 	
	drop table #tempdocLocal;
END;




