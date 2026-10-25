
CREATE PROCEDURE APZ_Commons_GetSetingCflObject(in userName nvarchar(50), in objType nvarchar(20), in lngCode nvarchar(50))
AS
BEGIN
	declare checkCount tinyint = 0;
	declare currentLng nvarchar(50);
	
	select IFNULL(:userName,'manager') into userName from DUMMY;
	select IFNULL(:lngCode,'-') into lngCode from DUMMY;
	currentLng = :lngCode;
	if :userName <> '-'
	THEN
		 currentLng = '-';
	END IF;
	
	select IFNULL((select top 1 1 from apz_formsetting where "UserSign" = :userName and "FormId" = 'CFL' and "ItemId" = 'UserDefinedData' || '-' || :objType),0) into checkCount from DUMMY;
	
	if checkCount = 0
	then
	 	userName = 'manager';
	end if;
	
	select IFNULL((select top 1 1 from apz_formsetting where "UserSign" = :userName and "FormId" = 'CFL' and "ItemId" = 'UserDefinedData' || '-' || :objType),0) into checkCount from DUMMY;
	if checkCount = 0
	then
	 	userName = '-';
	end if;
	
	select STRING_AGG('"' || "ColumnId" || '"', ',') "ColumnName" from APZ_FORMSETTING a WHERE "UserSign" = :userName and "FormId" = 'CFL' and "ItemId" = 'UserDefinedData' || '-' || :objType and "Visible" = 1;

end;




