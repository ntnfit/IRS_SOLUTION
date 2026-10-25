 

CREATE PROCEDURE apz_d_promotion_process
(
	UserCode nvarchar(50),
	OldCode nvarchar(50),
	transtype char(1) DEFAULT 'A'
)
AS
BEGIN

	DECLARE error INT = 0;
	DECLARE error_message nvarchar(254);
	DECLARE Code nvarchar(40);
	DECLARE UserID INT = IFNULL((SELECT USERID FROM OUSR WHERE USER_CODE = :UserCode), 0);
	
	create local temporary table #OPMOLocal as (select top 0 * from APZ_D_OPMO);
	exec 'drop table #OPMOLocal';
	exec 'create local temporary table #OPMOLocal as (select * from #APZ_D_OPMO)';
	
	create local temporary table #PMO1Local as (select top 0 * from APZ_D_PMO1);
	exec 'drop table #PMO1Local';
	exec 'create local temporary table #PMO1Local as (select * from #APZ_D_PMO1)';
	
	create local temporary table #PMO2Local as (select top 0 * from APZ_D_PMO2);
	exec 'drop table #PMO2Local';
	exec 'create local temporary table #PMO2Local as (select * from #APZ_D_PMO2)';
	
	
	IF :UserID = 0 THEN 
		error := :error + 1;
		error_message := n'Người dùng không tồn tại';
		SELECT :error, :error_message FROM DUMMY;
		RETURN;
	END IF;

	IF :transtype = 'A' THEN 
		IF (EXISTS (SELECT 1 FROM APZ_D_OPMO WHERE "Code" IN (SELECT "Code" FROM #OPMOLocal))) THEN 
			error := :error + 2;
			error_message := n'Mã đã tồn tại';
			SELECT :error, :error_message FROM DUMMY;
			RETURN;
		END IF;
	END IF;

	Code = ifnull((select "Code" from #OPMOLocal),'');
	
	IF (:transtype = 'U') THEN
		IF (EXISTS (SELECT 1 FROM APZ_D_OPMO WHERE "Code" IN (SELECT "Code" FROM #OPMOLocal) AND "Code" <> :OldCode)) THEN 
			error := :error + 3;
			error_message := n'Mã đã tồn tại';
			SELECT :error, :error_message FROM DUMMY;
			RETURN;
		END IF;
		
		IF (:Code <> :OldCode AND EXISTS (SELECT 1 FROM APZ_D_PMO3 WHERE "Code" = :OldCode)) THEN 
			error := :error + 4;
			error_message := n'Không thể thay đổi mã CTKM đang được sử dụng';
			SELECT :error, :error_message FROM DUMMY;
			RETURN;
		END IF;
	END IF;

	
	IF :Code = '' THEN 
		error := :error + 5;
		error_message := n'Mã không được để trống';
		SELECT :error, :error_message FROM DUMMY;
		RETURN;
	END IF;

	UPDATE #PMO1Local SET "Code" = :Code;
	UPDATE #PMO2Local SET "Code" = :Code;
	DELETE FROM APZ_D_OPMO WHERE "Code" = :OldCode;
	DELETE FROM APZ_D_PMO1 WHERE "Code" = :OldCode;
	DELETE FROM APZ_D_PMO2 WHERE "Code" = :OldCode;
	INSERT INTO APZ_D_OPMO (SELECT * FROM #OPMOLocal);
	INSERT INTO APZ_D_PMO1 (SELECT * FROM #PMO1Local);
	INSERT INTO APZ_D_PMO2 (SELECT * FROM #PMO2Local WHERE ifnull("BenefitCode",'')<>'' and "GroupCode" in (SELECT DISTINCT "GroupCode" FROM #PMO1Local));
	
	SELECT :error, :error_message FROM DUMMY;

END




