-- B1 DEPENDS: AFTER:PT:PROCESS_END AFTER:SP:_TmSp_BootCreateGlobalTempTables

create procedure TmSp_IVSR_AutomaticHealingRequiredQuery
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
--set nocount on

	openQty			DECIMAL(21,6);
	qtySum			DECIMAL(21,6);
	transNum		int;
	instance		int;
	lastTransNum	int;
	lastInstance	int;
	cnt 			int;

 	qty_decimals 	int;
 	sum_decimals 	int;
 	main_currency 	nvarchar(3);
 	DBImpacted 		nvarchar(1) := 'N';
 	Recalc_By_OILM 	nvarchar(1) := 'Y';		--Simulation is always as recalculation based on OILM
 	roundmthd 		nvarchar(1);
 	OCRNdeciamls 	int;
	directrate 		nvarchar(1); 
	
	Update_before 	nvarchar(1);
	StockIssue 		nvarchar(1);
	isArchived 		nvarchar(1);
	
	OldTransID 		int;
	OldCreateDate 	TIMESTAMP;
	OldTransType 	int;
	OldcreatedBy 	int;

	TransID 		int;
	CreateDate 		TIMESTAMP;
	TransType 		int;
	createdBy 		int;
	
	sqlStr 			nvarchar(4000);
	tmpStr 			nvarchar(256);
	DocTable 		nvarchar(4);
	
	OldMessageID_1 	int; 
	OldCreateDate_1 TIMESTAMP;
	OldCreateTime_1 int;
	MessageID_1 	int;
	CreateDate_1 	TIMESTAMP;
	CreateTime_1 	int;
	
	cursor zXCursor for select  "TransId", "CreateDate", "TransType", "CreatedBy" from OJDT
		where "TransType" IN (13,14,15,16,18,19,20,21,58,59,60,67,68,69,162,163,164,165,166,10000071,310000001)
		order by "TransId";
		
	cursor typesCur for select "DocTable" from "TmSp_IVSR_AHRQ_Disorder_detect_DocTables";
	
	cursor zXCursor2 for select "MessageID", "CreateDate", "CreateTime" from OILM
	  where "TransType" <> 195 
	  ---exclude inflation wizard postings
	  order by "MessageID"; 
	
Begin
	delete from "TmSp_IVSR_AHRQ_CheckStockResults";

-- ==================================================================================================
-- ==================================================================================================
-- ==================================================================================================
--							SOURCE: IVI_AutomaticHealingRequiredQuery.sql
-- ==================================================================================================
	select "MainCurncy" into main_currency from OADM;
	select "QtyDec" into qty_decimals from OADM;
	
	select count(1) into cnt from CIVI where "ReorderFrm" is not null;
	if :cnt = 0 then
		StockIssue := 'N';
	else 
		StockIssue := 'Y';
	end if;
	
	select count(1) into cnt from LIVI where "UpdateDate" is not null;
	if :cnt = 0 then
		Update_before := 'N';
	else
		Update_before := 'Y';
	end if;
	
	select count(1) into cnt from livi1 where "Result" = 'A';
	if :cnt > 0 then
		isArchived := 'Y';
	else
		isArchived := 'N';
	end if;

-- Recalculation by OILM
if :Recalc_By_OILM = 'Y' or :StockIssue = 'Y' then
	
/**
 * Au_D_1002_SBO_RSP_IVI_OIVL with wrong doclinenum for WKO_Detect 
 * Doclinenum for type 68 was set to zero for all wko1 lines due to old bug in IVU 2005sp1.
*/
	select count(1) into cnt from OILM where '08.80.000.00' <= ifnull("VersionNum",'') and ifnull("VersionNum",'') < '08.80.228.09';
	delete from "TmSp_IVSR_AHRQ_RSP_IVI_Autofx_Boms";
	
	if :Update_before = 'Y' and :cnt > 0 then  
		
		insert into "TmSp_IVSR_AHRQ_RSP_IVI_Autofx_Boms"
		select distinct T0."ItemCode", T0."CreatedBy", T0."TransType", T0."DocLineNum", T0."DocLineNum", T0."MessageID"
		from OIVL T0
		inner join WKO1 T1 on T0."CreatedBy" = T1."OrderNum" and T0."ItemCode" = T1."ItemCode" and 
			T0."InQty" - T0."OutQty" = T1."Quantity" and T0."TransType" = 68 
		order by "MessageID"; 
     
    	update "TmSp_IVSR_AHRQ_RSP_IVI_Autofx_Boms" T0 
    		set T0."DocLineNum" = (select count(1) from "TmSp_IVSR_AHRQ_RSP_IVI_Autofx_Boms" T1 
                         where T1."CreatedBy" = T0."CreatedBy" and T1."MessageID" < T0."MessageID");  
     
     	select count(1) into cnt from "TmSp_IVSR_AHRQ_RSP_IVI_Autofx_Boms" where "DocLineNum" <> "OldDocLineNum";
     	
     	if :cnt > 0 then
     		DBImpacted := 'Y';
     		insert into "TmSp_IVSR_AHRQ_CheckStockResults" values(N'DB is affected(1002)');
     	end if;
    end if;    	
    delete from "TmSp_IVSR_AHRQ_RSP_IVI_Autofx_Boms";
    	
