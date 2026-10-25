
CALL APZ_EXISTSPROC('APZ_SalaryParameters_Saves');

GO

Create PROCEDURE APZ_SalaryParameters_Saves
AS 
BEGIN
	Declare docId bigint = 0;
	Declare isCode bigint = 0;
	Declare Udfcols Nvarchar(500) = '';
	
	create local temporary table #APZ_OHSPTmp as (select top 0 * from APZ_OHSP);
	exec 'drop table #APZ_OHSPTmp;';
	exec 'create local temporary table #APZ_OHSPTmp as (select * from "#APZ_OHSP");';
	
	If Exists( Select 1 From #APZ_OHSPTmp a Where a."ValidFrom" > a."ValidTo") 
	Then
		Select 121 ,'Valid From cannot be later than Valid To' from dummy;
		Return;
	End if;
	
	Select Count(1) into isCode 
	From #APZ_OHSPTmp a INNER JOIN #APZ_OHSPTmp b ON ((b."ValidFrom" between a."ValidFrom" and a."ValidTo") 
		or (b."ValidTo" between a."ValidFrom" and a."ValidTo") 
		or (a."ValidFrom" between b."ValidFrom" and b."ValidTo") 
		or (a."ValidTo" between b."ValidFrom" and b."ValidTo")) and a."Code" != b."Code";
	
	If (Ifnull(isCode,0)=1)
	Then
		Select 123 ,'Salary Parameters is overlapping with an existing Salary Parameters' from dummy;
		Return;
	End if;
	
	--Get UDF
	SELECT IFNULL((SELECT string_agg('"' || COLUMN_NAME || '"',',') 
	FROM TABLE_COLUMNS where SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = 'APZ_OHSP' AND COLUMN_NAME like 'U_%'),'') Into Udfcols From DUMMY;
	
	Delete From APZ_OHSP;
	If (IFNULL(:Udfcols,'') = '') Then
		EXECUTE IMMEDIATE 'Insert Into APZ_OHSP SELECT ROW_NUMBER() OVER() AS "Code" , "ValidFrom", "ValidTo", "BasicSal", "BasicCur", 
			"PITSal", "PITCur", "AmtRelat", "AmtRelatCur" FROM #APZ_OHSPTmp;';
	Else
		EXECUTE IMMEDIATE 'Insert Into APZ_OHSP SELECT ROW_NUMBER() OVER() AS "Code" , "ValidFrom", "ValidTo", "BasicSal", "BasicCur", 
			"PITSal", "PITCur", "AmtRelat", "AmtRelatCur",' ||:Udfcols || ' FROM #APZ_OHSPTmp;';
	End If;
	
	Select 0, '' From dummy;
END;