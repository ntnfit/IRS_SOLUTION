
CREATE PROCEDURE APZ_SaveEducationType
AS
BEGIN
declare i int;
create local temporary table #temp_OHED as (select top 0* from OHED);
	
	-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
	execute immediate 'insert into #temp_OHED select * from "#dtOHED"';
	
	
	
     DELETE from  OHED;
     INSERT INTO OHED
     SELECT * FROM #temp_OHED;
     SELECT IFNULL((SELECT IFNULL(MAX("edType"),0) FROM OHED) ,0) into i from DUMMY;
     UPDATE ONNM SET "AutoKey"=:i +1 WHERE "ObjectCode"='175';
END




