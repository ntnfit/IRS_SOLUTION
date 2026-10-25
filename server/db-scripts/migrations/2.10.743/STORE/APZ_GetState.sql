
CALL APZ_EXISTSPROC('APZ_GetState');

GO

CREATE PROCEDURE APZ_GetState(in Code nvarchar(50000) default null)
as
BEGIN 
	if(:Code <> null ) or (:Code <> '')
	THEN
		select A."Code" ,A."Country", A."Name" ,A."UserSign",A."eCode",A."GNRECode" from OCST A LEFT JOIN OCRY B ON A."Country" = B."Code" where B."Code" = :Code;
	else 
		select cast(null as nvarchar(3)) "Code", cast(null as nvarchar(3)) "Country", cast(null as nvarchar(100)) "Name", 0 "UserSign", 0 "eCode", cast(null as nvarchar(4)) "GNRECode" from dummy;
	END if;
END
