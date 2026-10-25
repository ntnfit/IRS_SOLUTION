USE [SBODemoAU];
GO

CREATE OR ALTER PROCEDURE dbo.APZ_PromotionCondition_GetLines @docEntry int
AS
BEGIN
 SET NOCOUNT ON;
 DECLARE @structure TABLE(ID int IDENTITY,ColumnName nvarchar(50),Caption nvarchar(100),DataType varchar(50),SubType varchar(100),Width int,GroupIndex int,Visible tinyint,FormatType nvarchar(20),DisplayFormat nvarchar(20),ReadOnly tinyint,AllowEdit tinyint,AllowMove tinyint,AllowSize tinyint,AllowSort tinyint,Fixed varchar(1),BackColor nvarchar(20),ForeColor nvarchar(20),SummaryType varchar(10),GroupSummaryType varchar(10),GroupSummaryShowInFooter tinyint,SummaryFormat nvarchar(50),Expression nvarchar(4000),Parent tinyint,[Key] tinyint,PivotArea nvarchar(20));
 INSERT @structure(ColumnName,Caption,DataType,SubType,Width,GroupIndex,Visible,ReadOnly,AllowEdit,AllowMove,AllowSize,AllowSort,Fixed,BackColor) VALUES
 (N'DocEntry',N'DocEntry','int','',75,-1,0,1,1,1,1,0,'L','silver'),(N'Code',N'Code','nvarchar','',100,-1,0,1,1,1,1,0,'',NULL),
 (N'LineId',N'LineId','int','',100,-1,0,1,1,1,1,0,'',NULL),(N'TypeCode',N'Code','nvarchar','',100,-1,1,1,1,1,1,0,'',NULL),
 (N'TypeName',N'Name','nvarchar','',100,-1,1,0,1,1,1,0,'',NULL),(N'Rate',N'Rate','numeric','',75,-1,1,0,1,1,1,0,'',NULL),
 (N'Remark',N'Remark','nvarchar','',200,-1,1,0,1,1,1,0,'',NULL);
 SELECT * FROM @structure ORDER BY ID;
 SELECT * FROM dbo.APZ_PCN1 WHERE DocEntry=@docEntry ORDER BY LineId;
 SELECT CAST(NULL AS nvarchar(50)) ColumnName,CAST(NULL AS nvarchar(254)) Value,CAST(NULL AS nvarchar(254)) Description,CAST(NULL AS nvarchar(20)) BackColor,CAST(NULL AS nvarchar(20)) ForeColor WHERE 1=0;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_PromotionCondition_SearchPromotionCondition
 @code nvarchar(50),@fromDate datetime=NULL,@toDate datetime=NULL,@active nvarchar(20)=N''
