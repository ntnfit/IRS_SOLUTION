
CREATE PROCEDURE APZ_Commons_DocumentGetComBoTabAccounting(in Cond Nvarchar(254) default '') 
as
BEGIN
		SELECT "GroupNum" AS "Code", "PymntGroup" AS "Name" FROM OCTG;
		SELECT "PayMethCod" AS "Code", "Descript" AS "Name" FROM OPYM;
		SELECT "Code", "Name" FROM OIDC;

END;




