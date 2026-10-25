-- B1 DEPENDS: AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam

CREATE PROCEDURE TmSp_ArcCreateIndexesForArc(in TablePID varchar (5000),out TmpArcTableName_in nvarchar(128), out TmpArcBackTableName_in nvarchar(128))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
	--*With Encryption
AS
TmpArcTableName nvarchar(128);
TmpArcBackTableName nvarchar(128);

TmpArc  nvarchar(128);
TmpArcBack  nvarchar(128);
PairsArc  nvarchar(128);
TmpPairsArc  nvarchar(128);
StrExec varchar(4000);
Index tinyint;

temp_var1 INT;
temp_var2 INT;

-- add id for solve cursor problem in Tmsp_ArcProcess, 2012-04-19
i int;

BEGIN
TmpArcTableName:= :TmpArcTableName_in;
TmpArcBackTableName:= :TmpArcBackTableName_in;
-----------------------------------------------
-- Author:	<Schneider Boaz>
-- Create date: <Oct 26,2008>
-- Description:	<Pre procedure for clustering process>
-- 		Procedure that creates indexes and inserts value instead of null in tmp arc and TDAP 
-- 		(for better performance)
-- 		It will get process id  and create permannent table from temp table
--		('##TDAR_' & '##TDAP_' )
-- TMP tables names have changed due to problems in the code, with the number of characters (5.3.2009):
--	##TMP_ARC_<pid> to ##TDAR_<pid>
--	##ARC_MSSQL_PAIRS_<pid> to ##TDAP_<pid>
-----------------------------------------------

--Parameters validation for security issues
call _TmSp_ValidateSpParam(:TablePID);

--SET NOCOUNT ON;

TmpArc := '##TDAR_' || :TablePID;
TmpArcBack := 'BACKUP_TDAR_B';
TmpPairsArc := '##TDAP_' || :TablePID;
PairsArc := 'TDAP_B';
	
TmpArcTableName := :TmpArc; -- Return value for TmSp_ArcCreateArcClustering
TmpArcBackTableName := :TmpArcBack; -- Return value for TmSp_ArcCreateArcClustering
select count(*) into temp_var1 FROM "PUBLIC"."TABLES" WHERE TABLE_NAME='ARC_TAB';
IF temp_var1>0 then 
	drop table ARC_TAB;
end if;
CREATE GLOBAL TEMPORARY COLUMN TABLE ARC_TAB (TmpArcTableName nvarchar(128), TmpArcBackTableName nvarchar(128));
insert into ARC_TAB(TmpArcTableName,TmpArcBackTableName) values (:TmpArc,:TmpArcBack); 

-- Drop temp table if exists
SELECT COUNT("TABLE_OID") INTO temp_var1 FROM "PUBLIC"."TABLES" WHERE "IS_TEMPORARY" = 'true' and table_name =  :TmpArcBack;
--select "TABLE_OID" INTO temp_var1 from "PUBLIC"."TABLES" WHERE "IS_TEMPORARY" = 'true' and table_name =  :TmpArcBack;
--SELECT 1 INTO temp_var1 FROM  INFORMATION_SCHEMA.Tables where table_name =  :TmpArcBack;
--if :temp_var1 <> 0 then
--	exec('drop table '|| :TmpArcBack);
--end if;

-- Drop temp table if exists
select COUNT("TABLE_OID") INTO temp_var2 from "PUBLIC"."TABLES" WHERE "IS_TEMPORARY" = 'true' and table_name =  :PairsArc;
--select "TABLE_OID" INTO temp_var2 from "PUBLIC"."TABLES" WHERE "IS_TEMPORARY" = 'true' and table_name =  :PairsArc;
--SELECT 1 INTO temp_var2 FROM  INFORMATION_SCHEMA.Tables where table_name =  :PairsArc;
--if :temp_var2 <> 0 then
--	exec('drop table '|| :PairsArc);
--end if;

-- Insert data from temp table to real one 
-- cannot create table by using select * into table_1 from table_2, so...
StrExec:='CREATE TABLE '||:TmpArcBack||' ("DocType" int'||
      ',"DocNum" varchar(100)'||
      ',"DocAbs" int '||
      ',"Total" int '||
      ',"RefDate" date '||
      ',"ClusterId" int '||
      ',"Remarks" varchar(100)'||
      ',"CanArcObj" varchar(100)'||
      ',"CanArcClus" varchar(100)'||
      ',IFNULL("KeySeg1",''NA'') KeySeg1 varchar(100)'||
      ',IFNULL("KeySeg2",''NA'') KeySeg2 varchar(100)'||
      ',IFNULL("KeySeg3",''NA'') KeySeg3 varchar(100)'||
      ',IFNULL("KeySeg4",''NA'') KeySeg4 varchar(100)'||
      ',IFNULL("KeySeg5",''NA'') KeySeg5 varchar(100)'||
      ',IFNULL("KeySeg6",''NA'') KeySeg6 varchar(100)'||
      ',IFNULL("KeySeg7",''NA'') KeySeg7 varchar(100)'||
      ',IFNULL("KeySeg8",''NA'') KeySeg8 varchar(100)'||
      ',IFNULL("KeySeg9",''NA'') KeySeg9 varchar(100)'||
      ',IFNULL("KeySeg10",''NA'') KeySeg10 varchar(100)'||
      ',"Series" varchar(100)'||
      ',"DocSubType" varchar(100)'||
      ',"PIndicator" varchar(100)'||
      ',"Instance" varchar(100)'||
      ',"Segment"  varchar(100))';
