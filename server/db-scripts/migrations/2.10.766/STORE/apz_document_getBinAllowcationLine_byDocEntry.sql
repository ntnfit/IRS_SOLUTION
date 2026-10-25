
CALL APZ_EXISTSPROC('apz_document_getBinAllowcationLine_byDocEntry');

GO

CREATE PROCEDURE apz_document_getBinAllowcationLine_byDocEntry(
in docEntry nvarchar(5000) default null,
in ObjType VARCHAR(20))
AS
begin

	call apz_fun_document_getBinAllowcationLine_byDocEntry(:docentry, :objType, bin);
	select * from :bin a left join OBTN b on 1 = 0;

end