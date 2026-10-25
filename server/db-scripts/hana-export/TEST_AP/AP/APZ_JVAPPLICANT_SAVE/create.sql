
CREATE PROCEDURE APZ_JVApplicant_Save
AS
BEGIN
	DECLARE Id int=0;
	DECLARE i INTEGER = 1;
	DECLARE pos INTEGER = 0;
	DECLARE QuaCode Nvarchar(50) = '';
	
	create local temporary table #dtAPZ_OHAPLocal as (select top 0 * from APZ_OHAP);
	exec 'drop table #dtAPZ_OHAPLocal;';
	exec 'create local temporary table #dtAPZ_OHAPLocal as (select * from "#dtAPZ_OHAP");';
	
	--Get MaxID Applicant
	select ifnull((select max("ApplicantID") from APZ_OHAP),0) into Id from DUMMY;  	
	
	delete from APZ_OHAP;
	insert into APZ_OHAP select * from #dtAPZ_OHAPLocal;
	select :Id + 1 into Id from DUMMY;

    Delete from APZ_OHAQ;
    create local temporary table #dtAPZ_OHAPLocalIns as (select ROW_NUMBER() OVER () As "RowNumber",* from APZ_OHAP);
   	Select Count(1) Into pos From APZ_OHAP;
	WHILE :i <= :pos DO
		If Exists(Select 1 From #dtAPZ_OHAPLocalIns Where "RowNumber" = :i And IFNULL("Qualifications",'') LIKE '%;%' ) Then
			Select "Qualifications" Into QuaCode From #dtAPZ_OHAPLocalIns Where "RowNumber" = :i And IFNULL("Qualifications",'') LIKE '%;%';
			Insert Into APZ_OHAQ Select :id,Data From Split(:QuaCode, ';'); 
		Else
			Insert Into APZ_OHAQ select "ApplicantID","Qualifications" from #dtAPZ_OHAPLocalIns Where "RowNumber" = :i;
		End If;
		i := :i + 1;
	END WHILE;
END;


