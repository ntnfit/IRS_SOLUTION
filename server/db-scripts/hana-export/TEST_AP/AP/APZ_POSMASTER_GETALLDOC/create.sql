
CREATE  PROCEDURE APZ_PosMaster_GetAllDoc
AS
BEGIN
	Select a.* from APZ_RPOS a inner join APZ_STORE b on a."Store" = b."Code" and b."Locked" = 'N';
END




