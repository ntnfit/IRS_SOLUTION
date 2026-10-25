-- B1 DEPENDS: BEFORE:PT:CREATE_OBJECTS

CREATE procedure TmSp_UpgradeElectronicInvoiceMX()
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS

table_name   nvarchar(100);
object_abbev nvarchar(100);
action_type  nvarchar(100);
isAPdocument nvarchar(1);
	
isExists     int;
counter      int;
max_count    int;
nextAbsEntry int;
countUDF     int;

sqlStr       nvarchar(4000);
sqlStrWhere  nvarchar(4000);
sqlUDF_src   nvarchar(100);
sqlUDF_tgt   nvarchar(100);

BEGIN
	counter   := 0;
	max_count := 6;

	select count(*) into countUDF from CUFD where "TableID" = 'ECM2' and "AliasID" IN ('B1SYS_SignMsg', 'B1SYS_SignDigest');
	IF (:countUDF = 2) THEN
		sqlUDF_tgt := ', "U_B1SYS_SignMsg", "U_B1SYS_SignDigest"';
		sqlUDF_src := ', t0."SignMsg", t0."SignDigest"';
	ELSE
		sqlUDF_tgt := '';
		sqlUDF_src := '';
	END IF;

	-- get autoKey for ECM2
	--    - we do not need check ECM2.AbsEntry, it was not used in MX until now
	select count(*) into isExists from ONNM where "ObjectCode" = '1730000000';
	IF :IsExists <> 0 THEN
		select ifnull("AutoKey", 1) into nextAbsEntry from ONNM where "ObjectCode" = '1730000000';
	ELSE
		nextAbsEntry := 1;		
	END IF;
	nextAbsEntry := nextAbsEntry - 1;

	WHILE (counter < :max_count) DO
		IF :counter = 0 THEN
			table_name   := 'OINV';
			object_abbev := 'IN';
			action_type  := 'D';	-- LAT_DOCUMENT_AR
			isAPdocument := 'N';
		ELSEIF :counter = 1 THEN
			table_name   := 'ORIN';
			object_abbev := 'CN';
			action_type  := 'D';	-- LAT_DOCUMENT_AR
			isAPdocument := 'N';
		ELSEIF :counter = 2 THEN
			table_name   := 'ODPI';
			object_abbev := 'DT';
			action_type  := 'D';	-- LAT_DOCUMENT_AR
			isAPdocument := 'N';
		ELSEIF :counter = 3 THEN
			table_name   := 'OPCH';
			object_abbev := 'PU';
			action_type  := 'P';	-- LAT_DOCUMENT_AP
			isAPdocument := 'Y';
		ELSEIF :counter = 4 THEN
			table_name   := 'ORPC';
			object_abbev := 'PC';
			action_type  := 'P';	-- LAT_DOCUMENT_AP
			isAPdocument := 'Y';
		ELSEIF :counter = 5 THEN
			table_name   := 'ODPO';
			object_abbev := 'DT';
			action_type  := 'P';	-- LAT_DOCUMENT_AP
			isAPdocument := 'Y';
		END IF;

		/*
			OINV.EDocStatus
			'N' New
			'P' Pending
			'S' Sent
			'E' Error
			'C' OK

			ECM2.ActStatus
			'N' New
			'P' Pending
			'E' Error
			'O'	OK
			'S'	Sent
			'R'	Document Error
			'W'	Waiting
			'A'	Authorized
			'I'	In Process
			'J'	Rejected
			'D'	Denied
			'C'	Canceled
			'B'	Aborted
		*/

		IF :isAPdocument = 'N' THEN
			sqlStr := '
				 insert into ECM2
				   ("AbsEntry",
				    "Code", "ActType", "ActStatus",
				    "ActEnv", "BPLId", "ObjectID", "ReportID", 
				    "SrcObjType", "SrcObjAbs", "Cancel", "GenType",
				    "UserSign", "CreateDate", "CreateTS", "UserSign2", "UpdateDate", "UpdateTS"' || :sqlUDF_tgt || ') 
				 select 
				   ROW_NUMBER() OVER (Order by t0."DocEntry") + ' || CAST(:nextAbsEntry as nvarchar) || ', 
				   ''M'', ''' || :action_type || ''', (CASE WHEN t0."CANCELED" = ''Y'' THEN ''C'' WHEN t0."EDocStatus" = ''C'' THEN ''A'' ELSE t0."EDocStatus" END), 
				   (CASE WHEN t0."EDocTest" = ''Y'' THEN 0 ELSE 1 END), t0."BPLId", ''' || :object_abbev || ' '' || CAST(t0."DocNum" as nvarchar), t0."EDocNum", 
				   t0."ObjType", t0."DocEntry", t0."CANCELED", t0."EDocGenTyp",
				   t0."UserSign", t0."CreateDate", t0."CreateTS", t0."UserSign2", t0."UpdateDate", t0."UpdateTS"' || :sqlUDF_src || ' 
				 from ' || :table_name || ' t0 
				   inner join NNM1 t1 on t0."Series" = t1."Series"
					left join ecm2 t5 on t5."SrcObjType" = t0."ObjType" and t5."SrcObjAbs" = t0."DocEntry" 
				where t1."IsDigSerie"  = ''Y''
					and t0."CANCELED"   IN (''Y'', ''N'')
					and t0."EDocGenTyp" IN (''G'', ''L'')
					and t5."AbsEntry" is NULL ';
		ELSE
			sqlStr := '
				 insert into ECM2
				   ("AbsEntry",
				    "Code", "ActType", "ActStatus",
				    "ActEnv", "BPLId", "ObjectID", "ReportID", 
				    "SrcObjType", "SrcObjAbs", "Cancel", "GenType",
				    "UserSign", "CreateDate", "CreateTS", "UserSign2", "UpdateDate", "UpdateTS"' || :sqlUDF_tgt || ') 
				 select 
				   ROW_NUMBER() OVER (Order by t0."DocEntry") + ' || CAST(:nextAbsEntry as nvarchar) || ', 
				   ''M'', ''' || :action_type || ''', ''O'', 
				   (CASE WHEN t0."EDocTest" = ''Y'' THEN 0 ELSE 1 END), t0."BPLId", ''' || :object_abbev || ' '' || CAST(t0."DocNum" as nvarchar), t0."EDocNum", 
				   t0."ObjType", t0."DocEntry", t0."CANCELED", t0."EDocGenTyp",
				   t0."UserSign", t0."CreateDate", t0."CreateTS", t0."UserSign2", t0."UpdateDate", t0."UpdateTS"' || :sqlUDF_src || ' 
				 from ' || :table_name || ' t0 
				   inner join NNM1 t1 on t0."Series" = t1."Series"
					left join ecm2 t5 on t5."SrcObjType" = t0."ObjType" and t5."SrcObjAbs" = t0."DocEntry" 
				where ifnull(t0."EDocNum", '''') <> ''''
					and t0."CANCELED"   IN (''Y'', ''N'')
					and t5."AbsEntry" is NULL ';	 -- dont add data to ECM2 in case document already exists
		END IF;
		Exec(:sqlStr);
		
		sqlStr := ' 
				 insert into ECM3 
				   ("AbsEntry", 
				    "LogNum", "LogType", "ExportFmt",
				    "LogMessage", "LogData", 
				    "UserSign", "CreateDate", "CreateTS", "UserSign2", "UpdateDate", "UpdateTS")
				 select 
				   t5."AbsEntry", 
				   1, ''S'', t0."EDocExpFrm",
				   (CASE when "EDocErrCod" IS NULL then '''' ELSE "EDocErrCod" || '': '' || SUBSTRING("EDocErrMsg", 1, 245) END), t0."EDocCntnt", 
				   t0."UserSign", t0."CreateDate", t0."CreateTS", t0."UserSign2", t0."UpdateDate", t0."UpdateTS" 
				 from ' || :table_name || ' t0 
				   inner join NNM1 t1 on t0."Series" = t1."Series"
					left join ecm2 t5 on t5."SrcObjType" = t0."ObjType" and t5."SrcObjAbs" = t0."DocEntry" 
					left join ecm3 t6 on t6."AbsEntry" = t5."AbsEntry" ';
		
		IF :isAPdocument = 'N' THEN
			sqlStrWhere := 'where t1."IsDigSerie"  = ''Y''
				and t0."CANCELED"   IN (''Y'', ''N'')
				and t0."EDocGenTyp" IN (''G'', ''L'')
				and t6."AbsEntry" is NULL ';
		ELSE
			sqlStrWhere := 'where ifnull(t0."EDocNum", '''') <> ''''
				and t0."CANCELED"   IN (''Y'', ''N'')
				and t6."AbsEntry" is NULL ';	 -- dont add data to ECM3 in case document already exists
		END IF;
		sqlStr := CONCAT(:sqlStr, :sqlStrWhere);
		Exec(:sqlStr);

		-- update series to digital for AP Docs
		IF :isAPdocument = 'Y' THEN
				sqlStr := 'update nnm1 set nnm1."IsDigSerie" = ''Y'' from nnm1
					inner join ' || :table_name || ' opch on opch."Series" = nnm1."Series" and nnm1."ObjectCode" = opch."ObjType"
					inner join ecm2 on ecm2."SrcObjAbs" = opch."DocEntry" and ecm2."SrcObjType" = opch."ObjType"
					where nnm1."IsDigSerie" = ''N''';

				EXEC(:sqlStr);
		END IF;

		counter := :counter + 1;
		select ifnull(max("AbsEntry"), 0) + 1 into nextAbsEntry from ECM2;
	END WHILE;

	update ONNM set "AutoKey" = :nextAbsEntry where "ObjectCode" = '1730000000';  -- for ECM Electronic Communication

END;


