

CREATE PROCEDURE APZ_SubActivities_RemoveComment
(
	IN Id int,
	IN ObjType nvarchar(100)
)
AS
BEGIN			

	CREATE LOCAL TEMPORARY TABLE #listComment ("CmtCode" int);
	--get list of comment need to be deleted
	INSERT INTO #listComment SELECT "CmtCode" FROM APZ_CLG1 WHERE ID = :Id AND "ObjType" = :ObjType;
	--delete comment
	DELETE FROM APZ_CLG1 WHERE "CmtCode" IN (SELECT * FROM #listComment);
	--delete comment notify
	DELETE FROM APZ_CLG3 WHERE "CmtCode" IN (SELECT * FROM #listComment);

END




