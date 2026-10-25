

CREATE PROCEDURE Apz_D_GetEventListener (IN Pending char(1) DEFAULT 'Y')
AS
BEGIN
	SELECT * FROM APZ_D_OELN WHERE "Pending" = :Pending;
END




