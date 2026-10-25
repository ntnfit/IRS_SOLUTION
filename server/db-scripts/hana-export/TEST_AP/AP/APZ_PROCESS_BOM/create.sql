
create procedure apz_process_BOM
(
in TransType varchar(1) default '',
in Code nvarchar(50) default ''
)
as
begin
DECLARE ExistedHighlevelCode NVARCHAR(50);
DECLARE isItemCode int;
DECLARE isType int;
DECLARE isResName int;
DECLARE isWhs int;
DECLARE whsCode int;
DECLARE isQuantity int;
DECLARE isQauntity int;
Declare iscode int;
Declare isman int;
Declare isInvntItem int;
Declare isFather int;
Declare isRow int;
declare ocrCode nvarchar(50);
declare isocrCode nvarchar(50) ;
declare ocrCode2 nvarchar(50);
declare isocrCode2 nvarchar(50);
declare ocrCode3 nvarchar(50) ;
declare isocrCode3 nvarchar(50) ;
declare ocrCode4 nvarchar(50);
declare isocrCode4 nvarchar(50);
declare ocrCode5 nvarchar(50);
declare isocrCode5 nvarchar(50);
declare invalidOcrCode  nvarchar(50);
declare isinvalidOcrCode  int;
declare isinvalidOcrCode2  int;
declare invalidOcrCode2  nvarchar(50);
Declare isPriceList int;
Declare isCurrentCy int;
Declare isListNum int;
Declare isProjectLine int;
Declare isProject int;
declare invalidOcrCode3 nvarchar(50) ;
declare isinvalidOcrCode3 int;
Declare isinvalidOcrCode4 int;
declare invalidOcrCode4 nvarchar(50);
declare invalidOcrCode5 nvarchar(50) ;
declare isinvalidOcrCode5 int ;
DECLARE BnScode nvarchar(50);
Declare ocrCodehh nvarchar(50);

     
create local temporary table #APZ_DOCLINE_BOMLocal as (select top 0 * from ITT1);
execute immediate 'insert into #APZ_DOCLINE_BOMLocal select * from "#APZ_DOCLINE_BOM"';

create local temporary table #APZ_DOC_BOMLocalTempo as (select top 0 * from OITT);
execute immediate 'insert into #APZ_DOC_BOMLocalTempo select * from "#APZ_DOC_BOM"';
/*DEFAULT value */
	UPDATE #APZ_DOCLINE_BOMLocal SET "PriceList" = 0 WHERE Ifnull("PriceList",0) < 0;
	UPDATE #APZ_DOC_BOMLocalTempo SET "PriceList" = 0 WHERE Ifnull("PriceList",0) < 0;
	/* Validate Data */
-- Check Item Code is valid

	select count(1) into isItemCode  from oitm where "ItemCode"=:Code;
	if (ifnull(isItemCode,0)=0)
	then
		 select 1 , 'Document Item Code does not exists.' from dummy;
		return;
	end if;
	
	select count(1) into isType from #APZ_DOCLINE_BOMLocal a left join oitm b on a."Code" =b."ItemCode" where b."ItemCode" is null AND a."Type" = 4;
	if (ifnull(isType,0)=1) 
	then
		select 2 ,' Line Item Code does not exists.' from dummy;
		return ;
	end if;
	
	select Count(1) into  isResName from #APZ_DOCLINE_BOMLocal a left join ORSC b on a."Code"= b."ResCode" where b."ResCode" is null AND a."Type" = 290;
	if  (ifnull(isResName,0)=1) 
	then
		select 3,' Line resource does not exists.' from dummy;
		return ;
	end if;

	-- Warehouse code is valid  

	select b."WhsCode" into isWhs from #APZ_DOC_BOMLocalTempo a left join owhs b on a."ToWH" = b."WhsCode"  where b."WhsCode" is NULL AND LTRIM(RTRIM(IFNULL(a."ToWH", '')))  is null;
	if  (ifnull(isWhs,'') <>'') 
	then
		select 4 ,'Doc warehouse does not exists.' from dummy;
		return ;
	end if;
	
	select b."WhsCode" into whsCode from #APZ_DOCLINE_BOMLocal a left join owhs b on a."Warehouse"= b."WhsCode"  where b."WhsCode" is null  AND LTRIM(RTRIM(IFNULL(a."Warehouse", ''))) is null;
	if  (ifnull(whsCode,'')<>'') 
	then
		select 5,' Line warehouse does not exists.' from dummy;
		return ;
	end if;

	-- Check quantity