/**
 * Au_D_1003_Production Order Issue Method is not consistent
 * come from Supp_406_Production Order Issue Method is not consistent
 * The issue method in WOR1 is 'B'. But based on the transaction records, it should be 'M'.
 * 
*/
	select count(1) into cnt from 
	(select T0."DocEntry" from OILM T0
	 inner join WOR1 T1 on T0."TransType" = 60 and T0."BaseType" = 202 and T0."AppObjAbs" = T1."DocEntry"
	 	and T0."AppObjLine" = T1."LineNum" and T1."IssueType" = 'B'
	 inner join OWOR T2 on T1."DocEntry" = T2."DocEntry" and T2."Type" = 'S'
	 inner join OIGE T3 on T3."DocEntry" = T0."DocEntry" and  ifnull(T3."VersionNum",'') < '09.10.000.00' 
	 union
	 ---Recalculation by OILM. DB was archived
	 select T0."DocEntry" from OILM T0
	 inner join UWOR1 T1 on T0."TransType" = 60 and T0."BaseType" = 202 and T0."AppObjAbs" = t1."DocEntry"
	 	and "AppObjLine" = T1."LineNum" and T1."IssueType" = 'B'
	 	and  ifnull(T0."VersionNum",'') < '09.10.000.00' 
	 inner join UWOR T2 on T1."DocEntry" = T2."DocEntry" and T2."Type" = 'S'
	 	and (select top 1 "Result" from LIVI1 order by "LogEntry" DESC) = 'A'
	 union
	 ----Dissasembly Components in Reciept for Production with IssueType backflush 
	 ----Recalculation by UILM or by OILM. DB was not archived
	 select T0."DocEntry" from OILM T0 
	 inner join WOR1 T1 on T0."TransType" = 59 and T0."BaseType" = 202 and T0."AppObjAbs" = T1."DocEntry" 
		and T0."AppObjLine" = T1."LineNum" and T1."IssueType" = 'B' 
	inner join OWOR T2 on T1."DocEntry" = T2."DocEntry" and T2."Type" = 'D' 
	union
	----Recalculation by OILM. DB was archived 
	select T0."DocEntry" from OILM T0 
	inner join UWOR1 T1 on T0."TransType" = 59 and T0."BaseType" = 202 and T0."AppObjAbs" = T1."DocEntry" 
		and T0."AppObjLine" = T1."LineNum" and T1."IssueType" = 'B' 
	inner join UWOR T2 on T1."DocEntry" = T2."DocEntry" and T2."Type" = 'D' 
		 and (select top 1 "Result" from LIVI1 order by "LogEntry" DESC) = 'A');
	 
	if :cnt > 0 then
		DBImpacted := 'Y'; 		     
		insert into "TmSp_IVSR_AHRQ_CheckStockResults" values (N'DB is affected(1003)');		     
	end if;
	
/**
 * Au_D_1004_SBO_RSP_IVI_OILM.TotalLC_Is_zero & OILM.TotalSC_FC_Not_Zero_Detect 
 * Document amounts in local currency is zero but amounts in system of foreign currency are not zero
*/
	select count(1) into cnt from OILM t1 
	where  (select "LawsSet" from CINF) not in ( 'BR', 'IN' ) 
		and T1."ItemCode" in (select "ItemCode" from OITM where "InvntItem" = 'Y') 
		and T1."LocCode" not in (select "WhsCode" from OWHS where  "DropShip" = 'Y') 
		and t1."ExpensesLC" = 0 --- holds freights  
		and t1."ExpensesFC" = 0 --- holds freights  
		and t1."Price" = 0 
		and t1."TotalLC" = 0 
		and ( T1."TotalSC" <> 0 or t1."TotalFC" <> 0); 
						  
	if :cnt > 0 then 
		DBImpacted := 'Y'; 
		insert into "TmSp_IVSR_AHRQ_CheckStockResults" values (N'DB is affected(1004)');
		---Au_D_1004_SBO_RSP_IVI_OILM.TotalLC_Is_zero & OILM.TotalSC_FC_Not_Zero_update 
	end if;  
