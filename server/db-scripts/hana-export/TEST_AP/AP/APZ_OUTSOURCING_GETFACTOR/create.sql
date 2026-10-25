
CREATE PROCEDURE APZ_Outsourcing_GetFactor
(
	in itemCode nvarchar(50) default '',
	in whsCode nvarchar(50) default ''
)
AS
BEGIN
	Select 1 From Dummy;
END

