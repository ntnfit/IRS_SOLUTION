
CREATE PROCEDURE APZ_CFL_GetInfoColum (in userCode nvarchar(20) default '')
AS
BEGIN

	declare tblResult table ("Para" nvarchar(20), "ColumnField" nvarchar(50), "ColumnName" nvarchar(100), "DataType" nvarchar(10), "TableName" nvarchar(50), "ObjType" nvarchar(20), "UserSign" nvarchar(20), "FormId" nvarchar(100));
	declare tempResult table ("Para" nvarchar(20), "ColumnField" nvarchar(50), "ColumnName" nvarchar(100), "DataType" nvarchar(10), "TableName" nvarchar(50), "ObjType" nvarchar(20), row_id int);
	
	create local temporary table #tempData ("Para" nvarchar(50), "Column" nvarchar(50), "Table" nvarchar(10));
	exec 'insert into #tempData select * from "#tempTable"';
	exec 'drop table "#tempTable"';
	
	-- xóa các kí tự [, ] và " khỏi tên bảng, tên cột
	update #tempData set "Column" = replace(replace(replace("Column", '[', ''), ']', ''), '"', ''), "Table" = replace(replace(replace("Table", '[', ''), ']', ''), '"', '');
	
	INSERT INTO :tblResult
	SELECT DISTINCT a."Para", a."Column" AS "ColumnField"
					, CASE WHEN IFNULL(b."ColumnName", '') = '' AND IFNULL(d."Descr", '') = '' THEN a."Column" WHEN IFNULL(b."ColumnName", '') <> '' THEN b."ColumnName" ELSE d."Descr" END AS "ColumnName", 
					(SELECT DATA_TYPE_NAME FROM TABLE_COLUMNS WHERE SCHEMA_NAME = CURRENT_SCHEMA and TABLE_NAME = a."Table" AND COLUMN_NAME = a."Column") AS "DataType",
					 a."Table" AS "TableName", c."ObjType", b."UserSign", b."FormId"
	FROM #tempData a LEFT JOIN APZ_FORMSETTING b ON a."Column" = b."ColumnId" AND a."Table" = b."ItemId" AND b."UserSign" in (:userCode, '-') -- chỉ lấy column name trong formsetting của user đăng nhập hoặc mặc định
					 LEFT JOIN Apz_TblObject c ON b."ItemId" = c."TblName" OR b."ItemId" = c."TblLine" 
					 LEFT JOIN CUFD d ON a."Table" = d."TableID" AND a."Column" = 'U_' || d."AliasID" AND Left(a."Column", 2) = 'U_';
	
	
	-- cùng một columnId(ứng với một parameter) được lưu trong formsetting sẽ có nhiều tên khác nhau, vd: t1, t2,..
	-- thì description (label) của parameter sẽ hiển thị dạng: t1/ t2/ ...;
	-- Formsetting lấy theo user đăng nhập, nếu user chưa lưu formsetting thì lấy mặc định
	
	delete from :tblResult where ("Para", "ColumnField", "TableName", "FormId") in (select "Para", "ColumnField", "TableName", "FormId" from :tblResult where "UserSign" = :userCode) and "UserSign" <> :userCode; 
	
	-- xóa tên trùng nhau của cùng một parameter
	insert into :tempResult
	select "Para", "ColumnField", "ColumnName", "DataType", "TableName", "ObjType", row_number() over(partition by "Para", "ColumnName") from :tblResult;
	delete from :tempResult where row_id > 1;
	-- 
	SELECT "Para", '"' || max("ColumnField") || '"' "ColumnField", string_agg("ColumnName", '/ ') as "ColumnName", max("DataType") "DataType", '"' || max("TableName") || '"' "TableName", max("ObjType") "ObjType" from :tempResult group by "Para";
	
END

