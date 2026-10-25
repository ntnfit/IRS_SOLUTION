
CREATE PROCEDURE APZ_Process_DataOwnerShipAuthorization
AS
BEGIN
	
	/* Create temporary table */
	create local temporary table #HEM5Local("empId" integer, "Object" nvarchar(20), "Peer" varchar(1), "Manager" varchar(1), "Subord" varchar(1), "Dept" varchar(1), "Branch" varchar(1),"Team" varchar(1), "Cmpny" nvarchar(1));
	exec 'drop table #HEM5Local;';
	exec 'create local temporary table #HEM5Local as (select * from #HEM5);';
	
	/* Update default value */
	update #HEM5Local set "Peer" = 'N' where ifnull("Peer", '') = '';
	update #HEM5Local set "Manager" = 'N' where ifnull("Manager", '') = '';
	update #HEM5Local set "Subord" = 'N' where ifnull("Subord", '') = '';
	update #HEM5Local set "Dept" = 'N' where ifnull("Dept", '') = '';
	update #HEM5Local set "Branch" = 'N' where ifnull("Branch", '') = '';
	update #HEM5Local set "Team" = 'N' where ifnull("Team", '') = '';
	
	delete from HEM5 where "empID" = (select top 1 a."empId" from #HEM5Local a inner join HEM5 b on a."empId" = b."empID");
	insert into HEM5("empID", "Object", "Peer", "Manager", "Subord", "Dept", "Branch", "Team", "Cmpny") select * from #HEM5Local;
	
	select '' from dummy;
END;




