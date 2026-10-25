
CREATE PROCEDURE apz_formsetting_copy(
	in ItemId VARCHAR(50),
	in FormId VARCHAR(254),
	in UserName VARCHAR(20),
	in CopyUser varchar(4000),
	in CopyAll CHAR(1),
	in UserUpdate int
)
AS
BEGIN
	--IF	@UserName='manager'
	--BEGIN	
	--	SET @UserName=''
	--END	
	IF :CopyAll='Y'
	then
		DELETE from APZ_FORMSETTING WHERE "UserSign"<>:UserName and "UserSign" in (SELECT Data FROM Split(:CopyUser,','));

		INSERT INTO APZ_FORMSETTING
		        ( "FormId", "ItemId", "ColumnId", "UserSign", "Width", "Visible", "VisIndex", "Editable", "IsReadOnly", "ColumnName", "AllowChSet", "VisSetting", "Fixed", "UserUpdate", "UpdateDate", "GroupIndex")
		SELECT "FormId", "ItemId", "ColumnId", x.data, "Width", "Visible", "VisIndex", "Editable", "IsReadOnly", "ColumnName", "AllowChSet", "VisSetting", "Fixed",  "UserUpdate", CAST(CURRENT_DATE as date), "GroupIndex" FROM APZ_FORMSETTING a
		inner join (
			SELECT Data FROM Split(:CopyUser,',')
		) x ON 1 = 1
		 WHERE "UserSign"=:UserName AND (x.Data<>:UserName);
		 	
	ELSE
		DELETE FROM APZ_FORMSETTING WHERE "UserSign" <> :UserName AND "ItemId" = :ItemId AND "FormId" = :FormId and "UserSign" in (SELECT Data FROM Split(:CopyUser,','));

		INSERT INTO APZ_FORMSETTING ("FormId", "ItemId", "ColumnId", "UserSign", "Width", "Visible", "VisIndex", "Editable", "IsReadOnly", "ColumnName", "AllowChSet", "VisSetting", "Fixed", "UserUpdate", "UpdateDate", "GroupIndex") 
		SELECT "FormId", "ItemId", "ColumnId", x.data, "Width", "Visible", "VisIndex", "Editable", "IsReadOnly", "ColumnName", "AllowChSet", "VisSetting", "Fixed",  "UserUpdate", CAST(CURRENT_DATE as date), "GroupIndex" FROM APZ_FORMSETTING a
		inner join (
			SELECT Data FROM Split(:CopyUser,',')
		) x on 1 = 1
		 WHERE "UserSign"=:UserName  AND "ItemId"=:ItemId AND "FormId"=:FormId AND (x.Data<>:UserName );
		 
    END	IF;
END



