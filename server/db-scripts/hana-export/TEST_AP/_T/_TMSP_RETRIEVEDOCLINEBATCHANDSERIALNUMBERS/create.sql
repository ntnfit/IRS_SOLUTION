create PROCEDURE _TmSp_RetrieveDocLineBatchAndSerialNumbers
(
	IN docType integer,
	IN docEntry integer
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
	docTable nvarchar(4);
	sqlStatement nvarchar(4000);

	currentRow integer;
	number nvarchar(6000) :='';
	managedByOfLastLine integer;
	rowNo integer;
	managedBy integer;
	batch_number nvarchar(60);
	serial_number nvarchar(60);
	qty DECIMAL(21,6);
	lcPrice DECIMAL(21,6);
	doc1Price DECIMAL(21,6);
	doc1Rate DECIMAL(21,6);
	doc1Price2 DECIMAL(21,6);
	doc1Rate2 DECIMAL(21,6);
	
	rowCount integer;
	iter integer;
	hasNext integer;
BEGIN

	if :docType = 15 then
		docTable :='DLN1';
	elseif :docType =16 then
		docTable :='RDN1';
	elseif :docType =20 then
		docTable :='PDN1';
	elseif :docType =21 then
		docTable :='RPD1';
	elseif :docType =59 then
		docTable :='IGN1';
	elseif :docType =60 then
		docTable :='IGE1';
	elseif :docType =67 then
		docTable :='WTR1';
	elseif :docType =112 then
		docTable :='DRF1';
	end if;
	
	CREATE local temporary TABLE #RetrieveDocLineSnB_TempTable 
	(
		"ID" integer,
		"DocRow" integer,
		"ManagedBy" integer,
		"BatchNumber" nvarchar(60),
		"SerialNumber" nvarchar(60),
		"Quantity" DECIMAL(21,6),
		"SysNumber" integer,
		"Price" DECIMAL(21,6),
		"Rate" DECIMAL(21,6)
	);
	DELETE FROM #RetrieveDocLineSnB_TempTable WHERE 1=1;
	
	CREATE local temporary TABLE #RetrieveDocLineSnB_TempTable_Merge 
	(
		"DocRow" integer,
		"ManagedBy" integer,
		"BatchNumber" nvarchar(60),
		"SerialNumber" nvarchar(60),
		"Quantity" DECIMAL(21,6),
		"Price" DECIMAL(21,6),
		"Rate" DECIMAL(21,6),
		"ID" integer
		--"ID_in_Group" integer
	);
	DELETE FROM #RetrieveDocLineSnB_TempTable_Merge WHERE 1=1;
	
	CREATE local temporary TABLE #RetrieveDocLineSnB_ReturnTable
	(
		"DocumentEntry" integer,
		"DocRow" integer,
		"ManagedBy" integer,
		"BatchNumber" nvarchar(5000),
		"SerialNumber" nvarchar(5000),
		"Quantity" DECIMAL(21,6),
		"Price" DECIMAL(21,6)
	);
	DELETE FROM #RetrieveDocLineSnB_ReturnTable WHERE 1=1;
	
	--select t0."LineNum", t5."ManagedBy",t5."BatchNumber",t5."SerialNumber",t5."Quantity", t5."SysNumber" from PDN1 t0 
	--left join 
	--(
	--	select  t1."DocEntry",t1."DocLine", t1."ManagedBy",t3."DistNumber" as "BatchNumber", t4."DistNumber" as "SerialNumber", t2."Quantity" ,case t1."ManagedBy"
	--										when 10000044 then t3."SysNumber" 
	--										when 10000045 then t4."SysNumber"
	--										else NULL end as "SysNumber"   
	--	from OITL t1 
	--		join ITL1 t2 on t1."LogEntry"=t2."LogEntry" and t1."DocType"= 20 and t1."DocEntry"=3  
	--		left outer join OBTN t3 on t2."SysNumber"=t3."SysNumber" and t2."ItemCode"=t3."ItemCode" and t1."ManagedBy"=10000044
	--		left outer join OSRN t4 on t2."SysNumber"=t4."SysNumber" and t2."ItemCode"=t4."ItemCode" and t1."ManagedBy"=10000045
	--) t5 
	--on t0."DocEntry"=t5."DocEntry" and t0."LineNum"=t5."DocLine"
	--where t0."DocEntry"=3  
	--order by t0."LineNum", t5."SysNumber";

	sqlStatement := N'insert into #RetrieveDocLineSnB_TempTable("DocRow","ManagedBy","BatchNumber","SerialNumber","Quantity","SysNumber","Price","Rate") ';
	sqlStatement := :sqlStatement || N'select t0."LineNum",t5."ManagedBy",t5."BatchNumber",t5."SerialNumber",t5."Quantity", t5."SysNumber",t0."Price",t0."Rate" from ' || :docTable ||' t0  
										left join
										(
											select  t1."DocEntry",t1."DocLine", t1."ManagedBy",t3."DistNumber" as "BatchNumber", t4."DistNumber" as "SerialNumber", t2."Quantity" ,
													case t1."ManagedBy" 
														when 10000044 then t3."SysNumber"  
														when 10000045 then t4."SysNumber" 
														else NULL end as "SysNumber"   
											from OITL t1 
											join ITL1 t2 on t1."LogEntry"=t2."LogEntry" and t1."DocType"= ' || cast(:docType as nvarchar) ||'  and t1."DocEntry"=' || cast(:docEntry as nvarchar) || 
											N' left outer join OBTN t3 on t2."SysNumber"=t3."SysNumber" and t2."ItemCode"=t3."ItemCode" and t1."ManagedBy"=10000044 
											left outer join OSRN t4 on t2."SysNumber"=t4."SysNumber" and t2."ItemCode"=t4."ItemCode" and t1."ManagedBy"=10000045 
										) as t5
										on t0."DocEntry"=t5."DocEntry" and t0."LineNum"=t5."DocLine" 
										where t0."DocEntry"=' ||cast(:docEntry as nvarchar) || 
										N' order by t0."LineNum", t5."SysNumber"'; 										
	exec (:sqlStatement);													

	insert into #RetrieveDocLineSnB_TempTable_Merge 
	select t0."DocRow", t0."ManagedBy",t0."BatchNumber", t0."SerialNumber", t0."Quantity",t0."Price",t0."Rate", 
			ROW_NUMBER() over (order by t0."DocRow",t0."SysNumber") as "ID" 
			--ROW_NUMBER() over (PARTITION by t0."DocRow" order by t0."DocRow") as "id_in_group"
	from #RetrieveDocLineSnB_TempTable t0; 
	update #RetrieveDocLineSnB_TempTable_Merge t0 set t0."BatchNumber" = t0."BatchNumber" || '(' || cast (abs(t0."Quantity") as nvarchar) || ')' where t0."ManagedBy" = 10000044;
	
	select count(*) into rowCount  from #RetrieveDocLineSnB_TempTable_Merge;
	
	iter := 1;
	hasNext := 1;
	number := '';
	
	select t0."DocRow",t0."ManagedBy",t0."BatchNumber",t0."SerialNumber",t0."Quantity",t0."Price",t0."Rate" 
	into rowNo, managedBy, batch_number, serial_number, qty, doc1Price, doc1Rate 
	from #RetrieveDocLineSnB_TempTable_Merge t0 
	where t0."ID" = :iter;
	
	currentRow := :rowNo;

	while :iter <= :rowCount and :hasNext = 1 do
		if :managedBy = 10000044 then 
				number := :number || :batch_number || '; ';
		elseif :managedBy = 10000045 then 
				number := :number || :serial_number || '; ';
		end if;
		managedByOfLastLine := :managedBy;
		
		if iter < rowCount then 
			iter := :iter + 1;
			select t0."DocRow",t0."ManagedBy",t0."BatchNumber",t0."SerialNumber",t0."Quantity",t0."Price",t0."Rate" 
			into rowNo, managedBy, batch_number, serial_number, qty, doc1Price2, doc1Rate2 
			from #RetrieveDocLineSnB_TempTable_Merge t0 
			where t0."ID" = :iter;
		else
			hasNext := 0;
			
		end if;

		if :hasNext = 0  OR :currentRow <> :rowNo then 
			if :doc1Rate is not null and :doc1Rate <> 0 then
				lcPrice := :doc1Price * :doc1Rate;
			else
				lcPrice := :doc1Price;
			end if;
			
			if :managedByOfLastLine = 10000044 then
				insert into #RetrieveDocLineSnB_ReturnTable values(:docEntry,:currentRow,:managedByOfLastLine,substring(:number,1,length(:number) - 2),NULL,:qty,:lcPrice);
			elseif :managedByOfLastLine = 10000045 then
				insert into #RetrieveDocLineSnB_ReturnTable values(:docEntry,:currentRow,:managedByOfLastLine,NULL,substring(:number,1,length(:number) - 2),:qty,:lcPrice);
			else
				insert into #RetrieveDocLineSnB_ReturnTable values(:docEntry,:currentRow,:managedByOfLastLine,NULL,NULL,NULL,:lcPrice);
			end if;
			doc1Price := :doc1Price2;
			doc1Rate := :doc1Rate2;
			if :hasNext = 1 then
				currentRow := :rowNo;
				number := '';
			end if;
		end if;
			
	end while;
	
	--select * from #RetrieveDocLineSnB_TempTable;
	--select * from #RetrieveDocLineSnB_TempTable_Merge;
	select * from #RetrieveDocLineSnB_ReturnTable;
	drop table #RetrieveDocLineSnB_TempTable;
	drop table #RetrieveDocLineSnB_TempTable_Merge;
	drop table #RetrieveDocLineSnB_ReturnTable;
END;


