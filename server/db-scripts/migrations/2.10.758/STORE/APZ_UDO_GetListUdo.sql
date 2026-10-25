
CALL APZ_EXISTSPROC('APZ_UDO_GetListUdo');

GO


CREATE PROCEDURE APZ_UDO_GetListUdo
AS
    BEGIN
		
		DECLARE maxid INT;
		DECLARE faid INT;
		declare checkCount int = 0;
		
		declare tbl table
        (
            "ID" INT
			,"FatherId" INT
			,"Code" NVARCHAR(100)
			,"Name" NVARCHAR(100)
			,"LanguageKey" NVARCHAR(254)
			,"MenuType" NVARCHAR(254)
        );
		
		-- insert UDO mới tạo (nếu có) vào bảng layout
		INSERT INTO APZ_PRINTLAYOUT ("TypeCode", "TypeName", "DocCode", "DocName","FileName") 
            SELECT "Code", "Name", "TableName", "MenuCapt",'' FROM OUDO 
            WHERE NOT EXISTS 
                            (SELECT * FROM APZ_PRINTLAYOUT 
                                WHERE OUDO."Code" = APZ_PRINTLAYOUT."TypeCode");

		-- lấy ra max id của APZ_OMNI	
		select IFNULL((SELECT MAX("Id") FROM APZ_OMNI),0) into maxid from DUMMY;
		-- kiểm tra bảng APZ_OMNI đã có barSub_UDO chưa thì tạo
		if not exists (SELECT 1 FROM APZ_OMNI WHERE "Code" = N'barSub_UDO' AND "FatherId" = -1 AND "Name" = N'User Defined Object') then
			INSERT INTO APZ_OMNI ("Id", "FatherId", "Code", "Name", "LanguageKey", "MenuType")
			VALUES (:maxid+1, -1 ,N'barSub_UDO', N'User Defined Object', NULL, NULL);
			maxid = maxid + 1;
		end if;
		-- lấy ra id của barSub_UDO
		select ifnull((SELECT top 1 "Id" FROM APZ_OMNI WHERE "Code" = N'barSub_UDO' AND "FatherId" = -1 AND "Name" = N'User Defined Object'),0) into faid from DUMMY;

		INSERT INTO :tbl ("ID", "FatherId", "Code", "Name", "LanguageKey", "MenuType")
			SELECT row_number() over(order by "Code"), :faid, 'UDO_' || "Code", "MenuCapt", 'en-En', 'O'
            FROM OUDO
		    WHERE NOT EXISTS ( SELECT 1 FROM APZ_OMNI x
							    WHERE OUDO."Code" = REPLACE(x."Code", 'UDO_', '') AND x."MenuType" = 'O') 
                                        AND "MenuItem" = 'Y';
		INSERT INTO APZ_OMNI ("Id", "FatherId", "Code", "Name", "LanguageKey", "MenuType")
			SELECT maxid + "ID", "FatherId", "Code", "Name", "LanguageKey", "MenuType"
				FROM :tbl;
		
		-- update menu name (TH update trên sap)
		UPDATE APZ_OMNI SET "Name" = (SELECT x."MenuCapt" FROM OUDO x WHERE x."Code" = REPLACE(APZ_OMNI."Code", 'UDO_', '') AND x."MenuItem" = 'Y')
			WHERE "MenuType" = 'O';
		-- Xóa các UDO bỏ menu trên sap khỏi menu trên irs
		DELETE FROM APZ_OMNI WHERE "MenuType" = 'O' AND "Code" NOT IN (SELECT 'UDO_'|| x."Code" FROM OUDO x WHERE x."MenuItem" = 'Y');

		SELECT * FROM OUDO;
    END;