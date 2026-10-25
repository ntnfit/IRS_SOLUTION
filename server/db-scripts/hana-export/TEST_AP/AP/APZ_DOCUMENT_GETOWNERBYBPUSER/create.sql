
--	CALL Apz_UDR_APZ_Document_GetOwnerByBpUser;
CREATE PROCEDURE APZ_Document_GetOwnerByBpUser (
	IN 	cardCode NVARCHAR(100)
,		userID INT
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN
	declare slpCode int;
    declare salesEmpid int = 0;
    declare userEmpId int;
    select ifnull((select "SlpCode" From OCRD where "CardCode" = cardCode), 0) into slpCode from dummy;

	-- have sales/buyer persons
	if slpCode > 0
	then
		select ifnull((select "empID" from ohem where "salesPrson" = slpCode), 0) into salesEmpid from dummy;
		-- sales/buyer link to employee
		if salesEmpid > 0
		then
			select salesEmpid "OwnerCode",ifnull("lastName", '') || ', ' || ifnull("firstName", '') "OwnerName", slpCode "SlpCode" From OHEM where "empID" = salesEmpid;
			return;
		end if;
	end if;
	
	if salesEmpid <= 0
	then
		select ifnull((select top 1 "empID" from ohem where "userId" = userID), 0) into userEmpId from DUMMY;
		if userEmpId > 0
	then
			select userEmpId "OwnerCode", ifnull("lastName", '') || ', ' || ifnull("firstName", '') "OwnerName", 0 "SlpCode" From OHEM where "empID" = userEmpId;
		else
			select 0 "OwnerCode",'' "OwnerName", 0 "SlpCode" from DUMMY;
	    end if;
	end if;
END;




