-- B1 DEPENDS: AFTER:SP:TmSp_ArcCreateDistinctTmpArc AFTER:SP:TmSp_ArcCreateIndexesForArc AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam

CREATE PROCEDURE TmSp_ArcProcess(in TablePID varchar (5000))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
	--*With Encryption
AS
TmpArcName varchar(5000);
TmpArcTable  nvarchar(128);
TmpArcBackTable  nvarchar(128);
PairsArc  nvarchar(128);
StrExec VARCHAR(4000);
obj1Type  nvarchar(50);
obj2Type  nvarchar(50);
obj1Key1  nvarchar(50);
obj1Key2  nvarchar(50);
obj1Key3  nvarchar(50);
obj1Key4  nvarchar(50);
obj1Key5  nvarchar(50);
obj1Key6  nvarchar(50);

obj2Key1  nvarchar(50);
obj2Key2  nvarchar(50);
obj2Key3  nvarchar(50);
obj2Key4  nvarchar(50);
obj2Key5  nvarchar(50);
obj2Key6  nvarchar(50);

ClusterID1 int;
ClusterID2 int;

temp_var1 INT;
temp_var2 INT;
sql1 nvarchar(1000);
tmpClusterId INT;

-----------------------------20120420--------------------------------------------
--cannot use cursor here, because table "TDAP_B" does not exist, cursor will be replaced by loop temporary table, later in this procedure
--CURSOR  PairCursor FOR
--SELECT  distinct "obj1Type","obj1Key1","obj1Key2","obj1Key3","obj1Key4","obj1Key5","obj1Key6","obj2Type","obj2Key1","obj2Key2","obj2Key3","obj2Key4","obj2Key5","obj2Key6" from TDAP_B; -- edge
---------------------------------------------------------------------------------
next INT := 1;
NumOfCluster int;
NumOfCluster1 int;
index int;
index1 int;
i int;

BEGIN

-----------------------------------------------
-- Author:		<Schneider Boaz>
-- Create date: <Oct 26,2008>
-- Description:	<Procedure for clustering process>
--		Called after filling temporary data tables ##TDAR_<pId>
--		and ##TDAP_<pId>
--		Searches connections between the objects and marks all connected components with the same cluster number.
-- TMP tables names have changed due to problems in the code, with the number of characters (5.3.2009):
--	##TMP_ARC_<pid> to ##TDAR_<pid>
--	##ARC_MSSQL_PAIRS_<pid> to ##TDAP_<pid>
-----------------------------------------------

	--Vocabulary:
	--Vertex - is an object, for example Invoice No 50
	--Cluster - is group og objects connected with each others
	--Edge - Two connected objects
	--Note! The procedure based on the Korman's "Connected components" algorythm.

	-- Call to store procedure that do distinct to duplicate rows in tmp arc
	
--Parameters validation for security issues
call _TmSp_ValidateSpParam(:TablePID);

TmpArcName := '##TDAR_' || :TablePID;

call TmSp_ArcCreateDistinctTmpArc (:TmpArcName);--ODKOMENTOVAT

	
--SET NOCOUNT ON;

-- Call to procedure that create indexes and insert value instead of null in tmp arc and TDAP 
-- (for better performance)
-- It will get process id  and create permannent table from temp table ('##TDAR_' & '##TDAP_' )
delete from "TMP_ArcProcess_WithId";
--table created in TmSp_ArcCreateIndexesForArc, so have to created first in this procedure.
create global temporary column table BACKUP_TDAR_B(Line_ID int,DocType varchar(10),"DocNum" int,"DocAbs" int, "Total" int,"RefDate" date,"ClusterId" int,"Remarks" varchar(100),"CanArcObj" varchar(10),
     "CanArcClus" int,KeySeg1 varchar(100),KeySeg2 varchar(100),KeySeg3 varchar(10),KeySeg4 varchar(10),KeySeg5 varchar(10),KeySeg6 varchar(10),
	 KeySeg7 varchar(10),KeySeg8 varchar(10),KeySeg9 varchar(10),KeySeg10 varchar(10),"Series" varchar(100),"DocSubType" int,PIndicator varchar(10),
	 "Instance" varchar(10),Segment varchar(10));
	 
