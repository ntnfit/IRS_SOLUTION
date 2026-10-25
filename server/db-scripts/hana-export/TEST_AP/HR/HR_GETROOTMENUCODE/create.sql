CREATE FUNCTION HR_GetRootMenuCode(IN p_Code NVARCHAR(50))
RETURNS o_RootCode NVARCHAR(50)
LANGUAGE SQLSCRIPT AS
BEGIN
	DECLARE v_CurrentCode  NVARCHAR(50) := p_Code;
	DECLARE v_FatherId     INT;
	DECLARE v_FatherCode   NVARCHAR(50);

    WHILE (1 = 1) DO
    	
        -- Lấy FatherId của node hiện tại
        v_FatherId = IFNULL((SELECT "FatherId" FROM APZ_OMNI WHERE "Code" = v_CurrentCode), -1);

        -- Nếu FatherId IS NULL => đang ở root, dừng
        IF :v_FatherId = -1 THEN
            BREAK;
        END IF;

        -- Lấy Code của cha theo FatherId
        v_FatherCode = IFNULL((SELECT "Code" FROM APZ_OMNI WHERE "Id" = v_FatherId), '');

        v_CurrentCode := v_FatherCode;
    END WHILE;

    o_RootCode := v_CurrentCode;
END;
