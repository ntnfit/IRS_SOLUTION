

CREATE PROCEDURE APZ_PersonalIncomeTax_DeleteDocs(in code varchar(50) default '')
AS
BEGIN
	DELETE FROM APZ_PIT1 WHERE "Code" = :code;
	DELETE FROM APZ_OPIT WHERE "Code" = :code;
END;



