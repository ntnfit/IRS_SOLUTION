
CREATE PROCEDURE Apz_TaxGroup_LoadById(
	in code NVARCHAR(8) default ''
	)
AS
BEGIN
	Select "Code","Name","Rate" From OVTG Where IFNULL("Inactive",'N') = 'N' AND IFNULL("Code",'') = :code;
END;

