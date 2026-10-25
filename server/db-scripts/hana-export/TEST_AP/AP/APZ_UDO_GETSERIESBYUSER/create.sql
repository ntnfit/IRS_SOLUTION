
CREATE PROCEDURE APZ_UDO_GetSeriesByUser(in udoCode nvarchar(50), in userCode nvarchar(50))
AS
BEGIN
	declare dflSeries int = ifnull((select a."Series" from NNM2 a join OUSR b on a."UserSign"=b.USERID where a."ObjectCode"=:udoCode and b.USER_CODE=:userCode), 0);
	if :dflSeries = 0 then
		dflSeries = ifnull((select "DfltSeries" from ONNM where "ObjectCode"=:udoCode), 0);
	end if;
	
	select "Series", "SeriesName", :dflSeries "Default" from NNM1 where "ObjectCode" = :udoCode and cast("GroupCode" as nvarchar) in 
			(select substring("menucode", 19) from oprm where "menucode" like 'barSub_SeriesGroup%' and USER_CODE=:userCode and "authoriza"='F');
END



