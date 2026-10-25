

CREATE PROCEDURE Apz_D_GetEventListenerApp (IN Pending char(1), IN Success char(1))
AS
BEGIN
	SELECT * FROM APZ_D_ELN1 WHERE "Pending" = :Pending AND "Success" = :Success;
END



