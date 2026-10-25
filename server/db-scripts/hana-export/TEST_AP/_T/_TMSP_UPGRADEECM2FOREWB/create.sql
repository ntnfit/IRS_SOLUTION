CREATE PROCEDURE _TmSp_UpgradeECM2ForEWB
(	IN DocTable nvarchar(8),
	IN ProtocolCode nvarchar(1),
	IN ActType nvarchar(1),
	IN ObjName nvarchar(2)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
maxEntry int := 0;
isExist int := 0;
execStr nvarchar(4000);
BEGIN

	select ifnull(max("AbsEntry"), 0) into maxEntry from ECM2;
	
	SELECT COUNT(1) into isExist FROM SYS.TABLES WHERE "SCHEMA_NAME" = CURRENT_SCHEMA AND "TABLE_NAME" = 'ECM2_EWB';
	if (:isExist > 0) then
		execStr := 'drop table "ECM2_EWB"';
		exec(:execStr);
	end if;

	create table "ECM2_EWB" as
	(select * from "ECM2" where 1<>1);

	execStr := 'insert into ECM2_EWB ("AbsEntry", "Code", "ActType", "ActDesc", "ActStatus", "IsRemoved", "BPLId", "ObjectID", "SrcObjType", "SrcObjAbs", "Cancel", "GenType", "UserSign")
				select row_number() over(ORDER BY t0."DocEntry"), '''|| :ProtocolCode || ''', ''' || :ActType || ''', ''Upgrade'', ''W'', ''N'', ifnull(t0."BPLId", 0), concat(''' || :ObjName ||' '', t0."DocNum"),
				(case when ''' || :DocTable ||''' = ''ODRF'' then 112 else t0."ObjType" end), t0."DocEntry", t0."CANCELED", t0."EWBGenType", t0."UserSign"
				from ' || :DocTable || ' t0
				inner join NNM1 t1 on t0."Series" = t1."Series" and t1."IsDigSerie" = ''Y''
				where ("EWBGenType" in (''G'', ''L'')
				and "CANCELED" <> ''C''
				and "DocType" = ''I''
				and not exists (select 1 from ECM2 t2 where t2."SrcObjType" = (case when ''' || :DocTable ||''' = ''ODRF'' then 112 else t0."ObjType" end)
															and t2."SrcObjAbs" = t0."DocEntry" and t2."Code" = '''|| :ProtocolCode ||'''))
				';
	exec(:execStr);
	
	update ECM2_EWB set "AbsEntry" = "AbsEntry"  + :maxEntry;
	
	if (:DocTable = 'ODRF')	then
		update t0 set "ActType" = (case when "SrcObjType" in (13, 14, 15, 16) then 'F' else 'E' end),
					  "ObjectID" = concat((case when t1."ObjType" = 13 then 'IN'
									   when t1."ObjType" = 14 then 'CN'
									   when t1."ObjType" = 15 then 'DN'
									   when t1."ObjType" = 16 then 'RE'
									   when t1."ObjType" = 18 then 'PU'
									   when t1."ObjType" = 19 then 'PC'
									   when t1."ObjType" = 20 then 'PD'
									   when t1."ObjType" = 21 then 'PR' end), "ObjectID")
		from ECM2_EWB t0
		inner join ODRF t1 on t0."SrcObjAbs" = t1."DocEntry"
		where "SrcObjType" = 112;
	end if;
	
	insert into ECM2 select * from ECM2_EWB;
	
	execStr := 'drop table "ECM2_EWB"';
	exec(:execStr);
END


