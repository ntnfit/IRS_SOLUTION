
CREATE PROCEDURE apz_saveCategory
(	in catID int,
	in name nvarchar(5000),
	in userSign smallint)
AS
BEGIN
declare docnum int;
declare idCheck int default 0;
select IFNULL((select TOP 1 a."CategoryId" from OQCN a where a."CategoryId" = :catID ),0) into idCheck from Dummy;
if (idCheck <>0)
then
	BEGIN /* AutoKey*/
	
			-- Get new CategoryID from numering and series table
			
			select IFNULL((Select top 1 IFNULL( "AutoKey",1) from onnm where "ObjectCode" = '134'),0) into docnum from Dummy;

			-- set new card code to temp table
			-- increase next auto number
			UPDATE onnm set "AutoKey" = :docnum + 1  where "ObjectCode" = '134';	


	insert into OQCN ("CategoryId", "CatName", "PermMask", "UserSign") values (:docnum, :name, 'YYYYYYYYYYYYYY', :userSign);
	END;
else
	
	update OQCN SET "CatName" = :name WHERE "CategoryId" = :catID;
	
	END IF;
END 




