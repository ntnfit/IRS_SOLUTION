
CREATE PROCEDURE APZ_MRP_GetEndateForecast
(
	in code nvarchar(4000) default ''
)
AS
BEGIN
	select max("EndDate") "EndDate" from (select "EndDate" from APZ_OFCT where "Code" in (select trim(Data) from Split(:code,',')));
END;