end if;
---										End of Recalculation by OILM
-- ==================================================================================================
---									Start of Recalculation by OILM or UILM
---								The script which should be check in both case
/**
 * Au_D_2000_Transtype=58 -2 Pricecurr<>maincurr and Pricerate=0_Detect 
 * Document price in foreign currency but pricerate is zero
*/
	select count(1) into cnt from OILM T0
	where T0."TransType" in (58,-2) and ifnull("PriceCurr",'') <> :main_currency and "Price" <> 0 and "PriceRate" = 0;

	if :cnt > 0 then
    	DBImpacted := 'Y'; 
		insert into "TmSp_IVSR_AHRQ_CheckStockResults" values (N'DB is affected(2000)');
		---Run Au_F_2000_Transtype=58 -2 Pricecurr<>maincurr and Pricerate=0_Update 
  	end if;

/**
 * Au_D_2005_OIVL_invntact_negstckact_missing 
 * 
*/
	select count(1) into cnt from OIVL where (ifnull("SumStock", 0) <> 0 and ifnull("InvntAct",'') = '') or
		ifnull("NegInvAdjs", 0) <> 0 and ifnull("NegStckAct", '') = '';	
   
  	if :cnt > 0 then
		DBImpacted := 'Y'; 
		insert into "TmSp_IVSR_AHRQ_CheckStockResults" values (N'DB is affected(2005)');
		---Run Au_F_2005_OIVL_invntact_negstckact_missing 
	end if;  

/**
 * Au_D_2006_SBO_RSP_IVI_OILM.AppObjLine Is Not -1 
 * OILM.AppObjLine is not -1 for production father item
*/	
	select count(1) into cnt from OILM where "AppObjType" = 'P' and "AppObjLine" <> -1;
	
	if :cnt > 0 then
	  DBImpacted := 'Y'; 
	  insert into "TmSp_IVSR_AHRQ_CheckStockResults" values (N'DB is affected(2006)'); 
	end if;

