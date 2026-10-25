

CREATE PROCEDURE APZ_WBSTemplate_GetLineById(in code nvarchar(50) default '')
AS
BEGIN
	select * from APZ_TASK where "Code" = :code;
	select * from APZ_WBS1 where "Code" = :code;
END;



