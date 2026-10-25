CREATE PROCEDURE "SP_CHECK_EINVOICE_CONTEXT_ACTION"
(
    OUT O_Status  NVARCHAR(20),
    OUT O_Message NVARCHAR(200)
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
BEGIN

    DECLARE v_EInvoiceStatus  NVARCHAR(20);
    DECLARE v_EInvoiceMessage NVARCHAR(200);

    DECLARE v_EInvoiceHttpUri NVARCHAR(500);

    DECLARE v_Count			  INT;

    O_Status  := 'OK';
    O_Message := 'OK';

    /* =========================================================
       1. CHECK TOÀN BỘ CẤU HÌNH BKAV TRONG @EINVOICE
          Bao gồm:
          - PartnerGUID
          - PartnerToken
          - HttpUri
          - LinkViewInv
          - Name
          - Số dòng cấu hình
       ========================================================= */

    CALL "SP_CHECK_EINVOICE_CONFIG"
    (
        v_EInvoiceStatus,
        v_EInvoiceMessage
    );

    IF :v_EInvoiceStatus = 'ERROR' THEN
        O_Status  := 'ERROR';
        O_Message := :v_EInvoiceMessage;
        RETURN;
    END IF;

    /* =========================================================
       2. LẤY HttpUri CHUẨN TỪ @EINVOICE
       ========================================================= */

    SELECT TOP 1 TRIM("U_HttpUri" )
    INTO v_EInvoiceHttpUri DEFAULT ''
    FROM "@EINVOICE"
    WHERE "Name" = 'BKAV';

    /* =========================================================
       3. KIỂM TRA CÓ CẤU HÌNH APZ_OCTA HAY KHÔNG
       ========================================================= */

    SELECT COUNT(*)
    INTO v_Count
    FROM "APZ_OCTA"
    WHERE "ItemId" LIKE 'APZ_EI%'
      AND "ActionTypeEx" = 9
	  AND TRIM(IFNULL(CAST("HttpUri" AS NVARCHAR), '')) <> '';
	  
    IF :v_Count = 0 THEN
        O_Status := 'ERROR';
        O_Message := 'Không tìm thấy cấu hình hóa đơn điện tử cho HttpUri.';
        RETURN;
    END IF;

    /* =========================================================
       4. KIỂM TRA APZ_OCTA CÓ HttpUri KHỚP @EINVOICE KHÔNG
       ========================================================= */

    SELECT COUNT(*)
    INTO v_Count
    FROM "APZ_OCTA"
    WHERE "ItemId" LIKE 'APZ_EI%'
      AND "ActionTypeEx" = 9
      AND LOWER(TRIM(IFNULL(CAST("HttpUri" AS NVARCHAR), ''))) <> LOWER(TRIM(IFNULL(:v_EInvoiceHttpUri, '')));

    IF :v_Count > 0 THEN
        O_Status := 'ERROR';
        O_Message :=
            'Cấu hình HttpUri không khớp với HttpUri trong bảng thiết lập thông số hóa đơn điện tử BKAV. ' ||
            'Không thể thực hiện tích hợp BKAV.';
        RETURN;
    END IF;

    /* =========================================================
       5. TẤT CẢ ĐỀU OK
       ========================================================= */

    O_Status  := 'OK';
    O_Message := 'OK - Cấu hình BKAV và thông số kết nối hợp lệ.';

END;