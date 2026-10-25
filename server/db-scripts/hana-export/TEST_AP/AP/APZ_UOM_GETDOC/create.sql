
CREATE PROCEDURE APZ_UoM_GetDoc
AS
BEGIN

	SELECT c."UomEntry"
      ,c."UomCode"
      ,c."UomName"
      ,c."Locked"
      ,c."DataSource"
      ,c."UserSign"
      ,c."LogInstanc"
      ,c."UserSign2"
      ,c."UpdateDate"
      ,c."CreateDate"
	  ,c."Length1"
      ,c."Len1Unit"
	  --Nếu cả 2 giá trị 1 và 2 của các số đo đều khác 0 hoặc chỉ giá trị đầu tiên khác 0 thì sẽ hiển thị nếu không thì sẽ hiển thị chuỗi rỗng
	  ,CASE WHEN IFNULL(c."Length1",0) != 0 AND IFNULL(c."length2",0) != 0
				--Cắt bỏ bớt số '0' thừa đằng sau giá trị trả về 
				--"REPLACE(RTRIM(REPLACE(CONVERT(nvarchar,CONVERT(numeric(19,2),c."Length1")),'0',' ')),' ','0')"

				--Đầu tiên là thay thế tất cả các số 0 trong giá trị bằng 1 dấu cách. Ví dụ: 1.1020 => 1.1' '2' '
				--Sau đó cắt phần trắng ở đuôi bằng hàm RTRIM mục đich để loại bỏ số 0 thừa đằng sau => 1.1' '2
				--Trả lại số 0 ở giữa giá trị bằng cách thay thế dấu cách ' ' bằng số 0 => 1.102

				--Sau khi cắt số 0, giả sử số '1234.000' sẽ thành '1234.' => cần cắt bỏ cả dấu chấm thừa bằng cách làm như phần trên
				THEN REPLACE(RTRIM(REPLACE(REPLACE(RTRIM(REPLACE(TO_NVARCHAR(TO_DECIMAL(c."Length1",19,2)),'0',' ')),' ','0'),'.',' ')),' ','.') || IFNULL((select "UnitDisply" from (select "UnitDisply", row_number () over 
               (partition by a."UnitDisply" 
                ORDER BY a."UnitDisply" asc) as RN FROM OLGT a LEFT JOIN OUOM b ON a."UnitCode" = b."Len1Unit"  Where a."UnitCode" = c."Len1Unit") where rn = 2),'') || ' '
					|| REPLACE(RTRIM(REPLACE(REPLACE(RTRIM(REPLACE(TO_NVARCHAR(TO_DECIMAL(c."length2",19,2)),'0',' ')),' ','0'),'.',' ')),' ','.') || IFNULL((select "UnitDisply" from (select "UnitDisply", row_number () over 
               (partition by a."UnitDisply" 
                ORDER BY a."UnitDisply" asc) as RN FROM OLGT a LEFT JOIN OUOM b ON a."UnitCode" = b."Len2Unit"  Where a."UnitCode" = c."Len2Unit") where rn = 2),'')
			ELSE
				CASE WHEN IFNULL(c."Length1",0) != 0 AND IFNULL(c."length2",0) = 0
					THEN REPLACE(RTRIM(REPLACE(REPLACE(RTRIM(REPLACE(TO_NVARCHAR(TO_DECIMAL(c."Length1",19,2)),'0',' ')),' ','0'),'.',' ')),' ','.') || IFNULL((select "UnitDisply" from (select "UnitDisply", row_number () over 
               (partition by a."UnitDisply" 
                ORDER BY a."UnitDisply" asc) as RN FROM OLGT a LEFT JOIN OUOM b ON a."UnitCode" = b."Len1Unit"  Where a."UnitCode" = c."Len1Unit") where rn = 2),'') 
					
					ELSE '' 
					END
			END as "Length1Disp"

	  ,c."length2"
	  ,c."Len2Unit"
      ,c."Width1"
	  ,c."Wdth1Unit"
	  --Nếu cả 2 giá trị 1 và 2 của các số đo đều khác 0 hoặc chỉ giá trị đầu tiên khác 0 thì sẽ hiển thị nếu không thì sẽ hiển thị chuỗi rỗng
	  ,CASE WHEN IFNULL(c."Width1",0) != 0 AND IFNULL(c."Width2",0) != 0
				THEN REPLACE(RTRIM(REPLACE(REPLACE(RTRIM(REPLACE(TO_NVARCHAR(TO_DECIMAL(c."Width1",19,2)),'0',' ')),' ','0'),'.',' ')),' ','.') || IFNULL((select "UnitDisply" from (select "UnitDisply", row_number () over 
               (partition by a."UnitDisply" 
                ORDER BY a."UnitDisply" asc) as RN FROM OLGT a LEFT JOIN OUOM b ON a."UnitCode" = b."Wdth1Unit"  Where a."UnitCode" = c."Wdth1Unit") where rn = 2),'')  || ' '
					|| REPLACE(RTRIM(REPLACE(REPLACE(RTRIM(REPLACE(TO_NVARCHAR(TO_DECIMAL(c."Width2",19,2)),'0',' ')),' ','0'),'.',' ')),' ','.') || IFNULL((select "UnitDisply" from (select "UnitDisply", row_number () over 
               (partition by a."UnitDisply" 
                ORDER BY a."UnitDisply" asc) as RN FROM OLGT a LEFT JOIN OUOM b ON a."UnitCode" = b."Wdth2Unit"  Where a."UnitCode" = c."Wdth2Unit") where rn = 2),'')
			ELSE
				CASE WHEN IFNULL(c."Width1",0) != 0 AND  IFNULL(c."Width2",0) = 0
					THEN REPLACE(RTRIM(REPLACE(REPLACE(RTRIM(REPLACE(TO_NVARCHAR(TO_DECIMAL(c."Width1",19,2)),'0',' ')),' ','0'),'.',' ')),' ','.') || IFNULL((select "UnitDisply" from (select "UnitDisply", row_number () over 
               (partition by a."UnitDisply" 
                ORDER BY a."UnitDisply" asc) as RN FROM OLGT a LEFT JOIN OUOM b ON a."UnitCode" = b."Wdth1Unit"  Where a."UnitCode" = c."Wdth1Unit") where rn = 2),'')
					
					ELSE '' 
					END
			END as "Width1Disp"
			
      ,c."Width2"
	  ,c."Wdth2Unit"
      ,c."Height1"
	  ,c."Hght1Unit"
	  --Nếu cả 2 giá trị 1 và 2 của các số đo đều khác 0 hoặc chỉ giá trị đầu tiên khác 0 thì sẽ hiển thị nếu không thì sẽ hiển thị chuỗi rỗng
      ,CASE WHEN IFNULL(c."Height1",0) != 0 AND IFNULL(c."Height2",0) != 0
				THEN REPLACE(RTRIM(REPLACE(REPLACE(RTRIM(REPLACE(TO_NVARCHAR(TO_DECIMAL(c."Height1",19,2)),'0',' ')),' ','0'),'.',' ')),' ','.') || IFNULL((select "UnitDisply" from (select "UnitDisply", row_number () over 
               (partition by a."UnitDisply" 
                ORDER BY a."UnitDisply" asc) as RN FROM OLGT a LEFT JOIN OUOM b ON a."UnitCode" = b."Hght1Unit"  Where a."UnitCode" = c."Hght1Unit") where rn = 2),'') || ' '
					|| REPLACE(RTRIM(REPLACE(REPLACE(RTRIM(REPLACE(TO_NVARCHAR(TO_DECIMAL(c."Height2",19,2)),'0',' ')),' ','0'),'.',' ')),' ','.') || IFNULL((select "UnitDisply" from (select "UnitDisply", row_number () over 
               (partition by a."UnitDisply" 
                ORDER BY a."UnitDisply" asc) as RN FROM OLGT a LEFT JOIN OUOM b ON a."UnitCode" = b."Hght2Unit"  Where a."UnitCode" = c."Hght2Unit") where rn = 2),'')
			ELSE
				CASE WHEN IFNULL(c."Height1",0) != 0 AND IFNULL(c."Height2",0) = 0
					THEN REPLACE(RTRIM(REPLACE(REPLACE(RTRIM(REPLACE(TO_NVARCHAR(TO_DECIMAL(c."Height1",19,2)),'0',' ')),' ','0'),'.',' ')),' ','.') || IFNULL((select "UnitDisply" from (select "UnitDisply", row_number () over 
               (partition by a."UnitDisply" 
                ORDER BY a."UnitDisply" asc) as RN FROM OLGT a LEFT JOIN OUOM b ON a."UnitCode" = b."Hght1Unit"  Where a."UnitCode" = c."Hght1Unit") where rn = 2),'')
					
					ELSE '' 
					END
			END as "Height1Disp"
			
      ,c."Height2"
	  ,c."Hght2Unit"
      --,CASE WHEN CONVERT(nvarchar,CONVERT(int,c.Volume)) != '0'THEN CONVERT(nvarchar,CONVERT(int,c.Volume)) ELSE '' END as VolumeDisp
	  ,CASE WHEN IFNULL(c."Volume",0) = 0 THEN NULL ELSE TO_DECIMAL(c."Volume",19,2) END as "Volume"
      ,c."VolUnit"
	  ,c."Weight1"
	  ,c."WghtUnit"
	  --Nếu cả 2 giá trị 1 và 2 của các số đo đều khác 0 hoặc chỉ giá trị đầu tiên khác 0 thì sẽ hiển thị nếu không thì sẽ hiển thị chuỗi rỗng
	  ,CASE WHEN IFNULL(c."Weight1",0) != 0 AND IFNULL(c."Weight2",0) != 0
				THEN REPLACE(RTRIM(REPLACE(REPLACE(RTRIM(REPLACE(TO_NVARCHAR(TO_DECIMAL(c."Weight1",19,2)),'0',' ')),' ','0'),'.',' ')),' ','.') || IFNULL((select "UnitDisply" from (select "UnitDisply", row_number () over 
               (partition by a."UnitDisply" 
                ORDER BY a."UnitDisply" asc) as RN FROM OWGT a LEFT JOIN OUOM b ON a."UnitCode" = b."WghtUnit"  Where a."UnitCode" = c."WghtUnit") where rn = 2),'') || ' '
					|| REPLACE(RTRIM(REPLACE(REPLACE(RTRIM(REPLACE(TO_NVARCHAR(TO_DECIMAL(c."Weight2",19,2)),'0',' ')),' ','0'),'.',' ')),' ','.') || IFNULL((select "UnitDisply" from (select "UnitDisply", row_number () over 
               (partition by a."UnitDisply" 
                ORDER BY a."UnitDisply" asc) as RN FROM OWGT a LEFT JOIN OUOM b ON a."UnitCode" = b."Wght2Unit"  Where a."UnitCode" = c."Wght2Unit") where rn = 2),'')
			ELSE
				CASE WHEN IFNULL(c."Weight1",0) != 0 AND IFNULL(c."Weight2",0) = 0
					THEN REPLACE(RTRIM(REPLACE(REPLACE(RTRIM(REPLACE(TO_NVARCHAR(TO_DECIMAL(c."Weight1",19,2)),'0',' ')),' ','0'),'.',' ')),' ','.') || IFNULL((select "UnitDisply" from (select "UnitDisply", row_number () over 
               (partition by a."UnitDisply" 
                ORDER BY a."UnitDisply" asc) as RN FROM OWGT a LEFT JOIN OUOM b ON a."UnitCode" = b."WghtUnit"  Where a."UnitCode" = c."WghtUnit") where rn = 2),'')
					
					ELSE '' 
					END
			END as "Weight1Disp"
      
      ,c."Weight2"
	  ,c."Wght2Unit"
      ,c."IntSymbol"
  FROM OUOM c order by "UomEntry";
END
--APZ_UoM_GetDoc

--SELECT a.VolDisply FROM OLGT a LEFT JOIN OUOM b ON a."UnitCode" = b.Len1Unit




