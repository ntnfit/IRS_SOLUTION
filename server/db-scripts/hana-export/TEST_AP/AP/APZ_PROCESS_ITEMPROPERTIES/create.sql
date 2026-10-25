
CREATE PROCEDURE APZ_Process_ItemProperties
AS
BEGIN

	CREATE LOCAL TEMPORARY TABLE #OITGLocal as(select top 0 * from OITG);
	exec 'DROP TABLE #OITGLocal';
	exec 'CREATE LOCAL TEMPORARY TABLE #OITGLocal as (select * from "#dtOITG")';

	DELETE FROM OITG;
    INSERT INTO OITG("ItmsGrpNam", "ItmsTypCod", "UserSign") SELECT "ItmsGrpNam", "ItmsTypCod", "UserSign" FROM #OITGLocal; 
    UPDATE ONNM SET "AutoKey" = (Select MAX("ItmsTypCod") FROM OITG) + 1 WHERE "ObjectCode" = '8';
    
END;




