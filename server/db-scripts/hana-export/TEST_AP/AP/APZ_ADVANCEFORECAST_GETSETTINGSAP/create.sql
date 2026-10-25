
CREATE PROCEDURE APZ_AdvanceForecast_GetSettingSap

AS
BEGIN
	SELECT b."DateFormat", b."DateSep", a."WndFrm", a."WndTo", b."QtyDec" FROM OHLD a INNER JOIN OADM b ON a."HldCode" = b."HldCode";
END




