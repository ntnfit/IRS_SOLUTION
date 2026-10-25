
CREATE PROCEDURE APZ_WorkScheduling_GetDataDoc(in branch int)
AS
BEGIN
	select * from APZ_OWSH where ifnull("BplID",-1) = :branch;
END;



