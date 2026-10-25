
CREATE PROCEDURE APZ_Shift_GetData(in branch int)
AS
BEGIN
	Select * from APZ_OSHP where ifnull("BplId", 0) = :branch;
END



