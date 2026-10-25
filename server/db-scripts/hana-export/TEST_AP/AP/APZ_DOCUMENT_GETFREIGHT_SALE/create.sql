
create PROCEDURE Apz_Document_GetFreight_Sale(in Tbl nvarchar(50) default '', DocEntry nvarchar(4000) default '')
AS
BEGIN
	--Lấy thêm cột DocNum để điền vào Base Document trong form chọn Freight
	declare docTable nvarchar(10);
	declare checkCount int = 0;
	declare sqlString nvarchar(40000)='';
	select IFNULL((SELECT 1 FROM TABLES WHERE SCHEMA_NAME = current_schema and TABLE_NAME =:Tbl),0) into checkCount from DUMMY;
	
	IF :checkCount = 1
	THEN
		select 'O' || substring(:Tbl, 0, 3) into docTable from dummy;
		create local temporary table #tblDocEntry (Data nvarchar(50));
		insert into #tblDocEntry select Data from Split(:DocEntry, ',');
		
		sqlString = 
		N'Select
			a.*, b.*, c."DocNum"
		From ' || :Tbl || ' a Inner join OEXD b ON a."ExpnsCode"=b."ExpnsCode"' || ' inner join ' || :docTable || ' c on a."DocEntry" = c."DocEntry"' || ' Where cast(a."DocEntry" as nvarchar) in (select Data from #tblDocEntry);';
		--select :sqlstring from dummy;
		execute immediate :sqlString;
		drop table #tblDocEntry;
	END IF;
end