select count(*) into isQauntity from #APZ_DOC_BOMLocalTempo where "Qauntity"<=0;
	if (ifnull(isQuantity,0) > 0)
	then
		select 6, 'Product''s Quantity must be greater than zero' from dummy;
		return;
	end if;
	 select count(*) into isQuantity  from #APZ_DOCLINE_BOMLocal where "Quantity"<=0;
	if (ifnull(isQuantity,0) > 0)
	then
		select 7, 'Components Quantity must be greater than zero' from dummy;
		return;
	END if;
    
	if TransType <> 'A' AND TransType <> 'U'
	then
		select 8, 'Invalid action. Must be ''A''-Add ; ''U''-Update' from dummy;
		return;
	
	ELSEIF TransType = 'A'
	    then
			IF LTRIM(RTRIM(:code))=''
			then
				select 9, 'Father item empty.' from dummy;
				return;
	        
			ELSE
	        SELECT count(1) into iscode FROM OITT WHERE Code = :code;
				IF (ifnull(iscode,0)=1)
					then
						select 10, 'This entry already exists in the following tables  ''Product Tree'' (OITT).' from dummy;
						return;
					END if;
				END if;
	END if;
    
select count(1) into isman from #APZ_DOCLINE_BOMLocal a INNER JOIN OITM b ON a."Code"=b."ItemCode" WHERE a."IssueMthd"='B' AND (b."ManBtchNum" = 'Y' OR b."ManSerNum" = 'Y');
	if (ifnull(isman,0)=1)
	then
			select TOP 1 a."Code"  into BnScode from #APZ_DOCLINE_BOMLocal a INNER JOIN OITM b ON a."Code"=b."ItemCode" AND (b."ManBtchNum" = 'Y' OR b."ManSerNum" = 'Y');
			select 11,' Serial or batch managed items cannot be defined as backflush. [Bill of Materials - Components - Issue Method][Item: '+ :BnScode + ']' from dummy;
			RETURN;
      
	END if;
