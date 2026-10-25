

CREATE  PROCedure apz_authorization_copy(in source NVARCHAR(100), in target NVARCHAR(50000))
AS
BEGIN

	DELETE from oprm WHERE "USER_CODE" IN (SELECT data FROM  Split(:target,','));
	
	INSERT INTO OPRM
	        ( USER_CODE, "menucode", "authoriza")
	SELECT a.Data,"menucode","authoriza" FROM oprm,(SELECT data FROM  Split(:target,',')) a WHERE USER_CODE=:source;

END