exec (:StrExec);
StrExec := 'insert into into '||:TmpArcBack||' SELECT "Line_ID" '||
      ',"DocType"'||
      ',"DocNum"'||
      ',"DocAbs"'||
      ',"Total"'||
      ',"RefDate"'||
      ',"ClusterId"'||
      ',"Remarks"'||
      ',"CanArcObj"'||
      ',"CanArcClus"'||
      ',IFNULL("KeySeg1",''NA'') KeySeg1'||
      ',IFNULL("KeySeg2",''NA'') KeySeg2'||
      ',IFNULL("KeySeg3",''NA'') KeySeg3'||
      ',IFNULL("KeySeg4",''NA'') KeySeg4'||
      ',IFNULL("KeySeg5",''NA'') KeySeg5'||
      ',IFNULL("KeySeg6",''NA'') KeySeg6'||
      ',IFNULL("KeySeg7",''NA'') KeySeg7'||
      ',IFNULL("KeySeg8",''NA'') KeySeg8'||
      ',IFNULL("KeySeg9",''NA'') KeySeg9'||
      ',IFNULL("KeySeg10",''NA'') KeySeg10'||
      ',"Series"'||
      ',"DocSubType"'||
      ',"PIndicator"'||
      ',"Instance"'||
      ',"Segment" '||
      ' from ' || :TmpArc;
--select :StrExec from dummy;
exec (:StrExec);

StrExec := 'CREATE TABLE '||:PairsArc||' ( "Line_ID" int '||
      ',IFNULL(Obj1Type,''NA'')Obj1Type varchar(100)'||
      ',"Obj1Abs" int'||
      ',IFNULL("Obj1Key1",''NA'')Obj1Key1 varchar(100)'||
      ',IFNULL("Obj1Key2",''NA'')Obj1Key2 varchar(100)'||
      ',IFNULL("Obj1Key3",''NA'')Obj1Key3 varchar(100)'||
      ',IFNULL("Obj1Key4",''NA'')Obj1Key4 varchar(100)'||
      ',IFNULL("Obj1Key5",''NA'')Obj1Key5 varchar(100)'||
      ',IFNULL("Obj1Key6",''NA'')Obj1Key6 varchar(100)'||
      ',IFNULL("Obj2Type",''NA'')Obj2Type varchar(100)'||
      ',"Obj2Abs" int'||
      ',IFNULL("Obj2Key1",''NA'')Obj2Key1 varchar(100)'||
      ',IFNULL("Obj2Key2",''NA'')Obj2Key2 varchar(100)'||
      ',IFNULL("Obj2Key3",''NA'')Obj2Key3 varchar(100)'||
      ',IFNULL("Obj2Key4",''NA'')Obj2Key4 varchar(100)'||
      ',IFNULL("Obj2Key5",''NA'')Obj2Key5 varchar(100)'||
      ',IFNULL("Obj2Key6",''NA'')Obj2Key6  varchar(100))';
exec (:StrExec);

StrExec := 'INSERT INTO '||:PairsArc||' SELECT "Line_ID"'||
      ',IFNULL(Obj1Type,''NA'')Obj1Type'||
      ',"Obj1Abs"'||
      ',IFNULL("Obj1Key1",''NA'')Obj1Key1'||
      ',IFNULL("Obj1Key2",''NA'')Obj1Key2'||
      ',IFNULL("Obj1Key3",''NA'')Obj1Key3'||
      ',IFNULL("Obj1Key4",''NA'')Obj1Key4'||
      ',IFNULL("Obj1Key5",''NA'')Obj1Key5'||
      ',IFNULL("Obj1Key6",''NA'')Obj1Key6'||
      ',IFNULL("Obj2Type",''NA'')Obj2Type'||
      ',"Obj2Abs"'||
      ',IFNULL("Obj2Key1",''NA'')Obj2Key1'||
      ',IFNULL("Obj2Key2",''NA'')Obj2Key2'||
      ',IFNULL("Obj2Key3",''NA'')Obj2Key3'||
      ',IFNULL("Obj2Key4",''NA'')Obj2Key4'||
      ',IFNULL("Obj2Key5",''NA'')Obj2Key5'||
      ',IFNULL("Obj2Key6",''NA'')Obj2Key6 from ' || :TmpPairsArc;
--select :StrExec from dummy;
exec (:StrExec);

StrExec := 'ALTER TABLE ' || :TmpArcBack || 'ALTER COLUMN Line_ID int not null';
--select :StrExec from dummy;
exec (:StrExec);

StrExec := 'ALTER TABLE ' || :TmpArcBack || ' ADD  CONSTRAINT [PK_' || :TmpArcBack || '] PRIMARY KEY CLUSTERED '
	'([Line_ID] ASC) '||
	'WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]';
--select :StrExec from dummy;
exec (:StrExec);	

--We don't use Indexes in NewDB procedures	
/*StrExec := 'CREATE NONCLUSTERED INDEX IX_Line_id_doc ON ' || :TmpArcBack ||
	'(	DocType ASC,'||
		'KeySeg1 ASC,'||
		'KeySeg2 ASC,'||
		'KeySeg3 ASC,'||
		'KeySeg4 ASC,'||
		'KeySeg5 ASC,'||
		'KeySeg6 ASC'||
	')WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = OFF) ON [PRIMARY]';

exec (:StrExec);
*/
END;


