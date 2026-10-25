
CREATE PROCEDURE APZ_BomVersion_GetDataLine(in Father int)
AS
BEGIN
	select a.*, case 

			when a."Type" = 4 then b."ItemName"
			when a."Type" = 290 then c."ResName"
			when a."Type" = -18 then ''

		end "ItemName",
		case

				when ifnull(a."OcrCode",'') = '' then ''
				when ifnull(a."OcrCode2",'') = '' then a."OcrCode"
				when ifnull(a."OcrCode3", '') = '' then a."OcrCode" || ',' || a."OcrCode2"
				when ifnull(a."OcrCode4", '') = '' then a."OcrCode" || ',' || a."OcrCode2" || ',' || a."OcrCode3"
				when ifnull(a."OcrCode5", '') = '' then a."OcrCode" || ',' || a."OcrCode2" || ',' || a."OcrCode3" || ',' || a."OcrCode4"
				else a."OcrCode" || ',' || a."OcrCode2" || ',' || a."OcrCode3" || ',' || a."OcrCode4" || ',' || a."OcrCode5"

		   end "DisRule"
			from APZ_ITT1 a left join OITM b on a."Code" = b."ItemCode"
						left join ORSC c on a."Code" = c."ResCode" where "Father" = :Father;
END;