select count(1) into isInvntItem  from #APZ_DOC_BOMLocalTempo a INNER JOIN OITM b ON a."Code"=b."ItemCode" AND b."InvntItem" <> 'Y' WHERE a."TreeType" = 'P';
	if(ifnull(isInvntItem,0)=1)
		then
			select 12, 'BOM type cannot be Production for a non-Inventory item [Product Tree - Parent Item] , ''' + :Code + '''' from dummy;
			RETURN;
        END if;
	
	select Count(1) into isFather from #APZ_DOCLINE_BOMLocal a INNER JOIN (SELECT t."Father" FROM itt1 t WHERE t."Code"=:Code) b ON a."Code"=b."Father";
	if (ifnull(isFather,0)=1)
	then
		
		SELECT TOP 1 a."Code" into ExistedHighlevelCode from #APZ_DOCLINE_BOMLocal a INNER JOIN (SELECT t."Father",t."Code" FROM itt1 t WHERE t."Code"=:Code) b ON a."Code"=b."Father";

		select 13,'Item already defined at a higher level in the product tree [Bill of Materials - Components - Component Code][''' + :ExistedHighlevelCode + ''']' from dummy;
		
		RETURN;
	END if;

	SELECT count(1) into isRow FROM #APZ_DOCLINE_BOMLocal;
	IF (ifnull(isRow,0)=0)
	then
		select 14,'Components table have no rows' from dummy;
		RETURN;
	end if;
select  ifnull((select "OcrCode" from #APZ_DOC_BOMLocalTempo), '') into ocrCodehh from dummy;
	SELECT "OcrCode" into isocrCode  FROM oocr b WHERE "DimCode"=1 AND RTRIM(LTRIM(IFNULL("Locked",''))) <> 'Y' AND (RTRIM(LTRIM(:ocrCodehh))) <>'';
	IF  (ifnull(isocrCode,'')='')
	then
		select 15,'Invalid BOM OcrCode ''' || :ocrCodehh || '''' from dummy;
		RETURN;
	end if;


	
	select ifnull((select "OcrCode2"  from #APZ_DOC_BOMLocalTempo), '') into ocrCode2 from dummy;
	SELECT "OcrCode" into isocrCode2 FROM oocr b WHERE "DimCode"=2 AND RTRIM(LTRIM(IFNULL("Locked",''))) <> 'Y' AND (RTRIM(LTRIM(:ocrCode2))) <> '';
	IF  (ifnull(isocrCode2,'') <>'')
	then
		select 1501,'Invalid BOM OcrCode2 ''' || :ocrCode2 || '''' from dummy;
		RETURN;
	end if;
	
	
	select ifnull((select "OcrCode3" from #APZ_DOC_BOMLocalTempo), '') into ocrCode3 from dummy;
	SELECT "OcrCode" into isocrCode3 FROM oocr b WHERE "DimCode"=3  AND RTRIM(LTRIM(IFNULL("Locked",''))) <> 'Y' AND (RTRIM(LTRIM(:ocrCode3))) <> '';
	IF (ifnull(isocrCode3,'') <>'')
	then
		select 1502,'Invalid BOM OcrCode3 ''' || :ocrCode3 || '''' from dummy;
		RETURN;
	end if;
	
	select  ifnull((select "OcrCode4" from #APZ_DOC_BOMLocalTempo), '') into ocrCode4 from dummy;
	SELECT "OcrCode" into isocrCode4 FROM oocr b WHERE "DimCode"=4  AND RTRIM(LTRIM(IFNULL("Locked",''))) <> 'Y' AND (RTRIM(LTRIM(:ocrCode4))) <>'';
	IF (ifnull(isocrCode4,'')<>'')
	then
		select 1503,'Invalid BOM OcrCode4 ''' || :ocrCode4 || '''' from dummy;
		RETURN;
	end if;

	select  ifnull((select "OcrCode5" from #APZ_DOC_BOMLocalTempo), '') into ocrCode5 from dummy;
	SELECT "OcrCode" into isocrCode5 FROM oocr b WHERE "DimCode"=5 AND RTRIM(LTRIM(IFNULL("Locked",''))) <> 'Y' AND (RTRIM(LTRIM(:ocrCode5))) <> '';
	IF (ifnull(isocrCode5,'')<>'')
	then
		select 1504, 'Invalid BOM OcrCode5 ''' || :ocrCode5 || '''' from dummy;
		RETURN;
	end if;

	SELECT count(1) into isinvalidOcrCode FROM #APZ_DOCLINE_BOMLocal a where not exists (select 1 from OOCR b where "DimCode"=1 and a."OcrCode"=b."OcrCode" and RTRIM(LTRIM(IFNULL("Locked",''))) <> 'Y' AND RTRIM(LTRIM(IFNULL(a."OcrCode",'')))<>'');
	IF (ifnull(isinvalidOcrCode,0) =1) 
	then
		SELECT top 1 "OcrCode" into invalidOcrCode FROM #APZ_DOCLINE_BOMLocal a where not exists (select 1 from OOCR b where "DimCode"=1 and a."OcrCode"=b."OcrCode" and RTRIM(LTRIM(IFNULL("Locked",''))) <> 'Y' AND RTRIM(LTRIM(IFNULL(a."OcrCode",''))) <>'');
		select 1505, 'Invalid Components OcrCode ''' || :invalidOcrCode || '''' from dummy;
		RETURN;
	END if;
    
   
    SELECT count(1) into isinvalidOcrCode2 FROM #APZ_DOCLINE_BOMLocal a where not exists (select 1 from OOCR b where "DimCode"=2 and a."OcrCode2"=b."OcrCode" and RTRIM(LTRIM(IFNULL("Locked",''))) <> 'Y' AND RTRIM(LTRIM(IFNULL(a."OcrCode2",'')))<>'');
	IF  (ifnull(isinvalidOcrCode2,0) =1) 
	then
		SELECT top 1 "OcrCode2"  into invalidOcrCode2 FROM #APZ_DOCLINE_BOMLocal a where not exists (select 1 from OOCR b where "DimCode"=2 and a."OcrCode2"=b."OcrCode" and RTRIM(LTRIM(IFNULL("Locked",''))) <> 'Y' AND RTRIM(LTRIM(IFNULL(a."OcrCode2",''))) <>'');
		select 1506, 'Invalid Components OcrCode2 ''' + :invalidOcrCode2 + '''' from dummy;
		RETURN;
	END if;
  
SELECT count(1) into isinvalidOcrCode3 FROM #APZ_DOCLINE_BOMLocal a where not exists (select 1 from OOCR b where "DimCode"=3 and a."OcrCode3"=b."OcrCode" and RTRIM(LTRIM(IFNULL("Locked",''))) <> 'Y' AND RTRIM(LTRIM(IFNULL(a."OcrCode3",'')))<>'');
	IF  (ifnull(isinvalidOcrCode3,0) =1) 
	then
			
			SELECT top 1 "OcrCode3" into invalidOcrCode3 FROM #APZ_DOCLINE_BOMLocal a where not exists (select 1 from OOCR b where "DimCode"=3 and a."OcrCode3"=b."OcrCode" and RTRIM(LTRIM(IFNULL("Locked",''))) <> 'Y' AND RTRIM(LTRIM(IFNULL(a."OcrCode3",''))) <>'');
		select 1507, 'Invalid Components OcrCode3 ''' + :invalidOcrCode3 + '''' from dummy;
		RETURN ;
	END if;
	
	
	SELECT count(1) into isinvalidOcrCode4 FROM #APZ_DOCLINE_BOMLocal a where not exists (select 1 from OOCR b where "DimCode"=4 and a."OcrCode4"=b."OcrCode" and RTRIM(LTRIM(Ifnull("Locked",''))) <> 'Y' AND RTRIM(LTRIM(Ifnull(a."OcrCode4",''))) <>'') ;
	IF (ifnull(isinvalidOcrCode4,0)=1)
	
	then
		 SELECT top 1 "OcrCode4" into invalidOcrCode4 FROM #APZ_DOCLINE_BOMLocal a where not exists (select 1 from OOCR b where "DimCode"=4 and a."OcrCode4"=b."OcrCode" and RTRIM(LTRIM(IFNULL("Locked",''))) <> 'Y' AND RTRIM(LTRIM(IFNULL(a."OcrCode4",''))) <>'');
	
		select 1508, 'Invalid Components OcrCode4 ''' + :invalidOcrCode4 + '''' from dummy;
		RETURN;
	END if;
	
	SELECT count(1) into isinvalidOcrCode5 FROM #APZ_DOCLINE_BOMLocal a where not exists (select 1 from OOCR b where "DimCode"=5 and a."OcrCode5"=b."OcrCode" and RTRIM(LTRIM(Ifnull("Locked",''))) <> 'Y' AND RTRIM(LTRIM(Ifnull(a."OcrCode5",''))) <>'');
	IF (ifnull(isinvalidOcrCode5,0)=1) 
	then
		
		SELECT top 1 "OcrCode5" into invalidOcrCode5 FROM #APZ_DOCLINE_BOMLocal a where not exists (select 1 from OOCR b where "DimCode"=5 and a."OcrCode5"=b."OcrCode" and RTRIM(LTRIM(IFNULL("Locked",''))) <> 'Y' AND RTRIM(LTRIM(IFNULL(a."OcrCode5",''))) <>'');
		select 1509, 'Invalid Components OcrCode4 ''' + :invalidOcrCode5 + '''' from dummy;
		RETURN;
	END if;


SELECT count(1) into isProject FROM #APZ_DOC_BOMLocalTempo a WHERE "Project" NOT IN (SELECT "PrjCode" FROM OPRJ b) AND RTRIM(LTRIM(Ifnull(a."Project",''))) <>'';

	IF(ifnull(isProject,0)=1)
	then
		select 17 ,'Invalid BOM Project' from dummy;
		RETURN;
	END if;
    SELECT count(1) into isProjectLine FROM #APZ_DOCLINE_BOMLocal a WHERE "Project" NOT IN (SELECT "PrjCode" FROM OPRJ b) AND RTRIM(LTRIM(Ifnull(a."Project",''))) <>'';
	IF (ifnull(isProjectLine,0)=1)
	then
		select 18, 'Invalid Components Project' from dummy;
		RETURN;
	END if;
    
	SELECT count(1) into isListNum  FROM #APZ_DOC_BOMLocalTempo a WHERE "PriceList" NOT IN (SELECT "ListNum" FROM OPLN WHERE RTRIM(LTRIM(Ifnull("ValidFor",''))) = 'Y') AND RTRIM(LTRIM(Ifnull(a."PriceList",0))) > 0;
	IF (ifnull(isListNum,0)=1)
	then
		select 19, 'Invalid BOM price list' from dummy;
		RETURN;
	END if;
	
    SELECT count(1) into isPriceList  FROM #APZ_DOCLINE_BOMLocal a WHERE "PriceList" NOT IN (SELECT "ListNum" FROM OPLN WHERE RTRIM(LTRIM(Ifnull("ValidFor",''))) = 'Y') AND RTRIM(LTRIM(Ifnull(a."PriceList",0))) > 0;
	IF (ifnull(isPriceList,0)=1)
	then
		select 20,'Invalid Components price list' from dummy;
		RETURN;
	END if;
    
    
    SELECT count(1) into isCurrentCy FROM #APZ_DOCLINE_BOMLocal a WHERE "Currency" NOT IN (SELECT "CurrCode" FROM OCRN WHERE RTRIM(LTRIM(Ifnull("Locked",''))) <> 'Y') AND RTRIM(LTRIM(Ifnull(a."Currency", ''))) <> '';
    
	IF   (ifnull(isCurrentCy,0)=1)
	then
		select 21, 'Invalid Components currency' from dummy;
		RETURN;
	END if;

if TransType='U' -- Update
then 
	-- Doc
	delete from  OITT where "Code"=:Code;
	insert into OITT select * from #APZ_DOC_BOMLocalTempo;

	-- Lines
	-- update father code to line
	UPDATE #APZ_DOCLINE_BOMLocal SET "Father" =:Code;
	delete from  ITT1 where "Father"=:Code; 
	INSERT into ITT1 select * from #APZ_DOCLINE_BOMLocal;
ELSEIF  TransType='A' -- Add new
then 	
	-- Doc
	delete from OITT where "Code"=:Code;
	insert into OITT select * from #APZ_DOC_BOMLocalTempo;

	-- Lines
	-- update father code to line
	UPDATE #APZ_DOCLINE_BOMLocal SET "Father" =:Code;
	delete from ITT1 where "Father"=:Code ;
	INSERT into ITT1 select * from #APZ_DOCLINE_BOMLocal;

	-- document numering
	UPDATE onnm SET "AutoKey" = "AutoKey" + 1 WHERE "ObjectCode"='66';
	
	
	end if;
	drop table #APZ_DOCLINE_BOMLocal;
	drop table #APZ_DOC_BOMLocalTempo;
end;




