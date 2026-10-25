CREATE PROCEDURE "SP_CHECK_EINVOICE_CONFIG"
(
    OUT O_Status       NVARCHAR(20),
    OUT O_Message      NVARCHAR(200)
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
BEGIN

    DECLARE v_Count        INT;

    DECLARE v_PartnerGUID  NVARCHAR(500);
    DECLARE v_PartnerToken NVARCHAR(500);
    DECLARE v_HttpUri      NVARCHAR(500);
    DECLARE v_LinkViewInv  NVARCHAR(500);

    DECLARE v_Message      NVARCHAR(200);
    DECLARE v_Sep          NVARCHAR(10);

    v_Message := '';
    v_Sep := '';

    /* ============================================================
       1. @EINVOICE PHẢI CÓ ĐÚNG 1 DÒNG DUY NHẤT
       ============================================================ */

    SELECT COUNT(*)
    INTO v_Count
    FROM "@EINVOICE";

    /* ------------------------------------------------------------
       Không có cấu hình
       ------------------------------------------------------------ */
    IF :v_Count = 0 THEN
        O_Status  := 'ERROR';
        O_Message :=
            'Bảng thiết lập thông số HĐĐT chưa có cấu hình. ' ||
            'Yêu cầu phải có đúng 1 cấu hình BKAV.';
        RETURN;
    END IF;

    /* ------------------------------------------------------------
       Có từ 2 dòng trở lên
       ------------------------------------------------------------ */
    IF :v_Count > 1 THEN
        O_Status  := 'ERROR';
        O_Message :=
            'Bảng thiết lập thông số HĐĐT đang có ' ||
            TO_NVARCHAR(:v_Count) ||
            ' dòng. Chỉ được phép có đúng 1 cấu hình duy nhất.';
        RETURN;
    END IF;
    
    /* ============================================================
       2. KIỂM TRA NAME = BKAV
       ============================================================ */
	IF EXISTS
	(
	    SELECT 1
	    FROM "@EINVOICE"
	    WHERE IFNULL(TRIM("Name"), '') = ''
	)
	THEN
	    O_Status := 'ERROR';
	    O_Message :=
	        'Bảng thiết lập thông số HĐĐT không hợp lệ: ' ||
	        'Name không được để trống.';
	    RETURN;
	END IF;

    IF EXISTS
    (
        SELECT 1
        FROM "@EINVOICE"
        WHERE "Name" <> 'BKAV'
    )
    THEN
        O_Status  := 'ERROR';
        O_Message :=
            'Bảng thiết lập thông số HĐĐT không hợp lệ: ' ||
            'Yêu cầu Name = BKAV';
        RETURN;
    END IF;

    /* ============================================================
       3. LẤY CẤU HÌNH DUY NHẤT VÀO BIẾN
       ============================================================ */

    SELECT TOP 1
        "U_PartnerGUID",
        "U_PartnerToken",
        "U_HttpUri",
        "U_LinkViewInv"
    INTO
        v_PartnerGUID,
        v_PartnerToken,
        v_HttpUri,
        v_LinkViewInv
    DEFAULT '', '', '', ''
    FROM "@EINVOICE";
    
    /* ============================================================
       4. DATABASE PRODUCTION
          SBO_ILD_GOLIVE
       ============================================================ */

    IF CURRENT_SCHEMA = 'SBO_ILD_GOLIVE' THEN

        /* ========================================================
           PartnerGUID
           ======================================================== */

        IF IFNULL(TRIM(:v_PartnerGUID), '') = '' THEN
            v_Message := :v_Message ||
                :v_Sep ||
                'PartnerGUID không được để trống';
            v_Sep := '; ';
        ELSEIF :v_PartnerGUID <> 'dd6c2b67-15cf-473f-b33a-50bd6c0efa27' THEN
            v_Message := :v_Message ||
                :v_Sep ||
                'PartnerGUID không đúng';
            v_Sep := '; ';
        END IF;

        /* ========================================================
           PartnerToken
           ======================================================== */
        IF IFNULL(TRIM(:v_PartnerToken), '') = '' THEN
            v_Message := :v_Message ||
                :v_Sep ||
                'PartnerToken không được để trống';
            v_Sep := '; ';
        ELSEIF :v_PartnerToken <> '7904YFvFQTVxfqw7nys+oPTaHTdw8rja8XOVMzVzFt0=:5FezXcC8VgbzijujNAAmrg==' THEN
            v_Message := :v_Message ||
                :v_Sep ||
                'PartnerToken không đúng';
            v_Sep := '; ';
        END IF;

        /* ========================================================
           HttpUri
           ======================================================== */

        IF IFNULL(TRIM(:v_HttpUri), '') = '' THEN
            v_Message := :v_Message ||
                :v_Sep ||
                'HttpUri không được để trống';
            v_Sep := '; ';
        ELSEIF LOWER(TRIM(:v_HttpUri)) <> 'https://ws.ehoadon.vn/wspublicehoadon.asmx/execcommand' THEN
            v_Message := :v_Message ||
                :v_Sep ||
                'HttpUri không đúng';
            v_Sep := '; ';
        END IF;

        /* ========================================================
           LinkViewInv
           ======================================================== */

        IF IFNULL(TRIM(:v_LinkViewInv), '') = '' THEN
            v_Message := :v_Message ||
                :v_Sep ||
                'LinkViewInv không được để trống';
            v_Sep := '; ';
        ELSEIF LOWER(TRIM(:v_LinkViewInv)) <> 'https://van.ehoadon.vn/tchd?mtc=' THEN
            v_Message := :v_Message ||
                :v_Sep ||
                'LinkViewInv không đúng';
            v_Sep := '; ';
        END IF;

        /* ========================================================
           KẾT QUẢ PRODUCTION
           ======================================================== */

        IF :v_Message <> ''
        THEN
            O_Status := 'ERROR';
            O_Message :=
                'Bảng thiết lập thông số HĐĐT BKAV PRODUCTION không hợp lệ: ' ||
                :v_Message;
            RETURN;
        END IF;
		
        O_Status  := 'OK';
        O_Message := 'OK - BKAV PRODUCTION';

        RETURN;

	ELSE
	
	    /* ============================================================
       	   5. DATABASE DEMO
           TẤT CẢ DATABASE KHÁC
       	   ============================================================ */
	
	    /* ============================================================
       	   PartnerGUID
       	   ============================================================ */
	
	    IF IFNULL(TRIM(:v_PartnerGUID), '') = '' THEN
	        v_Message := :v_Message ||
	            :v_Sep ||
	            'PartnerGUID không được để trống';
	        v_Sep := '; ';
	    ELSEIF :v_PartnerGUID <> '8225b745-a89e-4945-8329-eebbe264b1c5' THEN
	        v_Message := :v_Message ||
	            :v_Sep ||
	            'PartnerGUID không đúng';
	        v_Sep := '; ';
	    END IF;
	
	    /* ============================================================
       	   PartnerToken
       	   ============================================================ */
	
	    IF IFNULL(TRIM(:v_PartnerToken), '') = '' THEN
	        v_Message := :v_Message ||
	            :v_Sep ||
	            'PartnerToken không được để trống';
	        v_Sep := '; ';
	    ELSEIF :v_PartnerToken <> 'CC9URjGYXrZexiVoWqC3jNMQQ8XSv5KH+rvMSzWT0v8=:ydmOduHPBIkKbvE+FhnLbg==' THEN
	        v_Message := :v_Message ||
	            :v_Sep ||
	            'PartnerToken không đúng';
	        v_Sep := '; ';
	    END IF;
	
	    /* ============================================================
       	   HttpUri
       	   ============================================================ */
	
	    IF IFNULL(TRIM(:v_HttpUri), '') = '' THEN
	        v_Message := :v_Message ||
	            :v_Sep ||
	            'HttpUri không được để trống';
	        v_Sep := '; ';
	    ELSEIF LOWER(TRIM(:v_HttpUri)) <> 'https://wsdemo.ehoadon.vn/wspublicehoadon.asmx/execcommand' THEN
	        v_Message := :v_Message ||
	            :v_Sep ||
	            'HttpUri không đúng';
	        v_Sep := '; ';
	    END IF;
	
	    /* ============================================================
       	   LinkViewInv
       	   ============================================================ */
	
	    IF IFNULL(TRIM(:v_LinkViewInv), '') = '' THEN
	        v_Message := :v_Message ||
	            :v_Sep ||
	            'LinkViewInv không được để trống';
	        v_Sep := '; ';
	    ELSEIF LOWER(TRIM(:v_LinkViewInv)) <> 'https://demo.ehoadon.vn/tchd?mtc=' THEN
	        v_Message := :v_Message ||
	            :v_Sep ||
	            'LinkViewInv không đúng';
	        v_Sep := '; ';
	    END IF;
	
	    /* ============================================================
       	   KẾT QUẢ DEMO
       	   ============================================================ */
	
	    IF :v_Message <> '' THEN
	        O_Status := 'ERROR';
	        O_Message :=
	            'Bảng thiết lập thông số HĐĐT BKAV DEMO không hợp lệ: ' ||
	            :v_Message;
	        RETURN;
	    END IF;
		
	    O_Status  := 'OK';
	    O_Message := 'OK - BKAV DEMO';
	    RETURN;
	END IF;
END;