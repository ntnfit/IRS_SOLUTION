
CALL APZ_EXISTSPROC('APZ_Series_LoadByPostingDate');

GO

CREATE PROCEDURE APZ_Series_LoadByPostingDate (in objectCode nvarchar(50), in docSubType nvarchar(2), in userCode nvarchar(50), in refDate datetime)
AS
BEGIN
	declare series int;
	
	if :objectCode = '13000' then
		objectCode = '13';
	elseif :objectCode = '25' then
		objectCode = '18';
	end if;
	
	select "Series" into series default -1 from NNM2 where "ObjectCode"=:objectCode and "DocSubType"=:docSubType and "UserSign"= (select USERID from OUSR where USER_CODE=:userCode);
	if :series = -1 then
		select "DfltSeries" into series default -1 from ONNM where "ObjectCode"=:objectCode and "DocSubType"=:docSubType;
	end if;

	SELECT "Series","SeriesName",IFNULL("BPLId",-1) "BPLId", case when "Series"=:series then 'Y' else 'N' end "IsDefault" FROM NNM1
	WHERE "ObjectCode"=:ObjectCode and "DocSubType"=:docSubType AND ("BPLId" IN (SELECT "BPLId" FROM USR6 WHERE "UserCode"=:UserCode) OR IFNULL("BPLId",-1)=-1)
	and "Indicator" in (select "Indicator" from OFPR where "F_RefDate" <= :refDate and "T_RefDate" >= :refDate)
	and cast("GroupCode" as nvarchar) in (select substring("menucode", 19) from oprm where "menucode" like 'barSub_SeriesGroup%' and USER_CODE=:userCode and "authoriza"='F');
END