
CREATE PROCEDURE SP_APZ_OUOM_SAVE
AS
BEGIN
	create local temporary table #temp_OUOM as (select top 0* from OUOM);
	
	-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
	exec 'insert into ##temp_OUOM select * from "#OUOM"'; 
	DELETE FROM OUOM;

	update #temp_OUOM set "UpdateDate" = CURRENT_DATE;
	INSERT INTO OUOM
           
	SELECT *
	FROM #temp_OUOM a
	WHERE a."UomEntry" <> 0 AND a."UomCode" <> '' AND a."UomName" <> '';

	UPDATE ONNM SET "AutoKey" = (Select MAX("UomEntry") FROM OUOM) +1 WHERE "ObjectCode" = '10000199';
	--SELECT a.Length1Disp + CONVERT(nvarchar,a.Length1) FROM #OUOM a WHERE a.UomEntry = '13'

END