-------------------------------------------------End of Recalculation by OILM or UILM
if (:isArchived = 'N' or :StockIssue = 'Y') then
  	
  	/**
	 * Au_D_3001_Disorder in OJDT.CreateDate in relation to TransID_First_Detect
	 *
	 * Script detects journal entry X with createdate higher than createdate 
	 * of journal entry Y which was created later
	 * script detects as well Nulls in createdate
	*/
	OldTransID := -9999;
	OldCreateDate := '1900-01-01';
	DBImpacted := 'N';
	
	open zXCursor;
	fetch zXCursor into TransID, CreateDate, TransType, createdBy; 
	
	while not zXCursor::NOTFOUND do
	
		if :OldTransID < :TransID and 
			ifnull(:OldCreateDate, '1900-01-01') > ifnull(:CreateDate, '1900-01-01') then
			DBImpacted := 'Y';
			break;
		end if;
		
		OldTransID := :TransID;
		OldCreateDate := :CreateDate;
		OldTransType := :TransType;
		OldcreatedBy := :createdBy;
		
		fetch zXCursor into TransID, CreateDate, TransType, createdBy;
	end while;
	close zXCursor;
	
	if :DBImpacted = 'Y' then 
		insert into "TmSp_IVSR_AHRQ_CheckStockResults" values (N'DB is affected(3001)');
    end if;
	
 	/**
	 * Au_D_3002_Disorder In Documents
	 *
	 * 
	*/
	delete from "TmSp_IVSR_AHRQ_Disorder_detect_DocTables"; 
	DBImpacted := 'N'; 

	insert into "TmSp_IVSR_AHRQ_Disorder_detect_DocTables" values('OINV');
	insert into "TmSp_IVSR_AHRQ_Disorder_detect_DocTables" values('ORIN');
	insert into "TmSp_IVSR_AHRQ_Disorder_detect_DocTables" values('OCSI');
	insert into "TmSp_IVSR_AHRQ_Disorder_detect_DocTables" values('OCSV');
	insert into "TmSp_IVSR_AHRQ_Disorder_detect_DocTables" values('ODLN');
	insert into "TmSp_IVSR_AHRQ_Disorder_detect_DocTables" values('ORDN');
	insert into "TmSp_IVSR_AHRQ_Disorder_detect_DocTables" values('OPCH');
	insert into "TmSp_IVSR_AHRQ_Disorder_detect_DocTables" values('ORPC');
	insert into "TmSp_IVSR_AHRQ_Disorder_detect_DocTables" values('OCPI');
	insert into "TmSp_IVSR_AHRQ_Disorder_detect_DocTables" values('OCPV');
	insert into "TmSp_IVSR_AHRQ_Disorder_detect_DocTables" values('OPDN');
	insert into "TmSp_IVSR_AHRQ_Disorder_detect_DocTables" values('ORPD');
	insert into "TmSp_IVSR_AHRQ_Disorder_detect_DocTables" values('OIGN');
	insert into "TmSp_IVSR_AHRQ_Disorder_detect_DocTables" values('OIGE');
	insert into "TmSp_IVSR_AHRQ_Disorder_detect_DocTables" values('OWTR');
	insert into "TmSp_IVSR_AHRQ_Disorder_detect_DocTables" values('OIPF');
	insert into "TmSp_IVSR_AHRQ_Disorder_detect_DocTables" values('OIQI');
	insert into "TmSp_IVSR_AHRQ_Disorder_detect_DocTables" values('OIQR');

	open typesCur;
	fetch typesCur into DocTable;

	while not typesCur::NOTFOUND do		
		
		delete from "TmSp_IVSR_AHRQ_Disorder_detect_record_found";
		
		sqlStr := 'insert into "TmSp_IVSR_AHRQ_Disorder_detect_record_found" select count(1) from ' || :DocTable ||
			' T0 inner join ' || :DocTable || ' T1 on T0."DocEntry" = T1."DocEntry" - 1 ' ||
			' where T0."CreateDate" > T1."CreateDate" or (T0."CreateDate" = T1."CreateDate" and T0."DocTime" > T1."DocTime")';
	
		exec(:sqlStr);
	    
		/*DB is impacted*/
		select top 1 "rec" into cnt from "TmSp_IVSR_AHRQ_Disorder_detect_record_found";
		
		if :cnt > 0 then
			DBImpacted := 'Y';
			break;
		end if;

		fetch typesCur into DocTable;
	end while;
	close typesCur;

	delete from "TmSp_IVSR_AHRQ_Disorder_detect_record_found";
	delete from "TmSp_IVSR_AHRQ_Disorder_detect_DocTables";

	if :DBImpacted = 'Y' then 
		insert into "TmSp_IVSR_AHRQ_CheckStockResults" values (N'DB is affected(3002)');
    end if;

	/**
	 * Au_D_3003_Disorders in OILM and without Journal Entry Step 2 _Detect
	 *
	 * 
	*/
	delete from "TmSp_IVSR_AHRQ_DISORDER_OILM_WITHOU_JE";
	delete from "TmSp_IVSR_AHRQ_DISORDER_OJDTTemp";

	/* collect the OILM Records without JE */
	insert into "TmSp_IVSR_AHRQ_DISORDER_OILM_WITHOU_JE" 
	select distinct "DocEntry", "TransType", "CreateDate" 
	from   OILM T0 
	where  T0."TransType" not in (-2,58,202,68,165,166) 
		and T0."AccumType" = 1 
		and T0."ActionType" not in (9,10)
		and not exists (select 1 from   OJDT T1 
						where  T1."TransType" = T0."TransType" and T1."CreatedBy" = T0."DocEntry"); 
								 
	insert into "TmSp_IVSR_AHRQ_DISORDER_OJDTTemp"
	select DISTINCT "TransType", max("CreatedBy") as "CreatedBy", "CreateDate" 
	from OJDT T1
	where "TransType" in (select DISTINCT "TransType" from "TmSp_IVSR_AHRQ_DISORDER_OILM_WITHOU_JE")
		and NOT ( ( T1."TransType" = 20
								AND ifnull(( SELECT "TransId"
											 FROM   OPDN T3
											 WHERE  T3."DocEntry" = T1."CreatedBy"
										   ), -1) <> T1."TransId"
							  )
							  OR ( T1."TransType" = 21
								   AND ifnull(( SELECT  "TransId"
												FROM    ORPD T3
												WHERE   T3."DocEntry" = T1."CreatedBy"
											  ), -1) <> T1."TransId"
								 )
							)
	 group by "TransType","CreateDate";
	 
	 insert into "TmSp_IVSR_AHRQ_DISORDER_OJDTTemp"
	 select DISTINCT "TransType", min("CreatedBy") as "CreatedBy", "CreateDate" 
	 from OJDT T1
	 where "TransType" in (select DISTINCT "TransType" from "TmSp_IVSR_AHRQ_DISORDER_OILM_WITHOU_JE")
	 and NOT ( ( T1."TransType" = 20
								AND ifnull(( SELECT "TransId"
											 FROM   OPDN T3
											 WHERE  T3."DocEntry" = T1."CreatedBy"
										   ), -1) <> T1."TransId"
							  )
							  OR ( T1."TransType" = 21
								   AND ifnull(( SELECT  "TransId"
												FROM    ORPD T3
												WHERE   T3."DocEntry" = T1."CreatedBy"
											  ), -1) <> T1."TransId"
								 )
							)
	 group by "TransType", "CreateDate";

	-- Creation of indexes is skipped, because current revision doesn't support it
		 
	-------------------------------------------------------------------- 
	select count(1) into cnt 
	from "TmSp_IVSR_AHRQ_DISORDER_OILM_WITHOU_JE" T0 
 	join "TmSp_IVSR_AHRQ_DISORDER_OJDTTemp" T1 on T1."TransType" = T0."TransType" 
 		AND T1."CreatedBy" < T0."DocEntry" AND T1."CreateDate" > T0."CreateDate";
 	
 	if :cnt = 0 then
 		select count(1) into cnt from "TmSp_IVSR_AHRQ_DISORDER_OILM_WITHOU_JE" T0 
 		join "TmSp_IVSR_AHRQ_DISORDER_OJDTTemp" T1
 		on T1."TransType" = T0."TransType" AND T1."CreatedBy" > T0."DocEntry" AND T1."CreateDate" < T0."CreateDate";
 	end if;	
 	
 	if :cnt > 0 then
		DBImpacted := 'Y';
		insert into "TmSp_IVSR_AHRQ_CheckStockResults" values (N'DB is affected(3003)');
	end if;
		
	delete from "TmSp_IVSR_AHRQ_DISORDER_OJDTTemp";
	delete from "TmSp_IVSR_AHRQ_DISORDER_OILM_WITHOU_JE";