call TmSp_ArcCreateIndexesForArc (:TablePID,:TmpArcTable,:TmpArcBackTable);--ODKOMENTOVAT
sql1 :='select top 1 "TMPARCTABLENAME" into TmpArcTable from ARC_TAB';
exec(:sql1);
sql1 :='select top 1 "TMPARCBACKTABLENAME" into TmpArcBackTable  from ARC_TAB';
exec(:sql1);
--insert into TAB(TmpArcTableName,TmpArcBackTableName) values (1,2);
--select * from TAB;

PairsArc := 'TDAP_B';
	
obj1Key1 := 0;
obj1Key2 := 0;
obj1Key3 := 0;
obj1Key4 := 0;
obj1Key5 := 0;
obj1Key6 := 0;
obj2Key1 := 0;
obj2Key2 := 0;
obj2Key3 := 0;
obj2Key4 := 0;
obj2Key5 := 0;
obj2Key6 := 0;

ClusterID1 := 0;
ClusterID2 := 0;
-- Drop the table if exists


select "TABLE_OID" into temp_var1 from "PUBLIC"."TABLES" WHERE table_name =  N'#Cluster';
IF :temp_var1 <> 0  THEN
	drop table "#Cluster";
end if;
select "TABLE_OID" into temp_var1 from "PUBLIC"."TABLES" WHERE table_name = N'#Cluster1';
IF :temp_var2 <> 0  THEN
	drop table "#Cluster1";
end if;
--IF  EXISTS (SELECT 1 FROM  tempdb.INFORMATION_SCHEMA.Tables where table_name =  N'#Cluster') drop table #Cluster
--IF  EXISTS (SELECT 1 FROM  tempdb.INFORMATION_SCHEMA.Tables where table_name =  N'#Cluster1') drop table #Cluster1


-- Cluster table holds 
-- vertexId  : A unique ID for the vertex. This is the line_Id from TEMP_ARC
-- clusterId : A unique identifier for the cluster - This is the output of the Algorithem
--DROP TABLE Cluster; 
Create Table Cluster ("ClusterID" Int,"vertexid" int);
Create Table Cluster1 ("ClusterID" Int,"vertexid" int);
--  Initiliaze Cluster with cluster id 
exec('insert into "Cluster" select LINE_ID,0  from ' || :TmpArcBackTable );

