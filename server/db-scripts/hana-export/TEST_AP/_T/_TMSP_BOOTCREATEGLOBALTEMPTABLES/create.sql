-- B1 DEPENDS: BEFORE:PT:PROCESS_START

-- THIS FILE IS FULL 
-- THERE IS A LIMITATION FOR CRSP field that is currently set to DBM_MEMO_MAX(72000)
-- PLEASE CONTINUE WRITING IN NEXT _TmSp_BootCreateGlobalTempTables files e.g. _TmSp_BootCreateGlobalTempTables2.txt

CREATE PROCEDURE _TmSp_BootCreateGlobalTempTables
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS 
sqlStr nvarchar(1000);
exists1 int;
BEGIN
	select count(*) into exists1 from "PUBLIC".m_tables
	where table_name = 'DocLineTypeLayoutTempTab' and schema_name = current_schema;

	IF :exists1 <> 0 then
	    exec('DROP TABLE "DocLineTypeLayoutTempTab"');
	end if;

		sqlStr := 'CREATE GLOBAL TEMPORARY TABLE "DocLineTypeLayoutTempTab" (	' ||
		 ' "DocEntry" int, ' ||
		 ' "LineNum" int, ' ||
		 ' "LineNum2" int, ' ||
		 ' "LineSeq2" int, ' || 
		 ' "LineType" NCHAR(1), ' || 
		 ' "LineText" NTEXT, ' ||
		 ' "TotalLevel" INT, ' || 	
		 ' "SubTotal" DECIMAL(21,6), ' || 
		 ' "TotalFrgn" DECIMAL(21,6), ' || 
		 ' "TotalSumSy" DECIMAL(21,6), ' ||  
		 ' "GTotal" DECIMAL(21,6), ' || 
		 ' "GTotalFC" DECIMAL(21,6), ' || 
		 ' "GTotalSC" DECIMAL(21,6), ' ||
		 ' "VatSum" DECIMAL(21,6), ' || 
		 ' "VatSumFrgn" DECIMAL(21,6), ' || 
		 ' "VatSumSy" DECIMAL(21,6))';
		
		
	exec (:sqlStr);
	
	select count(*) into exists1 from "PUBLIC".m_tables
	where table_name = 'DocLineTypeLayoutDOC1' and schema_name = current_schema;

	IF :exists1 <> 0 then
		exec ('DROP TABLE "DocLineTypeLayoutDOC1"');
	end if;
	
	sqlStr := ('CREATE GLOBAL TEMPORARY TABLE "DocLineTypeLayoutDOC1" (' ||
			   ' "DocEntry" int, ' ||
			   ' "VisOrder" int, ' ||
			   ' "LineType" nvarchar(1), ' ||
			   ' "LineTotal" DECIMAL(21,6),' ||
			   ' "TotalFrgn" DECIMAL(21,6),' ||
			   ' "TotalSumSy" DECIMAL(21,6),' || 
			   ' "GTotal" DECIMAL(21,6),' ||
			   ' "GTotalFC" DECIMAL(21,6),' ||
			   ' "GTotalSC" DECIMAL(21,6),' ||
			   ' "VatSum" DECIMAL(21,6),' ||
			   ' "VatSumFrgn" DECIMAL(21,6),' ||
			   ' "VatSumSy" DECIMAL(21,6))');
			    
	exec (:sqlStr);
	
	select count(*) into exists1 from "PUBLIC".m_tables
	where table_name = 'DocLineTypeLayoutDOC10' and schema_name = current_schema;

	IF :exists1 <> 0 then
		exec ('DROP TABLE "DocLineTypeLayoutDOC10"');
	end if;
	
	sqlStr := ('CREATE GLOBAL TEMPORARY TABLE "DocLineTypeLayoutDOC10" (' ||
				' "DocEntry" int, ' ||
				' "AftLineNum" int, ' ||
				' "LineSeq" int, ' ||
				' "LineType" nvarchar(1), ' ||
				' "LineText" NCLOB )');
	exec (:sqlStr);
	
	select count(*) into exists1 from "PUBLIC".m_tables
	where table_name = 'ConGrpLineOACTRecursAccountsToProcess' and schema_name = current_schema;
	
	IF :exists1 <> 0 then
		exec ('DROP TABLE "ConGrpLineOACTRecursAccountsToProcess"');
	end if;
	
	sqlStr := ('CREATE GLOBAL TEMPORARY TABLE "ConGrpLineOACTRecursAccountsToProcess" (' ||
				'"acct" NVARCHAR (15),' ||
				'"level" SMALLINT,' || 
				'"processed" SMALLINT)');
				
	exec (:sqlStr);
	
	select count(*) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPPubUN_UsersEmployees' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "CRSPPubUN_UsersEmployees";
	end if;
	
	create global TEMPORARY TABLE "CRSPPubUN_UsersEmployees"(
         "UserID" int,
         "EmployeeID" int,
         "Name" nvarchar(160)
    );
    
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPPubUN_Employees' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "CRSPPubUN_Employees";
	end if;
	
    create global TEMPORARY TABLE "CRSPPubUN_Employees"(
    	  "EmployeeID" Int,
    	  "FirstName" nvarchar(50),
    	  "MiddleName" nvarchar(50),
    	  "LastName" nvarchar(50)
    );
	select count(*) into exists1 from "PUBLIC".m_tables
	where table_name = 'CRSPGeneralLedgerAllAccountAcctFatherSonTable' and schema_name = current_schema;
	
	IF :exists1 <> 0 then
		exec ('DROP TABLE "CRSPGeneralLedgerAllAccountAcctFatherSonTable"');
	end if;
	
	sqlStr := 'create global temporary table "CRSPGeneralLedgerAllAccountAcctFatherSonTable" (' ||
			  ' GroupCode nvarchar(15), ' ||
			  ' AcctCode nvarchar(15), ' || 
			  ' AcctName nvarchar(100), ' ||
		      ' Father0 nvarchar(15), ' ||
		      ' Father1 nvarchar(15), ' ||
			  ' Father2 nvarchar(15), ' ||
		      ' Father3 nvarchar(15), ' ||
		      ' Father4 nvarchar(15))';
	exec (:sqlStr);
	
	select count(*) into exists1 from "PUBLIC".m_tables
	where table_name = 'CRSPGeneralLedgerAllAccountAcctFatherSon' and schema_name = current_schema;
	IF :exists1 <> 0 then
		exec ('DROP TABLE "CRSPGeneralLedgerAllAccountAcctFatherSon"');
	end if;

	sqlStr := 'create global temporary row table "CRSPGeneralLedgerAllAccountAcctFatherSon"
	(
		GroupCode nvarchar(15),
 		GroupName nvarchar(100),
 		GroupDirect nvarchar(4),
 		AcctCode nvarchar(15),
 		AcctName nvarchar(100)	
	)';
	exec (:sqlStr);

	select count(*) into exists1 from "PUBLIC".m_tables
	where table_name = 'CRSPGeneralLedgerAllAccountAcctFatherSonResult' and schema_name = current_schema;
	IF :exists1 <> 0 then
		exec ('DROP TABLE "CRSPGeneralLedgerAllAccountAcctFatherSonResult"');
	end if;
	
	sqlStr := 'create global temporary table "CRSPGeneralLedgerAllAccountAcctFatherSonResult"
	(
 		GroupCode nvarchar(15),
 		GroupName nvarchar(100),
 		GroupDirect nvarchar(15),
		AcctCode nvarchar(15),
 		AcctName nvarchar(100),
 		ALLYEAR int, 
 		ALLMONTH int, 
 		Account nvarchar(15), 
		Debit DECIMAL(21,6), 
		Credit DECIMAL(21,6), 
 		TransId int, 
 		Line_ID int, 
 		RefDate TimeStamp, 
 		YEAR int, 
 		MONTH int, 
 		DAY int, 
 		LineMemo nvarchar(50),
 		Number int	
	)';
	exec (:sqlStr);	

	select count(*) into exists1 from "PUBLIC".m_tables
	where table_name = 'CRSPGeneralLedgerAllAccountDateTable' and schema_name = current_schema;
	IF :exists1 <> 0 then
		exec ('DROP TABLE "CRSPGeneralLedgerAllAccountDateTable"');
	end if;
		
	sqlStr := 'create global temporary table "CRSPGeneralLedgerAllAccountDateTable"
	(
		AllYear integer, 
		AllMonth integer
	)';
	exec (:sqlStr);	
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPPubCTS_Calendar' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "CRSPPubCTS_Calendar";
	end if;
	
	create global temporary table "CRSPPubCTS_Calendar"("AbsEntry" int, "TimeSection" int);

	select count(*) into exists1 from "PUBLIC".m_tables
	where table_name = 'CRSPCalendarMonthlyActivityInLineCalendar' and schema_name = current_schema;
	IF :exists1 <> 0 then
		exec ('DROP TABLE "CRSPCalendarMonthlyActivityInLineCalendar"');
	end if;
		
	sqlStr := 'create global temporary table "CRSPCalendarMonthlyActivityInLineCalendar"
	(
		clgCode int,
		HldCode nvarchar(40),
		AbsEduSeq int,
		CallID int,
		Action nvarchar(1),
		Personal nvarchar(1),
		Details nvarchar(120),
		AttendUser int,
		AttendEmpl int,
		AssigneeUID int,
		QueueManagerUID int,
		TechUID int,
		TechEID int,
		SCInstance nvarchar(1)
	)';
	exec (:sqlStr);	

	select count(*) into exists1 from "PUBLIC".m_tables
	where table_name = 'CRSPCalendarMonthlyActivityInLineEmployeeAbsEdu' and schema_name = current_schema;
	IF :exists1 <> 0 then
		exec ('DROP TABLE "CRSPCalendarMonthlyActivityInLineEmployeeAbsEdu"');
	end if;
		
	sqlStr := 'create global temporary table "CRSPCalendarMonthlyActivityInLineEmployeeAbsEdu"
	(
		AbsEduSeq int,
		StartDate datetime,
		EndDate datetime,
		Details nvarchar(120),
		UserId int,
		EmployeeId int
	)';
	exec (:sqlStr);	


	select count(*) into exists1 from "PUBLIC".m_tables
	where table_name = 'CRSPCalendarMonthlyActivityInLineServiceCalls' and schema_name = current_schema;
	IF :exists1 <> 0 then
		exec ('DROP TABLE "CRSPCalendarMonthlyActivityInLineServiceCalls"');
	end if;
		
	sqlStr := 'create global temporary table "CRSPCalendarMonthlyActivityInLineServiceCalls"
	(
		ID int,
		CallID int, 
		StartDate datetime,
		EndDate datetime,
		BeginTime int,
		EndTime int,
		Details nvarchar(120),
		AssigneeUID int,
		QueueManagerUID int,
		TechUID int,
		TechEID int,
		Instance nvarchar(1)
	)';
	exec (:sqlStr);	
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPCALENDARMONTHLYACTIVITYINLINE_USERS' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table CRSPCalendarMonthlyActivityInLine_users;
	end if;
	
	create global temporary Table CRSPCalendarMonthlyActivityInLine_users (uId int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPCALENDARMONTHLYACTIVITYINLINE_EMPLOYEES' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table CRSPCalendarMonthlyActivityInLine_employees;
	end if;
	
	create global temporary Table CRSPCalendarMonthlyActivityInLine_employees (empId int);

	select count(*) into exists1 from "PUBLIC".m_tables
	where table_name = 'CRSPCalendarDailyAcrossItemsUsers' and schema_name = current_schema;
	IF :exists1 <> 0 then
		exec ('DROP TABLE "CRSPCalendarDailyAcrossItemsUsers"');
	end if;
		
	sqlStr := 'create global temporary table "CRSPCalendarDailyAcrossItemsUsers"
	(
		UserID smallint,
		UserCode nvarchar(16),
		UserName nvarchar(310)
	)';
	exec (:sqlStr);	

	select count(*) into exists1 from "PUBLIC".m_tables
	where table_name = 'CRSPCalendarDailyAcrossItemsEmployees' and schema_name = current_schema;
	IF :exists1 <> 0 then
		exec ('DROP TABLE "CRSPCalendarDailyAcrossItemsEmployees"');
	end if;
		
	sqlStr := 'create global temporary table "CRSPCalendarDailyAcrossItemsEmployees"
	(
		EmployeeID smallint,
		FirstName nvarchar(50),
		MiddleName nvarchar(50),
		LastName nvarchar(50)
	)';
	exec (:sqlStr);	

	select count(*) into exists1 from "PUBLIC".m_tables
	where table_name = 'CRSPCalendarDailyAcrossItemsCalendarMD' and schema_name = current_schema;
	IF :exists1 <> 0 then
		exec ('DROP TABLE "CRSPCalendarDailyAcrossItemsCalendarMD"');
	end if;
		
	sqlStr := 'create global temporary table "CRSPCalendarDailyAcrossItemsCalendarMD"
	(
		clgCode int,
		HldCode nvarchar(40),
		AbsEduSeq int,
		CallID int,
		Recontact datetime,
		EndDate datetime,
		BeginTime int,
		EndTime int,
		Action nvarchar(1),
		Personal nvarchar(1),
		Details nvarchar(120),
		UserId int,
		EmployeeId int,
		AssigneeUID int,
		QueueManagerUID int,
		TechUID int,
		TechEID int,
		SCInstance nvarchar(1)
	)';
	exec (:sqlStr);	

	select count(*) into exists1 from "PUBLIC".m_tables
	where table_name = 'CRSPCalendarDailyAcrossItemsEmployeesAbsEdu' and schema_name = current_schema;
	IF :exists1 <> 0 then
		exec ('DROP TABLE "CRSPCalendarDailyAcrossItemsEmployeesAbsEdu"');
	end if;
		
	sqlStr := 'create global temporary table "CRSPCalendarDailyAcrossItemsEmployeesAbsEdu"
	(
		AbsEduSeq int,
		StartDate datetime,
		EndDate datetime,
		Details nvarchar(120),
		UserId int,
		EmployeeId int
	)';
	exec (:sqlStr);	

	select count(*) into exists1 from "PUBLIC".m_tables
	where table_name = 'CRSPCalendarDailyAcrossItemsServiceCalls' and schema_name = current_schema;
	IF :exists1 <> 0 then
		exec ('DROP TABLE "CRSPCalendarDailyAcrossItemsServiceCalls"');
	end if;
		
	sqlStr := 'create global temporary table "CRSPCalendarDailyAcrossItemsServiceCalls"
	(
		CallID int,
		StartDate datetime,
		EndDate datetime,
		BeginTime int,
		EndTime int,
		Details nvarchar(120),
		AssigneeUID int,
		QueueManagerUID int,
		TechUID int,
		TechEID int,
		Instance nvarchar(1)
	)';
	exec (:sqlStr);	

	select count(*) into exists1 from "PUBLIC".m_tables
	where table_name = 'CRSPCalendarDailyAcrossItemsInstances' and schema_name = current_schema;
	IF :exists1 <> 0 then
		exec ('DROP TABLE "CRSPCalendarDailyAcrossItemsInstances"');
	end if;
		
	sqlStr := 'create global temporary table "CRSPCalendarDailyAcrossItemsInstances"
	(
		ClgCode int, 
		AttendUser int,
		AttendEmployee int,
		Recontact datetime,
		EndDate datetime,
		BeginTime int,
		EndTime int,
		Action nvarchar(1),
		Personal nvarchar(1),
		Details nvarchar(120)
	)';
	exec (:sqlStr);	

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPCalMGA_Res' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "CRSPCalMGA_Res";
	end if;
	
	create global temporary table "CRSPCalMGA_Res"
	( 
		"ID" int,
		"ClgCode" int,
		"AbsEduSeq" int, 
		"CallID" int, 
		"SCInstance" nvarchar(1),
		"CalendarDay" TIMESTAMP,
		"StartDate" TIMESTAMP,
		"EndDate" TIMESTAMP,
		"WeekNum" int
	);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPCalMGA_Seq' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "CRSPCalMGA_Seq";
	end if;

	create global temporary table "CRSPCalMGA_Seq"
	( 
		"ID" int,
		"ClgCode" int,
		"AbsEduSeq" int, 
		"CallID" int, 
		"SCInstance" nvarchar(1),
		"StrDate" TIMESTAMP,
		"EndDate" TIMESTAMP,
		"InstKey" int
	);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPCalMGA_Clg' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "CRSPCalMGA_Clg";
	end if;
	
	create global temporary table "CRSPCalMGA_Clg"
	(
		"ClgCode" int, 
		"ReContact" TIMESTAMP, 
		"EndDate" TIMESTAMP,
		"BeginTime" int,
		"EndTime" int
	);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPCalMGA_ServiceCalls' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "CRSPCalMGA_ServiceCalls";
	end if;
	
	create global temporary table "CRSPCalMGA_ServiceCalls"
	(
		"ID" int, 
		"CallID" int, 
		"StartDate" TIMESTAMP, 
		"EndDate" TIMESTAMP,
		"BeginTime" int, 
		"EndTime" int, 
		"Details" nvarchar(120), 
		"AssigneeUID" int, 
		"QueueManagerUID" int, 
		"TechUID" int, 
		"TechEID" int, 
		"Instance" nvarchar(1)
	);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPCalMGA_EmployeesAbsEdu' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "CRSPCalMGA_EmployeesAbsEdu";
	end if;
	
	create global temporary table "CRSPCalMGA_EmployeesAbsEdu"
	(
		"AbsEduSeq" int, 
		"StartDate" TIMESTAMP, 
		"EndDate" TIMESTAMP, 
		"Details" nvarchar(120), 
		"UserId" int, 
		"EmployeeId" int
	);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPPubDayItems_Users' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "CRSPPubDayItems_Users";
	end if;
		
	create global temporary table "CRSPPubDayItems_Users" ("UserID" int, "UserCode" nvarchar(16), "UserName" nvarchar(310));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPPubDayItems_Employees' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "CRSPPubDayItems_Employees";
	end if;

	
	create global temporary table "CRSPPubDayItems_Employees" (
		"EmployeeID" int, "FirstName" nvarchar(50), "MiddleName" nvarchar(50), "LastName" nvarchar(50));
		
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPPubDayItems_Calendar' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "CRSPPubDayItems_Calendar";
	end if;
	
	create global temporary table "CRSPPubDayItems_Calendar"("AbsEntry" int, "TimeSection" int, "EntryCount" int,
	"ClgCode1" int, "Action1" nvarchar(1), "Personal1" nvarchar(1), "Details1" nvarchar(120), "User1" int, "Employee1" int, "BeginTime1" int, 
	"EndTime1" int, "CallID1" int, "SCInstance1" nvarchar(1), "AssigneeUID1" int, "QueueManagerUID1" int, "TechUID1" int,"TechEID1" int, 
	"ClgCode2" int, "Action2" nvarchar(1), "Personal2" nvarchar(1), "Details2" nvarchar(120), "User2" int, "Employee2" int, "BeginTime2" int, 
	"EndTime2" int, "CallID2" int, "SCInstance2" nvarchar(1), "AssigneeUID2" int, "QueueManagerUID2" int, "TechUID2" int, "TechEID2" int, 
	"ClgCode3" int, "Action3" nvarchar(1), "Personal3" nvarchar(1), "Details3" nvarchar(120), "User3" int, "Employee3" int, "BeginTime3" int, 
	"EndTime3" int, "CallID3" int, "SCInstance3" nvarchar(1), "AssigneeUID3" int, "QueueManagerUID3" int, "TechUID3" int, "TechEID3" int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPPubDayItems_Activities' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "CRSPPubDayItems_Activities";
	end if;
	
	create global temporary table "CRSPPubDayItems_Activities" ("ClgCode" int, "Recontact" TIMESTAMP, "BeginTime" int, "EndTime" int, 
		"Action" nvarchar(1), "Personal" nvarchar(1), "Details" nvarchar(120), "UserId" int, "EmployeeId" int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPPubDayItems_Instances' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "CRSPPubDayItems_Instances" ;
	end if;
		
	create global temporary table "CRSPPubDayItems_Instances" ("ClgCode" int, "AttendUser" int, "AttendEmployee" int, 
	"Recontact" TIMESTAMP, "EndDate" TIMESTAMP, "BeginTime" int, "EndTime" int, "Action" nvarchar(1), "Personal" nvarchar(1), "Details" nvarchar(120));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPPubDayItems_ServiceCalls' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "CRSPPubDayItems_ServiceCalls";
	end if;
	
	create global temporary table "CRSPPubDayItems_ServiceCalls" ("CallID" int, "StartDate" TIMESTAMP, "EndDate" TIMESTAMP, "BeginTime" int, 
	"EndTime" int, "Details" nvarchar(120), "AssigneeUID" int, "QueueManagerUID" int, "TechUID" int, "TechEID" int, "Instance" nvarchar(1));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPPublicCrossDaysItemsHolidays' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		DROP TABLE "CRSPPublicCrossDaysItemsHolidays";
	end if;
	
	Create Global Temporary Table "CRSPPublicCrossDaysItemsHolidays"
		(Seq Int , StrDate DateTime, EndDate DateTime, Rmrks nvarchar(50));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPPublicCrossDaysItemsEmployeesAbsEdu' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		DROP TABLE "CRSPPublicCrossDaysItemsEmployeesAbsEdu";
	end if;
	
	Create Global Temporary Table "CRSPPublicCrossDaysItemsEmployeesAbsEdu"
		(AbsEduSeq Int, StartDate DateTime, EndDate DateTime, 
		Details NVarChar(120), UserId int, EmployeeId int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPPublicCrossDaysItemsServiceCalls' and "SCHEMA_NAME" = CURRENT_SCHEMA;

	if :exists1 <> 0 then
		DROP TABLE "CRSPPublicCrossDaysItemsServiceCalls";
	end if;

	Create GLOBAL temporary Table "CRSPPublicCrossDaysItemsServiceCalls"(CallID Int, StartDate DateTime, EndDate DateTime, BeginTime int, EndTime int, Details NVarChar(120), AssigneeUID int, QueueManagerUID int, TechUID int, TechEID int, Instance nvarchar(1));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPPublicCrossDaysItemsActivities' and "SCHEMA_NAME" = CURRENT_SCHEMA;

	if :exists1 <> 0 then
		DROP TABLE "CRSPPublicCrossDaysItemsActivities";
	end if;
	
	Create global temporary Table "CRSPPublicCrossDaysItemsActivities"(clgCode int, Recontact DateTime, EndDate DateTime, BeginTime int, EndTime int, Action nvarchar(1), Personal nvarchar(1), Details NVarChar(120), UserId int, EmployeeId int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPPublicCrossDaysItemsInstances' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		DROP TABLE "CRSPPublicCrossDaysItemsInstances";
	end if;
	
	Create global Temporary Table "CRSPPublicCrossDaysItemsInstances"(ClgCode int, AttendUser int, AttendEmployee int, Recontact DateTime, EndDate DateTime, BeginTime int, EndTime int, Action nvarchar(1), Personal nvarchar(1), Details NVarChar(120));
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPPublicCrossDaysItemsCalendarMD' and "SCHEMA_NAME" = CURRENT_SCHEMA;

	if :exists1 <> 0 then
		DROP TABLE "CRSPPublicCrossDaysItemsCalendarMD";
	end if;
	
	Create global temporary Table "CRSPPublicCrossDaysItemsCalendarMD"(Line int,
		Day1 DateTime, Pos1 int, HldSeq1 int, AbsEduSeq1 int, CallID1 int, ClgCode1 int, Action1 nvarchar(1), Personal1 nvarchar(1), User1 int, Employee1 int, AssigneeUID1 int, QueueManagerUID1 int, TechUID1 int, TechEID1 int, SCInstance1 nvarchar(1), BeginDate1 DateTime, EndDate1 DateTime, BeginTime1 int, EndTime1 int, Remark1 NVarChar(120),
		Day2 DateTime, Pos2 int, HldSeq2 int, AbsEduSeq2 int, CallID2 int, ClgCode2 int, Action2 nvarchar(1), Personal2 nvarchar(1), User2 int, Employee2 int, AssigneeUID2 int, QueueManagerUID2 int, TechUID2 int, TechEID2 int, SCInstance2 nvarchar(1), BeginDate2 DateTime, EndDate2 DateTime, BeginTime2 int, EndTime2 int, Remark2 NVarChar(120),
		Day3 DateTime, Pos3 int, HldSeq3 int, AbsEduSeq3 int, CallID3 int, ClgCode3 int, Action3 nvarchar(1), Personal3 nvarchar(1), User3 int, Employee3 int, AssigneeUID3 int, QueueManagerUID3 int, TechUID3 int, TechEID3 int, SCInstance3 nvarchar(1), BeginDate3 DateTime, EndDate3 DateTime, BeginTime3 int, EndTime3 int, Remark3 NVarChar(120),
		Day4 DateTime, Pos4 int, HldSeq4 int, AbsEduSeq4 int, CallID4 int, ClgCode4 int, Action4 nvarchar(1), Personal4 nvarchar(1), User4 int, Employee4 int, AssigneeUID4 int, QueueManagerUID4 int, TechUID4 int, TechEID4 int, SCInstance4 nvarchar(1), BeginDate4 DateTime, EndDate4 DateTime, BeginTime4 int, EndTime4 int, Remark4 NVarChar(120),
		Day5 DateTime, Pos5 int, HldSeq5 int, AbsEduSeq5 int, CallID5 int, ClgCode5 int, Action5 nvarchar(1), Personal5 nvarchar(1), User5 int, Employee5 int, AssigneeUID5 int, QueueManagerUID5 int, TechUID5 int, TechEID5 int, SCInstance5 nvarchar(1), BeginDate5 DateTime, EndDate5 DateTime, BeginTime5 int, EndTime5 int, Remark5 NVarChar(120),
		Day6 DateTime, Pos6 int, HldSeq6 int, AbsEduSeq6 int, CallID6 int, ClgCode6 int, Action6 nvarchar(1), Personal6 nvarchar(1), User6 int, Employee6 int, AssigneeUID6 int, QueueManagerUID6 int, TechUID6 int, TechEID6 int, SCInstance6 nvarchar(1), BeginDate6 DateTime, EndDate6 DateTime, BeginTime6 int, EndTime6 int, Remark6 NVarChar(120),
		Day7 DateTime, Pos7 int, HldSeq7 int, AbsEduSeq7 int, CallID7 int, ClgCode7 int, Action7 nvarchar(1), Personal7 nvarchar(1), User7 int, Employee7 int, AssigneeUID7 int, QueueManagerUID7 int, TechUID7 int, TechEID7 int, SCInstance7 nvarchar(1), BeginDate7 DateTime, EndDate7 DateTime, BeginTime7 int, EndTime7 int, Remark7 NVarChar(120));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPPublicCrossDaysItemsCalendarMDPositions' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		DROP TABLE "CRSPPublicCrossDaysItemsCalendarMDPositions";
	end if;
	
	Create global temporary Table "CRSPPublicCrossDaysItemsCalendarMDPositions"
	(
		WeekDate DateTime, 
		ClgCode int, 
		HldSeq int, 
		AbsEduSeq int, 
		CallID int, 
		SCInstance nvarchar(1), 
		Pos int);
		select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPPublicCrossDaysItemsUserIds' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		DROP TABLE "CRSPPublicCrossDaysItemsUserIds";
	end if;

	create global temporary table "CRSPPublicCrossDaysItemsUserIds" ( uId int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPPublicCrossDaysItemsEmployeeIds' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		DROP TABLE "CRSPPublicCrossDaysItemsEmployeeIds";
	end if;
	
	create global temporary table "CRSPPublicCrossDaysItemsEmployeeIds" (eId int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPPUBLICCROSSDAYSITEMS_USERS' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table CRSPPublicCrossDaysItems_Users;
	end if;
	
	create global temporary Table CRSPPublicCrossDaysItems_Users (UserID SmallInt, UserCode NVarChar(16), UserName NVarChar(155));


	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPPUBLICCROSSDAYSITEMS_EMPLOYEES' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table CRSPPublicCrossDaysItems_Employees;
	end if;
	
	create global temporary Table CRSPPublicCrossDaysItems_Employees (EmployeeID SmallInt, FirstName NVarChar(50), MiddleName NVarChar(50), LastName NVarChar(50));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'CRSPCalendarDailyGroupAcrossDaysItemsInstances' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then	
		DROP TABLE "CRSPCalendarDailyGroupAcrossDaysItemsInstances";
	end if;
	
	Create Global Temporary Table "CRSPCalendarDailyGroupAcrossDaysItemsInstances"
	(ClgCode int, AttendUser int, AttendEmployee int, Recontact DateTime, EndDate DateTime, BeginTime int, EndTime int, Action nvarchar(1), Personal nvarchar(1), Details NVarChar(120));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'CRSPCalendarDailyGroupAcrossDaysItemsCalendarMD' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then	
		drop table "CRSPCalendarDailyGroupAcrossDaysItemsCalendarMD";
	end if;
	
	Create global temporary Table "CRSPCalendarDailyGroupAcrossDaysItemsCalendarMD"(
	clgCode int, HldCode NVarChar(40), AbsEduSeq int, CallID Int, Recontact DateTime, EndDate DateTime, BeginTime int, EndTime int, Action nvarchar(1), Personal nvarchar(1), Details NVarChar(120), UserId int, EmployeeId int, AssigneeUID int, QueueManagerUID int, TechUID int, TechEID int, SCInstance nvarchar(1));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'CRSPCalendarDailyGroupAcrossDaysItemsUsers' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then	
		drop table "CRSPCalendarDailyGroupAcrossDaysItemsUsers";
	end if;
	
	Create global temporary Table "CRSPCalendarDailyGroupAcrossDaysItemsUsers" (UserID SmallInt, UserCode NVarChar(16), UserName NVarChar(310));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'CRSPCalendarDailyGroupAcrossDaysItemsEmployees' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "CRSPCalendarDailyGroupAcrossDaysItemsEmployees";
	end if;
	
	Create global temporary Table "CRSPCalendarDailyGroupAcrossDaysItemsEmployees" (EmployeeID SmallInt, FirstName NVarChar(50), MiddleName NVarChar(50), LastName NVarChar(50));
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'CRSPPubDayItems_CalM' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "CRSPPubDayItems_CalM";
	end if;

	create global temporary table "CRSPPubDayItems_CalM" ("CallID" int , "ClgCode" int, "EntryCount" int);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'CRSPPubDayItems_CalT' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "CRSPPubDayItems_CalT";
	end if;
	
	create global temporary table "CRSPPubDayItems_CalT" ("AbsEntry" int, "EntryCount" int , "CallID1" int , "CallID2" int , 
			"CallID3" int , "ClgCode1" int , "ClgCode2" int, "ClgCode3" int );
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'CRSPDetailLedgerAllAccountResult' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "CRSPDetailLedgerAllAccountResult";
	end if;	
	
	create global temporary row table "CRSPDetailLedgerAllAccountResult"
		(AcctCode nvarchar(15), AcctName nvarchar(100), RefDate timestamp, RefYear int, RefMonth int, RefDay int, Number int, Memo nvarchar(50), 
		FCDebit DECIMAL(21,6), FCCredit DECIMAL(21,6), FCCurrency nvarchar(3), Debit DECIMAL(21,6), Credit DECIMAL(21,6), BalDirect NVARCHAR(4), 
		LineID int, TransId int, CurOnRight nvarchar(1), DecSep nvarchar(1), SumDec smallint, ThousSep nvarchar(1), DateFormat nvarchar(1), CharMonth int, 
		DateSep nvarchar(1), TimeFormat nvarchar(1), LogoImage BLOB, LogoFile nvarchar(200), MnhlNote nvarchar(1), CompnyName nvarchar(100)); 
 
 	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'CRSPDetailLedgerOpenBalanceResult' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then	   
		drop table "CRSPDetailLedgerOpenBalanceResult";
	end if;
	
	create global temporary row table "CRSPDetailLedgerOpenBalanceResult" (debit DECIMAL(21,6), credit DECIMAL(21,6));
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'IVSRUserInteractionRequiredQuery_TempCheckStockResults' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "IVSRUserInteractionRequiredQuery_TempCheckStockResults";
	end if;
	
	Create global temporary Table "IVSRUserInteractionRequiredQuery_TempCheckStockResults" (Errors nvarchar(1000));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'IVSRUserInteractionRequiredQuery_SBO_RSP_IVI_Affected_Item' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "IVSRUserInteractionRequiredQuery_SBO_RSP_IVI_Affected_Item";
	end if;
	
	Create global temporary Table "IVSRUserInteractionRequiredQuery_SBO_RSP_IVI_Affected_Item" ("Description" nvarchar(100), "ItemCode" nvarchar(50), "DocNum" nvarchar(100), "DocEntry" int, "Document_Type" nvarchar(100));
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'IVSRUserInteractionRequiredQuery_SBO_RSP_IVI_Affected_WareHouse' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "IVSRUserInteractionRequiredQuery_SBO_RSP_IVI_Affected_WareHouse";
	end if;
	
	Create global temporary Table "IVSRUserInteractionRequiredQuery_SBO_RSP_IVI_Affected_WareHouse" ("Description" nvarchar(100), "ItemCode" nvarchar(50), "DocNum" nvarchar(100), "DocEntry" int, "Document_Type" nvarchar(100), "WareHouse" nvarchar(100));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'CustomerRankingFiscalY_CustomerRanking_1' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "CustomerRankingFiscalY_CustomerRanking_1";
	end if;
	
	Create global temporary Table "CustomerRankingFiscalY_CustomerRanking_1" ("DocNum" nvarchar(20), "SlpName" nvarchar(100), "DocDate" DateTime, "DocDueDate" DateTime, "CardCode" nvarchar(15), "CardName" nvarchar(100), "Sales" DECIMAL(21,6), "Paid" DECIMAL(21,6), "GrossProfit" DECIMAL(21,6), "GrossPrcnt" DECIMAL(21,6),  "SalesSys" DECIMAL(21,6), "PaidSys" DECIMAL(21,6), "GrossProfitSys" DECIMAL(21,6), "GrossPrcntSys" DECIMAL(21,6), "DocEntry" nvarchar(20), "DocType" VarChar (10), "SalesForPrcnt" DECIMAL(21,6), "SalesForPrcntSys" DECIMAL(21,6), "instID" DECIMAL(21,6), "instNum" DECIMAL(21,6));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'CRSPAnnualSalesAnalysisQuarter_TmpPsar' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "CRSPAnnualSalesAnalysisQuarter_TmpPsar";
	end if;
	
	create global temporary Table "CRSPAnnualSalesAnalysisQuarter_TmpPsar" ("CardCode" nvarchar(15), "CardName" nvarchar(100), "Quarter" int, "Year" int, "Sales" DECIMAL(21,6), "GrossPrft" DECIMAL(21,6), "SalesSys" DECIMAL(21,6), "GrossPrftSys" DECIMAL(21,6), "SalesForPrcnt" DECIMAL(21,6), "SalesForPrcntSys" DECIMAL(21,6));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'CRSPAnnualSalesAnalysisQuarterInsert_TMPMonth' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "CRSPAnnualSalesAnalysisQuarterInsert_TMPMonth";
	end if;
	
	create global temporary Table "CRSPAnnualSalesAnalysisQuarterInsert_TMPMonth" ("DocEntry" int, "Quarter" int);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'TmSp_IVSR_AHRQ_CheckStockResults' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmSp_IVSR_AHRQ_CheckStockResults";
	end if;
	
	create global temporary table "TmSp_IVSR_AHRQ_CheckStockResults" ("Errors" nvarchar(1000));
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'TmSp_IVSR_AHRQ_RSP_IVI_Autofx_Boms' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmSp_IVSR_AHRQ_RSP_IVI_Autofx_Boms";
	end if;
	
	create global temporary table "TmSp_IVSR_AHRQ_RSP_IVI_Autofx_Boms" ("ItemCode" nvarchar(50), "CreatedBy" int, 
	"TransType" int, "DocLineNum" int, "OldDocLineNum" int, "MessageID" int);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'TmSp_IVSR_AHRQ_Disorder_detect_record_found' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmSp_IVSR_AHRQ_Disorder_detect_record_found";
	end if;
	
	create global temporary table "TmSp_IVSR_AHRQ_Disorder_detect_record_found" ( "rec" int);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'TmSp_IVSR_AHRQ_Disorder_detect_DocTables' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmSp_IVSR_AHRQ_Disorder_detect_DocTables";
	end if;
	
	create global temporary table "TmSp_IVSR_AHRQ_Disorder_detect_DocTables" ("DocTable" nvarchar(4));
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmSp_IVSR_AHRQ_DISORDER_OILM_WITHOU_JE' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmSp_IVSR_AHRQ_DISORDER_OILM_WITHOU_JE";
	end if;
	
	create global temporary table "TmSp_IVSR_AHRQ_DISORDER_OILM_WITHOU_JE" ("DocEntry" int, "TransType" int, "CreateDate" TIMESTAMP);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmSp_IVSR_AHRQ_DISORDER_OJDTTemp' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmSp_IVSR_AHRQ_DISORDER_OJDTTemp";
	end if;
	
	create global temporary table "TmSp_IVSR_AHRQ_DISORDER_OJDTTemp" ("TransType" nvarchar(20), "CreatedBy" int, "CreateDate" TIMESTAMP);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Purch_An_Crd_Det_Y' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Purch_An_Crd_Det_Y";
	end if;
	
	create global temporary table "TmpPsar_Purch_An_Crd_Det_Y" ("DocEntry" nvarchar(20), "DocNum" nvarchar(20), "ObjCode" nvarchar(10), "SlpName" nvarchar(155), "DocDate" DateTime, "DocDueDate" DateTime, "CardCode" nvarchar(15), "CardName" nvarchar(100), "Sales" DECIMAL(21,6), "PaidSum" DECIMAL(21,6), "SalesSys" DECIMAL(21,6), "PaidSumSys" DECIMAL(21,6), "instID" DECIMAL(21,6), "instNum" DECIMAL(21,6), "AgrNo" int, "AgrAbsId" int);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Purch_An_Crd_Grp_M' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Purch_An_Crd_Grp_M";
	end if;
	
	create global temporary Table "TmpPsar_Purch_An_Crd_Grp_M" ("CardCode" nvarchar(15), "CardOrSlpName" nvarchar(155), "Month" int, "Year" Int, "Sales" DECIMAL(21,6), "SalesSys" DECIMAL(21,6));	
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Purch_An_Crd_Grp_Y' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Purch_An_Crd_Grp_Y";
	end if;
	
	create global temporary Table "TmpPsar_Purch_An_Crd_Grp_Y" ("CardCode" nvarchar(15), "CardOrSlpName" nvarchar(155), "DocCount" int, "Sales" DECIMAL(21,6), "OpenSum" DECIMAL(21,6), "SalesSys" DECIMAL(21,6), "OpenSumSys" DECIMAL(21,6));
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Purch_An_Crd_Sng_M' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Purch_An_Crd_Sng_M";
	end if;
	
	create global temporary Table "TmpPsar_Purch_An_Crd_Sng_M" ("CardCode" nvarchar(15), "CardOrSlpName" nvarchar(155), "Month" int, "Year" Int, "Sales" DECIMAL(21,6), "SalesSys" DECIMAL(21,6), "AgrNo" int, "AgrAbsId" int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Purch_An_Crd_Sng_Y' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Purch_An_Crd_Sng_Y";
	end if;
	
	create global temporary Table "TmpPsar_Purch_An_Crd_Sng_Y" ("CardCode" nvarchar(15), "CardOrSlpName" nvarchar(155), "DocCount" int, "Sales" DECIMAL(21,6), "OpenSum" DECIMAL(21,6),  "SalesSys" DECIMAL(21,6), "OpenSumSys" DECIMAL(21,6), "AgrNo" int, "AgrAbsId" int);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Purch_An_Itm_Det_Y' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Purch_An_Itm_Det_Y";
	end if;

	create global temporary Table "TmpPsar_Purch_An_Itm_Det_Y" ("DocEntry" nvarchar(20), "DocNum" nvarchar(20), "DocType" nvarchar(10), "SlpName" nvarchar(155), "DocDate" DateTime, "DocDueDate" DateTime, "ItemCode" nvarchar(50), "ItemName" nvarchar(100), "Qty" DECIMAL(21,6), "NoInvMove" nvarchar(1), "Price" DECIMAL(21,6), "Sales" DECIMAL(21,6), "PriceSys" DECIMAL(21,6),  "SalesSys" DECIMAL(21,6), "LineNum" int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Purch_An_Itm_Grp_M' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Purch_An_Itm_Grp_M";
	end if;

	create global temporary Table "TmpPsar_Purch_An_Itm_Grp_M" ("DocEntry" nvarchar(20), "DocNum" nvarchar(20), "SlpCode" nvarchar(20), "SlpName" nvarchar(155), "CardCode" nvarchar(20), "CardName" nvarchar(100), "DocDate" DateTime, "DocDueDate" DateTime, "ItemCode" nvarchar(50), "ItemName" nvarchar(100), "Quantity" DECIMAL(21,6), "Sales" DECIMAL(21,6), "SalesSys" DECIMAL(21,6), "LineNum" int);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Purch_An_Itm_Grp_Y' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Purch_An_Itm_Grp_Y";
	end if;

	create global temporary Table "TmpPsar_Purch_An_Itm_Grp_Y" ("DocEntry" nvarchar(20), "DocNum" nvarchar(20), "SlpCode" nvarchar(20), "SlpName" nvarchar(155), "CardCode" nvarchar(20), "CardName" nvarchar(100), "DocDate" DateTime, "DocDueDate" DateTime, "ItemCode" nvarchar(50), "ItemName" nvarchar(100), "Quantity" DECIMAL(21,6), "Sales" DECIMAL(21,6), "SalesSys" DECIMAL(21,6), "LineNum" int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Purch_An_Itm_Sng_M' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Purch_An_Itm_Sng_M";
	end if;

	create global temporary Table "TmpPsar_Purch_An_Itm_Sng_M" ("DocEntry" nvarchar(20), "DocNum" nvarchar(20), "SlpCode" nvarchar(20), "SlpName" nvarchar(155), "CardCode" nvarchar(20), "CardName" nvarchar(100), "DocDate" DateTime, "DocDueDate" DateTime, "ItemCode" nvarchar(50), "ItemName" nvarchar(100), "Quantity" DECIMAL(21,6), "Sales" DECIMAL(21,6), "SalesSys" DECIMAL(21,6), "LineNum" int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Purch_An_Itm_Sng_Y' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Purch_An_Itm_Sng_Y";
	end if;

	create global temporary Table "TmpPsar_Purch_An_Itm_Sng_Y" ("DocEntry" nvarchar(20), "DocNum" nvarchar(20), "SlpCode" nvarchar(20), "SlpName" nvarchar(155), "CardCode" nvarchar(20), "CardName" nvarchar(100), "DocDate" DateTime, "DocDueDate" DateTime, "ItemCode" nvarchar(50), "ItemName" nvarchar(100), "Quantity" DECIMAL(21,6), "Sales" DECIMAL(21,6), "SalesSys" DECIMAL(21,6), "LineNum" int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Purch_An_Slp_Sng_M' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Purch_An_Slp_Sng_M";
	end if;

	create global temporary Table "TmpPsar_Purch_An_Slp_Sng_M" ("CardCode" nvarchar(15), "CardOrSlpName" nvarchar(155), "Month" int, "Year" Int,	"Sales" DECIMAL(21,6), "SalesSys" DECIMAL(21,6));
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Purch_An_Slp_Sng_Y' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Purch_An_Slp_Sng_Y";
	end if;
	
	create global temporary Table "TmpPsar_Purch_An_Slp_Sng_Y" ("CardCode" nvarchar(15), "CardOrSlpName" nvarchar(155), "DocCount" int, "Sales" DECIMAL(21,6), "OpenSum" DECIMAL(21,6), "SalesSys" DECIMAL(21,6), "OpenSumSys" DECIMAL(21,6));
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Sales_An_Crd_Det_Y' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Sales_An_Crd_Det_Y";
	end if;
	
	create global temporary Table "TmpPsar_Sales_An_Crd_Det_Y" ("DocNum" nvarchar(20), "SlpName" nvarchar(155), "DocDate" DateTime, "DocDueDate" DateTime, "CardCode" nvarchar(15), "CardName" nvarchar(100), "Sales" DECIMAL(21,6), "Paid" DECIMAL(21,6), "GrossProfit" DECIMAL(21,6), "GrossPrcnt" DECIMAL(21,6), "SalesSys" DECIMAL(21,6), "PaidSys" DECIMAL(21,6), "GrossProfitSys" DECIMAL(21,6), "GrossPrcntSys" DECIMAL(21,6), "DocEntry" nvarchar(20), "DocType" VarChar (10), "SalesForPrcnt" DECIMAL(21,6), "SalesForPrcntSys" DECIMAL(21,6), "instID" DECIMAL(21,6), "instNum" DECIMAL(21,6), "AgrNo" int, "AgrAbsId" int);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Sales_An_Crd_Det_Y_LC' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Sales_An_Crd_Det_Y_LC";
	end if;
	
	create global temporary Table "TmpPsar_Sales_An_Crd_Det_Y_LC" ("DocNum" nvarchar(20), "SlpName" nvarchar(155), "DocDate" DateTime, "DocDueDate" DateTime, "CardCode" nvarchar(15), "CardName" nvarchar(100), "Sales" DECIMAL(21,6), "Paid" DECIMAL(21,6), "GrossProfit" DECIMAL(21,6), "GrossPrcnt" DECIMAL(21,6), "SalesSys" DECIMAL(21,6), "PaidSys" DECIMAL(21,6), "GrossProfitSys" DECIMAL(21,6), "GrossPrcntSys" DECIMAL(21,6), "DocEntry" nvarchar(20), "DocType" VarChar (10), "SalesForPrcnt" DECIMAL(21,6), "SalesForPrcntSys" DECIMAL(21,6), "instID" DECIMAL(21,6), "instNum" DECIMAL(21,6));
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Sales_An_Crd_Grp_M' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Sales_An_Crd_Grp_M";
	end if;
	
	create global temporary Table "TmpPsar_Sales_An_Crd_Grp_M" ("CardCode" nvarchar(15), "CardOrSlpName" nvarchar(155), "Month" int, "Year" Int, "Sales" DECIMAL(21,6), "GrossPrft" DECIMAL(21,6), "GrossPrcnt" DECIMAL(21,6), "SalesSys" DECIMAL(21,6), "GrossPrftSys" DECIMAL(21,6), "GrossPrcntSys" DECIMAL(21,6), "SalesForPrcnt" DECIMAL(21,6), "SalesForPrcntSys" DECIMAL(21,6));
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Sales_An_Crd_Grp_Y' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Sales_An_Crd_Grp_Y";
	end if;
	
	create global temporary Table "TmpPsar_Sales_An_Crd_Grp_Y" ("CardCode" nvarchar(15), "CardOrSlpName" nvarchar(155), "DocCount" int, "Sales" DECIMAL(21,6), "GrossPrft" DECIMAL(21,6), "GrossPrcnt" DECIMAL(21,6), "TotalOpen" DECIMAL(21,6), "SalesSys" DECIMAL(21,6), "GrossPrftSys" DECIMAL(21,6), "GrossPrcntSys" DECIMAL(21,6), "TotalOpenSys" DECIMAL(21,6), "SalesForPrcnt" DECIMAL(21,6), "SalesForPrcntSys" DECIMAL(21,6));
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Sales_An_Crd_Sng_M' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Sales_An_Crd_Sng_M";
	end if;
	
	create global temporary Table "TmpPsar_Sales_An_Crd_Sng_M" ("CardCode" nvarchar(15), "CardOrSlpName" nvarchar(155), "Month" int, "Year" Int, "Sales" DECIMAL(21,6), "GrossPrft" DECIMAL(21,6), "GrossPrcnt" DECIMAL(21,6), "SalesSys" DECIMAL(21,6), "GrossPrftSys" DECIMAL(21,6), "GrossPrcntSys" DECIMAL(21,6), "SalesForPrcnt" DECIMAL(21,6), "SalesForPrcntSys" DECIMAL(21,6), "AgrNo" int, "AgrAbsId" int);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Sales_An_Crd_Sng_M_LC' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Sales_An_Crd_Sng_M_LC";
	end if;
	
	create global temporary Table "TmpPsar_Sales_An_Crd_Sng_M_LC" ("CardCode" nvarchar(15), "CardOrSlpName" nvarchar(155), "Month" int, "Year" Int, "Sales" DECIMAL(21,6), "GrossPrft" DECIMAL(21,6), "GrossPrcnt" DECIMAL(21,6), "SalesSys" DECIMAL(21,6), "GrossPrftSys" DECIMAL(21,6), "GrossPrcntSys" DECIMAL(21,6), "SalesForPrcnt" DECIMAL(21,6), "SalesForPrcntSys" DECIMAL(21,6));
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Sales_An_Crd_Sng_Y' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Sales_An_Crd_Sng_Y";
	end if;
	
	create global temporary Table "TmpPsar_Sales_An_Crd_Sng_Y" ("CardCode" nvarchar(15), "CardOrSlpName" nvarchar(155), "DocCount" int, "Sales" DECIMAL(21,6), "GrossPrft" DECIMAL(21,6), "GrossPrcnt" DECIMAL(21,6), "TotalOpen" DECIMAL(21,6), "SalesSys" DECIMAL(21,6), "GrossPrftSys" DECIMAL(21,6), "GrossPrcntSys" DECIMAL(21,6),  "TotalOpenSys" DECIMAL(21,6), "SalesForPrcnt" DECIMAL(21,6), "SalesForPrcntSys" DECIMAL(21,6), "AgrNo" int, "AgrAbsId" int);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Sales_An_Crd_Sng_Y_LC' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Sales_An_Crd_Sng_Y_LC";
	end if;
	
	create global temporary Table "TmpPsar_Sales_An_Crd_Sng_Y_LC" ("CardCode" nvarchar(15), "CardOrSlpName" nvarchar(155), "DocCount" int, "Sales" DECIMAL(21,6), "GrossPrft" DECIMAL(21,6), "GrossPrcnt" DECIMAL(21,6), "TotalOpen" DECIMAL(21,6), "SalesSys" DECIMAL(21,6), "GrossPrftSys" DECIMAL(21,6), "GrossPrcntSys" DECIMAL(21,6), "TotalOpenSys" DECIMAL(21,6), "SalesForPrcnt" DECIMAL(21,6), "SalesForPrcntSys" DECIMAL(21,6));
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmpPsar_Sales_An_Itm_Det_Y' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "TmpPsar_Sales_An_Itm_Det_Y";
	end if;
	
	create global temporary Table "TmpPsar_Sales_An_Itm_Det_Y" ("DocEntry" nvarchar(20), "DocNum" nvarchar(20), "SlpName" nvarchar(155), "DocDate" DateTime, "DocDueDate" DateTime, "ItemCode" nvarchar(50), "ItemName" nvarchar(100), "Qty" DECIMAL(21,6), "NoInvMove" nvarchar(1), "Price" DECIMAL(21,6), "Sales" DECIMAL(21,6), "GrossProfit" DECIMAL(21,6), "GrossPrcntv" DECIMAL(21,6), "PriceSys" DECIMAL(21,6),  "SalesSys" DECIMAL(21,6), "GrossProfitSys" DECIMAL(21,6), "GrossPrcntSys" DECIMAL(21,6), "DocType" VarChar(10), "LineNum" int,  "SalesForPrcnt" DECIMAL(21,6), "SalesForPrcntSys" DECIMAL(21,6));
	
 	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'SalesAnItmGrp_M_TMPPSAR' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "SalesAnItmGrp_M_TMPPSAR";
	end if;	
	
	Create global temporary Table "SalesAnItmGrp_M_TMPPSAR" (DocEntry nvarchar(20), DocNum nvarchar(20), SlpCode nvarchar(20), SlpName nvarchar(155), CardCode nvarchar(20), CardName nvarchar(100), DocDate DateTime, DocDueDate DateTime, ItemCode nvarchar(50), ItemName nvarchar(100)
				, Quantity DECIMAL(21,6), Price DECIMAL(21,6), Sales DECIMAL(21,6), GrossProfit DECIMAL(21,6), GrossPrcnt DECIMAL(21,6),  PriceSys DECIMAL(21,6), SalesSys DECIMAL(21,6), 
					GrossProfitSys DECIMAL(21,6), GrossPrcntSys DECIMAL(21,6), LineNum int, SalesForPrcnt DECIMAL(21,6), SalesForPrcntSys DECIMAL(21,6));

 	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'SalesAnItmGrp_Y_TMPPSAR' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "SalesAnItmGrp_Y_TMPPSAR";
	end if;	
	
	Create global temporary Table "SalesAnItmGrp_Y_TMPPSAR" (DocEntry nvarchar(20), DocNum nvarchar(20), SlpCode nvarchar(20), SlpName nvarchar(155), CardCode nvarchar(20), CardName nvarchar(100), DocDate DateTime, DocDueDate DateTime, ItemCode nvarchar(50), ItemName nvarchar(100)
				, Quantity DECIMAL(21,6), Price DECIMAL(21,6), Sales DECIMAL(21,6), GrossProfit DECIMAL(21,6), GrossPrcnt DECIMAL(21,6),  PriceSys DECIMAL(21,6), SalesSys DECIMAL(21,6), 
					GrossProfitSys DECIMAL(21,6), GrossPrcntSys DECIMAL(21,6), LineNum int, SalesForPrcnt DECIMAL(21,6), SalesForPrcntSys DECIMAL(21,6));		

 	select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'SalesAnItmSng_M_TMPPSAR' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "SalesAnItmSng_M_TMPPSAR";
	end if;	

	Create global temporary Table "SalesAnItmSng_M_TMPPSAR" (DocEntry nvarchar(20), DocNum nvarchar(20), SlpCode  VarChar(20), SlpName nvarchar(155), CardCode nvarchar(20), CardName nvarchar(100), DocDate DateTime, DocDueDate DateTime, ItemCode nvarchar(50), ItemName nvarchar(100)
				, Quantity DECIMAL(21,6), Price DECIMAL(21,6), Sales DECIMAL(21,6), GrossProfit DECIMAL(21,6), GrossPrcnt DECIMAL(21,6),  PriceSys DECIMAL(21,6), SalesSys DECIMAL(21,6), 
					GrossProfitSys DECIMAL(21,6), GrossPrcntSys DECIMAL(21,6), LineNum int, SalesForPrcnt DECIMAL(21,6), SalesForPrcntSys DECIMAL(21,6));						

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CREATEOINMGROUPED_TEMPCORRINVDATA' and "SCHEMA_NAME" = CURRENT_SCHEMA;

	if :exists1 <> 0 then
		drop table CreateOINMGrouped_tempCorrInvData;
	end if;
	
	create global temporary Table CreateOINMGrouped_tempCorrInvData (_CID_TransType int, _CID_CreatedBy int, _CID_DocLineNum smallint, _CID_TotalQty DECIMAL(21,6), _CID_MaxTransSeq int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'SETLINENUMATDOC3_TAB' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table SetLineNumAtDOC3_tab;
	end if;
	
	create global temporary Table SetLineNumAtDOC3_tab ("RowNum" int, "DocEntry" int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'SETLINENUMATDOC5_TAB' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table SetLineNumAtDOC5_tab;
	end if;
	
	create global temporary Table SetLineNumAtDOC5_tab ("RowNum" int,"AbsEntry" int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'SETLINESEQATDOC4_TAB' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table SetLineSeqAtDOC4_tab;
	end if;
	
	create global temporary Table SetLineSeqAtDOC4_tab ("RowNum" int,"DocEntry" int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'SETLINESEQATPMN5_TAB' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table SetLineSeqAtPMN5_tab;
	end if;

	create global temporary Table SetLineSeqAtPMN5_tab ("RowNum" int,"DocNum" int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'UpdateDoubleName_tmp_table1' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "UpdateDoubleName_tmp_table1";
	end if;
	
	create global temporary Table "UpdateDoubleName_tmp_table1" ("seq" int, "gfield" nvarchar(128), "count11" int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'UpdateDoubleName_tmp_table2' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "UpdateDoubleName_tmp_table2";
	end if;
	
	create global temporary Table "UpdateDoubleName_tmp_table2" ("seq" int, "gfield" nvarchar(128));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'UPDATESUBLINENUMOINM_TMP_OINMALLASMBOMEX' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table UpdateSubLineNumOINM_tmp_OINMAllASMBomEx;
	end if;
	
	create global temporary Table UpdateSubLineNumOINM_tmp_OINMAllASMBomEx (TransSeq int, ItemCode nvarchar(5000), Warehouse nvarchar(5000), TransType int, CreatedBy int, DocLineNum int,	CostMethod varchar(5000), GroupNum int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'UPDATESUBLINENUMOINM_TMP_OINMALLASMBOM' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table UpdateSubLineNumOINM_tmp_OINMAllASMBom;
	end if;
	
	create global temporary Table UpdateSubLineNumOINM_tmp_OINMAllASMBom (MinTransSeq int, MaxTransSeq int, ItemCode nvarchar(5000), Warehouse nvarchar(5000), TransType int, CreatedBy int, DocLineNum int, NewSubLineNum int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'UPDATESUBLINENUMOINM_TMP_OINMALLASMBOM_INDEX' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table UpdateSubLineNumOINM_tmp_OINMAllASMBom_Index;
	end if;
	
	create global temporary Table UpdateSubLineNumOINM_tmp_OINMAllASMBom_Index (MinTransSeq int, MaxTransSeq int, Rnk int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'UpgradeDocLineNuminOINMforWKO_boms' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "UpgradeDocLineNuminOINMforWKO_boms";
	end if;
	
	create global temporary Table "UpgradeDocLineNuminOINMforWKO_boms" ("TransSeq" integer, "ItemCode" NVARCHAR(50), "CreatedBy" INTEGER, "TransType" INTEGER, "DocLineNum" SMALLINT);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'UpgradeDocLineNuminOINMforWKO_boms2' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "UpgradeDocLineNuminOINMforWKO_boms2";
	end if;
	
	create global temporary Table "UpgradeDocLineNuminOINMforWKO_boms2" ("TransSeq" integer, "ItemCode" NVARCHAR(50), "CreatedBy" INTEGER, "TransType" INTEGER, "DocLineNum" SMALLINT);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'UPGRADEFILLOILMREVALBASEDDOCSQUANTITY_TMP_NEWQTYTABLE' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table UpgradeFillOILMRevalBasedDocsQuantity_tmp_newqtytable;
	end if;
	
	create global temporary Table UpgradeFillOILMRevalBasedDocsQuantity_tmp_newqtytable (MessageId Integer, NewQty Double);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'UpgradeOIVLNonBaseDocsUpdateAllocation_temp' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table "UpgradeOIVLNonBaseDocsUpdateAllocation_temp";
	end if;
	
	create global temporary Table "UpgradeOIVLNonBaseDocsUpdateAllocation_temp" ("ObjType" NVARCHAR (20), "DocEntry" INTEGER, "LineNum" INTEGER, "StockSum" INTEGER);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'VATSUMCALC_TMPTAB' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table VatSumCalc_TmpTab;
	end if;
	
	create global temporary Table VatSumCalc_TmpTab (RowNum int,DocEntry int,LineNum int,VatPrcnt DECIMAL(21,6),LineTotal DECIMAL(21,6),TotalFrgn DECIMAL(21,6),TotalSumSy DECIMAL(21,6),VatSum DECIMAL(21,6),VatSumFc DECIMAL(21,6),Dfield DECIMAL(21,6),DiscPrcnt DECIMAL(21,6));

		select count(1) into exists1 from "PUBLIC"."M_TABLES"
		where "TABLE_NAME" = 'TMPPSAR_Sales_An_Itm_Sng_Y' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if :exists1 <> 0 then
		drop table "TMPPSAR_Sales_An_Itm_Sng_Y";
	end if;	
	
	Create global temporary Table "TMPPSAR_Sales_An_Itm_Sng_Y" (DocEntry nvarchar(20), DocNum nvarchar(20), SlpCode nvarchar(20), SlpName nvarchar(155),
		CardCode nvarchar(20), CardName nvarchar(100), DocDate DateTime, DocDueDate DateTime, ItemCode nvarchar(50), ItemName nvarchar(100), 
		Quantity DECIMAL(21,6), Price DECIMAL(21,6), Sales DECIMAL(21,6), GrossProfit DECIMAL(21,6), GrossPrcnt DECIMAL(21,6), PriceSys DECIMAL(21,6),
		SalesSys DECIMAL(21,6), GrossProfitSys DECIMAL(21,6), GrossPrcntSys DECIMAL(21,6), LineNum int, SalesForPrcnt DECIMAL(21,6), 
		SalesForPrcntSys DECIMAL(21,6));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TMSP_IVICheckDiskSpace_DiskSpace' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if  :exists1 > 0 then 
		drop table "TMSP_IVICheckDiskSpace_DiskSpace"; 
	end if;
	
	create global temporary table "TMSP_IVICheckDiskSpace_DiskSpace" (drive nchar(1), free_space int);
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TMSP_IVICheckDiskSpace_TableProp' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if  :exists1 > 0 then 
		drop table "TMSP_IVICheckDiskSpace_TableProp"; 
	end if;
	
	create global temporary table "TMSP_IVICheckDiskSpace_TableProp" ( 
           tablename  nvarchar(128) not null, 
           reserved   bigint
    );
	
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TMSP_IVICheckDiskSpace_UTableProp' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if  :exists1 > 0 then 
		drop table "TMSP_IVICheckDiskSpace_UTableProp"; 
	end if;
	
	create global temporary table "TMSP_IVICheckDiskSpace_UTableProp"
        ( 
           tablename  nvarchar(128) not null, 
           reserved   bigint 
        );
		
		
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmSp_IVIInventoryValueDiffsFIFOItems_TMP' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if  :exists1 > 0 then 
		drop table "TmSp_IVIInventoryValueDiffsFIFOItems_TMP"; 
	end if;
	
	create global temporary table "TmSp_IVIInventoryValueDiffsFIFOItems_TMP"("ItemCode" NVARCHAR(50),"ItemName" NVARCHAR(100),"InvnetoryMethod" NVARCHAR(1),"WarehouseCode" NVARCHAR(8),
		"QtyInWH" DECIMAL(21,6),"AvgPrice" DECIMAL(21,6),"QtyInUIVL" DECIMAL(21,6),"StockValue" DECIMAL(21,6),"Qty*StaticPrice" DECIMAL(21,6),"Sum_TransValue" DECIMAL(21,6),
		"Sum_OpenValue" DECIMAL(21,6));
		
	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'TmSp_IVIInventoryValueDiffsMAPnSTDItems_tmp' and "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	if  :exists1 > 0 then 
		drop table "TmSp_IVIInventoryValueDiffsMAPnSTDItems_tmp"; 
	end if;
	
		
	create global temporary table "TmSp_IVIInventoryValueDiffsMAPnSTDItems_tmp"("ItemCode" NVARCHAR(50),"ItemName" NVARCHAR(100),"InvnetoryMethod" NVARCHAR(1),"WarehouseCode" NVARCHAR(8), "QtyInWH" DECIMAL(21,6),
		"AvgPrice" DECIMAL(21,6),"QtyInUIVLM" DECIMAL(21,6),"StockValue" DECIMAL(21,6),"Qty*StaticPrice" DECIMAL(21,6),"Sum_TransValue" DECIMAL(21,6));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPGENERALLEDGEROPENBALANCE_TEMPLDGTABLE' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table CRSPGeneralLedgerOpenBalance_tempLDGTable;
	end if;
	
	create global temporary Table CRSPGeneralLedgerOpenBalance_tempLDGTable (GroupColumn nvarchar(15), AcctCode NVARCHAR(15), AcctName NVARCHAR(100), Father0 NVARCHAR(15), Father1 NVARCHAR(15), Father2 NVARCHAR(15), Father3 NVARCHAR(15), Father4 NVARCHAR(15), Postable NVARCHAR(1), Levels smallint, BalDirect NVARCHAR(4), Debit DECIMAL(21,6), Credit DECIMAL(21,6), TransId int, Line_id int, RefDate timestamp, Year int, Month int, Day int, LineMemo nvarchar(50), Number int);

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPTRAILBALANCEALLACCOUNTS_ACCTLIMITTED' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table CRSPTrailBalanceAllAccounts_AcctLimitted;
	end if;
	
	create global temporary Table CRSPTrailBalanceAllAccounts_AcctLimitted (AcctCode nvarchar(15),AcctName nvarchar(100),AccntntCod nvarchar(12),FrgnName nvarchar(100),Levels int,FatherNum nvarchar(15),Postable nvarchar(1),GroupMask int, GrpLine int, BalDirect nvarchar(4));

	select count(1) into exists1 from "PUBLIC"."M_TABLES"
	where "TABLE_NAME" = 'CRSPTRAILBALANCEALLACCOUNTS_CRSP_TB_AA_TEMP_OUTPUT' and "SCHEMA_NAME" = CURRENT_SCHEMA;
		
	if :exists1 <> 0 then
		drop table CRSPTrailBalanceAllAccounts_CRSP_TB_AA_TEMP_OUTPUT;
	end if;
	
	create global temporary Table CRSPTrailBalanceAllAccounts_CRSP_TB_AA_TEMP_OUTPUT ("AcctCode" NVARCHAR (15), "AcctName" NVARCHAR (100), "AccntntCod" NVARCHAR (12), "FrgnName" NVARCHAR (100), "Levels" int, "FatherNum" NVARCHAR (15), "Postable" NVARCHAR (1), "GroupMask" int, "GrpLine" INTEGER , "BalDirect" NVARCHAR (4), "VirtualGroup" int, "InitCredit" DECIMAL(21,6), "InitDebit" DECIMAL(21,6), "CurrentCredit" DECIMAL(21,6), "CurrentDebit" DECIMAL(21,6), "YearCredit" DECIMAL(21,6), "YearDebit" DECIMAL(21,6), "LCCredit" DECIMAL(21,6), "LCDebit" DECIMAL(21,6), "FC1Credit" DECIMAL(21,6), "FC1Debit" DECIMAL(21,6), "FC2Credit" DECIMAL(21,6), "FC2Debit" DECIMAL(21,6), "FC3Credit" DECIMAL(21,6), "FC3Debit" DECIMAL(21,6), "FC4Credit" DECIMAL(21,6), "FC4Debit" DECIMAL(21,6), "FC5Credit" DECIMAL(21,6), "FC5Debit" DECIMAL(21,6));
END;


