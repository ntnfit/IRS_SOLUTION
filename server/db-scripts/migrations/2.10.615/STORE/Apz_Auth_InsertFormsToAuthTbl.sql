
CALL APZ_EXISTSPROC('Apz_Auth_InsertFormsToAuthTbl');

GO

Create PROCEDURE Apz_Auth_InsertFormsToAuthTbl
AS
BEGIN 
	-- Temp tbl = "#APZ_AUTH"

	-- kiểm tra bảng tạm đã khởi tạo chưa
	DECLARE checkCount INT;
	DECLARE maxid INT;
	create local temporary table #temAuth as (select top 0 * from APZ_OMNI);
	exec 'insert into #temAuth select * from #APZ_AUTH';
	SELECT COUNT(1) into checkCount FROM #temAuth;
	IF ifnull(:checkCount,0) = 0
		then
			SELECT 100000, ' Temp data is not exist' from DUMMY;
			RETURN;
		END if;

	DELETE from APZ_OMNI;
    INSERT INTO APZ_OMNI SELECT * FROM #temAuth;
    
    -------------- lấy dữ liệu ID lớn nhất để gán cho các row tiếp theo --------------------
    select IFNULL((SELECT MAX("Id") FROM APZ_OMNI),0) into maxid from DUMMY;
    
    ------------------- INSERT thêm formsetting để có thể phân quyền ---------------------------
	checkCount = 0;
	SELECT count(1) into checkCount FROM apz_OMNI WHERE "Code" ='frm_FormSetting';
    IF ifnull(:checkCount,0) = 0
	THEN
		INSERT INTO APZ_OMNI
		(
		    "Id" ,
	        "FatherId" ,
	        "Code" ,
	        "Name" ,
	        "LanguageKey" ,
	        "MenuType"
		) 
		SELECT :maxid+1,IFNULL((SELECT "Id" FROM (select "Id", row_number() over(order by "Id") RowNum from apz_omni WHERE "Code"='barSub_Administration') x where RowNum = 1),1),'frm_FormSetting','Form Setting','FormSetting','L' from DUMMY;
		maxid = :maxid + 1;
	END IF;
	------------------------ END insert formsetting -----------------------------------------------

	--------------------------------- INSERT thêm form report form setting ------------------------------------
	checkCount = 0;
	SELECT count(1) into checkCount FROM apz_OMNI WHERE "Code" ='frm_ReportFormSettings';
	IF  ifnull(:checkCount,0) = 0
	THEN
		INSERT INTO APZ_OMNI
		(
		    "Id" ,
	        "FatherId" ,
	        "Code" ,
	        "Name" ,
	        "LanguageKey" ,
	        "MenuType"
		) 
		SELECT :maxid+1,IFNULL((SELECT "Id" FROM (select "Id", row_number() over(order by "Id") RowNum from apz_omni WHERE "Code"='barSub_Administration') x where RowNum = 1),1),'frm_ReportFormSettings','Report Form Setting','Report FormSetting','L' from DUMMY;
		maxid = :maxid+1;
	end IF;
	-------------------------------- END insert report form setting------------------------------------------------

	-- update lại father id của user report
	UPDATE APZ_OMNI SET "FatherId" = (SELECT y."Id" FROM APZ_OUDR x INNER JOIN APZ_OMNI y ON x."FatherMenu"=y."Code" WHERE 'frm_' || x."Code"=APZ_OMNI."Code")
	WHERE "MenuType"='C';
	select IFNULL((SELECT MAX("Id") FROM APZ_OMNI),0) into maxid from DUMMY;
	INSERT INTO APZ_OMNI
	        ( "Id" ,
	          "FatherId" ,
	          "Code" ,
	          "Name" ,
	          "LanguageKey" ,
	          "MenuType"
	        )
	SELECT :maxid + ROW_NUMBER() OVER (ORDER BY T0."FormId"),T1."Id",T0."AbsEntry",T0."Caption",T0."FormId",'W'
		FROM APZ_OFWZ T0
		INNER JOIN APZ_OMNI T1 ON T0."FatherMenu"=T1."Code";
		
	drop table #temAuth;

END;