
CREATE PROCEDURE APZ_Attachment_Add(in absEntry int)
AS
BEGIN

	if ifnull(:absEntry,0) = 0
	THEN
		select ifnull(max("AbsEntry"),0)+1 into absEntry From OATC;
        Update ONNM set "AutoKey"= :absEntry + 1 where "ObjectCode"='221';
	END IF;
	IF not exists(select 1 from OATC where "AbsEntry" = :absEntry)
	THEN
		insert into OATC ("AbsEntry") values (:absEntry);
	END IF;
	select :absEntry "AbsEntry" from DUMMY;

END




