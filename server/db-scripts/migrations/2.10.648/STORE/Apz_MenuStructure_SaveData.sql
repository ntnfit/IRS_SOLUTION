
CALL APZ_EXISTSPROC('Apz_MenuStructure_SaveData');

GO

CREATE PROCEDURE Apz_MenuStructure_SaveData 
(
	IN id INT DEFAULT -1,
	IN fatherId INT DEFAULT -1,
	IN code NVARCHAR(254) DEFAULT '',
	IN name NVARCHAR(254) DEFAULT '',
	IN language NVARCHAR(254) DEFAULT '',
	IN menuType NVARCHAR(1) DEFAULT '',
	IN type NVARCHAR(20) DEFAULT ''	-- Add/Delete/Relocate/Rename
)
AS
BEGIN

	IF :type = 'Add' THEN 
		DECLARE maxid integer := IFNULL((SELECT MAX("Id") FROM APZ_OMNI), 0);
		while exists(select 1 from APZ_OMNI where "Code"=:code) do
			code = :code || '_1';
		end while;
		INSERT INTO APZ_OMNI ("Id", "FatherId", "Code", "Name", "LanguageKey", "MenuType") VALUES (:maxid + 1, :fatherId, :code, :name, :language, :menuType);
	ELSEIF :type = 'Rename' THEN 
		DECLARE curCode nvarchar(254) := IFNULL((SELECT "Code" FROM APZ_OMNI WHERE "Id" = :id), '');
		-- cập nhật code, name mới
		UPDATE APZ_OMNI SET "Code" = :code, "Name" = :name WHERE "Id" = :id AND "FatherId" = :fatherId;
		--gán lại language cho menu có menu này làm cha
		UPDATE APZ_OMNI SET "LanguageKey" = :code WHERE "FatherId" = :id;
		-- gán lại father menu cho report form nếu có menu này làm cha
		UPDATE APZ_OUDR SET "FatherMenu" = :code WHERE "FatherMenu" = :curCode;
	ELSEIF :type = 'Relocate' THEN
		IF :menuType = 'O' AND :language = 'barSub_UDO' THEN
			language := '';
		END IF;
		IF :menuType = 'T' AND :language = 'barSub_UserDefineWindow' THEN
			language := '';
		END IF;
		UPDATE APZ_OMNI SET "FatherId" = :fatherId, "LanguageKey" = :language WHERE "Id" = :id;
	ELSEIF :type = 'Delete' THEN
		DELETE FROM APZ_OMNI WHERE "Id" = :id;
	END IF;

	SELECT 0, '' FROM DUMMY;
END