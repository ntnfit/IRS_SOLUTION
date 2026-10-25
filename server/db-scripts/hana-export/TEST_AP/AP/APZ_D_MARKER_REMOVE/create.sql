

CREATE PROCEDURE APZ_D_Marker_Remove(in code nvarchar(50) default '')
AS
BEGIN

	delete from APZ_D_MARKER where "Code" = :code; 
	select 0, '' from dummy;

END