-- end of if (@isArchived = 'N' or @88stockissue = 'Y')
end if;
-- AU_F 3004 should be at last to fix the OILM.CreateDate disorder issues after 3001~3003 fix other CreateDate disorder issue 
if (:Recalc_By_OILM = 'Y' or :StockIssue = 'Y') then 
/**
 * Au_D_3004_SBO_RSP_IVI_Disorder_In_OILM_between_CreateDate&MessageID_Detect 
 *	
 *
*/	
	OldMessageID_1 := -9999;
	OldCreateDate_1 := '1900-01-01'; 
	DBImpacted := 'N'; 

	open zXCursor2;
	fetch zXCursor2 into MessageID_1, CreateDate_1, CreateTime_1; 
	
	while not zXCursor2::NOTFOUND do
			
		if (( :OldMessageID_1 < :MessageID_1 ) 
			   and (( ifnull(:OldCreateDate_1, '1900-01-01') > ifnull(:CreateDate_1, '1900-01-01')) 
					  or ( ifnull(:OldCreateDate_1, '1900-01-01') = ifnull(:CreateDate_1, '1900-01-01') 
						   and ifnull(:OldCreateTime_1, 0) > ifnull(:CreateTime_1, 0) ))) then
			DBImpacted := 'Y';
			break;
		end if;
		
		OldMessageID_1 := :MessageID_1;
		OldCreateDate_1 := :CreateDate_1;
		OldCreateTime_1 := :CreateTime_1;
		
		fetch zXCursor2 into MessageID_1, CreateDate_1, CreateTime_1;
	end while;
	close zXCursor2;

	if :DBImpacted = 'Y' then
		insert into "TmSp_IVSR_AHRQ_CheckStockResults" values (N'DB is affected(3004)');
	end if;

-- end of if (:Recalc_By_OILM = 'Y' or :StockIssue = 'Y')
end if;

	select * from "TmSp_IVSR_AHRQ_CheckStockResults";
end;


