
CREATE PROCEDURE Util_AdvanceReportTemplate_GetDataReportType
AS
BEGIN
	create local temporary table #tblStructure
            (
             "Name" NVARCHAR(100) NOT NULL
            ,"Code" CHAR(1) NULL
            );
        INSERT INTO #tblStructure
						("Name",				"Code")
                select N'Type 1',			'1' from DUMMY
                	UNION ALL
				select N'Type 2',			'2' from dummy;
					 
	SELECT * FROM #tblStructure;
END