AS
BEGIN
 SET NOCOUNT ON;
 DECLARE @structure TABLE(ID int IDENTITY,ColumnName nvarchar(50),Caption nvarchar(100),DataType varchar(50),SubType varchar(100),Width int,GroupIndex int,Visible smallint,FormatType nvarchar(20),DisplayFormat nvarchar(20),ReadOnly smallint,AllowEdit smallint,AllowMove smallint,AllowSize smallint,AllowSort smallint,Fixed varchar(1),BackColor nvarchar(20),ForeColor nvarchar(20),SummaryType varchar(10),GroupSummaryType varchar(10),GroupSummaryShowInFooter smallint,SummaryFormat nvarchar(50),Expression nvarchar(4000),Parent smallint,[Key] smallint,PivotArea nvarchar(20));
 INSERT @structure(ColumnName,Caption,DataType,SubType,Width,GroupIndex,Visible,FormatType,DisplayFormat,ReadOnly,AllowEdit,AllowMove,AllowSize,AllowSort,Fixed,BackColor) VALUES
 (N'DocEntry',N'DocEntry','int','',75,-1,0,NULL,NULL,1,1,1,1,0,'L','silver'),(N'Code',N'Code','nvarchar','',75,-1,1,NULL,NULL,1,1,1,1,0,'',NULL),
 (N'Name',N'Name','nvarchar','',200,-1,1,NULL,NULL,0,1,1,1,0,'',NULL),(N'Type',N'Type','ValidValue','',100,-1,1,NULL,NULL,0,1,1,1,0,'',NULL),
 (N'ValueMin',N'ValueMin','numeric','',75,-1,1,NULL,NULL,0,1,1,1,0,'',NULL),(N'ValueMax',N'ValueMax','numeric','',75,-1,1,NULL,NULL,0,1,1,1,0,'',NULL),
 (N'Active',N'Active','ValidValue','',75,-1,1,'','',0,1,1,1,0,'',NULL),(N'Remark',N'Remark','nvarchar','',150,-1,1,'','',0,1,1,1,0,'',NULL),
 (N'UserSign',N'UserSign','nvarchar','',75,-1,0,'','',0,1,1,1,0,'',NULL),(N'CreateDate',N'CreateDate','datetime','',75,-1,0,'','',0,1,1,1,0,'',NULL),
 (N'UserSign2',N'UserSign2','nvarchar','',75,-1,0,'','',0,1,1,1,0,'',NULL),(N'UpdateDate',N'UpdateDate','datetime','',75,-1,0,'','',0,1,1,1,0,'',NULL);
 SELECT * FROM @structure ORDER BY ID;
 SELECT * FROM dbo.APZ_OPCN WHERE (ISNULL(@code,N'')=N'' OR Code=@code) AND (@fromDate IS NULL OR CreateDate BETWEEN @fromDate AND ISNULL(@toDate,@fromDate)) AND (ISNULL(@active,N'')=N'' OR Active=@active) ORDER BY DocEntry DESC;
 DECLARE @valid TABLE(ColumnName nvarchar(50),Value nvarchar(254),Description nvarchar(254),BackColor nvarchar(20),ForeColor nvarchar(20));
 INSERT @valid(ColumnName,Value,Description) VALUES (N'Type',N'1',N'Khách hàng'),(N'Type',N'2',N'Nhóm khách hàng'),(N'Type',N'3',N'Thứ tự khách hàng'),(N'Type',N'4',N'Số lượng/mặt hàng'),(N'Type',N'5',N'Số lượng/nhóm hàng'),(N'Type',N'6',N'Doanh thu/mặt hàng'),(N'Type',N'7',N'Doanh thu/nhóm mặt hàng'),(N'Type',N'8',N'Tuyến'),(N'Type',N'9',N'Kênh'),(N'Active',N'Y',N'Yes'),(N'Active',N'N',N'No');
 SELECT * FROM @valid;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Apz_PromotionCondition_SaveProcess @type char(1)=''
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 IF OBJECT_ID('tempdb..#APZ_OPCN') IS NULL BEGIN SELECT 404 ErrorCode,N'Temp data not exists' ErrorMessage; RETURN; END;
 DECLARE @id int=0,@code nvarchar(50)=ISNULL((SELECT TOP(1) Code FROM #APZ_OPCN),N'');
 IF @code=N'' BEGIN SELECT 405 ErrorCode,N'Code cannot be empty' ErrorMessage; RETURN; END;
 IF @type='D' BEGIN DELETE FROM dbo.APZ_PCN1 WHERE Code=@code; DELETE FROM dbo.APZ_OPCN WHERE Code=@code; SELECT 0 ErrorCode,N'' ErrorMessage; RETURN; END;
 IF @type='A' BEGIN SELECT @id=ISNULL(MAX(DocEntry),0)+1 FROM dbo.APZ_OPCN WITH(UPDLOCK,HOLDLOCK); UPDATE #APZ_OPCN SET UpdateDate=NULL,CreateDate=CONVERT(date,GETDATE()),DocEntry=@id; END
 ELSE BEGIN SELECT TOP(1) @id=DocEntry FROM #APZ_OPCN; DELETE FROM dbo.APZ_PCN1 WHERE DocEntry=@id; DELETE FROM dbo.APZ_OPCN WHERE DocEntry=@id; UPDATE #APZ_OPCN SET UpdateDate=CONVERT(date,GETDATE()); END;
 IF EXISTS(SELECT 1 FROM dbo.APZ_OPCN t JOIN #APZ_OPCN s ON s.Code=t.Code) BEGIN SELECT 123 ErrorCode,N'Object is overlapping with an existing object' ErrorMessage; RETURN; END;
 INSERT dbo.APZ_OPCN SELECT * FROM #APZ_OPCN;
 IF OBJECT_ID('tempdb..#APZ_PCN1') IS NOT NULL
 BEGIN
   UPDATE #APZ_PCN1 SET DocEntry=@id,Code=@code;
   ;WITH numbered AS(SELECT *,ROW_NUMBER() OVER(ORDER BY(SELECT NULL)) NewLine FROM #APZ_PCN1) UPDATE numbered SET LineId=NewLine;
   INSERT dbo.APZ_PCN1 SELECT * FROM #APZ_PCN1;
 END;
 SELECT 0 ErrorCode,N'' ErrorMessage;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_PromotionSetup_Save
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 DELETE FROM #APZ_D_OPRC WHERE ISNULL(LTRIM(Name),N'')=N'' AND ISNULL(LTRIM(CONVERT(nvarchar(max),Descr)),N'')=N'';
 DELETE FROM #APZ_D_OPRT WHERE ISNULL(LTRIM(Name),N'')=N'' AND ISNULL(LTRIM(CONVERT(nvarchar(max),Descr)),N'')=N'';
 DELETE FROM #APZ_D_OPRG WHERE ISNULL(LTRIM(Name),N'')=N'' AND ISNULL(LTRIM(CONVERT(nvarchar(max),Descr)),N'')=N'';
 IF EXISTS(SELECT 1 FROM #APZ_D_OPRC WHERE ISNULL(LTRIM(Name),N'')=N'') BEGIN SELECT 1 ErrorCode,N'Invalid name' ErrorMessage; RETURN; END;
 IF EXISTS(SELECT LOWER(Name) FROM #APZ_D_OPRC GROUP BY LOWER(Name) HAVING COUNT(*)>1) BEGIN SELECT 1 ErrorCode,N'Duplicate name' ErrorMessage; RETURN; END;
 IF EXISTS(SELECT 1 FROM #APZ_D_OPRT WHERE ISNULL(LTRIM(Name),N'')=N'') BEGIN SELECT 2 ErrorCode,N'Invalid name' ErrorMessage; RETURN; END;
 IF EXISTS(SELECT LOWER(Name) FROM #APZ_D_OPRT GROUP BY LOWER(Name) HAVING COUNT(*)>1) BEGIN SELECT 2 ErrorCode,N'Duplicate name' ErrorMessage; RETURN; END;
 IF EXISTS(SELECT 1 FROM #APZ_D_OPRG WHERE ISNULL(LTRIM(Name),N'')=N'') BEGIN SELECT 3 ErrorCode,N'Invalid name' ErrorMessage; RETURN; END;
 IF EXISTS(SELECT LOWER(Name) FROM #APZ_D_OPRG GROUP BY LOWER(Name) HAVING COUNT(*)>1) BEGIN SELECT 3 ErrorCode,N'Duplicate name' ErrorMessage; RETURN; END;
 DECLARE @base int;
 SELECT @base=ISNULL(MAX(ID),0) FROM #APZ_D_OPRC; WITH rows AS(SELECT *,ROW_NUMBER() OVER(ORDER BY(SELECT NULL)) rn FROM #APZ_D_OPRC WHERE ISNULL(ID,0)=0) UPDATE rows SET ID=@base+rn;
 SELECT @base=ISNULL(MAX(ID),0) FROM #APZ_D_OPRT; WITH rows AS(SELECT *,ROW_NUMBER() OVER(ORDER BY(SELECT NULL)) rn FROM #APZ_D_OPRT WHERE ISNULL(ID,0)=0) UPDATE rows SET ID=@base+rn;
 SELECT @base=ISNULL(MAX(ID),0) FROM #APZ_D_OPRG; WITH rows AS(SELECT *,ROW_NUMBER() OVER(ORDER BY(SELECT NULL)) rn FROM #APZ_D_OPRG WHERE ISNULL(ID,0)=0) UPDATE rows SET ID=@base+rn;
 DELETE FROM dbo.APZ_D_OPRC; INSERT dbo.APZ_D_OPRC(ID,Name,Descr) SELECT ID,Name,Descr FROM #APZ_D_OPRC;
 DELETE FROM dbo.APZ_D_OPRT; INSERT dbo.APZ_D_OPRT(ID,Name,Descr) SELECT ID,Name,Descr FROM #APZ_D_OPRT;
 DELETE FROM dbo.APZ_D_OPRG; INSERT dbo.APZ_D_OPRG(ID,Name,Descr) SELECT ID,Name,Descr FROM #APZ_D_OPRG;
 SELECT 0 ErrorCode,N'' ErrorMessage;
END;
GO
