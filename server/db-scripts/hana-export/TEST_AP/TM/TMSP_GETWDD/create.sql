-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_GetWDD(
in producerFrom		nvarchar(156),
in producerTo		nvarchar(156),
in approverFrom		nvarchar(156),
in approverTo		nvarchar(156),
in templateFrom		nvarchar(21),
in templateTo		nvarchar(21),
in dateFrom			nvarchar(10),
in dateTo			nvarchar(10),
in cardFrom			nvarchar(16),
in cardTo			nvarchar(16),
in totalFrom		nvarchar(41),
in totalTo			nvarchar(41),
in statusWait		nchar(2),
in statusApproved		nchar(2),
in statusRejected		nchar(2),
in statusProduced		nchar(2),
in statusCanceled		nchar(2),
in statusApprProduced	nchar(2),
in objectsList			nvarchar(4000),
in numObjects		int,
in docTable			nchar(4),
in absEntry			int
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
qString 	nvarchar(2048);
hadCond  	smallint;
hadObject  	smallint;
hadStatus  	smallint;
ii	  		int;
str1 		nvarchar (2048);
isInventoryTakingObj smallint;
begin
	hadCond := 0;
	hadObject := 0;
	hadStatus := 0;

	if (:docTable<>'OICD' and :docTable<>'OIPD' and :docTable<>'OIOD') then
		isInventoryTakingObj := 0;
	else
		isInventoryTakingObj := 1;
	end if;
		
	qString := 'select T0.*  from ' ||
	'OWDD T0 inner join OUSR T1 on T0."OwnerID" = T1."INTERNAL_K" ' ||
	'inner join WDD1 T2 on T0."WddCode" = T2."WddCode" ' ||
	'inner join OUSR T3 on T2."UserID" = T3."INTERNAL_K" ' ||
	'inner join OWTM T4 on T0."WtmCode" = T4."WtmCode" ' ||
	'inner join ' || :docTable || ' T5 on ';

	if (:docTable='ODRF') then
		qString := :qString || ' T0."DraftType" = N''112'' and T0."DraftEntry" = T5."DocEntry" and T0."IsDraft" = ''Y''';
	elseif (:docTable='OPDF') then
		qString := :qString || ' T0."DraftType" = N''140'' and T0."DraftEntry" = T5."DocEntry" and T0."IsDraft" = ''Y''';
	elseif (:docTable='OICD') then
		qString := :qString || ' T0."DraftType" = N''1470000109'' and T0."DraftEntry" = T5."DocEntry" and T0."IsDraft" = ''Y''';
	elseif (:docTable='OIOD') then
		qString := :qString || ' T0."DraftType" = N''1470000131'' and T0."DraftEntry" = T5."DocEntry" and T0."IsDraft" = ''Y''';
	elseif (:docTable='OIPD') then
		qString := :qString || ' T0."DraftType" = N''1470000136'' and T0."DraftEntry" = T5."DocEntry" and T0."IsDraft" = ''Y''';
	elseif (:docTable='OOAT') then
		if(:objectsList='1250000026') then
			qString := :qString || ' T0."ObjType" = N''1250000026'' and T5."ObjType"= N''1250000025'' and T5."BpType" =N''C'' and  T0."DraftEntry" = T5."AbsID" and T0."IsDraft" = ''N''';
		else
			qString := :qString || ' T0."ObjType" = N''1250000027'' and T5."ObjType"= N''1250000025'' and T5."BpType" =N''S'' and  T0."DraftEntry" = T5."AbsID" and T0."IsDraft" = ''N''';
		end if;
	else
		qString := :qString || ' T0."ObjType" = T5."ObjType" and T0."DocEntry" = T5."DocEntry" and T0."IsDraft" = ''N''';
	end if;

	if (:absEntry>0) then 
		hadCond := 1;

		if (:docTable='ODRF' or :docTable='OPDF' or :isInventoryTakingObj=1) then
			qString := :qString || ' where T0."DraftEntry" = ' || TO_CHAR(:absEntry) || ' and T0."IsDraft" = ';
			qString := :qString || 'N''Y'' ';
		else
			qString := :qString || ' where T0."DocEntry" = ' || TO_CHAR(:absEntry) || ' and T0."IsDraft" = ';
			qString := :qString || 'N''N'' ';
		end if;
	end if;

	--------------------------------------
	/*	CONDITIONS	*/
	--------------------------------------
	-- PRODUCER
	if (LENGTH(ltrim(rtrim(:producerFrom))) <> 0) then
		if (:hadCond<>1) then
			qString := :qString || '  where ';
			hadCond := 1;
		else
			qString := :qString || ' and ';
		end if;
		
		qString := :qString || ' ltrim(rtrim(T1."U_NAME")) >= ' || 'N''' || ltrim(rtrim(:producerFrom)) || '''';
	end if;

	if (LENGTH(ltrim(rtrim(:producerTo))) <> 0) then
		if (:hadCond<>1) then
			qString := :qString || ' where ';
			hadCond := 1;
		else
			qString := :qString || ' and ';
		end if;
		
		qString := :qString || ' ltrim(rtrim(T1.U_NAME)) <= ' || 'N''' || ltrim(rtrim(:producerTo)) || '''';
	end if;

	-- APPROVER
	if (LENGTH(ltrim(rtrim(:approverFrom))) <> 0) then
		if (:hadCond<>1) then
			qString := :qString || ' where ';
			hadCond := 1;
		else
			qString := :qString || ' and ';
		end if;
		
		qString := :qString || ' ltrim(rtrim(T3."U_NAME")) >= ' || 'N''' || ltrim(rtrim(:approverFrom)) || '''';
	end if;


	if (LENGTH(ltrim(rtrim(:approverTo))) <> 0) then
		if (:hadCond<>1) then
			qString := :qString || ' where ';
			hadCond := 1;
		else
			qString := :qString || ' and ';
		end if;
		
		qString := :qString || ' ltrim(rtrim(T3."U_NAME")) <= ' || 'N''' || ltrim(rtrim(:approverTo)) || '''';
	end if;

	-- TEMPLATES
	if (LENGTH(ltrim(rtrim(:templateFrom))) <> 0) then
		if (:hadCond<>1) then
			qString := :qString || ' where ';
			hadCond := 1;
		else
			qString := :qString || ' and ';
		end if;
		
		qString := :qString || ' ltrim(rtrim(T4."Name")) >= ' || 'N''' || ltrim(rtrim(:templateFrom)) || '''';
	end if;

	if (LENGTH(ltrim(rtrim(:templateTo))) <> 0) then
		if (:hadCond<>1) then
			qString := :qString || ' where ';
			hadCond := 1;
		else
			qString := :qString || ' and ';
		end if;
		
		qString := :qString || ' ltrim(rtrim(T4."Name")) <= ' || 'N''' || ltrim(rtrim(:templateTo)) || '''';
	end if;

	-- DATES
	if (LENGTH(ltrim(rtrim(:dateFrom))) <> 0) then
		if (:hadCond<>1) then
			qString := :qString || ' where ';
			hadCond := 1;
		else
			qString := :qString || ' and ';
		end if;
		
		qString := :qString || ' ltrim(rtrim(T0."CreateDate")) >= ' || 'TO_DATE(''' || ltrim(rtrim(:dateFrom)) || ''', ''YYYYMMDD'') ';
	end if;

	if (LENGTH(ltrim(rtrim(:dateTo))) <> 0) then
		if (:hadCond<>1) then
			qString := :qString || ' where ';
			hadCond := 1;
		else
			qString := :qString || ' and ';
		end if;
		
		qString := :qString || ' ltrim(rtrim(T0."CreateDate")) <= ' || 'TO_DATE(''' || ltrim(rtrim(:dateTo)) || ''', ''YYYYMMDD'') ';
	end if;

	-- CARD
	if (LENGTH(ltrim(rtrim(:cardFrom))) <> 0) then
		if (:hadCond<>1) then
			qString := :qString || ' where ';
			hadCond := 1;
		else
			qString := :qString || ' and ';
		end if;
		if (:docTable='OOAT') then
               		qString := :qString || ' ltrim(rtrim(T5."BpCode")) >= ' || 'N''' || ltrim(rtrim(:cardFrom))|| '''';
		else
               		qString := :qString || ' ltrim(rtrim(T5."CardCode")) >= ' || 'N''' || ltrim(rtrim(:cardFrom))|| '''';
		end if;
	end if;

	if (LENGTH(ltrim(rtrim(:cardTo))) <> 0) then
		if (:hadCond<>1) then
			qString := :qString || ' where ';
			hadCond := 1;
		else
			qString := :qString || ' and ';
		end if;
		if (:docTable='OOAT') then
               		qString := :qString || ' ltrim(rtrim(T5."BpCode")) <= ' || 'N''' || ltrim(rtrim(:cardTo))|| '''';
		else
               		qString := :qString || ' ltrim(rtrim(T5."CardCode")) <= ' || 'N''' || ltrim(rtrim(:cardTo))|| '''';
		end if;
	end if;


	-- TOTAL
	if (LENGTH(ltrim(rtrim(:totalFrom))) <> 0) then
		if (:hadCond<>1) then
			qString := :qString || ' where ';
			hadCond := 1;
		else
			qString := :qString || ' and ';
		end if;
		if (:docTable<>'OOAT') then
			qString := :qString || ' T5."DocTotal" >= ' || ltrim(rtrim(:totalFrom));
		end if;		
	end if;

	if (LENGTH(ltrim(rtrim(:totalTo))) <> 0) then
		if (:hadCond<>1) then
			qString := :qString || ' where ';
			hadCond := 1;
		else
			qString := :qString || ' and ';
		end if;
		if (:docTable<>'OOAT') then
			qString := :qString || ' T5."DocTotal" <= ' || ltrim(rtrim(:totalTo));
		end if;
	end if;
	--------------------------------------
	/*	STATUS	*/
	--------------------------------------
	if (:hadCond<>1) then
		qString := :qString || ' where ';
		hadCond := 1;
	else
		qString := :qString || ' and ';
	end if;

	qString := :qString || ' T0."ProcesStat" IN ( ';

	if ((ltrim(rtrim(:statusWait))) = 'Y') then
		if (:hadStatus<>1) then
			hadStatus := 1;
		else
			qString := :qString || '  ,  ';
		end if;
		
		qString := :qString || 'N''W''';
	end if;

	if ((ltrim(rtrim(:statusApproved))) = 'Y') then
		if (:hadStatus<>1) then
			hadStatus := 1;
		else
			qString := :qString || '  ,  ';
		end if;
		
		qString := :qString || 'N''Y''';
	end if;

	if ((ltrim(rtrim(:statusRejected))) = 'Y') then
		if (:hadStatus<>1) then
			hadStatus := 1;
		else
			qString := :qString || '  ,  ';
		end if;
		
		qString := :qString || 'N''N''';
	end if;

	if ((ltrim(rtrim(:statusProduced))) = 'Y') then
		if (:hadStatus<>1) then
			hadStatus := 1;	
		else
			qString := :qString || '  ,  ';
		end if;
		
		qString := :qString || 'N''P''';	
	end if;

	if ((ltrim(rtrim(:statusCanceled))) = 'Y') then
		if (:hadStatus <> 1) then
			hadStatus := 1;
		else
			qString := :qString || '  ,  ';
		end if;
		
		qString := :qString || 'N''C''';
	end if;

	if ((ltrim(rtrim(:statusApprProduced))) = 'Y') then
		if (:hadStatus<>1) then
			hadStatus := 1;	
		else
			qString := :qString || '  ,  ';
		end if;
		
		qString := :qString || 'N''A''';
	end if;

	qString := :qString || '  )';


	--------------------------------------
	/*	OBJECTS	*/
	--------------------------------------
	if(:numObjects>0 and :docTable<>'OOAT') then
		if (:hadCond<>1) then
			qString := :qString || ' where ';
			hadCond := 1;
		else
			qString := :qString || ' and ';
		end if;

		qString := :qString || 'T5."ObjType" IN (' || :objectsList || ')';
	end if;

	if (:hadCond<>1) then
		qString := :qString || ' where ';
		hadCond := 1;
	else
		qString := :qString || ' and ';
	end if;
	
	str1 := 'select * from OWDD where EXISTS ( ' || :qString ||
			'OWDD."WddCode" = T0."WddCode" ) ' ||
			'order by OWDD."ProcessID", OWDD."WddCode" ';

	EXECUTE IMMEDIATE (:str1);
END;


