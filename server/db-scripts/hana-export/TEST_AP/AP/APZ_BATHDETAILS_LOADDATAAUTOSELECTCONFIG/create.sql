
CREATE PROCEDURE APZ_BathDetails_LoadDataAutoSelectConfig
AS
BEGIN
	--------------------- biến để check dữ liệu --------------------------------
	declare checkCount integer = 0;
	
	create local temporary table #tempColumn("ColumnId" varchar(50), "ColumnName" nvarchar(254));
	
	insert into #tempColumn("ColumnId", "ColumnName")
	Select COLUMN_NAME "ColumnId", case COLUMN_NAME
		when 'ItemCode' then 'Item No'
		when 'SysNumber' then 'System Number'
		when 'DistNumber' then 'Batch Number'
		when 'MnfSerial' then 'Batch Attribute 1'
		when 'LotNumber' then 'Batch Attribute 2'
		when 'ExpDate' then 'Expiration Date'
		when 'MnfDate' then 'Manufacturing Date'
		when 'InDate' then 'Admission Date'
		when 'GrntStart' then 'Warranty Start Date'
		when 'GrntExp' then 'Warranty End Date'
		when 'CreateDate' then 'Creation Date'
		when 'Location' then 'Location'
		when 'Status' then 'Status'
		when 'Notes' then 'Details'
		when 'DataSource' then 'Data Source'
		when 'UserSign' then 'User Signature'
		when 'Transfered' then 'Transferred'
		when 'Instance' then 'Instance'
		when 'AbsEntry' then 'Abs Entry'
		when 'ObjType' then 'Object Type'
		when 'itemName' then 'Item Description'
		when 'LogInstanc' then 'Log Instance'
		when 'UserSign2' then 'Updating User'
		when 'UpdateDate' then 'Update Date'
		when 'CostTotal' then 'Total Cost of Batches'
		when 'Quantity' then 'Quantity'
		when 'QuantOut' then 'Output Quantity'
		when 'PriceDiff' then 'Price Difference'
		when 'Balance' then 'Batch Balance'
		when 'TrackingNt' then 'CCD Tracking Note'
		when 'TrackiNtLn' then 'CCD Tracking Note Line'
		when 'SumDec' then 'SumDec'
	end "ColumnName" FROM TABLE_COLUMNS WHERE TABLE_NAME = N'OBTN' AND SCHEMA_NAME = CURRENT_SCHEMA;
	
	--Update Udf column with unicode
	UPDATE a SET a."ColumnName" = b."Descr" FROM #tempColumn a INNER JOIN CUFD b ON "TableID" = 'OBTN' AND a."ColumnId" = CAST('U_' || b."AliasID" AS nvarchar) WHERE LEFT(a."ColumnId", 2) = 'U_';
	SELECT * FROM #tempColumn;

	select IFNULL((select count(1) from APZ_OABC),0) into checkCount from DUMMY;
	
	if :checkCount = 0
	then
		INSERT INTO APZ_OABC 
			select 'AbsEntry', 'A', 1, 'manager' from DUMMY
		UNION ALL
			select 'SysNumber', 'A', 2, 'manager' FROM DUMMY;
	END IF;
	SELECT * FROM APZ_OABC order by "SortId";

END;