/***************************************************************************************************************/
-- Clustering Step 1:  
--------------------------------------------------------
-- run on every pair and insert into 'Cluster' table: 
-- For each vertex (left,right) in the pair:
-- vertexid  <-- the line_Id ( from ##TDAR_<PID> ) of the vetex
-- Line_id is taken from the ##TDAR_<pid> table as a consiquent  enumarator of the records
-- clusterID <-- MIN (left.line_ID, right.line_Id) 
-- The algorythm: we take a pair and create a head of the cluster. At the end all the components will point to the same head.

--CURSOR  PairCursor FOR
--SELECT  distinct "obj1Type" into obj1Type,"obj1Key1" into obj1Key1,"obj1Key2","obj1Key3","obj1Key4","obj1Key5","obj1Key6","obj2Type","obj2Key1","obj2Key2","obj2Key3","obj2Key4","obj2Key5","obj2Key6" from TDAP_B; -- edge
sql1 :='select count(*) into next from TDAP_B';
exec (:sql1);

sql1 :='CREATE SEQUENCE TEMP_TDAP_B_ID INCREMENT BY 1 START WITH 1';
exec (:sql1);

sql1 :='INSERT INTO "TMP_ArcProcess_WithId" SELECT T2.*,TEMP_TDAP_B_ID.NEXTVAL AS ID FROM (SELECT distinct "obj1Type","obj1Key1","obj1Key2","obj1Key3","obj1Key4","obj1Key5","obj1Key6","obj2Type","obj2Key1","obj2Key2","obj2Key3","obj2Key4","obj2Key5","obj2Key6" FROM TDAP_B) T2';
exec (:sql1);


/* Cursor declartion */
--DECLARE PairCursor CURSOR LOCAL FORWARD_ONLY STATIC READ_ONLY    FOR
--OPEN PairCursor;
--FETCH PairCursor INTO obj1Type,obj1Key1,obj1Key2,obj1Key3,obj1Key4,obj1Key5,obj1Key6,obj2Type,obj2Key1,obj2Key2,obj2Key3,obj2Key4,obj2Key5,obj2Key6;

IF :next > 0 THEN
	
	--FETCH PairCursor INTO obj1Type,obj1Key1,obj1Key2,obj1Key3,obj1Key4,obj1Key5,obj1Key6,obj2Type,obj2Key1,obj2Key2,obj2Key3,obj2Key4,obj2Key5,obj2Key6;
	--IF PairCursor::NOTFOUND THEN
	--	next := -1;
	--END IF;
	
	FOR i IN 0..next do
		SELECT "obj1Type","obj1Key1","obj1Key2","obj1Key3","obj1Key4","obj1Key5","obj1Key6","obj2Type","obj2Key1","obj2Key2","obj2Key3","obj2Key4","obj2Key5","obj2Key6"
		into obj1Type,obj1Key1,obj1Key2,obj1Key3,obj1Key4,obj1Key5,obj1Key6,obj2Type,obj2Key1,obj2Key2,obj2Key3,obj2Key4,obj2Key5,obj2Key6
		from "TMP_ArcProcess_WithId"
		where id=:i;
		select IFNULL(LINE_ID,0) into ClusterID1 from BACKUP_TDAR_B where DocType = :obj1Type and KeySeg1 = :obj1Key1 and KeySeg2 = :obj1Key2 and KeySeg3 = :obj1Key3 and KeySeg4 = :obj1Key4 and KeySeg5 = :obj1Key5 and KeySeg6 = :obj1Key6;
		select IFNULL(LINE_ID,0) into ClusterID2 from BACKUP_TDAR_B where DocType = :obj2Type  and KeySeg1 = :obj2Key1 and KeySeg2 = :obj2Key2 and KeySeg3 = :obj2Key3 and KeySeg4 = :obj2Key4 and KeySeg5 = :obj2Key5 and KeySeg6 = :obj2Key6;
		
		if :ClusterID1 < :ClusterID2 then
			insert into Cluster values(:ClusterID1,:ClusterID1);
			insert into Cluster values(:ClusterID1,:ClusterID2);
			
		ELSE
			insert into Cluster values(:ClusterID2,:ClusterID1);
			insert into Cluster values(:ClusterID2,:ClusterID2);
			
		END IF;
	end for;
  --FETCH PairCursor INTO obj1Type,obj1Key1,obj1Key2,obj1Key3,obj1Key4,obj1Key5,obj1Key6,obj2Type,obj2Key1,obj2Key2,obj2Key3,obj2Key4,obj2Key5,obj2Key6;
END IF;

--CLOSE PairCursor;
--DEALLOCATE PairCursor;

-- Create index on Cluster - for better performance

---------------------------------------------------------

-- delete all 1,1 ;  2,2 etc.  so it will run in recursive

delete from Cluster where "ClusterID" = "vertexid";


select distinct * from Cluster into Cluster1;
--truncate table Cluster; TRUNCATE works only for temporary tables
insert into Cluster select * from Cluster1;
delete from Cluster1;

delete from "TMP_ArcProcess_WithId";

-- End of step 1
/***************************************************************************************************************/


/***************************************************************************************************************/

-- Step 2: Building clusters recursively:
---------------------------------------------------------
--
-- 1. For each clusterID
--       a. select all pairs that have this cluster ID
--       b. Union with recursive results - pairs that are linked indirectly to this cluster with the cluster ID
-- Example:
-- if we have pairs:
--             Cluster id |  Vertex
--             --------------------
--                  2     |   3
--                  2     |   6 
--                  3     |   4 
--       sub step (a.) will add to the result the pairs: 2,3 ; 2,6. sub step b will add the pair: 2,4 
----------------------------------------------------------------------------------------------------------------

-- recursive run to find child for each cluster
index := 1;

select count(1) into NumOfCluster from Cluster Where "vertexid" = 0;

delete from "TMP_ArcProcess_ClusterChilds";

while :index <= :NumOfCluster do

		insert into "TMP_ArcProcess_ClusterChilds" ("ClusterID","vertexid")
		SELECT  "ClusterID", "vertexid" -- Ancor part
			FROM Cluster e WHERE "ClusterID" = :index 
			UNION ALL
			SELECT  e."ClusterID", e."vertexid" -- Recursive part
			FROM Cluster e
			inner JOIN "TMP_ArcProcess_ClusterChilds" d
			ON 
			e."ClusterID" = d."vertexid";
			
		SELECT distinct :index, "vertexid"
		FROM "TMP_ArcProcess_ClusterChilds" into Cluster;

		index := :index+1;

end while;

--select * from "TMP_ArcProcess_ClusterChilds";
delete from "TMP_ArcProcess_ClusterChilds";


/***************************************************************************************************************/


-- Get rid of redundant pairs (pairs that have vertex with low cluster id i.e 2,3 2,6 3,6 get rid from 3,6)
-- insert  them to new table Cluster1 
 --insert  into #Cluster1
 select distinct  c1."ClusterID",c1."vertexid"
 from  Cluster c1
 left  join Cluster c2
 on c1."ClusterID" = c2."vertexid"
 where c2."ClusterID" is null into Cluster1;
---------------------------------------------------------------------------------


---------------------------------------------------------------------------------
-- Add index on Cluster1 - For better performance -------------------------------

---------------------------------------------------------------------------------


----------------------------------------------------------------------------------
-- change vertexid from 0 to point to itself, so it will be part of the cluster
update Cluster1
set "vertexid" = "ClusterID"
where "vertexid" = 0;
----------------------------------------------------------------------------------

-- End of step 2  

/***************************************************************************************************************/

-- Step 3: Update left vertex
----------------------------------------------------------------------------------
	

-- update cluster id of pairs that connect with left vertex i.e 2,6 3,6 --> 2,6 2,3
-- Do it while there is a connection with left vertex

-- We do it first for each cluster and then do a loop to cover those that was left - it be done in two steps for performance 	


index1 := 1;

NumOfCluster1 := :NumOfCluster;

while :index1 <= :NumOfCluster1 do

	update Cluster1 c0
	set c0."ClusterID" = (select c4.clusterid1
	from (select distinct c1."ClusterID" clusterid1,c2."ClusterID"  clusterid2  from Cluster1 c1
	inner join Cluster1 c2	on c1."vertexid" = c2."vertexid" and c1."ClusterID" < c2."ClusterID" and c1."vertexid" > 0) c4
	where c0."ClusterID" = c4.clusterid2
	and c4.clusterid1 = :index1)
	;


index1 := :index1 + 1;

end while;

select top 1 1 into temp_var1  from Cluster1 c1
	inner join Cluster1 c2
	on c1."vertexid" = c2."vertexid"
	and c1."ClusterID" < c2."ClusterID"
	and c1."vertexid" > 0;

While :temp_var1 <> 0 do
	select top 1 c1."ClusterID" into i from Cluster1 c1
	inner join Cluster1 c2
	on c1."vertexid" = c2."vertexid"
	and c1."ClusterID" < c2."ClusterID"
	and c1."vertexid" > 0 ;

	update Cluster1 c0
	set c0."ClusterID" = (select c4.clusterid1
	from (select distinct c1."ClusterID" clusterid1, c2."ClusterID"  clusterid2  from Cluster1 c1
	inner join Cluster1 c2	on c1."vertexid" = c2."vertexid" and c1."ClusterID" < c2."ClusterID" and c1."vertexid" > 0 ) c4
	where c0."ClusterID" = c4.clusterid2
	and  c4.clusterid1 = :i);


	select top 1 1 into temp_var1  from Cluster1 c1
	inner join Cluster1 c2
	on c1."vertexid" = c2."vertexid"
	and c1."ClusterID" < c2."ClusterID"
	and c1."vertexid" > 0;
end while;
-- update clusterid to start from 0

delete from "TMP_ArcProcess_RowNums";

sql1 := 'CREATE SEQUENCE my_s INCREMENT BY 1 START WITH 1';
exec ( :sql1);
sql1 := 'INSERT INTO "TMP_ArcProcess_RowNums" SELECT "ClusterID", my_s.NEXTVAL from Cluster1 group by "ClusterID"';
exec ( :sql1);
sql1 := 'DROP SEQUENCE my_s';
exec ( :sql1);

update Cluster1 c0
set c0."ClusterID" = (
select rownum from 
(select c1."ClusterID",count(*) as rownum from Cluster1 c1 group by c1."ClusterID"
) c2
where c0."ClusterID" = c2."ClusterID"
);

delete from "TMP_ArcProcess_RowNums";

-- End of step 3  
/***************************************************************************************************************/


/*****************************************************/
-- Step 4 Update  ##TDAR_ with final result

exec(' update ' || :TmpArcTable || ' set clusterid = C1."CLUSTERID" from Cluster C1 where line_id = C1."VERTEXID"');
/*****************************************************/

 --- Drop tables that was created for ARC process
 exec ('drop table cluster');
 exec ('drop table cluster1');
 exec ('drop table ' || :TmpArcBackTable );
 exec ('drop table ' || :PairsArc );
END;


